using System;
using System.Collections.Generic;

namespace APITest.Models
{
    public partial class Customers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Image { get; set; }
        public string CreatedBy { get; set; }
        public string UpdateBy { get; set; }
    }
}
