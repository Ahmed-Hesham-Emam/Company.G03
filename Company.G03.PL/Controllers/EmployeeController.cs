using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Company.G03.PL.Controllers
    {
    public class EmployeeController : Controller
        {


        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
            {
            _employeeRepository = employeeRepository;
            }
        public IActionResult Index()
            {
            var employee = _employeeRepository.GetAll();
            return View(employee);
            }

        [HttpGet]
        public IActionResult Create()
            {
            return View();
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateEmployeeDto model)
            {
            if (ModelState.IsValid) //server side validation
                {
                var Employee = new Employee
                    {
                    Name = model.Name,
                    Address = model.Address,
                    Age = model.Age,
                    Email = model.Email,
                    Phone = model.Phone,
                    Salary = model.Salary,
                    IsActive = model.IsActive,
                    IsDeleted = model.IsDeleted,
                    HiringDate = model.HiringDate,
                    CreatedAt = model.CreatedAt
                    };
                var count = _employeeRepository.Add(Employee);

                if (count > 0)
                    {
                    return RedirectToAction(nameof(Index));
                    }
                }
            return View(model);
            }

        [HttpGet]
        public IActionResult Details(int? id, string viewName = "Details")
            {
            if (id is null)
                {
                return NotFound();
                }

            var employee = _employeeRepository.Get(id.Value);
            if (employee == null)
                {
                return NotFound();
                }
            return View(viewName, employee);
            }

        [HttpGet]
        public IActionResult Edit(int? id)
            {
            var employee = _employeeRepository.Get(id.Value);
            if (employee is null) return NotFound(new { StatusCode = 404, message = $"Department with ID: {id} is not found" });
            var EmployeeDto = new CreateEmployeeDto()
                {
                Name = employee.Name,
                Address = employee.Address,
                Age = employee.Age,
                Email = employee.Email,
                Phone = employee.Phone,
                Salary = employee.Salary,
                IsActive = employee.IsActive,
                IsDeleted = employee.IsDeleted,
                HiringDate = employee.HiringDate,
                CreatedAt = employee.CreatedAt
                };
            return View(EmployeeDto);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, Employee model)
            {
            if (ModelState.IsValid)
                {
                var Employee = new Employee()
                    {
                    Name = model.Name,
                    Address = model.Address,
                    Age = model.Age,
                    Email = model.Email,
                    Phone = model.Phone,
                    Salary = model.Salary,
                    IsActive = model.IsActive,
                    IsDeleted = model.IsDeleted,
                    HiringDate = model.HiringDate,
                    CreatedAt = model.CreatedAt
                    };

                var count = _employeeRepository.Update(Employee);
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
            if (id is null)
                {
                return NotFound();
                }
            var employee = _employeeRepository.Get(id.Value);
            if (employee == null)
                {
                return NotFound();
                }
            _employeeRepository.Delete(employee);
            return RedirectToAction(nameof(Index));
            }
        }
    }
