using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class HackMode : MonoBehaviour
{
    private int level = 0;

    public Material nodeMaterial;
    public Material planeMaterial;
    public Transform nodeLabel;

    public Networkable Origin
    {
        set
        {
            MakePlane();
            level = value.level;
            DrawNode(value, true);
            DoLines(value);
        }
    }

    private Networkable[] items;
    private readonly HashSet<Networkable> logA = new HashSet<Networkable>();
    private readonly HashSet<Networkable> logB = new HashSet<Networkable>();

    private void DoLines(Networkable n)
    {
        if (logA.Contains(n)) return;
        logA.Add(n);
        DrawNode(n);
        foreach (var neighbor in n.neighbors)
        {
            DrawNode(neighbor);
            MakeLine(n,neighbor);
            if (neighbor.level <= level && neighbor.hacked)
                DoLines(neighbor);
        }
    }

    private void DrawNode(Networkable n, bool hacked = false)
    {
        if (logB.Contains(n)) return;
        logB.Add(n);
        var dir = -Camera.main.transform.forward;

        var circle = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        circle.position = n.transform.position + dir * 8;
        circle.parent = transform;
        circle.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        circle.GetComponent<MeshRenderer>().receiveShadows = false;
        circle.GetComponent<MeshRenderer>().sharedMaterial = nodeMaterial;
        circle.gameObject.AddComponent<NetNode>().label = nodeLabel;
        circle.GetComponent<NetNode>().Origin = n;
        circle.GetComponent<NetNode>().canHack = n.level <= level;
        if (hacked) circle.GetComponent<NetNode>().Hacked = true;
    }

    private void MakePlane()
    {
        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
        plane.position = FindObjectOfType<Level>().transform.position + Vector3.up*2.5f;
        plane.localScale = Vector3.one*40;
        plane.parent = transform;
        plane.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        plane.GetComponent<MeshRenderer>().receiveShadows = false;
        plane.GetComponent<MeshRenderer>().sharedMaterial = planeMaterial;
        plane.gameObject.layer = LayerMask.NameToLayer("Cover");
    }

    private void MakeLine(Networkable a, Networkable b)
    {
        var dir = -Camera.main.transform.forward;

        var line = new GameObject("Line").AddComponent<LineRenderer>();
        line.transform.parent = transform;
        line.SetWidth(0.2f, 0.2f);
        line.SetPosition(0, a.transform.position + dir * 7);
        line.SetPosition(1, b.transform.position + dir * 7);
        line.sharedMaterial = nodeMaterial;
    }

    private void Update()
    {
        var cam = Camera.main;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            var node = hit.transform.GetComponent<NetNode>();
            if (node != null)
            {
                node.hover = true;
                if (Input.GetButtonDown("Fire1"))
                {
                    node.Activate(new Creds("Hack",level, GameObject.FindGameObjectWithTag("Player").GetComponent<Human>()));
                    if (node.Hacked)
                    {
                        DoLines(node.Origin);
                    }
                }
            }
        }
    }
}
