using System;
using Leap.Core.Tools;
using Leap.Vision.Tools;

public class DpiBack
{
    public String BirthCountry { get; set; }
    public String BirthDepartment { get; set; }
    public String BirthTownship { get; set; }

    public int MaritalStatus { get; set; }
    public DateTime DueDate { get; set; }

    public String ResidenceCountry { get; set; }
    public String ResidenceDepartment { get; set; }
    public String ResidenceTownship { get; set; }

    public String MRZ { get; set; }

    public DpiBack()
    {
    }

    public DpiBack(String birthCountry, String birthDepartment, String birthTownship, int maritalStatus, DateTime dueDate,
                   String residenceCountry, String residenceDepartment, String residenceTownship, String mrz)
    {
        BirthCountry = birthCountry;
        BirthDepartment = birthDepartment;
        BirthTownship = birthTownship;

        MaritalStatus = maritalStatus;
        DueDate = dueDate;

        ResidenceCountry = residenceCountry;
        ResidenceDepartment = residenceDepartment;
        ResidenceTownship = residenceTownship;

        MRZ = mrz;
    }

    public DpiBack(VisionDpiBackResponse visionDpiBackResponse, int maritalStatus)
    {
        BirthCountry = visionDpiBackResponse.Result.BirthCountry;
        BirthDepartment = visionDpiBackResponse.Result.BirthState;
        BirthTownship = visionDpiBackResponse.Result.BirthCity;

        MaritalStatus = visionDpiBackResponse.Result.MaritalStatus == null ? -1 : maritalStatus;
        DueDate = visionDpiBackResponse.Result.DueDate == null ? new DateTime(1, 1, 1) : (DateTime)ConvertDate(visionDpiBackResponse.Result.DueDate);

        ResidenceCountry = visionDpiBackResponse.Result.ResidenceCountry;
        ResidenceDepartment = visionDpiBackResponse.Result.ResidenceState;
        ResidenceTownship = visionDpiBackResponse.Result.ResidenceCity;

        MRZ = visionDpiBackResponse.Result.Mrz;
    }

    public static DateTime ConvertDate(String date)
    {
        if (!date[1].IsDigit())
            date = "0" + date;
        if (!date[4].IsDigit())
            date = date[..3] + "0" + date[3..];
        return new DateTime(Convert.ToInt32(date[6..10]), Convert.ToInt32(date[3..5]), Convert.ToInt32(date[..2]));
    }
}
