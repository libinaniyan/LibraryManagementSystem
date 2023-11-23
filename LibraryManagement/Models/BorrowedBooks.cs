using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryMangement.Models
{
    [Table("borrowed_books")]
    public class BorrowedBooks
    {
        [Key]
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public Members Member { get; set; }
        public Books Book { get; set; }
    }
}
