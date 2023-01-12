using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class HighscoreTable : MonoBehaviour {
    [SerializeField] Alphabet alphabet;
    [SerializeField] private GameObject keyboard;
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highscoreEntryTransformList;
    [Header("Debug")]
    [SerializeField] private bool Send;
    [SerializeField] private int score;
    [SerializeField] private string nameEntry;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");
        entryTemplate.gameObject.SetActive(false);
        alphabet = keyboard.GetComponent<Alphabet>();
        string jsonString;
        Highscores highscores;
        GetHighscore(out jsonString, out highscores);

        if (highscores == null)
        {
            // There's no stored table, initialize
            AddHighscoreEntry(5000, "BOT");

            // Reload
            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private static void GetHighscore(out string jsonString, out Highscores highscores)
    {
        // keaboard assign

        jsonString = PlayerPrefs.GetString("highscoreTable");
        highscores = JsonUtility.FromJson<Highscores>(jsonString);
    }

    private void Update()
    {
        if(Send)
        {
            RegisterScore(score);
            Send = false;
        }
    }

    public void RegisterScore(int scoreValue)
    {
        GetHighscore(out var jsonString, out var highscores);
        UpdateHighscore();
        SortHighscore(highscores);

        int currentPosition = 0;
        for (int i = 0; i < highscores.highscoreEntryList.Count-1; i++)
        {
            var entry = highscores.highscoreEntryList[i];
            if (scoreValue > entry.score)
            {
                currentPosition = i+1;
                break;
            }
        }
        // Not in highscore
        if (currentPosition == 0 || currentPosition == 11)
        {
            return;
        }

        UpdateHighscore(currentPosition-1);

        keyboard.gameObject.SetActive(true);

        score = scoreValue;
        alphabet.action += OnRegister;
        var entryTransform = highscoreEntryTransformList[currentPosition-1];
        alphabet.text = entryTransform.Find("nameText").GetComponent<Text>();
        entryTransform.Find("posText").GetComponent<Text>().text = currentPosition.ToString();
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();
        //entryTransform.Find("nameText").GetComponent<Text>().text = "";
    }

    private void OnRegister(string value)
    {
        keyboard.gameObject.SetActive(false);
        AddHighscoreEntry(score, value);
        UpdateHighscore();
        alphabet.action -= OnRegister;
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList) {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank) {
        default:
            rankString = rank + "TH"; break;

        case 1: rankString = "1ST"; break;
        case 2: rankString = "2ND"; break;
        case 3: rankString = "3RD"; break;
        }

        entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;

        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;

        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Set background visible odds and evens, easier to read
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);
        
        // Highlight First
        if (rank == 1) {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        }

        // Set tropy
        switch (rank) {
        default:
            entryTransform.Find("trophy").gameObject.SetActive(false);
            break;
        case 1:
            entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("FFD200");
            break;
        case 2:
            entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("C6C6C6");
            break;
        case 3:
            entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("B76F56");
            break;
        }
        transformList.Add(entryTransform);
    }

    private void UpdateHighscore(int untilIndex = 10)
    {
        float templateHeight = 31f;
        string jsonString = PlayerPrefs.GetString("highscoreTable");
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
            entryTransform.Find("posText").GetComponent<Text>().text = "";
            entryTransform.Find("scoreText").GetComponent<Text>().text = "";
            entryTransform.Find("nameText").GetComponent<Text>().text = "";
        }

        for (int i = 0; i < untilIndex; i++)
        {
            var entryTransform = highscoreEntryTransformList[i];
            var highscoreEntry = highscores.highscoreEntryList[i];
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);

            int rank = i + 1;
            string rankString;
            switch (rank)
            {
                default:
                    rankString = rank + "TH"; break;

                case 1: rankString = "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
            }

            entryTransform.Find("posText").GetComponent<Text>().text = rankString;

            int score = highscoreEntry.score;

            entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

            string name = highscoreEntry.name;

            entryTransform.Find("nameText").GetComponent<Text>().text = name;

            // Set background visible odds and evens, easier to read
            entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

            // Highlight First
            if (rank == 1)
            {
                entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
                entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
                entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
            }

            // Set tropy
            switch (rank)
            {
                default:
                    entryTransform.Find("trophy").gameObject.SetActive(false);
                    break;
                case 1:
                    entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("FFD200");
                    break;
                case 2:
                    entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("C6C6C6");
                    break;
                case 3:
                    entryTransform.Find("trophy").GetComponent<Image>().color = UtilsClass.GetColorFromString("B76F56");
                    break;
            }
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
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }
    }

    public void AddHighscoreEntry(int score, string name) {

        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };
        
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null) {
            // There's no stored table, initialize
            highscores = new Highscores() {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);

        if (highscores.highscoreEntryList.Count > 10)
        {
            for (int h = highscores.highscoreEntryList.Count; h > 10; h--)
            {
                highscores.highscoreEntryList.RemoveAt(10);
            }
        }

        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores {
        public List<HighscoreEntry> highscoreEntryList;
    }

    /*
     * Represents a single High score entry
     * */
    [System.Serializable] 
    private class HighscoreEntry {
        public int score;
        public string name;
    }

}
