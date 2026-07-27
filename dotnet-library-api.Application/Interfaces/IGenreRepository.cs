
using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface IGenreRepository
{
    Task<List<Genre>> GetByIdsAsync(List<int> ids);
}
