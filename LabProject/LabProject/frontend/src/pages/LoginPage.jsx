import {
  Alert,
  AlertDescription,
  AlertIcon,
  AlertTitle,
  Box,
  Button,
  FormControl,
  FormLabel,
  Heading,
  Input,
  Stack,
  Text,
  useToast,
} from '@chakra-ui/react';
import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const toast = useToast();

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError(null);
    try {
      await login({ email, password });
      toast({
        title: 'Sesión iniciada',
        status: 'success',
        duration: 3000,
        isClosable: true,
      });
      const redirectTo = location.state?.from || '/';
      navigate(redirectTo, { replace: true });
    } catch (err) {
      setError(err.message || 'No se pudo iniciar sesión');
    }
  };

  return (
    <Box maxW="3xl" mx="auto" bg="white" _dark={{ bg: 'gray.800' }} p={8} borderRadius="2xl" boxShadow="xl">
      <Stack spacing={6} align="center" textAlign="center" mb={4}>
        <Heading size="lg">Acceso al laboratorio</Heading>
        <Text color="gray.600" _dark={{ color: 'gray.300' }} maxW="2xl">
          Ingresa con tu cuenta institucional para gestionar reservas, revisar la agenda y actualizar el inventario. La sesión se
          mantiene segura con autenticación basada en tokens.
        </Text>
      </Stack>
      <Stack spacing={4} as="form" onSubmit={handleSubmit}>
        <FormControl isRequired>
          <FormLabel>Correo institucional</FormLabel>
          <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="usuario@lab.com" />
        </FormControl>
        <FormControl isRequired>
          <FormLabel>Contraseña</FormLabel>
          <Input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Ingresa tu clave" />
        </FormControl>
        <Button type="submit" colorScheme="blue" size="lg">
          Iniciar sesión
        </Button>
        {error && (
          <Alert status="error" borderRadius="md">
            <AlertIcon />
            <Box>
              <AlertTitle>Acceso denegado</AlertTitle>
              <AlertDescription>{error}</AlertDescription>
            </Box>
          </Alert>
        )}
      </Stack>
    </Box>
  );
}
