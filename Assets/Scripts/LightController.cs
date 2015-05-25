using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

	public enum LightState{
		Red,
		Green
	}
	public LightState CurrentState;
	public bool Automaic = false;

	private SpriteRenderer _greenLight;
	private SpriteRenderer _redLight;
	private float _delay;
	// Use this for initialization
	void Start () {
		_greenLight = transform.Find ("GreenLight").gameObject.GetComponent<SpriteRenderer> ();
		_redLight = transform.Find ("RedLight").gameObject.GetComponent<SpriteRenderer> ();

		_greenLight.enabled = false;
		if (Automaic) {
			_delay = Random.Range (5, 10);
			Invoke ("SwitchLight", _delay);
		}
	}

	private void SwitchLight(){
		if (CurrentState == LightState.Green) {
			CurrentState = LightState.Red;
			_greenLight.enabled = false;
			_redLight.enabled = true;
		} else {
			CurrentState = LightState.Green;
			_greenLight.enabled = true;
			_redLight.enabled = false;
		}

		if (Automaic) {
			_delay = Random.Range (5, 10);
			Invoke ("SwitchLight", _delay);
		}
	}

	void OnMouseUp() {
		if (Automaic)
			return;
		SwitchLight ();
		Debug.Log("LightController::OnMouseUp()");
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.GameOver) {
			if (CurrentState == LightState.Green) SwitchLight();
			if(Automaic) CancelInvoke();
		}
	}
}
