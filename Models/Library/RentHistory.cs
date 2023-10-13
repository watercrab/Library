using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models.Library
{
    [Table("rent_histories", Schema = "dbo")]
    public partial class RentHistory
    {
        [Key]
        [Required]
        public int rent_id { get; set; }

        public int? book_id { get; set; }

        public int? user_id { get; set; }

        public DateTime? datetime_rent { get; set; }

        public DateTime? datetime_back { get; set; }

        public byte? is_return { get; set; }

    }
}