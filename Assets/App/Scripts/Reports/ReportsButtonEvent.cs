using UnityEngine;

public class ReportsButtonEvent : MonoBehaviour
{
    public string path;
    
    public void Vedomost1()
    {
        AttendanceDocFiller.Create(path,ConverterDataToReports.StudentAttendance(),ConverterDataToReports.StudentJustificationDocument(),ConverterDataToReports.StudentMakeupEntry(),true);
    }
}
