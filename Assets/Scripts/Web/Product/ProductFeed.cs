using System;
using UnityEngine;

using Leap.Graphics.Tools;

public class ProductFeed
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public String TitleImage
    {
        get => null;
        set => TitleSprite = value?.CreateSprite("News_" + PostId.ToString("D02"));
    }
    public Sprite TitleSprite { get; set; } = null;
    public String Title { get; set; }
    public String ProductSubtype { get; set; }
    public String SaleCountry { get; set; }
    public String SaleState { get; set; }
    public String Currency { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public String Link { get; set; }
    public int Favorite { get; set; }

    public DateTime PublicationDateTime { get; set; } = new DateTime(1753, 1, 1);
}
