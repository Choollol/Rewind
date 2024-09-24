using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public static bool isGameActive { get; private set; }

    public static int level {  get; private set; }
    private int levelCount = 3;
    [SerializeField] private int startingLevel;

    public static bool isInSceneTransit { get; private set; }
    public static bool isGameCompleted { get; private set; }

    public static bool isTimeFrozen = false;

    public static bool canRewind = true;

    private void OnEnable()
    {
        EventMessenger.StartListening("LevelComplete", NextLevel);
        EventMessenger.StartListening("FreezeTime", FreezeTime);
        EventMessenger.StartListening("UnfreezeTime", UnfreezeTime);
        EventMessenger.StartListening("Restart", Restart);
    }
    private void OnDisable()
    {
        EventMessenger.StopListening("LevelComplete", NextLevel);
        EventMessenger.StopListening("FreezeTime", FreezeTime);
        EventMessenger.StopListening("UnfreezeTime", UnfreezeTime);
        EventMessenger.StopListening("Restart", Restart);
    }

    void Start()
    {
        isGameActive = true;

        level = startingLevel;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Restart"))
        {
            Restart();
        }
    }
    private void Restart()
    {
        if (level >= levelCount)
        {
            return;
        }
        SceneManager.UnloadSceneAsync("Level " + level);
        LoadLevel();
    }
    private void FreezeTime()
    {
        isGameActive = false;
        Time.timeScale = 0;

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
        EventMessenger.TriggerEvent("StartTransition");
        isGameActive = false;
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
        yield return new WaitForSeconds(1f);
        isGameActive = true;
        isInSceneTransit = false;
        yield break;
    }
    private void LoadLevel()
    {
        if (SceneUtility.GetBuildIndexByScenePath("Level " + level) != -1)
        {
            SceneManager.LoadSceneAsync("Level " + level, LoadSceneMode.Additive);
            //levelNumberText.gameObject.SetActive(true);
            //UpdateLevelNumberText();
        }
        else
        {
            SceneManager.LoadSceneAsync("End Scene", LoadSceneMode.Additive);
            isGameCompleted = true;
            //levelNumberText.gameObject.SetActive(false);
        }
    }
    
}
