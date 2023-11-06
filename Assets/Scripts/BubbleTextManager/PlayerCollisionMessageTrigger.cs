using UnityEngine;
using SpeechText;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class PlayerCollisionMessageTrigger : MonoBehaviour
{
    public TextData textDataAsset;
    public string currentState = "Default";
    public GameObject targetGameObject;
    public float showDuration = 2;
    public GameObject objectToTrigger; // Указание объекта, который запустит действие
    private bool isReadyToTalk = true;

    
    private void OnTriggerStay(Collider other)
    {
        if (isReadyToTalk && other.gameObject == objectToTrigger)
        {
            var textLines = textDataAsset.GetTextLinesByStateName(currentState);
            if (BubbleTextManager.Instance != null)
            {
                BubbleTextManager.Instance.OnAddMessageToQueue(
                    new BubbleTextMessage { messageText = textLines[Random.Range(0, textLines.Count)].text, showDuration = showDuration, target = targetGameObject }
                    );
                StartCoroutine(AsyncWaitCooldown(textDataAsset.messageShowCooldown));
            }
        }
    }

    public void SetCurrentState(string newState)
    {
        currentState = newState;
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
