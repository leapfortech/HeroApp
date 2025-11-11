using System;

public class News
{
    public int Id { get; set; }
    public String Title { get; set; }
    public String Description { get; set; }
    public String Link { get; set; }
    public String Content { get; set; }
    public int Status { get; set; }


    public News()
    {
    }

    public News(int id, String title, String description, String link, String content, int status)
    {
        Id = id;
        Title = title;
        Description = description;
        Link = link;
        Content = content;
        Status = status;
    }
}
