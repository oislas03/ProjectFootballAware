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

        string myPlayer = GameObject.FindGameObjectWithTag("ControlDataBase").GetComponent<ControlDatabase>().myUsername;

        if (myPlayer.Equals(player1Name.text.ToString()))
        {            Instantiate(ball, new Vector2(), Quaternion.identity);

        }


        winnerText.gameObject.SetActive(false);
        loserText.gameObject.SetActive(false);
        goalText.gameObject.SetActive(false);
    }


    public void Leave()
    {
        GameObject.FindGameObjectWithTag("ControlDataBase").GetComponent<ControlDatabase>().leave();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Int32.Parse(player1score.text) == maxScore))
        {
            Winner();
        }
        else if ((Int32.Parse(player2score.text) == maxScore))
        {
            Loser();
        }
    }

    public void Goal()
    {
        goalText.gameObject.SetActive(true);
    }

    public void UpdateScores()
    {
        int p1score = Int32.Parse(player1score.text);
        int p2score = Int32.Parse(player2score.text);
        p1score++;
        p2score++;
        player1score.text = p1score.ToString();
        player2score.text = p2score.ToString();
        goalText.gameObject.SetActive(false);
        Instantiate(ball, new Vector2(), Quaternion.identity);
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

    private void RestartGame()
    {
        SceneManager.LoadScene("Football Field");
    }

    public void InstantiateBall(float vectorx, float vectory, float directionX, float directionY, float force)
    {
        Instantiate(ball, new Vector2(-vectorx, 4f), Quaternion.identity);
        ball.GetComponent<Swipe>().ApplyForceToRigidbody(vectorx, vectory, force);
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