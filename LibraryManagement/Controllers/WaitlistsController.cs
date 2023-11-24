using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            var successfullyLentBooks = new List<string>();
            var failedRequests = new List<string>();

            var waitlistRequests = _waitListContext.waitlists    // To get all the waitlist entries from the waitlists table
                .OrderBy(w => w.RequestedTime)
                .Include(w => w.Book)
                .ToList();
            foreach (var waitlistRequest in waitlistRequests)
            {
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
                    successfullyLentBooks.Add(waitlistRequest.Book.title);
                }
                else
                {
                    failedRequests.Add(waitlistRequest.Book.title);
                }
            }

            await _waitListContext.SaveChangesAsync();
            if (successfullyLentBooks.Count > 0)
            {
                var successMessage = $"Books successfully lent: {string.Join(", ", successfullyLentBooks)}";

                if (failedRequests.Count > 0)
                {
                    var failureMessage = $"Failed to lend books: {string.Join(", ", failedRequests)}";
                    return Ok($"{successMessage}. {failureMessage}");
                }

                return Ok(successMessage);
            }

            return BadRequest("No books could be lent.");                               
        }
        public class WaitlistRequest
        {
            public string Barcode { get; set; }
            public int MemberId { get; set; }
        }     
    }
}
