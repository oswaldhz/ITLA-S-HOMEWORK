import { Container } from '@chakra-ui/react';
import { Outlet, RouterProvider, createBrowserRouter } from 'react-router-dom';
import { NavBar } from './components/NavBar';
import { ProtectedRoute } from './components/ProtectedRoute';
import { AdminManagementPage } from './pages/AdminManagementPage';
import { CalendarAgendaPage } from './pages/CalendarAgendaPage';
import { EquipmentListPage } from './pages/EquipmentListPage';
import { LoginPage } from './pages/LoginPage';
import { ReservationPage } from './pages/ReservationPage';

const AppLayout = () => (
  <>
    <NavBar />
    <Container maxW="6xl" pb={8}>
      <Outlet />
    </Container>
  </>
);

const router = createBrowserRouter(
  [
    {
      element: <AppLayout />,
      children: [
        { path: '/login', element: <LoginPage /> },
        {
          path: '/',
          element: (
            <ProtectedRoute roles={["Admin", "Asistente", "Estudiante"]}>
              <EquipmentListPage />
            </ProtectedRoute>
          ),
        },
        {
          path: '/reservas',
          element: (
            <ProtectedRoute roles={["Admin", "Asistente", "Estudiante"]}>
              <ReservationPage />
            </ProtectedRoute>
          ),
        },
        {
          path: '/agenda',
          element: (
            <ProtectedRoute roles={["Admin", "Asistente", "Estudiante"]}>
              <CalendarAgendaPage />
            </ProtectedRoute>
          ),
        },
        {
          path: '/admin',
          element: (
            <ProtectedRoute roles={["Admin"]}>
              <AdminManagementPage />
            </ProtectedRoute>
          ),
        },
      ],
    },
  ],
  {
    future: {
      v7_startTransition: true,
      v7_relativeSplatPath: true,
    },
  }
);

function App() {
  return <RouterProvider router={router} />;
}

export default App;
