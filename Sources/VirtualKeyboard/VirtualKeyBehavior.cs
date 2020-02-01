using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualKeyBehavior : MonoBehaviour, ICancelHandler {

    string valueToSend;
    Button bRef;
    InputField ifRef;
    // Use this for initialization
	void Start () {
        valueToSend = gameObject.name;
        ifRef = transform.parent.parent.GetComponent<InputField>();
        bRef = gameObject.GetComponent<Button>();
        bRef.onClick.AddListener(() =>
        {
            ifRef.text += valueToSend;
            if(ifRef.text.Length > 3)
            {
                transform.parent.GetChild(transform.parent.childCount - 1).GetComponent<Button>().interactable = true;
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
