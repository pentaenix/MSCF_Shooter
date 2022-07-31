using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

	#region VARIABLES
	public float speed = 5;
	[HideInInspector] public bool isActive;

	private int damage = 1;
	private int TimeAlive = 0;
	private int SecondCounter;

	public Rigidbody2D RB2D;
	public SpriteRenderer sprite;
	public CircleCollider2D CircleCollider;
	#endregion
	void Awake() {
		SetInactiveState();
	}

	void Update() {
		if (TimeAlive > 5) {
			SetInactiveState();
		}
		if (isActive) {
			if (SecondCounter != (int)Time.time) {
				SecondCounter = (int)Time.time;
				TimeAlive++;
			}
			//Set bullet color depending on the damage
			switch (damage) {
				case 1:
					sprite.color = Color.white;
					break;
				case 2:
					sprite.color = Color.yellow;
					break;
				case 3:
					sprite.color = new Color(0.9f, 0.6f, 0.0f);
					break;
			}
			if (LevelManager.GameOver) {
				SetInactiveState();
			}
		}
	}

	public void ShootBullet(Vector3 playerTransform, int dmg) {
		damage = dmg;
		if(RB2D != null) RB2D.velocity = Vector2.up * speed;
		transform.position = playerTransform;
		sprite.enabled = CircleCollider.enabled = isActive = true;
	}

	public void SetInactiveState() {
		damage = 1;
		TimeAlive = 0;
		RB2D.velocity = Vector2.zero;
		transform.position = new Vector3(0.0f, -20.0f);
		sprite.enabled = CircleCollider.enabled = isActive = false;
		PlayerScript.activeBullets.Enqueue(this);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.transform.CompareTag("Enemy")) {
			collision.gameObject.GetComponent<EnemyBehavior>().TakeDamage(damage);
		}
		SetInactiveState();
	}
}
