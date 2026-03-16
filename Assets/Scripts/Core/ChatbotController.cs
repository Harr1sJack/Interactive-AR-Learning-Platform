using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class ChatbotController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Dependencies")]
    [SerializeField] private ModelContextProvider contextProvider;
    [SerializeField] private ChatApiService apiService;

    private GameObject currentLoadingBubble;

    public void OnSendPressed()
    {
        string userText = inputField.text;

        if (string.IsNullOrWhiteSpace(userText))
            return;

        CreateMessage(userText, true);

        inputField.text = "";
        sendButton.interactable = false;

        StartCoroutine(SendToBackend(userText));
    }

    private IEnumerator SendToBackend(string question)
    {
        // Create loading bubble
        currentLoadingBubble = CreateMessage("Typing...", false);

        yield return apiService.SendQuestion(
            question,
            contextProvider.CurrentModelName,
            OnApiSuccess,
            OnApiError
        );
    }

    private void OnApiSuccess(string json)
    {
        sendButton.interactable = true;

        // Remove loading bubble
        if (currentLoadingBubble != null)
            Destroy(currentLoadingBubble);

        ChatResponseWrapper wrapper = JsonUtility.FromJson<ChatResponseWrapper>(json);

        if (wrapper == null || !wrapper.success || wrapper.data == null)
        {
            CreateMessage("I couldn't understand that question. Try asking about the current model.", false);
            return;
        }

        var data = wrapper.data;

        if (string.IsNullOrEmpty(data.explanation))
        {
            CreateMessage("No explanation available for that topic.", false);
            return;
        }

        StringBuilder builder = new StringBuilder();

        builder.AppendLine($"<b><color=#4FC3F7>{data.title}</color></b>\n");
        builder.AppendLine(data.explanation + "\n");

        if (data.keyPoints != null)
        {
            foreach (var point in data.keyPoints)
            {
                builder.AppendLine("• " + point);
            }
        }

        StartCoroutine(TypeMessage(builder.ToString()));
    }

    private void OnApiError(string error)
    {
        sendButton.interactable = true;

        if (currentLoadingBubble != null)
            Destroy(currentLoadingBubble);

        CreateMessage("Network error occurred. Please try again.", false);
    }

    private GameObject CreateMessage(string text, bool isUser)
    {
        GameObject msg = Instantiate(messagePrefab, chatContent);

        TMP_Text tmp = msg.GetComponentInChildren<TMP_Text>();
        Image bg = msg.GetComponent<Image>();

        tmp.text = text;

        if (isUser)
        {
            bg.color = new Color32(46, 125, 255, 255);
            tmp.alignment = TextAlignmentOptions.Right;
        }
        else
        {
            bg.color = new Color32(42, 42, 42, 255);
            tmp.alignment = TextAlignmentOptions.Left;
        }

        ScrollToBottom();
        return msg;
    }

    private IEnumerator TypeMessage(string fullText)
    {
        GameObject msg = Instantiate(messagePrefab, chatContent);

        TMP_Text tmp = msg.GetComponentInChildren<TMP_Text>();
        Image bg = msg.GetComponent<Image>();

        bg.color = new Color32(42, 42, 42, 255);
        tmp.alignment = TextAlignmentOptions.Left;

        tmp.text = "";

        int i = 0;

        while (i < fullText.Length)
        {
            if (fullText[i] == '<')
            {
                // Immediately process rich text tags
                while (i < fullText.Length && fullText[i] != '>')
                {
                    tmp.text += fullText[i];
                    i++;
                }

                if (i < fullText.Length)
                {
                    tmp.text += fullText[i];
                    i++;
                }
            }
            else
            {
                tmp.text += fullText[i];
                i++;
                ScrollToBottom();
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}