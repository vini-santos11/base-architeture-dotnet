using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Enumerations.Enums;

namespace Enumerations.Helpers;

public class Utilities
{
    private static ReaderWriterLock _locker = new ReaderWriterLock();
        public static void WriteExceptionLog(Exception exception)
        {
            var logName =
                $"Exception_{DateTime.Now.Year.ToString().PadLeft(2, '0')}_{DateTime.Now.Month.ToString().PadLeft(2, '0')}_{DateTime.Now.Day.ToString().PadLeft(2, '0')}.log";
            var logFile = Path.Combine(Environment.CurrentDirectory, "log", logName);
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "log")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "log"));

            try
            {
                _locker.AcquireWriterLock(30000);
                using var writer = new StreamWriter(logFile, true);
                writer.WriteLine();
                writer.WriteLine("------------------------------------------");
                writer.WriteLine("Date : " + TimeZoneInfo.ConvertTime(DateTime.Now,
                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")));
                writer.WriteLine();

                writer.WriteLine(exception.GetType().FullName);
                writer.WriteLine("Source : " + exception.Source);

                writer.WriteLine("Messages : ");
                var errors = GetListException(exception);
                errors.ForEach(error => writer.WriteLine(error));


                writer.WriteLine("StackTrace : " + exception.StackTrace);
                writer.WriteLine("InnerException : " + exception.InnerException?.Message);
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }
        }

        public static List<string> GetListException(Exception ex)
        {
            if (ex == null)
                return new List<string>();
            return ex.InnerExceptions() == null
                ? new List<string>()
                : ex.InnerExceptions().Select(e => e.Message).ToList();
        }

        public static string GetExceptionList(Exception ex)
        {
            if (ex == null)
                return String.Empty;

            if (ex is BaseException)
            {
                var errors = ((BaseException)ex).GetDictionaryErros();
                var listE = (from key in errors.Keys from error in errors[key] select $"{key} - {error}").ToList();

                if (!string.IsNullOrEmpty(ex.Message))
                    listE.Insert(0, ex.Message);

                return String.Join(Environment.NewLine, listE);
            }

            var list = ex.InnerExceptions() == null ? new List<string>() : ex.InnerExceptions().Select(e => e.Message).ToList();

            return String.Join(Environment.NewLine, list);
        }

        public static string ReplaceDocument(string value)
        {
            return value.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty).Replace("_", string.Empty);
        }

        public static string OnlyNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            string justNumbers = new String(value.Where(Char.IsDigit).ToArray());

            return justNumbers;
        }

        public static bool ValidateDocument(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            var sanitizedValue = SanitizeDocument(value);

            return sanitizedValue.Length switch
            {
                11 => ValidateCpf(sanitizedValue),
                14 => ValidateCNPJ(sanitizedValue),
                _ => false
            };
        }

        private static string SanitizeDocument(string value)
        {
            return new string(value.Where(char.IsDigit).ToArray());
        }

        public static bool ValidateCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11 || IsSequential(cpf)) 
                return false;

            return ValidateCheckDigits(cpf, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 }, new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 });
        }

        public static bool ValidateCNPJ(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14) 
                return false;

            return ValidateCheckDigits(cnpj, new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }, new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
        }

        private static bool IsSequential(string value)
        {
            return value.Distinct().Count() == 1;
        }

        private static bool ValidateCheckDigits(string document, int[] multiplier1, int[] multiplier2)
        {
            var baseDocument = document.Substring(0, multiplier1.Length);
            var firstDigit = CalculateCheckDigit(baseDocument, multiplier1);
            var secondDigit = CalculateCheckDigit(baseDocument + firstDigit, multiplier2);

            return document.EndsWith(firstDigit.ToString() + secondDigit.ToString());
        }

        private static int CalculateCheckDigit(string baseDocument, int[] multipliers)
        {
            var sum = baseDocument
                .Select((digit, index) => int.Parse(digit.ToString()) * multipliers[index])
                .Sum();

            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }

        public static bool ValidateEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static object? GetProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property;
        }

        public static void Merge<T>(T target, T source) where T : class
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);

                if (prop.GetGetMethod().IsVirtual)
                {
                    if (value != null)
                        prop.SetValue(target, value, null);
                }
                else
                {
                    prop.SetValue(target, value, null);
                }
            }
        }
        public static string GetClaimsEnumDescription(ClaimsEnum claimsEnum)
        {
            var fi = claimsEnum.GetType().GetField(claimsEnum.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return claimsEnum.ToString();
        }

        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
                return attributes.First().Description;

            return value.ToString();
        }

        public static string GetHtmlString(string nameFile, string folderName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", folderName);
            var fileName = Path.Combine(path, nameFile);

            string html = string.Empty;
            using (var stream = new StreamReader(fileName))
            {
                html = stream.ReadToEnd();
            }

            return html;
        }

        public static BaseException CreateBaseException(string message, IEnumerable<string> messages)
        {
            BaseException ex = new BaseException(message);
            BaseException baseException = ex;
            foreach (var valor in messages)
            {
                var newException = new BaseException(valor);
                //ex.SetInnerException(newException);
                ex = newException;
            }

            return baseException;
        }
}