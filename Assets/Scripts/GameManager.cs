using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : Singleton<GameManager> {
	
	public GameObject[] carsPreFabs;
	List<PathDefinition> Paths = new List<PathDefinition>();
	public bool GameOver = false;
	public int MAX_CARS = 6;
	public bool ShowUI;
	public GUISkin skin;

	private List<CarController> _cars;
	private int _nextPathType;
	private float _delay;
	private static int _score;
	private bool _gameInSession;
	private int[] _carsOnPath;

	// Use this for initialization
	void Start () {
		Debug.Log("GameManager::Start");
		//DontDestroyOnLoad (gameObject);
		_cars = new List<CarController> ();
		_score = 0;
		_gameInSession = false;
		initLevel ();
	}

	public void addPath(PathDefinition path){
		Debug.Log("GameManager::addPath");
		Paths.Add (path);
	}

	public void initLevel() {
		Debug.Log("GameManager::initLevel Paths.Count"+Paths.Count);
		_carsOnPath =  new int[Paths.Count];
		this.startLevel ();
	}

	public void OnGUI () { 
		GUI.skin = skin;
		if(ShowUI)GUI.Label (new Rect (45, 25, 400, 80), new GUIContent("Score:" + _score));

		if (GameOver && ShowUI) {
			if (GUI.Button (new Rect ((Screen.width-400)/2, 150, 400, 100), "Replay")) {
				Application.LoadLevel(1);
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
		SpawnCar (0);
		SpawnCar (1);
		_delay = 1f;
		_nextPathType = Random.Range (0, Paths.Count);
		Invoke ("SpawnDelay", _delay);
	}

	private void SpawnDelay(){
		if (GameOver)
			return;
		SpawnCar (_nextPathType);
		_delay = Random.Range (80, 200) / 100;
		_nextPathType = Random.Range (0, Paths.Count);
		Invoke("SpawnDelay", _delay);
	}

	private void SpawnCar (int type){
		if (Paths [type].Lights.Length > 0 && _carsOnPath [type] >= MAX_CARS) {
			return;
		}
		this._carsOnPath [type]++;
		var initialCar = Instantiate (carsPreFabs[ Paths[type].direction] );
		initialCar.GetComponent<CarController> ().initPath (Paths [type]);
		initialCar.GetComponent<CarController> ().PathType = type;
		initialCar.GetComponent<CarController> ().MaxSpeed = Random.Range (5, 50) / 10;
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
			CancelInvoke();
			var imax = _cars.Count;
			for (var i=imax-1; i>=0; i--) {

				Destroy (_cars [i].gameObject);
				_carsOnPath[ _cars[i].PathType ]--;
				_cars.Remove (_cars [i]);

			}

			_gameInSession = false;
		}
	}
}
