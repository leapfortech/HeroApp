using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class ProductFeed
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public String TitleImage
    {
        get => null;
        set => TitleSprite = value?.CreateSprite("Product_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Title { get; set; }
    public String ProductSubtype { get; set; }
    public String SaleCountry { get; set; }
    public String SaleState { get; set; }
    public String Currency { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public String Link { get; set; }
    public int Favorite { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);

    public ProductFeed()
    {
    }

    public ProductFeed(long id, long postId, string titleImage, Sprite titleSprite, string title, string productSubtype, string saleCountry, string saleState,
                       string currency, double price, double discountPrice, string link, int favorite, DateTime publicationDateTime)
    {
        Id = id;
        PostId = postId;
        TitleImage = titleImage;
        TitleSprite = titleSprite;
        Title = title;
        ProductSubtype = productSubtype;
        SaleCountry = saleCountry;
        SaleState = saleState;
        Currency = currency;
        Price = price;
        DiscountPrice = discountPrice;
        Link = link;
        Favorite = favorite;
        PublicationDateTime = publicationDateTime;
    }

    public ProductFeed(ProductFull productFull)
    {
        Id = productFull.Id;
        PostId = productFull.PostId;
        TitleSprite = productFull.TitleSprite;
        Title = productFull.Title;
        ProductSubtype = productFull.ProductSubtypeId;
        SaleCountry = productFull.SaleCountryId;
        SaleState = productFull.SaleStateId;
        Currency = productFull.CurrencyId;
        Price = productFull.Price;
        DiscountPrice = productFull.DiscountPrice;
        Link = productFull.LinkFulls[0].Url;
        Favorite = productFull.Favorite;
        PublicationDateTime = productFull.PublicationDateTime;
    }
}
