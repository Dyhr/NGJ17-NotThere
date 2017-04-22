using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour {
    public bool generate;
    public int rooms;
    public int tries;
    public float wallThickness;
    public float wallHeight;
    public float doorWidth;
    public float doorHeight;
    public Material wallMaterial;
    public Material floorMaterial;

    public Vector2 minRoomSize;
    public Vector2 maxRoomSize;

    public Door Door;
    public Guard guard;
    public Civilian civilian;
    public Terminal terminal;
    public Transform Spawn;
    public Transform goal;
    public Transform cabinet;

    private void Start() {
        if (generate) Remake();
    }

    private void OnDrawGizmos() {
        if (generate) Remake();
    }

    private List<Room> GetMap() {
        var map = new List<Room>();

        map.Add(new Room(Vector2.zero, maxRoomSize, Vector4.zero));

        for (int k = 0; k < rooms; ++k) {
            var o = map[Random.Range(0, map.Count)];
            for (int i = 0; i < 4; ++i) {
                if (o.doors[i] != 0) continue;
                for (int t = 0; t < tries; ++t) {
                    var room = new Room(o.position,
                        new Vector2(Random.Range(minRoomSize.x, maxRoomSize.x),
                            Random.Range(minRoomSize.y, maxRoomSize.y)),
                        Vector4.zero);

                    var sx = Mathf.Min(o.size.x - doorWidth - wallThickness, room.size.x - doorWidth - wallThickness);
                    var sy = Mathf.Min(o.size.y - doorWidth - wallThickness, room.size.y - doorWidth - wallThickness);

                    float min, max;
                    switch (i) {
                        case 0:
                            room.position -= Vector2.right * (o.size.x + room.size.x) / 2;
                            room.position += Vector2.up * Random.Range(-sy, sy);

                            min = (room.position.y - o.position.y - room.size.y / 2 + o.size.y / 2 +
                                   (wallThickness + doorWidth / 2)) / o.size.y;
                            max = (room.position.y - o.position.y + room.size.y / 2 + o.size.y / 2 -
                                   (wallThickness + doorWidth / 2)) / o.size.y;
                            min = Mathf.Clamp(min, (wallThickness + doorWidth / 2) / o.size.y,
                                1 - (wallThickness + doorWidth / 2) / o.size.y);
                            max = Mathf.Clamp(max, (wallThickness + doorWidth / 2) / o.size.y,
                                1 - (wallThickness + doorWidth / 2) / o.size.y);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doors[0] = Random.Range(min, max);
                                room.doors[1] = (o.doors[0] * o.size.y + o.position.y - o.size.y / 2 + room.size.y / 2 -
                                                 room.position.y) / room.size.y;
                                o.neighbors[0] = room;
                                room.neighbors[1] = o;
                            }
                            break;
                        case 1:
                            room.position += Vector2.right * (o.size.x + room.size.x) / 2;
                            room.position += Vector2.up * Random.Range(-sy, sy);

                            min = (room.position.y - o.position.y - room.size.y / 2 + o.size.y / 2 +
                                   (wallThickness + doorWidth / 2)) / o.size.y;
                            max = (room.position.y - o.position.y + room.size.y / 2 + o.size.y / 2 -
                                   (wallThickness + doorWidth / 2)) / o.size.y;
                            min = Mathf.Clamp(min, (wallThickness + doorWidth / 2) / o.size.y,
                                1 - (wallThickness + doorWidth / 2) / o.size.y);
                            max = Mathf.Clamp(max, (wallThickness + doorWidth / 2) / o.size.y,
                                1 - (wallThickness + doorWidth / 2) / o.size.y);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doors[1] = Random.Range(min, max);
                                room.doors[0] = (o.doors[1] * o.size.y + o.position.y - o.size.y / 2 + room.size.y / 2 -
                                                 room.position.y) / room.size.y;
                                o.neighbors[1] = room;
                                room.neighbors[0] = o;
                            }
                            break;
                        case 2:
                            room.position -= Vector2.up * (o.size.y + room.size.y) / 2;
                            room.position += Vector2.right * Random.Range(-sx, sx);

                            min = (room.position.x - o.position.x - room.size.x / 2 + o.size.x / 2 +
                                   (wallThickness + doorWidth / 2)) / o.size.x;
                            max = (room.position.x - o.position.x + room.size.x / 2 + o.size.x / 2 -
                                   (wallThickness + doorWidth / 2)) / o.size.x;
                            min = Mathf.Clamp(min, (wallThickness + doorWidth / 2) / o.size.x,
                                1 - (wallThickness + doorWidth / 2) / o.size.x);
                            max = Mathf.Clamp(max, (wallThickness + doorWidth / 2) / o.size.x,
                                1 - (wallThickness + doorWidth / 2) / o.size.x);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doors[2] = Random.Range(min, max);
                                room.doors[3] = (o.doors[2] * o.size.x + o.position.x - o.size.x / 2 + room.size.x / 2 -
                                                 room.position.x) / room.size.x;
                                o.neighbors[2] = room;
                                room.neighbors[3] = o;
                            }
                            break;
                        case 3:
                            room.position += Vector2.up * (o.size.y + room.size.y) / 2;
                            room.position += Vector2.right * Random.Range(-sx, sx);

                            min = (room.position.x - o.position.x - room.size.x / 2 + o.size.x / 2 +
                                   (wallThickness + doorWidth / 2)) / o.size.x;
                            max = (room.position.x - o.position.x + room.size.x / 2 + o.size.x / 2 -
                                   (wallThickness + doorWidth / 2)) / o.size.x;
                            min = Mathf.Clamp(min, (wallThickness + doorWidth / 2) / o.size.x,
                                1 - (wallThickness + doorWidth / 2) / o.size.x);
                            max = Mathf.Clamp(max, (wallThickness + doorWidth / 2) / o.size.x,
                                1 - (wallThickness + doorWidth / 2) / o.size.x);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doors[3] = Random.Range(min, max);
                                room.doors[2] = (o.doors[3] * o.size.x + o.position.x - o.size.x / 2 + room.size.x / 2 -
                                                 room.position.x) / room.size.x;
                                o.neighbors[3] = room;
                                room.neighbors[2] = o;
                            }
                            break;
                    }

                    var done = true;
                    for (int j = 0; j < map.Count; ++j) {
                        var other = map[j];
                        var dp = room.position - other.position;
                        var ds = (room.size + other.size) / 2;
                        if (Mathf.Abs(dp.x) < ds.x && Mathf.Abs(dp.y) < ds.y) {
                            done = false;
                            break;
                        }
                    }
                    if (done) {
                        map.Add(room);
                        break;
                    }
                    o.doors[i] = 0;
                    o.neighbors[i] = null;
                }
            }
        }

        var end = Furthest(map[0]);
        var start = Furthest(end);

        start.Spawn(Spawn.transform, 1);
        start.Spawn(terminal.transform, 2);

        var path = Path(start, end);
        var branches = path.Select(room => room.neighbors.Where(r => r != null && !path.Contains(r)).ToArray())
            .ToList();
        var lev = 0;
        var l = 0;
        foreach (var branch in branches) {
            l++;
            lev = l / 2;
            foreach (var room in branch) {
                if (room == null) continue;

                var p = room.neighbors.Single(path.Contains);
                int i = 0;
                while (room.neighbors[i] != p) i++;
                room.neighbors[i] = null;

                if (room.level == 0) {
                    room.Spawn(guard.transform, 1);
                    room.Spawn(civilian.transform, 0, 10);
                }

                var e = Furthest(room);
                e.spawns.Add(terminal.transform, lev + Random.Range(0, 2));
                if (Random.value < 0.4f * lev)
                    room.Spawn(guard.transform, lev);
                if (Random.value < 0.6f)
                    room.Spawn(cabinet.transform, lev);

                p.level = lev;
                Iterate(room, r => r.level = lev);

                room.neighbors[i] = p;
            }
        }
        end.Spawn(goal.transform, lev);

        for (int i = 0; i < map.Count; ++i)
            if (map[i].level == 0)
                map[i].level = map[i].neighbors.First(r => r != null).level;

        return map;
    }


    private HashSet<Networkable> nLog = new HashSet<Networkable>();

    private bool HasTerminal(Networkable n, int level) {
        if (n == null || nLog.Contains(n)) return false;
        if (n.GetComponent<Terminal>() && n.level >= level) return true;

        nLog.Add(n);

        for (int i = 0; i < n.neighbors.Count; ++i) {
            if (HasTerminal(n.neighbors[i], level)) return true;
        }
        return false;
    }

    private void Iterate(Room room, Action<Room> action) {
        if (room == null || room.neighbors == null || rLog.Contains(room))
            return;
        rLog.Add(room);
        action(room);

        for (int j = 0; j < room.neighbors.Length; j++) {
            var neighbor = room.neighbors[j];
            if (neighbor == null || rLog.Contains(neighbor)) continue;
            Iterate(neighbor, action);
        }
    }

    private List<Room> Path(Room a, Room b) {
        rLog.Clear();
        var result = new List<Room>();
        IterateP(a, b, result);
        return result;
    }

    private bool IterateP(Room room, Room end, List<Room> path) {
        if (room == null || room.neighbors == null || rLog.Contains(room)) {
            return false;
        }
        rLog.Add(room);

        path.Add(room);
        if (room == end) return true;

        for (int j = 0; j < room.neighbors.Length; j++) {
            var neighbor = room.neighbors[j];
            if (neighbor == null || rLog.Contains(neighbor)) continue;

            var v = IterateP(neighbor, end, path);
            if (v) return true;
        }
        path.Remove(room);
        return false;
    }

    private HashSet<Room> rLog = new HashSet<Room>();

    private Room Furthest(Room room) {
        rLog.Clear();
        Room result;
        IterateF(room, out result, 0);
        return result;
    }

    private int IterateF(Room room, out Room result, int i) {
        if (room == null || room.neighbors == null || rLog.Contains(room)) {
            result = room;
            return i;
        }
        rLog.Add(room);

        var max = 0;
        var mroom = room;

        for (int j = 0; j < room.neighbors.Length; j++) {
            var neighbor = room.neighbors[j];
            if (neighbor == null || rLog.Contains(neighbor)) continue;
            Room r;
            var v = IterateF(neighbor, out r, i + 1);
            if (v > max) {
                max = v;
                mroom = r;
            }
        }
        result = mroom;
        return Mathf.Max(max, i);
    }

    public void Remake() {
        generate = false;
        while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);

        var map = GetMap().OrderBy(room => room.level);
        var maxLevel = 0;

        foreach (var room in map) {
            var g = new GameObject("Room").transform;
            g.parent = transform;
            g.position = new Vector3(room.position.x, 0, room.position.y);
            g.tag = "Merge";

            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            floor.parent = g;
            floor.localScale = new Vector3(room.size.x, 0.1f, room.size.y);
            floor.localPosition = new Vector3(0, -0.05f, 0);
            floor.localRotation = Quaternion.identity;
            floor.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            floor.gameObject.layer = LayerMask.NameToLayer("Ground");

            var patrol = new GameObject(room.level + "-Patrol").transform;
            patrol.parent = transform;
            patrol.position = new Vector3(room.position.x, 0, room.position.y);
            patrol.tag = "Patrol";

            if (room.level > maxLevel) maxLevel = room.level;

            Networkable startDoor = null;
            foreach (Transform t in room.spawns.Keys) {
                var n = room.amounts.ContainsKey(t) ? room.amounts[t] : 1;

                for (int i = 0; i < n; i++) {
                    var spawn = Instantiate(t);
                    spawn.transform.position = g.position + new Vector3(
                                                   Random.Range(-room.size.x / 2 + wallThickness * 2,
                                                       room.size.x / 2 - wallThickness * 2),
                                                   0.5f,
                                                   Random.Range(-room.size.y / 2 + wallThickness * 2,
                                                       room.size.y / 2 - wallThickness * 2));
                    spawn.parent = transform;
                    var human = spawn.GetComponent<Human>();
                    var net = spawn.GetComponent<Networkable>();
                    if (human != null) human.level = room.spawns[t];
                    if (net != null) net.level = room.spawns[t];
                    if (net != null && room.spawns.ContainsKey(Spawn.transform))
                        startDoor = net;
                }
            }

            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(wallThickness, wallHeight,
                room.size.y * (1 - room.doors[0]) - (room.doors[0] != 0 ? doorWidth / 2 : 0));
            wall.localPosition = new Vector3(-room.size.x / 2 + wallThickness / 2, wallHeight / 2,
                room.size.y * room.doors[0] / 2 + (room.doors[0] != 0 ? doorWidth / 4 : 0));
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(wallThickness, wallHeight,
                room.size.y * (1 - room.doors[1]) - (room.doors[1] != 0 ? doorWidth / 2 : 0));
            wall.localPosition = new Vector3(room.size.x / 2 - wallThickness / 2, wallHeight / 2,
                room.size.y * room.doors[1] / 2 + (room.doors[1] != 0 ? doorWidth / 4 : 0));
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");

            wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(wallThickness, wallHeight,
                room.size.y * room.doors[0] - (room.doors[0] != 0 ? doorWidth / 2 : 0));
            wall.localPosition = new Vector3(-room.size.x / 2 + wallThickness / 2, wallHeight / 2,
                -room.size.y * (1 - room.doors[0]) / 2 - (room.doors[0] != 0 ? doorWidth / 4 : 0));
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(wallThickness, wallHeight,
                room.size.y * room.doors[1] - (room.doors[1] != 0 ? doorWidth / 2 : 0));
            wall.localPosition = new Vector3(room.size.x / 2 - wallThickness / 2, wallHeight / 2,
                -room.size.y * (1 - room.doors[1]) / 2 - (room.doors[1] != 0 ? doorWidth / 4 : 0));
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");


            wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(room.size.x * (1 - room.doors[2]) - (room.doors[2] != 0 ? doorWidth / 2 : 0),
                wallHeight, wallThickness);
            wall.localPosition = new Vector3(room.size.x * room.doors[2] / 2 + (room.doors[2] != 0 ? doorWidth / 4 : 0),
                wallHeight / 2, -room.size.y / 2 + wallThickness / 2);
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(room.size.x * (1 - room.doors[3]) - (room.doors[3] != 0 ? doorWidth / 2 : 0),
                wallHeight, wallThickness);
            wall.localPosition = new Vector3(room.size.x * room.doors[3] / 2 + (room.doors[3] != 0 ? doorWidth / 4 : 0),
                wallHeight / 2, room.size.y / 2 - wallThickness / 2);
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");

            wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(room.size.x * room.doors[2] - (room.doors[2] != 0 ? doorWidth / 2 : 0),
                wallHeight, wallThickness);
            wall.localPosition = new Vector3(
                -room.size.x * (1 - room.doors[2]) / 2 - (room.doors[2] != 0 ? doorWidth / 4 : 0), wallHeight / 2,
                -room.size.y / 2 + wallThickness / 2);
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
            wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            wall.parent = g;
            wall.localScale = new Vector3(room.size.x * room.doors[3] - (room.doors[3] != 0 ? doorWidth / 2 : 0),
                wallHeight, wallThickness);
            wall.localPosition = new Vector3(
                -room.size.x * (1 - room.doors[3]) / 2 - (room.doors[3] != 0 ? doorWidth / 4 : 0), wallHeight / 2,
                room.size.y / 2 - wallThickness / 2);
            wall.localRotation = Quaternion.identity;
            wall.GetComponent<MeshRenderer>().sharedMaterial = floorMaterial;
            wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");

            var lev = Mathf.Max(room.level - (Random.value < 0.4 ? 1 : 0), 0);
            for (int i = 0; i < 4; ++i) {
                if (room.neighbors[i] == null || room.level <= 1) continue;

                var p = Vector3.zero;
                switch (i) {
                    case 0:
                        p = new Vector3(
                            -room.size.x / 2,
                            doorHeight / 2,
                            room.size.y * (room.doors[0]) - room.size.y / 2);
                        break;
                    case 1:
                        p = new Vector3(
                            room.size.x / 2,
                            doorHeight / 2,
                            room.size.y * (room.doors[1]) - room.size.y / 2);
                        break;
                    case 2:
                        p = new Vector3(
                            room.size.x * (room.doors[2]) - room.size.x / 2,
                            doorHeight / 2,
                            -room.size.y / 2);
                        break;
                    case 3:
                        p = new Vector3(
                            room.size.x * (room.doors[3]) - room.size.x / 2,
                            doorHeight / 2,
                            room.size.y / 2);
                        break;
                }

                var already = FindObjectsOfType<Door>()
                    .SingleOrDefault(d => Vector3.Distance(p + floor.position, d.transform.position) < 1);
                Transform door;
                if (already == null) {
                    door = Instantiate(Door).transform;
                    door.parent = g;
                    door.localScale = new Vector3(doorWidth, doorHeight, wallThickness);
                    door.localPosition = p;
                    if (i < 2)
                        door.Rotate(0, 90, 0);
                    door.parent = transform;
                    door.GetComponent<Networkable>().level = lev;
                    if (startDoor != null) {
                        door.GetComponent<Networkable>().neighbors.Add(startDoor);
                        startDoor.neighbors.Add(door.GetComponent<Networkable>());
                        door.GetComponent<Networkable>().level = 2;
                    }
                } else {
                    door = already.transform;
                    if (startDoor != null) {
                        door.GetComponent<Networkable>().neighbors.Add(startDoor);
                        startDoor.neighbors.Add(door.GetComponent<Networkable>());
                        door.GetComponent<Networkable>().level = 2;
                    }
                }
            }
        }

        var network = FindObjectsOfType<Networkable>();
        for (int i = 0; i < maxLevel; ++i) {
            var nodes = network.Where(n => n.level == i).ToArray();

            for (int l = 0; l < nodes.Length; ++l) {
                if (nodes[l].neighbors.Count > 0) continue;
                var j = 0;
                do {
                    j = Random.Range(0, nodes.Length);
                } while (j == l);
                nodes[l].neighbors.Add(nodes[j]);
                nodes[j].neighbors.Add(nodes[l]);
            }
        }
        var cabinets = FindObjectsOfType<Cabinet>();
        for (int i = 0; i < cabinets.Length; ++i) {
            nLog.Clear();
            var n = cabinets[i].GetComponent<Networkable>();
            var l = 0;
            while (!HasTerminal(n, n.level) && l++ < 5) {
                var j = 0;
                do {
                    j = Random.Range(0, network.Length);
                } while (j == i && n.neighbors.Contains(network[j]));
                n.neighbors.Add(network[j]);
                network[j].neighbors.Add(n);
            }
        }

        for (int i = 0; i < network.Length; ++i) {
            if (network[i].neighbors.Count > 0) continue;
            var j = 0;
            do {
                j = Random.Range(0, network.Length);
            } while (j == i && network[i].neighbors.Contains(network[j]));
            network[i].neighbors.Add(network[j]);
            network[j].neighbors.Add(network[i]);
        }

        //var merger = new GameObject("Merger").AddComponent<MergeMesh>();
        //merger.transform.parent = transform;
        //merger.Merge(GameObject.FindGameObjectsWithTag("Merge").Select(g=>g.transform).ToArray());
    }
}

class Room {
    public Vector2 position;
    public Vector2 size;
    public Vector4 doors;
    public Room[] neighbors;
    public Dictionary<Transform, int> spawns = new Dictionary<Transform, int>();
    public Dictionary<Transform, int> amounts = new Dictionary<Transform, int>();
    public int level;

    public Room(Vector2 position, Vector2 size, Vector4 doors) {
        this.position = position;
        this.size = size;
        this.doors = doors;
        this.neighbors = new Room[4];
        this.level = 0;
    }

    public void Spawn(Transform t, int l, int n = 1) {
        if (!spawns.ContainsKey(t)) {
            spawns[t] = l;
            amounts[t] = n;
        } else {
            amounts[t] += n;
        }
    }
}