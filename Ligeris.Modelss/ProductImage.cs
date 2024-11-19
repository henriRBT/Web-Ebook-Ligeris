﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ligeris.Modelss
{
    public class ProductImage
    {
        public int Id { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; } 
    }
}
