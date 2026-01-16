using System;

public class Disease
{
    public long Id { get; set; } = -1;
    public long TreatmentId { get; set; } = -1;
    public long DiseaseTypeId { get; set; } = -1;
    public int Status { get; set; } = -1;

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
