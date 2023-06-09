using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnjing : Mover
{
    //Experience
    public int xpValue = 1;

    //Logic
    public float triggerRange = 0.3f;
    public float chaseLength = 1.0f;

    private bool chasing;
    private bool collidingWithPlayer;
    private Transform  playerTransform;
    private Vector3 startingPosition;
    //public static bool isDead = false;

    //Hitbox
    private ContactFilter2D filter;
    private BoxCollider2D hitBox;
    private Collider2D[] hits = new Collider2D[10];

    public GameObject rewardItem;

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        if (PlayerPrefs.GetString("DeadEnemies").Contains(gameObject.name))
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {

        if (GetComponent<Rigidbody2D>().velocity.magnitude > 0.1)
        {
            GetComponent<Animator>().SetTrigger("Walking");
            GetComponent<Animator>().ResetTrigger("Idle");
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Idle");
            GetComponent<Animator>().ResetTrigger("Walking");
        }
        //Checking if position of player is inside the range
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerRange)
                chasing = true;

            Vector3 playerDirection = (gameObject.transform.position - playerTransform.position).normalized;
            if (chasing == true)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        //Collison mechanic
        collidingWithPlayer = false;
        hitBox.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }

            hits[i] = null;
        }
    }

    protected override void Death()
    {
        if (rewardItem != null)
        {
            rewardItem.transform.position = transform.position;
            rewardItem.SetActive(true);
        }
        Destroy(gameObject);
        GameManager.instance.experience += xpValue;
        GameManager.instance.deadEnemies += gameObject.name + "|";
        Debug.Log(GameManager.instance.deadEnemies);
        GameManager.instance.ShowText("+" + xpValue +  " xp",30,Color.magenta,transform.position,Vector3.up * 10, 0.5f);
    }
}
