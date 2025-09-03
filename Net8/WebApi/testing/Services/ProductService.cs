namespace Services;

using Domain;
using testing.Services.Interfaces;
using System.Linq;
using testing.Response;
using testing.Dto;
using testing.Request;
using Repository.Interface;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _productRepository;

    public ProductService(IGenericRepository<Product> productRepository)
    {
        this._productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<GetAllProductResponse> GetAllProducts()
    {
        var response = new GetAllProductResponse();

        try
        {
            var products = _productRepository.GetAll();
            products.ToList().ForEach(_ => response.Products.Add(new testing.Dto.ProductDto(_)));
        }
        catch (Exception e)
        {
            response.Error = new Error() {Message= e.Message};
        }
        
        return response;
    }

    public async Task<GetProductByIdResponse> GetProductById(int id)
    {
        var response = new GetProductByIdResponse();

        try
        {
            var productFound = _productRepository.GetById(id);
            if(productFound != null)
            {
                response.Product = new ProductDto(productFound);
            }
            else
            {
                response.Error = new Error() { Message = $"Product {id} does not exists" };
            }
           
        }
        catch (Exception e)
        {
            response.Error = new Error() { Message = e.Message };
        }
       

        return response;
    }

    public async Task<AddProductResponse> AddProduct(AddProductResquest product)
    {
        AddProductResponse response = new();
        try
        {
            var newProduct = new Product() { Name = product.Name, Price = product.Price };
            _productRepository.Add(newProduct);
            response.Success = true;
            response.Message = "Product Created";
        }
        catch (Exception e)
        {
            response.Error = new Error() { Message = e.Message };
        }

        return response;

    }

    public async Task<UpdateProductResponse> UpdateProduct(int id, UpdateProductResquest product)
    {
        UpdateProductResponse response = new();
        try
        {
            var productFound = _productRepository.GetById(id);

            if(productFound != null) 
            {
                productFound.Name = productFound.Name;
                productFound.Price = productFound.Price;
                _productRepository.Update(productFound);

                response.Success = true;
                response.Message = "Product Updated";
            }
            else
            {
                response.Success = false;
                response.Message = "Product does not exists";
            }

        }
        catch (Exception e)
        {
            response.Error = new Error() { Message = e.Message };
        }

        return response;
    }

    public async Task<DeleteProductResponse> DeleteProduct(int id)
    {
        var response = new DeleteProductResponse();

        try
        {
            _productRepository.Delete(id);
            response.Success = true;
            response.Message = "Product Removed";
        }
        catch (Exception e)
        {
            response.Error = new Error() { Message = e.Message };
        }

        return response;
    }
}