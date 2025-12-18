using System;

public class Disease
{
    public long Id { get; set; }
    public long TreatmentId { get; set; }
    public long DiseaseTypeId { get; set; }
    public int Status { get; set; }

    public Disease() 
    {
    }

    public Disease(long id, long treatmentId, long diseaseTypeId, int status)
    {
        Id = id;
        TreatmentId = treatmentId;
        DiseaseTypeId = diseaseTypeId;
        Status = status;
    }
}
