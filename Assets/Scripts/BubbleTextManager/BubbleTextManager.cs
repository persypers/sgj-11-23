using System.Collections;
using System.Collections.Generic;
using Fancy;
using UnityEngine;

public struct BubbleTextMessage
{
    public string messageText;
    public float showDuration;
    public GameObject target;
    public MessageIntonations? messageIntonation;

    public BubbleTextMessage(string text, float duration, GameObject targetObject, MessageIntonations? intonation)
    {
        messageText = text;
        showDuration = duration;
        target = targetObject;
        messageIntonation = intonation;
    }
}

public class BubbleTextManager : Fancy.MonoSingleton<BubbleTextManager>
{
    public ObjectPool messagesPool;
    private List<BubbleTextMessage> messageQueue = new List<BubbleTextMessage>();
    private HashSet<GameObject> messageExecuteState = new HashSet<GameObject>();

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnAddMessageToQueue(BubbleTextMessage messageObj)
    {
        messageQueue.Add(messageObj);
        ExecuteShowText();
    }

    private void ExecuteShowText()
    {
        List<BubbleTextMessage> messagesToRemove = new List<BubbleTextMessage>();

        foreach (BubbleTextMessage messageObj in messageQueue)
        {
            if (!messageExecuteState.Contains(messageObj.target))
            {
                GameObject textGameObject = messagesPool.Get();
                textGameObject.SetActive(true);
                BubbleText bubbleText = textGameObject.GetComponent<BubbleText>();

                if (bubbleText != null)
                {
                    messageExecuteState.Add(messageObj.target);
                    bubbleText.OnShowBubbleText(messageObj);
                    StartCoroutine(AsyncObjectDisable(messageObj, textGameObject));
                }
                messagesToRemove.Add(messageObj);
            }
        }

        foreach (BubbleTextMessage messageObj in messagesToRemove)
        {
            messageQueue.Remove(messageObj);
        }
    }

    IEnumerator AsyncObjectDisable(BubbleTextMessage messageObj, GameObject objectToDestroy)
    {
        yield return new WaitForSeconds(messageObj.showDuration);
        objectToDestroy.SetActive(false);
        messageExecuteState.Remove(messageObj.target);
        if (messageQueue.Count > 0)
        {
            ExecuteShowText();
        }
    }
}