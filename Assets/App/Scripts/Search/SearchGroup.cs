using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class SearchGroup : MonoBehaviour
{
    [SerializeField] private TMP_InputField searchPanel;
    [SerializeField] private Parsing parsingObject;

    public async void Search() {
        string groupName = searchPanel.text;

         await Task.Run(() => parsingObject.LoadingDefouldData(RedactSearchText.UpperText(groupName), true));

    }
}
