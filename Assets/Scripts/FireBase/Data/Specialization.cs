using System.Collections.Generic;

public class Specialization : IFireBaseData
{
    public const string key = "Specializations";
    public string Key { get => key; }
    public const string Key_NAME = "Name";
    public string Name { get; set; }
    public Dictionary<string, Year> Years = new();
}
