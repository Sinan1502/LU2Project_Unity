using UnityEngine;

public class Object2D : MonoBehaviour
{
    public ObjectManager objectManager;
    public bool isDragging = false;

    private void Update()
    {
        if (isDragging)
            this.transform.position = GetMousePosition();
    }

    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        if (!isDragging)
        {
            objectManager.ShowMenu();
            SaveObject();
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }

    private void SaveObject()
    {
        int prefabId = 1; // Pas aan naar een juiste ID als je prefab IDs hebt
        float x = transform.position.x;
        float y = transform.position.y;
        float scaleX = transform.localScale.x;
        float scaleY = transform.localScale.y;
        float rotation = transform.rotation.eulerAngles.z;
        int layer = gameObject.layer;

        objectManager.SaveObjectToAPI(prefabId, x, y, scaleX, scaleY, rotation, layer);
    }
}
