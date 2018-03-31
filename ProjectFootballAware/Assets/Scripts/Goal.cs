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
            Invoke("AddScore", 3.0f);
            Destroy(gameObject, 3.0f);
        }
    }

    private void AddScore()
    {
        GameControl.instance.UpdateScores();
    }
}