using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Events
{
	public class EventManager
	{
		public const float CleanUpInterval = 10.0f;

		private static EventManager _instance;

		public static EventManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new EventManager();
					_instance.AddHelper();
				}

				return _instance;
			}
			internal set
			{
				_instance = value;
			}
		}

		public Dictionary<Type, HandlerBase> Handlers { get; } = new Dictionary<Type, HandlerBase>(100);

		// Static helpers:
		// **********
		public static void Subscribe<T>(object watcher, Action<T> action) where T : struct
		{
			Instance.Sub(watcher, action);
		}

		public static void Unsubscribe<T>(Action<T> action) where T : struct
		{
			if (_instance != null)
			{
				Instance.Unsub(action);
			}
		}

		public static System.Collections.IEnumerator WaitFor<T>(Predicate<T> filter = null) where T : struct
		{
			bool hasFired = false;
			Action<T> handler = (ev) =>
			{
				if (filter == null) hasFired = true;
				else if (filter(ev)) hasFired = true;
			};
			Instance.Sub(handler, handler);
			while (!hasFired) yield return null;
			Instance.Unsub(handler);
		}

		public static void Fire<T>(T args) where T : struct
		{
			Instance.FireEvent(args);
		}

		public static bool HasWatchers<T>() where T : struct
		{
			return Instance.HasWatchersDirect<T>();
		}
		// **********

		private void Sub<T>(object watcher, Action<T> action)
		{
			if (!Handlers.TryGetValue(typeof(T), out var handler))
			{
				handler = new Handler<T>();
				Handlers.Add(typeof(T), handler);
			}

			(handler as Handler<T>)?.Subscribe(watcher, action);
		}

		private void Unsub<T>(Action<T> action)
		{
			if (Handlers.TryGetValue(typeof(T), out var handler))
			{
				(handler as Handler<T>)?.Unsubscribe(action);
			}
		}

		private void FireEvent<T>(T args)
		{
			if (!Handlers.TryGetValue(typeof(T), out var handler))
			{
				handler = new Handler<T>();
				Handlers.Add(typeof(T), handler);
			}

			(handler as Handler<T>)?.Fire(args);
		}

		private bool HasWatchersDirect<T>() where T : struct
		{
			if (Handlers.TryGetValue(typeof(T), out var container))
			{
				return (container.Watchers.Count > 0);
			}

			return false;
		}

		private void AddHelper()
		{
			var go = new GameObject("[EventHelper]");
			go.AddComponent<EventHelper>();
		}

		public void CheckHandlersOnLoad()
		{
			foreach (var item in Handlers)
			{
				item.Value.FixWatchers();
			}
		}

		public void CleanUp()
		{
			foreach (var item in Handlers)
			{
				item.Value.CleanUp();
			}
		}
	}
}