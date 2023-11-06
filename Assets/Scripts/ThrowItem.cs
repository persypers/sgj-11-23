using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : GrabItemBehaviour
{
    public float itemThrowDelay = 3;
    public float throwForce = 30;
    private bool hasItem = false;

    public override IEnumerator DoBehaviour(Item item)
    {
        yield return base.DoBehaviour(item);
        hasItem = true;
        StartCoroutine(AsyncThrowItem(item));
    }
    public override void OnBehaviourCanceled(Item item)
    {
        hasItem = false;
    }

    void Update()
    {

    }

    private IEnumerator AsyncThrowItem(Item item)
    {
        yield return new WaitForSeconds(itemThrowDelay);
        if (hasItem)
        {
            Vector3 throwDirection = transform.forward + new Vector3(0, 0.5f, 0);
            Rigidbody ballRigidbody = item.GetComponent<Rigidbody>();
            DropItem(item);
            ballRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            hasItem = false;
            Debug.Log("Item thrown");
        }
    }
}
