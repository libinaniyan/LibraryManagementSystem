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
            [Display(Name = "Title")]
            public string title { get; set; }

            [MaxLength(50)]
            [Display(Name = "Author")]
            public string author { get; set; }

            [MaxLength(50)]
            [Display(Name = "Published By")]
            public string publisher { get; set; }
             
            [Display(Name = "Published On")]
            public DateTime? publication_date { get; set; }

            [MaxLength(30)]
            [Display(Name = "Catergory")]
            public string genre { get; set; }

            [Required(ErrorMessage = "Price is required.")]
            [Column(TypeName = "decimal(5,2)")]
            [Display(Name = "Price")]
            public decimal price { get; set; }

            [Required(ErrorMessage = "Barcode is required.")]
            [StringLength(10)]
            [Display(Name = "Barcode")]
            public string barcode { get; set; }

            [Display(Name = "Rating")]
            [Range(0, 10)]
            public short? rating { get; set; }

            [Display(Name = "Review Count")]
            [Range(0, int.MaxValue)]
            public int? review_count { get; set; }

            [Required(ErrorMessage = "Total Copies is required.")]
            [Range(1, int.MaxValue)]
            [Display(Name = "Total no:of copies")]
            public int total_copies { get; set; }

            [Required(ErrorMessage = "Available Copies is required.")]
            [Range(0,int.MaxValue)]
            [Display(Name = "Available no:of copies")]
            public int? available_copies { get; set; }
        

    }

}

