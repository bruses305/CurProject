using System;

public static class Times
{
    public static DateTime TimeAdd3Day => DateTime.Now + new TimeSpan(3, 0, 0, 0);
    public static DateTime StartDayInMonth => DateTime.Today - new TimeSpan(DateTime.Today.Day - 1, 0, 0, 0);
    public static DateTime FbDefouldStartParsing => StartDayInMonth;
    public static DateTime Today => DateTime.Today;
    public static DateTime FbDefouldEndParsing => Today;
    public static DateTime PDefouldEndParsing => Today;
}