using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadPanelButtonBehavior : MonoBehaviour, ICancelHandler, ISelectHandler, IDeselectHandler
{
    private float height;
    GameObject parentContent;
    private float StartUpValue;

    static private Material privateMatOn;
    static private Material privateMatOff;

    private void Awake()
    {
        parentContent = transform.parent.parent.GetChild(2).GetChild(0).gameObject;

        if (privateMatOn == null)
        {
            privateMatOn = Instantiate(gameObject.GetComponent<Image>().material);
            privateMatOn.SetFloat("_IsActive", 1.0f);
        }

        if (privateMatOff == null)
        {
            privateMatOff = Instantiate(gameObject.GetComponent<Image>().material);
            privateMatOff.SetFloat("_IsActive", 0.0f);
        }

        UpdateValues();
    }

    public void UpdateValues()
    {
        if (parentContent.transform.childCount > 0)
        {
            height = parentContent.transform.GetChild(0).GetComponent<RectTransform>().rect.height + parentContent.transform.GetComponent<VerticalLayoutGroup>().spacing;
        }
        else
        {
            height = 0.0f;
        }
        StartUpValue = (parentContent.transform.childCount) * (height) - (height) + 100.0f;
    }

    public void OnCancel(BaseEventData eventData)
    {
        LoadShipManagement.GetSingleton().CancelLoadPanel();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Button refButton = gameObject.GetComponent<Button>();

        gameObject.GetComponent<Image>().material = privateMatOn;

        if (!refButton.interactable)
        {
            refButton.targetGraphic.color = refButton.colors.highlightedColor;
        }

        Navigation nav = refButton.navigation;
        int indexOn;
        if(LoadShipManagement.GetSingleton().HasPanelOn(out indexOn))
        {
            nav.selectOnLeft = parentContent.transform.GetChild(indexOn).GetComponent<Toggle>();
        }
        else
        {
            if (parentContent.transform.childCount > 0)
            {
                float t = parentContent.GetComponent<VerticalLayoutGroup>().padding.top;
                float index = (-t + StartUpValue) / (2 * height);

                nav.selectOnLeft = parentContent.transform.GetChild((int)index).GetComponent<Toggle>();
            }
            else
            {
                nav.selectOnLeft = null;
            }
        }
        refButton.navigation = nav;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Button refButton = gameObject.GetComponent<Button>();

        gameObject.GetComponent<Image>().material = privateMatOff;

        if (!refButton.interactable)
        {
            refButton.targetGraphic.color = refButton.colors.normalColor;
        }
    }
}
