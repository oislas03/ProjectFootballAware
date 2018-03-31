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

        winnerText.gameObject.SetActive(false);
        loserText.gameObject.SetActive(false);
        goalText.gameObject.SetActive(false);
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
}
