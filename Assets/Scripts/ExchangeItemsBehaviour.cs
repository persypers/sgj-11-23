using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeItemsBehaviour : GrabItemBehaviour
{
    public float exchangeDelay;
    public Item returnExchangeItem;
    public override IEnumerator DoBehaviour(Item item)
    {
        yield return base.DoBehaviour(item);
        var playerCollisionMessageTrigger = GetComponentInChildren<PlayerCollisionMessageTrigger>();
        if (item != null)
        {
            BubbleTextManager.Instance.DropNPCMessagesQueue(gameObject);
            if (playerCollisionMessageTrigger != null)
                playerCollisionMessageTrigger.SetCurrentState(item.type.ToString());
        }
        var textLines = playerCollisionMessageTrigger.textDataAsset.GetTextLinesByStateName(item.type.ToString());
        BubbleTextManager.Instance.DropNPCMessagesQueue(gameObject);
        BubbleTextManager.Instance.OnAddMessageToQueue(
                    new BubbleTextMessage
                    {
                        messageText = textLines[Random.Range(0, textLines.Count)].text,
                        showDuration = playerCollisionMessageTrigger.showDuration,
                        target = gameObject
                    });
        yield return new WaitForSeconds(exchangeDelay);
        OnItemDestroy(item, playerCollisionMessageTrigger);
    }

    private void OnItemDestroy(Item item, PlayerCollisionMessageTrigger playerCollisionMessageTrigger)
    {
        if (ParticlesManager.Instance != null)
            ParticlesManager.Instance.ShowParticlesOnTarget(item.gameObject, 3);
        GameObject.Destroy(item.gameObject);
        Debug.Log($"{item.type} destroyed");
        if (returnExchangeItem != null)
        {
            Instantiate(returnExchangeItem, item.transform.position, item.transform.rotation);
        }
        playerCollisionMessageTrigger.SetCurrentState("Default");
    }
}
