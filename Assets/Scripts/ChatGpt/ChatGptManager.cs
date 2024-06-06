using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;

public class ChatGptManager : MonoBehaviour
{
    [System.Serializable]
    public class OnReSponseEvent : UnityEvent<string> { }

    public OnReSponseEvent OnReSponse;

    private OpenAIApi openAI = new();
    private List<ChatMessage> messages = new();

    public async void Ask(string input)
    {
        ChatMessage newMsg = new ChatMessage();
        newMsg.Content = input;
        newMsg.Role = "user";

        messages.Add(newMsg);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);
            OnReSponse.Invoke(chatResponse.Content);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
