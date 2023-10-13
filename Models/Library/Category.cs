using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models.Library
{
    [Table("categories", Schema = "dbo")]
    public partial class Category
    {
        [Key]
        [Required]
        public int category_id { get; set; }

        public int? category_code { get; set; }

        public string category_name { get; set; }

    }
}