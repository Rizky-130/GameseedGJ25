using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistractionManager : MonoBehaviour {
	public static DistractionManager Instance { get; private set; }

	[SerializeField] private GameObject bubble_prefab;
	[SerializeField] private GameObject distraction_paper;
	[SerializeField] private GameObject boss;

	[Header("Paper")]
	[SerializeField] private float time_until_game_over = 30;//s
	[SerializeField] private float paper_start_y = 10;
	[SerializeField] private float paper_min_time = 8;//s
	[SerializeField] private float paper_max_time = 30;//s
	[SerializeField] private float paper_min_max_time = 12;//s

	[Header("Bubble")]
	[SerializeField] private string[] speeches;
	[SerializeField] private float bubble_min_time = 4;//s
	[SerializeField] private float bubble_max_time = 15;//s
	[SerializeField] private float bubble_min_max_time = 6;//s
	[SerializeField] private int max_bubbles = 6;
	public int bubble_count = 0;

	private GameObject stamped;
	private GameObject timer_bg;
	private GameObject timer_fill;
	private Stamp stamp;
	private bool can_tween = false;
	private bool is_tween_reversed = false;
	private float end_y = 0;
	private float time_remaining = 30;
	private bool is_paper_shown = false;

	void Awake() {
		Instance = this;
	}

	void Start() {
		if (!distraction_paper) {
			distraction_paper = GameObject.Find("DistractionPaper");
		}
		stamp = distraction_paper.transform.GetChild(0).GetComponent<Stamp>();
		timer_bg = distraction_paper.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
		timer_fill = timer_bg.transform.GetChild(0).gameObject;
		stamped = distraction_paper.transform.GetChild(2).gameObject;
		distraction_paper.transform.position = new Vector2(0, paper_start_y);

		StartCoroutine(StartSpawningDistractions());
	}

	IEnumerator StartSpawningDistractions() {
		yield return new WaitForSeconds(45);
		StartCoroutine(WaitForBoss());
		StartCoroutine(SpawnDistraction());
	}

	void Update() {
		if (can_tween) {
			if (!is_tween_reversed) {
				distraction_paper.transform.position = Vector2.Lerp(distraction_paper.transform.position, new Vector2(0, end_y), 5f * Time.deltaTime);
				if (distraction_paper.transform.position.y < end_y + 0.02) {
					distraction_paper.transform.position = new Vector2(0, end_y);
					stamp.can_drag = true;
					can_tween = false;
					time_remaining = time_until_game_over;
					is_paper_shown = true;
				}
			} else {
				distraction_paper.transform.position = Vector2.Lerp(distraction_paper.transform.position, new Vector2(0, paper_start_y), 5f * Time.deltaTime);
				if (distraction_paper.transform.position.y > paper_start_y - 0.02) {
					distraction_paper.transform.position = new Vector2(0, paper_start_y);
					stamp.transform.localPosition = stamp.start_pos;
					can_tween = false;
				}
			}
		}

		if (is_paper_shown) {
			time_remaining -= Time.deltaTime;
			if (time_remaining < 0) {
				// TEMP
				Debug.Log("ADD GAMEOVER HERE");
			}
		}

		timer_fill.GetComponent<RectTransform>().sizeDelta = new Vector2(
			timer_fill.GetComponent<RectTransform>().sizeDelta.x,
			time_remaining / time_until_game_over * timer_bg.GetComponent<RectTransform>().sizeDelta.y
		);
	}

	public void SpawnBubble(string text, Vector2 position) {
		GameObject bubble = Instantiate(bubble_prefab, position, Quaternion.identity);
		bubble.GetComponent<DistractionBubble>().text_to_display = text;
		int index = Random.Range(1, 7);
		bubble.GetComponent<SpriteRenderer>().sortingOrder = index;
		bubble.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = index;
		StartCoroutine(SpawnDistraction());
	}

	public void SummonBoss() {
		boss.GetComponent<Boss>().can_tween = true;
		boss.GetComponent<Boss>().is_tween_reversed = false;
	}

	public void ShowPaper() {
		stamped.SetActive(false);
		can_tween = true;
		is_tween_reversed = false;
	}

	public void Stamp() {
		stamped.SetActive(true);
		stamp.transform.localPosition = stamp.start_pos;
		StartCoroutine(HidePaper());
	}

	public IEnumerator HidePaper() {
		is_paper_shown = false;
		yield return new WaitForSeconds(1f);
		can_tween = true;
		is_tween_reversed = true;
		time_remaining = time_until_game_over;
		StartCoroutine(WaitForBoss());
	}

	IEnumerator WaitForBoss() {
		float delay = Random.Range(paper_min_time, Mathf.Max(paper_max_time - (0.36f * GameManager.Instance.customerServed), paper_min_max_time));
		yield return new WaitForSeconds(delay);
		SummonBoss();
	}

	IEnumerator SpawnDistraction() {
		float delay = Random.Range(bubble_min_time, Mathf.Max(bubble_max_time - (0.18f * GameManager.Instance.customerServed), bubble_min_max_time));
		yield return new WaitForSeconds(delay);
		if (bubble_count < max_bubbles) {
			SpawnBubble(speeches[Random.Range(0, speeches.Length - 1)], new Vector2(Random.Range(-7f, 7f), Random.Range(-4f, 4f)));
		} else {
			StartCoroutine(SpawnDistraction());
		}
	}
}
