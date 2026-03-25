using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMS.Models
{
    [Table("Students")]
    public class Studentsmodel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Batch is required")]
        [StringLength(20, ErrorMessage = "Batch cannot be longer than 20 characters")]
        public string batch { get; set; } = string.Empty;
    }
}
