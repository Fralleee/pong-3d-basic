using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tasks
/*
    Increase ball max speed with game time
    Check rules (if player has scored 10 or time is up)
    Handle UI
    Handle where to send ball in start of round
*/


public class GameManager : MonoBehaviour
{

  public enum StateType
  {
    NEWROUND, // Display score, await inputs
    FREEZE,   // Countdown from 5 (first time), nextState = LIVE
    LIVE,     // WHEN A ROUND IS LIVE, nextState: ROUNDEND
    GAMEEND   // WHEN GAME HAS ENDED, await input and change scene or replay
  }

  private Transform ball;

  [SerializeField] private GameObject uiBackground;
  [SerializeField] private GameObject countdownLabelText;
  [SerializeField] private GameObject countdownText;
  [SerializeField] private GameObject player1ScoreText;
  [SerializeField] private GameObject player2ScoreText;

  public static GameManager instance = null;

  // Stuff that gets reset every new game
  [SerializeField] private StateType state;
  public int player1Score;
  public int player2Score;
  private int ballLevel; // this controls ball max speed
  private int roundNo = 1;
  private bool roundStarted = false;



  private bool multiplayer = false;
  private bool player1Ready = false;
  private bool player2Ready = false;
  private int startFreezeTime = 5;
  private int freezeTime = 3;
  private float roundStartTime;

  void Awake()
  {
    if (instance == null) instance = this;
    else if (instance != this) Destroy(gameObject);
    DontDestroyOnLoad(gameObject);
    ball = GameObject.FindGameObjectWithTag("Ball").transform;
    InitGame();
  }

  void InitGame()
  {
    uiBackground.SetActive(false);
    countdownLabelText.SetActive(false);
    countdownText.SetActive(false);
    player1Score = 0;
    state = StateType.NEWROUND;
    ballLevel = 1;
  }

  void Update()
  {
    switch (state)
    {
      case StateType.NEWROUND:
        NewRoundState();
        break;
      case StateType.FREEZE:
        FreezeState();
        break;
      case StateType.LIVE:
        LiveState();
        break;
      case StateType.GAMEEND:
        GameEndState();
        break;
      default:
        Debug.Log("ERROR: Unknown game state: " + state);
        break;
    }
  }

  void NewRoundState()
  {
    // maybe upgrade this to some kind of state handler which manages state requirements and nextstates
    // we need to reset playerReady on score
    if (CheckIfPlayersReady())
    {
      roundStartTime = Time.time + (roundNo == 1 ? startFreezeTime : freezeTime);
      roundStarted = false;
      state = StateType.FREEZE;
    }
  }

  void FreezeState()
  {
    // After input let's start the freeze timer that counts down
    // 5.. 4.. 3.. ready.. set.. GO!

    // Apply UI text to display the countdown
    if (Time.time > roundStartTime)
      state = StateType.LIVE;
    else
    {
      float secondsLeft = roundStartTime - Time.time;

      if (roundNo == 1)
        countdownLabelText.SetActive(true);

      countdownText.SetActive(true);
      uiBackground.SetActive(true);

      string text = secondsLeft > 3 ? ((int)secondsLeft + 1).ToString() : secondsLeft > 2 ? "READY" : secondsLeft > 1 ? "SET" : "GO";
      countdownText.GetComponent<Text>().text = text;
    }
  }

  void LiveState()
  {
    if (!roundStarted)
    {
      uiBackground.SetActive(false);
      countdownLabelText.SetActive(false);
      countdownText.SetActive(false);
      roundStarted = true;
      float xPower = Random.Range(2, 4);
      float zPower = Random.Range(10, 20);
      zPower = roundNo % 2 == 0 ? zPower * -1 : zPower;
      Rigidbody body = ball.GetComponent<Rigidbody>();
      body.AddForce(new Vector3(Random.Range(xPower, xPower), 0, zPower));
    }
    else
    {
      if (ball.position.z < -30)
        Score(ref player2Score, ref player2ScoreText);
      else if (ball.position.z > 30)
        Score(ref player1Score, ref player1ScoreText);
    }
  }

  void GameEndState()
  {
    // Display who won
    // Time
    // Game end reason (score or time or forfeit)
  }

  public void OnApplicationQuit()
  {
    instance = null;
  }

  bool CheckIfPlayersReady()
  {
    if (Input.GetButtonDown("Fire1")) // Change this to P1Start
    {
      if (multiplayer) player1Ready = !player1Ready;
      else
      {
        player1Ready = true;
        player2Ready = true;
      }
    }
    if (Input.GetButtonDown("Fire2")) // Change this to P2Start
      player2Ready = !player2Ready;

    return player1Ready && player2Ready;
  }

  public void Score(ref int score, ref GameObject scoreText)
  {
    // Add points to the winner
    roundNo += 1;
    score += 1;
    scoreText.GetComponent<Text>().text = score.ToString();

    // Reset stuff
    ball.transform.position = new Vector3(0, 0.5f, 0);
    ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    player1Ready = false;
    player2Ready = false;

  // Change state
  state = StateType.NEWROUND;
  }
}
