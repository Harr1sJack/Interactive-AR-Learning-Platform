using UnityEngine;

public class ScanLineMover : MonoBehaviour
{
    public RectTransform rect;
    public float speed = 200f;
    public float top = 330f;
    public float bottom = -330f;

    void Update()
    {
        rect.anchoredPosition += Vector2.down * speed * Time.deltaTime;

        if (rect.anchoredPosition.y < bottom)
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, top);
    }
}