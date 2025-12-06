import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Heading,
  Input,
  Select,
  Stack,
  Text,
  Textarea,
  useToast,
} from '@chakra-ui/react';
import { useEffect, useState } from 'react';
import { apiPost, endpoints } from '../api/client';
import { useEquipos } from '../hooks/useEquipos';
import { useUsuarios } from '../hooks/useUsuarios';
import { useAuth } from '../hooks/useAuth';

const today = new Date().toISOString().slice(0, 10);

export function ReservationPage() {
  const { data: equipos } = useEquipos();
  const { user } = useAuth();
  const canManageUsers = user?.rol === 'Admin' || user?.rol === 'Asistente';
  const { data: usuarios } = useUsuarios(canManageUsers);
  const toast = useToast();
  const [form, setForm] = useState({
    equipoId: '',
    usuarioId: user?.id || '',
    fecha: today,
    horaInicio: '09:00',
    horaFin: '10:00',
    motivo: '',
  });

  useEffect(() => {
    if (user?.id) {
      setForm((prev) => ({ ...prev, usuarioId: user.id }));
    }
  }, [user]);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    try {
      await apiPost(endpoints.reservas, form);
      toast({
        title: 'Reserva creada',
        description: 'La solicitud fue enviada al laboratorio.',
        status: 'success',
        duration: 4000,
        isClosable: true,
      });
    } catch (err) {
      toast({
        title: 'No se pudo crear la reserva',
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
        { id: 1, nombre: 'Microscopio Óptico' },
        { id: 2, nombre: 'Impresora 3D' },
      ];
  const usuarioOptions = canManageUsers
    ? usuarios || []
    : user
      ? [{ id: user.id, nombre: user.nombre }]
      : [];

  return (
    <Stack spacing={6}>
      <Box>
        <Heading size="lg">Crear reserva</Heading>
        <Text color="gray.600" maxW="2xl">
          Selecciona el equipo, la fecha y el horario para tu práctica. El sistema evita horarios cruzados y valida que el equipo
          esté disponible.
        </Text>
      </Box>

      <Grid templateColumns={{ base: '1fr', lg: '2fr 1fr' }} gap={6} alignItems="start">
        <Stack spacing={4} as="form" onSubmit={handleSubmit} bg="white" _dark={{ bg: 'gray.800' }} p={5} borderRadius="xl" boxShadow="md">
          <FormControl isRequired>
            <FormLabel>Equipo</FormLabel>
            <Select placeholder="Selecciona un equipo" name="equipoId" value={form.equipoId} onChange={handleChange}>
              {equipoOptions.map((equipo) => (
                <option key={equipo.id || equipo.nombre} value={equipo.id || equipo.nombre}>
                  {equipo.nombre}
                </option>
              ))}
            </Select>
          </FormControl>

          <FormControl isRequired>
            <FormLabel>Usuario</FormLabel>
            <Select placeholder="Selecciona un usuario" name="usuarioId" value={form.usuarioId} onChange={handleChange}>
              {usuarioOptions.map((usuario) => (
                <option key={usuario.id || usuario.nombre} value={usuario.id || usuario.nombre}>
                  {usuario.nombre}
                </option>
              ))}
            </Select>
          </FormControl>

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
            <Button type="submit" colorScheme="blue" px={6}>
              Confirmar reserva
            </Button>
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
