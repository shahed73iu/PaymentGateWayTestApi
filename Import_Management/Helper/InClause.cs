using System.Linq;

namespace Import_Management.Helper
{
    public static class InClause
    {
        public static bool In<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }
    }
}
