using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour {
	public Vector3 start_pos;

	private Vector3 mouse_position_offset;
	private bool can_stamp = false;
	public bool can_drag = false;
	private bool is_offscreen = false;

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
			transform.position = GetMouseWorldPosition() + mouse_position_offset;
		}
	}

	private void OnMouseUp() {
		if (can_stamp) {
			can_stamp = false;
			can_drag = false;
			DistractionManager.Instance.Stamp();
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
