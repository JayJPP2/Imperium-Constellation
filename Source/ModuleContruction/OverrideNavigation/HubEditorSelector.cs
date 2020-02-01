using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HubEditorSelector : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.GetComponent<Image>().material.SetFloat("_IsActive", 0.0f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        gameObject.GetComponent<Image>().material.SetFloat("_IsActive", 1.0f);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ConstructionModuleMain.GetSingleton().ToggleModuleList[0].Find(x => x.isOn == true).gameObject);    
    }
}
