using System;
using UnityEngine;

using Leap.Core.Tools;
using Leap.Vision.Tools;

public class DpiFront
{
    public String CUI { get; set; }
    public String FirstName1 { get; set; }
    public String FirstName2 { get; set; }
    public String FirstName3 { get; set; }
    public String LastName1 { get; set; }
    public String LastName2 { get; set; }
    public int Gender { get; set; }
    public int MaritalStatus { get; set; }
    public String LastNameMarried { get; set; }
    public String Nationality { get; set; }
    public DateTime BirthDate { get; set; }
    public String BirthCountry { get; set; }
    public DateTime IssueDate { get; set; }
    public String IdentificationCountry { get; set; }
    public Texture2D Portrait { get; set; }

    public DpiFront()
    {
    }

    public DpiFront(String cui, String firstName1, String firstName2, String firstName3, String lastName1, String lastName2, int gender, int maritalStatus, String lastNameMarried,
                    String nationality, DateTime birthDate, String birthCountry, DateTime issueDate, String identificationCountry, Texture2D portrait)
    {
        CUI = cui;
        FirstName1 = firstName1;
        FirstName2 = firstName2;
        FirstName3 = firstName3;
        LastName1 = lastName1;
        LastName2 = lastName2;
        Gender = gender;
        MaritalStatus = maritalStatus;
        LastNameMarried = lastNameMarried;
        Nationality = nationality;
        BirthDate = birthDate;
        BirthCountry = birthCountry;
        IssueDate = issueDate;
        IdentificationCountry = identificationCountry;
        Portrait = portrait;
    }

    public DpiFront(VisionDpiFrontResponse visionDpiFrontResponse, int gender, int maritalStatus)
    {
        CUI = visionDpiFrontResponse.Result.Cui;
        FirstName1 = visionDpiFrontResponse.Result.FirstName1;
        FirstName2 = visionDpiFrontResponse.Result.FirstName2;
        FirstName3 = visionDpiFrontResponse.Result.FirstName3;
        LastName1 = visionDpiFrontResponse.Result.LastName1;
        LastName2 = visionDpiFrontResponse.Result.LastName2;
        Gender = visionDpiFrontResponse.Result.Gender == null ? -1 : gender;
        MaritalStatus = visionDpiFrontResponse.Result.MaritalStatus == null ? -1 : maritalStatus;
        LastNameMarried = visionDpiFrontResponse.Result.LastNameMarried;
        Nationality = visionDpiFrontResponse.Result.Nationality;
        BirthDate = visionDpiFrontResponse.Result.BirthDate == null ? new DateTime(1, 1, 1) : (DateTime)ConvertDate(visionDpiFrontResponse.Result.BirthDate);
        BirthCountry = visionDpiFrontResponse.Result.BirthCountry;
        IssueDate = visionDpiFrontResponse.Result.IssueDate == null ? new DateTime(1, 1, 1) : (DateTime)ConvertDate(visionDpiFrontResponse.Result.IssueDate);
        IdentificationCountry = visionDpiFrontResponse.Result.IdentificationCountry;

        if (visionDpiFrontResponse.Result.Face != null)
        {
            Texture2D photoTex = new Texture2D(2, 2);
            photoTex.LoadImage(Convert.FromBase64String(visionDpiFrontResponse.Result.Face));
            Portrait = photoTex;
        }
        else
            Portrait = null;
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
