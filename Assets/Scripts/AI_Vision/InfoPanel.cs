using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform arrow;

    public float openY = 0;
    public float closedY = -350;

    public float slideSpeed = 8f;

    private bool isOpen = false;
    private float targetY;

    void Start()
    {
        targetY = closedY;
    }

    void Update()
    {
        Vector2 pos = panel.anchoredPosition;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * slideSpeed);
        panel.anchoredPosition = pos;
    }

    public void TogglePanel()
    {
        if (isOpen)
        {
            targetY = closedY;

            // arrow up
            arrow.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            targetY = openY;

            // arrow down
            arrow.rotation = Quaternion.Euler(0, 0, 0);
        }

        isOpen = !isOpen;
    }
}