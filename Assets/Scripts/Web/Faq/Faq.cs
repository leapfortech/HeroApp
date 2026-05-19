using System;

public class Faq
{
    public long Id { get; set; }
    public long BoardUserId { get; set; }
    public long FaqTypeId { get; set; }
    public String Question { get; set; }
    public String Answer { get; set; }
    public int Status { get; set; }

    public Faq()
    { 
    }

    public Faq(long id, long boardUserId, long faqTypeId, String question, String answer, int status)
    {
        Id = id;
        BoardUserId = boardUserId;
        FaqTypeId = faqTypeId;
        Question = question;
        Answer = answer;
        Status = status;
    }
}
