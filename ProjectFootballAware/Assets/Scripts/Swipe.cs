using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    Vector2 startPos;
    Vector2 endPos;
    Vector2 direction;
    float touchTimeStart;
    float touchTimeFinish;
    float timeInterval;
    Rigidbody2D rb;
    bool throwAllowed = true;

    [Range(0.5f, 1f)]
    public float throwForce = 0.3f;

    // Use this for initialization
    void Start()
    {

        //Debug.Log("Añadiendo dirección y fuerza");
        rb = GetComponent<Rigidbody2D>();
        //startPos = new Vector2(3f, 3f);
        //endPos = new Vector2(2f, 2f);
        //direction = startPos - endPos;
        //rb.isKinematic = false;
        //rb.AddForce(new Vector2(3f, 2f) / 0.030f);
        //Debug.Log("Terminando el añadir dirección y fuerza");
        //GameObject.Find("ControlFirebase").GetComponent<ControlFirebase>().sendMatch(GameControl.instance.myMatch);

    }

    // Update is called once per frame
    void Update()
    {
           if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchTimeStart = Time.time;
            startPos = Input.GetTouch(0).position;
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(startPos);

            //if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos))
            //{
            //    throwAllowed = true;
            //}
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && throwAllowed)
        {
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;
            endPos = Input.GetTouch(0).position;
            direction = startPos - endPos;

            rb.isKinematic = false;
            rb.AddForce(-direction / timeInterval * throwForce);

            //throwAllowed = false;
        }
        Debug.Log("Posición inicial: " + startPos);
        Debug.Log("Posición final: " + endPos);
        Debug.Log("Fuerza: " + direction);
    }

    public void ApplyForceToRigidbody(float directionX, float directionY, float force)
    {
        rb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.velocity = new Vector2(-directionX, -directionY);
    }

    public void SendDataToPlayer()
    {   
        ControlDatabase.cb.UpdateMatch(rb.position.x, rb.position.y, direction.x, direction.y, (timeInterval*throwForce));
    }
}
