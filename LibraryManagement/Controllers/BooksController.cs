using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryMangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryMangementContext _bookContext;

        public BooksController(LibraryMangementContext context)
        {
            _bookContext = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooks(Books book)
        {
            if (book.available_copies < 0 || book.available_copies > book.total_copies)
            {
                return BadRequest("Invalid Value for Available or Total Copies");
            }
            if (ModelState.IsValid)
            {
                _bookContext.books.Add(book);
                await _bookContext.SaveChangesAsync();
                return Ok("Book created Successsfully");
            }
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooks(int id)
        {
            var book = await _bookContext.books.FindAsync(id);
            if (book == null)
            {
                return NotFound($"No record of book with id:{id}");
            }
            return Ok("Fetched the book details successfully.");
        }
    
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooks(int id, [FromBody] Books book)
        {
            //using this because currently we are not passing the id in the request body.
            if (book != null)
            {
                book.id = id;
            }
            var existingBook = BookExists(id);

            if (existingBook)
            {
                _bookContext.Entry(book).State = EntityState.Modified;
                await _bookContext.SaveChangesAsync();
                return Ok("Book details updated successfully");              
            }
            return BadRequest($"Book with ID {id} not found.");
        }           
                   
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooks(int id)
        {
            var exists = BookExists(id);           
            if (exists)
            {
                var book = await _bookContext.books.FindAsync(id);
                _bookContext.books.Remove(book);
                await _bookContext.SaveChangesAsync();
                return Ok("Deleted Succesfully");
            }
            else return NotFound($"No record of book with id:{id}");
        }
        private bool BookExists(int id)
        {
            return _bookContext.books.Any(e => e.id == id);
        }
       
    }   
}
