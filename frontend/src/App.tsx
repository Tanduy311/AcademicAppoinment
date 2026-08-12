import { Navigate, Route, Routes } from 'react-router-dom';
import { AppShell } from './components/AppShell';
import { ProtectedRoute } from './components/ProtectedRoute';
import { useAuth } from './context/AuthContext';
import { DashboardPage } from './pages/DashboardPage';
import { AdminRolesPage } from './pages/AdminRolesPage';
import { AdminUsersPage } from './pages/AdminUsersPage';
import { LecturerAppointmentsPage } from './pages/LecturerAppointmentsPage';
import { LecturerDetailPage } from './pages/LecturerDetailPage';
import { LecturersPage } from './pages/LecturersPage';
import { LoginPage } from './pages/LoginPage';
import { NotificationsPage } from './pages/NotificationsPage';
import { NotFoundPage } from './pages/NotFoundPage';
import { RegisterLecturerPage } from './pages/RegisterLecturerPage';
import { RegisterStudentPage } from './pages/RegisterStudentPage';
import { LecturerSlotsPage } from './pages/LecturerSlotsPage';
import { StudentAppointmentsPage } from './pages/StudentAppointmentsPage';
import { StudentProfilePage } from './pages/StudentProfilePage';

function AppRedirect() {
  const { user, loading } = useAuth();

  if (loading) return <div className="center-shell">Loading...</div>;
  if (!user) return <Navigate to="/login" replace />;
  return <Navigate to="/app" replace />;
}

export function App() {
  return (
    <Routes>
      <Route path="/" element={<AppRedirect />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register/student" element={<RegisterStudentPage />} />
      <Route path="/register/lecturer" element={<RegisterLecturerPage />} />

      <Route element={<ProtectedRoute />}>
        <Route path="/app" element={<AppShell />}>
          <Route index element={<DashboardPage />} />
          <Route path="lecturers" element={<LecturersPage />} />
          <Route path="lecturers/:lecturerId" element={<LecturerDetailPage />} />
          <Route path="notifications" element={<NotificationsPage />} />

          <Route element={<ProtectedRoute roles={['Student']} />}>
            <Route path="appointments" element={<StudentAppointmentsPage />} />
            <Route path="profile" element={<StudentProfilePage />} />
          </Route>

          <Route element={<ProtectedRoute roles={['Lecturer']} />}>
            <Route path="lecturer-appointments" element={<LecturerAppointmentsPage />} />
            <Route path="slots" element={<LecturerSlotsPage />} />
          </Route>

          <Route element={<ProtectedRoute roles={['Admin']} />}>
            <Route path="admin/users" element={<AdminUsersPage />} />
            <Route path="admin/roles" element={<AdminRolesPage />} />
          </Route>
        </Route>
      </Route>

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
