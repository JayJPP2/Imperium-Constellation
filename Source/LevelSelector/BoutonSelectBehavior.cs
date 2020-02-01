using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoutonSelectBehavior : MonoBehaviour, ISubmitHandler, ICancelHandler, ISelectHandler
{
    public static bool isSelected { get; private set; }

    private void Awake()
    {
        isSelected = false;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        GetComponent<Toggle>().targetGraphic.color = GetComponent<Toggle>().colors.pressedColor;
        isSelected = true;
        LevelManager.GetSingleton().PlanetHasBeenSelected(transform.GetSiblingIndex());
    }

    public void OnCancel(BaseEventData eventData)
    {
        GetComponent<Toggle>().targetGraphic.color = GetComponent<Toggle>().colors.normalColor;
        isSelected = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        LevelManager.GetSingleton().SetOrbitCanvas(transform.GetSiblingIndex());
    }
}
