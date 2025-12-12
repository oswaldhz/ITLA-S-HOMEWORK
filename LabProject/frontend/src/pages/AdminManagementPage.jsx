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
  const { data: equipos, setData: setEquipos } = useEquipos();
  const { data: softwares, setData: setSoftwares } = useSoftwares();
  const { user } = useAuth();
  const [softwareForm, setSoftwareForm] = useState({ nombre: '', version: '', licencia: '' });
  const [equipoForm, setEquipoForm] = useState({ nombre: '', laboratorio: '' });
  const [editingSoftwareId, setEditingSoftwareId] = useState(null);
  const [editingEquipoId, setEditingEquipoId] = useState(null);
  const [softwareEditForm, setSoftwareEditForm] = useState({ nombre: '', version: '', licencia: '' });
  const [equipoEditForm, setEquipoEditForm] = useState({ nombre: '', laboratorio: '' });

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
    try {
      const createdEquipo = await apiPost(endpoints.equipos, equipoForm);
      setEquipos((prev) => [...(prev || []), createdEquipo || equipoForm]);
      setEquipoForm({ nombre: '', laboratorio: '' });
      toast({
        title: 'Equipo registrado',
        description: `${equipoForm.nombre} está disponible para reservas`,
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
    try {
      await apiPut(`${endpoints.equipos}/${id}`, equipoEditForm);
      toast({
        title: 'Equipo actualizado',
        description: `${equipoEditForm.nombre} se actualizó correctamente`,
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

  const softwareList = softwares?.length
    ? softwares
    : [
        { id: 1, nombre: 'AutoCAD', version: '2023', estado: 'Disponible' },
        { id: 2, nombre: 'MATLAB', version: 'R2024a', estado: 'Actualizando' },
      ];

  const equiposList = equipos?.length
    ? equipos
    : [
        { id: 1, nombre: 'Microscopio Óptico', laboratorio: 'B-101', estado: 'Disponible' },
        { id: 2, nombre: 'Cámara Termográfica', laboratorio: 'B-305', estado: 'Reservado' },
      ];

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
              <FormLabel>Nombre</FormLabel>
              <Input value={equipoForm.nombre} onChange={(e) => setEquipoForm({ ...equipoForm, nombre: e.target.value })} />
            </FormControl>
            <FormControl>
              <FormLabel>Laboratorio</FormLabel>
              <Input value={equipoForm.laboratorio} onChange={(e) => setEquipoForm({ ...equipoForm, laboratorio: e.target.value })} />
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
                {softwareList.map((software) => (
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
                      <Badge colorScheme={software.estado === 'Disponible' ? 'green' : 'yellow'}>{software.estado || 'Pendiente'}</Badge>
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
                          <Button size="sm" colorScheme="red" variant="outline" onClick={() => handleSoftwareDelete(software.id)}>
                            Eliminar
                          </Button>
                        </Stack>
                      )}
                    </Td>
                  </Tr>
                ))}
              </Tbody>
            </Table>
          </Box>

          <Divider />

          <Box>
            <Heading size="md" mb={3}>Inventario de equipos</Heading>
            <Table variant="simple" size="sm">
              <Thead>
                <Tr>
                  <Th>Equipo</Th>
                  <Th>Laboratorio</Th>
                  <Th>Estado</Th>
                  <Th textAlign="right">Acciones</Th>
                </Tr>
              </Thead>
              <Tbody>
                {equiposList.map((equipo) => (
                  <Tr key={equipo.id || equipo.nombre}>
                    <Td>
                      {editingEquipoId === equipo.id ? (
                        <Input
                          size="sm"
                          value={equipoEditForm.nombre}
                          onChange={(e) => setEquipoEditForm({ ...equipoEditForm, nombre: e.target.value })}
                        />
                      ) : (
                        equipo.nombre
                      )}
                    </Td>
                    <Td>
                      {editingEquipoId === equipo.id ? (
                        <Input
                          size="sm"
                          value={equipoEditForm.laboratorio}
                          onChange={(e) => setEquipoEditForm({ ...equipoEditForm, laboratorio: e.target.value })}
                        />
                      ) : (
                        equipo.laboratorio || 'N/D'
                      )}
                    </Td>
                    <Td>
                      <Badge colorScheme={equipo.estado === 'Disponible' ? 'green' : equipo.estado === 'Reservado' ? 'yellow' : 'red'}>
                        {equipo.estado || 'Pendiente'}
                      </Badge>
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
                              setEquipoEditForm({ nombre: '', laboratorio: '' });
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
                              setEquipoEditForm({ nombre: equipo.nombre || '', laboratorio: equipo.laboratorio || '' });
                            }}
                          >
                            Editar
                          </Button>
                          <Button size="sm" colorScheme="red" variant="outline" onClick={() => handleEquipoDelete(equipo.id)}>
                            Eliminar
                          </Button>
                        </Stack>
                      )}
                    </Td>
                  </Tr>
                ))}
              </Tbody>
            </Table>
          </Box>
        </Stack>
      </Grid>
    </Stack>
  );
}
