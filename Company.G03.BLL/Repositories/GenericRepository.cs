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


        public async Task AddAsync(T model)
            {
            await _context.Set<T>().AddAsync(model);
            //return _context.SaveChanges();
            }

        public void Delete(T model)
            {
            _context.Set<T>().Remove(model);
            //return _context.SaveChanges();
            }

        public async Task<T?> GetAsync(int id)
            {
            if (typeof(T) == typeof(Employee))
                {
                return await _context.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id) as T;
                }
            return _context.Set<T>().Find(id);
            }

        public async Task<IEnumerable<T>> GetAllAsync()
            {
            if (typeof(T) == typeof(Employee))
                {
                return (IEnumerable<T>)await _context.Employees.Include(e => e.Department).ToListAsync();
                }
            return await _context.Set<T>().ToListAsync();
            }

        public async Task<List<T>> GetByNameAsync(string name)
            {
            if (typeof(T) == typeof(Employee))
                {
                return await _context.Employees
                    .Include(e => e.Department)
                    .Where(e => e.Name.Contains(name.ToLower()))
                    .Cast<T>()
                    .ToListAsync();
                }
            else if (typeof(T) == typeof(Department))
                {
                return await _context.Departments
                    .Include(d => d.Employees)
                    .Where(d => d.Name.Contains(name.ToLower()))
                    .Cast<T>()
                    .ToListAsync();
                }
            return new List<T>();
            }

        public void Update(T model)
            {
            _context.Set<T>().Update(model);
            //return _context.SaveChanges();
            }

        }
    }
