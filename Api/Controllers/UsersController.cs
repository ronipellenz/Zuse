using Api.Models.Request;
using Api.Models.Response;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Persistance.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("users")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public UsersController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRequest request)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString().Substring(0, 8),
                Name = request.Name,
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                Status = UserStatusEnum.CREATED.ToString()
            };

            await _dbContext.SaveUserAsync(user);

            var result = new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email, Username = user.Username };

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var user = await _dbContext.GetByIdAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _dbContext.GetUsersListAsync();

            var response = users.Select(x => new UserResponse { Id = x.Id, Name = x.Name, Email = x.Email, Username = x.Username });

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUserById(string id)
        {
            var user = await _dbContext.GetByIdAsync(id);

            if (user is null)
            {
                return BadRequest();
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Username = user.Username
            };

            return Accepted(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string id)
        {
            await _dbContext.DeleteUserAsync(id);

            return Accepted();
        }
    }
}
