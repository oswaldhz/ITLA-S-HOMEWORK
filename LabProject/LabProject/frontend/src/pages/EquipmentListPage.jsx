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
  const disponibles = items.filter((equipo) => equipo.estado === 'Disponible').length;

  return (
    <Stack spacing={6}>
      <Box
        p={6}
        borderRadius="2xl"
        bgGradient="linear(to-r, blue.600, blue.400)"
        color="white"
        boxShadow="lg"
      >
        <Stack direction={{ base: 'column', md: 'row' }} justify="space-between" spacing={4} align="center">
          <Box>
            <Text fontWeight="semibold" textTransform="uppercase" letterSpacing="0.08em" fontSize="sm" opacity={0.9}>
              Panel de equipos
            </Text>
            <Heading size="lg" mt={2} mb={1}>
              Laboratorio de computación ITLA
            </Heading>
            <Text maxW="2xl" opacity={0.9}>
              Consulta el estado de cada estación, verifica la disponibilidad y prepara tus reservas según el software que
              necesitas para tu práctica.
            </Text>
          </Box>
          <VStack spacing={1} align="center" bg="white" color="blue.700" borderRadius="lg" px={5} py={3} boxShadow="md">
            <Text fontSize="sm" fontWeight="semibold">
              Equipos listos
            </Text>
            <Heading size="lg">{disponibles}</Heading>
            <Text fontSize="sm">Disponibles para reservar</Text>
          </VStack>
        </Stack>
      </Box>

      {!isAuthenticated && (
        <Alert status="info" borderRadius="md" bg="blue.50" borderColor="blue.100">
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
          <Box
            key={equipo.id || equipo.nombre}
            borderWidth="1px"
            borderRadius="lg"
            p={4}
            boxShadow="sm"
            bg="white"
            _dark={{ bg: 'gray.800' }}
          >
            <VStack align="start" spacing={2}>
              <Heading size="md">{equipo.nombre}</Heading>
              <Badge
                colorScheme={
                  equipo.estado === 'Disponible' ? 'green' : equipo.estado === 'Reservado' ? 'yellow' : 'red'
                }
              >
                {equipo.estado}
              </Badge>
              <Text fontWeight="medium">Laboratorio: {equipo.laboratorio || 'N/D'}</Text>
              <Text color="gray.600" _dark={{ color: 'gray.400' }} fontSize="sm">
                Mantén este equipo libre de horarios cruzados. Si requiere software especial, verifica su disponibilidad en la
                sección de administración.
              </Text>
            </VStack>
          </Box>
        ))}
      </Grid>
    </Stack>
  );
}
