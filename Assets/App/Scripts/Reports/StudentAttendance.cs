using System.Collections.Generic;

public class StudentAttendance
{
    public string FullName { get; set; }
    public string StudyForm { get; set; } // � / �
    public Dictionary<int, int> DailyHours { get; set; } = new(); // ���� -> ����
    public int Total => TotalHours(DailyHours);
    public int Valid { get; set; }
    public int Invalid => Total-Valid;

    private static int TotalHours(Dictionary<int, int> dailyHours) {
        int total = 0;
        for (int i = 1; i <= 30; i++)
        {
            dailyHours.TryGetValue(i, out int value);
            total += value;
        }
        return total;
    }
}
