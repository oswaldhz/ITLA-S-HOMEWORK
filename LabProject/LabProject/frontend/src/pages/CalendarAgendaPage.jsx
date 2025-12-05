import { Badge, Box, Divider, Heading, HStack, Stack, Text, VStack } from '@chakra-ui/react';
import { useReservas } from '../hooks/useReservas';

export function CalendarAgendaPage() {
  const { data: reservas } = useReservas();

  const items = reservas?.length
    ? reservas
    : [
        { id: 1, fecha: '2024-05-01', horaInicio: '08:00', horaFin: '09:30', equipo: 'Microscopio Óptico', usuario: 'Ana López' },
        { id: 2, fecha: '2024-05-02', horaInicio: '10:00', horaFin: '12:00', equipo: 'Impresora 3D', usuario: 'Carlos Pérez' },
      ];

  return (
    <Stack spacing={4}>
      <Heading size="lg">Agenda del laboratorio</Heading>
      <Text color="gray.600">Visualiza las reservas confirmadas y próximas sesiones.</Text>
      <VStack align="stretch" spacing={3}>
        {items.map((reserva, index) => (
          <Box key={reserva.id || index} borderWidth="1px" borderRadius="md" p={4} boxShadow="xs">
            <HStack justify="space-between">
              <Heading size="md">{reserva.equipo}</Heading>
              <Badge colorScheme="blue">{reserva.fecha}</Badge>
            </HStack>
            <Divider my={3} />
            <Text fontWeight="medium">Horario: {reserva.horaInicio} - {reserva.horaFin}</Text>
            <Text color="gray.600">Responsable: {reserva.usuario}</Text>
          </Box>
        ))}
      </VStack>
    </Stack>
  );
}
