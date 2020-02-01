using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsUI : MonoBehaviour {

    public static void ToogleHorizontalUpdateUI(Toggle toggleCheck)
    {
        for (int i = 0; i < toggleCheck.transform.childCount; i++)
        {
            Image temp = toggleCheck.transform.GetChild(i).GetComponent<Image>();
            if (temp != null)
            {
                if (toggleCheck.isOn)
                {

                    temp.color = new Color(146.0f/255.0f, 146.0f / 255.0f, 1, 1);
                }
                else
                {
                    temp.color = Color.white;
                }
            }
        }
    }
}
