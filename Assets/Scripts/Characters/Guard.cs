using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Human))]
[RequireComponent(typeof(Seeker))]
public class Guard : MonoBehaviour {
    public float nextWaypointDistance = 3;
    public Vector3 targetPosition;

    public float slowSpeed;
    public float normalSpeed;
    public float fastSpeed;
    public float detectionAngle;
    public float alertTime;
    public float alertLevel;
    public AudioClip alarmSound;

    public bool canSeePlayer;

    private RaycastHit hit;
    private Human human;
    internal Seeker seeker;
    internal Path path;
    private int currentWaypoint = 0;
    public static AudioClip alarmClip;
    public static GameObject[] switches;
    public static readonly Dictionary<Transform, float> Patrols = new Dictionary<Transform, float>();
    public static Transform player;
    public static Rigidbody playerr;
    public static Guard[] guards;
    public static Civilian[] civilians;
    internal bool awaitingPath;
    internal int alert;

    private void Start() {
        alarmClip = alarmSound;
        human = GetComponent<Human>();
        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
        if (switches == null)
            switches = GameObject.FindGameObjectsWithTag("Switch");
        if (guards == null)
            guards = GameObject.FindGameObjectsWithTag("Guard").Select(g => g.GetComponent<Guard>()).ToArray();
        ;
        if (civilians == null)
            civilians = GameObject.FindGameObjectsWithTag("Civilian").Select(g => g.GetComponent<Civilian>()).ToArray();
        if (Patrols.Count == 0)
            foreach (var go in GameObject.FindGameObjectsWithTag("Patrol"))
                Patrols.Add(go.transform, 0);
    }

    public void PleaseDie() {
        StopAllCoroutines();
        Reinforce();
    }

    private void Reinforce() {
        if (human.level < alertLevel || FindObjectOfType<AstarPath>() == null || guards == null) return;
        if (player != null)
            player.GetComponent<AudioSource>().PlayOneShot(alarmSound);
        for (int i = 0; i < guards.Length; ++i) {
            if (guards[i] == null) continue;

            var g = guards[i].GetComponent<Guard>();
            g.path = null;
            g.targetPosition = transform.position;
            g.awaitingPath = true;
            g.seeker.StartPath(g.transform.position, g.targetPosition);
            g.alert = 2;
        }
    }

    private bool alarming;

    public void Alarm() {
        if (!alarming)
            StartCoroutine(DoAlarm());
    }

    IEnumerator DoAlarm() {
        alarming = true;
        yield return new WaitForSeconds(alertTime);
        alarming = false;
        if (!canSeePlayer) yield break;

        if (FindObjectOfType<AstarPath>() == null || guards == null) yield break;
        foreach (Guard guard in guards) {
            if (guard == null) continue;
            guard.path = null;
            guard.targetPosition = player.position;
            guard.awaitingPath = true;
            guard.seeker.StartPath(guard.transform.position, guard.targetPosition);
            guard.alert = 2;
        }
        foreach (var civilian in civilians) {
            if (civilian == null) continue;
            civilian.Alarm();
        }
    }

    public void FixedUpdate() {
        if (path != null) {
            if (currentWaypoint >= path.vectorPath.Count) {
                path = null;
                alert = 0;
                return;
            }

            human.inputControl = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
                currentWaypoint++;
        } else if (!awaitingPath) {
            var maxt = 0f;
            var max = new List<Transform>();
            foreach (var patrol in Patrols.Keys.ToArray()) {
                if (int.Parse(patrol.name.Split('-')[0]) != human.level) continue;
                var value = Patrols[patrol];
                Patrols[patrol] = value + Time.fixedDeltaTime;
                if (maxt < value) {
                    maxt = value;
                    max.Clear();
                }
                if (maxt == value) {
                    max.Add(patrol);
                    Patrols[patrol] = 0;
                }
            }
            if (max.Count > 0) {
                targetPosition = max[Random.Range(0, max.Count)].position;
                awaitingPath = true;
                seeker.StartPath(transform.position, targetPosition);
            }
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, 2)) {
            if (hit.transform.parent != null && hit.transform.parent.CompareTag("Switch")) {
                if (hit.transform.parent.GetComponent<Door>() != null) {
                    hit.transform.parent.SendMessage("Activate", human.Creds);
                }
                if (hit.transform.parent.GetComponent<Guard>() != null) {
                    Interrupt();
                    //human.InputControl = (transform.right - transform.forward).normalized;
                }
            }
        }

        if (player != null) {
            if (canSeePlayer) player.GetComponent<Player>().invisible = false;
            if (Vector3.Distance(player.position, transform.position) < 2)
                player.GetComponent<Player>().invisible = false;
        }

        canSeePlayer = false;
        var dir = player != null ? (player.position - transform.position).normalized : Vector3.zero;
        if (player != null && player.GetComponent<Player>().roomLevel > 0 && Physics.Raycast(transform.position, dir, out hit)) {
            if (hit.transform == player && !player.GetComponent<Player>().invisible &&
                Vector3.Dot(dir, transform.forward) > Mathf.Cos(detectionAngle * Mathf.Deg2Rad)) {
                canSeePlayer = true;
                if (!awaitingPath && Vector3.Distance(player.position, targetPosition) > 1) {
                    path = null;
                    targetPosition = player.position;
                    awaitingPath = true;
                    seeker.StartPath(transform.position, targetPosition);
                }
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation((player.position + transform.right * 0.25f + playerr.velocity * 0.2f) -
                                            transform.position), 0.5f);
                human.inputFire = true;
                human.lockRot = true;
                if (alert == 0)
                    Alarm();
                alert = 2;
            }
        }

        human.inputAim = alert > 0;
        human.speed = alert == 0 ? normalSpeed : (alert == 2 ? slowSpeed : fastSpeed);

        if (alert == 2)
            alert = 1;
    }

    public void Interrupt() {
        path = null;
    }

    public void OnPathComplete(Path p) {
        if (p.error) {
            Debug.LogError(p.errorLog);
            return;
        }
        path = p;
        currentWaypoint = 0;
        awaitingPath = false;

        for (var i = 1; i < p.vectorPath.Count; i++) {
            var a = p.vectorPath[i - 1];
            var b = p.vectorPath[i];
            Debug.DrawLine(a, b);
        }
    }
}