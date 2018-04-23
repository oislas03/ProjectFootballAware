using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{

    public static GameControl instance = null;
    public GameObject ball;
    public Text player1Name;
    public Text player2Name;
    public Text player1score;
    public Text player2score;
    public int maxScore;
    public int score = 0;
    public Text winnerText;
    public Text loserText;
    public Text goalText;
    public string myPlayer;

    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }



        //  player1Name.text = ControlDatabase.cb.myMatch.player1;
        //  player2Name.text = ControlDatabase.cb.myMatch.player2;
        player1Name.text = ControlDatabase.cb.myMatch.player1;
        player2Name.text = ControlDatabase.cb.myMatch.player2;

        myPlayer = GameObject.FindGameObjectWithTag("ControlDataBase").GetComponent<ControlDatabase>().myUsername;

        if (myPlayer.Equals(player1Name.text.ToString()))
        {
            Instantiate(ball, new Vector2(), Quaternion.identity);

        }


        winnerText.gameObject.SetActive(false);
        loserText.gameObject.SetActive(false);
        goalText.gameObject.SetActive(false);
    }


    public void Leave()
    {
       ControlDatabase.cb.leave();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Int32.Parse(player1score.text) == maxScore))
        {
            if (myPlayer.Equals(ControlDatabase.cb.myMatch.player1.ToString()))
             {
                Winner();
            }
            else
            {
                Loser();
            }
        }
        else if ((Int32.Parse(player2score.text) == maxScore))
        {
            if (myPlayer.Equals(ControlDatabase.cb.myMatch.player2.ToString()))
            {
                Winner();
            }
            else
            {
                Loser();
            }

        }

    }

    public void Goal()
    {
        goalText.gameObject.SetActive(true);

        if (myPlayer.Equals(player1Name.text.ToString()))
        {
            ControlDatabase.cb.UpdateScore(2);
            player2score.text = ControlDatabase.cb.myMatch.player2Score + "";
        }
        else if (myPlayer.Equals(player2Name.text.ToString()))
        {
            ControlDatabase.cb.UpdateScore(1);
            player1score.text = ControlDatabase.cb.myMatch.player1Score + "";

        }
        Invoke("RestartGameWithBall", 3.0f);
    }


    public void MyGoal()
    {

        goalText.gameObject.SetActive(true);
        goalText.text = "GOAL!";

            player1score.text = ControlDatabase.cb.myMatch.player1Score + "";
      
            player2score.text = ControlDatabase.cb.myMatch.player2Score + "";

        
        Invoke("RestartGameWithoutBall", 3.0f);


    }

    private void Winner()
    {
        winnerText.gameObject.SetActive(true);
        Invoke("RestartGame", 3.0f);
    }

    private void Loser()
    {
        loserText.gameObject.SetActive(true);
        Invoke("RestartGame", 3.0f);
    }

    public void RestartGame() {
        ControlDatabase.cb.leave();

    }

    private void RestartGameWithBall()
    {
        goalText.gameObject.SetActive(false);
        Instantiate(ball, new Vector2(), Quaternion.identity);
    }

    private void RestartGameWithoutBall()
    {
        goalText.gameObject.SetActive(false);
    }

    public void InstantiateBall(float vectorx, float vectory, float directionX, float directionY, float force)
    {
        Instantiate(ball, new Vector2(-vectorx, 4f), Quaternion.identity);
        ball.GetComponent<Swipe>().ApplyForceToRigidbody(directionX, directionY, force);
    }
}


public class Match
{
    public string player1;
    public string player2;
    public string isball;
    public string id;
    public string winner;
    public int player1Score;
    public int player2Score;
    public float vectorX;
    public float vectorY;
    public float force;
    public float directionX;
    public float directionY;




    public Match(string player1, string player2, string id)
    {
        this.id = id;
        this.player1 = player1;
        this.player2 = player2;
        this.player1Score = 0;
        this.player2Score = 0;
        this.vectorX = 0;
        this.vectorY = 0;
        this.force = 0;
        this.directionX = 0;
        this.directionY = 0;
        this.isball = "none";
    }
}