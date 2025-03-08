using Company.G03.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.BLL.Interfaces
    {
    public interface IDepartmentRepository
        {
        IEnumerable<Department> GetAll();

        Department? Get(int id);

        int Add(Department department);
        int Update(Department department);
        int Delete(Department department);

        }
    }
