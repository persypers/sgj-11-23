using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSpecchThrow : GrabItemBehaviour
{
    public float itemThrowDelay = 3;
    public float throwForce = 30;
	public string newStateName = "Magazine"; // The new speech state name when NPC gets an item

    public override IEnumerator DoBehaviour(Item item)
    {
        yield return base.DoBehaviour(item);

		var playerCollisionMessageTrigger = GetComponentInChildren< PlayerCollisionMessageTrigger >();
		if (playerCollisionMessageTrigger != null)
			playerCollisionMessageTrigger.SetCurrentState(newStateName);

		yield return new WaitForSeconds(itemThrowDelay);

		if (playerCollisionMessageTrigger != null)
			playerCollisionMessageTrigger.SetCurrentState("Default"); // Change "Default" to the appropriate default speech stat

		Vector3 throwDirection = transform.forward + new Vector3(0, 0.5f, 0);
		Rigidbody ballRigidbody = item.GetComponent<Rigidbody>();
		DropItem(item);
		ballRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
		Debug.Log("Item thrown");
	}

    public override void OnBehaviourCanceled(Item item)
    {
		var playerCollisionMessageTrigger = GetComponentInChildren< PlayerCollisionMessageTrigger >();
        // Reset the speech state if the behavior is canceled
        if (playerCollisionMessageTrigger != null)
        {
            playerCollisionMessageTrigger.SetCurrentState("Default"); // Change "Default" to the appropriate default speech state
        }
	}

}
