using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APITest.Models
{
    public partial class Customers
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(100)]
        public string Image { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; }
        [MaxLength(50)]
        public string UpdateBy { get; set; }
    }
}
