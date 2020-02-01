using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PressTransi : MonoBehaviour {

    [SerializeField] public RawImage RefUIBlack;


    IEnumerator PlayTransition()
    {
        RefUIBlack.gameObject.SetActive(true);
        float time = 0.0f;

        while (RefUIBlack.color.a != 1)
        {
            time += Time.deltaTime;
            Color temp = RefUIBlack.color;
            temp.a = Mathf.Lerp(0, 1, time);
            RefUIBlack.color = temp;
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene("Presentation");
    }

    // Update is called once per frame
    void Update () {

        if(Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            StartCoroutine(PlayTransition());
        }
		
	}
}
