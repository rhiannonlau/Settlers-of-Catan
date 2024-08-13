using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject Card1;
    public GameObject Card2;
    public GameObject PlayerArea;
    public GameObject EnemyArea1;
    public GameObject DropZone;

    List<GameObject> cards = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea1 = GameObject.Find("EnemyArea1");
        DropZone = GameObject.Find("DropZone");
    }
    
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        cards.Add(Card1);
        cards.Add(Card2);
        Debug.Log(cards);
    }

    [Command] // something the client will request of the server
    public void CmdDealCards()
    {
        for (int i = 0; i < 5; i++)
        {
            // instantiate = unity's make object on scene
            // quaternion.identity = object has no rotation
            GameObject card = Instantiate(cards[Random.Range(0, cards.Count)], new Vector2(0, 0), Quaternion.identity);
            
            // connectionToClient means that the client that called this method has authority over the gameobject
            NetworkServer.Spawn(card, connectionToClient);

            RpcShowCard(card, "Dealt");
        }
    }

    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");

        if (isServer)
        {
            UpdateTurnsPlayed();
        }
    }

    [Server]
    void UpdateTurnsPlayed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateTurnsPlayed();
        RpcLogToClients("Turns Played: " + gm.TurnsPlayed);
    }

    [ClientRpc]
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    [ClientRpc] // something the server will request of all clients
    private void RpcShowCard(GameObject card, string type)
    {
        if (type == "Dealt")
        {
            if (isOwned)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }

            else
            {
                card.transform.SetParent(EnemyArea1.transform, false);
                card.GetComponent<CardFlipper>().Flip();
                // to prevent cheating with the card flipping, instead of telling the
                // client what the card is, you could just use a placeholder card back
                // that just sits there until the card is actually played, then actually
                // spawn the card across the network
            }
        }

        else if (type == "Played")
        {
            card.transform.SetParent(DropZone.transform, false);

            if (!isOwned)
            {
                card.GetComponent<CardFlipper>().Flip();
            }
        }
    }

    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }

    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity opponentIdentity = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(opponentIdentity.connectionToClient);
    }

    [TargetRpc]
    void TargetSelfCard()
    {
        Debug.Log("Targeted by self");
    }

    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
        Debug.Log("Targeted by other");
    }

    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);
    }

    [ClientRpc]
    void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().NumberOfClicks++; // use hooks to get a trigger after syncvar update
        Debug.Log("This card has been clicked " + card.GetComponent<IncrementClick>().NumberOfClicks + " times!");
    }
}

// For turning into 4 player:
// implement a Game Manager of some sort to track whose turn it is, and have your server manage rendering cards in the appropriate locations using Commands, ClientRpcs, and TargetRpcs