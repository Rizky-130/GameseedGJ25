using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {
	[SerializeField] private float start_y = 10;
	[SerializeField] private float end_y = 0;

	private AudioSource audio_source;
	private bool can_play_jumpscare = true;
	public bool can_tween = false;
	public bool is_tween_reversed = false;

	void Start() {
		audio_source = GetComponent<AudioSource>();
	}

	void Update() {
		if (can_tween) {
			if (!is_tween_reversed) {
				transform.position = Vector2.Lerp(transform.position, new Vector2(0, end_y), 10f * Time.deltaTime);
				if (transform.position.y < start_y / 2 && can_play_jumpscare) {
					audio_source.Play();
					can_play_jumpscare = false;
				}
				if (transform.position.y < end_y + 0.02) {
					transform.position = new Vector2(0, end_y);
					can_tween = false;
					DistractionManager.Instance.StartCoroutine(DistractionManager.Instance.DespawnBoss());
				}
			} else {
				transform.position = Vector2.Lerp(transform.position, new Vector2(0, start_y), 10f * Time.deltaTime);
				if (transform.position.y > start_y - 0.02) {
					transform.position = new Vector2(0, start_y);
					can_tween = false;
					DistractionManager.Instance.ShowPaper();
					can_play_jumpscare = true;
				}
			}
		}
	}
}
