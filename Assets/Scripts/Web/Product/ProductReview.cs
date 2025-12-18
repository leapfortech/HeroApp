using System;

public class ProductReview
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public long AppUserId { get; set; }
    public int Rating { get; set; }
    public String Description { get; set; }
    public int Status { get; set; }

    public ProductReview() { }

    public ProductReview(long id, long productId, long appUserId, int rating, String description, int status)
    {
        Id = id;
        ProductId = productId;
        AppUserId = appUserId;
        Rating = rating;
        Description = description;
        Status = status;
    }
}
