using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repo;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public EmployeesController(IRepositoryManager repo, ILoggerManager logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var company = _repo.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesnt exist in the database.");
                return NotFound();
            }
            var employees = _repo.Employee.GetEmployees(companyId, trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

            return Ok(employeesDto);
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repo.Company.GetCompany(companyId, trackChanges: false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesnt exist in the database.");
                return NotFound();
            }

            var employee = _repo.Employee.GetEmployee(companyId, id, trackChanges: false);

            if (employee == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesnt exit in the database.");
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return Ok(employeeDto);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee == null)
            {
                _logger.LogError("EmployeeForCreationDto object sent form client is null.");
                return BadRequest("EmployeeForCreationDto object is null.");
            }

            var company = _repo.Company.GetCompany(companyId, trackChanges: false);
            
            if (company == null)
            {
                _logger.LogInfo($"Company with id: { companyId } doesnt exit in the database.");
                return NotFound();
            }
            
            var employeeEntity = _mapper.Map<Employee>(employee);
            
            _repo.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            _repo.Save();
            
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }
    }
}
