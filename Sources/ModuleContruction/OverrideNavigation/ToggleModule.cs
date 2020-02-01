using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ToggleModule : MonoBehaviour, ISelectHandler, ICancelHandler, ISubmitHandler
{
    public void ResetPosition()
    {
        transform.parent.GetComponent<HorizontalLayoutGroup>().padding.left = 0;
    }

    public void OnSelect(BaseEventData eventData)
    {
        this.GetComponent<Toggle>().isOn = true;
        float width = GetComponent<RectTransform>().rect.width;
        transform.parent.GetComponent<HorizontalLayoutGroup>().padding.left = -(int)(width * transform.GetSiblingIndex());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    public void OnCancel(BaseEventData eventData)
    {
        ConstructionModuleMain.GetSingleton().BackOutModuleWithCancel();
        Toggle anchorPointHolder = ConstructionModuleMain.GetSingleton().ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => x.GetComponent<Toggle>()).First();
        anchorPointHolder.isOn = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(anchorPointHolder.gameObject);
        ConstructionModuleMain.GetSingleton().ChangeCameraState(0);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        ConstructionModuleMain.GetSingleton().SaveUpModuleSelected();
        Toggle anchorPointHolder = ConstructionModuleMain.GetSingleton().ToggleHoriList.Where(x => x.GetComponent<Toggle>().isOn == true).Select(x => x.GetComponent<Toggle>()).First();
        anchorPointHolder.isOn = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(anchorPointHolder.gameObject);
        ConstructionModuleMain.GetSingleton().ChangeCameraState(0);
    }
}
