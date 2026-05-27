using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanDoCauCa.Models // QUAN TRỌNG: Phải có dòng này để định vị file
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        // Quan hệ 1-nhiều: Một danh mục có nhiều sản phẩm
        public virtual ICollection<Product>? Products { get; set; }
    }
}