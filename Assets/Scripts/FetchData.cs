using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;

public class FetchData : MonoBehaviour
{
    string serverUrl = "http://localhost:3000/player";
    List<PlayerData> playerList;
    PlayerData player;
    public GameObject playerDataPrefab;      // Prefab for each player entry
    public Transform playerListContent;      // Parent object for the list
    public GameObject playerData;            // UI for displaying individual player info

    void Start()
    {
        StartFetch();   // Fetch all players at start
    }

    // Fetch all players from server
    public IEnumerator GetData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log($"Received data: {json}");

                playerList = JsonConvert.DeserializeObject<List<PlayerData>>(json);

                PopulatePlayerList();  // Populate UI with all players
            }
            else
            {
                Debug.Log($"Error fetching data: {request.error}");
            }
        }
    }

    // Populate UI list with all players
    void PopulatePlayerList()
    {
        // Clear previous entries
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // Create new entries
        foreach (var p in playerList)
        {
            GameObject playerEntry = Instantiate(playerDataPrefab, playerListContent);
            playerEntry.transform.GetChild(0).GetComponent<TMP_Text>().text = p.screenName;
            playerEntry.transform.GetChild(1).GetComponent<TMP_Text>().text = p.firstName;
            playerEntry.transform.GetChild(2).GetComponent<TMP_Text>().text = p.lastName;
            playerEntry.transform.GetChild(3).GetComponent<TMP_Text>().text = p.dateStartPlaying;
            playerEntry.transform.GetChild(4).GetComponent<TMP_Text>().text = p.score.ToString();
        }
    }

    // Fetch a specific player by name
    public IEnumerator GetDataByName(string json, string playerName)
    {
        string url = serverUrl + "/" + playerName;
        Debug.Log(url);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            Debug.Log($"Success: {response}");

            player = JsonConvert.DeserializeObject<PlayerData>(response);

            GetPlayer();  // Update individual player UI
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }

    // Start fetching all players
    public void StartFetch()
    {
        StartCoroutine(GetData());
    }

    // Setup player data for a specific search by username
    public void SetupPlayerSearchData(string username)
    {
        player = new PlayerData();
        player.screenName = username;

        string json = JsonUtility.ToJson(player);
        Debug.Log(json);
        StartCoroutine(GetDataByName(json, username));
    }

    // Display individual player info in UI
    public void GetPlayer()
    {
        playerData.transform.GetChild(0).GetComponent<TMP_Text>().text = player.screenName;
        playerData.transform.GetChild(1).GetComponent<TMP_Text>().text = player.firstName;
        playerData.transform.GetChild(2).GetComponent<TMP_Text>().text = player.lastName;
        playerData.transform.GetChild(3).GetComponent<TMP_Text>().text = player.dateStartPlaying;
        playerData.transform.GetChild(4).GetComponent<TMP_Text>().text = player.score.ToString();
    }

    // Extract Player ID from JSON response
    string ExtractPlayerId(string jsonResponse)
    {
        int index = jsonResponse.IndexOf("\"playerid\":\"") + 12;
        if (index < 12) return "";
        int endIndex = jsonResponse.IndexOf("\"", index);
        return jsonResponse.Substring(index, endIndex - index);
    }
}

// PlayerData class for JSON deserialization
public class PlayerData
{
    public string screenName;
    public string firstName;
    public string lastName;
    public string dateStartPlaying;
    public int score;
}
