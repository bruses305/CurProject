
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectCells : MonoBehaviour
{
    [SerializeField]private Toggle isRedact;
    private Dictionary<Vector2Int,bool> SelectedCells => FormingTabelDate.MissingStudent;
    public static Dictionary<Vector2Int, bool> LocalSelectedCells = new();
    
    private Color colorSelect = new(45f / 255f, 60f / 255f, 63f / 255f);
    private Color colorBaseSelect = new(72f / 255f, 90f / 255f, 94f / 255f);
    private Color colorDefould = new(45f/255f, 42f / 255f, 63f / 255f);
    private List<List<Image>> NTableObjectCell;
    private Vector2Int defouldPositionPinned = new(-1,-1);
    private Vector2Int positionPinned = new(-1, -1);
    private void Start() {
        TableObjectData.updateTableData += updateTableData;
    }
    private void updateTableData(object sender, System.EventArgs e) {
        ConvertToImage(((TableObjectData)sender).tableTextCell.TableNCell);
    }
    public void isPinned(PointerEventData data, Vector2Int position) {
        if (!isRedact.isOn)
        {
            if (position == positionPinned)
            {
                positionPinned = defouldPositionPinned;
            }
            else
            {
                if (positionPinned != defouldPositionPinned)
                {
                    UnSelectNCells(null, positionPinned);
                    SelectNCells(null, position);
                }

                positionPinned = position;
            }
        }
        else
        {
            bool isActive = !SelectedCells.ContainsKey(position); //есть ли в базовом массиве эта переменная если есть то изменяет активность на false если нету то на true
            bool isLocalSelected = !LocalSelectedCells.Remove(position);
            if (isLocalSelected) {LocalSelectedCells[position] = isActive; //если в массиве есть элемент со значением true то он удаляется и не заходит в хуйню
            }
            //если в массиве нет элемента тоесть не выделена N то заходит в хуйню и смотрит isActive
            FormingTabelDate.Instance.TimeMissing(position, isActive?isLocalSelected:!isLocalSelected);
            RedactSelectedCells.SetActiveButton(LocalSelectedCells.Count > 0);
        }
    }

    public void SelectNCells(PointerEventData data, Vector2Int position) {
        if (positionPinned == defouldPositionPinned || data == null)
        {
            for (int x = 0; x < NTableObjectCell.Count; x++)
            {
                if (x == position.x)
                {
                    for (int y = 0; y < NTableObjectCell[x].Count; y++)
                    {
                        if (y == position.y)
                        {
                            NTableObjectCell[x][y].color = colorBaseSelect;
                        }
                        else
                            NTableObjectCell[x][y].color = colorSelect;
                    }
                }
                else
                    NTableObjectCell[x][position.y].color = colorSelect;
            }
        }
    }

    public void UnSelectNCells(PointerEventData data, Vector2Int position) {
        if (positionPinned == defouldPositionPinned || data == null)
        {
            for (int x = 0; x < NTableObjectCell.Count; x++)
            {
                if (x == position.x)
                {
                    for (int y = 0; y < NTableObjectCell[x].Count; y++)
                    {
                        NTableObjectCell[x][y].color = colorDefould;
                    }
                }
                else
                    NTableObjectCell[x][position.y].color = colorDefould;
            }
        }
    }

    private void ConvertToImage(List<List<TextMeshProUGUI>> TMPList) {
        List<List<Image>> ImageList = new();
        TMPList.ForEach(parentTMP => {
            List<Image> timeParametr = new();
            parentTMP.ForEach(TMP => timeParametr.Add(TMP.transform.parent.gameObject.GetComponent<Image>()));
            ImageList.Add(timeParametr);
            }
        );
        NTableObjectCell = ImageList;
    }
}
