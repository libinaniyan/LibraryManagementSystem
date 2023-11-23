using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    // Not making the controller as the ApiController since i want to return a view for creating new members , So not inheriting form ControlBase .
    public class MembersController : Controller
    {
        private readonly LibraryMangementContext _memberContext;

        public MembersController(LibraryMangementContext context)
        {
            _memberContext = context;
        }

        [HttpGet("api/Members/Create")]
        public IActionResult CreateMembers()
        {
            return View();
        }

        [HttpPost("api/Members/Create")]
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

        [HttpGet("api/Members/List")]
        public async Task<IActionResult> Details(int id)
        {          
            var members = await _memberContext.members
                .FirstOrDefaultAsync(m => m.Id == id);
            if (members == null)
            {
                return NotFound("Member not found");
            }
            return Ok("Member fetched successfully");
        }     

        [HttpPatch("api/Members/Edit")]
        public async Task<IActionResult> Update(int id, [FromBody] Members members)
        {
          if(members != null)
            {
               members.Id = id;
            }         
            var existingMember = await _memberContext.members.FindAsync(id);

            if (existingMember == null)
            {
                return BadRequest($"Member not found.");
            }
            foreach (var property in typeof(Members).GetProperties())
            {
                var updatedValue = property.GetValue(members);
                if (updatedValue != null)
                {
                    property.SetValue(existingMember, updatedValue);
                }
            }
            try
            {
                await _memberContext.SaveChangesAsync();
                return Ok($"Member details updated successfully.");
            }
            catch
            {
                return BadRequest($"Failed to update the Member.");
            }
        }   

      
        [HttpDelete("api/Members/Delete")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var exists= MembersExists(id);
            if (exists)
            {
                var member = await _memberContext.members.FindAsync(id);
                _memberContext.members.Remove(member);
                await _memberContext.SaveChangesAsync();
                return Ok("Deleted Successfully");
            }
            else return NotFound("Member doesnt exists");           
        }
        private bool MembersExists(int id)
        {
            return _memberContext.members.Any(e => e.Id == id);
        }
    }
}
