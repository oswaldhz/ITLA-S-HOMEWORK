import { endpoints } from '../api/client';
import { useResource } from './useResource';

export function useUsuarios(enabled = true) {
  return useResource(endpoints.usuarios, [], enabled);
}
