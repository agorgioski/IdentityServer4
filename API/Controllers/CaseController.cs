using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Models;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize("user")]
    public class CaseController : ControllerBase
    {
        private readonly ICaseRepository _repo;
        // private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public CaseController(ICaseRepository repo) 
        {
            _repo = repo;
            // , IActionDescriptorCollectionProvider actionDescriptor
            // _actionDescriptorCollectionProvider = actionDescriptor;
        }

        // GET: api/case
        [HttpGet]
        public async Task<ActionResult<List<Case>>> Get() 
        {
            List<Case> cases = await _repo.GetAllCases();

            if (cases == null) 
            {
                return NotFound();
            }
            return cases;


            // TODO: Move this for possible future use
            // StringBuilder sb = new StringBuilder();

            // foreach (ActionDescriptor ad in _actionDescriptorCollectionProvider.ActionDescriptors.Items)
            // {
            //     var action = Url.Action(new UrlActionContext()
            //     {
            //         Action = ad.RouteValues["action"],
            //         Controller = ad.RouteValues["controller"],
            //         Values = ad.RouteValues
            //     });

            //     sb.AppendLine(action).AppendLine().AppendLine();
            // }

            // return Ok(sb.ToString());
        }

        // GET: api/case/casekind/{type}
        [Route("/api/case/casekind/{type}")]
        [HttpGet("{type}")]
        public async Task<ActionResult<CaseKind>> CaseKind(string type) {
            CaseKind kind = await _repo.GetCaseKindByType(type);
            if (kind == null) 
            {
                return NotFound();
            }
            return kind;
        }

        // GET: api/case/{id}

        [HttpGet("{id}")]
        public async Task<ActionResult<Case>> GetCaseById(long id) 
        {
            Case @case;
            try 
            {
                @case = await _repo.GetCaseById(id);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }

            return @case;
        }

        // POST api/case/{id}
        [HttpPost]
        public async Task<ActionResult<Case>> PostCase(Case @case) 
        {
            try 
            {
                await _repo.InsertUpdateCase(@case, -1);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }

            return CreatedAtAction(nameof(GetCaseById), new { id = @case.Id}, @case);
        }

        // DELETE api/case/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCase(long id) 
        {
            try 
            {
                await _repo.DeleteCase(id);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }   
            return NoContent();
        }

        // PATCH api/case/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult<Case>> PatchCase(Case @case, long id) 
        {
            try 
            {
                await _repo.InsertUpdateCase(@case, id);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }
            
            return CreatedAtAction(nameof(GetCaseById), new { id = @case.Id}, @case);
        }
    }
}