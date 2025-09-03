using Domain;

namespace testing.Dto
{
    public class ProductDto
    {
        public ProductDto(Product product)
        {
            this.Id = product.Id;
            this.Name = product.Name;
            this.Price = product.Price;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
