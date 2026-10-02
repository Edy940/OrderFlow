import { useEffect, useState, type FormEvent } from 'react'
import * as clientesApi from '../api/clientes'
import { ApiError } from '../api/httpClient'
import { useAuth } from '../auth/AuthContext'
import { ErrorBanner } from '../components/ErrorBanner'
import type { Cliente } from '../types/api'

export function ClientesPage() {
  const { authFetch } = useAuth()
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [erro, setErro] = useState<string | null>(null)
  const [carregando, setCarregando] = useState(false)

  async function carregar() {
    const dados = await clientesApi.listar(authFetch)
    setClientes(dados)
  }

  useEffect(() => {
    carregar().catch(() => setErro('Não foi possível carregar os clientes.'))
  }, [])

  async function handleSubmit(evento: FormEvent) {
    evento.preventDefault()
    setErro(null)
    setCarregando(true)
    try {
      await clientesApi.criar(authFetch, nome, email)
      setNome('')
      setEmail('')
      await carregar()
    } catch (erroCapturado) {
      setErro(erroCapturado instanceof ApiError ? erroCapturado.message : 'Não foi possível criar o cliente.')
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 font-semibold text-slate-900">Novo cliente</h2>
        <form onSubmit={handleSubmit} className="flex flex-wrap items-end gap-3">
          <div>
            <label className="block text-xs font-medium text-slate-600">Nome</label>
            <input
              required
              value={nome}
              onChange={(evento) => setNome(evento.target.value)}
              className="mt-1 rounded-lg border border-slate-300 px-3 py-1.5 text-sm"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600">Email</label>
            <input
              type="email"
              required
              value={email}
              onChange={(evento) => setEmail(evento.target.value)}
              className="mt-1 rounded-lg border border-slate-300 px-3 py-1.5 text-sm"
            />
          </div>
          <button
            type="submit"
            disabled={carregando}
            className="rounded-lg bg-indigo-600 px-4 py-1.5 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
          >
            {carregando ? 'Salvando…' : 'Criar (autenticado)'}
          </button>
        </form>
        {erro && (
          <div className="mt-3">
            <ErrorBanner message={erro} />
          </div>
        )}
      </div>

      <div className="rounded-xl border border-slate-200 bg-white">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-slate-200 text-left text-slate-500">
              <th className="px-4 py-2 font-medium">Nome</th>
              <th className="px-4 py-2 font-medium">Email</th>
            </tr>
          </thead>
          <tbody>
            {clientes.map((cliente) => (
              <tr key={cliente.id} className="border-b border-slate-100 last:border-0">
                <td className="px-4 py-2 text-slate-800">{cliente.nome}</td>
                <td className="px-4 py-2 text-slate-500">{cliente.email}</td>
              </tr>
            ))}
            {clientes.length === 0 && (
              <tr>
                <td colSpan={2} className="px-4 py-6 text-center text-slate-400">
                  Nenhum cliente ainda.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
