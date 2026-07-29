namespace dotnet_library_api.DTOs.V2;

public record BookDtoV2
(
    int Id,
    string Title,
    int PublishedYear,
    string AuthorName,
    List<string> Genres,
    string? Publisher
);
