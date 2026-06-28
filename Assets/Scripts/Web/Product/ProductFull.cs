using System;
using System.Collections.Generic;

using UnityEngine;
using Leap.Graphics.Tools;

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
    public int Status { get; set; }

    public List<ProductReviewFull> ProductReviewFulls { get; set; }
    public String[] Images
    {
        get => null;
        set
        {
            ImageSprites = new List<Sprite>();
            for (int i = 0; i < value.Length; i++)
                if (value[i] != null)
                    ImageSprites.Add(value[i].CreateSprite("Product_" + i.ToString("D02")));
        }
    }
    public List<Sprite> ImageSprites { get; set; }


    public ProductFull()
    {
    }

    public ProductFull(long id, long postId, long appUserId, String appUserAlias,
                       long postSubtypeId,
                       long postCountryId, long postStateId,
                       String title, String titleImage, String summary, String description,
                       int imageCount, int favorite, int like, int likeCount, long reactionPhraseId,
                       DateTime publicationDateTime, int postStatus,
                       ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls,
                       long productSubtypeId, long saleCountryId, long saleStateId,
                       long currencyId, double price, double discountPrice,
                       long deliveryTypeId, String annotation,
                       int status,
                       List<ProductReviewFull> productReviewFulls,
                       String[] images)
        : base(postId, appUserId, appUserAlias, postSubtypeId,
                postCountryId, postStateId, title, titleImage, summary, description,
                imageCount, favorite, like, likeCount, reactionPhraseId, publicationDateTime, postStatus,
                contactFull, linkFulls, commentFulls)
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
        Status = status;

        ProductReviewFulls = productReviewFulls ?? new List<ProductReviewFull>();
        Images = images;
    }
}
