using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TEMP
public class DistractionCallerExamples : MonoBehaviour {
	void Update() {
		if (Input.GetKey(KeyCode.A)) {
			DistractionManager.Instance.ShowPaper();
		}
		if (Input.GetKey(KeyCode.D)) {
			DistractionManager.Instance.HidePaper();
		}
		if (Input.GetKey(KeyCode.Space)) {
			DistractionManager.Instance.SpawnBubble("asd asd asd asd asd asd", new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)));
		}
	}

	public void CallDistraction() {
		DistractionManager.Instance.SpawnBubble("asd asd asd asd asd asd", new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)));
	}
}
