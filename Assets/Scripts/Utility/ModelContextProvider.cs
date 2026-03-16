using UnityEngine;

public class ModelContextProvider : MonoBehaviour
{
    public string CurrentModelName { get; private set; }

    public void SetContext(string modelName)
    {
        CurrentModelName = modelName;
    }
}