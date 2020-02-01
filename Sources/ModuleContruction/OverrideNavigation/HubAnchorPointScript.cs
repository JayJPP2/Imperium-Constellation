using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HubAnchorPointScript : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    private Material privateMat;
    private bool hasSubmit;
    private void Awake()
    {
        gameObject.GetComponent<Image>().material = Instantiate(gameObject.GetComponent<Image>().material);
        privateMat = gameObject.GetComponent<Image>().material;
        hasSubmit = false;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if(!hasSubmit)
        {
            privateMat.SetFloat("_IsActive", 0.0f);
            privateMat.SetFloat("_IsOn", 0.0f);
        }
        hasSubmit = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        privateMat.SetFloat("_IsActive", 1.0f);
        privateMat.SetFloat("_IsOn", 0.0f);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        hasSubmit = true;
        privateMat.SetFloat("_IsActive", 0.0f);
        privateMat.SetFloat("_IsOn", 1.0f);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ConstructionModuleMain.GetSingleton().ToggleHoriList[0]);
    }
}
