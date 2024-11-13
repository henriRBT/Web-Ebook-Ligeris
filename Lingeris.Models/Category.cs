using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ligeris.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Category Name")]
        [Required(ErrorMessage = "Nama harus diisi")]
        [MaxLength(30, ErrorMessage = "Nama Tidak Boleh lebih 30 karakter")]
        public string Name { get; set; }

        [DisplayName("Display Order")]
        [Required(ErrorMessage = "Display Order tidak boleh Kosong!")]
        [Range(1, 100, ErrorMessage = "Display Order harus diantara 1-100")]
        public int? DisplayOrder { get; set; }
    }
}
