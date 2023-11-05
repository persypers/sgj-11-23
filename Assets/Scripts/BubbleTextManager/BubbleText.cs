using UnityEngine;
using TMPro;

public enum MessageIntonations
{
    Cry,
    Whisper,
    Default
}
public class BubbleText : MonoBehaviour
{
    public Transform playerCamera;

    private GameObject currentTarget;


    void Start()
    {
        GameObject cameraSmu = GameObject.Find("Main CameraSmu");
        playerCamera = cameraSmu.transform;
    }

    void Update()
    {
        if (currentTarget != null)
        {
            transform.position = currentTarget.transform.position + new Vector3(0, 1.5f, 0);
        }
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
    }

    private void PrepareTextMesh(TextMeshPro textObject, BubbleTextMessage bubbleTextMessage)
    {
        textObject.text = bubbleTextMessage.messageText;
        switch (bubbleTextMessage.messageIntonation)
        {
            case MessageIntonations.Cry:
                {
                    textObject.outlineWidth = 0.1f;
                    textObject.fontStyle = FontStyles.Bold;
                    textObject.color = Color.red;
                    textObject.outlineColor = Color.black;
                    break;
                }
            case MessageIntonations.Whisper:
                {
                    textObject.outlineWidth = 0.1f;
                    textObject.color = Color.yellow;
                    break;
                }
            case MessageIntonations.Default:
            default:
                {
                    break;
                }
        }
    }
    public void OnShowBubbleText(BubbleTextMessage bubbleTextMessage)
    {
        if (bubbleTextMessage.target != null)
        {
            currentTarget = bubbleTextMessage.target;
            Vector3 bubblePosition = bubbleTextMessage.target.transform.position + Vector3.up;
            transform.position = bubblePosition;
            TextMeshPro textMesh = GetComponent<TextMeshPro>();
            if (textMesh != null)
            {
                PrepareTextMesh(textMesh, bubbleTextMessage);
            }
        }
    }
}
