using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryMangement.Controllers
{
    // Not making the controller as the ApiController since i want to return a view for creating new books , So not inheriting form ControlBase .
    public class BooksController : Controller
    {        
        private readonly LibraryMangementContext _bookContext;

        public BooksController(LibraryMangementContext context)
        {
            _bookContext = context;
        }
        //A sample razor view for submitting entries
        [HttpGet("api/Books/Create")]
        public ActionResult CreateBooks()
        {
            return View();
        }

        [HttpPost("api/Books/Create")]
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
                return Ok("Added Successsfully");
            }
            return NoContent();
        }

        [HttpGet("api/Books/List/{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookContext.books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok("Fetched the book details");
        }     

       //Using HttpPatch instead of Put allowing any single field update to be possible
        [HttpPatch("api/Books/Update/{id}")] 
        public async Task<IActionResult> Update(int id,[FromBody] Books book)
        {
            // using this because currently we are not passing the id in the request body  and to check the body is null or not.
            if (book != null)
            {
                book.id = id;
            }             
            var existingBook = await _bookContext.books.FindAsync(id);

            if (existingBook == null)
            {
                return BadRequest($"Book with ID {id} not found.");
            }
            if (book.total_copies == 0)
            {
                book.total_copies = existingBook.total_copies;
            }
            foreach (var property in typeof(Books).GetProperties())
            {
                var updatedValue = property.GetValue(book);
                if (updatedValue != null)
                {
                    if (property.Name == "Price" && (decimal)updatedValue < 0)
                    {
                        return BadRequest("Price cannot be negative.");
                    }
                    property.SetValue(existingBook, updatedValue);
                }
            }
            try
            {
                await _bookContext.SaveChangesAsync();
                return Ok($"Book with ID {id} updated successfully.");
            }
            catch
            {
                return BadRequest($"Failed to update the book with ID {id}.");
            }
        }
                                
        [HttpDelete("api/Books/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = BookExists(id);           
            if (exists)
            {
                var book = await _bookContext.books.FindAsync(id);
                _bookContext.books.Remove(book);
                await _bookContext.SaveChangesAsync();
                return Ok("Deleted Succesfully");
            }
            else return NotFound();
        }
        private bool BookExists(int id)
        {
            return _bookContext.books.Any(e => e.id == id);
        }
    }


}
