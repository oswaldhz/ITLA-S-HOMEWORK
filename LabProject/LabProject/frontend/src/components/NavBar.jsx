import { Flex, HStack, Heading, IconButton, Spacer, useColorMode, Button, Text } from '@chakra-ui/react';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { MoonIcon, SunIcon } from '@chakra-ui/icons';
import { useAuth } from '../hooks/useAuth';

const baseLinks = [
  { to: '/', label: 'Equipos' },
  { to: '/reservas', label: 'Reservas' },
  { to: '/agenda', label: 'Agenda' },
];

export function NavBar() {
  const { colorMode, toggleColorMode } = useColorMode();
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const links = user?.rol === 'Admin' ? [...baseLinks, { to: '/admin', label: 'Administración' }] : baseLinks;

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <Flex as="header" align="center" gap={6} padding={4} boxShadow="sm" mb={6} bg="gray.50" _dark={{ bg: 'gray.900' }}>
      <Heading size="md">Laboratorio ITLA</Heading>
      <HStack spacing={4} as="nav">
        {links.map((link) => (
          <Button key={link.to} as={RouterLink} to={link.to} variant="ghost" fontWeight="semibold">
            {link.label}
          </Button>
        ))}
      </HStack>
      <Spacer />
      {isAuthenticated ? (
        <HStack spacing={3}>
          <Text fontWeight="semibold">{user?.nombre}</Text>
          <Button size="sm" onClick={handleLogout} variant="ghost">
            Cerrar sesión
          </Button>
        </HStack>
      ) : (
        <Button as={RouterLink} to="/login" size="sm" colorScheme="blue" variant="outline">
          Iniciar sesión
        </Button>
      )}
      <IconButton
        aria-label="Cambiar tema"
        onClick={toggleColorMode}
        icon={colorMode === 'light' ? <MoonIcon /> : <SunIcon />}
        variant="ghost"
      />
    </Flex>
  );
}
