using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table ("ProducImage")]
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        public string FileName { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
