using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserStore _repo;

        public AdminController(IUserStore repo) 
        {
            _repo = repo;
        }

        // GET: api/admin
        [HttpGet]
        public async Task<ActionResult<List<AppUser>>> GetAllUsers() 
        {
            List<AppUser> users = await _repo.GetAllOrOneUsers(id: -1);

            if (users == null) 
            {
                return NotFound();
            }

            return users;
        }


        // GET: api/admin/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id) 
        {
            AppUser user;
            try 
            {
                user = (await _repo.GetAllOrOneUsers(id))[0];
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }

            return user;
        }

        // POST: api/admin
        [HttpPost]
        public async Task<ActionResult<AppUser>> PostUser(AppUser user) 
        {
            try 
            {
                await _repo.SaveAppUser(user);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id}, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id) 
        {
            try 
            {
                await _repo.DeleteUser(id);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }   
            return NoContent();
        }

    }
}