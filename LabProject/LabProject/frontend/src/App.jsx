import { Container } from '@chakra-ui/react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { NavBar } from './components/NavBar';
import { AdminManagementPage } from './pages/AdminManagementPage';
import { CalendarAgendaPage } from './pages/CalendarAgendaPage';
import { EquipmentListPage } from './pages/EquipmentListPage';
import { ReservationPage } from './pages/ReservationPage';

function App() {
  return (
    <BrowserRouter>
      <NavBar />
      <Container maxW="6xl" pb={8}>
        <Routes>
          <Route path="/" element={<EquipmentListPage />} />
          <Route path="/reservas" element={<ReservationPage />} />
          <Route path="/agenda" element={<CalendarAgendaPage />} />
          <Route path="/admin" element={<AdminManagementPage />} />
        </Routes>
      </Container>
    </BrowserRouter>
  );
}

export default App;
