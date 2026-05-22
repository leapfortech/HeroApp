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
[HttpPathExt(WebServiceType.Main, "/recipe")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RecipeGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long id;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public RecipeFull recipeFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/recipe/FullByPostId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RecipeFullByPostIdGetFullOperation : HttpOperation
{
    [HttpQueryString]
    public long postId;
    [HttpQueryString]
    public long likeAppUserId;

    [HttpResponseJsonBody]
    public RecipeFull recipeFull;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/recipe/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RecipeGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<RecipeFull> recipeFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/recipe/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RecipeRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterRecipeRequest registerRecipeRequest;

    [HttpResponseTextBody]
    public String id;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/recipe")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class RecipePutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterRecipeRequest registerRecipeRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/recipe/Accept")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RecipeAcceptPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}

[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/recipe/Reject")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class RecipeRejectPutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public PostModerationRequest postModerationRequest;

    [HttpResponseTextBody]
    public bool response;
}
