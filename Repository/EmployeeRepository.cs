﻿using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Shared.RequestFeatures;





namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository

    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId),
 trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
              .Search(employeeParameters.SearchTerm)
             .OrderBy(e => e.Age)
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .Sort(employeeParameters.OrderBy)

            .ToListAsync();
            var count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges
            ).CountAsync();
            return new PagedList<Employee>(employees, count,
            employeeParameters.PageNumber, employeeParameters.PageSize);
        }


        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
                 await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
                .SingleOrDefaultAsync();

        public async Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }
        public async Task DeleteEmployeeAsync(Employee employee) => Delete(employee);
    }
}