/*using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : NetworkBehaviour
{
    public static ChatController Instance;

    [Header("UI")]
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject chatMessagePrefab;
    [SerializeField] private GameObject chatPanel;

    private bool isTyping = false;
    private bool isPanelVisible = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (chatPanel != null)
            chatPanel.SetActive(isPanelVisible);

        if (chatInputField != null)
            chatInputField.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Runner || !Runner.IsRunning) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleChatPanel();
        }

        if (!isPanelVisible) return; // Nếu panel đang ẩn thì không cho mở chat

        if (Input.GetKeyDown(KeyCode.Return)) // Enter
        {
            if (!isTyping)
            {
                OpenChatInput();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(chatInputField.text))
                {
                    SendChatMessage(chatInputField.text);
                }
                CloseChatInput();
            }
        }
    }

    private void ToggleChatPanel()
    {
        isPanelVisible = !isPanelVisible;
        chatPanel.SetActive(isPanelVisible);

        if (!isPanelVisible)
        {
            CloseChatInput();
        }
    }

    private void OpenChatInput()
    {
        isTyping = true;
        chatInputField.gameObject.SetActive(true);
        chatInputField.text = "";
        chatInputField.ActivateInputField();
    }

    private void CloseChatInput()
    {
        isTyping = false;
        chatInputField.DeactivateInputField();
        chatInputField.gameObject.SetActive(false);
    }

    private void SendChatMessage(string message)
    {
        foreach (var playerObj in FindObjectsOfType<PlayerProperties>())
        {
            if (playerObj.Object != null && playerObj.Object.HasInputAuthority)
            {
                RPC_SendChat(playerObj.NetworkedName, message);
                break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SendChat(string senderName, string message)
    {
        DisplayChatMessage(senderName, message);
    }

    private void DisplayChatMessage(string senderName, string message)
    {
        GameObject newMessage = Instantiate(chatMessagePrefab, chatContent);
        TMP_Text text = newMessage.GetComponent<TMP_Text>();
        text.text = $"<b>{senderName}:</b> {message}";

        Canvas.ForceUpdateCanvases();
        ScrollRect scroll = chatContent.GetComponentInParent<ScrollRect>();
        if (scroll != null)
        {
            scroll.verticalNormalizedPosition = 0f;
        }
    }
}
*/
using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : NetworkBehaviour
{
    public static ChatController Instance;

    [Header("UI")]
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private TextMeshProUGUI[] chatMessages; // Mảng cố định 5 ô Text

    public bool isTyping = false;
    private bool isPanelVisible = true;

    private Queue<string> messageQueue = new Queue<string>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (chatPanel != null)
            chatPanel.SetActive(false);

        if (chatInputField != null)
            chatInputField.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!Runner || !Runner.IsRunning) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleChatPanel();
        }

        if (!isPanelVisible) return; // Nếu panel đang ẩn thì không chat

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping)
            {
                OpenChatInput();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(chatInputField.text))
                {
                    SendChatMessage(chatInputField.text);
                }
                CloseChatInput();
            }
        }
    }

    private void ToggleChatPanel()
    {
        isPanelVisible = !isPanelVisible;
        chatPanel.SetActive(isPanelVisible);

        if (!isPanelVisible)
        {
            CloseChatInput();
        }
    }

    private void OpenChatInput()
    {
        isTyping = true;
        chatInputField.gameObject.SetActive(true);
        chatInputField.text = "";
        chatInputField.ActivateInputField();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //khóa di chuyển của player
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    private void CloseChatInput()
    {
        isTyping = false;
        chatInputField.DeactivateInputField();
        chatInputField.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //mở khóa di chuyển của player
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }

    }

    private void SendChatMessage(string message)
    {
        foreach (var playerObj in FindObjectsOfType<PlayerProperties>())
        {
            if (playerObj.Object != null && playerObj.Object.HasInputAuthority)
            {
                RPC_SendChat(playerObj.NetworkedName, message);
                break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SendChat(string senderName, string message)
    {
        DisplayChatMessage(senderName, message);
    }

    private void DisplayChatMessage(string senderName, string message)
    {
        string fullMessage = $"<b>{senderName}:</b> {message}";

        messageQueue.Enqueue(fullMessage);

        if (messageQueue.Count > chatMessages.Length)
        {
            messageQueue.Dequeue();
        }

        int index = 0;
        foreach (var msg in messageQueue)
        {
            chatMessages[index].text = msg;
            index++;
        }
    }
}
