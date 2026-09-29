using System;
using System.Collections.Generic;

public class NewsFull : PostFull
{
    public long Id { get; set; }
    public long NewsTypeId { get; set; }
    public String Place { get; set; }
    public String Source { get; set; }
    public DateTime? DateTime { get; set; }
    public int Status { get; set; }


    public NewsFull()
    {
    }

    public NewsFull(long id, long postId, long appUserId, String appUserAlias,
                    long postSubtypeId, long postCountryId, long postStateId,
                    String title, String titleImage, String description,
                    int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                    DateTime publicationDateTime, int postStatus,
                    AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, String[] images,
                    long newsTypeId, String place, String source, DateTime? dateTime, int status)
        : base(postId, appUserId, appUserAlias, postSubtypeId, postCountryId, postStateId, title, titleImage, description,
               imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
               appUserInfo, contactFull, linkFulls, commentFulls, images)
    {
        Id = id;
        NewsTypeId = newsTypeId;
        Place = place;
        Source = source;
        DateTime = dateTime;
        Status = status;
    }
}
