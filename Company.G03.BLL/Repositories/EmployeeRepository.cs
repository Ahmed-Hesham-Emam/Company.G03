using Company.G03.BLL.Interfaces;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.BLL.Repositories
    {
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
        {
        public EmployeeRepository(CompanyDbContext context) : base(context)
            {

            }
        //private readonly CompanyDbContext _context;
        //public EmployeeRepository(CompanyDbContext context)
        //    {
        //    _context = context;
        //    }
        //public int Add(Employee employee)
        //    {
        //    _context.Employees.Add(employee);
        //    return _context.SaveChanges();
        //    }

        //public int Delete(Employee employee)
        //    {
        //    _context.Employees.Remove(employee);
        //    return _context.SaveChanges();
        //    }

        //public Employee? Get(int id)
        //    {
        //    return _context.Employees.Find(id);
        //    }

        //public IEnumerable<Employee> GetAll()
        //    {
        //    return _context.Employees.ToList();
        //    }

        //public int Update(Employee employee)
        //    {
        //    _context.Employees.Update(employee);
        //    return _context.SaveChanges();
        //    }
        }
    }
