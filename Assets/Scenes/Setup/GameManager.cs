using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager GetInstance()
    {
        if (instance)
            return instance;
        else
            return instance = FindObjectOfType<GameManager>();
    }

    [SerializeField] private Transform ball;
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [SerializeField] private Transform ballStartPos;
    [SerializeField] private Transform player1StartPos;
    [SerializeField] private Transform player2StartPos;

    [SerializeField] private TextMeshProUGUI scoreText1;
    [SerializeField] private TextMeshProUGUI scoreText2;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI beginTimer;

    [SerializeField] private Generator generator;
    [SerializeField] private Transform drone;


    private float scorePlayer1;
    private float scorePlayer2;

    public float countdownTime = 180f;
    string formattedTime;


    private float currentTime;
    private bool timerPaused = true;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        scoreText1.text = "0";
        scoreText2.text = "0";
        timerText.text = FormatTime(countdownTime);
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        currentTime = countdownTime;
        drone.GetComponent<Drone>().Reset();
        generator.GeneratePoisson(generator.obstaclePrefab, 15, 50);
        generator.GeneratePoisson(generator.grass, 1, 50);
        Init();
    }

    public void Init()
    {
        ball.position = ballStartPos.position;
        player1.position = player1StartPos.position;
        player1.rotation = player1StartPos.rotation;
        player2.position = player2StartPos.position;
        player2.rotation = player2StartPos.rotation;
        ball.GetComponent<Ball>().Init();
        
        player1.GetComponent<Player1Controller>().CantMove();
        player2.GetComponent<Player2Controller>().CantMove();

        player1.GetComponent<Player1Controller>().FillEnergy();
        player2.GetComponent<Player2Controller>().FillEnergy();

        StartCoroutine(BeginTimer());

        timerText.text = FormatTime(countdownTime);
    }

    public void Update()
    {
        if (!timerPaused)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerText();
        }

        if (currentTime <= 0)
        {
            Debug.Log("Vegetto");
            timerPaused = true;
            ball.GetComponent <Rigidbody>().isKinematic = true;
            if (scorePlayer1 == scorePlayer2)
            {
                infoText.text = "ÉGALITÉ";
            }
            if (scorePlayer2 < scorePlayer1)
            {
                infoText.text = "VICTOIRE JOUEUR BLEU";
            }
            if (scorePlayer2 > scorePlayer1)
            {
                infoText.text = "VICTOIRE JOUEUR ROUGE";
            }
            infoText.gameObject.SetActive(true);
            StartCoroutine(RelaunchGame());
        }
    }

    public void ScorePoint(int playerId)
    {
        timerPaused = true;
        if (playerId == 1)
        {
            scorePlayer1++;
            scoreText1.text = scorePlayer1.ToString();
        }
        else
        {
            scorePlayer2++;
            scoreText2.text = scorePlayer2.ToString();
        }

        Debug.Log("Player" + playerId + " score point");
    }
    public string FormatTime(float totalTime)
    {
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);

        string formattedTime = minutes + ":" + seconds.ToString("00");

        return formattedTime;
    }

    public void UpdateTimerText()
    {
        // Update the UI text to display the remaining time
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            string timeString = string.Format("{0:0}:{1:00}", minutes, seconds);
            timerText.text = timeString;
        }
    }

    private IEnumerator BeginTimer()
    {
        beginTimer.gameObject.SetActive(true);
        yield return WaitForSecondsAndUpdateText(1, "2");
        yield return WaitForSecondsAndUpdateText(1, "1");
        yield return WaitForSecondsAndUpdateText(1, "GO!");
        player1.GetComponent<Player1Controller>().CanMove();
        player2.GetComponent<Player2Controller>().CanMove();

        yield return new WaitForSeconds(1.0f);
        beginTimer.text = "3";
        timerPaused = false;
        beginTimer.gameObject.SetActive(false);
    }
    private IEnumerator RelaunchGame()
    {
        currentTime = countdownTime;
        Debug.Log(currentTime);
        yield return new WaitForSeconds(5.0f);

        infoText.gameObject.SetActive(false);
        beginTimer.gameObject.SetActive(true);

        generator.ClearObstacles();
        Start();
    }
    private IEnumerator WaitForSecondsAndUpdateText(float seconds, string newText)
    {
        yield return new WaitForSeconds(seconds);
        beginTimer.text = newText;
    }
}