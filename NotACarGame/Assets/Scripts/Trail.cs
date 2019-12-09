using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour {
    public Transform parent;
    
    private Vector2 facingDirection;
    public int teamNumber;
    public int damage = 10;
    private float distance = 0.35f; //The spacing between each segment of the trail
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Turn to face parent
        facingDirection = parent.transform.position - transform.position;
        facingDirection = facingDirection.normalized;

        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        //Move up to the distance
        if (Vector2.Distance(transform.position, parent.position) > distance)
        {
            transform.Translate(Vector2.Distance(transform.position, parent.position) - distance, 0, 0);
        }
    }


    //public Transform player; //Used for alternate movement
    //public int trailNum; //Used for alternated movement

    //Invoke("Movement", 2f * trailNum);//alternate movement (would need some sort of parameter for the players movement Vector at this time)

    ////alternate movement
    //void Movement()
    //{
    //    facingDirection = new Vector2(player.GetComponent<Player>().rawVelocity.x, player.GetComponent<Player>().rawVelocity.y);
    //    facingDirection = facingDirection.normalized;

    //    float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
    //    transform.rotation = Quaternion.Euler(0, 0, angle);

    //    transform.Translate(player.GetComponent<Player>().speed * player.GetComponent<Player>().rawVelocity.magnitude * Time.deltaTime, 0, 0);
    //}
}
