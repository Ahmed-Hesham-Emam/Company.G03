using Company.G03.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.BLL.Interfaces
    {
    public interface IGenericRepository<T> where T : BaseEntity
        {
        IEnumerable<T> GetAll();
        T? Get(int id);
        int Add(T model);
        int Update(T model);
        int Delete(T model);
        List<T> GetByName(string name);
        }
    }
