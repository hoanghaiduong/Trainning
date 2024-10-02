
using Microsoft.AspNetCore.Diagnostics;

namespace Trainning.Common.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            // Your response object
            var error = new { message = exception.Message };
            await context.Response.WriteAsJsonAsync(error, cancellationToken);
            return true;
        }
    }
}