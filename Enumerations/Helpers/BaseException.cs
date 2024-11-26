namespace Enumerations.Helpers;

public class BaseException : Exception
{
    public BaseException(string message) : base(message)
    {

    }

    public Dictionary<string, List<string>> GetDictionaryErros()
    {
        if (Data["errors"] != null)
            return (Dictionary<string, List<string>>)Data["errors"];


        return new Dictionary<string, List<string>>();
    }

    public void SetDictionaryErros(Dictionary<string, List<string>> errors)
    {
        Data["errors"] = errors;
    }

    public static BaseException Create(string message, Dictionary<string, List<string>> errors)
    {
        BaseException exception = new(message);
        exception.Data["errors"] = errors;
        return exception;
    }
}