import axios from 'axios';

export const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

export const endpoints = {
  equipos: '/equipos',
  reservas: '/reservas',
  softwares: '/softwares',
  usuarios: '/usuarios',
};

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export async function apiFetch(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
    ...options,
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || 'Solicitud fallida');
  }

  return response.status === 204 ? null : response.json();
}

export async function apiPost(path, payload) {
  return apiFetch(path, {
    method: 'POST',
    body: JSON.stringify(payload),
  });
}

export async function apiPut(path, payload) {
  return apiFetch(path, {
    method: 'PUT',
    body: JSON.stringify(payload),
  });
}

export async function apiDelete(path) {
  return apiFetch(path, { method: 'DELETE' });
}
