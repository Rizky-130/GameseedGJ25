using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractionManager : MonoBehaviour {
	public static DistractionManager Instance { get; private set; }
	public GameObject bubble_prefab;

	private void Awake() {
		Instance = this;
	}

	public void SpawnBubble(string text, Vector2 position) {
		GameObject bubble = Instantiate(bubble_prefab, position, Quaternion.identity);
		bubble.GetComponent<DistractionBubble>().text_to_display = text;
	}
}
