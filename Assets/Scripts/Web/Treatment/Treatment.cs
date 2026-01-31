using System;

public class Treatment
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public String Ingredients { get; set; }
    public String Preparation { get; set; }
    public String Usage { get; set; }
    public String Annotation { get; set; }
    public int Status { get; set; }

    public Treatment() { }

    public Treatment(long id, long postId, String ingredients, String preparation,
                        String usage, String annotation, int status)
    {
        Id = id;
        PostId = postId;
        Ingredients = ingredients;
        Preparation = preparation;
        Usage = usage;
        Annotation = annotation;
        Status = status;
    }

    public Treatment(TreatmentFull treatmentFull)
    {
        Id = treatmentFull.Id;
        PostId = treatmentFull.PostId;
        Ingredients = treatmentFull.Ingredients;
        Preparation = treatmentFull.Preparation;
        Usage = treatmentFull.Usage;
        Annotation = treatmentFull.Annotation;
        Status = treatmentFull.Status;
    }

    public void Update(Treatment treatment)
    {
        Ingredients = treatment.Ingredients;
        Preparation = treatment.Preparation;
        Usage = treatment.Usage;
        Annotation = treatment.Annotation;
    }
}
