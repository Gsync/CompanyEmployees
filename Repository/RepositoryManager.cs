using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repoContext;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;
        public RepositoryManager(RepositoryContext repoContext)
        {
            _repoContext = repoContext;
        }
        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository == null)
                    _companyRepository = new CompanyRepository(_repoContext);
                return _companyRepository;
            }
        }

        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new EmployeeRepository(_repoContext);
                return _employeeRepository;
            }
        }

        public void Save() => _repoContext.SaveChanges();
    }
}
