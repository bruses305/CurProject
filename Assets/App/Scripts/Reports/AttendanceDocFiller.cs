using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UnityEngine;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;

public class AttendanceDocFiller : MonoBehaviour
{
    public void Create(string path) {
        StudentAttendance student = new()
        {
            FullName = "Лагун Сергей Сергеевич",
            StudyForm = "Б",
            Valid = 30,
        };
        student.DailyHours[3] = 4;
        CreateReport(path,
            new List<StudentAttendance>
        { student },
            new List<StudentJustificationDocument>
            { },
            new List<StudentMakeupEntry>
            { });
    }
    public void CreateReport(string path, List<StudentAttendance> attendanceList, List<StudentJustificationDocument> justifications, List<StudentMakeupEntry> makeupEntries) {
        //try
        //{
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
        //}
        //catch
        //{
        //    Debug.LogError("убедитесь что файл не открыт");
        //}
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
        header.Append(CreateDiagonalCell("Дата","ФИО"));
        header.Append(CreateCell("Форма обучения"));
        for (int i = 1; i <= 30; i++)
            header.Append(CreateCell(i.ToString()));
        header.Append(CreateRotatedCell("ИТОГО", 8f, Fonts.Calibri));
        AppendMergedTypeCell(table, header, out TableRow row2);
        table.Append(header);
        table.Append(row2);

        for (int idStudent = 0;idStudent< students.Count;idStudent++)
        {
            TableRow row = new TableRow();
            row.Append(CreateCell((idStudent+1).ToString()));
            row.Append(CreateCell(students[idStudent].FullName, 8f, Fonts.Calibri,false));
            row.Append(CreateCell(students[idStudent].StudyForm, 10f, Fonts.Calibri, false));
            for (int i = 1; i <= 30; i++)
                row.Append(CreateCell(students[idStudent].DailyHours.ContainsKey(i) ? students[idStudent].DailyHours[i].ToString() : "", 10f, Fonts.Calibri, false));
            row.Append(CreateCell(students[idStudent].Total.ToString(), 10f, Fonts.Calibri, false));
            row.Append(CreateCell(students[idStudent].Valid.ToString(), 10f, Fonts.Calibri, false));
            row.Append(CreateCell(students[idStudent].Invalid.ToString(), 10f, Fonts.Calibri, false));
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
        header.Append(CreateCell("Тип", 10f, Fonts.Calibri, false));
        header.Append(CreateCell("ФИО", 10f, Fonts.Calibri, false));
        header.Append(CreateCell("Дата / Номер", 10f, Fonts.Calibri, false));
        table.Append(header);

        foreach (var doc in list)
        {
            TableRow row = new TableRow();
            row.Append(CreateCell(doc.Type, 10f, Fonts.Calibri, false));
            row.Append(CreateCell(doc.Name, 10f, Fonts.Calibri, false));
            row.Append(CreateCell(doc.Value, 10f, Fonts.Calibri, false));
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
        header.Append(CreateCell("ФИО студента", 10f, Fonts.Calibri, false));
        header.Append(CreateCell("Дисциплина", 10f, Fonts.Calibri, false));
        header.Append(CreateCell("Количество часов отработки", 10f, Fonts.Calibri, false));
        table.Append(header);

        foreach (var m in list)
        {
            TableRow row = new TableRow();
            row.Append(CreateCell(m.FullName, 10f, Fonts.Calibri, false));
            row.Append(CreateCell(m.Subject, 10f, Fonts.Calibri, false));
            row.Append(CreateCell(m.Hours, 10f, Fonts.Calibri, false));
            table.Append(row);
        }

        return table;
    }

    private TableCell CreateCell(string text, float size = 7f, Fonts Font = Fonts.Arial, bool isBold = true, bool isItalic = true) {
        List<OpenXmlElement> openXmlElements = new() {
            new RunFonts { Ascii = Font.ToString() },
                    new FontSize { Val = (size * 2).ToString() },      // 12pt
                    new Color { Val = "000000" }      // чёрный
        };
        if (isBold) openXmlElements.Add(new Bold());
        if (isItalic) openXmlElements.Add(new Italic());

        return new TableCell(
            new Paragraph(new Run(new RunProperties(
                    openXmlElements
                ),
                new Text(text)))
        );
    }

    private TableCell CreateDiagonalCell(string text1, string text2, float size = 7f, Fonts Font = Fonts.Arial, bool isBold = true, bool isItalic = true) {
        List<OpenXmlElement> openXmlElements = new() {
            new RunFonts { Ascii = Font.ToString() },
                    new FontSize { Val = (size * 2).ToString() },      // 12pt
                    new Color { Val = "000000" }      // чёрный
        };
        if (isBold) openXmlElements.Add(new Bold());
        if (isItalic) openXmlElements.Add(new Italic());

        TableCell diagCell = new TableCell();
        diagCell.Append(new TableCellProperties(
            new TableCellBorders(
                new TopLeftToBottomRightCellBorder { Val = BorderValues.Single, Size = 4 }
            )
        ));

        // Содержимое: два параграфа, имитирующие текст "по диагонали"
        diagCell.Append(
        new Paragraph(
                new Run(new RunProperties(
                    openXmlElements
                ),
                new Text(text1)),
                new ParagraphProperties(new Justification { Val = JustificationValues.Right })

        ));
        diagCell.Append(new Paragraph(new Run()));
        diagCell.Append(new Paragraph(new Run()));
        diagCell.Append(
        new Paragraph(
                new Run(new Text("  " + text2), new RunProperties(openXmlElements)),
            new ParagraphProperties(new Justification { Val = JustificationValues.Left })
            )
        );
        return diagCell;
    }

    private void AppendMergedTypeCell(Table table, TableRow row1, out TableRow row2) {
        int totalColumns = 3 + 30 + 1; // ФИО + Форма + 30 дней + ИТОГО

        // Первая строка — верхняя ячейка "из них"


        var mergedCell = new TableCell(
            new Paragraph(new Run(new Text("из них")))
        );
        mergedCell.Append(new TableCellProperties(
            new GridSpan { Val = 2 }
        ));

        row1.Append(mergedCell);

        // Вторая строка — повёрнутые "По уваж." и "Без уваж."
        row2 = new TableRow();

        for (int i = 0; i < totalColumns; i++)
        {
            row2.Append(new TableCell(new Paragraph(new Run())));
        }

        row2.Append(CreateRotatedCell("По  уваж."));
        row2.Append(CreateRotatedCell("Без  уваж."));
    }

    private TableCell CreateRotatedCell(string text, float size = 7f, Fonts Font = Fonts.Arial, bool isBold = true, bool isItalic = true) {
        List<OpenXmlElement> openXmlElements = new() {
            new RunFonts { Ascii = Font.ToString() },
                    new FontSize { Val = (size * 2).ToString() },      // 12pt
                    new Color { Val = "000000" }      // чёрный
        };
        if (isBold) openXmlElements.Add(new Bold());
        if (isItalic) openXmlElements.Add(new Italic());
        return new TableCell(
            new Paragraph(new Run(new RunProperties(
                    openXmlElements
                ), 
                new Text(text)))
        )
        {
            TableCellProperties = new TableCellProperties(
                new TextDirection { Val = TextDirectionValues.BottomToTopLeftToRight}
            )
        };
    }
    private enum Fonts
    {
        Arial = 0,
        Calibri = 1,
    }
}
