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
import { useMemo, useState } from 'react';
import { apiDelete, apiFetch, apiPost, apiPut, endpoints } from '../api/client';
import { useUsuarios } from '../hooks/useUsuarios';
import { useAuth } from '../hooks/useAuth';

const ROLE_OPTIONS = ['Admin', 'Asistente', 'Estudiante'];

export function UserManagementPage() {
  const toast = useToast();
  const { user } = useAuth();
  const { data: usuarios, setData: setUsuarios, isLoading } = useUsuarios(true);

  const [createForm, setCreateForm] = useState({
    nombre: '',
    email: '',
    rol: 'Estudiante',
    password: '',
    confirmPassword: '',
  });

  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({
    nombre: '',
    email: '',
    rol: 'Estudiante',
    password: '',
    confirmPassword: '',
  });

  const canManage = user?.rol === 'Admin';

  const sortedUsers = useMemo(() => {
    const list = Array.isArray(usuarios) ? [...usuarios] : [];
    list.sort((a, b) => (a?.nombre || '').localeCompare(b?.nombre || ''));
    return list;
  }, [usuarios]);

  if (!canManage) {
    return (
      <Alert status="warning" borderRadius="md">
        <AlertIcon />
        <Box>
          <AlertTitle>Acceso restringido</AlertTitle>
          <AlertDescription>Solo los usuarios con rol Administrador pueden gestionar las cuentas.</AlertDescription>
        </Box>
      </Alert>
    );
  }

  const refreshUsuarios = async () => {
    try {
      const list = await apiFetch(endpoints.usuarios);
      setUsuarios(list || []);
    } catch (err) {
      toast({
        title: 'No se pudo actualizar la lista de usuarios',
        description: err.message,
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
    }
  };

  const handleCreate = async (event) => {
    event.preventDefault();

    if (!createForm.password || createForm.password.length < 6) {
      toast({
        title: 'Contraseña inválida',
        description: 'La contraseña debe tener al menos 6 caracteres.',
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
      return;
    }

    if (createForm.password !== createForm.confirmPassword) {
      toast({
        title: 'Las contraseñas no coinciden',
        description: 'Verifica la confirmación de contraseña.',
        status: 'error',
        duration: 3500,
        isClosable: true,
      });
      return;
    }

    try {
      const payload = {
        nombre: createForm.nombre,
        email: createForm.email,
        rol: createForm.rol,
        password: createForm.password,
      };

      const created = await apiPost(endpoints.usuarios, payload);
      setUsuarios((prev) => [...(prev || []), created].filter(Boolean));
      setCreateForm({ nombre: '', email: '', rol: 'Estudiante', password: '', confirmPassword: '' });

      toast({
        title: 'Usuario creado',
        description: `${payload.nombre} fue agregado correctamente.`,
        status: 'success',
        duration: 3000,
        isClosable: true,
      });

      // En caso de que el backend aplique normalizaciones, refrescamos.
      refreshUsuarios();
    } catch (err) {
      toast({ title: 'Error al crear usuario', description: err.message, status: 'error', duration: 3500, isClosable: true });
    }
  };

  const startEdit = (usuario) => {
    setEditingId(usuario.id);
    setEditForm({
      nombre: usuario.nombre || '',
      email: usuario.email || '',
      rol: usuario.rol || 'Estudiante',
      password: '',
      confirmPassword: '',
    });
  };

  const cancelEdit = () => {
    setEditingId(null);
    setEditForm({ nombre: '', email: '', rol: 'Estudiante', password: '', confirmPassword: '' });
  };

  const handleUpdate = async (id) => {
    if (editForm.password) {
      if (editForm.password.length < 6) {
        toast({
          title: 'Contraseña inválida',
          description: 'La contraseña debe tener al menos 6 caracteres.',
          status: 'error',
          duration: 3500,
          isClosable: true,
        });
        return;
      }
      if (editForm.password !== editForm.confirmPassword) {
        toast({
          title: 'Las contraseñas no coinciden',
          description: 'Verifica la confirmación de contraseña.',
          status: 'error',
          duration: 3500,
          isClosable: true,
        });
        return;
      }
    }

    try {
      const payload = {
        nombre: editForm.nombre,
        email: editForm.email,
        rol: editForm.rol,
        ...(editForm.password ? { password: editForm.password } : {}),
      };

      await apiPut(`${endpoints.usuarios}/${id}`, payload);
      toast({
        title: 'Usuario actualizado',
        description: 'Los cambios se guardaron correctamente.',
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      cancelEdit();
      refreshUsuarios();
    } catch (err) {
      toast({ title: 'Error al actualizar usuario', description: err.message, status: 'error', duration: 3500, isClosable: true });
    }
  };

  const handleDelete = async (id, nombre) => {
    const ok = window.confirm(`¿Eliminar el usuario "${nombre}"? Esta acción no se puede deshacer.`);
    if (!ok) return;

    try {
      await apiDelete(`${endpoints.usuarios}/${id}`);
      toast({
        title: 'Usuario eliminado',
        description: 'El usuario fue eliminado correctamente.',
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      refreshUsuarios();
    } catch (err) {
      toast({ title: 'Error al eliminar usuario', description: err.message, status: 'error', duration: 3500, isClosable: true });
    }
  };

  return (
    <Stack spacing={8}>
      <Box p={6} borderRadius="2xl" bgGradient="linear(to-r, purple.600, blue.500)" color="white" boxShadow="lg">
        <Heading size="lg">Usuarios</Heading>
        <Text opacity={0.9} maxW="2xl">
          Crea cuentas institucionales, asigna roles y administra accesos al sistema.
        </Text>
      </Box>

      <Grid templateColumns={{ base: '1fr', lg: '1fr 1fr' }} gap={6} alignItems="start">
        <Box borderWidth="1px" borderRadius="lg" p={4} boxShadow="md" bg="white" _dark={{ bg: 'gray.800' }}>
          <Heading size="md" mb={4}>Crear nuevo usuario</Heading>
          <Stack as="form" spacing={3} onSubmit={handleCreate}>
            <FormControl isRequired>
              <FormLabel>Nombre</FormLabel>
              <Input value={createForm.nombre} onChange={(e) => setCreateForm({ ...createForm, nombre: e.target.value })} />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Email</FormLabel>
              <Input type="email" value={createForm.email} onChange={(e) => setCreateForm({ ...createForm, email: e.target.value })} />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Rol</FormLabel>
              <Select value={createForm.rol} onChange={(e) => setCreateForm({ ...createForm, rol: e.target.value })}>
                {ROLE_OPTIONS.map((role) => (
                  <option key={role} value={role}>{role}</option>
                ))}
              </Select>
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Contraseña</FormLabel>
              <Input type="password" value={createForm.password} onChange={(e) => setCreateForm({ ...createForm, password: e.target.value })} />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Confirmar contraseña</FormLabel>
              <Input type="password" value={createForm.confirmPassword} onChange={(e) => setCreateForm({ ...createForm, confirmPassword: e.target.value })} />
            </FormControl>
            <Button type="submit" colorScheme="blue">Crear usuario</Button>
          </Stack>
        </Box>

        <Box borderWidth="1px" borderRadius="lg" p={4} boxShadow="md" bg="white" _dark={{ bg: 'gray.800' }}>
          <Heading size="md" mb={2}>Listado</Heading>
          <Text color="gray.600" _dark={{ color: 'gray.300' }} mb={4}>
            {isLoading ? 'Cargando usuarios...' : `${sortedUsers.length} usuario(s) registrado(s).`}
          </Text>

          <Table variant="simple" size="sm">
            <Thead>
              <Tr>
                <Th>Nombre</Th>
                <Th>Email</Th>
                <Th>Rol</Th>
                <Th textAlign="right">Acciones</Th>
              </Tr>
            </Thead>
            <Tbody>
              {isLoading ? (
                <Tr>
                  <Td colSpan={4}>
                    <Text color="gray.500">Cargando...</Text>
                  </Td>
                </Tr>
              ) : sortedUsers.length ? (
                sortedUsers.map((u) => (
                  <Tr key={u.id || u.email}>
                    <Td>
                      {editingId === u.id ? (
                        <Input size="sm" value={editForm.nombre} onChange={(e) => setEditForm({ ...editForm, nombre: e.target.value })} />
                      ) : (
                        <Text fontWeight="medium">{u.nombre}</Text>
                      )}
                    </Td>
                    <Td>
                      {editingId === u.id ? (
                        <Input size="sm" type="email" value={editForm.email} onChange={(e) => setEditForm({ ...editForm, email: e.target.value })} />
                      ) : (
                        <Text>{u.email}</Text>
                      )}
                    </Td>
                    <Td>
                      {editingId === u.id ? (
                        <Select size="sm" value={editForm.rol} onChange={(e) => setEditForm({ ...editForm, rol: e.target.value })}>
                          {ROLE_OPTIONS.map((role) => (
                            <option key={role} value={role}>{role}</option>
                          ))}
                        </Select>
                      ) : (
                        <Badge colorScheme={u.rol === 'Admin' ? 'purple' : u.rol === 'Asistente' ? 'blue' : 'green'}>
                          {u.rol}
                        </Badge>
                      )}
                    </Td>
                    <Td textAlign="right">
                      {editingId === u.id ? (
                        <Stack direction="row" spacing={2} justify="flex-end">
                          <Button size="sm" colorScheme="green" onClick={() => handleUpdate(u.id)}>Guardar</Button>
                          <Button size="sm" variant="outline" onClick={cancelEdit}>Cancelar</Button>
                        </Stack>
                      ) : (
                        <Stack direction="row" spacing={2} justify="flex-end">
                          <Button size="sm" variant="ghost" onClick={() => startEdit(u)}>Editar</Button>
                          <Button size="sm" colorScheme="red" variant="outline" onClick={() => handleDelete(u.id, u.nombre)} isDisabled={!u.id}>
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
                    <Text color="gray.500">No hay usuarios registrados.</Text>
                  </Td>
                </Tr>
              )}
            </Tbody>
          </Table>

          {editingId && (
            <>
              <Divider my={4} />
              <Box>
                <Heading size="sm" mb={2}>Opcional: cambiar contraseña</Heading>
                <Text fontSize="sm" color="gray.600" _dark={{ color: 'gray.300' }} mb={3}>
                  Si dejas estos campos vacíos, la contraseña actual se mantiene.
                </Text>
                <Grid templateColumns={{ base: '1fr', md: '1fr 1fr' }} gap={3}>
                  <FormControl>
                    <FormLabel>Nueva contraseña</FormLabel>
                    <Input type="password" value={editForm.password} onChange={(e) => setEditForm({ ...editForm, password: e.target.value })} />
                  </FormControl>
                  <FormControl>
                    <FormLabel>Confirmar contraseña</FormLabel>
                    <Input type="password" value={editForm.confirmPassword} onChange={(e) => setEditForm({ ...editForm, confirmPassword: e.target.value })} />
                  </FormControl>
                </Grid>
                <Text fontSize="sm" color="gray.500" mt={2}>
                  Consejo: usa una contraseña temporal y luego pídeles que la cambien.
                </Text>
              </Box>
            </>
          )}
        </Box>
      </Grid>
    </Stack>
  );
}
