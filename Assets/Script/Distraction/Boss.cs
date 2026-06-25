using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {
	[SerializeField] private float start_y = 10;
	private float end_y = 0;

	public bool can_tween = false;
	public bool is_tween_reversed = false;

	void Update() {
		if (can_tween) {
			if (!is_tween_reversed) {
				transform.position = Vector2.Lerp(transform.position, new Vector2(0, end_y), 10f * Time.deltaTime);
				if (transform.position.y < end_y + 0.02) {
					transform.position = new Vector2(0, end_y);
					can_tween = false;
					StartCoroutine(DespawnBoss());
				}
			} else {
				transform.position = Vector2.Lerp(transform.position, new Vector2(0, start_y), 10f * Time.deltaTime);
				if (transform.position.y > start_y - 0.02) {
					transform.position = new Vector2(0, start_y);
					can_tween = false;
					DistractionManager.Instance.ShowPaper();
				}
			}
		}
	}

	IEnumerator DespawnBoss() {
		yield return new WaitForSeconds(1f);
		can_tween = true;
		is_tween_reversed = true;
	}
}
