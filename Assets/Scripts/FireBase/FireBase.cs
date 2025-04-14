using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework.Internal;

public class FireBase
{
    static public FireBaseData fireBaseData = new();
    public event EventHandler ParsingFireBaseEnd;

    private DatabaseReference reference;
    private FirebaseAuth AutorizationPlayer;


    public async void Initialized() {

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        AutorizationPlayer = FirebaseAuth.DefaultInstance;

        object connectingTest = await ReadValue(reference.Child("Connection").Child("Test"));
        if (connectingTest is bool != true) Debug.Log("Fail Connection");
        else Debug.Log("successful connection, Initialization End");

    }
    public async Task<bool> LoadingData() {
        try
        {
            bool Admin = await Administration();
            bool Data = await LoadingForData(Parsing.parsingGroupName);
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
        fireBaseData = new();
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

    private async Task<List<DatabaseReference>> TestMetod<T>(DatabaseReference reference, Dictionary<string, T> fireBaseDataDictionary, string[] GroupNameDat, int idgr, bool addConditions = false, int addConditionsReference = 0) where T : IFireBaseData, new() {
        try
        {
            T t = new();

            List<DatabaseReference> referenceList = await GetChiledsAsync(reference, t.Key);

            if (referenceList.Count > 0)
            {
                foreach (DatabaseReference childReference in referenceList)
                {
                    if (childReference.Key != GroupNameDat[idgr] && GroupNameDat[idgr] != null 
                        && addConditions && int.Parse(childReference.Key) < addConditionsReference) { continue; }
                    t = new();
                    t.SetName(childReference.Key);
                    fireBaseDataDictionary[childReference.Key] = t;
                }
                return referenceList;
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

    public async Task<bool> LoadingForData(string baseGroupLoad) {
        string[] GroupNameDat = FormingTabelDate.ConverterGroupNameData(baseGroupLoad).ToArray(); //0-год 1-специальность 2-группа
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

                Dictionary<string, Specialization> specializations = new();
                List<DatabaseReference> specializationReferenceList = await TestMetod(facultyReference, specializations, GroupNameDat, 1); // 1 - специальность

                foreach (DatabaseReference specializationReference in specializationReferenceList)
                {
                    Dictionary<string, Year> years = new();
                    List<DatabaseReference> yearReferenceList = await TestMetod(specializationReference, years, GroupNameDat, 0, true, 25 - 3); // 0 - год  25 - текущий год

                    Specialization specialization = specializations[specializationReference.Key];

                    foreach (DatabaseReference yearReference in yearReferenceList)
                    {
                        Dictionary<string, Group> groups = new();
                        List<DatabaseReference> groupReferenceList = await TestMetod(specializationReference, groups, GroupNameDat, 2); // 2 - группа

                        Year year = years[yearReference.Key];

                        foreach (DatabaseReference groupReference in groupReferenceList)
                        {
                            Group group = groups[groupReference.Key];
                            //await GetChiledsAsync(groupReference,Dates.Key);   непонятный код

                            List<DatabaseReference> dateReferenceList = GetDatesReference(groupReference, DateTime.Now - new TimeSpan(1, 0, 0, 0, 0), DateTime.Now);
                            if (dateReferenceList.Count > 0)
                            {
                                Dictionary<string, Dates> dates = new();

                                foreach (DatabaseReference dateReference in dateReferenceList)
                                {
                                    Dates date = new Dates();


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

                                                lesson.StudentMissings = studentsMissing;
                                                lessons.Add(lesson);
                                            }
                                            else Debug.LogError("Data not foundit: LoadingForName, studentsMissingReference");

                                            lessons.Add(lesson);
                                        }

                                        date.lessons = lessons;
                                        dates[dateReference.Key] = date;
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
                        }

                        year.Groups = groups;
                        years[year.Name] = year;

                    }

                    specialization.Years = years;
                    specializations[specialization.Name] = specialization;

                }

                faculty.Specializations = specializations;

                fireBaseData.Faculties.Add(faculty);
            }
        }
        else Debug.LogError("Data not foundit: LoadingForName, facultiesReferenceList");


        return true;
    }

    #region WorkData
    private async Task<object> ReadValue(DatabaseReference reference_Data) {
        try
        {
            object totalChildren = null;

            await reference_Data.GetValueAsync().ContinueWith(task =>
            {
                totalChildren = task.Result.Value;
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
    private List<DatabaseReference> GetDatesReference(DatabaseReference reference, DateTime startDateTime, DateTime endDateTime) {
        try
        {
            DatabaseReference dateReference = reference.Child(Dates.Key);

            List<DatabaseReference> dates = new();
            if (endDateTime - startDateTime < new TimeSpan(93, 0, 0, 0))
            {
                for (DateTime dateTime = startDateTime; dateTime < endDateTime; dateTime += new TimeSpan(1, 0, 0, 0))
                {
                    dates.Add(dateReference.Child(dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year));
                }
            }
            return dates;

        }
        catch
        {
            return null;
        }
    }
    private async Task<List<DatabaseReference>> GetChiledsAsync(DatabaseReference reference, string key) {
        try
        {
            DatabaseReference chiledReference = reference.Child(key);
            return await ReadReferenceEnum(chiledReference);
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Create

    public async Task<bool> CreateGroup(string name, string idFaculty, string idSpecialization, string idYear) {
        try
        {


            DatabaseReference reference_Group = reference.Child(Faculty.key).Child(idFaculty)
                .Child(Specialization.key).Child(idSpecialization)
                .Child(Year.key).Child(idYear)
                .Child(Group.key);

            int countChiled = await ChildCount(reference_Group);
            DatabaseReference newFaculty = reference_Group.Child(Group.key + countChiled);
            await UpdateData(newFaculty, Group.Key_NAME, name);

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }
    public async Task<bool> CreateLesson(string name, LessonFireBase.TypeLesson type, string date, string idFaculty, int idSpecialization, int idYear, int idGroup) {
        try
        {

            DatabaseReference reference_Lesson = reference.Child(Faculty.key).Child(idFaculty)
                .Child(Specialization.key).Child(Specialization.key + idSpecialization)
                .Child(Year.key).Child(Year.key + idYear)
                .Child(Group.key).Child(Group.key + idGroup)
                .Child(Dates.Key).Child(date)
                .Child(LessonFireBase.Key);
            int countChiled = await ChildCount(reference_Lesson);
            DatabaseReference newFaculty = reference_Lesson.Child(LessonFireBase.Key + countChiled);
            await UpdateData(newFaculty, LessonFireBase.Key_NAME, name);
            await UpdateData(newFaculty, LessonFireBase.Key_TYPE, LessonFireBase.ConvertToString(type));

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

    public async void DeliteGroup(string name) {

        try
        {
            Debug.Log(name + " DeliteGroup");
            DatabaseReference group = await SearchGroupReference(name);
            await group.RemoveValueAsync();

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

    }

    private async Task<DatabaseReference> SearchGroupReference(string name) {
        string[] groupData = FormingTabelDate.ConverterGroupNameData(name);

        List<DatabaseReference> facultyReferenceList = await GetChiledsAsync(reference, Faculty.key);

        foreach (var facultyReference in facultyReferenceList)
        {
            List<DatabaseReference> specializationReferenceList = await GetChiledsAsync(reference, Faculty.key);
            foreach (var specializationReference in specializationReferenceList)
            {
                if (specializationReference.Key == groupData[1])
                {
                    List<DatabaseReference> GroupReferenceList = await GetChiledsAsync(specializationReference.Child(Year.key).Child(groupData[0]), Group.key);
                    foreach (var GroupReference in GroupReferenceList)
                    {
                        if((await ReadValue(GroupReference.Child(Group.Key_NAME))).ToString() == groupData[2])
                        {
                            return GroupReference;
                        }
                    }
                }
            }
        }

        return null;
    }
}
