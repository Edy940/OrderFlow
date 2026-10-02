export interface TokenResponse {
  accessToken: string
  accessTokenExpiraEm: string
  refreshToken: string
  refreshTokenExpiraEm: string
}

export interface JwtClaims {
  sub: string
  email: string
  name: string
  role: string
  jti: string
  exp: number
}

// Payload cru do token: o claim de papel sai com a URI longa do
// ClaimTypes.Role do .NET (http://schemas.microsoft.com/ws/2008/06/...),
// não como "role" curto — normalizado em AuthContext.montarSessao.
export interface RawJwtPayload {
  sub: string
  email: string
  name: string
  jti: string
  exp: number
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string
}

export interface Cliente {
  id: string
  nome: string
  email: string
}

export interface Produto {
  id: string
  nome: string
  preco: number
  estoque: number
}

export interface ItemPedidoResponse {
  produto: string
  quantidade: number
  precoUnitario: number
  subtotal: number
}

export interface PedidoResponse {
  id: string
  cliente: string
  data: string
  itens: ItemPedidoResponse[]
  total: number
}

export interface ResultadoPaginado<T> {
  itens: T[]
  pagina: number
  tamanhoPagina: number
  totalItens: number
  totalPaginas: number
}

export interface ConsultaPedidosParams {
  pagina?: number
  tamanhoPagina?: number
  ordenarPor?: 'data' | 'cliente' | 'valortotal'
  direcao?: 'asc' | 'desc'
}
