using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        string myUsername =  ControlDatabase.cb.myUsername;

        if (!myUsername.Equals(""))
        {
            GameObject.Find("username").GetComponent<Text>().text = myUsername;
            GameObject.Find("InputField").GetComponent<InputField>().text= myUsername; 

        }
        else {

            GameObject.Find("username").GetComponent<Text>().text = "No username";
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Connect() {

        ControlDatabase.cb.writeNewUser();
    }

    public void Disconnect() {
        ControlDatabase.cb.DeleteUser();

    }
}
