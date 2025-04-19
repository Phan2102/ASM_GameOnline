using Fusion;
using TMPro;
using UnityEngine;
using static Unity.Collections.Unicode;

public class ChatSystem : NetworkBehaviour
{
    public TextMeshProUGUI textMessage;
    public TMP_InputField inputFieldMessage;
    public GameObject sendButton;


    //Chạy ngay sau khi nhân vật được Spawn trong mạng
    public override void Spawned()
    {
        textMessage = GameObject.Find("Text Message").GetComponent<TextMeshProUGUI>();
        inputFieldMessage = GameObject.Find("InputField Message").GetComponent<TMP_InputField>();
        sendButton = GameObject.Find("Send Button");
        sendButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SendMessageChat);
        //var id = Runner.LocalPlayer.PlayerId;
        //RpcNotify(id);
    }

    public void SendMessageChat()
    {
        var message = inputFieldMessage.text;
        if (string.IsNullOrWhiteSpace(message)) return;

        var id = Runner.LocalPlayer.PlayerId;
        var text = $"Player {id}: {message}";
        RpcChat(text);
        inputFieldMessage.text = "";//Sau khi nhấn gửi tin nhắn thì clear nội dung tin nhắn
        //cũ trong InputField
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChat(string msg)
    {
        //textMessage.text += msg + "\n";

        // Check null để tránh lỗi
        if (textMessage == null)
            textMessage = GameObject.Find("Text Message")?.GetComponent<TextMeshProUGUI>();

        if (textMessage != null)
            textMessage.text += msg + "\n";
    }
}
