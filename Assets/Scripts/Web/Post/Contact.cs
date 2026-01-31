using System;

public class Contact
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public String Name { get; set; }
    public int Status { get; set; }

    public Contact() 
    { 
    }

    public Contact(long id, long postId, String name, int status)
    {
        Id = id;
        PostId = postId;
        Name = name;
        Status = status;
    }

    public Contact(ContactFull contactFull)
    {
        Id = contactFull.Id;
        PostId = contactFull.PostId;
        Name = contactFull.Name;
        Status = contactFull.Status;
    }

    public void Update(Contact contact)
    {
        Name = contact.Name;
    }
}
