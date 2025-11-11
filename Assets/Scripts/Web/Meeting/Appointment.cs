using System;

using Sirenix.OdinInspector;

public class Appointment
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public int AppUserId { get; set; }
    public int Status { get; set; }


    public Appointment()
    {
    }

    public Appointment(int id, int meetingId, int appUserId, int status)
    {
        Id = id;
        MeetingId = meetingId;
        AppUserId = appUserId;
        Status = status;
    }

    public Appointment(int meetingId, int appUserId)
    {
        Id = -1;
        MeetingId = meetingId;
        AppUserId = appUserId;
        Status = -1;
    }
}