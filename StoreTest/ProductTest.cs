using Xunit;
using BL;
using StoreModel;
using StoreDL;
using Moq;
using System.Collections.Generic;

namespace StoreTest
{
public class ProductTest
{
    [Fact]
    public void Should_Get_All_Product()
    {
        string productName = "Nike Air Max 3000";
        int productId = 1;
        Product product = new Product()
        {
            ProductId = productId,
            Name = productName
        };
        List<Product> expectedProducts = new List<Product>();
        expectedProducts.Add(product);
        

        Mock<IRepo> mockRepo = new Mock<IRepo>();
        mockRepo.Setup(repo => repo.GetAllProducts()).Returns(expectedProducts);

        IStoreBL customerBL = new StoreBL(mockRepo.Object);

        List<Product> actualProducts = customerBL.GetAllProducts();

        Assert.Same(expectedProducts, actualProducts); 
        Assert.Equal(productName, actualProducts[0].Name); 
        Assert.Equal(productId, actualProducts[0].ProductId); 
    }
}
}