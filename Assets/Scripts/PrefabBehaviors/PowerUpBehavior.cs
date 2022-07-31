using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour
{
	#region ENUMS
	//Different powerups
	public enum Status{ 
        Negative = -1,
        Positive = 1,
        SuperPositive = 2,
        SuperNegative = -2
    }
	#endregion
	#region VARIABLES
	[HideInInspector] public bool isActive;
    public Rigidbody2D RB2D;
    public SpriteRenderer sprite;
    public CircleCollider2D CircleCollider;
    public Status effect = Status.Positive;

    private float initialGravity = 0;
	#endregion

	void Awake()
    {
        initialGravity = RB2D.gravityScale;
        SetInactiveState();
    }


    void Update()
    {
        if(transform.position.y < -Boundary.window_height) {
            SetInactiveState();
		}

        //Set color depending on current effect
        switch (effect) {
            case Status.Negative:
                sprite.color = Color.gray;
                break;
            case Status.SuperNegative:
                sprite.color = Color.black;
                break;
            case Status.Positive:
                sprite.color = Color.blue;
                break;
            case Status.SuperPositive:
                sprite.color = Color.green;
                break;
        }
    }

    public void SpawnPowerUp(Vector3 enemyTransform) {
        effect = GetStatusFromInt(Random.Range(0, 5));
        transform.position = enemyTransform;
        sprite.enabled = CircleCollider.enabled = isActive = true;
        RB2D.gravityScale = initialGravity;
    }

    Status GetStatusFromInt(int val) {
        Status retValue = 0;
        if (val == 0) retValue = Status.Negative;
        if (val == 1) retValue = Status.SuperNegative;
        if (val == 2) retValue = Status.Positive;
        if (val == 3) retValue = Status.SuperPositive;
        return retValue;
    }

    public void SetInactiveState() {
        RB2D.velocity = Vector2.zero;
        RB2D.gravityScale = 0;
        transform.position = new Vector3(0.0f, -20.0f);
        sprite.enabled = CircleCollider.enabled = isActive = false;
    }
}
