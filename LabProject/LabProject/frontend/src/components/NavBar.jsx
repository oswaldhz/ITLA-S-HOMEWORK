import { Flex, HStack, Heading, IconButton, Spacer, useColorMode, Button } from '@chakra-ui/react';
import { Link as RouterLink } from 'react-router-dom';
import { MoonIcon, SunIcon } from '@chakra-ui/icons';

const links = [
  { to: '/', label: 'Equipos' },
  { to: '/reservas', label: 'Reservas' },
  { to: '/agenda', label: 'Agenda' },
  { to: '/admin', label: 'Administración' },
];

export function NavBar() {
  const { colorMode, toggleColorMode } = useColorMode();

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
      <IconButton
        aria-label="Cambiar tema"
        onClick={toggleColorMode}
        icon={colorMode === 'light' ? <MoonIcon /> : <SunIcon />}
        variant="ghost"
      />
    </Flex>
  );
}
