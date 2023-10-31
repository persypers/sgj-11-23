using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Utils.Events
{
	public abstract class HandlerBase
	{
		public static bool LogsEnabled
		{
			get { return false; }
		}

		public static bool AllFireLogs
		{
			get { return LogsEnabled; }
		}

		public List<object> Watchers { get; protected set; } = new List<object>(100);

		public virtual void CleanUp()
		{
		}

		public virtual bool FixWatchers()
		{
			return false;
		}

		protected void EnsureWatchers()
		{
			if (Watchers == null)
			{
				Watchers = new List<object>(100);
			}
		}
	}

	public class Handler<T> : HandlerBase
	{
		private Action<T>[] actionsArray = null;
		private readonly List<Action<T>> actions = new List<Action<T>>(100);
		private readonly List<Action<T>> removed = new List<Action<T>>(100);

		public void Subscribe(object watcher, Action<T> action)
		{
			if (removed.Contains(action))
			{
				removed.Remove(action);
			}

			if (!actions.Contains(action))
			{
				actionsArray = null;
				actions.Add(action);
				EnsureWatchers();
				Watchers.Add(watcher);
			}
			else if (LogsEnabled)
			{
				Debug.LogFormat("{0} tries to subscribe to {1} again.", watcher, action);
			}
		}

		public void Unsubscribe(Action<T> action)
		{
			SafeUnsubscribe(action);
		}

		private void SafeUnsubscribe(Action<T> action)
		{
			var index = actions.IndexOf(action);
			SafeUnsubscribe(index);
			if (index < 0 && LogsEnabled)
			{
				Debug.LogFormat("Trying to unsubscribe action {0} without watcher.", action);
			}
		}

		private void SafeUnsubscribe(int index)
		{
			if (index >= 0)
			{
				removed.Add(actions[index]);
			}
		}

		private void FullUnsubscribe(int index)
		{
			if (index >= 0)
			{
				actionsArray = null;
				actions.RemoveAt(index);
				if (index < Watchers.Count)
				{
					Watchers.RemoveAt(index);
				}
			}
		}

		private void FullUnsubscribe(Action<T> action)
		{
			var index = actions.IndexOf(action);
			FullUnsubscribe(index);
		}

		public void Fire(T arg)
		{
			Profiler.BeginSample("EventManager.Handler.Fire.CopyArray");
			if (actionsArray == null) actionsArray = actions.ToArray();
			Profiler.EndSample();
			foreach (var current in actionsArray)
			{
				if (!removed.Contains(current)) current.Invoke(arg);
			}

			CleanUp();
			if (AllFireLogs)
			{
				Debug.LogFormat("[{0}] fired (Listeners: {1})", typeof(T).Name, Watchers.Count);
			}
		}

		public override void CleanUp()
		{
			Profiler.BeginSample("EventManager.HAndler.CleanUp");
			foreach (var item in removed)
			{
				FullUnsubscribe(item);
			}

			removed.Clear();
			Profiler.EndSample();
		}

		public override bool FixWatchers()
		{
			CleanUp();
			int count = 0;
			EnsureWatchers();
			for (var i = 0; i < Watchers.Count; i++)
			{
				var watcher = Watchers[i];
				if (watcher is MonoBehaviour comp)
				{
					if (!comp)
					{
						SafeUnsubscribe(i);
						count++;
					}
				}
			}

			if (count > 0)
			{
				CleanUp();
			}

			if (count > 0 && LogsEnabled)
			{
				Debug.LogFormat("{0} destroyed scripts subscribed to event {1}.", count, typeof(T));
			}

			return count == 0;
		}
	}
}