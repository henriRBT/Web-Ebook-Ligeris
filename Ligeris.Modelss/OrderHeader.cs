using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ligeris.Modelss
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AplikasiUserId { get; set; }
        [ForeignKey("AplikasiUserId")]
        [ValidateNever]
        public AplikasiUser AplikasiUser { get; set; }

        public DateTime TanggalOrder { get; set; }
        public DateTime TanggalPengiriman { get; set; }
        public double TotalOrder { get; set; }

        public string? StatusOrder { get; set; }
        public string? StatusPembayaran { get; set; }
        public string? NomorPelacakan { get; set; }
        public string? kurir { get; set; }

        public DateTime TanggalPembayaran { get; set; }
        public DateOnly JatuhTempo { get; set; }

        public string?SessionId {get; set;}
        public string?  PaymentIntentId { get; set; }
        [Required]
        public string nomorHp { get; set; }
        [Required]
        public string alamat { get; set; }
        [Required]
        public string kota {get;  set; }
        [Required]
        public string negara { get; set; }
        [Required]
        public string kodePos { get; set; }
    }
}
