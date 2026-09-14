import { useEffect, useState, type FormEvent } from 'react'
import { ApiError } from '../api/httpClient'
import * as produtosApi from '../api/produtos'
import { useAuth } from '../auth/AuthContext'
import { ErrorBanner } from '../components/ErrorBanner'
import type { Produto } from '../types/api'

export function ProdutosPage() {
  const { authFetch } = useAuth()
  const [produtos, setProdutos] = useState<Produto[]>([])
  const [nome, setNome] = useState('')
  const [preco, setPreco] = useState('')
  const [estoque, setEstoque] = useState('')
  const [erro, setErro] = useState<string | null>(null)
  const [carregando, setCarregando] = useState(false)

  async function carregar() {
    const dados = await produtosApi.listar(authFetch)
    setProdutos(dados)
  }

  useEffect(() => {
    carregar().catch(() => setErro('Não foi possível carregar os produtos.'))
  }, [])

  async function handleSubmit(evento: FormEvent) {
    evento.preventDefault()
    setErro(null)

    const precoNumero = Number(preco)
    const estoqueNumero = Number(estoque)
    if (estoqueNumero === 0) {
      setErro('O backend rejeita estoque igual a 0 — informe um valor diferente.')
      return
    }
    if (precoNumero > 10000) {
      setErro('O backend rejeita preço acima de 10.000.')
      return
    }

    setCarregando(true)
    try {
      await produtosApi.criar(authFetch, nome, precoNumero, estoqueNumero)
      setNome('')
      setPreco('')
      setEstoque('')
      await carregar()
    } catch (erroCapturado) {
      setErro(erroCapturado instanceof ApiError ? erroCapturado.message : 'Não foi possível criar o produto.')
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 font-semibold text-slate-900">Novo produto</h2>
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
            <label className="block text-xs font-medium text-slate-600">Preço</label>
            <input
              type="number"
              step="0.01"
              required
              value={preco}
              onChange={(evento) => setPreco(evento.target.value)}
              className="mt-1 w-28 rounded-lg border border-slate-300 px-3 py-1.5 text-sm"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600">Estoque</label>
            <input
              type="number"
              required
              value={estoque}
              onChange={(evento) => setEstoque(evento.target.value)}
              className="mt-1 w-24 rounded-lg border border-slate-300 px-3 py-1.5 text-sm"
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
              <th className="px-4 py-2 font-medium">Preço</th>
              <th className="px-4 py-2 font-medium">Estoque</th>
            </tr>
          </thead>
          <tbody>
            {produtos.map((produto) => (
              <tr key={produto.id} className="border-b border-slate-100 last:border-0">
                <td className="px-4 py-2 text-slate-800">{produto.nome}</td>
                <td className="px-4 py-2 text-slate-500">
                  {produto.preco.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                </td>
                <td className="px-4 py-2 text-slate-500">{produto.estoque}</td>
              </tr>
            ))}
            {produtos.length === 0 && (
              <tr>
                <td colSpan={3} className="px-4 py-6 text-center text-slate-400">
                  Nenhum produto ainda.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}
