using System.Net;
using Application.Exceptions;
using Application.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Helpers;

public class ApiExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = GetStatusCode(context.Exception);

        if (response.StatusCode == (int)HttpStatusCode.InternalServerError)
        {
            WriteLogException(context.Exception, context.HttpContext.Request);
        }

        context.ExceptionHandled = true;

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true }
            },
            Formatting = Formatting.Indented
        };

        var data = BuildErrorResponse(context.Exception);

        response.WriteAsync(JsonConvert.SerializeObject(data, settings));
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            NotImplementedException => (int)HttpStatusCode.NotImplemented,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            BaseException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

    private static Response<object> BuildErrorResponse(Exception exception) =>
        new Response<object>
        {
            Success = false,
            Message = exception.Message,
            Errors = exception.Data["errors"] as Dictionary<string, List<string>>,
            Data = exception.Data["data"] != null
                ? new BaseExceptionResponse { Data = exception.Data["data"].ToString() }
                : null
        };

    private void WriteLogException(Exception exception, HttpRequest request)
    {
        string serverPath = AppDomain.CurrentDomain.BaseDirectory;
        string folderPath = serverPath + @"Log";

        string path = folderPath + @"\log_exceptions.txt";

        /* Verifica se a pasta existe */
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        /* Se o arquivo não existir já cria e insere o texto */
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                _ = WriteLogText(sw, request, exception);
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                _ = WriteLogText(sw, request, exception);
            }
        }
    }

    private async Task WriteLogText(StreamWriter sw, HttpRequest request, Exception exception)
    {
        try
        {
            string textException = string.Empty;

            textException += $"Data Exception: {DateTime.Now}" + Environment.NewLine;
            textException += $"Erro: {exception.Message}" + Environment.NewLine;
            textException += $"Endpoint: {request.Path.Value}" + Environment.NewLine;
            textException += $"Type: {request.Method}" + Environment.NewLine;

            //TODO: Falta colocar o Authorization no log quando for realizado a implementação do Bearer.
            //textException += $"Authorization: {request.Method}" + Environment.NewLine;

            textException += Environment.NewLine;

            request.EnableBuffering();
            request.Body.Position = 0;
            var rawRequestBody = await new StreamReader(request.Body).ReadToEndAsync();

            textException += $"Body:" + Environment.NewLine;
            textException += rawRequestBody;

            textException += Environment.NewLine;
            textException += Environment.NewLine;

            sw.WriteLine(textException);
        }
        catch (Exception)
        {

        }
    }
}