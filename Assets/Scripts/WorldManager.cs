using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    private string apiBaseUrl = "https://avansict2235816.azurewebsites.net/Environment2D";
    private string token;
    public WorldListWrapper playerWorlds;
    public Guid WorldId;
    

    private void Start()
    {
        token = PlayerPrefs.GetString("accessToken", "");
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("Geen toegangstoken gevonden! Zorg dat je bent ingelogd.");
            return;
        }
        LoadWorlds();
    }

    public async void LoadWorlds()
    {
        string response = await PerformApiCall(apiBaseUrl, "GET", null);
        Debug.Log("Response van de API: " + response);
        if (!string.IsNullOrEmpty(response))
        {
            playerWorlds = JsonUtility.FromJson<WorldListWrapper>("{\"items\":" + response + "}");
            Debug.Log($"Gevonden werelden: {playerWorlds.items.Length}");
            foreach (var environnment in playerWorlds.items)
            {
                Debug.Log($"Wereld: {environnment.name}");
            }
        }
    }

    public async void CreateWorld(string worldName, int maxHeight, int maxLength)
    {
        if (playerWorlds.items.Length >= 5)
        {
            Debug.LogError("Je hebt het maximum van 5 werelden bereikt!");
            return;
        }

        if (string.IsNullOrWhiteSpace(worldName) || worldName.Length < 1 || worldName.Length > 25)
        {
            Debug.LogError("Wereldnaam moet tussen 1 en 25 karakters lang zijn!");
            return;
        }
        Debug.Log($"Wereldnaam die naar de API wordt gestuurd: {worldName}");

        foreach (var world in playerWorlds.items)
        {
            if (world.name == worldName)
            {
                Debug.LogError("Je hebt al een wereld met deze naam!");
                return;
            }
        }

        Environment newWorld = new Environment
        {
            Name = worldName,
            MaxHeight = maxHeight,
            MaxLength = maxLength
        };

        string jsonData = JsonUtility.ToJson(newWorld);
        Debug.Log($"JSON Data die naar de API wordt gestuurd: {jsonData}");
        string response = await PerformApiCall(apiBaseUrl, "POST", jsonData);

        if (!string.IsNullOrEmpty(response))
        {
            Debug.Log($"Wereld '{worldName}' aangemaakt!");
            LoadWorlds();
        }
    }

    public async void LoadWorld(Guid Id)
    {
        string response = await PerformApiCall($"{apiBaseUrl}/{Id}", "GET", null);

        if (!string.IsNullOrEmpty(response))
        {
            Environment2D world = JsonUtility.FromJson<Environment2D>(response);
            Debug.Log("Wereld geladen: " + world.name);
            PlayerPrefs.SetString("currentWorldId", Id.ToString());
            PlayerPrefs.Save();

            // Speelbare wereld laden
            SceneManager.LoadScene("HomeScene");
        }
    }

    public async void DeleteWorld(Guid Id)
    {
        string response = await PerformApiCall($"{apiBaseUrl}/{Id}", "DELETE", null);
        Debug.Log("Wereld verwijderd: " + response);
        LoadWorlds();
    }

    private async Task<string> PerformApiCall(string url, string method, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + token);

            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"API Call failed: {request.error}");
                return null;
            }
        }
    }

    [Serializable]
    public class WorldListWrapper
    {
        public Environment2D[] items;
    }

    public static WorldManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    public async void SaveObjectToAPI(int prefabId, float x, float y, float scaleX, float scaleY, float rotation, int layer)
    {
        string environmentId = PlayerPrefs.GetString("currentWorldId", "");

        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("Geen environmentId gevonden! Kan object niet opslaan.");
            return;
        }

        GameObject2D obj = new GameObject2D(environmentId, prefabId, x, y, scaleX, scaleY, rotation, layer);
        string jsonData = JsonUtility.ToJson(obj);

        string apiUrl = "https://avansict2235816.azurewebsites.net/Object2D"; // Pas de endpoint aan indien nodig
        string response = await PerformApiCall(apiUrl, "POST", jsonData);

        if (!string.IsNullOrEmpty(response))
        {
            Debug.Log("Object succesvol opgeslagen!");
        }
    }

}
