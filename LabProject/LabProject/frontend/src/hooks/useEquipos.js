import { endpoints } from '../api/client';
import { useResource } from './useResource';

export function useEquipos() {
  return useResource(endpoints.equipos, []);
}
