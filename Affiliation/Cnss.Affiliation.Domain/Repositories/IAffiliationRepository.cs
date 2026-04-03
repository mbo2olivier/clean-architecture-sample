using Cnss.Affiliation.Domain.Aggregats;

namespace Cnss.Affiliation.Domain.Repositories;

public interface IAffiliationRepository
{
    Task AddEmployerAsync(Employer employer, CancellationToken cancellationToken = default);

    Task<Employer?> GetEmployerAsync(string employerIdentifier, CancellationToken cancellationToken = default);

    Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeAsync(string employeeIdentifier, CancellationToken cancellationToken = default);
}
