using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Networkable : MonoBehaviour
{
    public string Name;
    public string action;
    public int level;
    public string status;
    public bool hacked;
    public AudioClip activateClip;
    public AudioClip hackClip;
    public List<Networkable> neighbors;

    private void OnDrawGizmos()
    {
        Fix();

        Gizmos.color = Color.cyan;
        foreach (var networkable in neighbors)
        {
            Gizmos.DrawLine(transform.position + Vector3.up * 2, networkable.transform.position + Vector3.up * 2);
        }
    }

    private void Update()
    {
        Fix();
    }

    private void Fix()
    {
        foreach (var networkable in neighbors)
        {
            if (!networkable.neighbors.Contains(this)) networkable.neighbors.Add(this);
        }
    }
}

public class Creds
{
    public string name;
    public int level;
    public Human owner;

    public Creds(string name, int level, Human owner)
    {
        this.name = name;
        this.level = level;
        this.owner = owner;
    }
}
