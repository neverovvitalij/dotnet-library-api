using System.Diagnostics;
using MediatR;

namespace dotnet_library_api.Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();
        Console.WriteLine($"[{typeof(TRequest).Name}] took {stopwatch.ElapsedMilliseconds}ms");
        return response;
    }
}
