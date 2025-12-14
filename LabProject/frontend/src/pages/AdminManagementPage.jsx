import {
  Alert,
  AlertDescription,
  AlertIcon,
  AlertTitle,
  Badge,
  Box,
  Button,
  Divider,
  FormControl,
  FormLabel,
  Grid,
  Heading,
  Input,
  Select,
  Stack,
  Table,
  Tbody,
  Td,
  Text,
  Th,
  Thead,
  Tr,
  useToast,
} from '@chakra-ui/react';
import { useState } from 'react';
import { apiDelete, apiFetch, apiPost, apiPut, endpoints } from '../api/client';
import { useEquipos } from '../hooks/useEquipos';
import { useSoftwares } from '../hooks/useSoftwares';
import { useAuth } from '../hooks/useAuth';

export function AdminManagementPage() {
  const toast = useToast();
  const { data: equipos, setData: setEquipos, isLoading: isLoadingEquipos } = useEquipos();
  const { data: softwares, setData: setSoftwares, isLoading: isLoadingSoftwares } = useSoftwares();
  const { user } = useAuth();
  const [softwareForm, setSoftwareForm] = useState({ nombre: '', version: '', licencia: '' });
  const [equipoForm, setEquipoForm] = useState({ identificador: '', ubicacion: '', estado: 'Disponible' });
  const [editingSoftwareId, setEditingSoftwareId] = useState(null);
  const [editingEquipoId, setEditingEquipoId] = useState(null);
  const [softwareEditForm, setSoftwareEditForm] = useState({ nombre: '', version: '', licencia: '' });
  const [equipoEditForm, setEquipoEditForm] = useState({ identificador: '', ubicacion: '', estado: 'Disponible' });

  if (user?.rol !== 'Admin') {
    return (
      <Alert status="warning" borderRadius="md">
        <AlertIcon />
        <Box>
          <AlertTitle>Acceso restringido</AlertTitle>
          <AlertDescription>Solo los usuarios con rol Administrador pueden gestionar el inventario.</AlertDescription>
        </Box>
      </Alert>
    );
  }

  const handleSoftwareSubmit = async (event) => {
    event.preventDefault();
    try {
      const createdSoftware = await apiPost(endpoints.softwares, softwareForm);
      setSoftwares((prev) => [...(prev || []), createdSoftware || softwareForm]);
      setSoftwareForm({ nombre: '', version: '', licencia: '' });
      toast({
        title: 'Software creado',
        description: `${softwareForm.nombre} agregado al catálogo`,
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
    } catch (err) {
      toast({ title: 'Error', description: err.message, status: 'error', duration: 3000, isClosable: true });
    }
  };

  const handleEquipoSubmit = async (event) => {
    event.preventDefault();

    if (!equipoForm.identificador.trim() || !equipoForm.ubicacion.trim() || !equipoForm.estado.trim()) {
      toast({
        title: 'Faltan datos requeridos',
        description: 'Completa Identificador, Ubicación y Estado antes de registrar el equipo.',
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
      return;
    }

    try {
      const createdEquipo = await apiPost(endpoints.equipos, {
        identificador: equipoForm.identificador,
        ubicacion: equipoForm.ubicacion,
        estado: equipoForm.estado,
      });
      setEquipos((prev) => [...(prev || []), createdEquipo || equipoForm]);
      setEquipoForm({ identificador: '', ubicacion: '', estado: 'Disponible' });
      toast({
        title: 'Equipo registrado',
        description: `${equipoForm.identificador} está disponible para reservas`,
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
    } catch (err) {
      toast({ title: 'Error', description: err.message, status: 'error', duration: 3000, isClosable: true });
    }
  };

  const refreshSoftwares = async () => {
    try {
      const list = await apiFetch(endpoints.softwares);
      setSoftwares(list || []);
    } catch (err) {
      toast({
        title: 'No se pudo actualizar la lista de software',
        description: err.message,
        status: 'error',
        duration: 3000,
        isClosable: true,
      });
    }
  };

  const refreshEquipos = async () => {
    try {
      const list = await apiFetch(endpoints.equipos);
      setEquipos(list || []);
    } catch (err) {
      toast({
        title: 'No se pudo actualizar la lista de equipos',
        description: err.message,
        status: 'error',
        duration: 3000,
        isClosable: true,
      });
    }
  };

  const handleSoftwareUpdate = async (id) => {
    try {
      await apiPut(`${endpoints.softwares}/${id}`, softwareEditForm);
      toast({
        title: 'Software actualizado',
        description: `${softwareEditForm.nombre} se actualizó correctamente`,
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      setEditingSoftwareId(null);
      refreshSoftwares();
    } catch (err) {
      toast({ title: 'Error al actualizar', description: err.message, status: 'error', duration: 3000, isClosable: true });
    }
  };

  const handleSoftwareDelete = async (id) => {
    try {
      await apiDelete(`${endpoints.softwares}/${id}`);
      toast({
        title: 'Software eliminado',
        description: 'El registro fue eliminado del catálogo',
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      refreshSoftwares();
    } catch (err) {
      toast({ title: 'Error al eliminar', description: err.message, status: 'error', duration: 3000, isClosable: true });
    }
  };

  const handleEquipoUpdate = async (id) => {
    if (!equipoEditForm.identificador.trim() || !equipoEditForm.ubicacion.trim() || !equipoEditForm.estado.trim()) {
      toast({
        title: 'Faltan datos requeridos',
        description: 'Completa Identificador, Ubicación y Estado antes de guardar.',
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
      return;
    }

    try {
      await apiPut(`${endpoints.equipos}/${id}`, {
        identificador: equipoEditForm.identificador,
        ubicacion: equipoEditForm.ubicacion,
        estado: equipoEditForm.estado,
      });
      toast({
        title: 'Equipo actualizado',
        description: `${equipoEditForm.identificador} se actualizó correctamente`,
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      setEditingEquipoId(null);
      refreshEquipos();
    } catch (err) {
      toast({ title: 'Error al actualizar', description: err.message, status: 'error', duration: 3000, isClosable: true });
    }
  };

  const handleEquipoDelete = async (id) => {
    try {
      await apiDelete(`${endpoints.equipos}/${id}`);
      toast({
        title: 'Equipo eliminado',
        description: 'El equipo se eliminó del inventario',
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      refreshEquipos();
    } catch (err) {
      toast({ title: 'Error al eliminar', description: err.message, status: 'error', duration: 3000, isClosable: true });
    }
  };

  return (
    <Stack spacing={8}>
      <Box p={6} borderRadius="2xl" bgGradient="linear(to-r, teal.500, green.400)" color="white" boxShadow="lg">
        <Heading size="lg">Administración</Heading>
        <Text opacity={0.9} maxW="2xl">
          Gestiona el inventario de software, equipos y la configuración de datos para que el laboratorio esté siempre listo.
        </Text>
      </Box>

      <Grid templateColumns={{ base: '1fr', md: 'repeat(2, 1fr)' }} gap={6}>
        <Box borderWidth="1px" borderRadius="lg" p={4} boxShadow="md" bg="white" _dark={{ bg: 'gray.800' }}>
          <Heading size="md" mb={4}>Nuevo software</Heading>
          <Stack as="form" spacing={3} onSubmit={handleSoftwareSubmit}>
            <FormControl isRequired>
              <FormLabel>Nombre</FormLabel>
              <Input value={softwareForm.nombre} onChange={(e) => setSoftwareForm({ ...softwareForm, nombre: e.target.value })} />
            </FormControl>
            <FormControl>
              <FormLabel>Versión</FormLabel>
              <Input value={softwareForm.version} onChange={(e) => setSoftwareForm({ ...softwareForm, version: e.target.value })} />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Licencia</FormLabel>
              <Input value={softwareForm.licencia} onChange={(e) => setSoftwareForm({ ...softwareForm, licencia: e.target.value })} />
            </FormControl>
            <Button type="submit" colorScheme="blue">Guardar software</Button>
          </Stack>
        </Box>

        <Box borderWidth="1px" borderRadius="lg" p={4} boxShadow="md" bg="white" _dark={{ bg: 'gray.800' }}>
          <Heading size="md" mb={4}>Nuevo equipo</Heading>
          <Stack as="form" spacing={3} onSubmit={handleEquipoSubmit}>
            <FormControl isRequired>
              <FormLabel>Identificador</FormLabel>
              <Input
                placeholder="Ej: PC-001"
                value={equipoForm.identificador}
                onChange={(e) => setEquipoForm({ ...equipoForm, identificador: e.target.value })}
              />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Ubicación</FormLabel>
              <Input
                placeholder="Ej: Laboratorio 1"
                value={equipoForm.ubicacion}
                onChange={(e) => setEquipoForm({ ...equipoForm, ubicacion: e.target.value })}
              />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Estado</FormLabel>
              <Select
                value={equipoForm.estado}
                onChange={(e) => setEquipoForm({ ...equipoForm, estado: e.target.value })}
              >
                <option value="Disponible">Disponible</option>
                <option value="Reservado">Reservado</option>
                <option value="Mantenimiento">Mantenimiento</option>
              </Select>
            </FormControl>
            <Button type="submit" colorScheme="teal">Registrar equipo</Button>
          </Stack>
        </Box>
      </Grid>

      <Grid templateColumns="1fr" gap={6} alignItems="start">
        <Stack spacing={6} bg="white" _dark={{ bg: 'gray.800' }} p={4} borderRadius="lg" boxShadow="md">
          <Box>
            <Heading size="md" mb={3}>Catálogo de software</Heading>
            <Table variant="simple" size="sm">
              <Thead>
                <Tr>
                  <Th>Nombre</Th>
                  <Th>Versión</Th>
                  <Th>Estado</Th>
                  <Th textAlign="right">Acciones</Th>
                </Tr>
              </Thead>
              <Tbody>
                {isLoadingSoftwares ? (
                  <Tr>
                    <Td colSpan={4}>
                      <Text color="gray.500">Cargando softwares...</Text>
                    </Td>
                  </Tr>
                ) : softwares?.length ? (
                  softwares.map((software) => (
                    <Tr key={software.id || software.nombre}>
                      <Td>
                        {editingSoftwareId === software.id ? (
                          <Input
                            size="sm"
                            value={softwareEditForm.nombre}
                            onChange={(e) => setSoftwareEditForm({ ...softwareEditForm, nombre: e.target.value })}
                          />
                        ) : (
                          software.nombre
                        )}
                      </Td>
                      <Td>
                        {editingSoftwareId === software.id ? (
                          <Stack spacing={2}>
                            <Input
                              size="sm"
                              placeholder="Versión"
                              value={softwareEditForm.version}
                              onChange={(e) => setSoftwareEditForm({ ...softwareEditForm, version: e.target.value })}
                            />
                            <Input
                              size="sm"
                              placeholder="Licencia"
                              value={softwareEditForm.licencia}
                              onChange={(e) => setSoftwareEditForm({ ...softwareEditForm, licencia: e.target.value })}
                            />
                          </Stack>
                        ) : (
                          <Stack spacing={0}>
                            <Text>{software.version || 'N/D'}</Text>
                            <Text fontSize="sm" color="gray.500">
                              {software.licencia || 'Licencia no registrada'}
                            </Text>
                          </Stack>
                        )}
                      </Td>
                      <Td>
                        <Badge colorScheme={software.estado === 'Disponible' ? 'green' : 'yellow'}>
                          {software.estado || 'Pendiente'}
                        </Badge>
                      </Td>
                      <Td textAlign="right">
                        {editingSoftwareId === software.id ? (
                          <Stack direction="row" spacing={2} justify="flex-end">
                            <Button size="sm" colorScheme="green" onClick={() => handleSoftwareUpdate(software.id)}>
                              Guardar
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => {
                                setEditingSoftwareId(null);
                                setSoftwareEditForm({ nombre: '', version: '', licencia: '' });
                              }}
                            >
                              Cancelar
                            </Button>
                          </Stack>
                        ) : (
                          <Stack direction="row" spacing={2} justify="flex-end">
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={() => {
                                setEditingSoftwareId(software.id);
                                setSoftwareEditForm({
                                  nombre: software.nombre || '',
                                  version: software.version || '',
                                  licencia: software.licencia || '',
                                });
                              }}
                            >
                              Editar
                            </Button>
                            <Button
                              size="sm"
                              colorScheme="red"
                              variant="outline"
                              onClick={() => handleSoftwareDelete(software.id)}
                              isDisabled={!software.id}
                            >
                              Eliminar
                            </Button>
                          </Stack>
                        )}
                      </Td>
                    </Tr>
                  ))
                ) : (
                  <Tr>
                    <Td colSpan={4}>
                      <Text color="gray.500">No hay softwares registrados.</Text>
                    </Td>
                  </Tr>
                )}
              </Tbody>
            </Table>
          </Box>

          <Divider />

          <Box>
            <Heading size="md" mb={3}>Inventario de equipos</Heading>
            <Table variant="simple" size="sm">
              <Thead>
                <Tr>
                  <Th>Identificador</Th>
                  <Th>Ubicación</Th>
                  <Th>Estado</Th>
                  <Th textAlign="right">Acciones</Th>
                </Tr>
              </Thead>
              <Tbody>
                {isLoadingEquipos ? (
                  <Tr>
                    <Td colSpan={4}>
                      <Text color="gray.500">Cargando equipos...</Text>
                    </Td>
                  </Tr>
                ) : equipos?.length ? (
                  equipos.map((equipo) => (
                    <Tr key={equipo.id || equipo.identificador}>
                      <Td>
                        {editingEquipoId === equipo.id ? (
                          <Input
                            size="sm"
                            value={equipoEditForm.identificador}
                            onChange={(e) => setEquipoEditForm({ ...equipoEditForm, identificador: e.target.value })}
                          />
                        ) : (
                          equipo.identificador
                        )}
                      </Td>
                      <Td>
                        {editingEquipoId === equipo.id ? (
                          <Input
                            size="sm"
                            value={equipoEditForm.ubicacion}
                            onChange={(e) => setEquipoEditForm({ ...equipoEditForm, ubicacion: e.target.value })}
                          />
                        ) : (
                          equipo.ubicacion || 'N/D'
                        )}
                      </Td>
                      <Td>
                        {editingEquipoId === equipo.id ? (
                          <Select
                            size="sm"
                            value={equipoEditForm.estado}
                            onChange={(e) => setEquipoEditForm({ ...equipoEditForm, estado: e.target.value })}
                          >
                            <option value="Disponible">Disponible</option>
                            <option value="Reservado">Reservado</option>
                            <option value="Mantenimiento">Mantenimiento</option>
                          </Select>
                        ) : (
                          <Badge colorScheme={equipo.estado === 'Disponible' ? 'green' : equipo.estado === 'Reservado' ? 'yellow' : 'red'}>
                            {equipo.estado || 'Pendiente'}
                          </Badge>
                        )}
                      </Td>
                      <Td textAlign="right">
                        {editingEquipoId === equipo.id ? (
                          <Stack direction="row" spacing={2} justify="flex-end">
                            <Button size="sm" colorScheme="green" onClick={() => handleEquipoUpdate(equipo.id)}>
                              Guardar
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => {
                                setEditingEquipoId(null);
                                setEquipoEditForm({ identificador: '', ubicacion: '', estado: 'Disponible' });
                              }}
                            >
                              Cancelar
                            </Button>
                          </Stack>
                        ) : (
                          <Stack direction="row" spacing={2} justify="flex-end">
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={() => {
                                setEditingEquipoId(equipo.id);
                                setEquipoEditForm({
                                  identificador: equipo.identificador || '',
                                  ubicacion: equipo.ubicacion || '',
                                  estado: equipo.estado || 'Disponible',
                                });
                              }}
                            >
                              Editar
                            </Button>
                            <Button
                              size="sm"
                              colorScheme="red"
                              variant="outline"
                              onClick={() => handleEquipoDelete(equipo.id)}
                              isDisabled={!equipo.id}
                            >
                              Eliminar
                            </Button>
                          </Stack>
                        )}
                      </Td>
                    </Tr>
                  ))
                ) : (
                  <Tr>
                    <Td colSpan={4}>
                      <Text color="gray.500">No hay equipos registrados.</Text>
                    </Td>
                  </Tr>
                )}
              </Tbody>
            </Table>
          </Box>
        </Stack>
      </Grid>
    </Stack>
  );
}
