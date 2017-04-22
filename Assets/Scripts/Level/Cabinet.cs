using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Networkable))]
public class Cabinet : MonoBehaviour
{
    public Card Card;
    private bool Opened;
    private Networkable net;

    private void Start()
    {
        net = GetComponent<Networkable>();
    }

    public int Level
    {
        get { return GetComponent<Networkable>().level; }
    }

    private void Update()
    {
        if (Guard.player != null && Guard.player.GetComponent<Player>() != null)
            net.action = Guard.player.GetComponent<Player>().Hack == null ? "" : "Unlock";
    }

    public void Activate(Creds creds)
    {
        if (Opened || creds.level < Level || !GetComponent<Networkable>().hacked) return;

        net.status = "Unlocked";
        Opened = true;
        var card = Instantiate(Card);
        card.transform.position = transform.position + Vector3.up;
        card.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere*5);
        card.Level = net.level + Random.Range(1, 3);
        card.Player = Guard.player;
        GetComponent<Light>().enabled = false; GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().PlayOneShot(GetComponent<Networkable>().activateClip);
    }
}
