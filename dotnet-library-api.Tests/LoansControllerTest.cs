using dotnet_library_api.Application.Interfaces;
using dotnet_library_api.Controllers;
using dotnet_library_api.Domain.Models;
using dotnet_library_api.DTOs.V1;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace dotnet_library_api.Tests;

public class LoansControllerTest
{
    [Fact]
    public async Task CreateLoan_ShouldReturnCreated_WhenBookIsAvailable()
    {
        // Arrange
        var mockBookRepo = new Mock<IBookRepository>();
        var mockLoanRepo = new Mock<ILoanRepository>();

        var existingBook = new Book { Id = 1, Title = "Test Book" }; 

        mockBookRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingBook);
        mockLoanRepo.Setup(repo => repo.HasActiveLoanAsync(1)).ReturnsAsync(false);

        var controller = new LoansController(mockLoanRepo.Object, mockBookRepo.Object);
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
        var mockBookRepo = new Mock<IBookRepository>();
        var mockLoanRepo = new Mock<ILoanRepository>();

        var existingBook = new Book { Id = 1, Publisher = "Test Book" };

        mockBookRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingBook);
        mockLoanRepo.Setup(repo => repo.HasActiveLoanAsync(1)).ReturnsAsync(true);

        var controller = new LoansController(mockLoanRepo.Object, mockBookRepo.Object);
        var createLoan = new CreateLoanDto(1, "Max Mustermann");

        // Act
        var result = await controller.CreateLoan(createLoan);

        // Assert
        var createdResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.Equal("Das Buch ist bereits ausgeliehen", createdResult.Value);

    }
}