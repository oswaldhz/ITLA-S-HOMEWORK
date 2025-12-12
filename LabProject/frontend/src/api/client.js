import axios from 'axios';
import { getToken } from './authStorage';

const envApiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7095/api';
export const API_BASE_URL = envApiUrl.replace(/\/$/, '');

export const endpoints = {
  equipos: '/equipos',
  reservas: '/reservas',
  softwares: '/Software',
  usuarios: '/usuarios',
  login: '/auth/login',
};

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export async function apiFetch(path, options = {}) {
  const token = getToken();
  const authHeaders = token
    ? {
        Authorization: `Bearer ${token}`,
      }
    : {};

  try {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      headers: {
        'Content-Type': 'application/json',
        ...authHeaders,
        ...options.headers,
      },
      ...options,
    });

    if (!response.ok) {
      const message = await response.text();
      throw new Error(message || 'Solicitud fallida');
    }

    return response.status === 204 ? null : response.json();
  } catch (error) {
    const friendlyMessage =
      error?.message === 'Failed to fetch'
        ? `No se pudo conectar con la API en ${API_BASE_URL}. Asegúrate de que el backend esté en ejecución.`
        : error?.message || 'Solicitud fallida';

    throw new Error(friendlyMessage);
  }
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
