using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T:MonoBehaviour {

	private static T _instance;

	public static T Instance{
		get{
			if(_instance == null){
				_instance = GameObject.FindObjectOfType<T>();

				if(_instance == null){
					GameObject singleton = new GameObject(typeof(T).Name);
					_instance = singleton.AddComponent<T>();
				}
			}
			return _instance;
		}
	}

	public virtual void Awake(){
		if (_instance == null){
			_instance = this as T;
			DontDestroyOnLoad(gameObject);
		}else{
			Destroy (gameObject);
		}
	}

}
