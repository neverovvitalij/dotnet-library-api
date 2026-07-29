namespace dotnet_library_api.DTOs.V1;

public record BookDto
(
  int Id,
  string Title,
  int PublishedYear,
  string AuthorName,
  List<string> Genres
);
