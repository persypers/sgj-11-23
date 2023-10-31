using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace Utils.Events
{
	public class EventHelper : MonoBehaviour
	{
		[Serializable]
		public class EventData
		{
			public string Name = string.Empty;
			public Type Type = null;
			public List<MonoBehaviour> MonoWatchers = new List<MonoBehaviour>(100);
			public List<string> OtherWatchers = new List<string>(100);

			public EventData(Type type)
			{
				Type = type;
				Name = type.ToString();
			}
		}

		public bool AutoFill
		{
			get { return false; }
		}

		public List<EventData> Events = new List<EventData>(100);

		private readonly Dictionary<Type, string> typeCache = new Dictionary<Type, string>();
		private float cleanupTimer = 0;

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		private void OnDestroy()
		{
			EventManager.Instance = null;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			EventManager.Instance.CheckHandlersOnLoad();
		}

		private void SubscribeToLog<T>() where T : struct
		{
			EventManager.Subscribe<T>(this, OnLog);
		}

		private void OnLog<T>(T ev) where T : struct
		{
			Debug.LogFormat("Event: {0}", typeof(T));
		}

		private void Update()
		{
			TryCleanUp();
			if (AutoFill)
			{
				Fill();
			}
		}

		[ContextMenu("CheckEventHandlers")]
		public void CheckEventHandlers()
		{
			var handlers = EventManager.Instance.Handlers;
			foreach (var handler in handlers)
			{
				if (handler.Value.Watchers.Count > 0)
				{
					Debug.Log(handler.Key);
					foreach (var watcher in handler.Value.Watchers)
					{
						Debug.Log(handler.Key + " => " + watcher.GetType().ToString());
					}
				}
			}
		}

		[ContextMenu("ClearEventHandlers")]
		public void ClearEventHandlers()
		{
			EventManager.Instance.Handlers.Clear();
		}

		private void TryCleanUp()
		{
			if (cleanupTimer > EventManager.CleanUpInterval)
			{
				EventManager.Instance.CleanUp();
				cleanupTimer = 0;
			}
			else
			{
				cleanupTimer += Time.deltaTime;
			}
		}

		[ContextMenu("Fill")]
		public void Fill()
		{
			foreach (var pair in EventManager.Instance.Handlers)
			{
				var eventData = GetEventData(pair.Key);
				if (eventData == null)
				{
					eventData = new EventData(pair.Key);
					Events.Add(eventData);
				}

				FillEvent(pair.Value, eventData);
			}
		}

		private void FillEvent(HandlerBase handler, EventData data)
		{
			data.MonoWatchers.Clear();
			data.OtherWatchers.Clear();
			foreach (var item in handler.Watchers)
			{
				if (item is MonoBehaviour monoBehaviour)
					data.MonoWatchers.Add(monoBehaviour);
				else
					data.OtherWatchers.Add(item != null ? GetTypeNameFromCache(item.GetType()) : "null");
			}
		}

		private EventData GetEventData(Type type)
		{
			foreach (var item in Events)
			{
				if (item.Type == type)
					return item;
			}

			return null;
		}

		private string GetTypeNameFromCache(Type type)
		{
			string name = string.Empty;
			if (!typeCache.TryGetValue(type, out name))
			{
				name = type.ToString();
				typeCache.Add(type, name);
			}

			return name;
		}
	}
}