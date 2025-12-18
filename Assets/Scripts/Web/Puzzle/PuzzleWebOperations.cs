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
[HttpPathExt(WebServiceType.Main, "/puzzle/FullsByStatus")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PuzzleGetFullsOperation : HttpOperation
{
    [HttpQueryString]
    public int status;

    [HttpResponseJsonBody]
    public List<PuzzleFull> puzzleFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/puzzle/register")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class PuzzleRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public RegisterPuzzleRequest registerPuzzleRequest;

    [HttpResponseTextBody]
    public String id;
}

//UPDATE
[HttpPUT]
[HttpPathExt(WebServiceType.Main, "/puzzle")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("text/plain")]
[HttpFirebaseAuthorization]
public class PuzzlePutOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Puzzle puzzle;

    [HttpResponseJsonBody]
    public bool response;
}
