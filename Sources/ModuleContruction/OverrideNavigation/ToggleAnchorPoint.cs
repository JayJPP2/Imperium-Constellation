using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ToggleAnchorPoint : MonoBehaviour, ISelectHandler, ISubmitHandler, ICancelHandler, IDeselectHandler
{
    public void OnCancel(BaseEventData eventData)
    {
        this.GetComponent<Toggle>().isOn = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(transform.parent.parent.gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (gameObject.transform.childCount > 1)
        {
            for (int i = 1; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<Image>().color = gameObject.GetComponent<Toggle>().colors.normalColor;
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(gameObject.transform.childCount > 1)
        {
            for (int i = 1; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<Image>().color = gameObject.GetComponent<Toggle>().colors.highlightedColor;
            }
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        this.GetComponent<Toggle>().isOn = true;
    }
}
