using UnityEngine;
using SpeechText;
using System.Collections;
using RigidFps;

public class PlayerCollisionMessageTrigger : MonoBehaviour
{
    public TextData textDataAsset;
    public string currentState = "Default";
    public GameObject targetGameObject;
    public float showDuration = 2;
    public GameObject objectToTrigger = null; // Указание объекта, который запустит действие
    private bool isReadyToTalk = true;


    private void OnTriggerStay(Collider other)
    {
        var targ = objectToTrigger == null ? Player.Instance.gameObject : objectToTrigger;
        if (isReadyToTalk && other.gameObject == targ)
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
        if (textDataAsset.GetTextLinesByStateName(newState).Count > 0)
        {
            currentState = newState;
            Debug.Log("New state: " + newState);
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

    void OnDisable()
    {
        if( BubbleTextManager.Instance )
            BubbleTextManager.Instance.RemoveMessageByObject( targetGameObject );
    }
}
