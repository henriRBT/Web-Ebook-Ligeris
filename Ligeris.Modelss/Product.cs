using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ligeris.Modelss
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Deskripsi { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }

        [Required]
        [Display(Name = "List Price")]
        [Range(10000, 1000000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 10-100k")]
        [Range(10000, 1000000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 100-500k")]
        [Range(10000, 1000000)]
        public double Price100 { get; set; }

        [Required]
        [Display(Name = "Price for 500k+")]
        [Range(10000, 1000000)]
        public double Price500 { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
