using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistractionBubble : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI speech_text;
	[SerializeField] private GameObject close_button;
	[SerializeField] private AudioClip distracting_sound;
	[SerializeField] private Sprite bubble_1_sprite;
	[SerializeField] private Sprite bubble_2_sprite;
	[SerializeField] private Sprite bubble_3_sprite;
	[SerializeField] private Vector2[] bubble_1_pos;
	[SerializeField] private Vector2[] bubble_2_pos;
	[SerializeField] private Vector2[] bubble_3_pos;
	public bool resume_coroutine = false;
	private bool is_running = false;
	private int counter = 0;
	private AudioSource audio_source;
	private bool start = false;
	private bool end = false;
	public PauseMenuManager pause_manager;
	public string text_to_display = "Test text text text text text text text text text text";
	[SerializeField] private float time_between_characters = 0.05f;

	void Start() {
		this.transform.localScale = Vector2.zero;
		close_button.SetActive(false);
		speech_text.text = "";
		DistractionManager.Instance.bubble_count++;
		audio_source = GetComponent<AudioSource>();
		audio_source.PlayOneShot(distracting_sound);
		if (GetComponent<SpriteRenderer>().sprite == bubble_1_sprite) {
			transform.position = bubble_1_pos[Random.Range(0, bubble_1_pos.Length)];
		} else if (GetComponent<SpriteRenderer>().sprite == bubble_2_sprite) {
			transform.position = bubble_2_pos[Random.Range(0, bubble_2_pos.Length)];
		} else {
			transform.position = bubble_3_pos[Random.Range(0, bubble_3_pos.Length)];
		}
	}

	void Update() {
		if (pause_manager.isPaused) {
			if (is_running) {
				resume_coroutine = true;
			}
			gameObject.SetActive(false);
		}

		// Tween scale start
		if (!start) {
			this.transform.localScale = Vector2.Lerp(this.transform.localScale, Vector2.one, 8f * Time.deltaTime);
			// this.transform.localScale = Vector2.MoveTowards(this.transform.localScale, Vector2.one, 5f * Time.deltaTime);
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
			this.transform.localScale = Vector2.Lerp(this.transform.localScale, Vector2.zero, 8f * Time.deltaTime);
			// this.transform.localScale = Vector2.MoveTowards(this.transform.localScale, Vector2.zero, 5f * Time.deltaTime);
		}
		if (this.transform.localScale.x < 0.01f && end) {
			Destroy(this.gameObject);
		}
	}

	public IEnumerator TextVisible() {
		is_running = true;
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

		is_running = false;
	}

	public void CloseBubble() {
		audio_source.Play();
		Destroy(close_button.gameObject);
		Destroy(speech_text.gameObject);
		end = true;
		DistractionManager.Instance.bubbles.Remove(gameObject);
		DistractionManager.Instance.bubble_count--;
	}
}
