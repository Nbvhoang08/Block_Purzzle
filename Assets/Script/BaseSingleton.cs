using UnityEngine;
using System;

	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		[SerializeField] bool dontDestroyOnload;
		private static T _instance;

		private static bool _applicationIsQuitting;

		public static T Instance
		{
			get
			{
				if (_applicationIsQuitting)
					return null;
				return _instance;
			}
		}

		public static bool IsInstanceValid()
		{
			return (_instance != null);
		}

		//MUST OVERRIDE AWAKE AT CHILD CLASS
		public virtual void Awake()
		{
			if (_instance != null)
			{
				GameObject.Destroy(this.gameObject);
				return;
			}

			if (_instance == null)
				_instance = (T)(MonoBehaviour)this;

			if (_instance == null)
			{
				Debug.LogError("Awake xong van NULL " + typeof(T));
			}
		//Debug.LogError("Awake of " + typeof(T));
		if (dontDestroyOnload)
				DontDestroyOnLoad(this);
		}

		protected virtual void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
			}
		}


		private void OnApplicationQuit()
		{
			_applicationIsQuitting = true;
		}
	}
