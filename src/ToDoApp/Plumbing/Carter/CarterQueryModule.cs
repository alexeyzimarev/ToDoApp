using System;
using System.Linq;
using System.Threading.Tasks;
using Carter;
using Carter.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ToDoApp.Plumbing.Carter
{
    public abstract class CarterQueryModule<TConnection> : CarterModule where TConnection : class
    {
        protected void When<T, TResult>(string path, Func<T, TConnection, HttpContext, Task<TResult>> handle) where T : class, new() where TResult : class
            => Get(path, async http => {
                var connection = http.RequestServices.GetService<TConnection>();

                var (validationResult, qry) = await http.AutoBindAndValidate<T>();

                if (!validationResult.IsValid) {
                    http.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await http.Response.Negotiate(
                        new {
                            Errors = validationResult.Errors.Select(x => new { Key = x.ErrorCode, Message = x.ErrorMessage })
                        }
                    );

                    return;
                }

                var result = await handle(qry, connection, http);

                await http.Response.Negotiate(result, http.RequestAborted);
            });
    }
}