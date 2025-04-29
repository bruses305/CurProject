using System;

public class StudentJustificationDocument
{
    public TypeJust typeJust { get; set; }
    public string Type => typeJust.ToString();
    public string Name { get; set; }
    public DateTime startJust { get; set; }
    public DateTime endJust { get; set; }

    public string Value => (startJust.Day+"."+ startJust.Month + "." + startJust.Year) + "-" + (endJust.Day + "." + endJust.Month + "." + endJust.Year);

    public enum TypeJust {
        Справка, Заявление, Распоряжение
    }

}
