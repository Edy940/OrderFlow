import { useEffect, useState, type FormEvent } from 'react'
import * as clientesApi from '../api/clientes'
import { ApiError } from '../api/httpClient'
import * as pedidosApi from '../api/pedidos'
import type { ItemPedidoInput } from '../api/pedidos'
import * as produtosApi from '../api/produtos'
import { useAuth } from '../auth/AuthContext'
import { ErrorBanner } from '../components/ErrorBanner'
import type { Cliente, PedidoResponse, Produto } from '../types/api'

const TAMANHO_PAGINA = 10

function linhaVazia(): ItemPedidoInput {
  return { produtoId: '', quantidade: 1 }
}

export function PedidosPage() {
  const { authFetch } = useAuth()
  const [clientes, setClientes] = useState<Cliente[]>([])
  const [produtos, setProdutos] = useState<Produto[]>([])
  const [pedidos, setPedidos] = useState<PedidoResponse[]>([])
  const [pagina, setPagina] = useState(1)
  const [totalPaginas, setTotalPaginas] = useState(1)

  const [clienteId, setClienteId] = useState('')
  const [itens, setItens] = useState<ItemPedidoInput[]>([linhaVazia()])
  const [erro, setErro] = useState<string | null>(null)
  const [carregando, setCarregando] = useState(false)

  async function carregarPedidos(paginaAlvo: number) {
    const resultado = await pedidosApi.listar(authFetch, {
      pagina: paginaAlvo,
      tamanhoPagina: TAMANHO_PAGINA,
      ordenarPor: 'data',
      direcao: 'desc',
    })
    setPedidos(resultado.itens)
    setPagina(resultado.pagina)
    setTotalPaginas(resultado.totalPaginas)
  }

  useEffect(() => {
    Promise.all([clientesApi.listar(authFetch), produtosApi.listar(authFetch), carregarPedidos(1)]).then(
      ([listaClientes, listaProdutos]) => {
        setClientes(listaClientes)
        setProdutos(listaProdutos)
      },
    ).catch(() => setErro('Não foi possível carregar clientes/produtos/pedidos.'))
  }, [])

  function atualizarItem(indice: number, campo: keyof ItemPedidoInput, valor: string) {
    setItens((prev) =>
      prev.map((item, i) =>
        i === indice ? { ...item, [campo]: campo === 'quantidade' ? Number(valor) : valor } : item,
      ),
    )
  }

  async function handleSubmit(evento: FormEvent) {
    evento.preventDefault()
    setErro(null)

    const itensValidos = itens.filter((item) => item.produtoId && item.quantidade > 0)
    if (!clienteId || itensValidos.length === 0) {
      setErro('Selecione um cliente e ao menos um item com quantidade válida.')
      return
    }

    setCarregando(true)
    try {
      await pedidosApi.criar(authFetch, clienteId, itensValidos)
      setClienteId('')
      setItens([linhaVazia()])
      await carregarPedidos(1)
    } catch (erroCapturado) {
      setErro(erroCapturado instanceof ApiError ? erroCapturado.message : 'Não foi possível criar o pedido.')
    } finally {
      setCarregando(false)
    }
  }

  return (
    <div className="space-y-6">
      <div className="rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 font-semibold text-slate-900">Novo pedido</h2>
        <form onSubmit={handleSubmit} className="space-y-3">
          <div>
            <label className="block text-xs font-medium text-slate-600">Cliente</label>
            <select
              required
              value={clienteId}
              onChange={(evento) => setClienteId(evento.target.value)}
              className="mt-1 rounded-lg border border-slate-300 px-3 py-1.5 text-sm"
            >
              <option value="">Selecione…</option>
              {clientes.map((cliente) => (
                <option key={cliente.id} value={cliente.id}>
                  {cliente.nome}
                </option>
              ))}
            </select>
          </div>

          <div className="space-y-2">
            <label className="block text-xs font-medium text-slate-600">Itens</label>
            {itens.map((item, indice) => (
              <div key={indice} className="flex items-center gap-2">
                <select
                  value={item.produtoId}
                  onChange={(evento) => atualizarItem(indice, 'produtoId', evento.target.value)}
                  className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm"
                >
                  <option value="">Produto…</option>
                  {produtos.map((produto) => (
                    <option key={produto.id} value={produto.id}>
                      {produto.nome}
                    </option>
                  ))}
                </select>
                <input
                  type="number"
                  min={1}
                  value={item.quantidade}
                  onChange={(evento) => atualizarItem(indice, 'quantidade', evento.target.value)}
                  className="w-20 rounded-lg border border-slate-300 px-3 py-1.5 text-sm"
                />
                <button
                  type="button"
                  onClick={() => setItens((prev) => prev.filter((_, i) => i !== indice))}
                  disabled={itens.length === 1}
                  className="text-xs text-slate-400 hover:text-red-600 disabled:opacity-30"
                >
                  remover
                </button>
              </div>
            ))}
            <button
              type="button"
              onClick={() => setItens((prev) => [...prev, linhaVazia()])}
              className="text-xs font-medium text-indigo-600 hover:underline"
            >
              + adicionar item
            </button>
          </div>

          <button
            type="submit"
            disabled={carregando}
            className="rounded-lg bg-indigo-600 px-4 py-1.5 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
          >
            {carregando ? 'Salvando…' : 'Criar pedido (autenticado)'}
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
              <th className="px-4 py-2 font-medium">Cliente</th>
              <th className="px-4 py-2 font-medium">Data</th>
              <th className="px-4 py-2 font-medium">Itens</th>
              <th className="px-4 py-2 font-medium">Total</th>
            </tr>
          </thead>
          <tbody>
            {pedidos.map((pedido) => (
              <tr key={pedido.id} className="border-b border-slate-100 last:border-0">
                <td className="px-4 py-2 text-slate-800">{pedido.cliente}</td>
                <td className="px-4 py-2 text-slate-500">{new Date(pedido.data).toLocaleDateString('pt-BR')}</td>
                <td className="px-4 py-2 text-slate-500">{pedido.itens.map((item) => item.produto).join(', ')}</td>
                <td className="px-4 py-2 text-slate-500">
                  {pedido.total.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                </td>
              </tr>
            ))}
            {pedidos.length === 0 && (
              <tr>
                <td colSpan={4} className="px-4 py-6 text-center text-slate-400">
                  Nenhum pedido ainda.
                </td>
              </tr>
            )}
          </tbody>
        </table>

        {totalPaginas > 1 && (
          <div className="flex items-center justify-between border-t border-slate-200 px-4 py-2 text-xs text-slate-500">
            <button
              onClick={() => carregarPedidos(pagina - 1)}
              disabled={pagina <= 1}
              className="disabled:opacity-30"
            >
              ← anterior
            </button>
            <span>
              Página {pagina} de {totalPaginas}
            </span>
            <button
              onClick={() => carregarPedidos(pagina + 1)}
              disabled={pagina >= totalPaginas}
              className="disabled:opacity-30"
            >
              próxima →
            </button>
          </div>
        )}
      </div>
    </div>
  )
}
