using Ideageek.FightersArena.Core.Dtos;
using System.Net;

namespace Ideageek.FightersArena.Core.Handlers
{
    public static class ResponseHandler
    {
        public static object ResponseStatus(bool error, string message, object? value, HttpStatusCode? statusCode = null)
        {
            int count = 0;

            if (value != null)
            {
                var type = value.GetType();

                // Case 1: value is ResponseDto<T>
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ResponseDto<>))
                {
                    var dataProp = type.GetProperty("Data");
                    if (dataProp != null)
                    {
                        var dataValue = dataProp.GetValue(value);
                        if (dataValue is System.Collections.IEnumerable dataEnumerable && !(dataValue is string))
                        {
                            foreach (var _ in dataEnumerable)
                                count++;
                        }
                        else if (dataValue != null)
                        {
                            count = 1; // Single object in Data
                        }
                    }
                }
                // Case 2: value is IEnumerable (but not string)
                else if (value is System.Collections.IEnumerable enumerable && !(value is string))
                {
                    foreach (var _ in enumerable)
                        count++;
                }
                // Case 3: value is primitive or single object
                else
                {
                    count = 1;
                }
            }

            var code = statusCode ?? (error ? HttpStatusCode.BadRequest : HttpStatusCode.OK);

            return new
            {
                code,
                date = DateTime.Now.ToLongDateString(),
                error,
                message,
                value,
                count
            };
        }

    }
}
