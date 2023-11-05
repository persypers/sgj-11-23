using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : GrabItemBehaviour
{
    public float ballThrowDelay = 3;
    public float throwForce = 30;
    private bool hasBall = false;

    public override IEnumerator DoBehaviour(Item item)
    {
        yield return base.DoBehaviour(item);
        hasBall = true;
        StartCoroutine(AsyncThrowBall(item));
    }
    public override void OnBehaviourCanceled(Item item)
    {
        hasBall = false;
    }

    void Update()
    {

    }

    private IEnumerator AsyncThrowBall(Item ball)
    {
        yield return new WaitForSeconds(ballThrowDelay);
        if (hasBall)
        {
            Vector3 throwDirection = transform.forward + new Vector3(0, 0.5f, 0);
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
            DropItem(ball);
            ballRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            hasBall = false;
            Debug.Log("Ball thrown");
        }
    }
}
