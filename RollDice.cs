using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RollDice : NetworkBehaviour
{
    // public GameObject Card1;
    // public GameObject Card2;
    // public GameObject PlayerArea;

    public PlayerManager PlayerManager;

    public void OnClick()
    {
        // Old code for button to deal cards, now handled through PlayerManager
        // for (int i = 0; i < 5; i++)
        // {
        //     // instantiate = unity's make object on scene
        //     // quaternion.identity = object has no rotation
        //     GameObject card = Instantiate(Card1, new Vector2(0, 0), Quaternion.identity);
        //     // set the parent to be the player area so the cards are shown
        //     // set world position = false because you want it to spawn where the player area is
        //     card.transform.SetParent(PlayerArea.transform, false);
        // }

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerManager.CmdDealCards();
    }
}