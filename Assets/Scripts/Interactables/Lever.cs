using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
	public bool startingValue;
	public float angle = 30.0f;
	public bool IsOn { get; private set; }
	public HingeJoint hinge;

	public UnityEvent OnOn;
	public UnityEvent OnOff;
	// Start is called before the first frame update
	void Start()
	{
		hinge.useLimits = true;
		var limits = hinge.limits;
		limits.min = -angle;
		limits.max = angle;
		hinge.limits = limits;

		IsOn = !startingValue;		// to trigger OnSwitched
		Set( startingValue );
	}

	public void Set( bool value )
	{
		if( value == IsOn )
			return;
		IsOn = value;
		var spring = hinge.spring;
		spring.targetPosition = IsOn ? angle : -angle;
		hinge.spring = spring;

		OnSwitched();
	}

	virtual public void OnSwitched()
	{
		if( IsOn )
			OnOn.Invoke();
		else
			OnOff.Invoke();
	}
}
