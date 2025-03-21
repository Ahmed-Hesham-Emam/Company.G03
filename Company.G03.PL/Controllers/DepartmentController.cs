using AutoMapper;
using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Company.G03.PL.Controllers
    {
    public class DepartmentController : Controller
        {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentController(IDepartmentRepository departmentRepository, IMapper mapper)
            {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            }


        [HttpGet]
        public IActionResult Index(string? Search)
            {
            IEnumerable<Department> department;
            if (string.IsNullOrEmpty(Search))
                {
                department = _departmentRepository.GetAll();
                }
            else
                {
                department = _departmentRepository.GetByName(Search);
                }
            return View(department);
            }


        [HttpGet]
        public IActionResult Create()
            {
            return View();
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateDepartmentDto model)
            {
            if (ModelState.IsValid) //server side validation
                {
                var Department = new Department
                    {
                    Code = model.Code,
                    Name = model.Name,
                    CreatedAt = model.CreatedAt
                    };
                //var Department = _mapper.Map<CreateDepartmentDto, Department>(model);
                var count = _departmentRepository.Add(Department);

                if (count > 0)
                    {
                    TempData["Message"] = "Department Added Successfully";
                    return RedirectToAction(nameof(Index));
                    }
                }
            return View(model);
            }


        [HttpGet]
        public IActionResult Details(int? id, string viewName = "Details")
            {
            if (id == null)
                {
                return NotFound();
                }
            var department = _departmentRepository.Get(id.Value);
            if (department == null)
                {
                return NotFound();
                }
            return View(viewName, department);
            }


        [HttpGet]
        public IActionResult Edit(int? id)
            {
            if (id == null) return BadRequest();

            var Department = _departmentRepository.Get(id.Value);
            if (Department is null) return NotFound(new { StatusCode = 404, message = $"Department with ID: {id} is not found" });

            //var DepartmentDto = new CreateDepartmentDto()
            //    {
            //    Code = Department.Code,
            //    Name = Department.Name,
            //    CreatedAt = Department.CreatedAt
            //    };

            var DepartmentDto = _mapper.Map<CreateDepartmentDto>(Department);
            return View(DepartmentDto);
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, CreateDepartmentDto model)
            {
            if (ModelState.IsValid)
                {
                //var Department = new Department()
                //    {
                //    Id = id,
                //    Code = model.Code,
                //    Name = model.Name,
                //    CreatedAt = model.CreatedAt
                //    };
                var Department = _mapper.Map<Department>(model);
                var count = _departmentRepository.Update(Department);
                if (count > 0)
                    {
                    return RedirectToAction(nameof(Index));
                    }
                }
            return View(model);
            }


        [HttpGet]
        public IActionResult Delete(int? id)
            {
            if (id == null)
                {
                return NotFound();
                }
            var department = _departmentRepository.Get(id.Value);
            if (department == null)
                {
                return NotFound();
                }
            _departmentRepository.Delete(department);
            return RedirectToAction(nameof(Index));
            }

        }
    }

