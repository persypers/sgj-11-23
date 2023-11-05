using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    public string message = "Sample Text";
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

        if (bubbleTextManager != null)
        {
            int randomNumber = Random.Range(1, 101);
            string randomString = randomNumber.ToString();
            bubbleTextManager.OnAddMessageToQueue(new BubbleTextMessage { messageText = randomString, showDuration = 2, target = gameObject, messageIntonation = MessageIntonations.Cry });
        }
    }

    void OnTriggerExit(Collider other)
    {

    }
}
