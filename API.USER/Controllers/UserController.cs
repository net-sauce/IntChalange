using API.USER.Command;
using API.USER.Entities;
using API.USER.Query;
using COMMON.REPO.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using System;
using System.Threading.Tasks;

namespace API.USER.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger; 

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            _logger.LogInformation("In GetAll: Fetching all users.");
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery());
                _logger.LogInformation("In GetAll: Successfully fetched users. Count: {Count}", users?.Count() ?? 0);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In GetAll: An error occurred while fetching users.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("/{id}")]
        public async Task<ActionResult> Get(int id)
        {
            _logger.LogInformation("In Get: Fetching user by ID {ID}.", id);
            try
            {
                var user = await _mediator.Send(new GetUsersByIdQuery(id));
                if (user == null)
                {
                    _logger.LogWarning("In Get: No user found with ID {ID}.", id);
                    return NotFound($"User with ID {id} not found.");
                }

                _logger.LogInformation("In Get: Successfully fetched user with ID {ID}.", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In Get: An error occurred while fetching user with ID {ID}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        public async Task<ActionResult> AddNewUser([FromBody] UserEntity user)
        {
            _logger.LogInformation("In AddNewUser: Adding a new user.");
            try
            {
                if (await _mediator.Send(new AddNewUserCommand(user)))
                {
                    _logger.LogInformation("In AddNewUser: Successfully added a new user.");
                    return Ok();
                }

                _logger.LogWarning("In AddNewUser: Could not create the user.");
                return BadRequest("Could Not Create User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In AddNewUser: An error occurred while adding a new user.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateUser([FromBody] UserEntity user)
        {
            _logger.LogInformation("In UpdateUser: Updating user with ID {ID}.", user.UserID);
            try
            {
                var oldState = await _mediator.Send(new GetUsersByIdQuery(user.UserID));
                if (oldState == null)
                {
                    _logger.LogWarning("In UpdateUser: User with ID {ID} not found.", user.UserID);
                    return NotFound($"User with ID {user.UserID} not found.");
                }

                if (await _mediator.Send(new AddNewUserCommand(user)))
                {
                    _logger.LogInformation("In UpdateUser: Successfully updated user with ID {ID}.", user.UserID);
                    return Ok();
                }

                _logger.LogWarning("In UpdateUser: Could not update user with ID {ID}.", user.UserID);
                return BadRequest("User Not Updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In UpdateUser: An error occurred while updating user with ID {ID}.", user.UserID);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}