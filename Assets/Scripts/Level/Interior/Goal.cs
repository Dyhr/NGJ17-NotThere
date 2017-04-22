using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    public float speed;
    public float speed2;
    public Transform player;
    public AudioClip pickupClip;

    private Transform price;
    private Vector3 target;
    private int i;

    private void Start()
    {
        price = transform.FindChild("Price");
        StartCoroutine(Cycle());
    }

    private void Update()
    {
        if (price == null) return;
        price.Rotate(0, speed*Time.deltaTime, 0);
        price.localPosition = Vector3.Lerp(price.localPosition, target, 0.05f);

        if (player != null && Vector3.Distance(transform.position, player.position) < 1.5f)
        {
            price.parent = player;
            price.localPosition = -Vector3.forward*0.5f;
            price.localEulerAngles = new Vector3(90, 0, 0);
            price.Rotate(0,45,0,Space.Self);
            price = null;
            FindObjectOfType<Teleport>().Finished = true;
            player.GetComponent<AudioSource>().PlayOneShot(pickupClip,0.8f);

            Alert();
            var doors = FindObjectsOfType<Door>();
            foreach (var door in doors)
            {
                door.StopAllCoroutines();
                door.Open = true;
            }
        }
    }
    private void Alert()
    {
        if (FindObjectOfType<AstarPath>() == null || Guard.guards == null) return;
        var p = player;
        for (int i = 0; i < Guard.guards.Length; ++i)
        {
            if (Guard.guards[i] == null) continue;

            var g = Guard.guards[i].GetComponent<Guard>();
            g.path = null;
            g.targetPosition = p.transform.position;
            g.awaitingPath = true;
            g.seeker.StartPath(g.transform.position, g.targetPosition);
            g.alert = 2;
        }
        p.GetComponent<Player>().Hack = null;
    }

    private IEnumerator Cycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(speed2);
            target = Vector3.up*(++i%2 == 0 ? 0.4f : 1.1f);
        }
    }
}