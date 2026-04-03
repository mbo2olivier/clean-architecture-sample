using Cnss.Cotisation.Application.SubmitDeclaration;
using Cnss.Cotisation.Domain.Aggregats;
using Cnss.Cotisation.Domain.Events;
using Cnss.Cotisation.Domain.Factories;
using Cnss.Cotisation.Domain.Repositories;
using Cnss.Shared.Application.GetEmployerDetails;
using Cnss.Shared.Application.GetEmployerEmployeesDetails;
using FluentValidation;
using MDiator;

namespace Cnss.Cotisation.Application.Tests;

public sealed class SubmitDeclarationHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Publish_And_Persist_Declaration()
    {
        var repository = new FakeDeclarationRepository();
        var mediator = new FakeMediator();
        var handler = new SubmitDeclarationHandler(
            repository,
            new DeclarationFactory(),
            mediator,
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

        Assert.Collection(
            mediator.Requests,
            request => Assert.IsType<GetEmployerDetailsRequest>(request),
            request => Assert.IsType<GetEmployerEmployeesDetailsRequest>(request));

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
        var mediator = new FakeMediator();
        var handler = new SubmitDeclarationHandler(
            repository,
            new DeclarationFactory(),
            mediator,
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

    [Fact]
    public async Task Handle_Should_Stop_When_Employer_Does_Not_Exist()
    {
        var repository = new FakeDeclarationRepository();
        var mediator = new FakeMediator
        {
            EmployerLookupException = new KeyNotFoundException("Employer not found.")
        };

        var handler = new SubmitDeclarationHandler(
            repository,
            new DeclarationFactory(),
            mediator,
            new SubmitDeclarationRequestValidator());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(
                new SubmitDeclarationRequest(
                    "EMP-UNKNOWN",
                    2026,
                    4,
                    [new SubmitDeclarationItemRequest("SAL-0001", 1000m)]),
                CancellationToken.None));

        Assert.Null(repository.SavedDeclaration);
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

    private sealed class FakeMediator : IMediator
    {
        public List<object> Requests { get; } = [];

        public Exception? EmployerLookupException { get; init; }

        public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken)
            where TEvent : IMDiatorEvent
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>(IMDiatorRequest<TResponse> request, CancellationToken cancellationToken)
        {
            Requests.Add(request);

            if (request is GetEmployerDetailsRequest)
            {
                if (EmployerLookupException is not null)
                {
                    throw EmployerLookupException;
                }

                return Task.FromResult((TResponse)(object)new GetEmployerDetailsResponse(
                    "EMP-0001",
                    "RCCM-001",
                    "ACME SARL",
                    ["SAL-0001", "SAL-0002"]));
            }

            if (request is GetEmployerEmployeesDetailsRequest employeeDetailsRequest)
            {
                var employees = employeeDetailsRequest.EmployeeIdentifiers
                    .Select(id => new EmployerEmployeeDetailsResponse(id, $"MAT-{id}", "John", "Doe"))
                    .ToArray();

                return Task.FromResult((TResponse)(object)new GetEmployerEmployeesDetailsResponse(
                    employeeDetailsRequest.EmployerIdentifier,
                    employees));
            }

            throw new InvalidOperationException($"Unsupported request type: {request.GetType().Name}");
        }
    }
}
