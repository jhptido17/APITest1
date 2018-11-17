using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APITest.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        //[Required]
        [MaxLength(50)]
        public string Username { get; set; }
        //[Required]
        [MaxLength(50)]
        public string Password { get; set; }
        [MaxLength(5)]
        public string Role { get; set; }
        public int? Status { get; set; }
    }
}
