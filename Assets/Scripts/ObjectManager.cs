using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;

public class ObjectManager : MonoBehaviour
{
    public GameObject UISideMenu;
    public List<GameObject> prefabObjects;
    private List<GameObject> placedObjects;

    public void PlaceNewObject2D(int index)
    {
        UISideMenu.SetActive(false);
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
        spawnPosition.z = 0;

        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], spawnPosition, Quaternion.identity);
        Object2D object2D = instanceOfPrefab.GetComponent<Object2D>();
        object2D.objectManager = this;
        object2D.isDragging = true;
    }

    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public async void SaveObjectToAPI(int prefabId, float x, float y, float scaleX, float scaleY, float rotation, int layer)
    {
        string environmentId = PlayerPrefs.GetString("currentWorldId", "");
        Debug.Log($"🌍 Opgehaalde environmentId: {environmentId}");

        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("❌ Geen environmentId gevonden! Kan object niet opslaan.");
            return;
        }


        GameObject2D obj = new GameObject2D(environmentId, prefabId, x, y, scaleX, scaleY, rotation, layer);
        string jsonData = JsonUtility.ToJson(obj);

        string apiUrl = "https://avansict2235816.azurewebsites.net/Object2D";
        Task<string> responseTask = PerformApiCall(apiUrl, "POST", jsonData);
        string response = await responseTask;

        if (!string.IsNullOrEmpty(response))
        {
            Debug.Log("✅ Object succesvol opgeslagen!");
        }
    }

    private async Task<string> PerformApiCall(string url, string method, string jsonData = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // ✅ Voeg hier de Authorization-header toe
            string token = PlayerPrefs.GetString("accessToken", "");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }
            else
            {
                Debug.LogError("⚠️ Geen access token gevonden! Mogelijk niet ingelogd.");
                return null;
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ API-aanroep succesvol: " + request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError("❌ Fout bij API-aanroep: " + request.error);
                Debug.LogError("⚠️ Response: " + request.downloadHandler.text);
                return null;
            }
        }
    }

}
