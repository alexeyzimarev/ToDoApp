using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Carter;
using Carter.Response;
using Microsoft.AspNetCore.Http;

namespace ToDoApp.Plumbing.Carter
{
    public abstract class CarterCommandModule : CarterModule
    {
        readonly ExecuteCommand _executeCommand;

        protected CarterCommandModule(ExecuteCommand executeCommand) => _executeCommand = executeCommand;

        protected void Post<T>(string path) where T : class, new() => Post(path, Handle<T>);
        
        protected void Put<T>(string path) where T : class, new() => Put(path, Handle<T>);

        async Task Handle<T>(HttpContext http) where T : new()
        {
            var (result, cmd) = await http.AutoBindAndValidate<T>();

            if (!result.IsValid)
            {
                http.Response.StatusCode = StatusCodes.Status400BadRequest;
                await http.Response.Negotiate(MessageResponse.Failure(new ValidationException(result.Errors)));

                return;
            }

            var response = await _executeCommand(cmd, http.User, http.RequestAborted);

            http.Response.StatusCode = response.Status == MessageHandlingStatus.Success
                ? StatusCodes.Status200OK
                : StatusCodes.Status409Conflict;

            await http.Response.Negotiate(response, http.RequestAborted);
        }
    }

    public delegate ValueTask<MessageResponse> ExecuteCommand(object cmd, ClaimsPrincipal user, CancellationToken cancellationToken);
}