using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ToggleModuleGroup : MonoBehaviour {

	Toggle toggleRef;


	void Start ()
	{
		toggleRef = gameObject.GetComponent<Toggle>();
	}

	void Update () 
	{
		if(!toggleRef.isOn)
		{
			return;
		}

        int valueToADD = 0;
		if(Input.GetAxis("Vertical") < 0)
		{
            valueToADD = 1;
		}
        else if(Input.GetAxis("Vertical") > 0)
        {
            valueToADD = -1;
        }
        else
        {
            return;
        }

        int indexActual = gameObject.transform.GetSiblingIndex() + 1;
        if (indexActual + valueToADD < 1 || indexActual + valueToADD>= ConstructionModuleMain.GetSingleton().ToggleList.Length)
        {
            return;
        }
        else
        {
            if(ConstructionModuleMain.GetSingleton().ToggleList[valueToADD + indexActual].interactable)
            {
                ConstructionModuleMain.GetSingleton().ToogleUpdateUI(valueToADD + indexActual);
            }
        }
	}
}
