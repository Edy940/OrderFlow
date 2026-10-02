import type { AuthFetch } from './httpClient'
import type { Cliente } from '../types/api'

export function listar(authFetch: AuthFetch) {
  return authFetch<Cliente[]>('/clientes')
}

export function criar(authFetch: AuthFetch, nome: string, email: string) {
  return authFetch<Cliente>('/clientes', { method: 'POST', body: { nome, email } })
}
