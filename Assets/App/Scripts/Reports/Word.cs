using UnityEngine;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine.Windows;
using Debug = UnityEngine.Debug;
using File = System.IO.File;

public class Word : MonoBehaviour
{
    public bool isopenToEnd = false;
    [Range(min:0,max:1)]public float progressBar;
    public int countStudent = 0;

    private string[] NameCells = {
        "№ п/п","         \r\n                       Дата\r\n\r\n\r","        ФИО\r\n", "форма\r\nобучения\r\n(бюджет/платно)\r\n", "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9",  "10","11",  "12",  "13",  "14",  "15",  "16",  "17",  "18",  "19",  "20","21",  "22",  "23",  "24",  "25",  "26",  "27",  "28",  "29",  "30","ИТОГО","из них"
    };
    private const int countCellRowTable1 = 36;
    public void unlock()
    {
        progressBar = 0;
        StartCoroutine(outTick());
        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open("Ved.docx", true))
        {
            try
            {
                ClearDocument(wordDoc);
                
                var body = wordDoc.MainDocumentPart.Document.Body;

                Table table = new Table();

                // ������� �������
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
                progressBar = .2f;
                StartCoroutine(outTick());
                Title1PreData(table);
                progressBar = .4f;
                StartCoroutine(outTick());
                Table1PreData(table);
                progressBar = .7f;
                StartCoroutine(outTick());
                for(int idStudent = 1; idStudent <= countStudent; idStudent++)
                {
                    TableRow row = new();
                    for(int idCell = 0; idCell< countCellRowTable1;idCell++)
                    {
                        defouldFillCell(row, "");
                    }
                    table.Append(row);
                }
                progressBar = 0.9f;
                StartCoroutine(outTick());
                /*
                // ������ (0,1)
                row1.Append(new TableCell(new Paragraph(new Run(
                    new RunProperties(new Border { Val = BorderValues.BasicThinLines }),
                    new Text("A211111111")
                    ))));

                table.Append(row1);*/

                body.Append(table);
                wordDoc.MainDocumentPart.Document.Save();
                progressBar = 1;
                StartCoroutine(outTick());
            }
            catch (Exception e)
            {
                Debug.LogError($"������ ��� �������� �������: {e.Message}\n{e.StackTrace}");
            }
        }

        if(isopenToEnd) Process.Start("Ved.docx");
    }

    private void ClearDocument(WordprocessingDocument wordDoc) {
        try
        {
            var mainPart = wordDoc.MainDocumentPart;

            if (mainPart == null)
            {
                // ���� ����� ��� ���, ������ ����� ���������
                mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
            }

            // ������ ������ ����� "Body", ��� ����� ������ �� ������ ����������
            mainPart.Document.Body = new Body();

            mainPart.Document.Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ��� �������: {e.Message}\n{e.StackTrace}");
        }
    }

    private void defouldFillCell(TableRow row, string text) {
        row.Append(new TableCell(new Paragraph(new Run(new Text(text)))));
    }
    
    private void Title1PreData(Table table) {

    }
    private void Table1PreData(Table table) {
        // -------- ������ ������ --------
        TableRow row1 = new TableRow();

        // ������ (0,0) � � ����������
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

                // ����������: ��� ���������, ����������� ����� "�� ���������"
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
    new Paragraph(new Run(new Text("���� ��������� ������")))
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

        // ------------- ������ ������: ��� ���������� ������ -------------
        TableRow row2 = new TableRow();

        // ������ ������ (1�4), ����� ���������
        for (int i = 0; i < 34; i++)
        {
            row2.Append(new TableCell(new Paragraph()));
        }

        // ������ 5 � ��������� �������
        TableCell rotatedCell1 = new TableCell(
            new Paragraph(new Run(new Text("�������� 1")))
        );
        rotatedCell1.Append(new TableCellProperties(
            new TextDirection { Val = TextDirectionValues.BottomToTopLeftToRight } // ������� �� 90�
        ));
        row2.Append(rotatedCell1);

        // ������ 6 � ��������� �������
        TableCell rotatedCell2 = new TableCell(
            new Paragraph(new Run(new Text("�������� 2")))
        );
        rotatedCell2.Append(new TableCellProperties(
            new TextDirection { Val = TextDirectionValues.BottomToTopLeftToRight }
        ));
        row2.Append(rotatedCell2);

        table.Append(row2);
    }

    IEnumerator outTick()
    {
        yield return new WaitForSeconds(0.001f);
    }
}
