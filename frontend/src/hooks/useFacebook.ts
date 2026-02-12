import { useState, useCallback } from 'react';
import { useFacebookStore } from '../store/facebookStore';
import { facebookService } from '../services/facebookService';

export function useFacebook() {
  const { setPages, removePage, setLoading } = useFacebookStore();
  const [error, setError] = useState<string | null>(null);

  const fetchPages = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const pages = await facebookService.getPages();
      setPages(pages);
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        'Failed to fetch Facebook pages.';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [setPages, setLoading]);

  const connectFacebook = useCallback(async () => {
    setError(null);
    try {
      const { redirectUrl } = await facebookService.initiateConnect();
      window.location.href = redirectUrl;
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
        'Failed to connect to Facebook.';
      setError(message);
    }
  }, []);

  const disconnectPage = useCallback(
    async (pageId: string) => {
      setError(null);
      try {
        await facebookService.disconnectPage(pageId);
        removePage(pageId);
      } catch (err: unknown) {
        const message =
          (err as { response?: { data?: { message?: string } } })?.response?.data?.message ||
          'Failed to disconnect page.';
        setError(message);
      }
    },
    [removePage]
  );

  return {
    fetchPages,
    connectFacebook,
    disconnectPage,
    error,
    setError,
  };
}
