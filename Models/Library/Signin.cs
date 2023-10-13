using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models.Library
{
    [Table("signin", Schema = "dbo")]
    public partial class Signin
    {
        [Key]
        [Required]
        public int signin_id { get; set; }

        public int? user_id { get; set; }

        public DateTime? time_in { get; set; }

        public DateTime? time_out { get; set; }

    }
}