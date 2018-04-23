using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Goal Net"))
        {
            GameControl.instance.Goal();
            Destroy(gameObject);

        }
        else if (collision.gameObject.tag.Equals("Field Limit"))
        {
            GameObject.FindGameObjectWithTag("Ball").GetComponent<Swipe>().SendDataToPlayer();
            Destroy(GameObject.FindGameObjectWithTag("Ball"));
        }
    }
}