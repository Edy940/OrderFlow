import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { SessionPanel } from '../components/SessionPanel'

const linkBase = 'rounded-lg px-3 py-2 text-sm font-medium transition'
const linkAtivo = 'bg-indigo-50 text-indigo-700'
const linkInativo = 'text-slate-600 hover:bg-slate-100'

export function DashboardLayout() {
  const { session, logout } = useAuth()
  const navigate = useNavigate()

  async function handleLogout() {
    await logout()
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-slate-50">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
          <div className="flex items-center gap-6">
            <span className="text-lg font-semibold text-slate-900">OrderFlow</span>
            <nav className="flex gap-1">
              <NavLink to="/" end className={({ isActive }) => `${linkBase} ${isActive ? linkAtivo : linkInativo}`}>
                Clientes
              </NavLink>
              <NavLink to="/produtos" className={({ isActive }) => `${linkBase} ${isActive ? linkAtivo : linkInativo}`}>
                Produtos
              </NavLink>
              <NavLink to="/pedidos" className={({ isActive }) => `${linkBase} ${isActive ? linkAtivo : linkInativo}`}>
                Pedidos
              </NavLink>
            </nav>
          </div>
          <div className="flex items-center gap-3 text-sm">
            <span className="text-slate-500">{session?.claims.email}</span>
            <button
              onClick={handleLogout}
              className="rounded-lg border border-slate-300 px-3 py-1.5 font-medium text-slate-700 hover:bg-slate-100"
            >
              Sair
            </button>
          </div>
        </div>
      </header>

      <main className="mx-auto flex max-w-6xl gap-6 px-4 py-6">
        <div className="min-w-0 flex-1">
          <Outlet />
        </div>
        <SessionPanel />
      </main>
    </div>
  )
}
