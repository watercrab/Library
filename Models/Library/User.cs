using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models.Library
{
    [Table("users", Schema = "dbo")]
    public partial class User
    {
        [Key]
        [Required]
        public int user_id { get; set; }

        [Required]
        public int user_code { get; set; }

        [Required]
        public string first_name { get; set; }

        [Required]
        public string last_name { get; set; }

        [Required]
        public string tel { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public byte login_status { get; set; }

    }
}