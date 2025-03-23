using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.BLL
    {
    public class UnitOfWork : IUnitOfWork
        {
        private readonly CompanyDbContext _context;
        private readonly Lazy<IEmployeeRepository> _employeeRepository;
        private readonly Lazy<IDepartmentRepository> _departmentRepository;

        //public IEmployeeRepository EmployeeRepository { get; }
        //public IDepartmentRepository DepartmentRepository { get; }
        public IEmployeeRepository EmployeeRepository => _employeeRepository.Value;
        public IDepartmentRepository DepartmentRepository => _departmentRepository.Value;

        public UnitOfWork(CompanyDbContext context)
            {

            _context = context;

            _employeeRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(_context));
            _departmentRepository = new Lazy<IDepartmentRepository>(() => new DepartmentRepository(_context));

            }

        public async Task<int> CompleteAsync()
            {
            return await _context.SaveChangesAsync();
            }

        public async ValueTask DisposeAsync()
            {
            await _context.DisposeAsync();
            }
        }
    }