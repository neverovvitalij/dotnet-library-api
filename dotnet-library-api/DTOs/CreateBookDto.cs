namespace dotnet_library_api.DTOs;

public record CreateBookDto
(
   string Title,
   int PublishedYear,
   int AuthorId,
   List<int> GenreIds
 );
