﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager service) => _service = service;
        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
       
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
[FromQuery] EmployeeParameters employeeParameters)
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);
            var result = await _service.EmployeeService.GetEmployeesAsync(companyId,
            linkParams, trackChanges: false);
            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) :
                Ok(result.linkResponse.ShapedEntities);
        }
    

        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task <IActionResult> GetEmployeeForCompanyAsync(Guid companyId, Guid id)
        {
            var employee = await _service.EmployeeService.GetEmployeeAsync(companyId, id,
            trackChanges: false);
            return Ok(employee);
        }
        [HttpPost]

        public async Task< IActionResult > CreateEmployeeForCompanyAsync(Guid companyId, [FromBody]
            EmployeeForCreationDto employee)
        {
            if (employee is null)
                return BadRequest("EmployeeForCreationDto object is null");
            var employeeToReturn =
           await  _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges:
            false);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                id =
            employeeToReturn.Id
            },
            employeeToReturn);
        }
        [HttpDelete("{id:guid}")]
        public async Task <IActionResult> DeleteEmployeeForCompanyAsync(Guid companyId, Guid id)
        {
            try
            {
               await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges:
            false);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound("id not found "+ex.Message);
            }
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEmployeeForCompanyASync(Guid companyId, Guid id,
[FromBody] EmployeeForUpdateDto employee)
        {
            if (employee is null)
                return BadRequest("EmployeeForUpdateDto object is null");
            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee,
            compTrackChanges: false, empTrackChanges: true);
            return NoContent();
        }
        [HttpPatch("{id:guid}")]
        public async Task< IActionResult> PartiallyUpdateEmployeeForCompanyAsync(Guid companyId, Guid id,
[FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");
            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id,
            compTrackChanges: false,
            empTrackChanges: true);
            patchDoc.ApplyTo(result.employeeToPatch);
            _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch,
            result.employeeEntity);
            return NoContent();
        }


    }

}
