const TOKEN_KEY = 'labproject_token';

export function saveToken(token) {
  localStorage.setItem(TOKEN_KEY, token);
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY);
}

export function parseUserFromToken(token) {
  if (!token) return null;
  try {
    const payload = token.split('.')[1];
    const decoded = JSON.parse(atob(payload));
    return {
      id: Number(decoded.sub),
      nombre: decoded.name || decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
      email: decoded.email || decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
      rol: decoded.role || decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/rol"]
    };
  } catch (err) {
    console.error('No se pudo decodificar el token JWT', err);
    return null;
  }
}

export function getStoredUser() {
  const token = getToken();
  return parseUserFromToken(token);
}
