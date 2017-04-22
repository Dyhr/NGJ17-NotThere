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

    public Vector2 minRoomSize;
    public Vector2 maxRoomSize;

    public RoomSettings roomSettings;

    private void Start() {
        if (generate) Remake();
    }

    private void OnDrawGizmos() {
        if (generate) Remake();
    }

    private List<Room> GetMap() {
        var map = new List<Room>();

        map.Add(Room.Create(Vector2.zero, maxRoomSize));

        for (int k = 0; k < rooms; ++k) {
            var o = map[Random.Range(0, map.Count)];
            for (int i = 0; i < 4; ++i) {
                if (o.doorPos[i] != 0) continue;
                for (int t = 0; t < tries; ++t) {
                    var room = Room.Create(o.position, new Vector2(
                        Random.Range(minRoomSize.x, maxRoomSize.x),
                        Random.Range(minRoomSize.y, maxRoomSize.y)));

                    var sx = Mathf.Min(o.size.x - roomSettings.doorWidth - roomSettings.wallThickness,
                        room.size.x - roomSettings.doorWidth - roomSettings.wallThickness);
                    var sy = Mathf.Min(o.size.y - roomSettings.doorWidth - roomSettings.wallThickness,
                        room.size.y - roomSettings.doorWidth - roomSettings.wallThickness);

                    float min, max;
                    switch (i) {
                        case 0:
                            room.position -= Vector2.right * (o.size.x + room.size.x) / 2;
                            room.position += Vector2.up * Random.Range(-sy, sy);

                            min = (room.position.y - o.position.y - room.size.y / 2 + o.size.y / 2 +
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.y;
                            max = (room.position.y - o.position.y + room.size.y / 2 + o.size.y / 2 -
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.y;
                            min = Mathf.Clamp(min, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y,
                                1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y);
                            max = Mathf.Clamp(max, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y,
                                1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doorPos[0] = Random.Range(min, max);
                                room.doorPos[1] = (o.doorPos[0] * o.size.y + o.position.y - o.size.y / 2 +
                                                   room.size.y / 2 -
                                                   room.position.y) / room.size.y;
                                o.neighbors[0] = room;
                                room.neighbors[1] = o;
                            }
                            break;
                        case 1:
                            room.position += Vector2.right * (o.size.x + room.size.x) / 2;
                            room.position += Vector2.up * Random.Range(-sy, sy);

                            min = (room.position.y - o.position.y - room.size.y / 2 + o.size.y / 2 +
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.y;
                            max = (room.position.y - o.position.y + room.size.y / 2 + o.size.y / 2 -
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.y;
                            min = Mathf.Clamp(min, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y,
                                1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y);
                            max = Mathf.Clamp(max, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y,
                                1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.y);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doorPos[1] = Random.Range(min, max);
                                room.doorPos[0] = (o.doorPos[1] * o.size.y + o.position.y - o.size.y / 2 +
                                                   room.size.y / 2 -
                                                   room.position.y) / room.size.y;
                                o.neighbors[1] = room;
                                room.neighbors[0] = o;
                            }
                            break;
                        case 2:
                            room.position -= Vector2.up * (o.size.y + room.size.y) / 2;
                            room.position += Vector2.right * Random.Range(-sx, sx);

                            min = (room.position.x - o.position.x - room.size.x / 2 + o.size.x / 2 +
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.x;
                            max = (room.position.x - o.position.x + room.size.x / 2 + o.size.x / 2 -
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.x;
                            min = Mathf.Clamp(min, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x,
                                1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x);
                            max = Mathf.Clamp(max, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x,
                            1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doorPos[2] = Random.Range(min, max);
                                room.doorPos[3] = (o.doorPos[2] * o.size.x + o.position.x - o.size.x / 2 +
                                                   room.size.x / 2 -
                                                   room.position.x) / room.size.x;
                                o.neighbors[2] = room;
                                room.neighbors[3] = o;
                            }
                            break;
                        case 3:
                            room.position += Vector2.up * (o.size.y + room.size.y) / 2;
                            room.position += Vector2.right * Random.Range(-sx, sx);

                            min = (room.position.x - o.position.x - room.size.x / 2 + o.size.x / 2 +
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.x;
                            max = (room.position.x - o.position.x + room.size.x / 2 + o.size.x / 2 -
                                   (roomSettings.wallThickness + roomSettings.doorWidth / 2)) / o.size.x;
                            min = Mathf.Clamp(min, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x,
                                1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x);
                            max = Mathf.Clamp(max, (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x,
                                1 - (roomSettings.wallThickness + roomSettings.doorWidth / 2) / o.size.x);
                            if (!(min == max && (max == 0 || max == 1))) {
                                o.doorPos[3] = Random.Range(min, max);
                                room.doorPos[2] = (o.doorPos[3] * o.size.x + o.position.x - o.size.x / 2 +
                                                   room.size.x / 2 -
                                                   room.position.x) / room.size.x;
                                o.neighbors[3] = room;
                                room.neighbors[2] = o;
                            }
                            break;
                    }

                    var done = true;
                    foreach (var other in map) {
                        var dp = room.position - other.position;
                        var ds = (room.size + other.size) / 2;
                        if (!(Mathf.Abs(dp.x) < ds.x) || !(Mathf.Abs(dp.y) < ds.y)) continue;

                        done = false;
                        break;
                    }
                    if (done) {
                        map.Add(room);
                        break;
                    }
                    DestroyImmediate(room.gameObject);
                    o.doorPos[i] = 0;
                    o.neighbors[i] = null;
                }
            }
        }

        var end = map[0].Furthest();
        var start = end.Furthest();

        start.Spawn(roomSettings.spawn.transform, 1);
        start.Spawn(roomSettings.terminal.transform, 2);

        var path = start.Path(end);
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
                    room.Spawn(roomSettings.guard.transform, 1);
                    room.Spawn(roomSettings.civilian.transform, 0, 10);
                }

                var e = room.Furthest();
                e.spawns.Add(roomSettings.terminal.transform, lev + Random.Range(0, 2));
                if (Random.value < 0.4f * lev)
                    room.Spawn(roomSettings.guard.transform, lev);
                if (Random.value < 0.6f)
                    room.Spawn(roomSettings.cabinet.transform, lev);

                p.level = lev;
                room.Iterate(r => r.level = lev);

                room.neighbors[i] = p;
            }
        }
        end.Spawn(roomSettings.goal.transform, lev);

        foreach (Room room in map)
            if (room.level == 0)
                room.level = room.neighbors.First(r => r != null).level;

        return map;
    }


    private readonly HashSet<Networkable> nLog = new HashSet<Networkable>();

    private bool HasTerminal(Networkable n, int level) {
        if (n == null || nLog.Contains(n)) return false;
        if (n.GetComponent<Terminal>() && n.level >= level) return true;

        nLog.Add(n);

        for (int i = 0; i < n.neighbors.Count; ++i) {
            if (HasTerminal(n.neighbors[i], level)) return true;
        }
        return false;
    }

    public void Remake() {
        generate = false;
        while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);

        var map = GetMap().OrderBy(room => room.level);
        var maxLevel = 0;

        foreach (var room in map) {
            room.transform.parent = transform;
            if (room.level > maxLevel) maxLevel = room.level;
            room.Generate(roomSettings);
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

    [Serializable]
    public class RoomSettings {
        public float wallThickness = 0.5f;
        public float wallHeight = 2;
        public float doorWidth = 2;
        public float doorHeight = 2;
        public Material wallMaterial;
        public Material floorMaterial;
        public Door door;
        public Guard guard;
        public Civilian civilian;
        public Terminal terminal;
        public Transform spawn;
        public Transform goal;
        public Transform cabinet;
    }
}