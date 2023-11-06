using UnityEngine;
using SpeechText;
using System.Collections;

public class ChangeSpeechState : GrabItemBehaviour
{
    public string newStateName = "Magazine"; // The new speech state name when NPC gets an item
    private bool hasItem = false;
    public PlayerCollisionMessageTrigger playerCollisionMessageTrigger;

    public override IEnumerator DoBehaviour(Item item)
    {
        yield return base.DoBehaviour(item);

        // Check if the item is valid and the NPC does not already have an item
        if (item != null && !hasItem)
        {
            // Change the speech state directly from this script
            playerCollisionMessageTrigger.SetCurrentState(newStateName);
            hasItem = true;
        }

        // Rest of your code...
    }

    public override void OnBehaviourCanceled(Item item)
    {
        // Reset the speech state if the behavior is canceled
        if (hasItem)
        {
            playerCollisionMessageTrigger.SetCurrentState("Default"); // Change "Default" to the appropriate default speech state
            hasItem = false;
        }

        // Rest of your code...
    }

}
