using testing.Dto;

namespace testing.Response
{
    public class GetProductByIdResponse : BaseResponse
    {
        public ProductDto Product { get; set; } 
    }
}
