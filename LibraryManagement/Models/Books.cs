using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryMangement.Models
{
    [Table("books")]
    public class Books
    {
            [Key]
            public int id { get; set; }

            [Required(ErrorMessage = "Title is required.")]
            [MaxLength(150)]
            public string title { get; set; }

            [MaxLength(50)]
            public string author { get; set; }

            [MaxLength(50)]
            public string publisher { get; set; }
             
            public DateTime? publication_date { get; set; }

            [MaxLength(30)]
            public string genre { get; set; }

            [Required(ErrorMessage = "Price is required.")]
            [Column(TypeName = "decimal(5,2)")]
            public decimal price { get; set; }

            [Required(ErrorMessage = "Barcode is required.")]
            [StringLength(10)]
            public string barcode { get; set; }

            [Range(0, 10)]
            public short? rating { get; set; }

            [Range(0, int.MaxValue)]
            public int? review_count { get; set; }

            [Required(ErrorMessage = "Total Copies is required.")]
            [Range(1, int.MaxValue)]
            public int total_copies { get; set; }

            [Required(ErrorMessage = "Available Copies is required.")]
            [Range(0,int.MaxValue)]
            public int? available_copies { get; set; }
        

    }
}



