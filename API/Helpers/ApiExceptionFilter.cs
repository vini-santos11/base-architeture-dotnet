using System.Net;
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
        response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        WriteLogException(context.Exception, context.HttpContext.Request);

        context.ExceptionHandled = true;

        var errorResponse = new Response<object>
        {
            Success = false,
            Message = "An unexpected error occurred.",
            Data = null,
            Errors = null,
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        var json = JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true }
            },
            Formatting = Formatting.Indented
        });

        response.WriteAsync(json);
    }

    private void WriteLogException(Exception exception, HttpRequest request)
    {
        var serverPath = AppDomain.CurrentDomain.BaseDirectory;
        var folderPath = serverPath + @"Log";
        var path = folderPath + @"\log_exceptions.txt";
        
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        
        if (!File.Exists(path))
        {
            using var sw = File.CreateText(path);
            _ = WriteLogText(sw, request, exception);
        }
        else
        {
            using var sw = File.AppendText(path);
            _ = WriteLogText(sw, request, exception);
        }
    }

    private static async Task WriteLogText(StreamWriter sw, HttpRequest request, Exception exception)
    {
        try
        {
            var textException = string.Empty;

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

            await sw.WriteLineAsync(textException);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}