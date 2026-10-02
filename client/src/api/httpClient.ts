const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5186/api/v1'

export class ApiError extends Error {
  status: number
  fieldErrors?: Record<string, string[]>

  constructor(message: string, status: number, fieldErrors?: Record<string, string[]>) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.fieldErrors = fieldErrors
  }
}

export interface RequestOptions {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  body?: unknown
  accessToken?: string | null
}

export type AuthFetch = <T>(path: string, options?: Omit<RequestOptions, 'accessToken'>) => Promise<T>

async function parseErrorMessage(response: Response): Promise<ApiError> {
  let mensagem = `Erro ${response.status} ao chamar a API.`
  let fieldErrors: Record<string, string[]> | undefined

  try {
    const data = await response.json()
    if (typeof data.mensagem === 'string') {
      mensagem = data.mensagem
    } else if (typeof data.detail === 'string') {
      mensagem = data.detail
    } else if (typeof data.title === 'string') {
      mensagem = data.title
    }
    if (data.errors && typeof data.errors === 'object') {
      fieldErrors = data.errors
    }
  } catch {
    // corpo vazio ou não-JSON (ex.: 204, 429 sem corpo) — mantém a mensagem genérica
  }

  return new ApiError(mensagem, response.status, fieldErrors)
}

export async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' }
  if (options.accessToken) {
    headers.Authorization = `Bearer ${options.accessToken}`
  }

  const response = await fetch(`${BASE_URL}${path}`, {
    method: options.method ?? 'GET',
    headers,
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
  })

  if (!response.ok) {
    throw await parseErrorMessage(response)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}
