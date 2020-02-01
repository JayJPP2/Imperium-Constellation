using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InputFieldBehavior : InputField
{
    private string saveUpLastString;

    private new void Awake()
    {
        saveUpLastString = null;
        base.Awake();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.targetGraphic.color = this.colors.highlightedColor;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.targetGraphic.color = this.colors.normalColor;
    }

    public void RollBackToSave()
    {
        this.text = saveUpLastString;
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.targetGraphic.color = this.colors.pressedColor;
        //PUT THE CONDITION TO SEE IF WE ARE WITH KEYBOARD OR CONTROLLER
        EventSystem.current.SetSelectedGameObject(null);
        transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
        saveUpLastString = this.text;
        EventSystem.current.SetSelectedGameObject(transform.GetChild(transform.childCount - 1).GetChild(0).gameObject);
    }
}
