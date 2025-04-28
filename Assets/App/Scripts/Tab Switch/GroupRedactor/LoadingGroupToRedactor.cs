using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingGroupToRedactor : MonoBehaviour
{
    [SerializeField] Button activatorListGrops;
    [SerializeField] GameObject GroupsRedactorParent;
    [SerializeField] GameObject prefabGroupRedacror;

    private void Awake() {
        Parsing.PageEvent += FireBase_ParsingFireBaseEnd;
        activatorListGrops.onClick.AddListener(()=>OpenGroupRedactor.ActivateListGroups());
    }

    private void FireBase_ParsingFireBaseEnd(object sender, System.EventArgs e) {

        activatorListGrops.gameObject.SetActive(false);
        if (FireBase.fireBaseData.IsAdministration)
        {
            Debug.Log("Loading Groups Redactor");
            activatorListGrops.gameObject.SetActive(true);
            CreateGroupsRedactor();
        }
    }

    private void CreateGroupsRedactor() {
        foreach(var groupName in FireBase.fireBaseData.NameGroupAdministration)
        {
            GameObject groupObject = Instantiate(prefabGroupRedacror, GroupsRedactorParent.transform);
            groupObject.transform.GetChild(0).GetChild(0).TryGetComponent(out TextMeshProUGUI tmp);
            tmp.text = groupName;
            groupObject.TryGetComponent(out Button buttonComponent);
            buttonComponent.onClick.AddListener(() => OpenGroupRedactor.LoadingRedactor(groupName));
        }
    }
}
