using System;

public class AliasRequest
{
    public long AppUserId { get; set; } = -1;
    public String Alias { get; set; }

    public AliasRequest()
    {
    }

    public AliasRequest(String alias)
    {
        Alias = alias;
    }

    public AliasRequest(long appUserId, String alias)
    {
        AppUserId = appUserId;
        Alias = alias;
    }
}
