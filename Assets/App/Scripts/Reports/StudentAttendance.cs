using System.Collections.Generic;

public class StudentAttendance
{
    public string FullName { get; set; }
    public string StudyForm { get; set; } // Б / П
    public Dictionary<int, int> DailyHours { get; set; } = new(); // День -> часы
    public int Total => TotalHours(DailyHours);
    public int Valid { get; set; }
    public int Invalid => Total-Valid;

    public static int TotalHours(Dictionary<int, int> DailyHours) {
        int Total = 0;
        for (int i = 1; i <= 30; i++)
        {
            DailyHours.TryGetValue(i, out int value);
            Total += value;
        }
        return Total;
    }
}
