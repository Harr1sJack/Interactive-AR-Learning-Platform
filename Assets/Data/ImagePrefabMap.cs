using UnityEngine;

[CreateAssetMenu(menuName = "AR Learning/Image Prefab Map")]
public class ImagePrefabMap : ScriptableObject
{
    public ImageEntry[] entries;

    [System.Serializable]
    public class ImageEntry
    {
        public string imageName;
        public GameObject prefab;
    }

    public GameObject GetPrefab(string imageName)
    {
        foreach (var e in entries)
            if (e.imageName == imageName)
                return e.prefab;
        return null;
    }
}
