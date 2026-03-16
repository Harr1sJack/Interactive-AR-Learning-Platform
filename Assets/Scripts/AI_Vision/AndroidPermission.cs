using UnityEngine;
using UnityEngine.Android;

public class AndroidPermission : MonoBehaviour
{
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
    }
}