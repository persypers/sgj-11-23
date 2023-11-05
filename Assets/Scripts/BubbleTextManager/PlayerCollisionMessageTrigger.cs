using UnityEngine;
using SpeechText;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class PlayerCollisionMessageTrigger : MonoBehaviour
{
    public TextData textDataAsset;
    public string currentState = "Default";
    public BubbleTextManager bubbleTextManager;

    public GameObject targetGameObject;

    public GameObject objectToTrigger; // Указание объекта, который запустит действие

    private bool isReadyToTalk = true;

    private void OnTriggerStay(Collider other)
    {
        if (isReadyToTalk && other.gameObject == objectToTrigger)
        {
            var textLines = textDataAsset.GetTextLinesByStateName(currentState);
            if (bubbleTextManager != null)
            {
                bubbleTextManager.OnAddMessageToQueue(
                    new BubbleTextMessage { messageText = textLines[Random.Range(0, textLines.Count)].text, showDuration = 2, target = targetGameObject }
                    );
                StartCoroutine(AsyncWaitCooldown(textDataAsset.messageShowCooldown));
            }
        }
    }

    private IEnumerator AsyncWaitCooldown(float cooldown)
    {
        isReadyToTalk = false;
        yield return new WaitForSeconds(cooldown);
        isReadyToTalk = true;
    }
    private void Start()
    {
    }

    void OnTriggerExit(Collider other)
    {

    }
}
