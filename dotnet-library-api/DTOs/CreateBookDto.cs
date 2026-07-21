namespace dotnet_library_api.DTOs
{
    record CreateBookDto
    (
       string Title,
       int PublishedYear,
       int AuthorId,
       List<int> GenreIds
     );
}
