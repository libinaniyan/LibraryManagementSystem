using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryMangement.Models
{
    [Table("waitlists")]
    public class Waitlists
    {
        [Key]
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public DateTime RequestedTime { get; set; }
        public Members Member { get; set; }
        public Books Book { get; set; }

    }
}
