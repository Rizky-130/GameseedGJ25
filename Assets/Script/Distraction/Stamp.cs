using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour {
	[SerializeField] private float x_size = 7.85f;
	[SerializeField] private float y_size = 4f;

	private AudioSource audio_source;
	private Vector3 mouse_position_offset;
	private bool can_stamp = false;
	private bool is_offscreen = false;
	public bool can_drag = false;
	public Vector3 start_pos;

	void Start() {
		audio_source = GetComponent<AudioSource>();
	}

	private Vector3 GetMouseWorldPosition() {
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	private void OnMouseDown() {
		if (can_drag) {
			mouse_position_offset = transform.position - GetMouseWorldPosition();
		}
	}

	private void OnMouseDrag() {
		if (can_drag) {
			Vector3 unclamped = GetMouseWorldPosition() + mouse_position_offset;
			transform.position = new Vector3(Mathf.Clamp(unclamped.x, -x_size, x_size), Mathf.Clamp(unclamped.y, -y_size, y_size), unclamped.z);
		}
	}

	private void OnMouseUp() {
		if (can_stamp) {
			can_stamp = false;
			can_drag = false;
			DistractionManager.Instance.Stamp();
			audio_source.Play();
		}
		if (is_offscreen && can_drag) {
			transform.localPosition = start_pos;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.name == "DistractionPaper") {
			can_stamp = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.name == "DistractionPaper") {
			can_stamp = false;
		}
	}

	private void OnBecameInvisible() {
		is_offscreen = true;
	}

	private void OnBecameVisible() {
		can_drag = true;
		is_offscreen = false;
	}
}
