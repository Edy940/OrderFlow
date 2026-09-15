import { Navigate, Route, Routes } from 'react-router-dom'
import { RequireAuth } from './auth/RequireAuth'
import { ClientesPage } from './pages/ClientesPage'
import { DashboardLayout } from './pages/DashboardLayout'
import { LoginPage } from './pages/LoginPage'
import { PedidosPage } from './pages/PedidosPage'
import { ProdutosPage } from './pages/ProdutosPage'
import { RegisterPage } from './pages/RegisterPage'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/registrar" element={<RegisterPage />} />

      <Route element={<RequireAuth />}>
        <Route element={<DashboardLayout />}>
          <Route index element={<ClientesPage />} />
          <Route path="produtos" element={<ProdutosPage />} />
          <Route path="pedidos" element={<PedidosPage />} />
        </Route>
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
