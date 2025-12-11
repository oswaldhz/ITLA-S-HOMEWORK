import { createContext, useCallback, useMemo, useState } from 'react';
import { endpoints, apiPost } from '../api/client';
import { clearToken, getStoredUser, getToken, parseUserFromToken, saveToken } from '../api/authStorage';

export const AuthContext = createContext({
  isAuthenticated: false,
  user: null,
  token: null,
  login: async () => {},
  logout: () => {},
});

function normalizeUser(user) {
  if (!user) return null;
  return {
    id: user.id,
    nombre: user.nombre,
    email: user.email,
    rol: user.rol || user.role || user.Rol,
  };
}

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => getToken());
  const [user, setUser] = useState(() => normalizeUser(getStoredUser()));

  const login = useCallback(async (credentials) => {
    const response = await apiPost(endpoints.login, credentials);
    const nextToken = response.token;
    saveToken(nextToken);
    setToken(nextToken);

    const sessionUser = normalizeUser(response.user) || normalizeUser(parseUserFromToken(nextToken));
    setUser(sessionUser);
    return sessionUser;
  }, []);

  const logout = useCallback(() => {
    clearToken();
    setToken(null);
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({
      isAuthenticated: Boolean(token),
      token,
      user,
      login,
      logout,
    }),
    [login, logout, token, user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
