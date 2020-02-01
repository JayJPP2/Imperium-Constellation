using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleScript : MonoBehaviour, ISelectHandler, ISubmitHandler, ICancelHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        this.GetComponent<Toggle>().isOn = true;
        float width = GetComponent<RectTransform>().rect.width;
        transform.parent.GetComponent<HorizontalLayoutGroup>().padding.left = -(int)(width * transform.GetSiblingIndex() + width / 2.0f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }


    public void OnCancel(BaseEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(transform.parent.parent.parent.GetChild(0).gameObject);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        ConstructionModuleMain.GetSingleton().SelectHub();
    }
}
