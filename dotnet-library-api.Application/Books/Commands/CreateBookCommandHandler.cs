

using dotnet_library_api.Application.Books.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Domain.Models;
using MediatR;

namespace dotnet_library_api.Application.Books.Commands;
public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookReadModel?>
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IGenreRepository _genreRepository;

    public CreateBookCommandHandler(IBookRepository bookRepository, IAuthorRepository authorRepository, IGenreRepository genreRepository)   
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _genreRepository = genreRepository;
    }

    public async Task<BookReadModel?> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.GetByIdAsync(request.AuthorId);
        if(author == null)
        {
            return null;
        }

        var genreList = await _genreRepository.GetByIdsAsync(request.GenreIds);

        var book = new Book
        {
            AuthorId = author.Id,
            Title = request.Title,
            Genres = genreList,
            PublishedYear = request.PublishedYear,
        };
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
        
        var bookReadModel = new BookReadModel(book.Id, book.Title, book.PublishedYear, author.Name, 
            book.Genres.Select(g => g.Name).ToList());
        return bookReadModel;
    }
}
