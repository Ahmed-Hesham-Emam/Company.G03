using AutoMapper;
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
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IMapper mapper)
            {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            }
        public IActionResult Index(string? Search)
            {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(Search))
                {
                employees = _employeeRepository.GetAll();
                }
            else
                {
                employees = _employeeRepository.GetByName(Search);
                }
            return View(employees);
            }



        [HttpGet]
        public IActionResult Create()
            {
            var departments = _departmentRepository.GetAll();
            ViewData["departments"] = departments;
            return View();
            }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateEmployeeDto model)
            {
            if (ModelState.IsValid) //server side validation
                {
                //var Employee = new Employee
                //    {
                //    Name = model.Name,
                //    Address = model.Address,
                //    Age = model.Age,
                //    Email = model.Email,
                //    Phone = model.Phone,
                //    Salary = model.Salary,
                //    IsActive = model.IsActive,
                //    IsDeleted = model.IsDeleted,
                //    HiringDate = model.HiringDate,
                //    CreatedAt = model.CreatedAt,
                //    DepartmentId = model.DepartmentId
                //}
                ;
                var Employee = _mapper.Map<CreateEmployeeDto, Employee>(model);
                var count = _employeeRepository.Add(Employee);

                if (count > 0)
                    {
                    TempData["Message"] = "Employee Added Successfully";
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
            var departments = _departmentRepository.GetAll();
            ViewData["departments"] = departments;

            var employee = _employeeRepository.Get(id.Value);
            if (employee is null) return NotFound(new { StatusCode = 404, message = $"Employee with ID: {id} is not found" });
            //var EmployeeDto = new CreateEmployeeDto()
            //    {
            //    Name = employee.Name,
            //    Address = employee.Address,
            //    Age = employee.Age,
            //    Email = employee.Email,
            //    Phone = employee.Phone,
            //    Salary = employee.Salary,
            //    IsActive = employee.IsActive,
            //    IsDeleted = employee.IsDeleted,
            //    HiringDate = employee.HiringDate,
            //    CreatedAt = employee.CreatedAt,
            //    DepartmentId = employee.DepartmentId
            //    };
            var EmployeeDto = _mapper.Map<CreateEmployeeDto>(employee);
            return View(EmployeeDto);
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, Employee model)
            {
            var departments = _departmentRepository.GetAll();
            ViewData["departments"] = departments;
            if (ModelState.IsValid)
                {
                //var Employee = new Employee()
                //    {
                //    Name = model.Name,
                //    Address = model.Address,
                //    Age = model.Age,
                //    Email = model.Email,
                //    Phone = model.Phone,
                //    Salary = model.Salary,
                //    IsActive = model.IsActive,
                //    IsDeleted = model.IsDeleted,
                //    HiringDate = model.HiringDate,
                //    CreatedAt = model.CreatedAt,
                //    DepartmentId = model.DepartmentId
                //    };
                var Employee = _mapper.Map<Employee>(model);
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
