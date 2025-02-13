using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highScoreEntryTransformList;

    private void Awake()
    {

        entryContainer = transform.Find("HighScoreEntryContainer");
        entryTemplate = entryContainer.Find("HighScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("HighScoreTable");
        HighScores highScores = string.IsNullOrEmpty(jsonString) ? new HighScores() : JsonUtility.FromJson<HighScores>(jsonString);

        if (highScores.highScoreEntryList != null)
        {
            // sort by score...
            for (int i = 0; i < highScores.highScoreEntryList.Count; i++)
            {
                for (int j = i + 1; j < highScores.highScoreEntryList.Count; j++)
                {
                    if (highScores.highScoreEntryList[j].score > highScores.highScoreEntryList[i].score)
                    {
                        // swap values...
                        (highScores.highScoreEntryList[j], highScores.highScoreEntryList[i]) = (highScores.highScoreEntryList[i], highScores.highScoreEntryList[j]);
                    }
                }
            }
            //sort to max 10 players
            if (highScores.highScoreEntryList.Count > 10)
            {
                for (int h = highScores.highScoreEntryList.Count; h > 10; h--)
                {
                    highScores.highScoreEntryList.RemoveAt(10);
                }
            }

            highScoreEntryTransformList = new List<Transform>();
            foreach (HighScoreEntry highScoreEntry in highScores.highScoreEntryList)
            {
                CreateHighScoreEntryTransform(highScoreEntry, entryContainer, highScoreEntryTransformList);
            }
        }
        else Debug.Log($"highScores.highScoreEntryList == Null");
    }


    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 60f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH"; break;

            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }
        entryTransform.Find("PosText").GetComponent<TextMeshProUGUI>().text = rankString;

        int score = highScoreEntry.score;
        entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();

        string name = highScoreEntry.name;
        entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = name;

        //Set background visible odds and evens, easier to read
        entryTransform.Find("ScoreBackGround").gameObject.SetActive(rank % 2 == 1);

        //HighLigt first
        if (rank == 1)
        {
            entryTransform.Find("PosText").GetComponent<TextMeshProUGUI>().color = Color.green;
            entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().color = Color.green;
            entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().color = Color.green;
        }

        Image trophyImage = entryTransform.Find("Trophy").GetComponent<Image>();
        switch (rank)
        {
            default:
                entryTransform.Find("Trophy").gameObject.SetActive(false);
                break;
            case 1:
                ColorUtility.TryParseHtmlString("#FFD200", out Color goldColor);
                trophyImage.color = goldColor;
                trophyImage.gameObject.SetActive(true);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#C6C6C6", out Color silverColor);
                trophyImage.color = silverColor;
                trophyImage.gameObject.SetActive(true);
                break;
            case 3:
                ColorUtility.TryParseHtmlString("#B76F56", out Color bronzeColor);
                trophyImage.color = bronzeColor;
                trophyImage.gameObject.SetActive(true);
                break;

        }

        transformList.Add(entryTransform);
    }

    // Function to add new values
    public static void AddHighScoreEntry(int score, string name)
    {
        // Create highscore entry
        HighScoreEntry highScoreEntry = new() { score = score, name = name };

        // Load saved HighScores
        string jsonString = PlayerPrefs.GetString("HighScoreTable");
        HighScores highScores = string.IsNullOrEmpty(jsonString) ? new HighScores() : JsonUtility.FromJson<HighScores>(jsonString);


        if (highScores.highScoreEntryList == null)
        {
            highScores.highScoreEntryList = new List<HighScoreEntry>();
        }

        // Add new entry to HighScores
        highScores.highScoreEntryList.Add(highScoreEntry);

        //sort to max 10 players
        if (highScores.highScoreEntryList.Count > 10)
        {
            for (int h = highScores.highScoreEntryList.Count; h > 10; h--)
            {
                highScores.highScoreEntryList.RemoveAt(10);
            }
        }

        // Save updated HighScores
        string json = JsonUtility.ToJson(highScores);
        PlayerPrefs.SetString("HighScoreTable", json);
        PlayerPrefs.Save();
    }
    private class HighScores
    {
        public List<HighScoreEntry> highScoreEntryList;
    }

    [System.Serializable]
    private class HighScoreEntry
    {
        public int score;
        public string name;
    }

    // clear the table
    public void ClearHighScores()
    {
        PlayerPrefs.DeleteKey("HighScoreTable");
        PlayerPrefs.Save();
    }
}