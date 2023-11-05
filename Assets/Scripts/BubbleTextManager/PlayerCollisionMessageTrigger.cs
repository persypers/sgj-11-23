using UnityEngine;
using SpeechText;
using System.Collections.Generic;
using System.Linq;

public class TriggerTest : MonoBehaviour
{
    public string message = "Sample Text";
    public TextData textDataAsset;

    public string currentState = "Default";
    private BubbleTextManager bubbleTextManager;

    private void Start()
    {
        bubbleTextManager = GameObject.Find("BubbleTextManager").GetComponent<BubbleTextManager>();

        if (bubbleTextManager == null)
        {
            Debug.Log("NO BUBBLE");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var textLines = textDataAsset.GetTextLinesByStateName(currentState);
        if (bubbleTextManager != null)
        {
            bubbleTextManager.OnAddMessageToQueue(
                new BubbleTextMessage { messageText = textLines[Random.Range(0, textLines.Count)].text, showDuration = 2, target = gameObject }
                );
        }
    }

    void OnTriggerExit(Collider other)
    {

    }
}
