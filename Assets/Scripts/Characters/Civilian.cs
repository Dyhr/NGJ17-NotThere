using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Human))]
[RequireComponent(typeof(Seeker))]
public class Civilian : MonoBehaviour {

    public float nextWaypointDistance = 3;
    public Vector3 targetPosition;

    public float slowSpeed;
    public float fastSpeed;

    private RaycastHit hit;
    private Human human;
    internal Seeker seeker;
    internal Path path;
    private int currentWaypoint = 0;
    public static readonly Dictionary<Transform, float> Patrols = new Dictionary<Transform, float>();
    private Transform patrol;
    internal bool awaitingPath;
    public bool leaving;

    private void Start()
    {
        human = GetComponent<Human>();
        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
        if (Patrols.Count == 0)
            foreach (var go in GameObject.FindGameObjectsWithTag("Patrol"))
                Patrols.Add(go.transform, 0);
    }

    private bool alarming;
    public void Alarm()
    {
        if(!alarming)
            StartCoroutine(DoAlarm());
    }

    IEnumerator DoAlarm()
    {
        Interrupt();
        awaitingPath = true;
        yield return new WaitForSeconds(1);

        leaving = true;
        targetPosition = FindObjectOfType<Teleport>().transform.position;
        seeker.StartPath(transform.position, targetPosition);
    }

    public void FixedUpdate()
    {
        if (path != null)
        {
            if (currentWaypoint >= path.vectorPath.Count)
            {
                path = null;
                if(leaving) Destroy(gameObject);
                return;
            }

            human.inputControl = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                currentWaypoint++;
        }
        else if (!awaitingPath)
        {
            var maxt = 0f;
            var max = new List<Transform>();
            foreach (var patrol in Patrols.Keys.ToArray())
            {
                if (int.Parse(patrol.name.Split('-')[0]) != human.level) continue;
                var value = Patrols[patrol];
                Patrols[patrol] = value + Time.fixedDeltaTime;
                if (maxt < value)
                {
                    maxt = value;
                    max.Clear();
                }
                if (maxt == value)
                {
                    max.Add(patrol);
                    Patrols[patrol] = 0;
                }
            }
            if (max.Count > 0)
            {
                targetPosition = max[Random.Range(0, max.Count)].position;
                awaitingPath = true;
                seeker.StartPath(transform.position, targetPosition);
            }
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, 2))
        {
            if (hit.transform.parent != null && hit.transform.parent.CompareTag("Switch"))
            {
                if (hit.transform.parent.GetComponent<Door>() != null)
                {
                    hit.transform.parent.SendMessage("Activate", human.Creds);
                }
                if (hit.transform.parent.GetComponent<Guard>() != null)
                {
                    Interrupt();
                    //human.InputControl = (transform.right - transform.forward).normalized;
                }
            }
        }
    }

    public void Interrupt()
    {
        path = null;
    }

    public void OnPathComplete(Path p)
    {
        if (p.error) {
            Debug.LogError(p.errorLog);
            return;
        }
        path = p;
        currentWaypoint = 0;
        awaitingPath = false;

        for (var i = 1; i < p.vectorPath.Count; i++) {
            var a = p.vectorPath[i-1];
            var b = p.vectorPath[i];
            Debug.DrawLine(a,b);
        }
    }
}
