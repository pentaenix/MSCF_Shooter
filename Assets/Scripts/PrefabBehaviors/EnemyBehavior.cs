using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {
	#region VARIABLES
	public bool isActive;
    public float speed = 3;

    [Range(1,3)] public int Tier = 1;
    private int HP = 1;

    public Rigidbody2D RB2D;
    public SpriteRenderer sprite;
    public BoxCollider2D BoxCollider;
    private PowerUpBehavior powerUp;
	#endregion
	private void Start() {
        //save reference to powerUp
        powerUp = FindObjectOfType<PowerUpBehavior>();
	}
	void Update()
    {
        BoxCollider.enabled = (transform.position.y < (Boundary.window_height/2)-0.2f);

        switch (HP){
            case 1:
                sprite.color = Color.green;
                break;
            case 2:
                sprite.color = Color.blue;
                break;
            case 3:
                sprite.color = Color.red;
                break;
        }
        if(transform.position.y < LevelManager.EnemyLimitAtYAxis) {
            LevelManager.GameOver = true;
		}
    }

    public void TakeDamage(int damage) {
        HP -= damage;
        if(HP <= 0) {
            LevelManager.score += Tier * ScoreMultiplier();
            LevelManager.EnemiesInScene.Remove(this);
            if (!powerUp.isActive && Random.Range(0, 15) < 2) {
                powerUp.SpawnPowerUp(transform.position);
            }
            SetInactiveState();
        }
	}

    int ScoreMultiplier() {
        return (int)Mathf.Ceil((transform.position.y + (Boundary.window_height / 2)) * 3 / Boundary.window_height);
	}

    public void SpawnEnemy(Vector3 spawnPosition, int tier, float speedBoost) {
        HP = Tier = tier;
        RB2D.velocity = Vector2.down * (speed + speedBoost);
        transform.position = spawnPosition;
        sprite.enabled = BoxCollider.enabled = isActive = true;
        
    }

    public void SetInactiveState() {
        RB2D.velocity = Vector2.zero;
        transform.position = new Vector3(0.0f, 20.0f);
        sprite.enabled = BoxCollider.enabled = isActive = false;

    }
}
