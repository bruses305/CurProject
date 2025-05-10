using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

public class FireBase
{
    private static FireBase _instance;

    public static FireBaseData fireBaseData = new();

    private DatabaseReference reference;
    private FirebaseAuth _authPlayer;
    private static bool _isStartLoading = false;

    private string facultyName;
    public FireBase(){
        _instance = this;
    }

    public async void Initialized()
    {
        try
        {
            reference = FirebaseDatabase.DefaultInstance.RootReference;
            _authPlayer = FirebaseAuth.DefaultInstance;
            
            object connectingTest = await ReadValue(reference.Child("Connection").Child("Test"));
            Debug.Log(connectingTest is bool != true ? "Fail Connection" : "successful connection, Initialization End");
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
    public async Task<bool> LoadingData(string groupName, DateTime? startDateTime = null, DateTime? endDateTime = null) {
        try
        {
            Debug.Log("Start Pars FB");
            await LockUser();
            if(!_isStartLoading) await StartLoadingDataFb();
            
            bool data = await LoadingForData(groupName, startDateTime, endDateTime);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }
    private async Task LockUser()
    {
        DatabaseReference blockReference = reference.Child(FireBaseData.Key_BLOCK);
        object isBlock = await ReadValue(blockReference.Child(StartLoadingData.UUID));
        if (isBlock == null)
        {
            
            await UpdateData(blockReference,StartLoadingData.UUID, false);
            DatabaseReference deviceReference = reference.Child(FireBaseData.Key_DEVICES);
            await UpdateData(deviceReference.Child(StartLoadingData.UUID),"DeviceModel", SystemInfo.deviceModel);
            await UpdateData(deviceReference.Child(StartLoadingData.UUID),"deviceName", SystemInfo.deviceName);
            await UpdateData(deviceReference.Child(StartLoadingData.UUID),"deviceType", SystemInfo.deviceType.ToString());
            await UpdateData(deviceReference.Child(StartLoadingData.UUID),"systemMemorySize", SystemInfo.systemMemorySize);
            await UpdateData(deviceReference.Child(StartLoadingData.UUID),"processorModel", SystemInfo.processorModel);
            
        }
        if (Convert.ToBoolean(isBlock))
        {
            Notification.SendNotificationMessage("ВЫ ЗАБАНЕНЫ †††", Color.red,10f);
            await Task.Delay(500);
            ExitApp.Exit();
        }
    }

    private async Task<List<DatabaseReference>> ParsClassData<T>(DatabaseReference reference, Dictionary<string, T> fireBaseDataDictionary, bool addConditions = false, int addConditionsReference = 0) where T : IFireBaseData, new() {
        try
        {
            T t = new();

            List<DatabaseReference> referenceList = await GetChildAsync(reference, t.Key);
            List<DatabaseReference> outReferenceList = new();

            if (referenceList.Count > 0)
            {
                foreach (DatabaseReference childReference in referenceList)
                {
                    if (addConditions && int.Parse(childReference.Key) < addConditionsReference) { continue; }
                    
                    outReferenceList.Add(childReference);
                    t = new();
                    t.SetName(childReference.Key);
                    fireBaseDataDictionary[childReference.Key] = t;
                        
                }
                return outReferenceList;
            }
            else
            {
                Debug.LogWarning($"Data not found: Loading{t.GetType()}");
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

    private async Task FillGroup(string baseGroupLoad, DatabaseReference groupReference,
        Dictionary<string, Group> groups, DateTime startDateTime, DateTime endDateTime)
    {
        if (baseGroupLoad == null) return;
        string groupNumber = groupReference.Key;
        Group group = new()
        {
            Name = groupNumber,
        };

        GroupParsing groupParsing = new GroupParsing() { Name = baseGroupLoad };

        List<DatabaseReference> studentReferenceList = await GetChildAsync(groupReference, Student.Key);
        
        ProgressBar.Progress = 0.25f;
        if (studentReferenceList.Count > 0)
        {
            List<Student> students = new();

            for (int idStudent = 0;idStudent < studentReferenceList.Count;idStudent++)
            {
                DatabaseReference studentReference = studentReferenceList.Find(obj=> obj.Key == Student.KeyChild + idStudent);
                Student student = new Student()
                {
                    Name = (await ReadValue(studentReference.Child(Student.Key_NAME))).ToString(),
                    ID = Convert.ToInt32(await ReadValue(studentReference.Child(Student.Key_ID))),
                    Type = Convert.ToBoolean((await ReadValue(studentReference.Child(Student.Key_TYPE))) ?? true),
                };
                List<DatabaseReference> certificatesReference =
                    await GetChildAsync(studentReference, Student.Key_CERTIFICATES);
                foreach (DatabaseReference certificateReference in certificatesReference)
                {
                    student.Certificates[certificateReference.Key] =
                        (await ReadValue(certificateReference.Child(Student.Key_CERTIFICATESSTART))).ToString();
                }

                students.Add(student);
            }
            
            group.Students = students;
        }
//#if UNITY_EDITOR
        
        else Debug.LogWarning("Data not found: LoadingForName, studentReference");
        
//#endif
        ProgressBar.Progress = 0.35f;
        List<DatabaseReference> dateReferenceList = GetDatesReference(groupReference, startDateTime, endDateTime);
        if (dateReferenceList.Count > 0)
        {
            Dictionary<string, Dates> dates = new();
            
            foreach (DatabaseReference dateReference in dateReferenceList)
            {
                ProgressBar.Progress += 0.65f / dateReferenceList.Count;
                Dates date = new Dates();
                DateParse dateParse = new() { dateTime = dateReference.Key };

                List<DatabaseReference> lessonReferenceList = await GetChildAsync(dateReference, LessonFireBase.Key);

                if (lessonReferenceList.Count > 0)
                {
                    List<LessonFireBase> lessons = new List<LessonFireBase>();

                    foreach (DatabaseReference lessonReference in lessonReferenceList)
                    {
                        LessonFireBase lesson = new LessonFireBase()
                        {
                            Name = (await ReadValue(lessonReference.Child(LessonFireBase.Key_NAME))).ToString(),
                            type = LessonFireBase.isTypeLesson(
                                (await ReadValue(lessonReference.Child(LessonFireBase.Key_TYPE))).ToString())
                        };


                        List<DatabaseReference> studentMissingReferenceList =
                            await GetChildAsync(lessonReference, StudentMissing.Key);

                        if (studentMissingReferenceList.Count > 0)
                        {
                            List<StudentMissing> studentsMissing = new List<StudentMissing>();

                            foreach (DatabaseReference studentMissingReference in studentMissingReferenceList)
                            {
                                StudentMissing studentMissing = new StudentMissing()
                                {
                                    ID = Convert.ToInt32(
                                        await ReadValue(studentMissingReference.Child(StudentMissing.Key_ID)))
                                };
                                studentMissing.Type = Convert.ToBoolean(
                                    await ReadValue(
                                        studentMissingReference.Child(StudentMissing
                                            .Key_TYPE)));
                                if (!studentMissing.Type)
                                {
                                    studentMissing.Type = FindCertificate(
                                        group.Students[studentMissing.ID].Certificates,
                                        DateTime.Parse(dateReference.Key));
                                    if (studentMissing.Type) await UpdateMissingStudents(studentMissingReference, true);
                                }

                                studentsMissing.Add(studentMissing);
                            }

                            lesson.StudentsMissing = studentsMissing;
                        }
                        //else Debug.LogWarning("Data not found: LoadingForName, studentsMissingReference");

                        lessons.Add(lesson);
                        dateParse.Lessons.Add(lesson.Name);
                    }

                    date.lessons = lessons;
                    dates[dateReference.Key] = date;
                    groupParsing.DateParses.Add(dateParse);
                }
                //else Debug.Log("Data not found: LoadingForName, lessonReference");

            }

            group.Dates = dates;
        }
        else Debug.LogError("Data not found: LoadingForName, lessonReference");

        groups[group.Name] = group;
        Parsing.ParsingData1[baseGroupLoad] = groupParsing;
    }

    private async Task<bool> LoadingForData(string baseGroupLoad, DateTime? startDateTime = null, DateTime? endDateTime = null)
    {
        ProgressBar.Progress = 0;
        DateTime startDateTimeNn = startDateTime ?? Times.FbDefouldStartParsing;
        DateTime endDateTimeNn = endDateTime ?? Times.FbDefouldEndParsing;
        
        string[] GroupNameDat = RedactSearchText.ConverterGroupNameData(baseGroupLoad).ToArray(); 
        
            if (GroupNameDat[0] == null) return false;

            List<Faculty> facultyList = fireBaseData.Faculties;

        if (facultyList.Count > 0)
        {
            foreach (Faculty faculty in facultyList)
            {
                if (baseGroupLoad == null)
                {
                    Dictionary<string, Specialization> specializations = new();
                    List<DatabaseReference> specializationReferenceList = await ParsClassData(faculty.Reference, specializations/*, GroupNameDat, 1*/);

                    foreach (DatabaseReference specializationReference in specializationReferenceList)
                    {
                        Dictionary<string, Year> years = new();
                        List<DatabaseReference> yearReferenceList = await ParsClassData(specializationReference, years/*, GroupNameDat, 0*/, true, 25 - 3);

                        specializations.TryGetValue(specializationReference.Key, out Specialization specialization);

                        foreach (DatabaseReference yearReference in yearReferenceList)
                        {

                            Dictionary<string, Group> groups = new();
                            List<DatabaseReference> groupReferenceList = await ParsClassData(yearReference, groups/*, GroupNameDat, 2*/);

                            years.TryGetValue(yearReference.Key, out Year year);

                            foreach (DatabaseReference groupReference in groupReferenceList)
                            {
                                await FillGroup(baseGroupLoad,groupReference,groups, startDateTimeNn,endDateTimeNn);
                            }

                            year.Groups = groups;
                            years[year.Name] = year;

                        }

                        specialization.Years = years;
                        specializations[specialization.Name] = specialization;

                    }

                    faculty.Specializations = specializations;
                }
            }
        }
        else Debug.LogError("Data not found: LoadingForName, facultiesReferenceList");

        if (baseGroupLoad != null)
        {
            ProgressBar.Progress += 0.1f;
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
                ProgressBar.Progress += 0.1f;
                await FillGroup(baseGroupLoad,groupReference, year.Groups, startDateTimeNn, endDateTimeNn);
                ProgressBar.Progress = 1f;
            }
            else
            {
                return false;
            }
        }
        return true;
    }


    private async Task StartLoadingDataFb()
    {
        bool adminLoad = await Administration();
        bool facultyLoad = await StartLoadingFaculty();
        _isStartLoading = adminLoad && facultyLoad;
    }
    private async Task<bool> Administration() {
        try
        {
            DatabaseReference administrationReference = reference.Child(FireBaseData.Key_ADMINISTRATION).Child(StartLoadingData.UUID);
            if (administrationReference == null) return false;
        
            List<string> administrationGroupEnum = (await ReadValueEnum<string>(administrationReference));

            if (administrationGroupEnum.Count > 0)
            {
                fireBaseData.IsAdministration = true;

                fireBaseData.NameGroupAdministration = administrationGroupEnum;
            }
            else
            {
                fireBaseData.IsAdministration = false;
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }

    }
    private async Task<bool> StartLoadingFaculty()
    {
        List<DatabaseReference> facultyReferenceList = await GetChildAsync(reference, Faculty.key);

        if (facultyReferenceList.Count > 0)
        {
            foreach (DatabaseReference facultyReference in facultyReferenceList)
            {
                Faculty faculty = new Faculty(facultyReference)
                {
                    Name = facultyReference.Key
                };
                fireBaseData.Faculties.Add(faculty);
            }

            if (fireBaseData.Faculties.Count > 0) return true;
        }

        return false;
    }

    #region WorkData
    private async Task<object> ReadValue(DatabaseReference reference_Data) {
        try
        {
            object totalChildren = null;

            await reference_Data.GetValueAsync().ContinueWith(task =>
            {
                //DataSnapshot result = task?.Result;
                totalChildren = task?.Result?.Value;
                //Do more stuff
            });

            return totalChildren;
        }
        catch
        {
            return null;
        }
    }
    private static async Task<List<DatabaseReference>> ReadReferenceEnum(DatabaseReference referenceData) {
        try
        {
            List<DatabaseReference> totalChildrenRef = new();

            await referenceData.GetValueAsync().ContinueWith(task =>
            {
                List<DataSnapshot> totalChildren = task.Result.Children.ToList();
                totalChildren.ForEach(obj => totalChildrenRef.Add(obj.Reference));
                //Do more stuff
            });

            return totalChildrenRef;
        }
        catch
        {
            return new();
        }
    }
    private async Task<List<T>> ReadValueEnum<T>(DatabaseReference reference_Data){
        try
        {
            List<DataSnapshot> totalChildren = new();
            List<T> totalChildrenValue = new();

            await reference_Data.GetValueAsync().ContinueWith(task =>
            {
                totalChildren = task.Result.Children.ToList();
                totalChildren.ForEach(obj => totalChildrenValue.Add((T)obj.Value));
                //Do more stuff
            });

            return totalChildrenValue;
        }
        catch
        {
            return new();
        }
    }
    private async Task<int> ChildCount(DatabaseReference referenceData) {
        try
        {
            int totalChildren = 0;

            await referenceData.GetValueAsync().ContinueWith(task =>
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
    private List<DatabaseReference> GetDatesReference(DatabaseReference reference, DateTime startDateTime, DateTime endDateTime) {
        try
        {
            DatabaseReference datesReference = reference.Child(Dates.Key);

            List<DatabaseReference> dates = new();
            if (endDateTime - startDateTime < new TimeSpan(93, 0, 0, 0))
            {
                for (DateTime dateTime = startDateTime; dateTime <= endDateTime; dateTime += new TimeSpan(1, 0, 0, 0))
                {
                    dates.Add(datesReference.Child(dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year));
                }
            }

            return dates;

        }
        catch
        {
            return null;
        }
    }
    private static async Task<List<DatabaseReference>> GetChildAsync(DatabaseReference reference, string key = null) {
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
            int childCount = await ChildCount(reference_Group);
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
            if (DateTime.Parse(date) >= Times.TimeAdd3Day) { return true; }
            DatabaseReference referenceLesson = referenceGroup
                .Child(Dates.Key).Child(date)
                .Child(LessonFireBase.Key);

            List<DatabaseReference> allLesson = await GetChildAsync(referenceLesson);
            for (int idLesson = 0;idLesson <= allLesson.Count; idLesson++)
            {
                if (idLesson == allLesson.Count)
                {
                    for (; idLesson < names.Count; idLesson++)
                    {
                        DatabaseReference newLesson = referenceLesson.Child(LessonFireBase.Key + idLesson);

                        await UpdateData(newLesson, LessonFireBase.Key_NAME, names[idLesson]);
                        await UpdateData(newLesson, LessonFireBase.Key_TYPE, LessonFireBase.ConvertToString(types[idLesson]));
                    }
                    break;
                }

                DatabaseReference lesson = allLesson[idLesson];

                if (idLesson == names.Count)
                {
                    for (; idLesson < allLesson.Count; idLesson++)
                    {
                        await DeleteLesson(lesson);
                    }
                    break;
                }

                bool isNewLesson = (await ReadValue(lesson.Child(LessonFireBase.Key_NAME))).ToString() != names[idLesson] || (await ReadValue(lesson.Child(LessonFireBase.Key_TYPE))).ToString() != types[idLesson].ToString();


                if (false && isNewLesson)
                {
                    await DeleteLesson(lesson);

                    DatabaseReference newLesson = referenceLesson.Child(LessonFireBase.Key + idLesson);
                    await UpdateData(newLesson, LessonFireBase.Key_NAME, names[idLesson]);
                    await UpdateData(newLesson, LessonFireBase.Key_TYPE, LessonFireBase.ConvertToString(types[idLesson]));
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
    public static async Task<bool> CreateStudent(string groupName, string studentName, bool typeStudent) {
        try
        {
            DatabaseReference groupReference = await _instance.SearchGroupReference(groupName);
            DatabaseReference studentReference = groupReference.Child(Student.Key);
            ProgressBar.Progress = 0.1f;
            int childCount = await _instance.ChildCount(studentReference);
            ProgressBar.Progress = 0.3f;
            DatabaseReference newStudent = studentReference.Child(Student.KeyChild + childCount);
            await _instance.UpdateData(newStudent, Student.Key_NAME, studentName);
            ProgressBar.Progress = 0.6f;
            await _instance.UpdateData(newStudent, Student.Key_ID, childCount);
            ProgressBar.Progress = 0.9f;
            await _instance.UpdateData(newStudent, Student.Key_TYPE, typeStudent);
            ProgressBar.Progress = 1f;

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }
    public static async Task CreateCertificate(string groupName,int studentID, string dateStart, string dateEnd)
    {
        ProgressBar.Progress = 0.5f;
        DatabaseReference groupReference = await _instance.SearchGroupReference(groupName);
        ProgressBar.Progress = 0.7f;
        DatabaseReference dateCertificateReference = groupReference.
            Child(Student.Key).Child("Student" + studentID).
            Child(Student.Key_CERTIFICATES).Child(dateEnd);
        
        await _instance.UpdateData(dateCertificateReference, Student.Key_CERTIFICATESSTART, dateStart);
        ProgressBar.Progress = 1f;
    }
    

    #region Update

    public static async Task UpdateMissingStudents(string groupName,string dateLesson,int lessonID,int studentID, bool isCreate)
    {
        DatabaseReference groupReference = await _instance.SearchGroupReference(groupName);
        DatabaseReference studentMissingReference = groupReference.Child(Dates.Key).Child(dateLesson)
            .Child(LessonFireBase.Key).Child(LessonFireBase.Key + lessonID)
            .Child(StudentMissing.Key).Child(StudentMissing.Key + studentID);

        if (isCreate)
        {
            await _instance.UpdateData(studentMissingReference, StudentMissing.Key_TYPE, false);
            await _instance.UpdateData(studentMissingReference, StudentMissing.Key_ID, studentID);
        }
        else
        {
            await DeleteStudentMissing(studentMissingReference);
        }
        
    }
    private static async Task UpdateMissingStudents(DatabaseReference studentMissingReference,bool isLegal)
    {
        await _instance.UpdateData(studentMissingReference, StudentMissing.Key_TYPE, isLegal);
    }
    private async Task UpdateData(DatabaseReference reference, string key, object data) {
        await reference.Child(key).SetValueAsync(data);
    }

    #endregion
    
    #endregion


    private static bool FindCertificate(Dictionary<string,string> certificates, DateTime dateTotal)
    {
        foreach (var keyValuePair in certificates)
        {
            if (DateTime.Parse(keyValuePair.Key) >= dateTotal)
            {
                if (DateTime.Parse(keyValuePair.Value) <= dateTotal)
                {
                    return true;
                };
            }
        }

        return false;
    }
    public static List<KeyValuePair<string,string>> FindCertificate(Dictionary<string,string> certificates, DateTime monthTotal,bool first)
    {
        List<KeyValuePair<string,string>> certificatesList = new();
        foreach (var keyValuePair in certificates)
        {
            if (DateTime.Parse(keyValuePair.Key) >= monthTotal)
            {
                if (DateTime.Parse(keyValuePair.Value).Month <= monthTotal.Month)
                {
                    certificatesList.Add(keyValuePair);
                    if (first) return certificatesList;
                };
            }
        }

        return certificatesList;
    }
    public async Task<bool> IsCreatingDate(string date, DatabaseReference groupReference)
    {
        DatabaseReference dateReference = groupReference.Child(Dates.Key).Child(date);
        int childCount = await ChildCount(dateReference);
        return childCount == 0;
    }
    private async Task DeleteLesson(DatabaseReference lessonReference) {
        try
        {
            List<DatabaseReference> childReferenceGroup = await GetChildAsync(lessonReference);
            foreach (var item in childReferenceGroup)
            {
                if(item.Key == StudentMissing.Key)
                {
                    List<DatabaseReference> childReferenceStudentMissing = await GetChildAsync(item);
                    foreach (var itemStudent in childReferenceStudentMissing)
                    {
                        await itemStudent.Child(StudentMissing.Key_ID).RemoveValueAsync();
                        await itemStudent.Child(StudentMissing.Key_TYPE).RemoveValueAsync();
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
    private static async Task DeleteStudentMissing(DatabaseReference studentMissingReference) {
        try
        {
            List<DatabaseReference> childReferenceGroup = await GetChildAsync(studentMissingReference);
            foreach (var item in childReferenceGroup)
            {
                if(item.Key is StudentMissing.Key_TYPE or StudentMissing.Key_ID)
                {
                    await item.RemoveValueAsync();
                }
            }

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
    private async Task DeleteGroup(string name, DatabaseReference groupReference = null) {

        try
        {
            DatabaseReference group;
            if (groupReference != null) group = groupReference;
            else group = await SearchGroupReference(name);

            Debug.Log(name + " DeleteGroup");
            List<DatabaseReference> childReferenceGroup = await GetChildAsync(group.Parent, group.Key);
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
        string facultyNameDefould = "New";

        foreach (Faculty faculty in fireBaseData.Faculties)
        {
            DatabaseReference specializationReference = faculty.Reference.Child(Specialization.key).Child(name);

            if (await ChildCount(specializationReference) > 0)
                return faculty.Reference.Key;
        }

        return facultyNameDefould;

    }
    public async Task<DatabaseReference> SearchGroupReference(string name) {
        string[] groupData = RedactSearchText.ConverterGroupNameData(name);

        foreach (Faculty faculty in fireBaseData.Faculties)
        {
            DatabaseReference specializationReference = faculty.Reference.Child(Specialization.key).Child(groupData[1]);
            DatabaseReference yearReference = specializationReference.Child(Year.key).Child(groupData[0]);

            List<DatabaseReference> groupReferenceList = await GetChildAsync(yearReference, Group.key);
            foreach (var groupReference in groupReferenceList)
            {
                if (groupReference.Key == groupData[2])
                {
                    facultyName = faculty.Reference.Key;
                    return groupReference;
                }
            }
        }

        return null;
    }
}
