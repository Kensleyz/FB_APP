import { useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { Facebook, Unlink, RefreshCw, ExternalLink } from 'lucide-react';
import { useFacebook } from '../hooks/useFacebook';
import { useFacebookStore } from '../store/facebookStore';
import { Button } from '../components/common/Button';
import { Card } from '../components/common/Card';
import { Alert } from '../components/common/Alert';
import { Loading } from '../components/common/Loading';
import { formatDate } from '../utils/formatters';

export function ConnectFacebook() {
  const { fetchPages, connectFacebook, disconnectPage, error, setError } = useFacebook();
  const { pages, loading } = useFacebookStore();
  const [searchParams, setSearchParams] = useSearchParams();

  useEffect(() => {
    const connected = searchParams.get('connected');
    const err = searchParams.get('error');
    if (connected === 'true' || err) {
      setSearchParams({}, { replace: true });
      if (err) setError(decodeURIComponent(err));
    }
    fetchPages();
  }, []);

  return (
    <div className="space-y-6 max-w-3xl">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Facebook Pages</h1>
        <p className="text-sm text-gray-500 mt-1">
          Connect and manage your Facebook Business Pages.
        </p>
      </div>

      {error && <Alert type="error">{error}</Alert>}

      <Card>
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="p-3 bg-blue-50 rounded-lg">
              <Facebook className="w-6 h-6 text-blue-600" />
            </div>
            <div>
              <h3 className="font-semibold text-gray-900">Connect a New Page</h3>
              <p className="text-sm text-gray-500">
                Link your Facebook Business Page to start creating content.
              </p>
            </div>
          </div>
          <Button onClick={connectFacebook}>
            <ExternalLink className="w-4 h-4 mr-2" />
            Connect
          </Button>
        </div>
      </Card>

      {loading ? (
        <Loading message="Loading pages..." />
      ) : pages.length === 0 ? (
        <Card>
          <p className="text-sm text-gray-500 text-center py-4">
            No pages connected yet. Connect your first Facebook page to get started.
          </p>
        </Card>
      ) : (
        <div className="space-y-3">
          <h3 className="text-lg font-semibold text-gray-900">
            Connected Pages ({pages.length})
          </h3>
          {pages.map((page) => (
            <Card key={page.id}>
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  {page.profilePictureUrl ? (
                    <img
                      src={page.profilePictureUrl}
                      alt={page.pageName}
                      className="w-10 h-10 rounded-full"
                    />
                  ) : (
                    <div className="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center">
                      <Facebook className="w-5 h-5 text-blue-600" />
                    </div>
                  )}
                  <div>
                    <p className="font-medium text-gray-900">{page.pageName}</p>
                    <p className="text-xs text-gray-500">
                      {page.pageCategory} &middot; {page.followerCount.toLocaleString()} followers
                      &middot; Connected {formatDate(page.connectedAt)}
                    </p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => fetchPages()}
                    className="p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100"
                    title="Refresh"
                  >
                    <RefreshCw className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => disconnectPage(page.id)}
                    className="p-2 text-gray-400 hover:text-red-600 rounded-lg hover:bg-red-50"
                    title="Disconnect"
                  >
                    <Unlink className="w-4 h-4" />
                  </button>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
