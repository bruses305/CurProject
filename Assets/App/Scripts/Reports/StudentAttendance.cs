using System.Collections.Generic;

public class StudentAttendance
{
    public int Number { get; set; }
    public string FullName { get; set; }
    public string StudyForm { get; set; } // � / �
    public Dictionary<int, string> DailyHours { get; set; } = new(); // ���� -> ����
    public string Total { get; set; }
    public string Valid { get; set; }
    public string Invalid { get; set; }
}
