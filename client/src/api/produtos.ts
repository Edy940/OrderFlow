import type { AuthFetch } from './httpClient'
import type { Produto } from '../types/api'

export function listar(authFetch: AuthFetch) {
  return authFetch<Produto[]>('/produtos')
}

export function criar(authFetch: AuthFetch, nome: string, preco: number, estoque: number) {
  return authFetch<Produto>('/produtos', { method: 'POST', body: { nome, preco, estoque } })
}
