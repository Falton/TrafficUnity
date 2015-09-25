using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {
	
	public GameObject[] carsPreFabs;
	public PathDefinition[] Paths;
	public static bool GameOver =false;
	public int MAX_CARS = 6;
	public bool ShowUI;
	public GUISkin skin;

	private List<CarController> _cars;
	private int _nextCarType;
	private float _delay;
	private static int _score;
	private bool _gameInSession;
	private int[] _carsOnPath;

	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad (gameObject);
		_cars = new List<CarController> ();
		_score = 0;
		_gameInSession = false;
		_carsOnPath =  new int[Paths.Length];
		startLevel ();
	}

	void Awake() {
		Application.targetFrameRate = 60;
	}

	public void OnGUI () { 
		GUI.skin = skin;
		if(ShowUI)GUI.Label (new Rect (45, 25, 400, 80), new GUIContent("Score:" + _score));

		if (GameOver && ShowUI) {
			if (GUI.Button (new Rect ((Screen.width-400)/2, 150, 400, 100), "Replay")) {
				Application.LoadLevel(2);
			}
		}
	}

	public void UpdateScore(int score){
		_score += score;
		Debug.Log ("Score:" + _score);
	}

	public void startLevel(){
		if (_gameInSession)
			return;
		_gameInSession = true;
		GameOver = false;
		var imax = Paths.Length;
		for (var i=0; i<imax; i++) {
			Paths[i].Id = i;
		}
	}
	

	public void SpawnCar (int type){
		if (Paths [type].Lights.Length > 0 && _carsOnPath [type] >= MAX_CARS) {
			return;
		}
		Debug.Log ("SpawnCar: " + type.ToString());
		this._carsOnPath [type]++;
		var initialCar = Instantiate (carsPreFabs[ Paths[type].direction] );
		initialCar.GetComponent<CarController> ().initPath (Paths [type]);
		initialCar.GetComponent<CarController> ().PathType = type;
		initialCar.GetComponent<CarController> ().MaxSpeed = Random.Range (10, 50) / 10;
		_cars.Add (initialCar.GetComponent<CarController> ());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		var imax = _cars.Count;
		for (var i=imax-1; i>=0; i--) {
			if(_cars[i].EndReached){
				Destroy(_cars[i].gameObject);
				_carsOnPath[ _cars[i].PathType ]--;
				_cars.Remove(_cars[i]);
			}
		}

	}

	void Update (){
		if (GameOver) {
			//CancelInvoke();
			var imax = _cars.Count;
			for (var i=imax-1; i>=0; i--) {

				Destroy (_cars [i].gameObject);
				_carsOnPath[ _cars[i].PathType ]--;
				_cars.Remove (_cars [i]);

			}

			imax = Paths.Length;
			for(var i = 0; i < imax; i++){
				Paths[i].cancelCars();
			}
			_gameInSession = false;
		}
	}
}
