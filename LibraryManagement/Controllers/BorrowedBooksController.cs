using LibraryMangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowedBooksController : ControllerBase
    {
        private readonly LibraryMangementContext _borrowedBookContext;

        public BorrowedBooksController(LibraryMangementContext context)
        {
            _borrowedBookContext = context;
        }
        [HttpPost("lend")]
        public async Task<IActionResult> LendBook([FromBody] LendRequest lendRequest)
        {
            if (lendRequest == null || lendRequest.MemberId <= 0 || string.IsNullOrWhiteSpace(lendRequest.Barcode))
            {
                return NotFound("Invalid Member Id or Barcode");
            }
            bool canBorrow = CanBorrowBook(lendRequest);  // checking the member is allowed to borrow books or not.
            if (canBorrow)
            {
                var member = _borrowedBookContext.members.Find(lendRequest.MemberId); // Validation for memeberId
                if (member == null)
                {
                    return NotFound("Member not found.");
                }
                var Barcode = _borrowedBookContext.books.FirstOrDefault(b => b.barcode == lendRequest.Barcode); //Validation for Barcode
                if (Barcode == null)
                {
                    return NotFound("Barcode not found");
                }

                //getting the BookId from barcode . Useful for the scenarios where we want the book to add in waitlist
                var bookId = _borrowedBookContext.books.Where(b => b.barcode == lendRequest.Barcode).Select(b => b.id).FirstOrDefault(); 

                var book = _borrowedBookContext.books.Where(b => b.barcode == lendRequest.Barcode && b.available_copies > 0).FirstOrDefault();

                if (book == null) // if the book is not avilable at the moment ,Adding the requested book to waitlist.
                {
                    var waitlistEntry = new Waitlists
                    {
                        MemberId = lendRequest.MemberId,
                        BookId = bookId,
                        RequestedTime = DateTime.Now
                    };

                    _borrowedBookContext.waitlists.Add(waitlistEntry);
                    await _borrowedBookContext.SaveChangesAsync();
                    return Ok("Added to waitlist.");
                }
                var dueDate = DateTime.Now.AddDays(14);

                var borrowedBook = new BorrowedBooks //if the book is avilable at the moment, lending the requested book to memeber.
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
            
            else // if the member limit is over
            {
                return NotFound("Member book lent limit is over");
            }
            return Ok("Book successfully lent.");
        }
      
        [HttpPost("return")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnRequest returnRequest)
        {          
            if (returnRequest == null || string.IsNullOrWhiteSpace(returnRequest.Barcode))
            {
                return BadRequest("Invalid Barcode");
            }            
            var borrowedBook =  await _borrowedBookContext.borrowedBooks
                .Include(bb => bb.Book) 
                .FirstOrDefaultAsync(bb => bb.Book.barcode == returnRequest.Barcode && bb.ReturnDate == null);

            if (borrowedBook == null)
            {
                return NotFound("Book not borrowed.");
            }
            borrowedBook.ReturnDate = DateTime.Now;
            borrowedBook.Book.available_copies++;
            await _borrowedBookContext.SaveChangesAsync();

            return Ok("Book successfully returned.");
        }

        [HttpPost("fullfill")]
        public async Task<IActionResult> FulfillWaitlist()  
        {
            var successfullyLentBooks = new List<string>();
            var failedRequests = new List<string>();

            var waitlistRequests = _borrowedBookContext.waitlists    // To get all the waitlist entries from the waitlists table
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
                    _borrowedBookContext.borrowedBooks.Add(borrowedBook);
                    waitlistRequest.Book.available_copies--;
                    _borrowedBookContext.waitlists.Remove(waitlistRequest);
                    successfullyLentBooks.Add(waitlistRequest.Book.title);
                }
                else
                {
                    failedRequests.Add(waitlistRequest.Book.title);
                }
            }
            await _borrowedBookContext.SaveChangesAsync();

            if (successfullyLentBooks.Count > 0)
            {
                var successMessage = $"Books successfully lent: {string.Join(", ", successfullyLentBooks)}";

                if (failedRequests.Count > 0)
                {
                    var failureMessage = $"Failed to lend books: {string.Join(", ", failedRequests)},since its copies are not available at the moment";
                    return Ok($"{successMessage}. {failureMessage}");
                }
                return Ok(successMessage);
            }
            return BadRequest("No books could be lent.");
        }

        public bool CanBorrowBook(LendRequest lendRequest)
        {
            var bookGenre = _borrowedBookContext.books.Where(b => b.barcode == lendRequest.Barcode).Select(b => b.genre).FirstOrDefault();

            //  Checking for total  borrowed books
            if (_borrowedBookContext.borrowedBooks.Count(b => b.MemberId == lendRequest.MemberId && b.ReturnDate == null) >= 3)   
            {
                return false; 
            }

            // Checking limit for same genre
            if (_borrowedBookContext.borrowedBooks.Count(b =>b.MemberId == lendRequest.MemberId && b.Book.genre == bookGenre && b.ReturnDate == null) >= 2)
            {
                return false; 
            }
            return true;
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

