using System;

public class RadioLanguage
{
    public long Id { get; set; }
    public long RadioId { get; set; }
    public long LanguageId { get; set; }
    public int Status { get; set; }

    public RadioLanguage()
    {
    }

    public RadioLanguage(long id, long radioId, long languageId, int status)
    {
        Id = id;
        RadioId = radioId;
        LanguageId = languageId;
        Status = status;
    }
}
