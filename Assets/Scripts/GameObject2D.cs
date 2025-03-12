[System.Serializable]
public class GameObject2D
{
    public string environmentId;
    public int prefabId;
    public float positionX;
    public float positionY;
    public float scaleX;
    public float scaleY;
    public float rotationZ;
    public int sortingLayer;

    public GameObject2D(string envId, int prefab, float x, float y, float sX, float sY, float rot, int layer)
    {
        environmentId = envId;
        prefabId = prefab;
        positionX = x;
        positionY = y;
        scaleX = sX;
        scaleY = sY;
        rotationZ = rot;
        sortingLayer = layer;
    }
}
