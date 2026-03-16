using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class ChatApiService : MonoBehaviour
{
    private const string API_URL = "https://ar-learning-backend.onrender.com/api/chat";

    public IEnumerator SendQuestion(
        string question,
        string domain,
        System.Action<string> onSuccess,
        System.Action<string> onError)
    {
        ChatRequest requestData = new ChatRequest
        {
            question = question,
            domain = domain
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}

[System.Serializable]
public class ChatRequest
{
    public string question;
    public string domain;
}

[System.Serializable]
public class ChatResponseWrapper
{
    public bool success;
    public ChatResponseData data;
}

[System.Serializable]
public class ChatResponseData
{
    public string title;
    public string explanation;
    public string[] keyPoints;
}