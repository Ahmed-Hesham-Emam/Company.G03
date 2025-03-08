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

        public DepartmentController(IDepartmentRepository departmentRepository)
            {
            _departmentRepository = departmentRepository;
            }


        [HttpGet]
        public IActionResult Index()
            {
            var department = _departmentRepository.GetAll();
            return View(department);
            }


        [HttpGet]
        public IActionResult Create()
            {
            return View();
            }


        [HttpPost]
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
                var count = _departmentRepository.Add(Department);

                if (count > 0)
                    {
                    return RedirectToAction(nameof(Index));
                    }
                }
            return View(model);
            }

        [HttpGet]
        public IActionResult Details(int id, CreateDepartmentDto model)
            {
            var department = _departmentRepository.Get(id);
            if (department == null)
                {
                return NotFound();
                }
            return View(department);
            }

        [HttpGet]
        public IActionResult Edit(int id, EditDepartmentDto model)
            {
            var Department = _departmentRepository.Get(id);
            model.Id = Department.Id;
            model.Code = Department.Code;
            model.Name = Department.Name;
            model.CreatedAt = Department.CreatedAt;

            //_departmentRepository.Update(Department);

            return View(model);
            }

        [HttpPost]
        public IActionResult Edit(int id, CreateDepartmentDto model)
            {
            var Department = _departmentRepository.Get(id);
            Department.Code = model.Code;
            Department.Name = model.Name;
            Department.CreatedAt = model.CreatedAt;
            _departmentRepository.Update(Department);
            return RedirectToAction(nameof(Index));
            }

        [HttpGet]
        public IActionResult Delete(int id)
            {
            var department = _departmentRepository.Get(id);
            _departmentRepository.Delete(department);
            return RedirectToAction(nameof(Index));
            }

        }
    }

