import { useEffect, useState } from 'react';
import { apiFetch } from '../api/client';
import { getToken } from '../api/authStorage';

export function useResource(path, fallback = [], enabled = true) {
  const [data, setData] = useState(fallback);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!enabled) {
      setIsLoading(false);
      setData(fallback);
      return undefined;
    }

    let isMounted = true;
    setIsLoading(true);
    apiFetch(path)
      .then((payload) => {
        if (isMounted) {
          setData(payload || fallback);
        }
      })
      .catch((err) => {
        if (isMounted) {
          setError(err.message);
        }
      })
      .finally(() => {
        if (isMounted) {
          setIsLoading(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, [path, fallback, enabled, getToken()]);

  return { data, isLoading, error, setData };
}
