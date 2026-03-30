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

    // FIX: Two separate coroutine handles.
    // requestCoroutine  = the UnityWebRequest in flight (NEVER stopped mid-flight)
    // typewriterCoroutine = the character-by-character display (safe to stop anytime)
    //
    // The previous version stored only one handle and StopCoroutine() inside
    // OnPanelClosed() was killing the web request itself mid-yield, causing
    // Unity to report every subsequent request as a network error.
    private Coroutine requestCoroutine;
    private Coroutine typewriterCoroutine;

    private bool isResponding = false;

    public void OnSendPressed()
    {
        string userText = inputField.text;

        if (string.IsNullOrWhiteSpace(userText))
            return;

        SetInputLocked(true);

        CreateMessage(userText, true);
        inputField.text = "";

        // Store the request coroutine separately so we never accidentally stop it.
        requestCoroutine = StartCoroutine(SendToBackend(userText));
    }

    // Called by ChatPanelController when the panel closes.
    // Stops the typewriter (safe) but deliberately leaves the web request
    // running to completion so Unity's networking stack stays clean.
    public void OnPanelClosed()
    {
        // Stop ONLY the typewriter — never the web request.
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        // Clean up the loading bubble if visible.
        if (currentLoadingBubble != null)
        {
            Destroy(currentLoadingBubble);
            currentLoadingBubble = null;
        }

        // Unlock input immediately for the user even if the request is still
        // in flight. When the request eventually finishes, OnApiSuccess /
        // OnApiError will call FinishResponse() which is a no-op if already unlocked.
        SetInputLocked(false);
        isResponding = false;
    }

    private IEnumerator SendToBackend(string question)
    {
        isResponding = true;
        currentLoadingBubble = CreateMessage("Typing...", false);

        yield return apiService.SendQuestion(
            question,
            contextProvider.CurrentModelName,
            OnApiSuccess,
            OnApiError
        );

        // Coroutine handle is done — clear it.
        requestCoroutine = null;
    }

    private void OnApiSuccess(string json)
    {
        if (currentLoadingBubble != null)
        {
            Destroy(currentLoadingBubble);
            currentLoadingBubble = null;
        }

        // If the panel was closed before the response came back, discard
        // the result silently — don't write into a closed panel.
        if (!isResponding)
            return;

        ChatResponseWrapper wrapper = JsonUtility.FromJson<ChatResponseWrapper>(json);

        if (wrapper == null || !wrapper.success || wrapper.data == null)
        {
            CreateMessage("I couldn't understand that question. Try asking about the current model.", false);
            FinishResponse();
            return;
        }

        var data = wrapper.data;

        if (string.IsNullOrEmpty(data.explanation))
        {
            CreateMessage("No explanation available for that topic.", false);
            FinishResponse();
            return;
        }

        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"<b><color=#4FC3F7>{data.title}</color></b>\n");
        builder.AppendLine(data.explanation + "\n");

        if (data.keyPoints != null)
        {
            foreach (var point in data.keyPoints)
                builder.AppendLine("• " + point);
        }

        typewriterCoroutine = StartCoroutine(TypeMessage(builder.ToString()));
    }

    private void OnApiError(string error)
    {
        if (currentLoadingBubble != null)
        {
            Destroy(currentLoadingBubble);
            currentLoadingBubble = null;
        }

        if (!isResponding)
            return;

        CreateMessage("Network error occurred. Please try again.", false);
        FinishResponse();
    }

    private void FinishResponse()
    {
        isResponding = false;
        typewriterCoroutine = null;
        SetInputLocked(false);
    }

    private void SetInputLocked(bool locked)
    {
        sendButton.interactable = !locked;
        inputField.interactable = !locked;
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

                if (i % 5 == 0)
                    ScrollToBottom();

                yield return new WaitForSeconds(0.01f);
            }
        }

        ScrollToBottom();
        FinishResponse();
    }

    private void ScrollToBottom()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent as RectTransform);
        scrollRect.verticalNormalizedPosition = 0f;
    }
}