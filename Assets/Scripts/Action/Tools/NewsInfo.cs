using System;

public class NewsInfo
{
    public News News { get; set; }
    public String Image { get; set; }

    public NewsInfo()
    {
    }

    public NewsInfo(News news, String image)
    {
        News = news;
        Image = image;
    }
}
