using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractionManager : MonoBehaviour {
	public static DistractionManager Instance { get; private set; }

	[SerializeField] private GameObject bubble_prefab;
	[SerializeField] private GameObject distraction_paper;
	[SerializeField] private GameObject stamp;
	[SerializeField] private float time_until_game_over = 30;//s

	private bool can_tween = false;
	private bool is_tween_reversed = false;
	private float start_y = 10;
	private float end_y = 0;
	private Stamp st;
	private float time_remaining = 30;
	private bool is_paper_shown = false;

	void Awake() {
		Instance = this;
	}

	void Start() {
		if (!distraction_paper) {
			distraction_paper = GameObject.Find("DistractionPaper");
		}
		distraction_paper.transform.position = new Vector2(0, start_y);
		if (stamp) {
			st = stamp.GetComponent<Stamp>();
		}
	}

	void Update() {
		if (can_tween) {
			if (!is_tween_reversed) {
				st.can_drag = true;
				distraction_paper.transform.position = Vector2.Lerp(distraction_paper.transform.position, new Vector2(0, end_y), 5f * Time.deltaTime);
				if (distraction_paper.transform.position.y < end_y + 0.02) {
					distraction_paper.transform.position = new Vector2(0, end_y);
					can_tween = false;
					time_remaining = time_until_game_over;
					is_paper_shown = true;
				}
			} else {
				distraction_paper.transform.position = Vector2.Lerp(distraction_paper.transform.position, new Vector2(0, start_y), 5f * Time.deltaTime);
				if (distraction_paper.transform.position.y > start_y - 0.02) {
					distraction_paper.transform.position = new Vector2(0, start_y);
					st.transform.localPosition = st.start_pos;
					can_tween = false;
				}
			}
		}

		if (is_paper_shown) {
			time_remaining -= Time.deltaTime;
			if (time_remaining < 0) {
				Debug.Log("FUCCCCCCCCCCCCCKKKKKKKKKKKKKKKKKKKKKK");
			}
		}
	}

	public void SpawnBubble(string text, Vector2 position) {
		GameObject bubble = Instantiate(bubble_prefab, position, Quaternion.identity);
		bubble.GetComponent<DistractionBubble>().text_to_display = text;
	}

	public void ShowPaper() {
		can_tween = true;
		is_tween_reversed = false;
	}

	public void HidePaper() {
		can_tween = true;
		is_tween_reversed = true;
		is_paper_shown = false;
	}
}
