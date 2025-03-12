using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;

    // Lijst met objecten die geplaatst zijn in de wereld
    private List<GameObject> placedObjects;

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        // Verberg het zijmenu
        UISideMenu.SetActive(false);
        // Instantieer het prefab object op de positie (0,0,0) met geen rotatie
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10)); // Zorg dat het op een zichtbare plek komt
        spawnPosition.z = 0; // Z op 0 zodat het niet achter de camera staat

        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], spawnPosition, Quaternion.identity);

        // Haal het Object2D component op van het nieuw geplaatste object
        Object2D object2D = instanceOfPrefab.GetComponent<Object2D>();
        // Stel de objectManager van het object in op deze instantie van ObjectManager
        object2D.objectManager = this;
        // Zet de isDragging eigenschap van het object op true zodat het gesleept kan worden
        object2D.isDragging = true;
    }

    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

    // Methode om de huidige scène te resetten
    public void Reset()
    {
        // Laad de huidige scène opnieuw
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
