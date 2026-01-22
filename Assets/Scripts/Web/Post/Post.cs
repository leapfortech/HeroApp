using System;

public class Post
{
    public long Id { get; set; }
    public long AppUserId { get; set; }
    public long PostSubtypeId { get; set; }
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

    public Post(long id, long appUserId, long postSubtypeId, long countryId,
                long stateId, String title, String summary, String description, int imageCount,
                int likeCount, DateTime publicationDateTime, DateTime? approvalDateTime,
                DateTime? expirationDateTime, int status)
    {
        Id = id;
        AppUserId = appUserId;
        PostSubtypeId = postSubtypeId;
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
        PostSubtypeId = taleFull.PostSubtypeId;
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
}
