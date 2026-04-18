using AINPC.Scripts.Core.Gameplay;
using AINPC.Scripts.Data;
using UnityEngine;

public class ChatWindowController : MonoBehaviour
{
    [SerializeField] private Transform chatScroll;
    [SerializeField] private ChatBubble chatBubblePrefab;
    
    void Start()
    {
        GlobalEventHandler.Instance.ApiResponseRecieved += CreateChatBubble;        
    }

    private void CreateChatBubble(ApiResponse apiResponse)
    {
        var chatBubble = Instantiate(chatBubblePrefab, chatScroll);
        chatBubble.Initialize(apiResponse.response);
    }
}
