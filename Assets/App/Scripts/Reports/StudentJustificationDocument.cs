public class StudentJustificationDocument
{
    public TypeJust typeJust { get; set; }
    public string Type => GetType(); // Справка, Заявление, Распоряжение
    public string Name { get; set; }
    public string Value { get; set; } // Дата или Номер

    public enum TypeJust {
        Справка, Заявление, Распоряжение
    }

    public string GetType() {
        return typeJust.ToString();
    }

}
