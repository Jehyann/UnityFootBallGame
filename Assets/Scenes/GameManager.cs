using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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


    private float scorePlayer1;
    private float scorePlayer2;

    public float countdownTime = 300f;
    private float currentTime;
    private bool timerPaused = false;


    // Start is called before the first frame update
    void Start()
    {
        scoreText1.text = "0";
        scoreText2.text = "0";
        scorePlayer1 = 0;
        scorePlayer2 = 0;
        ball.GetComponent<Rigidbody>().isKinematic = true;
        currentTime = countdownTime;
        Init();
    }

    public void Init()
    {
        timerPaused = false;
        ball.position = ballStartPos.position;
        player1.position = player1StartPos.position;
        player1.rotation = player1StartPos.rotation;
        player2.position = player2StartPos.position;
        player2.rotation = player2StartPos.rotation;
        ball.GetComponent<Ball>().Init();

        player1.GetComponent<Player1Controller>().FillEnergy();
        player2.GetComponent<Player2Controller>().FillEnergy();

        StartCoroutine(BeginTimer());
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
            Debug.Log("Vgetto");
            timerPaused = true;
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

    public void UpdateTimerText()
    {
        // Update the UI text to display the remaining time
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }
    }
    private IEnumerator BeginTimer()
    {
        yield return WaitForSecondsAndUpdateText(1, "2");
        yield return WaitForSecondsAndUpdateText(1, "1");
        yield return WaitForSecondsAndUpdateText(1, "GO!");
        yield return new WaitForSeconds(1.0f);

        beginTimer.gameObject.SetActive(false);
    }
    private IEnumerator RelaunchGame()
    {
        currentTime = countdownTime;

        yield return new WaitForSeconds(5.0f);

        infoText.gameObject.SetActive(false);
        beginTimer.text = "3";
        beginTimer.gameObject.SetActive(true);

        Start();
    }
    private IEnumerator WaitForSecondsAndUpdateText(float seconds, string newText)
    {
        yield return new WaitForSeconds(seconds);
        beginTimer.text = newText;
    }
}