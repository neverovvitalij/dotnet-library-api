using dotnet_library_api.Application.Books.Commands;
using dotnet_library_api.Application.Books.Models;
using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Controllers;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.DTOs.V1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace dotnet_library_api.Tests;

public class LoansControllerTest
{
    [Fact]
    public async Task CreateLoan_ShouldReturnCreated_WhenBookIsAvailable()
    {
        // Arrange
        var mockLoanRepo = new Mock<ILoanRepository>();
        var mockMediator = new Mock<IMediator>();

        var expectedResult = new CreateLoanResult
            (
                new LoanReadModel(1, "Test Book", "Max Mustermann", DateTime.UtcNow, null),
                CreateLoanError.None
            );
        mockMediator.Setup(m => m.Send(It.IsAny<CreateLoanCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        var controller = new LoansController(mockLoanRepo.Object ,mockMediator.Object);
        var createLoanDto = new CreateLoanDto(1, "Max Mustermann");

        // Act
        var result = await controller.CreateLoan(createLoanDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var loanDto = Assert.IsType<LoanDto>(createdResult.Value);
        Assert.Equal("Max Mustermann", loanDto.BorrowerName);
    }

    [Fact]
    public async Task CreateLoan_ShouldReturnConflict_WhenBookIsAlreadyLoaned()
    {
        // Arrange
        var mockLoanRepo = new Mock<ILoanRepository>();
        var mockMediator = new Mock<IMediator>();


        var expectedResult = new CreateLoanResult
            (
               null, CreateLoanError.AlreadyLoaned
            );
        mockMediator.Setup(m => m.Send(It.IsAny<CreateLoanCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);
        var controller = new LoansController(mockLoanRepo.Object, mockMediator.Object);

        var createLoan = new CreateLoanDto(1, "Max Mustermann");

        // Act
        var result = await controller.CreateLoan(createLoan);

        // Assert
        var createdResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal("Das Buch ist bereits ausgeliehen", createdResult.Value);

    }
}