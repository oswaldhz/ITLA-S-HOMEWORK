import { endpoints } from '../api/client';
import { useResource } from './useResource';

function buildQuery(params) {
  if (!params) return '';

  const cleaned = Object.fromEntries(
    Object.entries(params).filter(([, value]) => value !== undefined && value !== null && value !== '')
  );

  const qs = new URLSearchParams(cleaned).toString();
  return qs ? `?${qs}` : '';
}

/**
 * @param {Record<string, any>=} params Optional query params for /reservas.
 * @param {boolean=} enabled
 */
export function useReservas(params, enabled = true) {
  const query = buildQuery(params);
  return useResource(`${endpoints.reservas}${query}`, [], enabled);
}
