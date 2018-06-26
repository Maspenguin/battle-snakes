using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public Player player1;
    public Player player2;
    public Text p1HP;
    public Text p2HP;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        p1HP.text = "P1 HP: " + player1.healthPoints;
        p2HP.text = "P2 HP: " + player2.healthPoints;
    }
}
