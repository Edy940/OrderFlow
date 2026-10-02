import { createContext, useContext, useEffect, useRef, useState, type ReactNode } from 'react'
import { jwtDecode } from 'jwt-decode'
import * as authApi from '../api/auth'
import { ApiError, request, type AuthFetch, type RequestOptions } from '../api/httpClient'
import type { JwtClaims, RawJwtPayload, TokenResponse } from '../types/api'

const RENOVAR_ANTES_MS = 60_000
const CLAIM_ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

interface Session {
  accessToken: string
  refreshToken: string
  accessTokenExpiraEm: string
  refreshTokenExpiraEm: string
  claims: JwtClaims
}

interface AuthContextValue {
  session: Session | null
  refreshLog: string[]
  login: (email: string, senha: string) => Promise<void>
  registrar: (nome: string, email: string, senha: string) => Promise<void>
  logout: () => Promise<void>
  authFetch: AuthFetch
}

const AuthContext = createContext<AuthContextValue | null>(null)

function montarSessao(tokens: TokenResponse): Session {
  const payload = jwtDecode<RawJwtPayload>(tokens.accessToken)
  const claims: JwtClaims = {
    sub: payload.sub,
    email: payload.email,
    name: payload.name,
    jti: payload.jti,
    exp: payload.exp,
    role: payload[CLAIM_ROLE],
  }

  return {
    accessToken: tokens.accessToken,
    refreshToken: tokens.refreshToken,
    accessTokenExpiraEm: tokens.accessTokenExpiraEm,
    refreshTokenExpiraEm: tokens.refreshTokenExpiraEm,
    claims,
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  // sessionRef é a fonte de verdade usada por authFetch/refresh (sempre atual, sem
  // closures obsoletas dentro do setTimeout). `session` no state só existe para a UI
  // re-renderizar (ex.: SessionPanel).
  const sessionRef = useRef<Session | null>(null)
  const refreshTimerRef = useRef<number | undefined>(undefined)
  const [session, setSession] = useState<Session | null>(null)
  const [refreshLog, setRefreshLog] = useState<string[]>([])

  useEffect(() => () => window.clearTimeout(refreshTimerRef.current), [])

  function registrarLog(mensagem: string) {
    const hora = new Date().toLocaleTimeString('pt-BR')
    setRefreshLog((prev) => [`${hora} — ${mensagem}`, ...prev].slice(0, 5))
  }

  function limparSessao() {
    sessionRef.current = null
    setSession(null)
    window.clearTimeout(refreshTimerRef.current)
  }

  function aplicarSessao(tokens: TokenResponse, mensagemLog: string) {
    const nova = montarSessao(tokens)
    sessionRef.current = nova
    setSession(nova)
    registrarLog(mensagemLog)

    window.clearTimeout(refreshTimerRef.current)
    const msRestantes = new Date(nova.accessTokenExpiraEm).getTime() - Date.now()
    const atraso = Math.max(msRestantes - RENOVAR_ANTES_MS, 0)
    refreshTimerRef.current = window.setTimeout(() => void doRefresh(), atraso)
  }

  async function doRefresh(): Promise<boolean> {
    const atual = sessionRef.current
    if (!atual) return false
    try {
      const tokens = await authApi.renovar(atual.refreshToken)
      aplicarSessao(tokens, 'Access token renovado automaticamente')
      return true
    } catch {
      limparSessao()
      registrarLog('Falha ao renovar — sessão encerrada')
      return false
    }
  }

  async function login(email: string, senha: string) {
    const tokens = await authApi.login(email, senha)
    aplicarSessao(tokens, 'Login realizado')
  }

  async function registrar(nome: string, email: string, senha: string) {
    const tokens = await authApi.registrar(nome, email, senha)
    aplicarSessao(tokens, 'Conta criada e login automático')
  }

  async function logout() {
    const atual = sessionRef.current
    limparSessao()
    registrarLog('Logout')
    if (atual) {
      await authApi.revogar(atual.refreshToken).catch(() => undefined)
    }
  }

  async function authFetch<T>(path: string, options: Omit<RequestOptions, 'accessToken'> = {}): Promise<T> {
    const atual = sessionRef.current
    try {
      return await request<T>(path, { ...options, accessToken: atual?.accessToken ?? null })
    } catch (erro) {
      if (erro instanceof ApiError && erro.status === 401 && atual) {
        const renovou = await doRefresh()
        if (renovou) {
          return await request<T>(path, { ...options, accessToken: sessionRef.current?.accessToken ?? null })
        }
      }
      throw erro
    }
  }

  return (
    <AuthContext.Provider value={{ session, refreshLog, login, registrar, logout, authFetch }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth precisa ser usado dentro de <AuthProvider>')
  return ctx
}
