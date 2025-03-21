﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
namespace CompanyEmployees.Presentation.Controllers

{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetCompaniesAsync()
        {
            var companies = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }
        [HttpGet("{id:guid}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompanyAsync(Guid id)
        {            
            var company =await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }

        [HttpPost]
        public async Task <IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company is null)
                return BadRequest("CompanyForCreationDto object is null");
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id },
            createdCompany);
        }
        
        [HttpPost("collection")]
        public async Task< IActionResult> CreateCompanyCollection([FromBody]IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result =
            await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids },
            result.companies);
        }
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]

        public async Task <IActionResult> GetCompanyCollectionAsync([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)

        {

            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);

            return Ok(companies);

        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompanyAsync(Guid id)
        {
            _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        public async Task <IActionResult> UpdateCompanyAsync(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            if (company is null)
                return BadRequest("CompanyForUpdateDto object is null");
            _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
            return NoContent();
        }
      




    }

}

