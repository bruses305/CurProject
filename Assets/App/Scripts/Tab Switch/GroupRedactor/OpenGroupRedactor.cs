using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class OpenGroupRedactor : MonoBehaviour
{
    public static OpenGroupRedactor Instance;
    [SerializeField] private GameObject parentGroupsRedactor;
    [SerializeField] private GameObject Redactor;
    [SerializeField] private Transform imageIconList;
    public static event EventHandler LoadingEnd;
    private const int ID_WINDOW = 5;
    private void Awake() {
        Instance = this;
    }
    public static void ActivateListGroups() {
        Instance.imageIconList.rotation = !Instance.parentGroupsRedactor.activeSelf ? new(0, 0, 0, 0) : new(0, 0, -90, 0);
        Instance.parentGroupsRedactor.SetActive(!Instance.parentGroupsRedactor.activeSelf);
    }
    public async static void LoadingRedactor(string groupName) {
        TabsSwitcher.Instance.SwitchTabID(ID_WINDOW);
        await Parsing.Instance.LoadingDefouldData(groupName, true);
        LoadingEnd.Invoke(null,EventArgs.Empty);
    }
}
