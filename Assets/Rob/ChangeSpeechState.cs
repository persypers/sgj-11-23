using UnityEngine;
using SpeechText;
using System.Collections;

public class ChangeSpeechState : GrabItemBehaviour
{
    public string newStateName = "Magazine"; // The new speech state name when NPC gets an item
    private bool holdingItem = false;
    public PlayerCollisionMessageTrigger playerCollisionMessageTrigger;

    public override IEnumerator DoBehaviour(Item item)
    {
        yield return base.DoBehaviour(item);

        // Check if the item is valid and the NPC does not already have an item
        if (item != null && !holdingItem)
        {
            // Change the speech state directly from this script
            playerCollisionMessageTrigger.SetCurrentState(newStateName);
            holdingItem = true;
        }

        // Rest of your code...
    }

    public override void OnBehaviourCanceled(Item item)
    {
        // Reset the speech state if the behavior is canceled
        if (holdingItem)
        {
            playerCollisionMessageTrigger.SetCurrentState("Default"); // Change "Default" to the appropriate default speech state
            holdingItem = false;
        }

        // Rest of your code...
    }

}
