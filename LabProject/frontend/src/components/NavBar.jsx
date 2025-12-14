import { Flex, HStack, Heading, IconButton, Spacer, useColorMode, Button, Text, Box } from '@chakra-ui/react';
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

  // Only show navigation links after login
  const links = !isAuthenticated
    ? []
    : user?.rol === 'Admin'
      ? [...baseLinks, { to: '/admin', label: 'Administración' }, { to: '/usuarios', label: 'Usuarios' }]
      : baseLinks;

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Flex
      as="header"
      align="center"
      gap={6}
      padding={4}
      boxShadow="md"
      mb={8}
      position="sticky"
      top={0}
      zIndex={10}
      bgGradient={colorMode === 'light' ? 'linear(to-r, blue.50, white)' : 'linear(to-r, gray.900, gray.800)'}
      borderBottomWidth="1px"
      borderColor={colorMode === 'light' ? 'blue.100' : 'gray.700'}
    >
      <Box>
        <Heading size="md">Laboratorio ITLA</Heading>
        <Text fontSize="sm" color="gray.600" _dark={{ color: 'gray.400' }}>
          Reservas y gestión académica
        </Text>
      </Box>
      <HStack spacing={4} as="nav">
        {links.map((link) => (
          <Button
            key={link.to}
            as={RouterLink}
            to={link.to}
            variant="ghost"
            fontWeight="semibold"
            _hover={{ bg: colorMode === 'light' ? 'blue.50' : 'gray.700' }}
          >
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
