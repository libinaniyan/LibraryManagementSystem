using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class BorrowedBooksController : Controller
    {
        private readonly LibraryMangementContext _borrowedBookContext;

        public BorrowedBooksController(LibraryMangementContext context)
        {
            _borrowedBookContext = context;
        }
        [HttpPost]
        public async Task<IActionResult> LendBook([FromBody] LendRequest lendRequest)
        {
            if (lendRequest == null || lendRequest.MemberId <= 0 || string.IsNullOrWhiteSpace(lendRequest.Barcode))
            {
                return NotFound("Invalid Member Id or Barcode");

            }
            else
            {
                var member = _borrowedBookContext.members.FindAsync(lendRequest.MemberId); // Validation
                if (member == null)
                {
                    return NotFound("Member not found.");
                }
                var Barcode = _borrowedBookContext.books.FirstOrDefault(b => b.barcode == lendRequest.Barcode); //Validation
                if (Barcode == null)
                {
                    return NotFound("Barcode not found");
                }
                var book = _borrowedBookContext.books
                                .Where(b => b.barcode == lendRequest.Barcode && b.available_copies > 0)
                                .FirstOrDefault();
                var dueDate = DateTime.Now.AddDays(14);

                var borrowedBook = new BorrowedBooks
                {
                    MemberId = lendRequest.MemberId,
                    BookId = book.id,
                    BorrowDate = DateTime.Now,
                    DueDate = dueDate
                };

                _borrowedBookContext.borrowedBooks.Add(borrowedBook);
                book.available_copies--;
                 await _borrowedBookContext.SaveChangesAsync();

            }
            return Ok("Book successfully lent.");
        }
      
        [HttpPost]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnRequest returnRequest)
        {          
            if (returnRequest == null || string.IsNullOrWhiteSpace(returnRequest.Barcode))
            {
                return BadRequest("Invalid Barcode");
            }            
            var borrowedBook = _borrowedBookContext.borrowedBooks
                .Include(bb => bb.Book) 
                .FirstOrDefault(bb => bb.Book.barcode == returnRequest.Barcode && bb.ReturnDate == null);

            if (borrowedBook == null)
            {
                return NotFound("Book not borrowed.");
            }
            borrowedBook.ReturnDate = DateTime.Now;
            borrowedBook.Book.available_copies++;
            await _borrowedBookContext.SaveChangesAsync();

            return Ok("Book successfully returned.");
        }
    }

    public class LendRequest
    {
        public int MemberId { get; set; }
        public string Barcode { get; set; }
    }

    public class ReturnRequest
    {
        public string Barcode { get; set; }
    }




}

