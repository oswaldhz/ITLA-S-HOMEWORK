import { endpoints } from '../api/client';
import { useResource } from './useResource';

export function useSoftwares() {
  return useResource(endpoints.softwares, []);
}
