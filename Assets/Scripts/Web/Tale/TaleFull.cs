using System;
using System.Collections.Generic;

public class TaleFull : PostFull
{
    public long Id { get; set; }
    public int Status { get; set; }


    public TaleFull()
    {
    }

    public TaleFull(long id, long postId, long appUserId, String appUserAlias,
                    long postSubtypeId, long postCountryId, long postStateId,
                    String title, String titleImage, String description,
                    int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                    DateTime publicationDateTime, int postStatus,
                    AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, String[] images,
                    int status)
        : base(postId, appUserId, appUserAlias, postSubtypeId, postCountryId, postStateId, title, titleImage, description,
               imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
               appUserInfo, contactFull, linkFulls, commentFulls, images)
    {
        Id = id;
        Status = status;
    }
}
