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
			StartCoroutine("SwitchLightAuto");
		}
	}

	IEnumerator SwitchLightAuto(){
		for(;;){
			SwitchLight();
			_delay = Random.Range (3, 7);
			yield return new WaitForSeconds(_delay);
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
	}

	void OnMouseUp() {
		if (Automaic)
			return;
		SwitchLight ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.GameOver) {
			if (CurrentState == LightState.Green) SwitchLight();
			//if(Automaic) {
				//Debug.Log("GAME OVER - STOPPING COROUTINE");
				//StopCoroutine("SwitchLightAuto");
			//}
		}
	}
}
