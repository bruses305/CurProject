using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

public class AttendanceDocFiller
{
    public void CreateReport(string path, List<StudentAttendance> attendanceList, List<StudentJustificationDocument> justifications, List<StudentMakeupEntry> makeupEntries) {
        using (WordprocessingDocument doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());
            var body = mainPart.Document.Body;

            // 1. Таблица посещаемости
            body.Append(CreateAttendanceTable(attendanceList));
            body.Append(new Paragraph(new Run(new Break()))); // Разделитель

            // 2. Таблица справок
            body.Append(CreateJustificationTable(justifications));
            body.Append(new Paragraph(new Run(new Break())));

            // 3. Таблица отработок
            body.Append(CreateMakeupTable(makeupEntries));
            mainPart.Document.Save();
        }
    }

    private Table CreateAttendanceTable(List<StudentAttendance> students) {
        Table table = new Table();
        table.AppendChild(new TableProperties(new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 4 },
            new BottomBorder { Val = BorderValues.Single, Size = 4 },
            new LeftBorder { Val = BorderValues.Single, Size = 4 },
            new RightBorder { Val = BorderValues.Single, Size = 4 },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
        )));

        // Заголовок
        TableRow header = new TableRow();
        header.Append(CreateCell("№ п/п"));
        header.Append(CreateCell("ФИО"));
        header.Append(CreateCell("Форма обучения"));
        for (int i = 1; i <= 30; i++)
            header.Append(CreateCell(i.ToString()));
        header.Append(CreateCell("ИТОГО"));
        header.Append(CreateCell("По уваж."));
        header.Append(CreateCell("Без уваж."));
        table.Append(header);

        foreach (var s in students)
        {
            TableRow row = new TableRow();
            row.Append(CreateCell(s.Number.ToString()));
            row.Append(CreateCell(s.FullName));
            row.Append(CreateCell(s.StudyForm));
            for (int i = 1; i <= 30; i++)
                row.Append(CreateCell(s.DailyHours.ContainsKey(i) ? s.DailyHours[i] : ""));
            row.Append(CreateCell(s.Total));
            row.Append(CreateCell(s.Valid));
            row.Append(CreateCell(s.Invalid));
            table.Append(row);
        }

        return table;
    }

    private Table CreateJustificationTable(List<StudentJustificationDocument> list) {
        Table table = new Table();
        table.AppendChild(new TableProperties(new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 4 },
            new BottomBorder { Val = BorderValues.Single, Size = 4 },
            new LeftBorder { Val = BorderValues.Single, Size = 4 },
            new RightBorder { Val = BorderValues.Single, Size = 4 },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
        )));

        // Заголовки
        TableRow header = new TableRow();
        header.Append(CreateCell("Тип"));
        header.Append(CreateCell("ФИО"));
        header.Append(CreateCell("Дата / Номер"));
        table.Append(header);

        foreach (var doc in list)
        {
            TableRow row = new TableRow();
            row.Append(CreateCell(doc.Type));
            row.Append(CreateCell(doc.Name));
            row.Append(CreateCell(doc.Value));
            table.Append(row);
        }

        return table;
    }

    private Table CreateMakeupTable(List<StudentMakeupEntry> list) {
        Table table = new Table();
        table.AppendChild(new TableProperties(new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 4 },
            new BottomBorder { Val = BorderValues.Single, Size = 4 },
            new LeftBorder { Val = BorderValues.Single, Size = 4 },
            new RightBorder { Val = BorderValues.Single, Size = 4 },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
        )));

        TableRow header = new TableRow();
        header.Append(CreateCell("ФИО студента"));
        header.Append(CreateCell("Дисциплина"));
        header.Append(CreateCell("Часы отработки"));
        table.Append(header);

        foreach (var m in list)
        {
            TableRow row = new TableRow();
            row.Append(CreateCell(m.FullName));
            row.Append(CreateCell(m.Subject));
            row.Append(CreateCell(m.Hours));
            table.Append(row);
        }

        return table;
    }

    private TableCell CreateCell(string text) {
        return new TableCell(
            new Paragraph(new Run(new Text(text)))
        );
    }
}
