import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Heading,
  Input,
  Grid,
  Select,
  Stack,
  Text,
  Textarea,
  useToast,
} from '@chakra-ui/react';
import { useEffect, useMemo, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { apiFetch, apiPost, apiPut, endpoints } from '../api/client';
import { useEquipos } from '../hooks/useEquipos';
import { useUsuarios } from '../hooks/useUsuarios';
import { useAuth } from '../hooks/useAuth';

const today = new Date().toISOString().slice(0, 10);

function pad2(value) {
  return String(value).padStart(2, '0');
}

function toLocalDateInputValue(dateValue) {
  const d = new Date(dateValue);
  return `${d.getFullYear()}-${pad2(d.getMonth() + 1)}-${pad2(d.getDate())}`;
}

function toLocalTimeInputValue(dateValue) {
  const d = new Date(dateValue);
  return `${pad2(d.getHours())}:${pad2(d.getMinutes())}`;
}

export function ReservationPage() {
  const { data: equipos } = useEquipos();
  const { user } = useAuth();
  const canManageUsers = user?.rol === 'Admin' || user?.rol === 'Asistente';
  const { data: usuarios } = useUsuarios(canManageUsers);
  const toast = useToast();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();

  const editId = useMemo(() => {
    const raw = searchParams.get('edit');
    const parsed = raw ? Number(raw) : NaN;
    return Number.isFinite(parsed) && parsed > 0 ? parsed : null;
  }, [searchParams]);

  const isEditMode = !!editId;
  const [loadingEdit, setLoadingEdit] = useState(false);

  const [form, setForm] = useState({
    equipoId: '',
    usuarioId: user?.id ? String(user.id) : '',
    fecha: today,
    horaInicio: '09:00',
    horaFin: '10:00',
    motivo: '',
    softwareIds: [],
  });

  useEffect(() => {
    if (user?.id) {
      setForm((prev) => ({ ...prev, usuarioId: String(user.id) }));
    }
  }, [user]);

  useEffect(() => {
    if (!isEditMode) return;

    // Seguridad UX: si no es Admin/Asistente, no mostramos el modo edición.
    if (!canManageUsers) {
      toast({
        title: 'Acceso denegado',
        description: 'Solo Admin o Asistente puede editar reservas.',
        status: 'error',
        duration: 4000,
        isClosable: true,
      });
      navigate('/agenda', { replace: true });
      return;
    }

    let isMounted = true;
    setLoadingEdit(true);
    apiFetch(`${endpoints.reservas}/${editId}`)
      .then((reserva) => {
        if (!isMounted) return;
        const start = new Date(reserva.fechaInicio);
        const end = new Date(reserva.fechaFin);

        setForm((prev) => ({
          ...prev,
          equipoId: reserva.equipoId ? String(reserva.equipoId) : '',
          usuarioId: reserva.usuarioId ? String(reserva.usuarioId) : prev.usuarioId,
          fecha: toLocalDateInputValue(start),
          horaInicio: toLocalTimeInputValue(start),
          horaFin: toLocalTimeInputValue(end),
          softwareIds: Array.isArray(reserva.softwareIds) ? reserva.softwareIds.map((id) => String(id)) : [],
        }));
      })
      .catch((err) => {
        toast({
          title: 'No se pudo cargar la reserva',
          description: err.message,
          status: 'error',
          duration: 4000,
          isClosable: true,
        });
        navigate('/agenda', { replace: true });
      })
      .finally(() => {
        if (isMounted) setLoadingEdit(false);
      });

    return () => {
      isMounted = false;
    };
  }, [isEditMode, editId, canManageUsers, navigate, toast]);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!form.fecha || !form.horaInicio || !form.horaFin) {
      toast({
        title: 'Campos incompletos',
        description: 'Completa fecha, hora de inicio y hora de fin.',
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
      return;
    }

    const fechaInicio = new Date(`${form.fecha}T${form.horaInicio}`);
    const fechaFin = new Date(`${form.fecha}T${form.horaFin}`);

    if (fechaFin <= fechaInicio) {
      toast({
        title: 'Rango de tiempo inválido',
        description: 'La hora de fin debe ser posterior a la hora de inicio.',
        status: 'error',
        duration: 4000,
        isClosable: true,
      });
      return;
    }

    const equipoId = Number(form.equipoId);
    const usuarioId = Number(form.usuarioId || user?.id || 0);

    if (!equipoId) {
      toast({
        title: 'Falta el equipo',
        description: 'Selecciona un equipo antes de crear la reserva.',
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
      return;
    }

    if (!usuarioId) {
      toast({
        title: 'Falta el usuario',
        description: 'No se pudo determinar el usuario. Cierra sesión e inicia de nuevo.',
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
      return;
    }

    const payload = {
      EquipoId: equipoId,
      UsuarioId: usuarioId,
      FechaInicio: fechaInicio.toISOString(),
      FechaFin: fechaFin.toISOString(),
      SoftwareIds: (form.softwareIds || []).map((id) => Number(id)),
    };

    try {
      if (isEditMode) {
        await apiPut(`${endpoints.reservas}/${editId}`, payload);
        toast({
          title: 'Reserva actualizada',
          description: 'Los cambios se guardaron correctamente.',
          status: 'success',
          duration: 4000,
          isClosable: true,
        });
        navigate('/agenda');
      } else {
        await apiPost(endpoints.reservas, payload);
        toast({
          title: 'Reserva creada',
          description: 'La reserva fue registrada correctamente. Te llevamos a la Agenda para confirmarla.',
          status: 'success',
          duration: 4000,
          isClosable: true,
        });
        navigate('/agenda');
      }
    } catch (err) {
      toast({
        title: isEditMode ? 'No se pudo actualizar la reserva' : 'No se pudo crear la reserva',
        description: err.message,
        status: 'error',
        duration: 4000,
        isClosable: true,
      });
    }
  };

  const equipoOptions = equipos?.length
    ? equipos
    : [
        { id: 1, identificador: 'PC-001', estado: 'Disponible', ubicacion: 'Laboratorio 1' },
        { id: 2, identificador: 'PC-002', estado: 'Disponible', ubicacion: 'Laboratorio 2' },
      ];
  const usuarioOptions = canManageUsers
    ? usuarios || []
    : user
      ? [{ id: user.id, nombre: user.nombre }]
      : [];

  return (
    <Stack spacing={6}>
      <Box>
        <Heading size="lg">{isEditMode ? 'Editar reserva' : 'Crear reserva'}</Heading>
        <Text color="gray.600" maxW="2xl">
          Selecciona el equipo, la fecha y el horario para tu práctica. El sistema evita horarios cruzados y valida que el equipo
          esté disponible.
        </Text>
        {isEditMode && (
          <Text mt={2} color="gray.500">
            Modo edición (Admin/Asistente). Puedes cambiar equipo, usuario y horario.
          </Text>
        )}
      </Box>

      <Grid templateColumns={{ base: '1fr', lg: '2fr 1fr' }} gap={6} alignItems="start">
        <Stack
          spacing={4}
          as="form"
          onSubmit={handleSubmit}
          bg="white"
          _dark={{ bg: 'gray.800' }}
          p={5}
          borderRadius="xl"
          boxShadow="md"
          opacity={loadingEdit ? 0.7 : 1}
          pointerEvents={loadingEdit ? 'none' : 'auto'}
        >
          <FormControl isRequired>
            <FormLabel>Equipo</FormLabel>
            <Select placeholder="Selecciona un equipo" name="equipoId" value={form.equipoId} onChange={handleChange}>
              {equipoOptions.map((equipo) => (
                <option key={equipo.id} value={String(equipo.id)}>
                  {equipo.identificador || equipo.nombre || `Equipo #${equipo.id}`}
                </option>
              ))}
            </Select>
          </FormControl>


          {canManageUsers ? (
            <FormControl isRequired>
              <FormLabel>Usuario</FormLabel>
              <Select
                placeholder="Selecciona un usuario"
                name="usuarioId"
                value={form.usuarioId}
                onChange={handleChange}
              >
                {usuarioOptions.map((usuario) => (
                  <option key={usuario.id} value={String(usuario.id)}>
                    {usuario.nombre || usuario.email || `Usuario #${usuario.id}`}
                  </option>
                ))}
              </Select>
            </FormControl>
          ) : (
            <FormControl>
              <FormLabel>Usuario</FormLabel>
              <Input value={user?.nombre || user?.email || ''} isReadOnly />
            </FormControl>
          )}

          <Grid templateColumns={{ base: '1fr', sm: 'repeat(2, 1fr)' }} gap={4}>
            <FormControl isRequired>
              <FormLabel>Fecha</FormLabel>
              <Input type="date" name="fecha" value={form.fecha} min={today} onChange={handleChange} />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Hora inicio</FormLabel>
              <Input type="time" name="horaInicio" value={form.horaInicio} onChange={handleChange} />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Hora fin</FormLabel>
              <Input type="time" name="horaFin" value={form.horaFin} onChange={handleChange} />
            </FormControl>
          </Grid>

          <FormControl>
            <FormLabel>Motivo</FormLabel>
            <Textarea name="motivo" value={form.motivo} onChange={handleChange} placeholder="Describe la práctica o sesión" />
          </FormControl>

          <Box>
            <Stack direction={{ base: 'column', sm: 'row' }} spacing={3}>
              <Button type="submit" colorScheme="blue" px={6}>
                {isEditMode ? 'Guardar cambios' : 'Confirmar reserva'}
              </Button>
              {isEditMode && (
                <Button variant="outline" onClick={() => navigate('/agenda')}>
                  Volver
                </Button>
              )}
            </Stack>
          </Box>
        </Stack>

        <Stack spacing={4} bg="blue.50" _dark={{ bg: 'gray.700' }} p={5} borderRadius="xl" boxShadow="md">
          <Heading size="md">Consejos rápidos</Heading>
          <Text color="gray.700" _dark={{ color: 'gray.200' }}>
            • Verifica que el equipo tenga el software que necesitas.
            <br />• Usa rangos de tiempo realistas para evitar rechazos.
            <br />• Si necesitas varios equipos, crea reservas separadas para cada uno.
          </Text>
          <Box borderLeftWidth="4px" borderColor="blue.400" pl={3}>
            <Text fontWeight="semibold">Recordatorio</Text>
            <Text color="gray.600" _dark={{ color: 'gray.200' }}>
              Puedes editar o cancelar reservas desde la agenda siempre que no se superpongan con otra sesión aprobada.
            </Text>
          </Box>
        </Stack>
      </Grid>
    </Stack>
  );
}
