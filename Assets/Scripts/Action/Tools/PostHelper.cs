using System;

public static class PostHelper
{
    public static String GetFeedDelay(TimeSpan timeSpan)
    {
        String sDelay = "hace ";
        int delay = 0;
        if (timeSpan.TotalDays >= 365)
        {
            delay = (int)(timeSpan.TotalDays / 365);
            sDelay += delay.ToString() + (delay > 1 ? " años" : " año");
        }
        else if (timeSpan.TotalDays > 30)
        {
            delay = (int)(timeSpan.TotalDays / 30);
            sDelay += delay.ToString() + (delay > 1 ? " meses" : " mes");
        }
        else if (timeSpan.TotalDays >= 7)
        {
            delay = (int)(timeSpan.TotalDays / 7);
            sDelay += delay.ToString() + (delay > 1 ? " semanas" : " semana");
        }
        else if (timeSpan.TotalDays >= 1)
        {
            delay = (int)timeSpan.TotalDays;
            sDelay += delay.ToString() + (delay > 1 ? " días" : " día");
        }
        else if (timeSpan.TotalHours >= 1)
        {
            delay = (int)timeSpan.TotalHours;
            sDelay += delay.ToString() + (delay > 1 ? " horas" : " hora");
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            delay = (int)timeSpan.TotalMinutes;
            sDelay += delay.ToString() + (delay > 1 ? " minutos" : " minuto");
        }
        else
            sDelay = "ahora";
        return sDelay;
    }
}
