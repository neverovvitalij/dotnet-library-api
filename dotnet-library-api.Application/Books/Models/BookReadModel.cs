
namespace dotnet_library_api.Application.Books.Models;
public record BookReadModel
(
  int Id,
  string Title,
  int PublishedYear,
  string AuthorName,
  List<string> Genres
);
