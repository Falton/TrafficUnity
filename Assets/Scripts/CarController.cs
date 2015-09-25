using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour {
	public const float SkinWidth = .02f;
	public LayerMask PlatformMask;
	public float MaxSpeed = 0.5f;
	public PathDefinition Path;
	public float Speed { get { return _speed; } }
	public float MaxDistanceToGoal = .1f;
	public GameObject deathParticles;
	public int PathType{ get; set;}

	public bool EndReached{ get; private set; }
	private IEnumerator<Transform> _currentPoint;
	private int _currentWaitPoint;
	private float _speed;

	private Vector3 _raycastButtomRight;
	private Vector3 _raycastButtomLeft;
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;

	private int _myScore;
	// Use this for initialization
	void Start () {
		EndReached = false;
		_currentWaitPoint = 0;
		_speed = MaxSpeed;
		_myScore = 600;
		_transform = transform;
		_localScale = transform.localScale;
		_boxCollider = GetComponent<BoxCollider2D> ();
	}

	public void initPath(PathDefinition path){
		if (path == null) {
			Debug.LogError("Path cannot be null", gameObject);
			return;
		}
		Path = path;
		_currentPoint = Path.GetPathsEnumerator ();
		_currentPoint.MoveNext ();
		
		if (_currentPoint.Current == null)
			return;
		
		transform.position = _currentPoint.Current.position;
	}
	

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag != gameObject.tag && other.gameObject.tag != "Light") {
			Debug.Log ("EXPLODE -- GAME OVER other:" + other.gameObject.tag);
			GameManager.GameOver = true;
			Instantiate(deathParticles, transform.position,Quaternion.Euler(0,0,0));
		}
			
	}
		
	// Update is called once per frame
	void LateUpdate () {
		if (_currentPoint == null || _currentPoint.Current == null || EndReached ) {
			return;
		}
		_speed = MaxSpeed;
		CalculateRayOrigins ();
		var deltaMovement = Time.deltaTime * _speed;
		CalculateSpeed (ref deltaMovement);
		transform.position = Vector3.MoveTowards (transform.position, _currentPoint.Current.position, deltaMovement);

		if (deltaMovement == 0 && _myScore>0) {
			_myScore--;
		}

		var distanceSquared = (transform.position -  _currentPoint.Current.position).sqrMagnitude;
		if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal && _currentPoint.Current != Path.Points [Path.Points.Length - 1]){
			if (Path.DependantPoints.Length != 0 && _currentWaitPoint<Path.DependantPoints.Length && Path.DependantPoints [_currentWaitPoint] == _currentPoint.Current && Path.Lights [_currentWaitPoint].CurrentState == LightController.LightState.Red)
				return;
			else if (Path.DependantPoints.Length != 0 && _currentWaitPoint<Path.DependantPoints.Length && Path.DependantPoints [_currentWaitPoint] == _currentPoint.Current && Path.Lights [_currentWaitPoint].CurrentState == LightController.LightState.Green)
				_currentWaitPoint++;
			_currentPoint.MoveNext ();
		}else if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal && _currentPoint.Current == Path.Points [Path.Points.Length - 1]) {
			EndReached = true;
			GameManager gameMng = GameManager.FindObjectOfType<GameManager>();
			if(Path.Lights.Length==0) _myScore =0;
			gameMng.UpdateScore(_myScore);
			//Destroy (gameObject,0f);
		}
	}

	private void CalculateRayOrigins(){
		var size = new Vector2 (_boxCollider.size.x * Mathf.Abs (_localScale.y), _boxCollider.size.y * Mathf.Abs (_localScale.y)) / 2;
		var center = new Vector2 (_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);

		_raycastButtomRight = _transform.position + new Vector3 (center.x + size.x + SkinWidth, center.y - size.y + SkinWidth);
		_raycastButtomLeft = _transform.position + new Vector3 (center.x - size.x - SkinWidth, center.y - size.y + SkinWidth);
	}

	private void CalculateSpeed(ref float deltaMovement){
		var isGoingRight = deltaMovement > 0;
		var rayDistance = Mathf.Abs (deltaMovement) + SkinWidth;
		var rayOrigin = isGoingRight ? _raycastButtomLeft : _raycastButtomLeft;
		var rayDirection = isGoingRight ? new Vector3(1,0) : new Vector3(-1,0);

		var rayVector = new Vector2(rayOrigin.x,rayOrigin.y);
		Debug.DrawRay(rayVector,rayDirection*rayDistance,Color.red);

		var rayCastHit = Physics2D.Raycast( rayVector, rayDirection, rayDistance, PlatformMask);
		if (!rayCastHit) {
		
		} else {

			deltaMovement = rayCastHit.point.x - rayVector.x;
			rayDistance = Mathf.Abs (deltaMovement);
		}
	}
}
