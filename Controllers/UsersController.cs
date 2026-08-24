using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UserManagementApi.Models;

namespace UserManagementApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // Inject the logger through the constructor
        private readonly ILogger<UsersController> _logger;
                
        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        // Optimized: Changed List<T> to ConcurrentDictionary<int, T> for O(1) lookups and thread-safety
        private static readonly ConcurrentDictionary<int, User> _users = new ConcurrentDictionary<int, User>(
            new Dictionary<int, User>
            {
                {1, new User { Id = 1, FirstName = "Mike", LastName = "Jordan", Email = "mj@hotmail.com", IsActive = false }},
                {2, new User { Id = 2, FirstName = "Tee", LastName = "Up", Email = "tu@hotmail.com", IsActive = true}},
                {3, new User { Id = 3, FirstName = "Slam", LastName = "Dunk", Email = "sd@hotmail.com", IsActive = false}}
            }   
        );

        // Optimized: Thread-safe O(1) counter for ID generation
        private static int _nextId = 3;
        
        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                // .Values returns the collection in O(1)
                return Ok(_users.Values);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                return StatusCode(500, "An internal server error occurred while processing your request.");
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID. ID must be greater than zero.");

            try
            {
                // Optimized: O(1) dictionary lookup
                if (_users.TryGetValue(id, out User? match))
                {
                    return Ok(match);
                }
                
                return NotFound($"User with ID {id} not found.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user. ID: {UserId}.", id);
                return StatusCode(500, "An internal server error occurred while processing your request.");
            }
        }

        // POST: api/users/
        [HttpPost]
        public ActionResult<User> NewUser([FromBody] User? newUser)
        {
            if (newUser == null) return BadRequest("User data is null.");

            try
            {
                // Optimized: O(1) thread-safe ID generation instead of O(N) LINQ .Max()
                int newId = Interlocked.Increment(ref _nextId);
                newUser.Id = newId;

                // Optimized: O(1) insertion
                _users.TryAdd(newId, newUser);

                return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return StatusCode(500, "An internal server error occurred while processing your request");
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User? updatedUser)
        {
            if (id <= 0) return BadRequest("Invalid ID.");
            if (updatedUser == null) return BadRequest("User data is null.");

            // Validation: Ensure route ID matches body ID
            if (id != updatedUser.Id) return BadRequest("The ID in the URL must match the ID in the request body.");

            try
            {
                // Optimized: O(1) lookup
                if (!_users.TryGetValue(id, out User? match))
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Update the object in memory
                match.FirstName = updatedUser.FirstName;
                match.LastName = updatedUser.LastName;
                match.Email = updatedUser.Email;
                match.IsActive = updatedUser.IsActive;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user. ID: {UserId}.", id);
                return StatusCode(500, "An internal server error occurred while processing your request.");
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID");

            try
            {
                // Optimized: O(1) deletion
                if (!_users.TryRemove(id, out _))
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user. ID: {UserId}.", id);
                return StatusCode(500, "An internal server error occurred while processing your request.");
            }
        }
    }
}


