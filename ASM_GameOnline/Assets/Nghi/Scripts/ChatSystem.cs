using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

public class ChatSystem : NetworkBehaviour
{
    /*private TextMeshProUGUI textMessage;
    private TMP_InputField inputFieldMessage;
    private Button sendButton;
    private Button openChatButton;
    private Button closeChatButton;
    private CanvasGroup chatPanelGroup;

    public override void Spawned()
    {
        // Tìm và gán các component UI
        textMessage = GameObject.Find("Text Message")?.GetComponent<TextMeshProUGUI>();
        inputFieldMessage = GameObject.Find("InputField Message")?.GetComponent<TMP_InputField>();
        sendButton = GameObject.Find("Send Button")?.GetComponent<Button>();
        openChatButton = GameObject.Find("Open Chat Button")?.GetComponent<Button>();
        closeChatButton = GameObject.Find("Close Chat Button")?.GetComponent<Button>();
        chatPanelGroup = GameObject.Find("Chat Panel")?.GetComponent<CanvasGroup>();

        if (sendButton != null) sendButton.onClick.AddListener(SendMessageChat);
        if (openChatButton != null) openChatButton.onClick.AddListener(ShowChatPanel);
        if (closeChatButton != null) closeChatButton.onClick.AddListener(HideChatPanel);

        HideChatPanel(); // Ẩn chat ngay từ đầu
    }

    private void ShowChatPanel()
    {
        if (chatPanelGroup == null) return;
        chatPanelGroup.alpha = 1;
        chatPanelGroup.blocksRaycasts = true;
        chatPanelGroup.interactable = true;
    }

    private void HideChatPanel()
    {
        if (chatPanelGroup == null) return;
        chatPanelGroup.alpha = 0;
        chatPanelGroup.blocksRaycasts = false;
        chatPanelGroup.interactable = false;
    }

    public void SendMessageChat()
    {
        var message = inputFieldMessage?.text;
        if (string.IsNullOrWhiteSpace(message)) return;

        var id = Runner.LocalPlayer.PlayerId;
        var text = $"Player {id}: {message}";
        RpcChat(text);

        if (inputFieldMessage != null)
            inputFieldMessage.text = "";
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChat(string msg)
    {
        if (textMessage == null)
            textMessage = GameObject.Find("Text Message")?.GetComponent<TextMeshProUGUI>();

        if (textMessage != null)
            textMessage.text += msg + "\n";
    }

*/

   
}
