using System.Collections.Generic;

using hg.ApiWebKit.core.http;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.providers;
using hg.ApiWebKit.mappers;
using hg.ApiWebKit.authorizations;

using Leap.Data.Web;
using System;

// GET
[HttpGET]
[HttpPathExt(WebServiceType.Main, "/meeting/ByDates")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class MeetingGetOperation : HttpOperation
{
    [HttpQueryString]
    public DateTime startDateTime;
    [HttpQueryString]
    public DateTime endDateTime;

    [HttpResponseJsonBody]
    public Meeting[] meetings;
}

[HttpGET]
[HttpPathExt(WebServiceType.Main, "/meeting/Fulls")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class MeetingFullsGetOperation : HttpOperation
{
    [HttpResponseJsonBody]
    public MeetingFull[] meetingFulls;
}


[HttpGET]
[HttpPathExt(WebServiceType.Main, "/meeting/FullsByDates")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class MeetingFullsByDatesGetOperation : HttpOperation
{
    [HttpQueryString]
    public DateTime startDateTime;
    [HttpQueryString]
    public DateTime endDateTime;

    [HttpResponseJsonBody]
    public MeetingFull[] meetingFulls;
}

// REGISTER
[HttpPOST]
[HttpPathExt(WebServiceType.Main, "/meeting/RegisterAppointment")]
[HttpProvider(typeof(HttpUnityWebAzureClient))]
[HttpContentType("application/json")]
[HttpAccept("application/json")]
[HttpFirebaseAuthorization]
public class AppointmentRegisterOperation : HttpOperation
{
    [HttpRequestJsonBody]
    public Appointment appointment;

    [HttpResponseTextBody]
    public String appointmentId;
}
