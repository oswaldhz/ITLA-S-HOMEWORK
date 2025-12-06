import {
  Badge,
  Box,
  Button,
  Divider,
  Alert,
  AlertIcon,
  AlertTitle,
  AlertDescription,
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
import { apiPost, endpoints } from '../api/client';
import { useEquipos } from '../hooks/useEquipos';
import { useSoftwares } from '../hooks/useSoftwares';
import { useAuth } from '../hooks/useAuth';

export function AdminManagementPage() {
  const toast = useToast();
  const { data: equipos } = useEquipos();
  const { data: softwares } = useSoftwares();
  const { user } = useAuth();
  const [softwareForm, setSoftwareForm] = useState({ nombre: '', version: '' });
  const [equipoForm, setEquipoForm] = useState({ nombre: '', laboratorio: '' });

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
      await apiPost(endpoints.softwares, softwareForm);
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
      await apiPost(endpoints.equipos, equipoForm);
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

      <Grid templateColumns={{ base: '1fr', lg: '2fr 1fr' }} gap={6} alignItems="start">
        <Stack spacing={6} bg="white" _dark={{ bg: 'gray.800' }} p={4} borderRadius="lg" boxShadow="md">
          <Box>
            <Heading size="md" mb={3}>Catálogo de software</Heading>
            <Table variant="simple" size="sm">
              <Thead>
                <Tr>
                  <Th>Nombre</Th>
                  <Th>Versión</Th>
                  <Th>Estado</Th>
                </Tr>
              </Thead>
              <Tbody>
                {softwareList.map((software) => (
                  <Tr key={software.id || software.nombre}>
                    <Td>{software.nombre}</Td>
                    <Td>{software.version || 'N/D'}</Td>
                    <Td>
                      <Badge colorScheme={software.estado === 'Disponible' ? 'green' : 'yellow'}>{software.estado || 'Pendiente'}</Badge>
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
                </Tr>
              </Thead>
              <Tbody>
                {equiposList.map((equipo) => (
                  <Tr key={equipo.id || equipo.nombre}>
                    <Td>{equipo.nombre}</Td>
                    <Td>{equipo.laboratorio || 'N/D'}</Td>
                    <Td>
                      <Badge colorScheme={equipo.estado === 'Disponible' ? 'green' : equipo.estado === 'Reservado' ? 'yellow' : 'red'}>
                        {equipo.estado || 'Pendiente'}
                      </Badge>
                    </Td>
                  </Tr>
                ))}
              </Tbody>
            </Table>
          </Box>
        </Stack>

        <Stack spacing={3} p={4} borderRadius="lg" bg="teal.50" _dark={{ bg: 'gray.700' }} boxShadow="md">
          <Heading size="md">Acceso a la base de datos</Heading>
          <Text color="gray.700" _dark={{ color: 'gray.200' }}>
            La API usa la cadena <strong>DefaultConnection</strong> definida en <code>LabProject.Api/appsettings.json</code>. Puedes
            actualizarla para apuntar a tu servidor SQL o sobreescribirla con la variable de entorno
            <code>ConnectionStrings__DefaultConnection</code> antes de iniciar la API.
          </Text>
          <Box borderLeftWidth="4px" borderColor="teal.400" pl={3}>
            <Text fontWeight="semibold">Tip de despliegue</Text>
            <Text color="gray.700" _dark={{ color: 'gray.100' }}>
              Mantén una copia de seguridad del archivo de configuración y evita exponer credenciales sensibles en el repositorio.
            </Text>
          </Box>
        </Stack>
      </Grid>
    </Stack>
  );
}
