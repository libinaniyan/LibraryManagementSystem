using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly LibraryMangementContext _memberContext;

        public MembersController(LibraryMangementContext context)
        {
            _memberContext = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMembers(Members members)
        {
            if (ModelState.IsValid)
            {
                _memberContext.Add(members);
                await _memberContext.SaveChangesAsync();
                return Ok("Added Successsfully");
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembers(int id)
        {          
            var members = await _memberContext.members.FindAsync(id);
            if (members == null)
            {
                return NotFound("Member not found");
            }
            return Ok("Member details fetched successfully");
        }     

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMembers(int id, [FromBody] Members members)
        {
            if(members != null)
            {
               members.Id = id;
            }         
            var existingMember = MembersExists(id);

            if (existingMember)
            {
                _memberContext.Entry(members).State = EntityState.Modified;
                await _memberContext.SaveChangesAsync();
                return Ok("Member details updated successfully");
            }
            return BadRequest($"Member with ID {id} not found.");
        }   

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembers(int id)
        {
            var exists= MembersExists(id);
            if (exists)
            {
                var member = await _memberContext.members.FindAsync(id);
                _memberContext.members.Remove(member);
                await _memberContext.SaveChangesAsync();
                return Ok("Deleted Successfully");
            }
            return NotFound("Member doesnt exists");           
        }
        private bool MembersExists(int id)
        {
            return _memberContext.members.Any(e => e.Id == id);
        }
    }
}
