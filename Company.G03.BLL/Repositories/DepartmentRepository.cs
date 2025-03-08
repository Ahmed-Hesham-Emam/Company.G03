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
    public class DepartmentRepository : IDepartmentRepository
        {

        private readonly CompanyDbContext _context;

        public DepartmentRepository(CompanyDbContext context)
            {
            _context = context;
            }

        public int Add(Department department)
            {
            _context.Departments.Add(department);
            return _context.SaveChanges();
            }

        public int Delete(Department department)
            {
            _context.Departments.Remove(department);
            return _context.SaveChanges();
            }

        public Department? Get(int id)
            {
            return _context.Departments.Find(id);
            }

        public IEnumerable<Department> GetAll()
            {
            return _context.Departments.ToList();
            }

        public int Update(Department department)
            {
            _context.Departments.Update(department);
            return _context.SaveChanges();
            }
        }
    }
