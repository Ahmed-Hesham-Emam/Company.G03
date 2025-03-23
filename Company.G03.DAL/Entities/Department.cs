using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.G03.DAL.Entities
    {
    public class Department : BaseEntity
        {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public required List<Employee> Employees { get; set; }
        }
    }
