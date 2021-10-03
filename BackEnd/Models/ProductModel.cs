using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(200)]
        public string Discription { get; set; }
        [Required]
        public bool OutOfStock { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
