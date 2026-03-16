using UnityEngine;

[CreateAssetMenu(menuName = "AR Learning/Model Info")]
public class ModelInfoData : ScriptableObject
{
    public string title;

    [TextArea(4, 8)]
    public string description;
}
