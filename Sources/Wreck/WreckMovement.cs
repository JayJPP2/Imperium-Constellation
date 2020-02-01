using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckMovement : MonoBehaviour
{
	public void InitPart(float speed)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Rigidbody>().velocity = speed * transform.GetChild(i).forward;
        }
    }
}
