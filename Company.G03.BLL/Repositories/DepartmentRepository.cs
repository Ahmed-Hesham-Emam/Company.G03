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
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
        {
        public DepartmentRepository(CompanyDbContext context) : base(context)
            {

            }

        }
    }
