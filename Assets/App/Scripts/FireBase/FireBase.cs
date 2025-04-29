using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using DocumentFormat.OpenXml.Wordprocessing;

public class FireBase
{
    public static FireBaseData fireBaseData = new();
    public static event EventHandler ParsingFireBaseEnd;

    private DatabaseReference reference;
    private FirebaseAuth AutorizationPlayer;

    private DateTime TimeAdd3Day = DateTime.Now + new TimeSpan(3,0,0,0);

    private DateTime defouldStartParsing = DateTime.Today - new TimeSpan(DateTime.Today.Day-1, 0, 0, 0);
    private DateTime defouldEndParsing = DateTime.Today - new TimeSpan(1, 0, 0, 0); 

    private string facultyName;

    public async void Initialized() {

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        AutorizationPlayer = FirebaseAuth.DefaultInstance;

        object connectingTest = await ReadValue(reference.Child("Connection").Child("Test"));
        if (connectingTest is bool != true) Debug.Log("Fail Connection");
        else Debug.Log("successful connection, Initialization End");

    }
    public async Task<bool> LoadingData(string groupName) {
        try
        {
            Debug.Log("Start Pars FB");
            bool Admin = await Administration();
            bool Data = await LoadingForData(groupName);
            return Admin && Data;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public async void LoadingAllData() {
        Debug.Log("Start Loading All Data");
        fireBaseData.Faculties = new();
        bool FBAllData = await LoadingForData(null);
        Debug.Log("FBAllData: " + FBAllData);
        ParsingFireBaseEnd.Invoke(this, EventArgs.Empty);
    }

    public async Task<bool> Administration() {
        DatabaseReference AdministrationReference = reference.Child(FireBaseData.Key_ADMINISTRATION).Child(StartLoadingData.UUID);

        List<string> NameGroup = new();
        List<object> AdministrationGroupEnum = (await ReadValueEnum(AdministrationReference));

        if (AdministrationGroupEnum.Count > 0)
        {
            fireBaseData.IsAdministration = true;

            AdministrationGroupEnum.ForEach(obj => NameGroup.Add(obj.ToString()));
            fireBaseData.NameGroupAdministration = NameGroup;
        }
        else
        {
            fireBaseData.IsAdministration = false;
        }
        return true;

    }

    private async Task<List<DatabaseReference>> ParsClassData<T>(DatabaseReference reference, Dictionary<string, T> fireBaseDataDictionary/*, string[] GroupNameDat, int idgr*/, bool addConditions = false, int addConditionsReference = 0) where T : IFireBaseData, new() {
        try
        {
            T t = new();

            List<DatabaseReference> referenceList = await GetChiledsAsync(reference, t.Key);
            List<DatabaseReference> outReferenceList = new();

            if (referenceList.Count > 0)
            {
                foreach (DatabaseReference childReference in referenceList)
                {
                    if (/*childReference.Key != GroupNameDat[idgr] && GroupNameDat[idgr] != null ||*/
                        addConditions && int.Parse(childReference.Key) < addConditionsReference) { continue; }
                    
                    outReferenceList.Add(childReference);
                    t = new();
                    t.SetName(childReference.Key);
                    fireBaseDataDictionary[childReference.Key] = t;
                        
                }
                return outReferenceList;
            }
            else
            {
                Debug.LogWarning($"Data not foundit: Loading{t.GetType()}");
                return new();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            fireBaseDataDictionary.Clear();
            return new();
        }
    }
    private async Task FillGroup(string baseGroupLoad, DatabaseReference groupReference, Dictionary<string, Group> groups, DateTime? startDateTime, DateTime? endDateTime) {

        string groupNumber = RedactSearchText.ConverterGroupNameData(baseGroupLoad)[2];
        Group group = new() { Name = groupNumber };

        GroupParsing groupParsing = new GroupParsing() { Name = baseGroupLoad };

        List<DatabaseReference> dateReferenceList = GetDatesReference(groupReference, startDateTime, endDateTime);
        if (dateReferenceList.Count > 0)
        {
            Dictionary<string, Dates> dates = new();

            foreach (DatabaseReference dateReference in dateReferenceList)
            {
                Dates date = new Dates();
                DateParse dateParse = new() { dateTime = dateReference.Key };

                List<DatabaseReference> lessonReferenceList = await GetChiledsAsync(dateReference, LessonFireBase.Key);

                if (lessonReferenceList.Count > 0)
                {
                    List<LessonFireBase> lessons = new List<LessonFireBase>();

                    foreach (DatabaseReference lessonReference in lessonReferenceList)
                    {
                        LessonFireBase lesson = new LessonFireBase()
                        {
                            Name = (await ReadValue(lessonReference.Child(LessonFireBase.Key_NAME))).ToString(),
                            type = LessonFireBase.isTypeLesson((await ReadValue(lessonReference.Child(LessonFireBase.Key_TYPE))).ToString())
                        };


                        List<DatabaseReference> studentMissingReferenceList = await GetChiledsAsync(lessonReference, StudentMissing.Key);

                        if (studentMissingReferenceList.Count > 0)
                        {
                            List<StudentMissing> studentsMissing = new List<StudentMissing>();

                            foreach (DatabaseReference studentMissingReference in studentMissingReferenceList)
                            {
                                StudentMissing studentMissing = new StudentMissing()
                                {
                                    Type = Convert.ToBoolean(await ReadValue(studentMissingReference.Child(StudentMissing.Key_TYPE))),
                                    ID = Convert.ToInt32(await ReadValue(studentMissingReference.Child(StudentMissing.Key_ID)))
                                };

                                studentsMissing.Add(studentMissing);
                            }

                            lesson.StudentsMissing = studentsMissing;
                            lessons.Add(lesson);
                        }
                        else Debug.LogError("Data not foundit: LoadingForName, studentsMissingReference");

                        lessons.Add(lesson);
                        dateParse.Lessons.Add(lesson.Name);
                    }
                    date.lessons = lessons;
                    dates[dateReference.Key] = date;
                    groupParsing.dateParses.Add(dateParse);
                }
                //else Debug.Log("Data not foundit: LoadingForName, lessonReference");

            }

            group.Dates = dates;
        }
        else Debug.LogError("Data not foundit: LoadingForName, lessonReference");


        List<DatabaseReference> studentReferenceList = await GetChiledsAsync(groupReference, Student.Key);

        if (studentReferenceList.Count > 0)
        {
            List<Student> students = new();

            foreach (DatabaseReference studentReference in studentReferenceList)
            {
                Student student = new Student()
                {
                    Name = (await ReadValue(studentReference.Child(Student.Key_NAME))).ToString(),
                    ID = Convert.ToInt32(await ReadValue(studentReference.Child(Student.Key_ID)))
                };

                students.Add(student);
            }

            group.Students = students;
        }
        else Debug.LogWarning("Data not foundit: LoadingForName, studentReference");


        groups[group.Name] = group;
        Parsing.ParsingData1[baseGroupLoad] = groupParsing;
    }
    public async Task<bool> LoadingForData(string baseGroupLoad, DateTime? startDateTime = null, DateTime? endDateTime = null) {
        if(startDateTime == null || endDateTime == null)
        {
            startDateTime = defouldStartParsing;
            endDateTime = defouldEndParsing;
        }
        string[] GroupNameDat = RedactSearchText.ConverterGroupNameData(baseGroupLoad).ToArray(); //0-��� 1-������������� 2-������
        
            if (GroupNameDat.Length != 3) return false;

            List<DatabaseReference> facultyReferenceList = await GetChiledsAsync(reference, Faculty.key);

        if (facultyReferenceList.Count > 0)
        {
            foreach (DatabaseReference facultyReference in facultyReferenceList)
            {
                Faculty faculty = new Faculty()
                {
                    Name = facultyReference.Key
                };
                if (baseGroupLoad == null)
                {
                    Dictionary<string, Specialization> specializations = new();
                    List<DatabaseReference> specializationReferenceList = await ParsClassData(facultyReference, specializations/*, GroupNameDat, 1*/); // 1 - �������������

                    foreach (DatabaseReference specializationReference in specializationReferenceList)
                    {
                        Dictionary<string, Year> years = new();
                        List<DatabaseReference> yearReferenceList = await ParsClassData(specializationReference, years/*, GroupNameDat, 0*/, true, 25 - 3); // 0 - ���  25 - ������� ���

                        specializations.TryGetValue(specializationReference.Key, out Specialization specialization);

                        foreach (DatabaseReference yearReference in yearReferenceList)
                        {

                            Dictionary<string, Group> groups = new();
                            List<DatabaseReference> groupReferenceList = await ParsClassData(yearReference, groups/*, GroupNameDat, 2*/); // 2 - ������

                            years.TryGetValue(yearReference.Key, out Year year);

                            foreach (DatabaseReference groupReference in groupReferenceList)
                            {
                                await FillGroup(baseGroupLoad,groupReference,groups, startDateTime,endDateTime);
                            }

                            year.Groups = groups;
                            years[year.Name] = year;

                        }

                        specialization.Years = years;
                        specializations[specialization.Name] = specialization;

                    }

                    faculty.Specializations = specializations;
                }
                fireBaseData.Faculties.Add(faculty);
            }
        }
        else Debug.LogError("Data not foundit: LoadingForName, facultiesReferenceList");

        if (baseGroupLoad != null)
        {
            DatabaseReference groupReference = await SearchGroupReference(baseGroupLoad);
            if (groupReference != null)
            {
                int idFaculty = fireBaseData.Faculties.FindIndex(obj=>obj.Name == facultyName);
                Faculty faculty = fireBaseData.Faculties[idFaculty];
                faculty.Specializations.TryGetValue(GroupNameDat[1], out Specialization specialization);
                if (specialization == null)
                {
                    specialization = new Specialization() { Name = GroupNameDat[1] };
                    faculty.Specializations[GroupNameDat[1]] = specialization;
                }
                specialization.Years.TryGetValue(GroupNameDat[0], out Year year);
                if (year == null) {
                    year = new Year() { Name = GroupNameDat[0] };
                    specialization.Years[GroupNameDat[0]] = year;
                }
                await FillGroup(baseGroupLoad,groupReference, year.Groups, startDateTime, endDateTime);

            }
            else
            {
                return false;
            }
        }
        return true;
    }

    #region WorkData
    private async Task<object> ReadValue(DatabaseReference reference_Data) {
        try
        {
            object totalChildren = null;

            await reference_Data.GetValueAsync().ContinueWith(task =>
            {
                //DataSnapshot result = task?.Result;
                totalChildren = task?.Result.Value;
                //Do more stuff
            });

            return totalChildren;
        }
        catch
        {
            return null;
        }
    }
    private async Task<List<DatabaseReference>> ReadReferenceEnum(DatabaseReference reference_Data) {
        try
        {
            List<DataSnapshot> totalChildren = new();
            List<DatabaseReference> totalChildrenRef = new();

            await reference_Data.GetValueAsync().ContinueWith(task =>
            {
                totalChildren = task.Result.Children.ToList();
                totalChildren.ForEach(obj => totalChildrenRef.Add(obj.Reference));
                //Do more stuff
            });

            return totalChildrenRef;
        }
        catch
        {
            return null;
        }
    }
    private async Task<List<object>> ReadValueEnum(DatabaseReference reference_Data) {
        try
        {
            List<DataSnapshot> totalChildren = new();
            List<object> totalChildrenValue = new();

            await reference_Data.GetValueAsync().ContinueWith(task =>
            {
                totalChildren = task.Result.Children.ToList();
                totalChildren.ForEach(obj => totalChildrenValue.Add(obj.Value));
                //Do more stuff
            });

            return totalChildrenValue;
        }
        catch
        {
            return null;
        }
    }
    public async Task<int> ChildCount(DatabaseReference reference_Data) {
        try
        {
            int totalChildren = 0;

            await reference_Data.GetValueAsync().ContinueWith(task =>
            {
                totalChildren = (int)task.Result.ChildrenCount;
                //Do more stuff
            });

            return totalChildren;
        }
        catch
        {
            return 0;
        }
    }
    private List<DatabaseReference> GetDatesReference(DatabaseReference reference, DateTime? startDateTime, DateTime? endDateTime) {
        try
        {
            DatabaseReference dateReference = reference.Child(Dates.Key);

            List<DatabaseReference> dates = new();
            if (endDateTime - startDateTime < new TimeSpan(93, 0, 0, 0))
            {
                for (DateTime? dateTime = startDateTime; dateTime <= endDateTime; dateTime += new TimeSpan(1, 0, 0, 0))
                {
                    dates.Add(dateReference.Child(dateTime?.Day + "-" + dateTime?.Month + "-" + dateTime?.Year));
                }
            }
            return dates;

        }
        catch
        {
            return null;
        }
    }
    private async Task<List<DatabaseReference>> GetChiledsAsync(DatabaseReference reference, string key = null) {
        try
        {
            if(key!= null)
                reference = reference.Child(key);
            return await ReadReferenceEnum(reference);
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Create

    public async Task<bool> CreateGroup(string name, string idFaculty = null, string idSpecialization = null, string idYear = null, DatabaseReference groupReference = null) {
        try
        {
            if(idFaculty == null)
            {
                string[] groupData = RedactSearchText.ConverterGroupNameData(name);
                if(groupData[2] != null)
                {
                    idSpecialization = groupData[1];
                    idYear = groupData[0];
                    name = groupData[2];
                    idFaculty = await FindFaculty(idSpecialization);
                }
                else {
                    return false;
                }
            }
            DatabaseReference reference_Group;
            if (groupReference != null) reference_Group = groupReference;
            else
            {
                reference_Group = reference.Child(Faculty.key).Child(idFaculty)
                    .Child(Specialization.key).Child(idSpecialization)
                    .Child(Year.key).Child(idYear)
                    .Child(Group.key);
            }
            int countChiled = await ChildCount(reference_Group);
            DatabaseReference newFaculty = reference_Group.Child(name);
            await UpdateData(newFaculty, Group.Key_NAME, name);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }
    public async Task<bool> CreateLesson(List<string> names, List<LessonFireBase.TypeLesson> types, string date, DatabaseReference referenceGroup) {
        try
        {
            if (DateTime.Parse(date) >= TimeAdd3Day) { return true; }
            DatabaseReference reference_Lesson = referenceGroup
                .Child(Dates.Key).Child(date)
                .Child(LessonFireBase.Key);

            List<DatabaseReference> AllLesson = await GetChiledsAsync(reference_Lesson);
            for (int i = 0;i <= AllLesson.Count; i++)
            {
                if (i == AllLesson.Count)
                {
                    for (; i < names.Count; i++)
                    {
                        DatabaseReference newLesson = reference_Lesson.Child(LessonFireBase.Key + i);

                        await UpdateData(newLesson, LessonFireBase.Key_NAME, names[i]);
                        await UpdateData(newLesson, LessonFireBase.Key_TYPE, LessonFireBase.ConvertToString(types[i]));
                    }
                    break;
                }

                DatabaseReference lesson = AllLesson[i];

                if (i == names.Count)
                {
                    for (; i < AllLesson.Count; i++)
                    {
                        await DeliteLesson(lesson);
                    }
                    break;
                }

                bool isNewLesson = (await ReadValue(lesson.Child(LessonFireBase.Key_NAME))).ToString() != names[i] || (await ReadValue(lesson.Child(LessonFireBase.Key_TYPE))).ToString() != types[i].ToString();


                if (false && isNewLesson)
                {
                    await DeliteLesson(lesson);

                    DatabaseReference newLesson = reference_Lesson.Child(LessonFireBase.Key + i);
                    await UpdateData(newLesson, LessonFireBase.Key_NAME, names[i]);
                    await UpdateData(newLesson, LessonFireBase.Key_TYPE, LessonFireBase.ConvertToString(types[i]));
                }
                
                
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }
    public async Task<bool> CreateStudentMissing(string name, LessonFireBase.TypeLesson type, string date, string idFaculty, int idSpecialization, int idYear, int idGroup, int idLesson) {
        try
        {

            DatabaseReference reference_StudentMissing = reference.Child(Faculty.key).Child(idFaculty)
                .Child(Specialization.key).Child(Specialization.key + idSpecialization)
                .Child(Year.key).Child(Year.key + idYear)
                .Child(Group.key).Child(Group.key + idGroup)
                .Child(LessonFireBase.Key).Child(LessonFireBase.Key + idLesson)
                .Child(StudentMissing.Key);
            int countChiled = await ChildCount(reference_StudentMissing);
            DatabaseReference newFaculty = reference_StudentMissing.Child(StudentMissing.Key + countChiled);
            await UpdateData(newFaculty, StudentMissing.Key_ID, name);
            await UpdateData(newFaculty, StudentMissing.Key_TYPE, false);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }
    public async Task<bool> CreateStudent(string name, string idFaculty, int idSpecialization, int idYear, int idGroup) {
        try
        {
            DatabaseReference group = await SearchGroupReference(name);
            DatabaseReference reference_Student = reference.Child(Faculty.key).Child(idFaculty)
                .Child(Specialization.key).Child(Specialization.key + idSpecialization)
                .Child(Year.key).Child(Year.key + idYear)
                .Child(Group.key).Child(Group.key + idGroup)
                .Child(Student.Key);
            int countChiled = await ChildCount(reference_Student);
            DatabaseReference newFaculty = reference_Student.Child(Student.Key + countChiled);
            await UpdateData(newFaculty, Student.Key_NAME, name);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }
    public async Task UpdateData(DatabaseReference reference, string key, object data) {
        await reference.Child(key).SetValueAsync(data);
    }
    #endregion
    public async Task<bool> isUpdate() {
        string DateTimeNow = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour;
        DatabaseReference updateReference = reference.Child(FireBaseData.Key_CONNECTION).Child(FireBaseData.Key_UPDATEALL);
        if(Convert.ToBoolean(
            await ReadValue(updateReference.Child(DateTimeNow))
            ))
        {
            Debug.Log("isUpdate false");
            return false;
        }
        else
        {
            Debug.Log("isUpdate true");
            await UpdateData(updateReference, DateTimeNow, true);
            return true;
        }
    }
    private async Task DeliteLesson(DatabaseReference lessonReference) {
        try
        {
            List<DatabaseReference> childReferenceGroup = await GetChiledsAsync(lessonReference);
            foreach (var item in childReferenceGroup)
            {
                if(item.Key == StudentMissing.Key)
                {
                    List<DatabaseReference> childReferenceStudentMissing = await GetChiledsAsync(item, StudentMissing.Key);
                    foreach (var itemStudent in childReferenceGroup)
                    {
                        await itemStudent.RemoveValueAsync();
                    }
                }
                else
                    await item.RemoveValueAsync();
            }

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
    private async Task DeliteGroup(string name, DatabaseReference groupReference = null) {

        try
        {
            DatabaseReference group;
            if (groupReference != null) group = groupReference;
            else group = await SearchGroupReference(name);

            Debug.Log(name + " DeliteGroup");
            List<DatabaseReference> childReferenceGroup = await GetChiledsAsync(group.Parent, group.Key);
            foreach (var item in childReferenceGroup)
            {
                await item.RemoveValueAsync();
            }

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }
    private async Task<string> FindFaculty(string name) {
        string FacultyNameDefould = "New";

        string[] groupData = RedactSearchText.ConverterGroupNameData(name);

        List<DatabaseReference> facultyReferenceList = await GetChiledsAsync(reference, Faculty.key);

        foreach (var facultyReference in facultyReferenceList)
        {
            DatabaseReference specializationReference = facultyReference.Child(Specialization.key).Child(name);

            if((await ChildCount(specializationReference))>0)
                    return facultyReference.Key;
        }

        return FacultyNameDefould;

    }
    public async Task<DatabaseReference> SearchGroupReference(string name) {
        string[] groupData = RedactSearchText.ConverterGroupNameData(name);

        List<DatabaseReference> facultyReferenceList = await GetChiledsAsync(reference, Faculty.key);

        foreach (var facultyReference in facultyReferenceList)
        {
            DatabaseReference specializationReference = facultyReference.Child(Specialization.key).Child(groupData[1]);

            List<DatabaseReference> GroupReferenceList = await GetChiledsAsync(specializationReference.Child(Year.key).Child(groupData[0]), Group.key);
            foreach (var GroupReference in GroupReferenceList)
            {
                if ((await ReadValue(GroupReference.Child(Group.Key_NAME))).ToString() == groupData[2])
                {
                    facultyName = facultyReference.Key;
                    return GroupReference;
                }
            }
        }

        return null;
    }
}
