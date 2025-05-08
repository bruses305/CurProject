using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoadingGroupToRedactor : MonoBehaviour
{
    [SerializeField] private Button activatorListGrops;
    [SerializeField] private GameObject groupsRedactorParent;
    [SerializeField] private GameObject prefabGroupRedacror;

    private void Awake() {
        Parsing.PageEvent += FireBase_ParsingFireBaseEnd;
        activatorListGrops.onClick.AddListener(()=>OpenGroupRedactor.ActivateListGroups());
    }

    private void FireBase_ParsingFireBaseEnd(object sender, System.EventArgs e) {

        activatorListGrops.gameObject.SetActive(false);
        if (FireBase.fireBaseData.IsAdministration)
        {
            activatorListGrops.gameObject.SetActive(true);
            CreateGroupsRedactor();
        }
    }

    private void CreateGroupsRedactor() {
        foreach(var groupName in FireBase.fireBaseData.NameGroupAdministration)
        {
            GameObject groupObject = Instantiate(prefabGroupRedacror, groupsRedactorParent.transform);
            groupObject.transform.GetChild(0).GetChild(0).TryGetComponent(out TextMeshProUGUI tmp);
            tmp.text = groupName;
        }
    }
}
