using System;

public class Product
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long ProductSubtypeId { get; set; }
    public long SaleCountryId { get; set; }
    public long SaleStateId { get; set; }
    public long CurrencyId { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public long DeliveryTypeId { get; set; }
    public String Annotation { get; set; }
    public int Status { get; set; }

    public Product() 
    {
    }

    public Product(long id, long postId, long productSubtypeId, long saleCountryId, long saleStateId, long currencyId,
                    double price, double discountPrice, long deliveryTypeId, String annotation, int status)
    {
        Id = id;
        PostId = postId;
        ProductSubtypeId = productSubtypeId;
        SaleCountryId = saleCountryId;
        SaleStateId = saleStateId;
        CurrencyId = currencyId;
        Price = price;
        DiscountPrice = discountPrice;
        DeliveryTypeId = deliveryTypeId;
        Annotation = annotation;
        Status = status;
    }

    public Product(ProductFull productFull)
    {
        Id = productFull.Id;
        PostId = productFull.PostId;
        ProductSubtypeId = productFull.ProductSubtypeId;
        SaleCountryId = productFull.SaleCountryId;
        SaleStateId = productFull.SaleStateId;
        CurrencyId = productFull.CurrencyId;
        Price = productFull.Price;
        DiscountPrice = productFull.DiscountPrice;
        DeliveryTypeId = productFull.DeliveryTypeId;
        Annotation = productFull.Annotation;
        Status = productFull.Status;
    }

    public void Update(Product product)
    {
        ProductSubtypeId = product.ProductSubtypeId;
        SaleCountryId = product.SaleCountryId;
        SaleStateId = product.SaleStateId;
        CurrencyId = product.CurrencyId;
        Price = product.Price;
        DiscountPrice = product.DiscountPrice;
        DeliveryTypeId = product.DeliveryTypeId;
        Annotation = product.Annotation;
    }
}
