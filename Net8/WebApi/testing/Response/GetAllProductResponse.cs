using testing.Dto;

namespace testing.Response
{
    public class GetAllProductResponse : BaseResponse
    {
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
