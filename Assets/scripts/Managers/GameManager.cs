using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RamailoGames;

public class GameManager : MonoBehaviour
{
    public CharacterController character;
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        
    }

    #endregion

    [Header("UIS")]
    #region Tmp Text

    public TMP_Text GameOverScoreText;
    public TMP_Text GameOverhighscoreText;
    public TMP_Text gamePlayScoreText;
    public TMP_Text gamePlayhighscoreText;
    public Transform clockUiTransform;
    #endregion

    [Header("SpawnPos")]
    #region Transforms
    public Transform spawnPosLeft;
    public Transform spawnPosRight;
    public Transform spawnPosTop;
    public Transform deletePosBottom;
    public Transform clockminPos;
    public Transform clockMaxpos;
    public Transform bgspawnPosLeft;
    public Transform bgspawnPosRight;
    public Transform bgspawnPosTop;
    public Transform bgdeletePosBottom;
    public GameObject handImg;

    #endregion

    #region Prefabs
    [SerializeField] GameObject PlatformPrefab;
    [SerializeField] GameObject cloudPrefab;
    [SerializeField] GameObject bgPrefab;
    #endregion

    #region List of objects
    [SerializeField] GameTheme[] gameThemes;
    #endregion

    #region Private Serialized Fields
    [SerializeField] int score;
    [SerializeField] float maxRemainingTime;
    [SerializeField] float remainingTime;

    #endregion

    #region Private Fields

    bool paused;
    float startTime;
    bool gameStarted;


    #endregion

    #region Public Fields
    public ThemeType activeTheme;
    #endregion

    #region MonoBehaviour Functions

    void Start()
    {
        score = 0;
        ScoreAPI.GameStart((bool s) => {
        });
        int themeIndex = Random.Range(0, gameThemes.Length);
        activeTheme = gameThemes[themeIndex].themeType;
        //activeTheme = ThemeType.snow;
        float startY = -3;
        for (int i = 0; i < 16; i++)
        {
            float offset;
            if (i % 2 == 0)
            {
                offset = 0;
            }
            else
                offset = 0.5f;
            SpawnPlatformSeries(new(offset,startY+0.75f*i));
        }
        for (int i = -3; i <= 3; i++)
        {
            GameObject cloud = Instantiate(bgPrefab, new(5*i,0), Quaternion.identity);
            cloud.GetComponent<SpriteRenderer>().sprite = GetCurrentTheme().bg;
        }
        for (int i = -3; i <= 3; i++)
        {
            GameObject cloud = Instantiate(cloudPrefab, new(4 * i, 5.88f), Quaternion.identity);
            cloud.GetComponent<SpriteRenderer>().sprite = GetCurrentTheme().cloud;
        }
        for (int i = -3; i <= 3; i++)
        {
            GameObject cloud = Instantiate(cloudPrefab, new(4 * i, 5.88f+4), Quaternion.identity);
            cloud.GetComponent<SpriteRenderer>().sprite = GetCurrentTheme().cloud;
        }
        for (int i = -3; i <= 3; i++)
        {
            GameObject cloud = Instantiate(cloudPrefab, new(4 * i, 5.88f + 8), Quaternion.identity);
            cloud.GetComponent<SpriteRenderer>().sprite = GetCurrentTheme().cloud;
        }
    }
    public void StartGame()
    {
        gameStarted = true;
        remainingTime = maxRemainingTime;

    }
    public void spawnBgCloud(Vector2 pos)
    {
        
        GameObject cloud = Instantiate(cloudPrefab,pos,Quaternion.identity);
        cloud.GetComponent<SpriteRenderer>().sprite = GetCurrentTheme().cloud;
    }
    private void SpawnPlatformSeries(Vector2 pos)
    {
        for (int i = -6; i <= 5; i++)
        {
            GameObject plat = Instantiate(PlatformPrefab, new(pos.x+i,pos.y), Quaternion.identity);
            plat.GetComponent<PlatformController>().spawnPowerUps = false;
        }
    }

    void Update()
    {
        if (!gameStarted)
            return;
        remainingTime -= Time.deltaTime;
        clockUiTransform.position = new(clockUiTransform.position.x-(clockMaxpos.position.x-clockminPos.position.x)/maxRemainingTime *Time.deltaTime, clockUiTransform.transform.position.y,0);
        if (remainingTime <= 0)
            GameOver();
    }

    #endregion

    #region Public Functions
    public void SpawnPlatform(Vector2 position)
    {
        GameObject plat = Instantiate(PlatformPrefab, position, Quaternion.identity);
        plat.GetComponent<PlatformController>().spawnPowerUps = true;
    }

    public GameTheme GetCurrentTheme()
    {
        foreach (GameTheme theme in gameThemes)
        {
            if (theme.themeType == activeTheme)
                return theme;
        }
        Debug.LogWarning("something wrong");
        return null;
    }
    public GameTheme GetOtherTheme()
    {
        return gameThemes[Random.Range(0, gameThemes.Length)];
    }
    public void PauseGame()
    {
        
        Time.timeScale = 0;
        paused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        paused = false;
    }

    public void AddScore(int amount)
    {
        score += amount ;
        gamePlayScoreText.text = score.ToString();
        setHighScore(gamePlayhighscoreText);
        character.SetPos();

    }

    #endregion

    #region Private Functions

    public void AddTime(int amount)
    {
        remainingTime += amount;
        if (remainingTime > maxRemainingTime)
        {
            remainingTime = maxRemainingTime;
            clockUiTransform.position = clockMaxpos.position;
            return;
        }
        clockUiTransform.position = new(clockUiTransform.position.x + (clockMaxpos.position.x - clockminPos.position.x) / maxRemainingTime * amount, clockUiTransform.transform.position.y,0); ;

    }
    public void GameOver()
    {
        PauseGame();
        UIManager.instance.SwitchCanvas(UIPanelType.GameOver);
        UIManager.instance.SwitchCanvas(UIPanelType.GameOver);
        GameOverScoreText.text = score.ToString();
        int playTime = (int)(Time.unscaledTime - startTime);
        ScoreAPI.SubmitScore(score, playTime, (bool s, string msg) => { });
        GetHighScore();
        //Debug.Log("Game Over");
    }

    

    void setHighScore(TMP_Text highscroreTextUI)
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                if (score >= d.high_score)
                {
                    highscroreTextUI.text = score.ToString();

                }
                else
                {
                    highscroreTextUI.text = d.high_score.ToString();
                }

            }
        });
    }
    


    void GetHighScore()
    {
        ScoreAPI.GetData((bool s, Data_RequestData d) => {
            if (s)
            {
                if (score >= d.high_score)
                {
                    GameOverhighscoreText.text = score.ToString();

                }
                else
                {
                    GameOverhighscoreText.text =d.high_score.ToString();
                }

            }
        });

    }

    #endregion

    #region Coroutines

    #endregion
}
