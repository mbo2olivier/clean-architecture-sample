using Cnss.Affiliation.Domain.Aggregats;
using Cnss.Affiliation.Domain.Repositories;
using Cnss.Affiliation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cnss.Affiliation.Infrastructure.Repositories;

public sealed class AffiliationRepository : IAffiliationRepository
{
    private readonly AffiliationDbContext _dbContext;

    public AffiliationRepository(AffiliationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddEmployerAsync(Employer employer, CancellationToken cancellationToken = default)
    {
        await _dbContext.Employers.AddAsync(MapEmployer(employer), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Employer?> GetEmployerAsync(string employerIdentifier, CancellationToken cancellationToken = default)
    {
        var employerRecord = await _dbContext.Employers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Identifier == employerIdentifier, cancellationToken);

        if (employerRecord is null)
        {
            return null;
        }

        var employeeRecords = await _dbContext.Employees
            .AsNoTracking()
            .Where(x => x.EmployerIdentifier == employerIdentifier)
            .ToListAsync(cancellationToken);

        var employees = employeeRecords
            .Select(MapEmployee)
            .ToArray();

        return Employer.Restore(
            employerRecord.Identifier,
            employerRecord.RegistrationNumber,
            employerRecord.CompanyName,
            employees);
    }

    public async Task UpdateEmployerAsync(Employer employer, CancellationToken cancellationToken = default)
    {
        var employerRecord = await _dbContext.Employers
            .SingleAsync(x => x.Identifier == employer.Identifier, cancellationToken);

        employerRecord.RegistrationNumber = employer.RegistrationNumber;
        employerRecord.CompanyName = employer.CompanyName;
        employerRecord.EmployeeIdentifiers = employer.EmployeeIdentifiers.ToArray();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _dbContext.Employees.AddAsync(MapEmployee(employee), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Employee?> GetEmployeeAsync(string employeeIdentifier, CancellationToken cancellationToken = default)
    {
        var employeeRecord = await _dbContext.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Identifier == employeeIdentifier, cancellationToken);

        return employeeRecord is null ? null : MapEmployee(employeeRecord);
    }

    private static AffiliationEmployerRecord MapEmployer(Employer employer)
    {
        return new AffiliationEmployerRecord
        {
            Identifier = employer.Identifier,
            RegistrationNumber = employer.RegistrationNumber,
            CompanyName = employer.CompanyName,
            EmployeeIdentifiers = employer.EmployeeIdentifiers.ToArray()
        };
    }

    private static AffiliationEmployeeRecord MapEmployee(Employee employee)
    {
        return new AffiliationEmployeeRecord
        {
            Identifier = employee.Identifier,
            RegistrationNumber = employee.RegistrationNumber,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EmployerIdentifier = employee.EmployerIdentifier
        };
    }

    private static Employee MapEmployee(AffiliationEmployeeRecord employeeRecord)
    {
        return Employee.Restore(
            employeeRecord.Identifier,
            employeeRecord.RegistrationNumber,
            employeeRecord.FirstName,
            employeeRecord.LastName,
            employeeRecord.EmployerIdentifier);
    }
}
