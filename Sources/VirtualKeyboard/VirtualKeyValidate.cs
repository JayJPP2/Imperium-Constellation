using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualKeyValidate : MonoBehaviour, ICancelHandler
{
    Button bRef;
    InputField ifRef;
    // Use this for initialization
    void Start()
    {
        ifRef = transform.parent.parent.GetComponent<InputField>();
        bRef = gameObject.GetComponent<Button>();
        bRef.onClick.AddListener(() =>
        {
            if (ifRef.text.Length > 3)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(transform.parent.parent.gameObject);
                transform.parent.gameObject.SetActive(false);
                ConstructionModuleMain.GetSingleton().ValidateSaveButtonIsOn();
            }
        });
    }

    public void OnCancel(BaseEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(transform.parent.parent.gameObject);
        transform.parent.parent.GetComponent<InputFieldBehavior>().RollBackToSave();
        transform.parent.gameObject.SetActive(false);
    }
}
