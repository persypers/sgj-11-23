using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    public ConfigurableJoint ballJoint;
    public ItemTrigger ballTrigger;
    ConfigurableJoint joint;
    public float ballThrowDelay = 3;
    public float throwForce = 30;
    private bool hasBall = false;
    void Start()
    {
        ballTrigger.actionWithItem.AddListener(PickBall);
    }

    void Update()
    {

    }

    void PickBall(Item ball)
    {
        if (!joint && !hasBall)
        {
            joint = Fancy.Helpers.CopyComponent(ballJoint, ball.gameObject);
            hasBall = true;
            ball.OnPickUp.AddListener(DropBall);
            StartCoroutine(AsyncThrowBall(ball));
        }
    }
    private IEnumerator AsyncThrowBall(Item ball)
    {
        yield return new WaitForSeconds(ballThrowDelay);
        if (hasBall)
        {
            Vector3 throwDirection = transform.forward + new Vector3(0, 0.5f, 0);
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
            ballRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            Debug.Log("Ball thrown");
            DropBall(ball);
        }
    }

    void DropBall(Item ball)
    {
        joint.GetComponent<Item>().OnPickUp.RemoveListener(DropBall);
        GameObject.Destroy(joint);
        hasBall = false;
    }
}
