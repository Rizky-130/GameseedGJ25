using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractionCaller : MonoBehaviour {
	public void CallDistraction() {
		DistractionManager.Instance.SpawnBubble("asd asd asd asd asd asd", new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)));
	}
}
