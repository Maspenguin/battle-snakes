using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public Player player1;
    public Player player2;
    // Use this for initialization
    void Start ()
    {
        print("that");
        if (GameObject.Find("Player1") == null && GameObject.Find("Player2") == null)
        {
            print("Error in HUD script, Players do not exist yet");
        }
        else
        {
            player1 = GameObject.Find("Player1").GetComponent<Player>();
            player2 = GameObject.Find("Player2").GetComponent<Player>();
        }

        transform.Find("P1Panel").Find("HPBar").GetComponent<Slider>().maxValue = 10;
        transform.Find("P2Panel").Find("HPBar").GetComponent<Slider>().maxValue = 10;
        for (int i = 0; i < 7; i++)
        {
            if (player1.abilities.Count > i)//player1
            {
                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Icon").GetComponent<Image>().sprite = ((Ability)player1.abilities[i]).abilityIcon;
                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Charges").GetComponent<Slider>().maxValue = ((Ability)player1.abilities[i]).maxChargesAvailable;
                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Cooldown").GetComponent<Slider>().maxValue = ((Ability)player1.abilities[i]).maxChargeReplenishCooldown;
                print(transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Cooldown").GetComponent<Slider>().maxValue);
            }
            else
            {
                print(player1.abilities.Count);
                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/defaultAbilityIcon") as Sprite;
                //default icon
            }
            if (player2.abilities.Count > i)//player2
            {
                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Icon").GetComponent<Image>().sprite = ((Ability)player2.abilities[i]).abilityIcon;
                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Charges").GetComponent<Slider>().maxValue = ((Ability)player2.abilities[i]).maxChargesAvailable;
                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Cooldown").GetComponent<Slider>().maxValue = ((Ability)player2.abilities[i]).maxChargeReplenishCooldown;
            }
            else
            {
                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/defaultAbilityIcon") as Sprite;
                //default icon
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        transform.Find("P1Panel").Find("HPBar").GetComponent<Slider>().value = player1.healthPoints;
        transform.Find("P1Panel").Find("HPBar").Find("Text").GetComponent<Text>().text = "HP: " + player1.healthPoints;
        transform.Find("P2Panel").Find("HPBar").GetComponent<Slider>().value = player2.healthPoints;
        transform.Find("P2Panel").Find("HPBar").Find("Text").GetComponent<Text>().text = "HP: " + player2.healthPoints;

        transform.Find("P1Panel").Find("EndLag").GetComponent<Slider>().value = player1.maxEndLag - player1.endLag;
        transform.Find("P1Panel").Find("EndLag").GetComponent<Slider>().maxValue = player1.maxEndLag;

        transform.Find("P2Panel").Find("EndLag").GetComponent<Slider>().value = player2.maxEndLag - player2.endLag;
        transform.Find("P2Panel").Find("EndLag").GetComponent<Slider>().maxValue = player2.maxEndLag;

        //AbilityInfo:
        for (int i = 0; i < 7; i++)
        {
            if (player1.abilities.Count > i)//player1 abilities
            {
                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Charges").Find("Text").GetComponent<Text>().text = "" + ((Ability)player1.abilities[i]).chargesAvailable;
                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Charges").GetComponent<Slider>().value = ((Ability)player1.abilities[i]).chargesAvailable;

                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Cooldown").Find("Text").GetComponent<Text>().text = ((Ability)player1.abilities[i]).chargeReplenishCooldown.ToString("F1");
                transform.Find("P1Panel").Find("Ability" + i + "Panel").Find("Cooldown").GetComponent<Slider>().value = ((Ability)player1.abilities[i]).maxChargeReplenishCooldown - ((Ability)player1.abilities[i]).chargeReplenishCooldown;
            }
            // else: no ability here

            if(player2.abilities.Count > i)//player2 abilities
            {
                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Charges").Find("Text").GetComponent<Text>().text = "" + ((Ability)player2.abilities[i]).chargesAvailable;
                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Charges").GetComponent<Slider>().value = ((Ability)player2.abilities[i]).chargesAvailable;

                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Cooldown").Find("Text").GetComponent<Text>().text = ((Ability)player2.abilities[i]).chargeReplenishCooldown.ToString("F1");
                transform.Find("P2Panel").Find("Ability" + i + "Panel").Find("Cooldown").GetComponent<Slider>().value = ((Ability)player2.abilities[i]).maxChargeReplenishCooldown - ((Ability)player2.abilities[i]).chargeReplenishCooldown;
            }
            //else: no ability here
        }       
    }
}
