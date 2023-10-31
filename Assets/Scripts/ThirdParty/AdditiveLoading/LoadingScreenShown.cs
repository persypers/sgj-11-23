using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fancy
{

public class LoadingScreenShown : StateMachineBehaviour
{
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.GetComponent<LoadingScreen>().OnShown();
	}
}
}
