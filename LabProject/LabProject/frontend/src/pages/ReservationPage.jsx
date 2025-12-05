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
import { useState } from 'react';
import { apiPost, endpoints } from '../api/client';
import { useEquipos } from '../hooks/useEquipos';
import { useUsuarios } from '../hooks/useUsuarios';

const today = new Date().toISOString().slice(0, 10);

export function ReservationPage() {
  const { data: equipos } = useEquipos();
  const { data: usuarios } = useUsuarios();
  const toast = useToast();
  const [form, setForm] = useState({
    equipoId: '',
    usuarioId: '',
    fecha: today,
    horaInicio: '09:00',
    horaFin: '10:00',
    motivo: '',
  });

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
  const usuarioOptions = usuarios?.length
    ? usuarios
    : [
        { id: 1, nombre: 'Ana López' },
        { id: 2, nombre: 'Carlos Pérez' },
      ];

  return (
    <Stack spacing={4} as="form" onSubmit={handleSubmit}>
      <Heading size="lg">Crear reserva</Heading>
      <Text color="gray.600">Completa los datos para reservar un equipo de laboratorio.</Text>

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

      <FormControl>
        <FormLabel>Motivo</FormLabel>
        <Textarea name="motivo" value={form.motivo} onChange={handleChange} placeholder="Describe la práctica o sesión" />
      </FormControl>

      <Box>
        <Button type="submit" colorScheme="blue">Confirmar reserva</Button>
      </Box>
    </Stack>
  );
}
