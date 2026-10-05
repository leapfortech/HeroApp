using System;
using System.Collections.Generic;

public class ProductFull : PostFull
{
    public long Id { get; set; }
    public long ProductSubtypeId { get; set; }
    public long SaleCountryId { get; set; }
    public long SaleStateId { get; set; }
    public long CurrencyId { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public long DeliveryTypeId { get; set; }
    public String Annotation { get; set; }
    public int Favorite { get; set; } = 0;
    public int Status { get; set; }

    public List<ProductReviewFull> ProductReviewFulls { get; set; }


    public ProductFull()
    {
    }

    public ProductFull(long id, long postId, long appUserId, String appUserAlias,
                       long postSubtypeId, long postCountryId, long postStateId,
                       String title, String titleImage, String description,
                       int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                       DateTime publicationDateTime, int postStatus,
                       AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, String[] images,
                       long productSubtypeId, long saleCountryId, long saleStateId, long currencyId, double price, double discountPrice,
                       long deliveryTypeId, String annotation, int favorite, int status,
                       List<ProductReviewFull> productReviewFulls)
        : base(postId, appUserId, appUserAlias, postSubtypeId, postCountryId, postStateId, title, titleImage, description,
               imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
               appUserInfo, contactFull, linkFulls, commentFulls, images)
    {
        Id = id;
        ProductSubtypeId = productSubtypeId;
        SaleCountryId = saleCountryId;
        SaleStateId = saleStateId;
        CurrencyId = currencyId;
        Price = price;
        DiscountPrice = discountPrice;
        DeliveryTypeId = deliveryTypeId;
        Annotation = annotation;
        Favorite = favorite;
        Status = status;

        ProductReviewFulls = productReviewFulls ?? new List<ProductReviewFull>();
    }

    public void Update(ProductFull productFull)
    {
        Title = productFull.Title;
        Description = productFull.Description;

        ProductSubtypeId = productFull.ProductSubtypeId;
        SaleCountryId = productFull.SaleCountryId;
        SaleStateId = productFull.SaleStateId;
        CurrencyId = productFull.CurrencyId;
        Price = productFull.Price;
        DiscountPrice = productFull.DiscountPrice;
    }
}
