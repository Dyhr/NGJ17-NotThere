using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Networkable))]
public class Terminal : MonoBehaviour
{
    private void Start()
    {
        //GetComponent<Networkable>().Level = Random.Range(1, 4);
    }

    public void Activate(Creds creds)
    {
        if (creds.owner != null && creds.owner.GetComponent<Player>())
        {
            var n = GetComponent<Networkable>();
            var player = creds.owner.GetComponent<Player>();
            player.Hack = player.Hack != n ? n : null;
            n.status = player.Hack == n ? "Active" : "Idle";
            GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().PlayOneShot(GetComponent<Networkable>().activateClip);

            if(creds.name == "Hack")
                FakeAlert();
        }
    }

    private void FakeAlert()
    {
        if (FindObjectOfType<AstarPath>() == null || Guard.guards == null) return;
        var p = GameObject.FindGameObjectWithTag("Player");
        var offset = Random.onUnitSphere*2;
        offset.y = 0;
        for (int i = 0; i < Guard.guards.Length; ++i)
        {
            if (Guard.guards[i] == null) continue;

            var g = Guard.guards[i].GetComponent<Guard>();
            g.path = null;
            g.targetPosition = transform.position + offset;
            g.awaitingPath = true;
            g.seeker.StartPath(g.transform.position, g.targetPosition);
            g.alert = 2;
        }
        p.GetComponent<AudioSource>().PlayOneShot(Guard.alarmClip);
    }
}
