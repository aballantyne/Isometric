using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animator : MonoBehaviour
{
	public Animator anim;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)){
        	if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow)){
        		Debug.Log("Up diagonal");
        	}else if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow)){
        		Debug.Log("down diagonal");
        	}else {
        		Debug.Log("left / right");
        	}
        }
        if (Input.GetKey(KeyCode.UpArrow)){
        	Debug.Log("Up");
        }
        if (Input.GetKey(KeyCode.DownArrow)){
        	anim.SetBool("down", true);
        }else {
        	anim.SetBool("down", false);
        }
    }
}
