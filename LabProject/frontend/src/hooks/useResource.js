import { useEffect, useRef, useState } from 'react';
import { apiFetch } from '../api/client';

export function useResource(path, fallback = [], enabled = true) {
  const fallbackRef = useRef(fallback);
  const [data, setData] = useState(fallback);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fallbackRef.current = fallback;

    if (!enabled) {
      setData(fallback);
    }
  }, [fallback, enabled]);

  useEffect(() => {
    if (!enabled) {
      setIsLoading(false);
      setData(fallbackRef.current);
      return undefined;
    }

    let isMounted = true;
    setIsLoading(true);
    apiFetch(path)
      .then((payload) => {
        if (isMounted) {
          setData(payload || fallbackRef.current);
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
  }, [path, enabled]);

  return { data, isLoading, error, setData };
}
