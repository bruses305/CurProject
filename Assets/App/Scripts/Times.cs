using System;

public static class Times
{
    public static DateTime TimeAdd3Day => DateTime.Now + new TimeSpan(3, 0, 0, 0);
    public static DateTime StartDayInTotalMonth => DateTime.Today - new TimeSpan(DateTime.Today.Day - 1, 0, 0, 0);
    public static DateTime FbDefouldStartParsing => StartDayInTotalMonth;
    public static DateTime Today => DateTime.Today;
    public static DateTime FbDefouldEndParsing => Today;
    public static DateTime PDefouldEndParsing => Today;

    public static DateTime StartDayInMonth(DateTime? date)
    {
        date ??= DateTime.Today;
        return date.Value - new TimeSpan(date.Value.Day - 1, 0, 0, 0);
    }

}