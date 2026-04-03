using Cnss.Cotisation.Application.SubmitDeclaration;
using Cnss.Cotisation.Domain.Aggregats;
using Cnss.Cotisation.Domain.Events;
using Cnss.Cotisation.Domain.Factories;
using Cnss.Cotisation.Domain.Repositories;
using FluentValidation;

namespace Cnss.Cotisation.Application.Tests;

public sealed class SubmitDeclarationHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Publish_And_Persist_Declaration()
    {
        var repository = new FakeDeclarationRepository();
        var handler = new SubmitDeclarationHandler(
            repository,
            new DeclarationFactory(),
            new SubmitDeclarationRequestValidator());

        var response = await handler.Handle(
            new SubmitDeclarationRequest(
                "EMP-0001",
                2026,
                4,
                [
                    new SubmitDeclarationItemRequest("SAL-0001", 1000m),
                    new SubmitDeclarationItemRequest("SAL-0002", 2000m)
                ]),
            CancellationToken.None);

        var persisted = Assert.IsType<Declaration>(repository.SavedDeclaration);
        Assert.True(persisted.IsPublished);
        Assert.Equal(2, persisted.Items.Count);
        Assert.Equal(150m, persisted.TotalAmount);

        var domainEvent = Assert.Single(persisted.DomainEvents);
        Assert.IsType<DeclarationPublishedEvent>(domainEvent);

        Assert.Equal(persisted.Identifier, response.DeclarationIdentifier);
        Assert.Equal("EMP-0001", response.EmployerIdentifier);
        Assert.Equal(2, response.ItemsCount);
        Assert.Equal(150m, response.TotalAmount);
    }

    [Fact]
    public async Task Handle_Should_Throw_ValidationException_When_Request_Is_Invalid()
    {
        var repository = new FakeDeclarationRepository();
        var handler = new SubmitDeclarationHandler(
            repository,
            new DeclarationFactory(),
            new SubmitDeclarationRequestValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(
                new SubmitDeclarationRequest(
                    string.Empty,
                    1999,
                    13,
                    []),
                CancellationToken.None));
    }

    private sealed class FakeDeclarationRepository : IDeclarationRepository
    {
        public Declaration? SavedDeclaration { get; private set; }

        public Task AddAsync(Declaration declaration, CancellationToken cancellationToken = default)
        {
            SavedDeclaration = declaration;
            return Task.CompletedTask;
        }

        public Task<Declaration?> GetAsync(string declarationIdentifier, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(SavedDeclaration);
        }
    }
}
