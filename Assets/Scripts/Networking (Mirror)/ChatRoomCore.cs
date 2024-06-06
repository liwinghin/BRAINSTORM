using UnityEngine;
using Mirror;
using UniRx;
using TMPro;

public class ChatRoomCore : NetworkBehaviour
{
    public TextMeshProUGUI chatLog;
    public TMP_InputField chatInput;

    public void Start()
    {
        chatLog = GameObject.Find("ChatLog").GetComponent<TextMeshProUGUI>();
        chatInput = GameObject.Find("ChatInput").GetComponent<TMP_InputField>();

        if (isLocalPlayer)
        {
            chatInput.onEndEdit.AsObservable()
                    .Where(_ => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    .Subscribe(_ => OnSendMessage())
                    .AddTo(this);
        }
    }


    private void OnSendMessage()
    {
        if (string.IsNullOrEmpty(chatInput.text)) return;

        CmdSendMessage(chatInput.text);
        chatInput.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcReceiveMessage(message);
    }

    [ClientRpc]
    private void RpcReceiveMessage(string message)
    {
        chatLog.text += message + "\n";
    }
}