using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistractionManager : MonoBehaviour {
	public static DistractionManager Instance { get; private set; }

	[SerializeField] private GameObject[] bubble_prefabs;
	[SerializeField] private GameObject distraction_paper;
	[SerializeField] private GameObject boss;

	[SerializeField] private int p1_customers_upper_bound = 15;
	[SerializeField] private int p2_customers_upper_bound = 30;

	[Header("Paper")]
	[SerializeField] private float time_until_game_over = 30;//s
	[SerializeField] private float time_remaining = 30;
	[SerializeField] private float paper_start_y = 10;

	[SerializeField] private float p1_paper_min_time = 15;//s
	[SerializeField] private float p1_paper_max_time = 30;//s
	[SerializeField] private float p2_paper_min_time = 12;//s
	[SerializeField] private float p2_paper_max_time = 28;//s
	[SerializeField] private float p3_paper_min_time = 9;//s
	[SerializeField] private float p3_paper_max_time = 22;//s

	[Header("Bubble")]
	[SerializeField] private string[] speeches;
	[SerializeField] private float bubble_min_x_distance = 2f;
	[SerializeField] private float bubble_min_y_distance = 1.5f;
	[SerializeField] private float p1_bubble_min_time = 10;//s
	[SerializeField] private float p1_bubble_max_time = 20;//s
	[SerializeField] private float p2_bubble_min_time = 8;//s
	[SerializeField] private float p2_bubble_max_time = 18;//s
	[SerializeField] private float p3_bubble_min_time = 7;//s
	[SerializeField] private float p3_bubble_max_time = 18;//s
	[SerializeField] private int max_bubbles = 6;
	public int bubble_count = 0;

	private AudioSource audio_source;
	private GameObject stamped;
	private GameObject timer_bg;
	private GameObject timer_fill;
	private bool can_play_on_enter = true;
	private bool can_play_on_exit = true;
	private Stamp stamp;
	private bool can_tween = false;
	private bool is_tween_reversed = false;
	private float end_y = 0;
	private bool is_paper_shown = false;

	void Awake() {
		Instance = this;
	}

	void Start() {
		if (!distraction_paper) {
			distraction_paper = GameObject.Find("DistractionPaper");
		}
		audio_source = GetComponent<AudioSource>();
		stamp = distraction_paper.transform.GetChild(0).GetComponent<Stamp>();
		timer_fill = distraction_paper.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
		timer_bg = timer_fill.transform.GetChild(0).gameObject;
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
				time_remaining = time_until_game_over;
				if (distraction_paper.transform.position.y < paper_start_y / 2 && can_play_on_enter) {
					audio_source.Play();
					can_play_on_enter = false;
				}
				if (distraction_paper.transform.position.y < end_y + 0.02) {
					distraction_paper.transform.position = new Vector2(0, end_y);
					stamp.can_drag = true;
					can_tween = false;
					is_paper_shown = true;
					can_play_on_enter = true;
				}
			} else {
				distraction_paper.transform.position = Vector2.Lerp(distraction_paper.transform.position, new Vector2(0, paper_start_y), 5f * Time.deltaTime);
				if (distraction_paper.transform.position.y > paper_start_y / 2 && can_play_on_exit) {
					audio_source.Play();
					can_play_on_exit = false;
				}
				if (distraction_paper.transform.position.y > paper_start_y - 0.02) {
					distraction_paper.transform.position = new Vector2(0, paper_start_y);
					stamp.transform.localPosition = stamp.start_pos;
					can_tween = false;
					can_play_on_exit = true;
				}
			}
		}

		if (is_paper_shown) {
			time_remaining -= Time.deltaTime;
			if (time_remaining < 0) {
				if (GameOverManager.Instance != null) {
					GameOverManager.Instance.ShowGameOver();
				}
			}
		}

		timer_fill.GetComponent<Image>().fillAmount = 1 - (time_remaining / time_until_game_over);
	}

	public void SpawnBubble(string text, Vector2 position) {
		GameObject bubble = Instantiate(bubble_prefabs[Random.Range(0, bubble_prefabs.Length)], position, Quaternion.identity);
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
		time_remaining = time_until_game_over;
		yield return new WaitForSeconds(1f);
		can_tween = true;
		is_tween_reversed = true;
		StartCoroutine(WaitForBoss());
	}

	IEnumerator WaitForBoss() {
		float delay;
		if (GameManager.Instance.customerServed <= p1_customers_upper_bound) {
			delay = Random.Range(p1_paper_min_time, p1_paper_max_time);
		} else if (GameManager.Instance.customerServed <= p2_customers_upper_bound) {
			delay = Random.Range(p2_paper_min_time, p2_paper_max_time);
		} else {
			delay = Random.Range(p3_paper_min_time, p3_paper_max_time);
		}
		yield return new WaitForSeconds(delay);
		SummonBoss();
	}

	IEnumerator SpawnDistraction() {
		float delay;
		if (GameManager.Instance.customerServed <= p1_customers_upper_bound) {
			delay = Random.Range(p1_bubble_min_time, p1_bubble_max_time);
		} else if (GameManager.Instance.customerServed <= p2_customers_upper_bound) {
			delay = Random.Range(p2_bubble_min_time, p2_bubble_max_time);
		} else {
			delay = Random.Range(p3_bubble_min_time, p3_bubble_max_time);
		}
		yield return new WaitForSeconds(delay);

		if (bubble_count < max_bubbles) {
			Vector2 bubble_pos;
			bool is_valid_pos;
			int attempts = 0;
			do {
				is_valid_pos = true;
				bubble_pos = new Vector2(Random.Range(-7f, 7f), Random.Range(-3f, 3f));
				GameObject[] spawned_bubbles = GameObject.FindGameObjectsWithTag("DistractionBubble");
				for (int i = 0; i < spawned_bubbles.Length; i++) {
					if (Mathf.Abs(bubble_pos.x - spawned_bubbles[i].transform.position.x) < bubble_min_x_distance ||
						Mathf.Abs(bubble_pos.y - spawned_bubbles[i].transform.position.y) < bubble_min_y_distance) {
						is_valid_pos = false;
						break;
					}
				}
				attempts++;
			} while (!is_valid_pos && attempts < 20);
			if (is_valid_pos) {
				SpawnBubble(speeches[Random.Range(0, speeches.Length)], bubble_pos);
				Debug.Log("Valid Position Found");
			} else {
				StartCoroutine(SpawnDistraction());
				Debug.Log("Retrying Spawning Bubble...");
			}
		} else {
			StartCoroutine(SpawnDistraction());
			Debug.Log("Can't Spawn Any More Bubbles");
		}
	}
}
