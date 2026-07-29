
namespace dotnet_library_api.Application.Common;
public record PagedResult<T>(List<T> Items, int TotalCount);