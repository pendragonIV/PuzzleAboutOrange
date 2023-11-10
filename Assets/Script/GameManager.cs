using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public SceneChanger sceneChanger;
    public GameScene gameScene;
    Level currentLevelData;
    #region Game status
    [SerializeField]
    private bool isGameWin = false;
    [SerializeField]
    private bool isLose = false;
    [SerializeField]
    private float timeLeft = 0;
    [SerializeField]
    public int achivement = 0;
    private const int MAX_ACHIVE = 3;
    #endregion

    private void Start()
    {
        currentLevelData = LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex);
        Time.timeScale = 1;
        timeLeft = currentLevelData.time;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        gameScene.SetTime(timeLeft);    
        if (timeLeft <= 0 && !isGameWin && !isLose)
        {
            Lose();
        }
    }

    public void Win()
    {
        if (LevelManager.instance.levelData.GetLevels().Count > LevelManager.instance.currentLevelIndex + 1)
        {
            if (LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex + 1).isPlayable == false)
            {
                LevelManager.instance.levelData.SetLevelData(LevelManager.instance.currentLevelIndex + 1, true, false, 0);
            }
        }
        SetAchivement();
        if (achivement > LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex).achivement)
        {
            LevelManager.instance.levelData.SetLevelData(LevelManager.instance.currentLevelIndex, true, true, achivement);
        }
        else
        {
            LevelManager.instance.levelData.SetLevelData(LevelManager.instance.currentLevelIndex, true, true, LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex).achivement);
        }
        isGameWin = true;

        gameScene.ShowWinPanel();
        Time.timeScale = 0;
        LevelManager.instance.levelData.SaveDataJSON();
    }

    private void SetAchivement()
    {
        achivement = (int)((timeLeft/currentLevelData.time) * MAX_ACHIVE) + 1;
    }

    public void Lose()
    {
        isLose = true;
        gameScene.ShowLosePanel();
        Time.timeScale = 0;
    }

    public bool IsGameWin()
    {
        return isGameWin;
    }

    public bool isGameLose()
    {
        return isLose;
    }
}

