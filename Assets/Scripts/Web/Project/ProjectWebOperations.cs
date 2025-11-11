using System.Collections.Generic;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;
using System;

// GET

// PROJECTS

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/project/LikeIdsByAppUserId")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class LikeIdsByAppUserIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int appUserId;

    [HttpResponseJsonBody]
    public List<int> projectLikeIds;
}

// PROJECT IMAGES

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/project/ImagesById")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
[HttpTimeout(40f)]
public class ImagesByIdGetOperation : HttpOperation
{
    [HttpQueryString]
    public int id;

    [HttpResponseJsonBody]
    public String[] projectImages;
}

// REGISTER

[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/project/RegisterLike")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class LikeRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public ProjectLike projectLike;

    [HttpResponseTextBody]
    public String projectLikeId;
}