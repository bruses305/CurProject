using System.Collections.Generic;

public class Student
{
    public const string Key = "Students";
    public const string Key_NAME = "Name";
    public const string Key_ID = "ID";
    public const string Key_TYPE = "Type";
    public const string Key_CERTIFICATES = "Certificates";
    public const string Key_CERTIFICATESSTART = "dateStart";
    public string Name { get; set; }
    public int ID { get; set; }
    public bool Type { get; set; }
    public Dictionary<string,string> Certificates = new();
}
