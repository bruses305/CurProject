using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

public class AttendanceDocParser
{
    public List<StudentAttendance> ParseMainTable(Table table) {
        var result = new List<StudentAttendance>();
        var rows = table.Elements<TableRow>().ToList();

        for (int i = 2; i < rows.Count; i++) // пропускаем заголовки
        {
            var cells = rows[i].Elements<TableCell>().Select(c => c.InnerText.Trim()).ToList();
            if (cells.Count < 35) continue; // защита от пустых строк

            var student = new StudentAttendance
            {
                Number = int.TryParse(cells[0], out int n) ? n : 0,
                FullName = cells[1],
                StudyForm = cells[2],
                Total = cells[33],
                Valid = cells[34],
                Invalid = cells[35]
            };

            for (int d = 1; d <= 30; d++)
            {
                student.DailyHours[d] = cells[2 + d];
            }

            result.Add(student);
        }

        return result;
    }

    public List<StudentJustificationDocument> ParseJustifications(Table table) {
        var docs = new List<StudentJustificationDocument>();
        var rows = table.Elements<TableRow>().Skip(2); // первые 2 — заголовки

        foreach (var row in rows)
        {
            var cells = row.Elements<TableCell>().Select(c => c.InnerText.Trim()).ToList();
            if (cells.Count >= 6)
            {
                if (!string.IsNullOrWhiteSpace(cells[0]))
                    docs.Add(new StudentJustificationDocument { Type = "Справка", Name = cells[0], Value = cells[1] });
                if (!string.IsNullOrWhiteSpace(cells[2]))
                    docs.Add(new StudentJustificationDocument { Type = "Заявление", Name = cells[2], Value = cells[3] });
                if (!string.IsNullOrWhiteSpace(cells[4]))
                    docs.Add(new StudentJustificationDocument { Type = "Распоряжение", Name = cells[4], Value = cells[5] });
            }
        }

        return docs;
    }

    public List<StudentMakeupEntry> ParseMakeupTable(Table table) {
        var list = new List<StudentMakeupEntry>();
        var rows = table.Elements<TableRow>().Skip(1); // первая строка — заголовок

        foreach (var row in rows)
        {
            var cells = row.Elements<TableCell>().Select(c => c.InnerText.Trim()).ToList();
            if (cells.Count >= 3)
            {
                list.Add(new StudentMakeupEntry
                {
                    FullName = cells[0],
                    Subject = cells[1],
                    Hours = cells[2]
                });
            }
        }

        return list;
    }
}
