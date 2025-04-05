using AutoMapper;
using Company.G03.BLL.Interfaces;
using Company.G03.BLL.Repositories;
using Company.G03.DAL.Entities;
using Company.G03.PL.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Company.G03.PL.Controllers
    {

    [Authorize]
    public class DepartmentController : Controller
        {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        public DepartmentController(IUnitOfWork unitOfWork, IMapper mapper)
            {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            }


        [HttpGet]
        public async Task<IActionResult> Index(string? Search)
            {
            IEnumerable<Department> department;
            if (string.IsNullOrEmpty(Search))
                {
                department = await _unitOfWork.DepartmentRepository.GetAllAsync();
                }
            else
                {
                department = await _unitOfWork.DepartmentRepository.GetByNameAsync(Search);
                }
            return View(department);
            }

        public async Task<IActionResult> Search(string Search)
            {
            var department = await _unitOfWork.DepartmentRepository.GetByNameAsync(Search);

            return PartialView("DepartmentPartialView/_DepartmentsTablePartialView", department);
            }

        #region Create

        [HttpGet]
        public IActionResult Create()
            {
            return View();
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentDto model)
            {
            if (ModelState.IsValid) //server side validation
                {
                var Department = _mapper.Map<CreateDepartmentDto, Department>(model);
                await _unitOfWork.DepartmentRepository.AddAsync(Department);
                var count = await _unitOfWork.CompleteAsync();

                if (count > 0)
                    {
                    TempData["Message"] = "Department Added Successfully";
                    return RedirectToAction(nameof(Index));
                    }
                }
            return View(model);
            }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
            {
            if (id == null)
                {
                return NotFound();
                }
            var department = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if (department == null)
                {
                return NotFound();
                }
            return View(viewName, department);
            }

        #endregion

        #region Update

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null) return BadRequest();

            var Department = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if (Department is null) return NotFound(new { StatusCode = 404, message = $"Department with ID: {id} is not found" });

            var DepartmentDto = _mapper.Map<CreateDepartmentDto>(Department);

            return View(DepartmentDto);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, CreateDepartmentDto model)
            {
            if (ModelState.IsValid)
                {
                var Department = _mapper.Map<Department>(model);
                _unitOfWork.DepartmentRepository.Update(Department);
                var count = await _unitOfWork.CompleteAsync();
                if (count > 0)
                    {
                    return RedirectToAction(nameof(Index));
                    }
                }
            return View(model);
            }

        #endregion

        #region Delete

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
            {
            if (id == null) return BadRequest();
            var department = await _unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if (department == null) return NotFound();
            try
                {
                _unitOfWork.DepartmentRepository.Delete(department);
                var count = await _unitOfWork.CompleteAsync();

                if (count > 0)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Something went wrong");
                }
            catch (Exception ex)
                {
                return BadRequest(ex.Message);
                }
            return RedirectToAction(nameof(Index));
            }

        #endregion
        }
    }

