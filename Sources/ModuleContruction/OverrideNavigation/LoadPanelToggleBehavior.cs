using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadPanelToggleBehavior : MonoBehaviour, ICancelHandler, ISelectHandler, ISubmitHandler
{
    public void OnCancel(BaseEventData eventData)
    {
        LoadShipManagement.GetSingleton().CancelLoadPanel();
    }

    public void OnSelect(BaseEventData eventData)
    {
        float height = GetComponent<RectTransform>().rect.height + transform.parent.GetComponent<VerticalLayoutGroup>().spacing;
        float startingValue = (gameObject.transform.parent.childCount) * height - height + 100.0f;
        gameObject.transform.parent.GetComponent<VerticalLayoutGroup>().padding.top = (int)(startingValue - height * gameObject.transform.GetSiblingIndex() * 2);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    public void OnSubmit(BaseEventData eventData)
    {
        this.GetComponent<Toggle>().isOn = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(LoadShipManagement.GetSingleton().CancelButtonLoadPanel.gameObject);
    }
}
