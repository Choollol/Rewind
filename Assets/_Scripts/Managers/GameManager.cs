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

    public static bool isInSceneTransit { get; private set; }
    public static bool isGameCompleted { get; private set; }

    public static bool isTimeFrozen = false;

    public static bool canRewind = true;

    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable()
    {
        EventMessenger.StartListening("LevelComplete", NextLevel);
        EventMessenger.StartListening("FreezeTime", FreezeTime);
        EventMessenger.StartListening("UnfreezeTime", UnfreezeTime);
        EventMessenger.StartListening("Restart", Restart);
        EventMessenger.StartListening("TransitionEnded", EndTransition);
    }
    private void OnDisable()
    {
        EventMessenger.StopListening("LevelComplete", NextLevel);
        EventMessenger.StopListening("FreezeTime", FreezeTime);
        EventMessenger.StopListening("UnfreezeTime", UnfreezeTime);
        EventMessenger.StopListening("Restart", Restart);
        EventMessenger.StopListening("TransitionEnded", EndTransition);
    }

    void Start()
    {
        isGameActive = true;

        level = int.Parse(SceneManager.GetSceneAt(1).name.Split(" ")[1]);
    }
    private void Update()
    {
        if (Input.GetButtonDown("Restart") && !isGameCompleted && isGameActive)
        {
            Restart();
        }
    }
    private void Restart()
    {
        FreezeTime();
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
        yield break;
    }
    private void EndTransition()
    {
        isGameActive = true;
        isInSceneTransit = false;
    }
    private void LoadLevel()
    {
        if (SceneUtility.GetBuildIndexByScenePath("Level " + level) != -1)
        {
            SceneManager.LoadSceneAsync("Level " + level, LoadSceneMode.Additive);
            UpdateLevelText();
        }
        else
        {
            SceneManager.LoadSceneAsync("End Scene", LoadSceneMode.Additive);
            isGameCompleted = true;
            levelText.gameObject.SetActive(false);
        }
        UnfreezeTime();
    }
    private void UpdateLevelText()
    {
        levelText.text = level.ToString();
    }
}
