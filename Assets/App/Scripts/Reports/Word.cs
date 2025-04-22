using UnityEngine;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;

public class Word : MonoBehaviour
{
    public int countStudent = 0;

    private string[] NameCells = {
        "№ п/п","         \r\n                       Дата\r\n\r\n\r","        ФИО\r\n", "форма\r\nобучения\r\n(бюджет/платно)\r\n", "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9",  "10","11",  "12",  "13",  "14",  "15",  "16",  "17",  "18",  "19",  "20","21",  "22",  "23",  "24",  "25",  "26",  "27",  "28",  "29",  "30","ИТОГО","из них"
    };
    private const int countCellRowTable1 = 36;
    public void unlock() {
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open("Ved.docx", true))
        {
            try
            {
                ClearDocument(wordDoc);
                
                var body = wordDoc.MainDocumentPart.Document.Body;

                Table table = new Table();

                // Границы таблицы
                table.AppendChild(new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder { Val = BorderValues.Single, Size = 4 },
                        new RightBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                    )
                ));
                Title1PreData(table);

                Table1PreData(table);

                for(int idStudent = 1; idStudent <= countStudent; idStudent++)
                {
                    TableRow row = new();
                    for(int idCell = 0; idCell< countCellRowTable1;idCell++)
                    {
                        defouldFillCell(row, "");
                    }
                    table.Append(row);
                }
                /*
                // Ячейка (0,1)
                row1.Append(new TableCell(new Paragraph(new Run(
                    new RunProperties(new Border { Val = BorderValues.BasicThinLines }),
                    new Text("A211111111")
                    ))));

                table.Append(row1);*/

                body.Append(table);
                wordDoc.MainDocumentPart.Document.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при создании таблицы: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    private void ClearDocument(WordprocessingDocument wordDoc) {
        try
        {
            var mainPart = wordDoc.MainDocumentPart;

            if (mainPart == null)
            {
                // Если файла ещё нет, создаём новую структуру
                mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
            }

            // Просто создаём новый "Body", тем самым удаляя всё старое содержимое
            mainPart.Document.Body = new Body();

            mainPart.Document.Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при очистке: {e.Message}\n{e.StackTrace}");
        }
    }

    private void defouldFillCell(TableRow row, string text) {
        row.Append(new TableCell(new Paragraph(new Run(new Text(text)))));
    }
    
    private void Title1PreData(Table table) {

    }
    private void Table1PreData(Table table) {
        // -------- Первая строка --------
        TableRow row1 = new TableRow();

        // Ячейка (0,0) — с диагональю
        for (int id = 0; id < 36; id++)
        {
            if (id == 1)
            {
                TableCell diagCell = new TableCell();
                diagCell.Append(new TableCellProperties(
                    new TableCellBorders(
                        new TopLeftToBottomRightCellBorder { Val = BorderValues.Single, Size = 4 }
                    )
                ));

                // Содержимое: два параграфа, имитирующие текст "по диагонали"
                diagCell.Append(
                    new Paragraph(
                        new Run(new Text(NameCells[id])) { RunProperties = new RunProperties() }
                    )
                    {
                        ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Left })
                    }
                );

                diagCell.Append(
                    new Paragraph(
                        new Run(new Text(NameCells[id++])) { RunProperties = new RunProperties() }
                    )
                    {
                        ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Right })
                    }
                );
                row1.Append(diagCell);
            }
            else if (id == 34)
            {
                TableCell rotatedCell = new TableCell(
            new Paragraph(new Run(new Text(NameCells[id])))
        );
                rotatedCell.Append(new TableCellProperties(
                    new TextDirection { Val = TextDirectionValues.BottomToTopLeftToRight }
                ));
                row1.Append(rotatedCell);
            }
            else if (id == 35)
            {
                TableCell cell5 = new TableCell(
    new Paragraph(new Run(new Text("Верх вложенной ячейки")))
);
                cell5.Append(new TableCellProperties(
                    new GridSpan { Val = 2 }
                ));

                row1.Append(cell5);
            }
            else
            {
                defouldFillCell(row1, NameCells[id]);
            }
        }
        table.Append(row1);

        // ------------- Вторая строка: две повернутые ячейки -------------
        TableRow row2 = new TableRow();

        // Пустые ячейки (1–4), чтобы выровнять
        for (int i = 0; i < 34; i++)
        {
            row2.Append(new TableCell(new Paragraph()));
        }

        // Ячейка 5 с повёрнутым текстом
        TableCell rotatedCell1 = new TableCell(
            new Paragraph(new Run(new Text("Вертикал 1")))
        );
        rotatedCell1.Append(new TableCellProperties(
            new TextDirection { Val = TextDirectionValues.BottomToTopLeftToRight } // Поворот на 90°
        ));
        row2.Append(rotatedCell1);

        // Ячейка 6 с повёрнутым текстом
        TableCell rotatedCell2 = new TableCell(
            new Paragraph(new Run(new Text("Вертикал 2")))
        );
        rotatedCell2.Append(new TableCellProperties(
            new TextDirection { Val = TextDirectionValues.BottomToTopLeftToRight }
        ));
        row2.Append(rotatedCell2);

        table.Append(row2);
    }
}
