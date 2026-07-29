using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Review
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        public int ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Đánh giá sao phải từ 1 đến 5")]
        public int Rating { get; set; } = 5;

        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        [StringLength(1000, ErrorMessage = "Bình luận tối đa 1000 ký tự")]
        public string Comment { get; set; } = string.Empty;
    }
}
