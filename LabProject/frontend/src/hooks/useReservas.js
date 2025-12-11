import { endpoints } from '../api/client';
import { useResource } from './useResource';

export function useReservas() {
  return useResource(endpoints.reservas, []);
}
