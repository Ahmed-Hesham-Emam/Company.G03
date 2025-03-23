﻿using AutoMapper;
using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Company.G03.PL.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Company.G03.PL.Controllers
    {
    public class EmployeeController : Controller
        {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
            {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            }


        public async Task<IActionResult> Index(string? Search)
            {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(Search))
                {
                employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
                }
            else
                {
                employees = await _unitOfWork.EmployeeRepository.GetByNameAsync(Search);
                }
            return View(employees);
            }



        [HttpGet]
        public async Task<IActionResult> Create()
            {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["departments"] = departments;
            return View();
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeDto model)
            {
            if (ModelState.IsValid) //server side validation
                {

                if (model.Image is not null)
                    {
                    model.ImageName = AttachmentSettings.UploadFile(model.Image, "Imgs"); // Save the image to the server
                    }

                var Employee = _mapper.Map<CreateEmployeeDto, Employee>(model);
                await _unitOfWork.EmployeeRepository.AddAsync(Employee);
                var count = await _unitOfWork.CompleteAsync();

                if (count > 0)
                    {
                    TempData["Message"] = "Employee Added Successfully";
                    return RedirectToAction(nameof(Index));
                    }

                }
            return View(model);
            }


        [HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
            {
            if (id is null)
                {
                return NotFound();
                }

            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);
            if (employee == null)
                {
                return NotFound();
                }
            return View(viewName, employee);
            }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
            {
            if (id is null)
                {
                return NotFound(new { StatusCode = 404, message = $"Employee with ID: {id} is not found" });
                }

            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["departments"] = departments;

            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);
            if (employee is null) return NotFound(new { StatusCode = 404, message = $"Employee with ID: {id} is not found" });

            var EmployeeDto = _mapper.Map<CreateEmployeeDto>(employee);
            return View(EmployeeDto);
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, CreateEmployeeDto model)
            {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["departments"] = departments;
            if (ModelState.IsValid)
                {
                if (model.ImageName is not null && model.Image is not null)
                    {
                    AttachmentSettings.DeleteFile("Imgs", model.ImageName);
                    }
                if (model.Image is not null)
                    {
                    model.ImageName = AttachmentSettings.UploadFile(model.Image, "Imgs");
                    }

                var Employee = _mapper.Map<Employee>(model);
                _unitOfWork.EmployeeRepository.Update(Employee);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                    {
                    return RedirectToAction(nameof(Index));
                    }
                }
            return View(model);
            }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
            {
            if (id is null)
                {
                return NotFound();
                }
            var employee = await _unitOfWork.EmployeeRepository.GetAsync(id.Value);
            if (employee == null)
                {
                return NotFound();
                }
            _unitOfWork.EmployeeRepository.Delete(employee);
            await _unitOfWork.CompleteAsync();
            if (employee.ImageName is not null)
                {
                AttachmentSettings.DeleteFile("Imgs", employee.ImageName);
                }
            return RedirectToAction(nameof(Index));
            }
        }
    }
