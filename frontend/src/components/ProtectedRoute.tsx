import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export function ProtectedRoute({ roles }: { roles?: string[] }) {
  const { user, loading } = useAuth();

  if (loading) {
    return <div className="center-shell">Loading...</div>;
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (roles?.length && user.roleName && !roles.includes(user.roleName)) {
    return <Navigate to="/app" replace />;
  }

  return <Outlet />;
}
