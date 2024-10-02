using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Trainning.Common.Middlewares;

namespace Trainning.Common.Extensions
{
    public static class WebServiceExtension
    {
        public static WebApplication AddWebService(this WebApplication app)
        {
            app.UseExceptionHandler(_ => {});
            // app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            return app;
        }
    }
}