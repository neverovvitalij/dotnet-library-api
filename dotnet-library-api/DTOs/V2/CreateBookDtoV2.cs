namespace dotnet_library_api.DTOs.V2;

public record CreateBookDtoV2
(
   string Title,
   int PublishedYear,
   int AuthorId,
   List<int> GenreIds,
   string? Publisher
 );
