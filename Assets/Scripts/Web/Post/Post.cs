using System;

public class Post
{
    public long Id { get; set; }
    public long AppUserId { get; set; }
    public long PostTypeId { get; set; }
    public long CountryId { get; set; }
    public long StateId { get; set; }
    public String Title { get; set; }
    public String Summary { get; set; }
    public String Description { get; set; }
    public int ImageCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime PublicationDateTime { get; set; }
    public DateTime? ApprovalDateTime { get; set; }
    public DateTime? ExpirationDateTime { get; set; }
    public int Status { get; set; }

    public Post() { }

    public Post(long id, long appUserId, long postTypeId, long countryId,
                long stateId, String title, String summary, String description, int imageCount,
                int likeCount, DateTime publicationDateTime, DateTime? approvalDateTime,
                DateTime? expirationDateTime, int status)
    {
        Id = id;
        AppUserId = appUserId;
        PostTypeId = postTypeId;
        CountryId = countryId;
        StateId = stateId;
        Title = title;
        Summary = summary;
        Description = description;
        ImageCount = imageCount;
        LikeCount = likeCount;
        PublicationDateTime = publicationDateTime;
        ApprovalDateTime = approvalDateTime;
        ExpirationDateTime = expirationDateTime;
        Status = status;
    }

    public Post(TaleFull taleFull)
    {
        Id = taleFull.PostId;
        AppUserId = taleFull.AppUserId;
        PostTypeId = taleFull.PostTypeId;
        CountryId = taleFull.PostCountryId;
        StateId = taleFull.PostStateId;
        Title = taleFull.Title;
        Summary = taleFull.Summary;
        Description = taleFull.Description;
        ImageCount = taleFull.ImageCount;
        LikeCount = taleFull.LikeCount;
        PublicationDateTime = taleFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = taleFull.PostStatus;
    }

    public Post(RecipeFull recipeFull)
    {
        Id = recipeFull.PostId;
        AppUserId = recipeFull.AppUserId;
        PostTypeId = recipeFull.PostTypeId;
        CountryId = recipeFull.PostCountryId;
        StateId = recipeFull.PostStateId;
        Title = recipeFull.Title;
        Summary = recipeFull.Summary;
        Description = recipeFull.Description;
        ImageCount = recipeFull.ImageCount;
        LikeCount = recipeFull.LikeCount;
        PublicationDateTime = recipeFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = recipeFull.PostStatus;
    }

    public Post(TreatmentFull treatmentFull)
    {
        Id = treatmentFull.PostId;
        AppUserId = treatmentFull.AppUserId;
        PostTypeId = treatmentFull.PostTypeId;
        CountryId = treatmentFull.PostCountryId;
        StateId = treatmentFull.PostStateId;
        Title = treatmentFull.Title;
        Summary = treatmentFull.Summary;
        Description = treatmentFull.Description;
        ImageCount = treatmentFull.ImageCount;
        LikeCount = treatmentFull.LikeCount;
        PublicationDateTime = treatmentFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = treatmentFull.PostStatus;
    }

    public Post(RadioFull radioFull)
    {
        Id = radioFull.PostId;
        AppUserId = radioFull.AppUserId;
        PostTypeId = radioFull.PostTypeId;
        CountryId = radioFull.PostCountryId;
        StateId = radioFull.PostStateId;
        Title = radioFull.Title;
        Summary = radioFull.Summary;
        Description = radioFull.Description;
        ImageCount = radioFull.ImageCount;
        LikeCount = radioFull.LikeCount;
        PublicationDateTime = radioFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = radioFull.PostStatus;
    }

    public Post(ProductFull productFull)
    {
        Id = productFull.PostId;
        AppUserId = productFull.AppUserId;
        PostTypeId = productFull.PostTypeId;
        CountryId = productFull.PostCountryId;
        StateId = productFull.PostStateId;
        Title = productFull.Title;
        Summary = productFull.Summary;
        Description = productFull.Description;
        ImageCount = productFull.ImageCount;
        LikeCount = productFull.LikeCount;
        PublicationDateTime = productFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = productFull.PostStatus;
    }

    public Post(HappeningFull happeningFull)
    {
        Id = happeningFull.PostId;
        AppUserId = happeningFull.AppUserId;
        PostTypeId = happeningFull.PostTypeId;
        CountryId = happeningFull.PostCountryId;
        StateId = happeningFull.PostStateId;
        Title = happeningFull.Title;
        Summary = happeningFull.Summary;
        Description = happeningFull.Description;
        ImageCount = happeningFull.ImageCount;
        LikeCount = happeningFull.LikeCount;
        PublicationDateTime = happeningFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = happeningFull.PostStatus;
    }

    public Post(NewsFull newsFull)
    {
        Id = newsFull.PostId;
        AppUserId = newsFull.AppUserId;
        PostTypeId = newsFull.PostTypeId;
        CountryId = newsFull.PostCountryId;
        StateId = newsFull.PostStateId;
        Title = newsFull.Title;
        Summary = newsFull.Summary;
        Description = newsFull.Description;
        ImageCount = newsFull.ImageCount;
        LikeCount = newsFull.LikeCount;
        PublicationDateTime = newsFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = newsFull.PostStatus;
    }

    public Post(PuzzleFull puzzleFull)
    {
        Id = puzzleFull.PostId;
        AppUserId = puzzleFull.AppUserId;
        PostTypeId = puzzleFull.PostTypeId;
        CountryId = puzzleFull.PostCountryId;
        StateId = puzzleFull.PostStateId;
        Title = puzzleFull.Title;
        Summary = puzzleFull.Summary;
        Description = puzzleFull.Description;
        ImageCount = puzzleFull.ImageCount;
        LikeCount = puzzleFull.LikeCount;
        PublicationDateTime = puzzleFull.PublicationDateTime;
        ApprovalDateTime = null;
        ExpirationDateTime = null;
        Status = puzzleFull.PostStatus;
    }

    public void Update(Post post)
    {
        Title = post.Title;
        Summary = post.Summary;
        Description = post.Description;
    }
}
