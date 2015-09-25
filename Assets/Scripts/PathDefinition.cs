using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathDefinition : MonoBehaviour {

	public enum PathType{
		Continues,
		Dependant
	}

	public Transform[] Points;
	public PathType Type;
	public int direction;
	public Transform[] DependantPoints;
	public LightController[] Lights;
	private int id;
	private float _delay;

	void Start () {
		_delay = Random.Range (2, 5);
	}

	public IEnumerator<Transform> GetPathsEnumerator(){
		if (Points == null || Points.Length < 1)
			yield break;
		
		var direction = 1;
		var index = 0;
		while (true) {
			yield return Points[index];
			
			if(Points.Length == 1) continue;
			//if(Points.Length-1 == index ) continue;
			if(index <=0) direction=1;
			else if(index >= Points.Length - 1) direction =-1;
			
			index=index+direction;
		}
	}

	public int Id {
		get {
			return id;
		}
		set {
			id = value;
			StartCoroutine("Spawn");
		}
	}

	IEnumerator Spawn() {
		for (;;) {
			GameManager gameMng = GameManager.FindObjectOfType<GameManager> ();
			gameMng.SpawnCar (Id);
			_delay = Random.Range (0.3f, 2f);
			yield return new WaitForSeconds(_delay);
		}

	}

	public void cancelCars(){
		StopCoroutine ("Spawn");
	}

	public void OnDrawGizmos(){
		if (Points == null || Points.Length < 2)
			return;
		
		for (var i = 1; i<Points.Length; i++) {
			Gizmos.DrawLine(Points[i-1].position,Points[i].position);
		}
	}
}
