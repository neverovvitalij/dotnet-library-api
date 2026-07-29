namespace dotnet_library_api.DTOs.V1;

public record CreateBookDto
(
   string Title,
   int PublishedYear,
   int AuthorId,
   List<int> GenreIds
 );
