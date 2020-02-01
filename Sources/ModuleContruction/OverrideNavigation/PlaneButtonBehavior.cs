using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaneButtonBehavior : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    Selectable refButton;
    static private Material privateMatOn;
    static private Material privateMatOff;
    private void Start()
    {
        refButton = gameObject.GetComponent<Selectable>();

        if(privateMatOn == null)
        {
            privateMatOn = Instantiate(gameObject.GetComponent<Image>().material);
            privateMatOn.SetFloat("_IsActive", 1.0f);
        }

        if (privateMatOff == null)
        {
            privateMatOff = Instantiate(gameObject.GetComponent<Image>().material);
            privateMatOff.SetFloat("_IsActive", 0.0f);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!refButton.interactable)
        {
            refButton.targetGraphic.color = refButton.colors.highlightedColor;
        }

        gameObject.GetComponent<Image>().material = privateMatOn;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!refButton.interactable)
        {
            refButton.targetGraphic.color = refButton.colors.normalColor;
        }
        gameObject.GetComponent<Image>().material = privateMatOff;
    }

    public void OnSubmit(BaseEventData eventData)
    {
        gameObject.GetComponent<Image>().material = privateMatOn;
    }
}
