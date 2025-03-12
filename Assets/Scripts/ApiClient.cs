using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // Zorg ervoor dat je de TextMeshPro namespace toevoegt
using UnityEngine.SceneManagement;  // Voeg SceneManager toe om scènes te kunnen laden

public class ApiClient : MonoBehaviour
{
    // Verwijs naar de TMP_InputFields en Buttons
    public TMP_InputField emailInputField;  // Verbind dit met je email TMP_InputField
    public TMP_InputField passwordInputField;  // Verbind dit met je wachtwoord TMP_InputField
    public UnityEngine.UI.Button loginButton;  // Verbind dit met je login knop
    public UnityEngine.UI.Button registerButton;
    public TMP_Text errorMessageText;
    // Verbind dit met je register knop

    // Start is called before the first frame update
    void Start()
    {
        
        // Voeg listeners toe aan de knoppen
        loginButton.onClick.AddListener(Login);  // Wanneer je op de login knop klikt, wordt de Login functie uitgevoerd
        registerButton.onClick.AddListener(Register);  // Wanneer je op de register knop klikt, wordt de Register functie uitgevoerd
    }

    // Functie om in te loggen via een button-click
    public async void Login()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        var request = new PostLoginRequestDto
        {
            email = email,
            password = password
        };

        var jsondata = JsonUtility.ToJson(request);
        Debug.Log("JSON voor Login: " + jsondata);

        var response = await PerformApiCall("https://avansict2235816.azurewebsites.net/account/login", "POST", jsondata);

        if (!string.IsNullOrEmpty(response))
        {
            var responseDto = JsonUtility.FromJson<PostLoginResponseDto>(response);
            if (!string.IsNullOrEmpty(responseDto.accessToken))
            {
                PlayerPrefs.SetString("accessToken", responseDto.accessToken);
                PlayerPrefs.Save();
                SceneManager.LoadScene("EnvironmentScene");
            }
            else
            {
                ShowErrorMessage("Login mislukt! Geen toegangstoken ontvangen.");
            }
        }
        else
        {
            ShowErrorMessage("Login mislukt! Controleer je e-mail of wachtwoord.");
        }
    }



// Functie om te registreren via een button-click
public async void Register()
    {
        string email = emailInputField.text;  // Verkrijg de email van het TMP_InputField
        string password = passwordInputField.text;  // Verkrijg het wachtwoord van het TMP_InputField

        var request = new PostRegisterRequestDto
        {
            email = email,
            password = password
        };

        var jsondata = JsonUtility.ToJson(request);
        Debug.Log("JSON voor Register: " + jsondata);

        var response = await PerformApiCall("https://avansict2235816.azurewebsites.net/account/register", "POST", jsondata);
        Debug.Log(response);
    }

    // Functie om de API-aanroep uit te voeren
    private async Task<string> PerformApiCall(string url, string method, string jsonData = null, string token = null)
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

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API-aanroep is succesvol: " + request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                Debug.Log("Fout bij API-aanroep: " + request.error);
                Debug.Log("Response: " + request.downloadHandler.text);
                return null;
            }
        }
    }
    private void ShowErrorMessage(string message)
    {
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
    }

}
