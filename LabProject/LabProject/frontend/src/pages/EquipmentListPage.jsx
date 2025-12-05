import {
  Badge,
  Box,
  Grid,
  Heading,
  HStack,
  Spinner,
  Stack,
  Text,
  VStack,
  Alert,
  AlertIcon,
} from '@chakra-ui/react';
import { useEquipos } from '../hooks/useEquipos';
import { useAuth } from '../hooks/useAuth';

export function EquipmentListPage() {
  const { data: equipos, isLoading, error } = useEquipos();
  const { isAuthenticated } = useAuth();

  const placeholder = [
    { id: 1, nombre: 'Microscopio Óptico', estado: 'Disponible', laboratorio: 'B-101' },
    { id: 2, nombre: 'Impresora 3D', estado: 'En mantenimiento', laboratorio: 'B-201' },
    { id: 3, nombre: 'Cámara Termográfica', estado: 'Reservado', laboratorio: 'B-305' },
  ];

  const items = equipos?.length ? equipos : placeholder;

  return (
    <Stack spacing={4}>
      <Heading size="lg">Equipos de laboratorio</Heading>
      {!isAuthenticated && (
        <Alert status="info" borderRadius="md">
          <AlertIcon />
          Inicia sesión para ver la disponibilidad en tiempo real y realizar reservas.
        </Alert>
      )}
      {isLoading && (
        <HStack>
          <Spinner />
          <Text>Cargando equipos...</Text>
        </HStack>
      )}
      {error && <Text color="red.500">{error}</Text>}
      <Grid templateColumns={{ base: 'repeat(1, 1fr)', md: 'repeat(3, 1fr)' }} gap={4}>
        {items.map((equipo) => (
          <Box key={equipo.id || equipo.nombre} borderWidth="1px" borderRadius="lg" p={4} boxShadow="xs">
            <VStack align="start" spacing={2}>
              <Heading size="md">{equipo.nombre}</Heading>
              <Badge colorScheme={equipo.estado === 'Disponible' ? 'green' : equipo.estado === 'Reservado' ? 'yellow' : 'red'}>
                {equipo.estado}
              </Badge>
              <Text fontWeight="medium">Laboratorio: {equipo.laboratorio || 'N/D'}</Text>
            </VStack>
          </Box>
        ))}
      </Grid>
    </Stack>
  );
}
