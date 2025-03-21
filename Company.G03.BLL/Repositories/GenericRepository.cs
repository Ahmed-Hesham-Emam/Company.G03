using Company.G03.BLL.Interfaces;
using Company.G03.DAL.Data.Contexts;
using Company.G03.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.BLL.Repositories
    {
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
        {
        private readonly CompanyDbContext _context;

        public GenericRepository(CompanyDbContext context)
            {
            _context = context;
            }


        public int Add(T model)
            {
            _context.Set<T>().Add(model);
            return _context.SaveChanges();
            }

        public int Delete(T model)
            {
            _context.Set<T>().Remove(model);
            return _context.SaveChanges();
            }

        public T? Get(int id)
            {
            if (typeof(T) == typeof(Employee))
                {
                return _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id) as T;
                }
            return _context.Set<T>().Find(id);
            }

        public IEnumerable<T> GetAll()
            {
            if (typeof(T) == typeof(Employee))
                {
                return (IEnumerable<T>)_context.Employees.Include(e => e.Department).ToList();
                }
            return _context.Set<T>().ToList();
            }

        public List<T> GetByName(string name)
            {
            if (typeof(T) == typeof(Employee))
                {
                return _context.Employees
                    .Include(e => e.Department)
                    .Where(e => e.Name.Contains(name.ToLower()))
                    .Cast<T>()
                    .ToList();
                }
            else if (typeof(T) == typeof(Department))
                {
                return _context.Departments
                    .Include(d => d.Employees)
                    .Where(d => d.Name.Contains(name.ToLower()))
                    .Cast<T>()
                    .ToList();
                }
            return null;
            }

        public int Update(T model)
            {
            _context.Set<T>().Update(model);
            return _context.SaveChanges();
            }
        }
    }
