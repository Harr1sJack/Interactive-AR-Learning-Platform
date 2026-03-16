using TMPro;
using UnityEngine;

public class DotsAnimator : MonoBehaviour
{
    public TextMeshProUGUI text;
    private float timer;
    private int dotCount;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            dotCount = (dotCount + 1) % 4;
            text.text = "Scanning" + new string('.', dotCount);
            timer = 0;
        }
    }
}