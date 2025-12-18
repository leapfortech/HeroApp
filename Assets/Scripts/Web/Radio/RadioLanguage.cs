using System;

public class RadioLanguage
{
    public long Id { get; set; }
    public long RadioId { get; set; }
    public long LanguageTypeId { get; set; }
    public int Status { get; set; }

    public RadioLanguage()
    {
    }

    public RadioLanguage(long id, long radioId, long languageTypeId, int status)
    {
        Id = id;
        RadioId = radioId;
        LanguageTypeId = languageTypeId;
        Status = status;
    }
}
