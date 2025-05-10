using System;
using System.Collections.Generic;
using System.Diagnostics;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using Debug = UnityEngine.Debug;

public static class AttendanceDocFiller
{
    private static bool _isOpenToEnd;
    private const int Key_CM = 567;
    private const float Key_TOP_BODY = 0.75f;
    private const float Key_BUTTON_BODY = 0.5f;
    private const float Key_LEFT_BODY = 1.27f;
    private const float Key_RIGHT_BODY = 1.27f;
    private const float Key_LEFT_TABLE = -0.76f;
    private const float Key_HEIGHT_ROWS = (0.42f * Key_CM);
    public static void Create(string path,List<StudentAttendance> students,List<StudentJustificationDocument> jasts,List<StudentMakeupEntry> makeup, bool isOpenToEnd = false) {
        _isOpenToEnd = isOpenToEnd;
        CreateReport(path,
            students,
            jasts,
            makeup);
    }
    private static void CreateReport(string path, List<StudentAttendance> attendanceList, List<StudentJustificationDocument> justifications, List<StudentMakeupEntry> makeupEntries) {
        try
        {
            using (WordprocessingDocument
                   doc = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                var body = new Body();
                mainPart.Document = new Document(body);
                // Добавляем настройки страницы
                body.AppendChild(new SectionProperties(
                    new PageMargin
                    {
                        Top = Convert.ToInt32(Key_CM * Key_TOP_BODY),
                        Bottom = Convert.ToInt32(Key_CM * Key_BUTTON_BODY),
                        Left = Convert.ToUInt32(Key_CM * Key_LEFT_BODY),
                        Right = Convert.ToUInt32(Key_CM * Key_RIGHT_BODY)
                    }
                ));

                CreateTitle1(body);
                // 1. Таблица посещаемости
                body.Append(CreateAttendanceTable(attendanceList));

                body.Append(CreateParagraph(" ", size: 10)); // Разделитель

                // 2. Таблица справок
                body.Append(CreateJustificationTable(justifications));
                CreateTitle2(body);

                // 3. Таблица отработок
                body.Append(CreateMakeupTable(makeupEntries));
                mainPart.Document.Save();
                CreateTitle3(body);
            }

            if (_isOpenToEnd) Process.Start(path);
        }
        catch
        {
            Debug.LogError("убедитесь что файл не открыт");
        }
    }

    private static Table CreateAttendanceTable(List<StudentAttendance> students) {
        Table table = new Table();
        table.AppendChild(new TableProperties(
             new TableIndentation
             {
                 Type = TableWidthUnitValues.Dxa, // можно также Pct
                 Width = Convert.ToInt32(Key_CM * Key_LEFT_TABLE) // 720 dxa = 0.5 дюйма ≈ 1.27 см
             },
            new TableLayout { Type = TableLayoutValues.Fixed }, 
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
        )));

        

        
        // Заголовок
        TableRow header = new TableRow();
        header.Append(CreateCell("№ п/п", widthDxa: 0.44f));
        header.Append(CreateDiagonalCell("Дата","ФИО", widthDxa: 2.91f));
        header.Append(CreateCell("Форма обучения", widthDxa: 0.44f));
        for (int i = 1; i <= 30; i++)
            header.Append(CreateCell(i.ToString(), widthDxa: 0.44f,isCenter: true));
        header.Append(CreateRotatedCell("ИТОГО", 8, Fonts.Calibri, widthDxa: 0.44f));
        header.Append(CreateRotatedCell("По  уваж.", 8, Fonts.Calibri, widthDxa: 0.44f));
        header.Append(CreateRotatedCell("Без  уваж.", 8, Fonts.Calibri, widthDxa: 0.44f));
        table.Append(header);
        
        for (int idStudent = 0;idStudent< students.Count;idStudent++)
        {
            TableRow row = new TableRow();
            row.AppendChild(new TableRowProperties(
    new TableRowHeight { Val = Convert.ToUInt32(Key_HEIGHT_ROWS), HeightType = HeightRuleValues.Exact } // или Exact
));
            row.Append(CreateCell((idStudent+1).ToString(), widthDxa: 0.44f));
            row.Append(CreateCell(students[idStudent].FullName, 8, Fonts.Calibri,false, widthDxa: 2.91f));
            row.Append(CreateCell(students[idStudent].StudyForm, 10, Fonts.Calibri, false, widthDxa: 0.44f));
            for (int i = 1; i <= 30; i++)
                row.Append(CreateCell(students[idStudent].DailyHours.ContainsKey(i) ? students[idStudent].DailyHours[i].ToString() : "", 10, Fonts.Calibri, false, widthDxa: 0.44f));
            row.Append(CreateCell(students[idStudent].Total.ToString(), 10, Fonts.Calibri, false, widthDxa: 0.87f));
            row.Append(CreateCell(students[idStudent].Valid.ToString(), 10, Fonts.Calibri, false, widthDxa: 0.75f));
            row.Append(CreateCell(students[idStudent].Invalid.ToString(), 10, Fonts.Calibri, false, widthDxa: 0.75f));
            table.Append(row);
        }

        return table;
    }

    private static Table CreateJustificationTable(List<StudentJustificationDocument> list) {
        Table table = new Table();
        table.AppendChild(new TableProperties(
            new TableIndentation
            {
                Type = TableWidthUnitValues.Dxa, // можно также Pct
                Width = Convert.ToInt32(Key_CM * Key_LEFT_TABLE) // 720 dxa = 0.5 дюйма ≈ 1.27 см
            },
            new TableLayout { Type = TableLayoutValues.Fixed }, 
            new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 4 },
            new BottomBorder { Val = BorderValues.Single, Size = 4 },
            new LeftBorder { Val = BorderValues.Single, Size = 4 },
            new RightBorder { Val = BorderValues.Single, Size = 4 },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
        )));

        // Заголовки
        TableRow header = new TableRow();
        header.AppendChild(new TableRowProperties(
    new TableRowHeight { Val = Convert.ToUInt32(Key_HEIGHT_ROWS), HeightType = HeightRuleValues.Exact } // или Exact
));
        header.Append(CreateCellGrou("Справки", 2, 10, Fonts.Calibri, widthDxa: 7f,justificationValues: JustificationValues.Center));
        header.Append(CreateCellGrou("Заявления", 2, 10, Fonts.Calibri, widthDxa: 6.22f, justificationValues: JustificationValues.Center));
        header.Append(CreateCellGrou("Распоряжения", 2, 10, Fonts.Calibri, widthDxa: 6.25f, justificationValues: JustificationValues.Center));

        TableRow header2 = new TableRow();
        header2.AppendChild(new TableRowProperties(
    new TableRowHeight { Val = Convert.ToUInt32(Key_HEIGHT_ROWS), HeightType = HeightRuleValues.Exact } // или Exact
));
        header2.Append(CreateCell("Ф.И.О.", 10, Fonts.Calibri, false, justificationValues: JustificationValues.Center, widthDxa: 3.25f));
        header2.Append(CreateCell("Дата",  10, Fonts.Calibri, false, justificationValues: JustificationValues.Center, widthDxa: 3.75f));

        header2.Append(CreateCell("Ф.И.О.", 10, Fonts.Calibri, false, justificationValues: JustificationValues.Center, widthDxa: 3f));
        header2.Append(CreateCell("Дата", 10, Fonts.Calibri, false, justificationValues: JustificationValues.Center, widthDxa: 3.22f));

        header2.Append(CreateCell("Ф.И.О.", 10, Fonts.Calibri, false, justificationValues: JustificationValues.Center, widthDxa: 3.28f));
        header2.Append(CreateCell("Номер и дата", 10, Fonts.Calibri, false, justificationValues: JustificationValues.Center, widthDxa: 2.96f));

        table.Append(header);
        table.Append(header2);

        for (;list.Count > 0;)
        {
            TableRow row = new TableRow();
            row.AppendChild(new TableRowProperties(
                new TableRowHeight { Val = Convert.ToUInt32(Key_HEIGHT_ROWS), HeightType = HeightRuleValues.Exact } // или Exact
            ));

            StudentJustificationDocument justS = list.Find(obj=>obj.typeJust == StudentJustificationDocument.TypeJust.Справка);
            row.Append(CreateCell(justS?.Name, 10, Fonts.Calibri, false));
            row.Append(CreateCell(justS?.Value, 10, Fonts.Calibri, false));

            list.Remove(justS);
            
            StudentJustificationDocument justZ = list.Find(obj=>obj.typeJust == StudentJustificationDocument.TypeJust.Заявление);
            row.Append(CreateCell(justZ?.Name, 10, Fonts.Calibri, false));
            row.Append(CreateCell(justZ?.Value, 10, Fonts.Calibri, false));

            list.Remove(justZ);
            
            StudentJustificationDocument justR = list.Find(obj=>obj.typeJust == StudentJustificationDocument.TypeJust.Распоряжение);
            row.Append(CreateCell(justR?.Name, 10, Fonts.Calibri, false));
            row.Append(CreateCell(justR?.Value, 10, Fonts.Calibri, false));

            list.Remove(justR);

            table.Append(row);
        }

        return table;
    }

    private static Table CreateMakeupTable(List<StudentMakeupEntry> list) {
        Table table = new Table();
        table.AppendChild(new TableProperties(
            new TableLayout { Type = TableLayoutValues.Fixed },
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4 },
                new BottomBorder { Val = BorderValues.Single, Size = 4 },
                new LeftBorder { Val = BorderValues.Single, Size = 4 },
                new RightBorder { Val = BorderValues.Single, Size = 4 },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
        )));

        TableRow header = new TableRow();

        header.Append(CreateCell("ФИО студента", 10, Fonts.Calibri, false, widthDxa: 4.93f));
        header.Append(CreateCell("Дисциплина", 10, Fonts.Calibri, false, widthDxa: 10.23f, justificationValues: JustificationValues.Center));
        header.Append(CreateCell("Количество часов отработки", 10, Fonts.Calibri, false, widthDxa: 3.79f, justificationValues: JustificationValues.Center));
        table.Append(header);

        foreach (var m in list)
        {
            TableRow row = new TableRow();
            row.AppendChild(new TableRowProperties(
    new TableRowHeight { Val = Convert.ToUInt32(Key_HEIGHT_ROWS), HeightType = HeightRuleValues.Exact } // или Exact
));
            row.Append(CreateCell(m.FullName, 10, Fonts.Calibri, false, widthDxa: 4.93f));
            row.Append(CreateCell(m.Subject, 10, Fonts.Calibri, false, widthDxa: 10.23f));
            row.Append(CreateCell(m.Hours.ToString(), 10, Fonts.Calibri, false, widthDxa: 3.79f));
            table.Append(row);
        }

        return table;
    }

    private static void CreateTitle1(Body body) {
        body.Append(CreateParagraph("ВЕДОМОСТЬ", justificationValues: JustificationValues.Center));
        body.Append(CreateParagraph("учета учебных часов, пропущенных студентами", justificationValues: JustificationValues.Center));
        body.Append(CreateParagraph("за _________________ месяц  202__ г.", justificationValues: JustificationValues.Center));
        body.Append(CreateCustomParagraph("Курс   _____   ГРУППА   ", ".                             ."));
        body.Append(CreateParagraph(" ", size: 6));
    }
    private static void CreateTitle2(Body body) {
        body.Append(CreateParagraph(" ", size: 6));
        body.Append(CreateParagraph("Дата                     _______________", size: 10));
        body.Append(CreateParagraph("Староста:          _______________         ___________________", size: 10));
        body.Append(CreateParagraph("                                                                                                  (Ф.И.О.)", size: 9));
        body.Append(CreateParagraph("Куратор:              _______________        ___________________", size: 10));
        body.Append(CreateParagraphBreak("                                                                                                  (Ф.И.О.)", size: 9));

        body.Append(CreateParagraph("ВЕДОМОСТЬ УЧЕТА ПЛАТНЫХ ОТРАБОТОК ЗА ___________________ 202__ г., курс   _______   Группа   ________________", size: 10));
        body.Append(CreateParagraph("                                                                                                         месяц", size: 10, isBold: false));
    }
    private static void CreateTitle3(Body body) {
        body.Append(CreateParagraph(" ", size: 6));
        body.Append(CreateParagraph("Дата                     _______________", size: 10));
        body.Append(CreateParagraph("Староста:          _______________         ___________________", size: 10));
        body.Append(CreateParagraph("                                                                                                  (Ф.И.О.)", size: 9));
        body.Append(CreateParagraph("Куратор:              _______________        ___________________", size: 10));
        body.Append(CreateParagraph("                                                                                                  (Ф.И.О.)", size: 9));
    }

    private static Paragraph CreateParagraphBreak(string text, int size = 12, Fonts font = Fonts.Calibri, bool isBold = true, bool isItalic = true, JustificationValues? justificationValues = null) {
        Paragraph paragraph = CreateParagraph(text,size,font,isBold,isItalic,justificationValues);
        paragraph.Append(new Run(new Break() { Type = BreakValues.Page }));
        return paragraph;
    }
    private static Paragraph CreateParagraph(string text, int size = 12, Fonts font = Fonts.Calibri, bool isBold = true, bool isItalic = true, JustificationValues? justificationValues = null) {
        Paragraph paragraph = new Paragraph(
            new ParagraphProperties(
                new SpacingBetweenLines
                {
                    Before = "0",   // интервал до (в 1/20 pt) — 200 = 10 pt
                    After = "0",    // интервал после — тоже 10 pt
                    Line = "0",     // межстрочный интервал — 240 = 12 pt
                    LineRule = LineSpacingRuleValues.Exact // или Exactly, AtLeast
                },
                new Justification { Val = justificationValues ?? JustificationValues.Left }
            ),
            new Run(
                new RunProperties(CreateRunProperties(size: size, isBold: isBold)),
            new Text(text) { Space = SpaceProcessingModeValues.Preserve }
            )
        );
        return paragraph;
    }
    private static Paragraph CreateCustomParagraph(string text1,string text2, int size = 12, Fonts font = Fonts.Calibri, bool isBold = true, bool isItalic = true, JustificationValues? justificationValues = null) {

        Paragraph paragraph = new Paragraph(
            new ParagraphProperties(
                new SpacingBetweenLines
                {
                    Before = "0",   // интервал до (в 1/20 pt) — 200 = 10 pt
                    After = "0",    // интервал после — тоже 10 pt
                    Line = "0",     // межстрочный интервал — 240 = 12 pt
                    LineRule = LineSpacingRuleValues.Exact // или Exactly, AtLeast
                },
                new Justification { Val = justificationValues != null ? justificationValues : JustificationValues.Left }
            ),
            new Run(
                new RunProperties(CreateRunProperties()),
            new Text(text1) { Space = SpaceProcessingModeValues.Preserve }
            ),

            new Run(
                    new RunProperties(new Border { Val = BorderValues.BasicThinLines }),
                    new Text(text2) { Space = SpaceProcessingModeValues.Preserve }
            )
        );
        return paragraph;
    }
    private static TableCell CreateCell(string text, int size = 7, Fonts font = Fonts.Arial, bool isBold = true, bool isItalic = true, float? widthDxa = null, bool isCenter = false, JustificationValues? justificationValues = null) {
        var props = new List<OpenXmlElement>
    {
        new RunFonts
    {
        Ascii = RetFont(font),
        HighAnsi = RetFont(font),
        EastAsia = RetFont(font),
        ComplexScript = RetFont(font)
    },
        new FontSize { Val = (size * 2).ToString() },
        new Color { Val = "000000" }
    };

        if (isBold) props.Add(new Bold());
        if (isItalic) props.Add(new Italic());

        var cellProps = new TableCellProperties();
        if (widthDxa.HasValue) cellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = (Convert.ToInt32(widthDxa.Value * Key_CM)).ToString(), });
        if (isCenter) cellProps.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });

        var cell = new TableCell(
            new Paragraph(new ParagraphProperties(
                new Justification { Val = justificationValues ?? JustificationValues.Left }
            ),
                new Run(
                    new RunProperties(props),
                    new Text(text) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
        );

        if (widthDxa.HasValue)
        {
            cell.Append(cellProps);
        }

        return cell;
    }
    private static TableCell CreateCellGrou(string text,int grouCount = 1, int size = 7, Fonts font = Fonts.Arial, bool isBold = true, bool isItalic = true, float? widthDxa = null, bool isCenter = false, JustificationValues? justificationValues = null) {
        var props = new List<OpenXmlElement>
    {
        new RunFonts
    {
        Ascii = RetFont(font),
        HighAnsi = RetFont(font),
        EastAsia = RetFont(font),
        ComplexScript = RetFont(font)
    },
        new FontSize { Val = (size * 2).ToString() },
        new Color { Val = "000000" }
    };
        if (isBold) props.Add(new Bold());
        if (isItalic) props.Add(new Italic());

        var cellProps = new TableCellProperties();
        if (widthDxa.HasValue) cellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = (Convert.ToInt32(widthDxa.Value * Key_CM)).ToString(), });
        if (isCenter) cellProps.Append(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });

        var cell = new TableCell(
            new TableCellProperties(
                    new TableCellProperties(
                    new GridSpan { Val = 2 }
                )),
            new Paragraph(
                new ParagraphProperties(
                new Justification { Val = justificationValues ?? JustificationValues.Left }
            ),
                new Run(
                    new RunProperties(props),
                    new Text(text) { Space = SpaceProcessingModeValues.Preserve }
                )
            )
        );

        if (widthDxa.HasValue)
        {
            cell.Append(cellProps);
        }

        return cell;
    }
    private static TableCell CreateDiagonalCell(string text1, string text2, int size = 7, Fonts font = Fonts.Arial, bool isBold = true, bool isItalic = true,float? widthDxa = null, JustificationValues? justificationValues = null) {
        List<OpenXmlElement> openXmlElements = new() {
            new RunFonts
    {
        Ascii = RetFont(font),
        HighAnsi = RetFont(font),
        EastAsia = RetFont(font),
        ComplexScript = RetFont(font)
    },
                    new FontSize { Val = (size * 2).ToString() },      // 12pt
                    new Color { Val = "000000" }      // чёрный
        };
        if (isBold) openXmlElements.Add(new Bold());
        if (isItalic) openXmlElements.Add(new Italic());

        var cellProps = new TableCellProperties();
        if (widthDxa.HasValue)
        {
            cellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = (Convert.ToInt32(widthDxa.Value * Key_CM)).ToString(), });
        }

        TableCell diagCell = new TableCell();

        if (widthDxa.HasValue)
            diagCell.Append(cellProps);

        diagCell.Append(new TableCellProperties(
            new TableCellBorders(
                new TopLeftToBottomRightCellBorder { Val = BorderValues.Single, Size = 4 }
            )
        ));
        var runProps1 = new RunProperties();
        foreach (var el in openXmlElements)
            runProps1.Append(el.CloneNode(true)); // важно использовать CloneNode(true)

        var paragraph1 = new Paragraph(
            new Run(runProps1, new Text(text1))
        )
        {
            ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Right })
        };

        var runProps2 = new RunProperties();
        foreach (var el in openXmlElements)
            runProps2.Append(el.CloneNode(true)); // важно использовать CloneNode(true)

        var paragraph2 = new Paragraph(
            new Run(runProps2, new Text(text2))
        )
        {
            ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left })
        };
        // Содержимое: два параграфа, имитирующие текст "по диагонали"
        diagCell.Append(paragraph1);
        diagCell.Append(paragraph2);

        
        return diagCell;
    }
    private static TableCell CreateRotatedCell(string text, int size = 7, Fonts font = Fonts.Arial, bool isBold = true, bool isItalic = true, float? widthDxa = null, JustificationValues? justificationValues = null) {
        List<OpenXmlElement> openXmlElements = new() {
            new RunFonts
    {
        Ascii = RetFont(font),
        HighAnsi = RetFont(font),
        EastAsia = RetFont(font),
        ComplexScript = RetFont(font)
    },
                    new FontSize { Val = (size * 2).ToString() },      // 12pt
                    new Color { Val = "000000" }      // чёрный
        };
        if (isBold) openXmlElements.Add(new Bold());
        if (isItalic) openXmlElements.Add(new Italic());

        var cellProps = new TableCellProperties();
        if (widthDxa.HasValue)
        {
            cellProps.Append(new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = (Convert.ToInt32(widthDxa.Value * Key_CM)).ToString(), });
        }

        TableCell cell = new TableCell(
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
        cell.Append(cellProps);
        return cell;
    }
    private static List<OpenXmlElement> CreateRunProperties(IEnumerable<OpenXmlElement> etc = null, int size = 12, Fonts font = Fonts.Calibri, bool isBold = true, bool isItalic = true) {
        var props = new List<OpenXmlElement>
        {
            new RunFonts
            {
                Ascii = RetFont(font),
                HighAnsi = RetFont(font),
                EastAsia = RetFont(font),
                ComplexScript = RetFont(font)
            },
            new FontSize { Val = (size * 2).ToString() },
            new Color { Val = "000000" },
        };

        if (etc != null)
        {
            Debug.Log("Dop Element");
            foreach (OpenXmlElement e in etc)
                props.Add(e);
        }

        if (isBold) props.Add(new Bold());
        if (isItalic) props.Add(new Italic());

        return props;
    }
    private enum Fonts
    {
        Arial = 0,
        Calibri = 1,
    }
    private static string RetFont(Fonts font) {
        switch (font)
        {
            case Fonts.Arial:
                return "Arial";
            case Fonts.Calibri:
                return "Calibri";
            default:
                return "Times New Roman";
        }
    }
}
