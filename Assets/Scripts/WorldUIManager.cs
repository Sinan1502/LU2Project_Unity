using System;
using TMPro;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.HID.HID;

public class WorldUIManager : MonoBehaviour
{
    public TMP_InputField worldNameInput;
    public UnityEngine.UI.Button createWorldButton;
    public UnityEngine.UI.Button loadWorldsButton;
    public UnityEngine.UI.Button deleteWorldButton;
    public Transform buttonPanel;
    public GameObject buttonPrefab;
    public Environment2D ChosenWorld;
    private WorldManager worldManager;

    public Guid ChosenWorldId;

    void Start()
    {
        buttonPanel.gameObject.SetActive(false);
        worldManager = FindAnyObjectByType<WorldManager>();


        if (worldManager == null)
        {
            Debug.LogError("WorldManager niet gevonden in de scene!");
            return;
        }

        createWorldButton.onClick.AddListener(CreateWorld);
        loadWorldsButton.onClick.AddListener(LoadWorlds);
        deleteWorldButton.onClick.AddListener(DeleteWorld);
    }

    void CreateWorld()
    {
        string worldName = worldNameInput.text;
        worldManager.CreateWorld(worldName, 10, 10);
    }

    void LoadWorlds()
    {
        worldManager.LoadWorlds();
        buttonPanel.gameObject.SetActive(true);
        PopulateWorldButtons();
    }

    void DeleteWorld()
    {
        Debug.Log("Voer een wereld-ID in om te verwijderen!");
    }

    public void PopulateWorldButtons()
    {
        // Verwijder alleen oude knoppen als er al knoppen zijn
        if (buttonPanel.childCount > 0)
        {
            foreach (Transform child in buttonPanel)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Environment2D environment in WorldManager.Instance.playerWorlds.items)
        {
            GameObject button = Instantiate(buttonPrefab, buttonPanel);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = environment.name;

            UnityEngine.UI.Button worldButton = button.GetComponent<UnityEngine.UI.Button>();
            worldButton.onClick.AddListener(() => OnWorldButtonClicked(environment));

            // Voeg een Delete-knop toe
            GameObject deleteButton = Instantiate(buttonPrefab, buttonPanel);
            TMP_Text deleteText = deleteButton.GetComponentInChildren<TMP_Text>();
            deleteText.text = "Delete" + environment.name;
            UnityEngine.UI.Button deleteComponent = deleteButton.GetComponent<UnityEngine.UI.Button>();
            deleteComponent.onClick.AddListener(() => DeleteWorld(environment));
        }
    }

    void OnWorldButtonClicked(Environment2D world)
    {
        Debug.Log("Wereld geselecteerd: " + world.name);
        ChosenWorld = world;
        Debug.Log(ChosenWorld.name);

        ChosenWorldId = Guid.Parse(ChosenWorld.id);
        WorldManager.Instance.WorldId = ChosenWorldId;

        // 🌍 Sla de gekozen wereld-ID op in PlayerPrefs
        PlayerPrefs.SetString("currentWorldId", ChosenWorldId.ToString());
        PlayerPrefs.Save();

        SceneManager.LoadScene("HomeScene");
    }

    void DeleteWorld(Environment2D world)
    {
        Debug.Log("Verwijderen van wereld: " + world.name);
        WorldManager.Instance.DeleteWorld(Guid.Parse(world.id));

        // Wacht een klein beetje om zeker te zijn dat de wereld verwijderd is
        Invoke(nameof(LoadWorlds), 1f);
    }

}
