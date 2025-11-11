using System;

using Sirenix.OdinInspector;

public class Meeting
{
    public int Id { get; set; }
    public int BoardUserId { get; set; }
    public int MeetingTypeId { get; set; }
    public String Subject { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public String Description { get; set; }
    public int Status { get; set; }


    public Meeting()
    {
    }

    public Meeting(int id, int boardUserId, int meetingTypeId, String subject, DateTime startDateTime, DateTime endDateTime, String description, int status)
    {
        Id = id;
        BoardUserId = boardUserId;
        MeetingTypeId = meetingTypeId;
        Subject = subject;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Description = description;
        Status = status;
    }
}