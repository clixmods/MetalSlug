using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;



public partial class HighscoreTable : MonoBehaviour
{
    [SerializeField] Alphabet alphabet;
    [SerializeField] private GameObject keyboard;
    [SerializeField] private Transform entryContainer;
    [SerializeField] private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;
    [Header("Debug")]
    [SerializeField] private bool Send;
    [SerializeField] private int score;
    [SerializeField] private string nameEntry;

    private void Awake()
    {
        entryTemplate.gameObject.SetActive(false);
        alphabet = keyboard.GetComponent<Alphabet>();
        string jsonString;
        Highscores highscores;
        GetHighScore(out jsonString, out highscores);

        if (highscores == null)
        {
            // There's no stored table, initialize
            AddHighscoreEntry(1000, "BOT");
            AddHighscoreEntry(2000, "BOT");
            AddHighscoreEntry(3000, "BOT");
            AddHighscoreEntry(4000, "BOT");
            AddHighscoreEntry(5000, "BOT");
            AddHighscoreEntry(6000, "BOT");
            AddHighscoreEntry(7000, "BOT");
            AddHighscoreEntry(8000, "BOT");
            AddHighscoreEntry(9000, "BOT");
            AddHighscoreEntry(10000, "BOT");
            // Reload
            jsonString = PlayerPrefs.GetString("highScoreMetalSlugLeaderBoard");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        SortHighscore(highscores);

        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighScoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
        LevelManager.eventEndgame += LevelManager_eventEndgame;
    }

    private void LevelManager_eventEndgame()
    {
        gameObject.SetActive(true);
    }

    private static void GetHighScore(out string jsonString, out Highscores highscores)
    {
        // keaboard assign
        jsonString = PlayerPrefs.GetString("highScoreMetalSlugLeaderBoard");
        highscores = JsonUtility.FromJson<Highscores>(jsonString);
    }

    private void Update()
    {
        if (Send)
        {
            RegisterScore(score);
            Send = false;
        }
    }

    UIHighscoreEntry uiHighScoreEntry;
    public void RegisterScore(int scoreValue)
    {
        GetHighScore(out var jsonString, out var highscores);
        UpdateHighscore();
        SortHighscore(highscores);
        
        int currentPosition = 0;
        for (int i = 0; i < highscores.highscoreEntryList.Count - 1; i++)
        {
            var entry = highscores.highscoreEntryList[i];
            if (scoreValue > entry.score)
            {
                currentPosition = i + 1;
                break;
            }
        }
        // Not in highscore
        if (currentPosition == 0 || currentPosition == 11)
        {
            LevelManager.ResetSession();
            return;
        }

        UpdateHighscore(currentPosition - 1);

        keyboard.gameObject.SetActive(true);

        score = scoreValue;
        alphabet.action += OnRegister;
        var entryTransform = highscoreEntryTransformList[currentPosition - 1];
        uiHighScoreEntry = entryTransform.GetComponent<UIHighscoreEntry>();
        alphabet.text = entryTransform.Find("nameText").GetComponent<Text>();
        
        uiHighScoreEntry.SetRank(currentPosition.ToString()); 
        uiHighScoreEntry.SetScore(score.ToString());
        uiHighScoreEntry.SetHightlight(true);
        alphabet.action += OnAnim;
    }
    private void OnRegister(string value)
    {
        keyboard.gameObject.SetActive(false);
        AddHighscoreEntry(score, value);
        UpdateHighscore();
        alphabet.action -= OnRegister;
        LevelManager.ResetSession();
    }
    private void OnAnim(string value)
    {
        uiHighScoreEntry.SetHightlight(false);
        alphabet.action -= OnAnim;
        uiHighScoreEntry = null;
    }

    private void CreateHighscoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);
        var highscoreEntryComponent = entryTransform.GetComponent<UIHighscoreEntry>();

        int rank = transformList.Count + 1;
        
        SetValueOnEntry(rank, highscoreEntryComponent, highScoreEntry, entryTransform);

        transformList.Add(entryTransform);
    }

    private void UpdateHighscore(int untilIndex = 10)
    {
        float templateHeight = 31f;
        string jsonString = PlayerPrefs.GetString("highScoreMetalSlugLeaderBoard");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        SortHighscore(highscores);

        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }
        // Clear each line
        for (int i = 0; i < highscoreEntryTransformList.Count; i++)
        {
            var entryTransform = highscoreEntryTransformList[i];
            var highscoreEntryComponent = entryTransform.GetComponent<UIHighscoreEntry>();
            highscoreEntryComponent.SetRank("");
            highscoreEntryComponent.SetScore("");
            highscoreEntryComponent.SetName("");
        }

        for (int i = 0; i < untilIndex; i++)
        {
            var entryTransform = highscoreEntryTransformList[i];
            var highscoreEntry = highscores.highscoreEntryList[i];
            var highscoreEntryComponent = entryTransform.GetComponent<UIHighscoreEntry>();
            
            RectTransform entryRectTransform = highscoreEntryComponent.rectTransform;
            
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);
            int rank = i + 1;
            
            SetValueOnEntry(rank, highscoreEntryComponent, highscoreEntry, entryTransform);
        }
    }

    private static void SetValueOnEntry(int rank, UIHighscoreEntry highscoreEntryComponent, HighScoreEntry highScoreEntry,
        Transform entryTransform)
    {
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH";
                break;

            case 1:
                rankString = "1ST";
                break;
            case 2:
                rankString = "2ND";
                break;
            case 3:
                rankString = "3RD";
                break;
        }

        highscoreEntryComponent.SetRank(rankString);
        int score = highScoreEntry.score;
        highscoreEntryComponent.SetScore(score.ToString());
        string name = highScoreEntry.name;
        highscoreEntryComponent.SetName(name);
        // Set background visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);
        // Set tropy
        switch (rank)
        {
            default:
                highscoreEntryComponent.SetColorTrophy(Color.clear);
                break;
            case 1:
                highscoreEntryComponent.SetColorTrophy(UtilsClass.GetColorFromString("FFD200"));
                highscoreEntryComponent.SetRank(rankString, Color.green);
                highscoreEntryComponent.SetScore(score.ToString(), Color.green);
                highscoreEntryComponent.SetName(name, Color.green);
                break;
            case 2:
                highscoreEntryComponent.SetColorTrophy(UtilsClass.GetColorFromString("C6C6C6"));
                break;
            case 3:
                highscoreEntryComponent.SetColorTrophy(UtilsClass.GetColorFromString("B76F56"));
                break;
        }
    }

    private static void SortHighscore(Highscores highscores)
    {
        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // Swap
                    HighScoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }
    }

    public void AddHighscoreEntry(int score, string name)
    {
        // Create HighscoreEntry
        HighScoreEntry highScoreEntry = new HighScoreEntry { score = score, name = name };

        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highScoreMetalSlugLeaderBoard");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // There's no stored table, initialize
            highscores = new Highscores()
            {
                highscoreEntryList = new List<HighScoreEntry>()
            };
        }

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highScoreEntry);

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);

        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        PlayerPrefs.SetString("highScoreMetalSlugLeaderBoard", json);
        PlayerPrefs.Save();
    }

    /*
     * Represents a single High score entry
     * */
    /*IEnumerator LerpFunction(Color endValue, float duration, Text textToFade)
    {
        float time = 0;
        Color startValue = textToFade.color;
        while (time < duration)
        {
            textToFade.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        textToFade.color = endValue;
    }*/
}

