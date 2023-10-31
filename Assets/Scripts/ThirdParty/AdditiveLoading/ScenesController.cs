using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fancy
{
	public class ScenesController : MonoSingleton< ScenesController >
	{
		private string _nextScene = "Entry";
		public Fancy.LoadingScreen screen;
		public string firstScene = "Level1";

		public void Start()
		{
			GameObject.DontDestroyOnLoad(screen.gameObject);
			screen.gameObject.SetActive(false);

			LoadScene( firstScene );
		}

		public void LoadScene(string nextSceneName, bool skipLoading = false)
		{
			string prevSceneName = _nextScene;
			_nextScene = nextSceneName;
			var async = SceneManager.LoadSceneAsync(_nextScene, LoadSceneMode.Additive);
			async.allowSceneActivation = false;
			async.completed += OnLoadingLoadComplete;
			screen.Show(() =>
			{
				//WindowsController.Instance.Clear();
				var prevScene = SceneManager.GetSceneByName(prevSceneName);
				if( prevScene.isLoaded )
				{
					var unload = SceneManager.UnloadSceneAsync(prevScene);
				}
				async.allowSceneActivation = true;
			}, skipLoading, async);
		}

		private void OnLoadingLoadComplete(AsyncOperation obj)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(_nextScene));
			screen.Hide();
			//EventManager.Fire(new SceneChanged());
		}

		public void ReloadCurrentScene(bool skipLoadingAnimation = false)
		{
			screen.Show(null, true);
			string sceneName = SceneManager.GetActiveScene().name;
			var unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			unloadOp.completed += (unload) => {
				var loadAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
				//WindowsController.Instance.Clear();
				loadAsync.completed += (load) => {screen.Hide();};
			};
		}
	}
}