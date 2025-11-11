using System;

public class ProjectLike
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int AppUserId { get; set; }
    public int Status { get; set; }


    public ProjectLike()
    {
    }

    public ProjectLike(int id, int projectId, int appUserId, int status)
    {
        Id = id;
        ProjectId = projectId;
        AppUserId = appUserId;
        Status = status;
    }

    public ProjectLike(int projectId, int appUserId)
    {
        Id = -1;
        ProjectId = projectId;
        AppUserId = appUserId;
        Status = -1;
    }
}