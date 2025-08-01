using UnityEngine;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Reflection;
using Firebase.Database;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;

public class Parsing : MonoBehaviour
{
    public static Parsing Instance;
    private FireBase _fireBase = new();
    public static event EventHandler PageEvent;
    public static Dictionary<string, GroupParsing> ParsingData1 = new();
    private GroupParsing ParsingDataDefould { get; set; }


    private const int FAIL_LIMIT = 5;
    private const string POLESSU_URL_TYPE_2 = "https://www.polessu.by/ruz/ng/?";
    private const string POLESSU_FILE2 = ".//div[@class='container']";
    private const string GroupNamePlayerPrefs = "GROUP_NAME";
    public static string ParsingGroupName = "22ИТ-3";


    private void Awake() {
        Instance = this;
        if (PlayerPrefs.HasKey(GroupNamePlayerPrefs))
            ParsingGroupName = PlayerPrefs.GetString(GroupNamePlayerPrefs);
        else
            PlayerPrefs.SetString(GroupNamePlayerPrefs, ParsingGroupName);
        _fireBase.Initialized();
    }
    private async void Start() {
        await LoadingDefouldData(ParsingGroupName, true);
    }
    public async Task LoadingDefouldData(string parsGroupName, bool isUpdate = false,DateTime? dateStart = null, DateTime? dateEnd = null) {
        int countParsingFail = 0;
        do
        {
            bool FB = await TimeTrigger(_fireBase.LoadingData(parsGroupName, dateStart, dateEnd), 10);

            ParsingDataDefould = await TimeTrigger(ParsingMetod(parsGroupName, isUpdate, dateEnd, !FB));
            if (ParsingDataDefould != null || FB)
            {
                //await fireBase.CreateGroup(ParsingDataDefould.Name);
                PlayerPrefs.SetString(GroupNamePlayerPrefs, parsGroupName);
                ParsingGroupName = parsGroupName;
                ParsingData1[parsGroupName].MergingObjectDate(ParsingDataDefould?.DateParses);
                Debug.Log("EndDefouldParsing");
                PageEvent.Invoke(this, EventArgs.Empty);

                return;
            }

            countParsingFail++;
        } while (countParsingFail < FAIL_LIMIT);

        // ������� ����� �������� �������� ������������ � fireBase
        Debug.LogError("Error Loading Data");
        ProgressBar.ErrorProgress("Error Loading Data");

    }
    private async Task<GroupParsing> ParsingMetod(string GroupName, bool updateData, DateTime? endDateTime = null, bool testingGroup = false)
    {
        ProgressBar.Progress = 0f;
        endDateTime ??= Times.PDefouldEndParsing;

        GroupParsing groupParsing = new();
        groupParsing.Name = GroupName;

        string GroupAndDate = "group=" + GroupName + "&day=";

        try
        {
            using (HttpClientHandler hdl = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None })
            {
                using (var client = new HttpClient(hdl))
                {
                    using (HttpResponseMessage resp = await client.GetAsync(POLESSU_URL_TYPE_2 + GroupAndDate + (endDateTime.Value.Year + "-" + endDateTime.Value.Month + "-" + endDateTime.Value.Day)))
                    {
                        if (resp.IsSuccessStatusCode)
                        {

                            var html = resp.Content.ReadAsStringAsync().Result;

                            if (!string.IsNullOrEmpty(html))
                            {
                                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                                document.LoadHtml(html);

                                var tables = document.DocumentNode.SelectSingleNode(POLESSU_FILE2);
                                try
                                {
                                    ProgressBar.Progress = 0.32f;
                                    (await WorkingTabelType2(tables, groupParsing.Name, updateData, endDateTime, testingGroup)).ForEach(groupParsing.DateParses.Add);
                                    ProgressBar.Progress = 1f;
                                }
                                catch
                                {
                                    Debug.LogWarning("Data ParsingMetod2 WorkingTabelsType2 not foundit");
                                    ProgressBar.ErrorProgress("Рассписание не загружено");
                                    return null;
                                }

                            }
                        }
                    }
                }
            }

            return groupParsing;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        return null;
    }

    private List<string> ConvertToCollectionString(IList<HtmlNode> htmlNode, List<int> DontConvert = null ) {
        List<string> HTMLInnerText = new();
        for(int i = 0; i < htmlNode.Count; i++)
        {
            if(DontConvert==null|| DontConvert.Any(obj => obj != i))
            {
                HTMLInnerText.Add(htmlNode[i].InnerText);
            }
        }
        return HTMLInnerText;
    }
    private async Task<List<DateParse>> WorkingTabelType2(HtmlNode table, string groupName, bool updateData, DateTime? endDateTime, bool testingGroup = false) {
        List<DateParse> dateParses = new();
        ListAndInt<DateParse> dateParsesNotLoading = new(){DatList = new List<DateParse>(), IntList = new List<int>()};
        DatabaseReference groupReference = !updateData? null:await _fireBase.SearchGroupReference(groupName);

        if (table != null)
        {

            HtmlNodeCollection dateNodesName = table.SelectNodes(".//small[@class='text-muted']");
            if (dateNodesName is { Count: > 1 })
            {
                if (testingGroup) await _fireBase.CreateGroup(groupName);
                List<string> DateNodesName = ConvertToCollectionString(dateNodesName, new() {0}); // ��������� ���� �� ������� ����� ����������
                int NodesAdd = int.MaxValue;
                for (int i = 0;i< DateNodesName.Count;i++)
                {
                    DateTime timeLesson = ConvertStringToDateTime(DateNodesName[i]);
                    string dateString = timeLesson.Day + "-" + timeLesson.Month + "-" + timeLesson.Year;
                    if (timeLesson < Times.Today && await _fireBase.IsCreatingDate(dateString, groupReference))
                        dateParsesNotLoading.Add(new DateParse { dateTime = dateString }, i);
                    if (timeLesson >= Times.Today && timeLesson <= endDateTime)
                    {
                        dateParses.Add(new DateParse { dateTime = dateString });
                        if (NodesAdd == int.MaxValue)
                            NodesAdd = i;
                    }
                }
                ProgressBar.Progress = 0.4f;
                HtmlNodeCollection DateNodes = table.SelectNodes(".//div[@class='row acty-group']"); // ��� ������ tbody
                if (DateNodes is { Count: > 0 })
                {
                    for (int DateID = 0, DateNotFound = 0; (DateID + NodesAdd < DateNodes.Count && DateID < dateParses.Count) || DateNotFound < dateParsesNotLoading.Count; DateID++, DateNotFound++)
                    {
                        ProgressBar.Progress += 0.6f / (dateParses.Count + dateParsesNotLoading.Count);
                        bool isNotFound = DateNotFound < dateParsesNotLoading.Count;
                        int idDate = isNotFound ? dateParsesNotLoading.IntList[DateNotFound] : DateID + NodesAdd;
                        if (isNotFound) DateID--;
                        else DateNotFound--;
                        IList<DateParse> dateParsesTime = isNotFound ? dateParsesNotLoading.DatList : dateParses;
                        DateFill(DateNodes[idDate],out List<HtmlNode> LessonNodes,out List<HtmlNode> LessonTypeNodes);
                        idDate -= isNotFound ? 0 : NodesAdd;
                        if (LessonNodes != null && LessonNodes.Count > 0)
                        {
                            List<string> LessonsName = ConvertToCollectionString(LessonNodes);
                            if (updateData)
                            {
                                List<LessonFireBase.TypeLesson> LessonsType = new();
                                ConvertToCollectionString(LessonTypeNodes).ForEach(obj => LessonsType.Add(LessonFireBase.isTypeLesson(obj)));

                                if (LessonsType.Count == LessonsName.Count) {
                                    await _fireBase.CreateLesson(LessonsName, LessonsType, dateParsesTime[idDate].dateTime, groupReference);
                                }
                            }
                            LessonsName.ForEach(obj => dateParsesTime[idDate].Lessons.Add(obj));
                        }
                        else
                        {
                            Debug.Log($"������� �� {idDate} ����");
                        }
                    }
                }
                else
                {
                    Debug.LogError("������ � ������� � ����������� �� ����������");
                }
            }
            else
            {
                Debug.LogWarning("small[@class='text-muted ��� �� ����������");
                return null;
            }

        }
        else
        {
            Debug.LogWarning("��� ����������");
            return null;
        }

        return dateParses;
    }

    private class ListAndInt<T>
    {
        public IList<T> DatList;
        public IList<int> IntList;
        public int Count = 0;

        public void Add(T dat, int id)
        {
            Count++;
            DatList.Add(dat);
            IntList.Add(id);
        }
    }

    private static DateTime ConvertStringToDateTime(string data) {

        DateTime.TryParseExact(data, "d.M", null, System.Globalization.DateTimeStyles.None, out DateTime time);
        return time;
    }

    private void DateFill(HtmlNode dateNode, out List<HtmlNode> lessonNodes, out List<HtmlNode> lessonsTypeNodes)
    {
        HtmlNodeCollection LessonsNodes = dateNode.SelectNodes(".//div[@class='acty-item clearfix']");
        lessonNodes = new();
        lessonsTypeNodes = new();
        foreach (var LessonsNode in LessonsNodes)
        {
            lessonNodes.Add(LessonsNode.SelectSingleNode(".//span[@class='acty-subjects']"));

            if (LessonsNode.SelectSingleNode(".//span[@title]") == null)
                lessonsTypeNodes.Add(LessonsNode.SelectSingleNode(".//span[@class='acty-subjects']"));
            else
                lessonsTypeNodes.Add(LessonsNode.SelectSingleNode(".//span[@title]"));
        }


    }

    private async Task<T> TimeTrigger<T>(Task<T> task, float timeOut = 10f) {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        Coroutine corotine = StartCoroutine(ResourceTickOver(cancelTokenSource, typeof(T).ToString(), timeOut));;
        T result = default(T);

        try
        {
            //научиться отключать Task<T> одекватно а не то как сейчас
            result = await Task.Run(() => task, cancelTokenSource.Token);
        }
        catch
        {
            Debug.Log("Error Connecting");
        }
        StopCoroutine(corotine);
        return result;
    }
    IEnumerator ResourceTickOver(CancellationTokenSource token, string taskType = "", float waitTime = 10) {
        yield return new WaitForSeconds(waitTime);
        if (token.IsCancellationRequested !=true)
        {
            token.Dispose();
        }
        else
        {
            Debug.Log("Seccfulied Connecting " + taskType);
        }
    }
}
    

