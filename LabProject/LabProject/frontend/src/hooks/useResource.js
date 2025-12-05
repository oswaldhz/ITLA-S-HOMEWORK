import { useEffect, useState } from 'react';
import { apiFetch } from '../api/client';

export function useResource(path, fallback = []) {
  const [data, setData] = useState(fallback);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
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
  }, [path, fallback]);

  return { data, isLoading, error, setData };
}
