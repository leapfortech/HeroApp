using System;
using System.Collections.Generic;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;

// GET

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/post/ImagesById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
[HttpTimeout(40f)]
public class ImagesByIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public long id;
    
    [HttpQueryString]
    public String first;

    [HttpResponseJsonBody]
    public String[] projectImages;
}

// POST
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/PostFeed")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
//[HttpTimeout(1)]
[HttpFirebaseAuthorization]
public class PostFeedOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostFeedRequest postFeedRequest;

    [HttpResponseJsonBody]
    public PostFeedResponse postFeedResponse;
}

// POST
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/CommentFeed")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class CommentFeedOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public CommentFeedRequest commentFeedRequest;

    [HttpResponseJsonBody]
    public CommentFeedResponse commentFeedResponse;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterShare")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class ShareRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Share share;

    [HttpResponseTextBody]
    public String shareId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterFavorite")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class FavoriteRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Favorite favorite;

    [HttpResponseTextBody]
    public String favoriteId;
}

[HttpDELETE]
[HttpPathExt(WebServiceType.Main, "/post/DeleteFavorite")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class FavoriteDeleteOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Favorite favorite;

    [HttpResponseTextBody]
    public String done;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterLike")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class LikeRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Like like;

    [HttpResponseTextBody]
    public String likeId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/UpdateLike")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class LikeUpdateOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Like like;

    [HttpResponseTextBody]
    public String likeId;
}

[HttpDELETE]
[HttpPathExt(WebServiceType.Main, "/post/DeleteLike")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class LikeDeleteOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Like like;

    [HttpResponseTextBody]
    public String likeId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterReaction")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class ReactionRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Reaction reaction;

    [HttpResponseTextBody]
    public String reactionId;
}

[HttpDELETE]
[HttpPathExt(WebServiceType.Main, "/post/DeleteReaction")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class ReactionDeleteOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Reaction reaction;

    [HttpResponseTextBody]
    public String done;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterComment")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class CommentRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Comment comment;

    [HttpResponseTextBody]
    public String commentId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterCommentPlaint")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class CommentPlaintRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public CommentPlaint commentPlaint;

    [HttpResponseTextBody]
    public String commentPlaintId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterPostPlaint")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class PostPlaintRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostPlaint postPlaint;

    [HttpResponseTextBody]
    public String postPlaintId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterPostRead")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class PostReadRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostRead postRead;

    [HttpResponseTextBody]
    public String postReadId;
}
