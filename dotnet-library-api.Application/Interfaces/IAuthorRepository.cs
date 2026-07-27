

using dotnet_library_api.Domain.Models;

namespace dotnet_library_api.Application.Interfaces;
public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(int id);
}
