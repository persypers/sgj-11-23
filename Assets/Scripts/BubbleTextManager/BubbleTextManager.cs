using System.Collections;
using System.Collections.Generic;
using Fancy;
using UnityEngine;
using System.Linq;

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

    public void DropNPCMessagesQueue(GameObject targetNPC)
    {
        messageQueue = messageQueue.Where(item => item.target != targetNPC).ToList();
        messageExecuteState.Remove(targetNPC);
        RemoveMessageByObject(targetNPC);
    }

    public void RemoveMessageByObject(GameObject target)
    {
        var pool = messagesPool.pool;
        foreach (GameObject poolObject in pool)
        {
            var bubbleText = poolObject.GetComponent<BubbleText>();
            if (bubbleText.GetCurrentTarget() == target)
            {
                poolObject.SetActive(false);
            }
        }
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
                    PlayRandomTargetSound(messageObj.target);
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

    private void PlayRandomTargetSound(GameObject target)
    {
        AudioSource audioSource = target.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = target.AddComponent<AudioSource>();
        }

        PlayerCollisionMessageTrigger playerCollisionMessageTrigger = target.GetComponentInChildren<PlayerCollisionMessageTrigger>();

        if (playerCollisionMessageTrigger != null)
        {
            List<AudioClip> audioClips = playerCollisionMessageTrigger.textDataAsset.audioClips;

            if (audioClips.Count > 0)
            {
                AudioClip randomClip = audioClips[Random.Range(0, audioClips.Count)];
                audioSource.clip = randomClip;
                audioSource.Play();
            }
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