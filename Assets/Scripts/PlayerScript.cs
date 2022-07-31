using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour {
	#region VARIABLES
	//Player Variables
	public int bulletDamage = 1;
	public float speed = 1;
	public Rigidbody2D RB2D;
	//Input variables
	private PlayerInputAction PlayerInputs;
	//Bullet Variables
	[SerializeField] GameObject bulletPefab;
	static public Queue<BulletBehavior> activeBullets = new Queue<BulletBehavior>();
	#endregion

	private void Awake() {
		RB2D = GetComponent<Rigidbody2D>();
	}

	void Start() {
		BulletBufferBuilder();
	}

	void FixedUpdate() {
		MovePlayer();
	}

	private void BulletBufferBuilder() {
		int bulletBuffer = 25;
		for (int i = 0; i < bulletBuffer; i++) {
			Instantiate(bulletPefab, transform);
		}
	}

	public void ResetPlayer() {
		bulletDamage = 1;
		transform.position = new Vector2(0, transform.position.y);
		RB2D.velocity = Vector2.zero;
	}


	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("PowerUp")) {
			PowerUpBehavior powerUp = collision.GetComponent<PowerUpBehavior>();
			bulletDamage += (int)powerUp.effect;
			if (bulletDamage < 1) bulletDamage = 1;
			if (bulletDamage > 3) bulletDamage = 3;
			powerUp.SetInactiveState();
		}
	}

	#region INPUT SYSTEM FUNCTIONS
	private void OnEnable() {
		//reenabling the input sistem on load
		if (PlayerInputs == null) {
			PlayerInputs = new PlayerInputAction();
		}

		PlayerInputs.Enable();
		PlayerInputs.Player.Move.performed += Move_performed;
		PlayerInputs.Player.Shot.performed += Shoot;
		activeBullets = new Queue<BulletBehavior>();
	}
	private void OnDisable() {
		//diabling the input system on unload
		PlayerInputs.Disable();
		PlayerInputs.Player.Move.performed -= Move_performed;
		PlayerInputs.Player.Shot.performed -= Shoot;
	}
	private void Move_performed(InputAction.CallbackContext context) {
		RB2D.AddForce(Vector2.right * speed * context.ReadValue<float>());
	}

	public void MovePlayer() {

		RB2D.AddForce(Vector2.right * speed * PlayerInputs.Player.Move.ReadValue<float>());

		//Make sure the player cannot leave the screen under any circunstances
		if (transform.position.x < -(Boundary.window_width / 2) + transform.localScale.x / 2) {
			transform.position = new Vector2(-(Boundary.window_width / 2) + transform.localScale.x / 2, transform.position.y);
		}
		if (transform.position.x > (Boundary.window_width / 2) - transform.localScale.x / 2) {
			transform.position = new Vector2((Boundary.window_width / 2) - transform.localScale.x / 2, transform.position.y);
		}
	}

	public void Shoot(InputAction.CallbackContext context) {
		if (activeBullets.Count > 0) {
			activeBullets.Peek().ShootBullet(transform.position, bulletDamage);
			activeBullets.Dequeue();
		}

	}
	#endregion
}
