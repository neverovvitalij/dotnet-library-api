namespace dotnet_library_api.DTOs
{
   record BookDto
   (
      int Id,
      string Title,
      int PublishedYear,
      string AuthorName,
      List<string> Genres
   );
}
