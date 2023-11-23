using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class WaitlistsController : Controller
    {
        private readonly LibraryMangementContext _waitListContext;

        public WaitlistsController(LibraryMangementContext context)
        {
            _waitListContext = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddToWaitlist([FromBody] WaitlistRequest waitlistRequest)
        {
            if (waitlistRequest == null || waitlistRequest.MemberId <= 0 || string.IsNullOrWhiteSpace(waitlistRequest.Barcode))
            {
                return BadRequest("Invalid Member Id or Barcode.");
            }
            else
            {
                var member = _waitListContext.members.Find(waitlistRequest.MemberId); //Validation
                if (member == null)
                {
                    return NotFound("Member not found");
                }
                var Barcode = _waitListContext.books.FirstOrDefault(b => b.barcode == waitlistRequest.Barcode); //Validation
                if (Barcode == null)
                {
                    return NotFound("Barcode not found");
                }
                var book = _waitListContext.books.FirstOrDefault(b => b.barcode == waitlistRequest.Barcode && b.available_copies == 0);
                if (book == null)
                {
                    return BadRequest("Copies of book is available");
                }
                var waitlistEntry = new Waitlists
                {
                    MemberId = waitlistRequest.MemberId,
                    BookId = book.id,
                    RequestedTime = DateTime.Now
                };

                _waitListContext.waitlists.Add(waitlistEntry);
                await _waitListContext.SaveChangesAsync();
                return Ok("Added to waitlist.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> FulfillWaitlist()
        {
            var waitlistRequest = _waitListContext.waitlists
                .OrderBy(w => w.RequestedTime)
                .Include(w => w.Book)
                .FirstOrDefault();

            if (waitlistRequest != null && waitlistRequest.Book.available_copies > 0)
            {
                var borrowedBook = new BorrowedBooks
                {
                    MemberId = waitlistRequest.MemberId,
                    BookId = waitlistRequest.BookId,
                    BorrowDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14)
                };
                _waitListContext.borrowedBooks.Add(borrowedBook);
                waitlistRequest.Book.available_copies--;
                _waitListContext.waitlists.Remove(waitlistRequest);
                await _waitListContext.SaveChangesAsync();
                return Ok("Book successfully lented");
            }
            else
                return BadRequest("No items in waitlist");
        }
        public class WaitlistRequest
        {
            public string Barcode { get; set; }
            public int MemberId { get; set; }
        }     
    }
}
