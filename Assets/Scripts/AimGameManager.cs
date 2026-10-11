using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// Quản lý điểm, thời gian, độ chính xác và màn hình kết thúc.
public class AimGameManager : MonoBehaviour
{
    public static AimGameManager Instance { get; private set; }

    [Header("Cài đặt")]
    public float gameDuration = 60f;

    [Header("UI (TextMeshPro)")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text accuracyText;
    public GameObject gameOverPanel;
    public TMP_Text finalResultText;

    public int Score { get; private set; }
    public int Shots { get; private set; }
    public int Hits { get; private set; }
    public bool IsPlaying { get; private set; }

    public event Action OnGameOver;

    float timeLeft;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Score = Shots = Hits = 0;
        timeLeft = gameDuration;
        IsPlaying = true;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateUI();
    }

    void Update()
    {
        if (IsPlaying)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                EndGame();
            }
            UpdateUI();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void RegisterShot(bool hit, int points = 0)
    {
        if (!IsPlaying) return;
        Shots++;
        if (hit)
        {
            Hits++;
            Score += points;
        }
    }

    void EndGame()
    {
        IsPlaying = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        OnGameOver?.Invoke();

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalResultText)
            finalResultText.text =
                $"HẾT GIỜ!\nĐiểm: {Score}\nBắn trúng: {Hits}/{Shots}\nĐộ chính xác: {Accuracy():0.0}%\n\nNhấn R để chơi lại";
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    float Accuracy() => Shots == 0 ? 0f : (float)Hits / Shots * 100f;

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Điểm: {Score}";
        if (timerText) timerText.text = $"Thời gian: {Mathf.CeilToInt(timeLeft)}";
        if (accuracyText) accuracyText.text = $"Chính xác: {Accuracy():0}%";
    }
}
