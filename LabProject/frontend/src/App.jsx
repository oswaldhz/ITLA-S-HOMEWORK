import { Container } from '@chakra-ui/react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { NavBar } from './components/NavBar';
import { ProtectedRoute } from './components/ProtectedRoute';
import { AdminManagementPage } from './pages/AdminManagementPage';
import { UserManagementPage } from './pages/UserManagementPage';
import { CalendarAgendaPage } from './pages/CalendarAgendaPage';
import { EquipmentListPage } from './pages/EquipmentListPage';
import { LoginPage } from './pages/LoginPage';
import { ReservationPage } from './pages/ReservationPage';

function App() {
  return (
    <BrowserRouter>
      <NavBar />
      <Container maxW="6xl" pb={8}>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/"
            element={
              <ProtectedRoute roles={["Admin", "Asistente", "Estudiante"]}>
                <EquipmentListPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/reservas"
            element={
              <ProtectedRoute roles={["Admin", "Asistente", "Estudiante"]}>
                <ReservationPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/agenda"
            element={
              <ProtectedRoute roles={["Admin", "Asistente", "Estudiante"]}>
                <CalendarAgendaPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin"
            element={
              <ProtectedRoute roles={["Admin"]}>
                <AdminManagementPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/usuarios"
            element={
              <ProtectedRoute roles={["Admin"]}>
                <UserManagementPage />
              </ProtectedRoute>
            }
          />
        </Routes>
      </Container>
    </BrowserRouter>
  );
}

export default App;
