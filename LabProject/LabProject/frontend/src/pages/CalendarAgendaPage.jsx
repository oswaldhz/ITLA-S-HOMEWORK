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
    <Stack spacing={6}>
      <Box>
        <Heading size="lg">Agenda del laboratorio</Heading>
        <Text color="gray.600">Visualiza las reservas confirmadas y próximas sesiones.</Text>
      </Box>

      <VStack align="stretch" spacing={4} position="relative">
        <Box position="absolute" top={0} left="18px" height="100%" width="2px" bg="blue.100" />
        {items.map((reserva, index) => (
          <HStack key={reserva.id || index} align="flex-start" spacing={4}>
            <Badge colorScheme="blue" minW="90px" textAlign="center" borderRadius="full" px={3} py={1} boxShadow="sm">
              {reserva.fecha}
            </Badge>
            <Box flex={1} borderWidth="1px" borderRadius="lg" p={4} boxShadow="md" bg="white" _dark={{ bg: 'gray.800' }}>
              <HStack justify="space-between" align="start">
                <Heading size="md">{reserva.equipo}</Heading>
                <Badge colorScheme="purple">{reserva.horaInicio} - {reserva.horaFin}</Badge>
              </HStack>
              <Divider my={3} />
              <Text fontWeight="medium">Responsable: {reserva.usuario}</Text>
              <Text color="gray.600" _dark={{ color: 'gray.300' }} fontSize="sm" mt={1}>
                Asegúrate de liberar el equipo a tiempo para evitar conflictos en la siguiente sesión.
              </Text>
            </Box>
          </HStack>
        ))}
      </VStack>
    </Stack>
  );
}
