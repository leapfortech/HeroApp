using System;

public class News
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public long NewsTypeId { get; set; }
    public String Place { get; set; }
    public String Source { get; set; }
    public DateTime? DateTime { get; set; }
    public int Status { get; set; }

    public News() { }

    public News(long id, long postId, long newsTypeId, String place, String source,
                DateTime? dateTime, int status)
    {
        Id = id;
        PostId = postId;
        NewsTypeId = newsTypeId;
        Place = place;
        Source = source;
        DateTime = dateTime;
        Status = status;
    }

    public News(NewsFull newsFull)
    {
        Id = newsFull.Id;
        PostId = newsFull.PostId;
        NewsTypeId = newsFull.NewsTypeId;
        Place = newsFull.Place;
        Source = newsFull.Source;
        DateTime = newsFull.DateTime;
        Status = newsFull.Status;
    }

    public void Update(News news)
    {
        NewsTypeId = news.NewsTypeId;
        Place = news.Place;
        Source = news.Source;
    }
}
