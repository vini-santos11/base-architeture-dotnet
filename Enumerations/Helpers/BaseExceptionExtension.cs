namespace Enumerations.Helpers;

public static class BaseExceptionExtension
{
    public static IEnumerable<Exception> InnerExceptions(this Exception exception)
    {
        Exception ex = exception;

        while (ex != null)
        {
            yield return ex;
            ex = ex.InnerException;
        }
    }
}