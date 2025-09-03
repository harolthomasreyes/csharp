using Domain;
using testing.Request;
using testing.Response;

namespace testing.Services.Interfaces
{
    public interface IProductService
    {
        Task<GetAllProductResponse> GetAllProducts();

        Task<GetProductByIdResponse> GetProductById(int id);

        Task<AddProductResponse> AddProduct(AddProductResquest product);

        Task<UpdateProductResponse> UpdateProduct(int id,UpdateProductResquest product);

        Task<DeleteProductResponse> DeleteProduct(int id);
    }
}
