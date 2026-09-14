import type { AuthFetch } from './httpClient'
import type { ConsultaPedidosParams, PedidoResponse, ResultadoPaginado } from '../types/api'

export function listar(authFetch: AuthFetch, params: ConsultaPedidosParams = {}) {
  const query = new URLSearchParams()
  if (params.pagina) query.set('pagina', String(params.pagina))
  if (params.tamanhoPagina) query.set('tamanhoPagina', String(params.tamanhoPagina))
  if (params.ordenarPor) query.set('ordenarPor', params.ordenarPor)
  if (params.direcao) query.set('direcao', params.direcao)

  const suffix = query.toString() ? `?${query.toString()}` : ''
  return authFetch<ResultadoPaginado<PedidoResponse>>(`/pedidos${suffix}`)
}

export interface ItemPedidoInput {
  produtoId: string
  quantidade: number
}

export function criar(authFetch: AuthFetch, clienteId: string, itens: ItemPedidoInput[]) {
  return authFetch<string>('/pedidos', { method: 'POST', body: { clienteId, itens } })
}
