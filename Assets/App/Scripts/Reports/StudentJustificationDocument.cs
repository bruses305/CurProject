public class StudentJustificationDocument
{
    public TypeJust typeJust { get; set; }
    public string Type => GetType(); // �������, ���������, ������������
    public string Name { get; set; }
    public string Value { get; set; } // ���� ��� �����

    public enum TypeJust {
        �������, ���������, ������������
    }

    public string GetType() {
        return typeJust.ToString();
    }

}
