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
        await _dbContext.Employers.AddAsync(employer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Employer?> GetEmployerAsync(string employerIdentifier, CancellationToken cancellationToken = default)
    {
        var employerRecord = await _dbContext.Employers
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == employerIdentifier, cancellationToken);

        if (employerRecord is null)
        {
            return null;
        }

        var employeeRecords = await _dbContext.Employees
            .AsNoTracking()
            .Where(x => x.EmployerIdentifier == employerIdentifier)
            .ToListAsync(cancellationToken);

        var employees = employeeRecords.ToArray();

        return Employer.Restore(employerRecord.Identifier, employerRecord.RegistrationNumber, employerRecord.CompanyName, employees);
    }

    public async Task UpdateEmployerAsync(Employer employer, CancellationToken cancellationToken = default)
    {
        _dbContext.Employers.Update(employer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Employee?> GetEmployeeAsync(string employeeIdentifier, CancellationToken cancellationToken = default)
    {
        var employeeRecord = await _dbContext.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == employeeIdentifier, cancellationToken);

        return employeeRecord;
    }
}
