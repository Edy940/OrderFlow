import { request } from './httpClient'
import type { TokenResponse } from '../types/api'

export function registrar(nome: string, email: string, senha: string) {
  return request<TokenResponse>('/auth/registrar', {
    method: 'POST',
    body: { nome, email, senha },
  })
}

export function login(email: string, senha: string) {
  return request<TokenResponse>('/auth/login', {
    method: 'POST',
    body: { email, senha },
  })
}

export function renovar(refreshToken: string) {
  return request<TokenResponse>('/auth/refresh', {
    method: 'POST',
    body: { refreshToken },
  })
}

export function revogar(refreshToken: string) {
  return request<void>('/auth/revogar', {
    method: 'POST',
    body: { refreshToken },
  })
}
