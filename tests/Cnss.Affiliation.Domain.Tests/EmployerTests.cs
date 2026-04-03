using Cnss.Affiliation.Domain.Aggregats;
using Cnss.Affiliation.Domain.Events;

namespace Cnss.Affiliation.Domain.Tests;

public sealed class EmployerTests
{
    [Fact]
    public void Affiliate_Should_Raise_EmployerAffiliatedEvent()
    {
        var employer = Employer.Affiliate("EMP-0001", "RCCM-001", "ACME SARL");

        var domainEvent = Assert.Single(employer.DomainEvents);
        var affiliatedEvent = Assert.IsType<EmployerAffiliatedEvent>(domainEvent);

        Assert.Equal("EMP-0001", affiliatedEvent.EmployerIdentifier);
        Assert.Equal("RCCM-001", affiliatedEvent.RegistrationNumber);
        Assert.Equal("ACME SARL", affiliatedEvent.CompanyName);
    }

    [Fact]
    public void AttachEmployee_Should_Raise_EmployeeAttachedEvent_And_Associate_Employee()
    {
        var employer = Employer.Affiliate("EMP-0001", "RCCM-001", "ACME SARL");
        employer.ClearDomainEvents();

        var employee = Employee.Create("SAL-0001", "MAT-001", "John", "Doe");

        employer.AttachEmployee(employee);

        var domainEvent = Assert.Single(employer.DomainEvents);
        var attachedEvent = Assert.IsType<EmployeeAttachedToEmployeeEvent>(domainEvent);

        Assert.Equal("EMP-0001", attachedEvent.EmployerIdentifier);
        Assert.Equal("SAL-0001", attachedEvent.EmployeeIdentifier);
        Assert.Equal("EMP-0001", employee.EmployerIdentifier);
        Assert.Contains("SAL-0001", employer.EmployeeIdentifiers);
    }
}
