using System.Linq;

namespace IBMSPaymentGateway.Helper
{
    public static class InClause
    {
        public static bool In<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }
    }
}
