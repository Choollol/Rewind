using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    public static bool isGameActive { get; private set; }

    public static int level {  get; private set; }
    [SerializeField] private int startingLevel;

    public static bool isInSceneTransit { get; private set; }
    public static bool isGameCompleted { get; private set; }

    public static bool isTimeFrozen = false;

    public static bool canRewind = true;


    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelTitle;
    [SerializeField] private List<string> levelTitles;

    private void OnEnable()
    {
        EventMessenger.StartListening("LevelComplete", NextLevel);
        EventMessenger.StartListening("FreezeTime", FreezeTime);
        EventMessenger.StartListening("UnfreezeTime", UnfreezeTime);
        EventMessenger.StartListening("Restart", Restart);
        EventMessenger.StartListening("TransitionEnded", TransitionEnded);
    }
    private void OnDisable()
    {
        EventMessenger.StopListening("LevelComplete", NextLevel);
        EventMessenger.StopListening("FreezeTime", FreezeTime);
        EventMessenger.StopListening("UnfreezeTime", UnfreezeTime);
        EventMessenger.StopListening("Restart", Restart);
        EventMessenger.StopListening("TransitionEnded", TransitionEnded);
    }

    void Start()
    {
        isGameActive = true;

        //level = int.Parse(SceneManager.GetSceneAt(1).name.Split(" ")[1]);
        level = startingLevel;
        LoadLevel();
    }
    private void Update()
    {
        if (!isGameCompleted && isGameActive) {
            if (Input.GetButtonDown("Restart"))
            {
                Restart();
            }
            else if (Input.GetButtonDown("Skip"))
            {
                EventMessenger.TriggerEvent("LevelComplete");
            }
            else if (Input.GetButtonDown("Back"))
            {
                StartCoroutine(TransitionLevel(level - 1));
            }
            else if (Input.GetButtonDown("Hint"))
            {
                levelTitle.gameObject.SetActive(true);
            }
        }
    }
    private void Restart()
    {
        StartCoroutine(HandleRestart());
    }
    private IEnumerator HandleRestart()
    {
        canRewind = false;
        AsyncOperation load = LoadLevel();
        yield return load;
        SceneManager.UnloadSceneAsync("Level " + level);
        yield break;
    }
    private void FreezeTime()
    {
        isGameActive = false;
        //Time.timeScale = 0;

    }
    private void UnfreezeTime()
    {
        Time.timeScale = 1;
        isGameActive = true;
    }
    private void NextLevel()
    {
        StartCoroutine(TransitionLevel(level + 1));
    }
    private IEnumerator TransitionLevel(int newLevel)
    {
        isInSceneTransit = true;
        isGameActive = false;
        EventMessenger.TriggerEvent("StartTransition");
        yield return new WaitForSeconds(2);
        if (isGameCompleted)
        {
            SceneManager.UnloadSceneAsync("End_Scene");
        }
        else
        {
            SceneManager.UnloadSceneAsync("Level " + level);
        }
        level = newLevel;
        LoadLevel();
        yield return new WaitForSeconds(0.5f);
        EventMessenger.TriggerEvent("EndTransition");
        levelTitle.gameObject.SetActive(false);
        yield break;
    }
    private void TransitionEnded()
    {
        UnfreezeTime();
        isInSceneTransit = false;
    }
    private AsyncOperation LoadLevel()
    {
        AsyncOperation load;
        if (SceneUtility.GetBuildIndexByScenePath("Level " + level) != -1)
        {
            load = SceneManager.LoadSceneAsync("Level " + level, LoadSceneMode.Additive);
            UpdateLevelText();
        }
        else
        {
           load = SceneManager.LoadSceneAsync("End Scene", LoadSceneMode.Additive);
            isGameCompleted = true;
            levelText.gameObject.SetActive(false);
        }
        canRewind = true;
        return load;
    }
    private void UpdateLevelText()
    {
        levelText.text = level.ToString();
        levelTitle.text = "\"" + levelTitles[level - 1] + "\"";
    }
}
