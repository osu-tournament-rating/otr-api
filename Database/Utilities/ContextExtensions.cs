namespace Database.Utilities;

public static class ContextExtensions
{
    public static IQueryable<object>? Set(this OtrContext context, Type entityType)
    {
        try
        {
            return (IQueryable<object>?)context
                .GetType()
                .GetMethod("Set", 1, [])
                ?.MakeGenericMethod(entityType)
                .Invoke(context, null);
        }
        catch
        {
            return null;
        }
    }
}
