import {
  Badge,
  Box,
  Button,
  Divider,
  Heading,
  HStack,
  Progress,
  Stack,
  Text,
  VStack,
  useToast,
} from '@chakra-ui/react';
import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiDelete, apiFetch, endpoints } from '../api/client';
import { useAuth } from '../hooks/useAuth';
import { useReservas } from '../hooks/useReservas';

function toDateLabel(value) {
  if (!value) return '';
  const date = new Date(value);
  // YYYY-MM-DD in local time
  return date.toLocaleDateString('en-CA');
}

function toTimeLabel(value) {
  if (!value) return '';
  const date = new Date(value);
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function isFuture(value) {
  const date = new Date(value);
  return date.getTime() > Date.now();
}

function formatDuration(ms) {
  const totalSeconds = Math.max(0, Math.floor(ms / 1000));
  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;

  const pad2 = (v) => String(v).padStart(2, '0');
  return `${pad2(hours)}:${pad2(minutes)}:${pad2(seconds)}`;
}

function buildQuery(params) {
  if (!params) return '';
  const cleaned = Object.fromEntries(
    Object.entries(params).filter(([, value]) => value !== undefined && value !== null && value !== '')
  );
  const qs = new URLSearchParams(cleaned).toString();
  return qs ? `?${qs}` : '';
}

export function CalendarAgendaPage() {
  const { user } = useAuth();
  const toast = useToast();
  const navigate = useNavigate();

  const [now, setNow] = useState(Date.now());

  useEffect(() => {
    // Solo necesitamos “cronómetro” para reservas en curso.
    // Un tick de 1s es suficiente y no rompe el performance para listas pequeñas.
    const id = window.setInterval(() => setNow(Date.now()), 1000);
    return () => window.clearInterval(id);
  }, []);

  const isAdminOrAssistant = user?.rol === 'Admin' || user?.rol === 'Asistente';
  const isStudent = user?.rol === 'Estudiante';

  // Estudiante: ver solo sus reservas (por privacidad) y para poder cancelar fácil.
  const query = useMemo(() => {
    if (isStudent && user?.id) return { usuarioId: String(user.id) };
    return undefined;
  }, [isStudent, user?.id]);

  const queryString = useMemo(() => buildQuery(query), [query]);

  const { data: reservas, setData: setReservas } = useReservas(query);

  const items = useMemo(() => {
    const list = Array.isArray(reservas) ? reservas : [];
    return list
      .slice()
      .sort((a, b) => new Date(a.fechaInicio).getTime() - new Date(b.fechaInicio).getTime());
  }, [reservas]);

  const canCancel = (reserva) => {
    if (isAdminOrAssistant) return true;
    // Estudiante: solo puede cancelar su reserva y solo si no ha empezado.
    if (isStudent && reserva?.usuarioId === user?.id) {
      return isFuture(reserva?.fechaInicio);
    }
    return false;
  };

  const canEdit = (reserva) => {
    // Solo Admin/Asistente editan.
    return isAdminOrAssistant;
  };

  const handleCancel = async (reservaId) => {
    if (!reservaId) return;
    const ok = window.confirm('¿Seguro que deseas cancelar esta reserva?');
    if (!ok) return;

    try {
      await apiDelete(`${endpoints.reservas}/${reservaId}`);

      // Refrescamos desde API para garantizar que realmente se borró en la BD.
      // (También evita estados inconsistentes si hubo cambios concurrentes.)
      try {
        const list = await apiFetch(`${endpoints.reservas}${queryString}`);
        setReservas(Array.isArray(list) ? list : []);
      } catch {
        // Fallback UI: si el refresh falla, al menos lo removemos localmente.
        setReservas((prev) => (Array.isArray(prev) ? prev.filter((r) => r.id !== reservaId) : prev));
      }

      toast({
        title: 'Reserva cancelada',
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
    } catch (err) {
      toast({
        title: 'No se pudo cancelar',
        description:
          err?.message?.toLowerCase?.().includes('no encontrado') || err?.message?.toLowerCase?.().includes('not found')
            ? 'Esta reserva ya no existe (alguien la eliminó o ya fue cancelada).'
            : err.message,
        status: 'error',
        duration: 4000,
        isClosable: true,
      });
    }
  };

  const handleEdit = (reservaId) => {
    if (!reservaId) return;
    navigate(`/reservas?edit=${reservaId}`);
  };

  return (
    <Stack spacing={6}>
      <Box>
        <Heading size="lg">Agenda del laboratorio</Heading>
        <Text color="gray.600">Visualiza las reservas confirmadas y próximas sesiones.</Text>
      </Box>

      <VStack align="stretch" spacing={4} position="relative">
        <Box position="absolute" top={0} left="18px" height="100%" width="2px" bg="blue.100" />
        {!items.length ? (
          <Box
            borderWidth="1px"
            borderRadius="lg"
            p={6}
            bg="white"
            _dark={{ bg: 'gray.800' }}
            boxShadow="md"
            ml={6}
          >
            <Heading size="md">No hay reservas aún</Heading>
            <Text mt={2} color="gray.600" _dark={{ color: 'gray.300' }}>
              Cuando se registren reservas, aparecerán aquí en orden cronológico.
            </Text>
            <Button mt={4} colorScheme="blue" onClick={() => navigate('/reservas')}>
              Ir a Reservas
            </Button>
          </Box>
        ) : (
          items.map((reserva) => (
            <HStack key={reserva.id} align="flex-start" spacing={4}>
              <Badge colorScheme="blue" minW="110px" textAlign="center" borderRadius="full" px={3} py={1} boxShadow="sm">
                {toDateLabel(reserva.fechaInicio)}
              </Badge>
              <Box flex={1} borderWidth="1px" borderRadius="lg" p={4} boxShadow="md" bg="white" _dark={{ bg: 'gray.800' }}>
                <HStack justify="space-between" align="start" spacing={3} flexWrap="wrap">
                  <Box>
                    <Heading size="md">{reserva.equipoIdentificador || `Equipo #${reserva.equipoId}`}</Heading>
                    <Text mt={1} fontWeight="medium">
                      Responsable: {reserva.usuarioNombre || `Usuario #${reserva.usuarioId}`}
                    </Text>
                  </Box>
                  <HStack spacing={2}>
                    <Badge colorScheme="purple">
                      {toTimeLabel(reserva.fechaInicio)} - {toTimeLabel(reserva.fechaFin)}
                    </Badge>
                  </HStack>
                </HStack>

                {(() => {
                  const start = new Date(reserva.fechaInicio).getTime();
                  const end = new Date(reserva.fechaFin).getTime();
                  const total = Math.max(0, end - start);

                  if (!Number.isFinite(start) || !Number.isFinite(end) || total <= 0) return null;

                  const isInProgress = now >= start && now < end;
                  const isUpcoming = now < start;
                  const isFinished = now >= end;

                  if (isInProgress) {
                    const elapsed = now - start;
                    const remaining = end - now;
                    const pct = Math.min(100, Math.max(0, (elapsed / total) * 100));

                    return (
                      <Box mt={3} p={3} borderRadius="md" bg="green.50" _dark={{ bg: 'green.900' }}>
                        <HStack justify="space-between" flexWrap="wrap" gap={2}>
                          <Badge colorScheme="green">En curso</Badge>
                          <Text fontSize="sm" color="gray.700" _dark={{ color: 'gray.200' }}>
                            Duración: {formatDuration(total)}
                          </Text>
                        </HStack>
                        <Progress mt={2} value={pct} size="sm" borderRadius="full" />
                        <HStack mt={2} justify="space-between" flexWrap="wrap" gap={2}>
                          <Text fontSize="sm">⏱️ Transcurrido: {formatDuration(elapsed)}</Text>
                          <Text fontSize="sm">🕒 Restante: {formatDuration(remaining)}</Text>
                        </HStack>
                      </Box>
                    );
                  }

                  if (isUpcoming) {
                    return (
                      <Box mt={3}>
                        <Badge colorScheme="blue">Programada</Badge>
                        <Text mt={1} fontSize="sm" color="gray.600" _dark={{ color: 'gray.300' }}>
                          El cronómetro se activará automáticamente cuando llegue la hora de inicio.
                        </Text>
                      </Box>
                    );
                  }

                  if (isFinished) {
                    return (
                      <Box mt={3}>
                        <Badge colorScheme="gray">Finalizada</Badge>
                        <Text mt={1} fontSize="sm" color="gray.600" _dark={{ color: 'gray.300' }}>
                          Duración: {formatDuration(total)}
                        </Text>
                      </Box>
                    );
                  }

                  return null;
                })()}

                <Divider my={3} />

                <HStack justify="space-between" align="center" flexWrap="wrap" gap={2}>
                  <Text color="gray.600" _dark={{ color: 'gray.300' }} fontSize="sm">
                    Asegúrate de liberar el equipo a tiempo para evitar conflictos en la siguiente sesión.
                  </Text>

                  <HStack spacing={2}>
                    {canEdit(reserva) && (
                      <Button size="sm" variant="outline" onClick={() => handleEdit(reserva.id)}>
                        Editar
                      </Button>
                    )}
                    {canCancel(reserva) && (
                      <Button size="sm" colorScheme="red" variant="outline" onClick={() => handleCancel(reserva.id)}>
                        Cancelar
                      </Button>
                    )}
                  </HStack>
                </HStack>
              </Box>
            </HStack>
          ))
        )}
      </VStack>
    </Stack>
  );
}
