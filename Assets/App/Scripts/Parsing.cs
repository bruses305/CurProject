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

public class Parsing : MonoBehaviour
{
    public FireBase fireBase = new();
    public event EventHandler PageEvent;
    public static Dictionary<string, GroupParsing> ParsingData1 = new();
    private GroupParsing ParsingDataDefould { get; set; }


    private const int FAIL_LIMIT = 5;
    private const string POLESSU_URL_TYPE_2 = "https://www.polessu.by/ruz/ng/?";
    private const string POLESSU_FILE2 = ".//div[@class='container']";
    private const string GroupNamePlayerPrefs = "GROUP_NAME";
    public static string parsingGroupName = "22ИТ-3";
    private DateTime dateTime_Now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


    private async void Awake() {
        if (PlayerPrefs.HasKey(GroupNamePlayerPrefs))
            parsingGroupName = PlayerPrefs.GetString(GroupNamePlayerPrefs);
        else
            PlayerPrefs.SetString(GroupNamePlayerPrefs, parsingGroupName);
        fireBase.Initialized();

        await LoadingDefouldData(parsingGroupName, true);
    }
    private void Start() {
        fireBase.ParsingFireBaseEnd += ParsingWebSite;
    }

    private void ParsingWebSite(object sender, EventArgs e) {
        Debug.Log("All Parsing Web Site");
        AllParsing();
    }
    private async Task<T> TimeTrigger<T>(Task<T> task, float timeOut = 10f) {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        var fieldInfo = typeof(Task<T>).GetField("m_action", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = fieldInfo.GetValue(task);


        Coroutine corotine = StartCoroutine(ResourceTickOver(cancelTokenSource, task.ToString(), timeOut));
        T Data = default(T);

        try
        {
            Data = await Task.Run(() => task, cancelTokenSource.Token);
        }
        catch
        {
            Debug.Log("Error Connecting");
        }
        
        StopCoroutine(corotine);
        Debug.Log(Data);
        return Data;
    }
    public async void ButtonEventLoadingAllData() {
        await Task.Run(fireBase.LoadingAllData);
    }
    private async Task LoadingDefouldData(string parsGroupName = null, bool isUpdate = false) {
        int countParsingFail = 0;
        do
        {
            Debug.Log("StartDefouldParsing");
            bool FB = await TimeTrigger(fireBase.LoadingData(parsGroupName));  //нужно реализовать чтобы данные о пройденных занятиях подключались к GroupParsing2 а потом и к GroupParssing1 при полном парсинге
            if (FB)
            {
                ParsingDataDefould = await TimeTrigger(ParsingMetod(parsGroupName, isUpdate));
                if (ParsingDataDefould != default(GroupParsing))
                {
                    ParsingData1[parsGroupName].MergingObjectDate(ParsingDataDefould.dateParses);
                    PageEvent.Invoke(this, EventArgs.Empty);  // вызов инвента на обновление таблицы
                    //ивент на закрытие загрузочного экрана
                    return;
                }
            }
            countParsingFail++;
        }
        while (countParsingFail < FAIL_LIMIT);

        // сделать чтобы пыталось повторно подключиться к fireBase
        Debug.LogError("Error Loading Data");

    }
    public void LastPage() {
        
    }
    public void NextPage() {
        
    }
    private async void AllParsing() {
        List<string> GroupName = new();
        foreach (var faculty in FireBase.fireBaseData.Faculties)
        {
            foreach (var specialithation in faculty.Specializations)
            {
                string name_specialithation = specialithation.Value.Name;
                foreach (var year in specialithation.Value.Years)
                {
                    string name_year = year.Value.Name;
                    foreach (var group in year.Value.Groups)
                    {
                        string name_group = group.Value.Name;
                        GroupName.Add(name_year + name_specialithation + "-" + name_group);
                    }
                }
            }
        }
        Debug.Log("GroupName" + GroupName.Count);
        bool isUpdate = await fireBase.isUpdate();
        foreach (string groupName in GroupName)
        {
            GroupParsing groupParsing = await ParsingMetod(groupName, isUpdate);
            if(groupParsing!=null) ParsingData1[groupName]= groupParsing;
        }

        Debug.Log("ParsingData1 CountGroup Parsing: " + ParsingData1.Count);
    }

    private async Task<GroupParsing> ParsingMetod(string GroupName, bool updateData) {
        GroupParsing groupParsing = new();
        groupParsing.Name = GroupName;

        string datePars_Now = dateTime_Now.Year + "-" + dateTime_Now.Month + "-" + dateTime_Now.Day;
        string GroupAndDate = "group=" + GroupName + "&day=" + datePars_Now;

        try
        {
            using (HttpClientHandler hdl = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None })
            {
                using (var client = new HttpClient(hdl))
                {
                    using (HttpResponseMessage resp = await client.GetAsync(POLESSU_URL_TYPE_2 + GroupAndDate))
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
                                    (await WorkingTabelsType2(tables, groupParsing.Name, updateData)).ForEach(groupParsing.dateParses.Add);
                                }
                                catch
                                {
                                    Debug.LogWarning("Data ParsingMetod2 WorkingTabelsType2 not foundit");
                                }

                            }
                        }
                    }
                }
            }

            if (groupParsing.dateParses.Count > 0) return groupParsing;
            return null;
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
            if(DontConvert==null||!DontConvert.All(obj => obj == i))
            {
                HTMLInnerText.Add(htmlNode[i].InnerText);
            }
        }
        return HTMLInnerText;
    }
    private async Task<List<DateParse>> WorkingTabelsType2(HtmlNode table, string groupName, bool updateData) {
        List<DateParse> dateParses = new();
        DatabaseReference referenceGroup = !updateData? null:await fireBase.SearchGroupReference(groupName);
        Debug.Log(groupName);
        if (table != null)
        {

            HtmlNodeCollection dateNodesName = table.SelectNodes(".//small[@class='text-muted']");
            if (dateNodesName != null && dateNodesName.Count > 1)
            {
                List<string> DateNodesName = ConvertToCollectionString(dateNodesName, new() {0}); // сохраняет даты на которые стоит расписание
                int NodesAdd = int.MaxValue;
                for (int i = 0;i< DateNodesName.Count;i++)
                {
                    DateTime timeLesson = ConvertStringToDateTime(DateNodesName[i]);
                    if (NodesAdd == int.MaxValue && (timeLesson.Day >= DateTime.Now.Day || timeLesson.Month > DateTime.Now.Month))
                        NodesAdd = i;
                    if (timeLesson.Day >= DateTime.Now.Day || timeLesson.Month > DateTime.Now.Month)
                        dateParses.Add(new DateParse { dateTime = (timeLesson.Day + "-" + timeLesson.Month + "-" + timeLesson.Year) });
                }

                HtmlNodeCollection DateNodes = table.SelectNodes(".//div[@class='row acty-group']"); // все строки tbody
                if (DateNodes != null && DateNodes.Count > 0)
                {
                    for (int DateID = 0; DateID + NodesAdd < DateNodes.Count; DateID++)
                    {
                        HtmlNodeCollection LessonsNodes = DateNodes[DateID + NodesAdd].SelectNodes(".//div[@class='acty-item clearfix']");
                        List<HtmlNode> LessonNodes = new();
                        List<HtmlNode> LessonsTypeNodes = new();
                        foreach (var LessonsNode in LessonsNodes)
                        {
                            LessonNodes.Add(LessonsNode.SelectSingleNode(".//span[@class='acty-subjects']"));

                            if(LessonsNode.SelectSingleNode(".//span[@title]") == null)
                                LessonsTypeNodes.Add(LessonsNode.SelectSingleNode(".//span[@class='acty-subjects']"));
                            else
                                LessonsTypeNodes.Add(LessonsNode.SelectSingleNode(".//span[@title]"));
                        }

                        if (LessonNodes != null && LessonNodes.Count > 0)
                        {
                            List<string> LessonsName = ConvertToCollectionString(LessonNodes);
                            if (updateData) {
                                List<LessonFireBase.TypeLesson> LessonsType = new();
                                ConvertToCollectionString(LessonsTypeNodes).ForEach(obj => LessonsType.Add(LessonFireBase.isTypeLesson(obj)));

                                if (LessonsType.Count == LessonsName.Count) {
                                    await fireBase.CreateLesson(LessonsName, LessonsType, dateParses[DateID].dateTime, referenceGroup);
                                }
                            }
                            LessonsName.ForEach(obj => dateParses[DateID].Lessons.Add(obj));
                        }
                        else
                        {
                            Debug.Log($"Занятий на {DateID} нету");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Таблиц с данными о рассписании не существует");
                }
            }
            else
            {
                Debug.LogWarning("small[@class='text-muted дат не существует");
            }

        }
        else
        {
            Debug.LogWarning("Нет результата");
        }

        return dateParses;
    }
    private DateTime ConvertStringToDateTime(string data) {

        DateTime.TryParseExact(data, "d.M", null, System.Globalization.DateTimeStyles.None, out DateTime time);
        return time;
    }
    IEnumerator ResourceTickOver(CancellationTokenSource token, string taskType = "", float waitTime = 10) {
        yield return new WaitForSeconds(waitTime);
        if (token.IsCancellationRequested !=true)
        {
            token.Dispose();
            Debug.Log("Error Connecting " + taskType);
        }
        else
        {
            Debug.Log("Seccfulied Connecting " + taskType);
        }
    }
}
    

