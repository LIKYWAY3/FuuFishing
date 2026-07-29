using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Data.Entities
{
    public class Category : BaseEntity
    {
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Slug { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; }
            = new List<Category>();



        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}