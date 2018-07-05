using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public Player player1;
    public Player player2;
    public Text p1HP;
    public Text p1Icon;

    public Text p2HP;
    public Text p2Icon;

    //public Text p1Ability1Icon;

    public Text[] abilityInfo = new Text[28];
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        p1HP.text = "HP: " + player1.healthPoints;
        p2HP.text = "HP: " + player2.healthPoints;
        //AbilityInfo:
        for(int i = 0; i < 7; i++)
        {
            if (player1.abilities.Capacity > i)
            {
                abilityInfo[i].text = "" + ((Ability)player1.abilities[i]).chargesAvailable;
                abilityInfo[7 + i].text = "" + ((Ability)player1.abilities[i]).chargeReplenishCooldown;

                abilityInfo[14 + i].text = "" + ((Ability)player2.abilities[i]).chargesAvailable;
                abilityInfo[21 + i].text = "" + ((Ability)player2.abilities[i]).chargeReplenishCooldown;
            }
            else
            {
                //no ability here
            }
        }       
    }
}
