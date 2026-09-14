import { useEffect, useState } from 'react'
import { useAuth } from '../auth/AuthContext'

function formatarContagem(ms: number) {
  if (ms <= 0) return '0:00'
  const totalSegundos = Math.floor(ms / 1000)
  const minutos = Math.floor(totalSegundos / 60)
  const segundos = totalSegundos % 60
  return `${minutos}:${segundos.toString().padStart(2, '0')}`
}

export function SessionPanel() {
  const { session, refreshLog } = useAuth()
  const [agora, setAgora] = useState(Date.now())

  useEffect(() => {
    const id = window.setInterval(() => setAgora(Date.now()), 1000)
    return () => window.clearInterval(id)
  }, [])

  if (!session) return null

  const msRestantes = new Date(session.accessTokenExpiraEm).getTime() - agora

  return (
    <aside className="w-full max-w-sm shrink-0 space-y-4 rounded-xl border border-slate-200 bg-white p-4 text-sm">
      <div>
        <h2 className="font-semibold text-slate-900">Sessão</h2>
        <p className="text-slate-500">Painel de depuração — pensado pro estudo de AppSec.</p>
      </div>

      <div>
        <p className="text-slate-500">Access token expira em</p>
        <p className={`font-mono text-lg ${msRestantes < 60_000 ? 'text-amber-600' : 'text-slate-900'}`}>
          {formatarContagem(msRestantes)}
        </p>
      </div>

      <div>
        <p className="mb-1 text-slate-500">Claims do JWT (decodificadas no cliente, não validadas)</p>
        <dl className="grid grid-cols-[auto_1fr] gap-x-2 gap-y-1 font-mono text-xs text-slate-700">
          <dt className="text-slate-400">sub</dt>
          <dd className="truncate">{session.claims.sub}</dd>
          <dt className="text-slate-400">email</dt>
          <dd className="truncate">{session.claims.email}</dd>
          <dt className="text-slate-400">name</dt>
          <dd className="truncate">{session.claims.name}</dd>
          <dt className="text-slate-400">role</dt>
          <dd className="truncate">{session.claims.role}</dd>
          <dt className="text-slate-400">jti</dt>
          <dd className="truncate">{session.claims.jti}</dd>
        </dl>
      </div>

      <div>
        <p className="mb-1 text-slate-500">Histórico</p>
        <ul className="space-y-0.5 text-xs text-slate-600">
          {refreshLog.length === 0 && <li className="text-slate-400">Nenhum evento ainda.</li>}
          {refreshLog.map((linha) => (
            <li key={linha}>{linha}</li>
          ))}
        </ul>
      </div>

      <div className="rounded-lg bg-slate-50 p-3 text-xs text-slate-500">
        <p className="font-medium text-slate-600">Nota de segurança</p>
        <p>
          Os tokens ficam só em memória (nunca em localStorage/sessionStorage) — abra o
          DevTools → Application → Local Storage e confirme que está vazio. Isso reduz o
          impacto de um XSS, mas significa que dar F5 nesta página encerra a sessão.
        </p>
      </div>
    </aside>
  )
}
