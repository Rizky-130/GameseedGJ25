using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistractionBubble : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI speech_text;
	[SerializeField] private GameObject close_button;

	private bool start = false;
	private bool end = false;
	public string text_to_display = "Test text text text text text text text text text text";
	[SerializeField] private float time_between_characters = 0.05f;

	void Start() {
		this.transform.localScale = Vector2.zero;
		close_button.SetActive(false);
		speech_text.text = "";
	}

	void Update() {
		// Tween scale start
		if (!start) {
			this.transform.localScale = Vector2.Lerp(this.transform.localScale, Vector2.one, 0.05f);
		}
		if (this.transform.localScale.x > 0.99f && !start) {
			this.transform.localScale = Vector2.one;
			close_button.SetActive(true);
			if (speech_text) {
				speech_text.text = text_to_display;
				StartCoroutine(TextVisible());
			}
			start = true;
		}

		// Tween scale end
		if (end) {
			this.transform.localScale = Vector2.Lerp(this.transform.localScale, Vector2.zero, 0.05f);
		}
		if (this.transform.localScale.x < 0.01f && end) {
			Destroy(this.gameObject);
		}
	}

	private IEnumerator TextVisible() {
		speech_text.ForceMeshUpdate();
		int total_characters = speech_text.textInfo.characterCount;
		int counter = 0;

		while (true) {
			int visible_count = counter % (total_characters + 1);
			speech_text.maxVisibleCharacters = visible_count;

			if (visible_count >= total_characters) {
				break;
			}

			counter += 1;
			yield return new WaitForSeconds(time_between_characters);
		}
	}

	public void CloseBubble() {
		Destroy(close_button.gameObject);
		Destroy(speech_text.gameObject);
		end = true;
	}
}
