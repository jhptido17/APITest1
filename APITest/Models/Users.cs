using System;
using System.Collections.Generic;

namespace APITest.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int? Status { get; set; }
    }
}
