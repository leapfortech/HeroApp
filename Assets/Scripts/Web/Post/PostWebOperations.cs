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
[HttpPathExt(WebServiceType.Main, "/post/FullsPaged")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PostFullsPagedOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostFeedRequest postFeedRequest;

    [HttpResponseJsonBody]
    public PostFeedResponse postFeedResponse;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterShare")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
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
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class FavoriteRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Favorite favorite;

    [HttpResponseTextBody]
    public String favoriteId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterComment")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
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
[HttpAccept("application/json")]
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
[HttpAccept("application/json")]
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
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PostReadRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostRead postRead;

    [HttpResponseTextBody]
    public String postReadId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterReaction")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class ReactionRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Reaction reaction;

    [HttpResponseTextBody]
    public String reactionId;
}

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/post/RegisterLike")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class LikeRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Like like;

    [HttpResponseTextBody]
    public String likeId;
}
