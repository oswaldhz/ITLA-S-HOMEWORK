import { endpoints } from '../api/client';
import { useResource } from './useResource';

export function useUsuarios() {
  return useResource(endpoints.usuarios, []);
}
