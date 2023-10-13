using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models.Library
{
    [Table("books", Schema = "dbo")]
    public partial class Book
    {
        [Key]
        [Required]
        public int book_id { get; set; }

        public int? category_id { get; set; }

        public int? book_code { get; set; }

        public string book_name { get; set; }

        public string publisher { get; set; }

        public int? price { get; set; }

    }
}