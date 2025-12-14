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
  Button,
  Center,
} from '@chakra-ui/react';
import { useEquipos } from '../hooks/useEquipos';
import { useAuth } from '../hooks/useAuth';
import { Link as RouterLink } from 'react-router-dom';

export function EquipmentListPage() {
  const { data: equipos, isLoading, error } = useEquipos();
  const { isAuthenticated, user } = useAuth();

  const items = equipos || [];
  const disponibles = items.filter((equipo) => equipo.estado === 'Disponible').length;
  const showEmptyState = !isLoading && !error && items.length === 0;

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

      {showEmptyState ? (
        <Center>
          <Alert
            status="info"
            borderRadius="lg"
            alignItems="start"
            bg="blue.50"
            borderColor="blue.100"
            maxW="3xl"
            boxShadow="md"
          >
            <AlertIcon />
            <Box>
              <Heading size="sm" mb={1} color="blue.900">
                Aún no hay equipos registrados
              </Heading>
              <Text color="gray.700" mb={3}>
                Agrega el inventario del laboratorio para comenzar a gestionar las reservas y el seguimiento de disponibilidad.
              </Text>
              {user?.rol === 'Admin' && (
                <Button as={RouterLink} to="/admin" size="sm" colorScheme="blue">
                  Ir al panel de administración
                </Button>
              )}
            </Box>
          </Alert>
        </Center>
      ) : (
        <Grid templateColumns={{ base: 'repeat(1, 1fr)', md: 'repeat(3, 1fr)' }} gap={4}>
          {items.map((equipo) => (
            <Box
              key={equipo.id || equipo.identificador}
              borderWidth="1px"
              borderRadius="lg"
              p={4}
              boxShadow="sm"
              bg="white"
              _dark={{ bg: 'gray.800' }}
            >
              <VStack align="start" spacing={2}>
                <Heading size="md">{equipo.identificador || `Equipo #${equipo.id}`}</Heading>
                <Badge
                  colorScheme={
                    equipo.estado === 'Disponible' ? 'green' : equipo.estado === 'Reservado' ? 'yellow' : 'red'
                  }
                >
                  {equipo.estado}
                </Badge>
                <Text fontWeight="medium">Ubicación: {equipo.ubicacion || 'N/D'}</Text>
                <Text color="gray.600" _dark={{ color: 'gray.400' }} fontSize="sm">
                  Mantén este equipo libre de horarios cruzados. Si requiere software especial, verifica su disponibilidad en la
                  sección de administración.
                </Text>
              </VStack>
            </Box>
          ))}
        </Grid>
      )}
    </Stack>
  );
}
