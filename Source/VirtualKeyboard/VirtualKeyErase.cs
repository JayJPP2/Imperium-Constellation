using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualKeyErase : MonoBehaviour, ICancelHandler
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
            if(ifRef.text.Length > 0)
            {
                ifRef.text = ifRef.text.Substring(0, ifRef.text.Length - 1);
                if (ifRef.text.Length  < 4)
                {
                    transform.parent.GetChild(transform.parent.childCount - 1).GetComponent<Button>().interactable = false;
                }
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
