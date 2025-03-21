using AutoMapper;
using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;

namespace Company.G03.PL.AutoMapper
    {
    public class DepartmentProfile : Profile
        {
        public DepartmentProfile()
            {
            CreateMap<CreateDepartmentDto, Department>().ReverseMap();
            }
        }
    }
