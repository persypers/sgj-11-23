using UnityEngine;
using UnityEngine.UI;

namespace Fancy
{
public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private Image progressBar;
	//[SerializeField] private TMPro.TMP_Text progressLabel;
	private AsyncOperation operation;
	private Animator animator;
	private System.Action onShow;
	private System.Action onHide;
	public void Show(System.Action onDone = null, bool instant = false, AsyncOperation operation = null)
	{
		var anim = GetComponent<Animator>();
		this.onShow = onDone;
		this.operation = operation;
		gameObject.SetActive(true);
		anim.SetTrigger(instant ? "ShowInstant" : "Show");
	}

	public void Hide(System.Action onDone = null, bool instant = false)
	{
		var anim = GetComponent<Animator>();
		this.onHide = onDone;
		gameObject.SetActive(true);
		anim.SetTrigger(instant ? "HideInstant" : "Hide");
	}

	public void Update()
	{
		if(operation != null) {
			if(progressBar != null) progressBar.fillAmount = operation.progress;
/*			if(progressLabel != null) {
				int progress = Mathf.RoundToInt(operation.progress);
				if(progress != prevProgress) progressLabel.text = string.Format("{0}%", progress * 100);
				prevProgress = progress;
			}
*/
		}
	}

	public void OnShown()
	{
		if(onShow != null) onShow();
		onShow = null;
	}
	public void OnHidden()
	{
		if(onHide != null) onHide();
		onHide = null;
		gameObject.SetActive(false);
	}
}
}