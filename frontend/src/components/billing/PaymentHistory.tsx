import type { TransactionDto } from '../../types/billing';
import { formatCurrency, formatDate } from '../../utils/formatters';

interface PaymentHistoryProps {
  transactions: TransactionDto[];
}

const statusBadge = {
  Pending: 'bg-amber-100 text-amber-700',
  Complete: 'bg-green-100 text-green-700',
  Failed: 'bg-red-100 text-red-700',
  Refunded: 'bg-gray-100 text-gray-600',
};

export function PaymentHistory({ transactions }: PaymentHistoryProps) {
  if (transactions.length === 0) {
    return (
      <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Payment History</h3>
        <p className="text-sm text-gray-500 text-center py-6">No transactions yet.</p>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden">
      <div className="p-6 pb-0">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Payment History</h3>
      </div>
      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-gray-200">
              <th className="text-left px-6 py-3 text-xs font-medium text-gray-500 uppercase">Date</th>
              <th className="text-left px-6 py-3 text-xs font-medium text-gray-500 uppercase">Type</th>
              <th className="text-left px-6 py-3 text-xs font-medium text-gray-500 uppercase">Amount</th>
              <th className="text-left px-6 py-3 text-xs font-medium text-gray-500 uppercase">Status</th>
              <th className="text-left px-6 py-3 text-xs font-medium text-gray-500 uppercase">Method</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {transactions.map((txn) => (
              <tr key={txn.id} className="hover:bg-gray-50">
                <td className="px-6 py-3 text-gray-900">{formatDate(txn.createdAt)}</td>
                <td className="px-6 py-3 text-gray-600 capitalize">{txn.transactionType}</td>
                <td className="px-6 py-3 font-medium text-gray-900">
                  {formatCurrency(txn.amount, txn.currency)}
                </td>
                <td className="px-6 py-3">
                  <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${statusBadge[txn.status]}`}>
                    {txn.status}
                  </span>
                </td>
                <td className="px-6 py-3 text-gray-600">{txn.paymentMethod || '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
