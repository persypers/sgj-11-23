using UnityEngine;

namespace Fancy
{
	public abstract class MonoService : MonoBehaviour{ }
	public abstract class MonoService<T> : MonoService where T : MonoService<T> {

		static T instance;
		protected bool isInited = false;
		public static T Instance {
			get {
				if(!instance) {
					instance = Helpers.FindSceneComponent<T>(false);
				}
				if(instance && !instance.isInited)
				{
					instance.Init();
				}
				return instance;
			}
		}

		protected virtual void OnEnable()
		{
			if(instance == null) instance = this as T;
		}

		protected virtual void OnDisable()
		{
			if(instance == this)
			{
				instance = null;
				instance = Instance;
			}
		}

		protected virtual void Awake() {
			if(!isInited) {
				Init();
				isInited = true;
			}
			instance = this as T;
		}
		protected virtual void Init(){}
	}
}
