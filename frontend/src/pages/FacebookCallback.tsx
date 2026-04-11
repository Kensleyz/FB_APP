import { useEffect, useRef } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { facebookService } from '../services/facebookService';
import { Loading } from '../components/common/Loading';

export function FacebookCallback() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const called = useRef(false);

  useEffect(() => {
    if (called.current) return;
    called.current = true;

    const code = searchParams.get('code');
    const state = searchParams.get('state');
    const errorMsg = searchParams.get('error_message');

    if (errorMsg) {
      navigate(`/connect-facebook?error=${encodeURIComponent(errorMsg)}`, { replace: true });
      return;
    }

    if (!code) {
      navigate('/connect-facebook?error=No+authorization+code+received', { replace: true });
      return;
    }

    // Extract the redirect URL from state (format: "userId|redirectUrl")
    const redirectUrl = state?.split('|')[1] ?? '/connect-facebook';
    const path = redirectUrl.replace(window.location.origin, '') || '/connect-facebook';

    facebookService
      .callback(code, state ?? undefined)
      .then(() => {
        navigate(`${path}?connected=true`, { replace: true });
      })
      .catch((err) => {
        const msg = err?.response?.data?.errors?.[0] ?? 'Failed to connect Facebook page.';
        navigate(`${path}?error=${encodeURIComponent(msg)}`, { replace: true });
      });
  }, []);

  return <Loading message="Connecting your Facebook page..." />;
}
