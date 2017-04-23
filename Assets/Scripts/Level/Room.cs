using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour {

    public Vector2 position;
    public Vector2 size;
    public Vector4 doorPos;
    public Door[] doors = new Door[4];
    public Room[] neighbors = new Room[4];
    public Dictionary<Transform, int> spawns = new Dictionary<Transform, int>();
    public Dictionary<Transform, int> amounts = new Dictionary<Transform, int>();
    public int level;

    private readonly HashSet<Room> rLog = new HashSet<Room>();


	private void Start () {
		
	}

	public void Generate (Level.RoomSettings settings) {
	    transform.position = new Vector3(position.x, 0, position.y);
	    transform.tag = "Merge";

	    var floor = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    floor.parent = transform;
	    floor.localScale = new Vector3(size.x, 0.1f, size.y);
	    floor.localPosition = new Vector3(0, -0.05f, 0);
	    floor.localRotation = Quaternion.identity;
	    floor.GetComponent<MeshRenderer>().sharedMaterial = settings.floorMaterial;
	    floor.gameObject.layer = LayerMask.NameToLayer("Ground");

	    var patrol = new GameObject(level + "-Patrol").transform;
	    patrol.parent = transform;
	    patrol.position = new Vector3(position.x, 0, position.y);
	    patrol.tag = "Patrol";

	    Networkable startDoor = null;
	    foreach (Transform t in spawns.Keys) {
	        var n = amounts.ContainsKey(t) ? amounts[t] : 1;

	        for (int i = 0; i < n; i++) {
	            var spawn = Instantiate(t);
	            spawn.transform.position = transform.position + new Vector3(
	                                           Random.Range(-size.x / 2 + settings.wallThickness * 2,
	                                               size.x / 2 - settings.wallThickness * 2),
	                                           0.5f,
	                                           Random.Range(-size.y / 2 + settings.wallThickness * 2,
	                                               size.y / 2 - settings.wallThickness * 2));
	            spawn.parent = transform;
	            var human = spawn.GetComponent<Human>();
	            var net = spawn.GetComponent<Networkable>();
	            if (human != null) human.level = spawns[t];
	            if (net != null) net.level = spawns[t];
	            if (net != null && spawns.ContainsKey(settings.spawn.transform))
	                startDoor = net;
	        }
	    }

	    var lamp = new GameObject("Lamp", typeof(Light)).GetComponent<Light>();
	    lamp.transform.parent = transform;
	    lamp.transform.localPosition = Vector3.up * settings.lampHeight * Mathf.Min(size.x, size.y) / 10;
	    lamp.transform.LookAt(transform);
	    lamp.range = settings.lampHeight * 2 * Mathf.Min(size.x, size.y) / 10;
	    lamp.spotAngle = 110;
	    lamp.intensity = 8;
	    lamp.renderMode = LightRenderMode.ForcePixel;
	    lamp.type = LightType.Spot;
	    lamp.color = level == 0 ? settings.goodColor : settings.badColor;
	    lamp.cullingMask = ~(1 << LayerMask.NameToLayer("Cover"));


	    var trigger = gameObject.AddComponent<BoxCollider>();
	    trigger.isTrigger = true;
	    trigger.size = new Vector3(size.x, settings.wallHeight, size.y);
	    trigger.center = Vector3.up * settings.wallHeight/2;


	    var wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(settings.wallThickness, settings.wallHeight,
	        size.y * (1 - doorPos[0]) - (doorPos[0] != 0 ? settings.doorWidth / 2 : 0));
	    wall.localPosition = new Vector3(-size.x / 2 + settings.wallThickness / 2, settings.wallHeight / 2,
	        size.y * doorPos[0] / 2 + (doorPos[0] != 0 ? settings.doorWidth / 4 : 0));
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
	    wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(settings.wallThickness, settings.wallHeight,
	        size.y * (1 - doorPos[1]) - (doorPos[1] != 0 ? settings.doorWidth / 2 : 0));
	    wall.localPosition = new Vector3(size.x / 2 - settings.wallThickness / 2, settings.wallHeight / 2,
	        size.y * doorPos[1] / 2 + (doorPos[1] != 0 ? settings.doorWidth / 4 : 0));
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");

	    wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(settings.wallThickness, settings.wallHeight,
	        size.y * doorPos[0] - (doorPos[0] != 0 ? settings.doorWidth / 2 : 0));
	    wall.localPosition = new Vector3(-size.x / 2 + settings.wallThickness / 2, settings.wallHeight / 2,
	        -size.y * (1 - doorPos[0]) / 2 - (doorPos[0] != 0 ? settings.doorWidth / 4 : 0));
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
	    wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(settings.wallThickness, settings.wallHeight,
	        size.y * doorPos[1] - (doorPos[1] != 0 ? settings.doorWidth / 2 : 0));
	    wall.localPosition = new Vector3(size.x / 2 - settings.wallThickness / 2, settings.wallHeight / 2,
	        -size.y * (1 - doorPos[1]) / 2 - (doorPos[1] != 0 ? settings.doorWidth / 4 : 0));
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");


	    wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(size.x * (1 - doorPos[2]) - (doorPos[2] != 0 ? settings.doorWidth / 2 : 0),
	        settings.wallHeight, settings.wallThickness);
	    wall.localPosition = new Vector3(size.x * doorPos[2] / 2 + (doorPos[2] != 0 ? settings.doorWidth / 4 : 0),
	        settings.wallHeight / 2, -size.y / 2 + settings.wallThickness / 2);
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
	    wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(size.x * (1 - doorPos[3]) - (doorPos[3] != 0 ? settings.doorWidth / 2 : 0),
	        settings.wallHeight, settings.wallThickness);
	    wall.localPosition = new Vector3(size.x * doorPos[3] / 2 + (doorPos[3] != 0 ? settings.doorWidth / 4 : 0),
	        settings.wallHeight / 2, size.y / 2 - settings.wallThickness / 2);
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");

	    wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(size.x * doorPos[2] - (doorPos[2] != 0 ? settings.doorWidth / 2 : 0),
	        settings.wallHeight, settings.wallThickness);
	    wall.localPosition = new Vector3(
	        -size.x * (1 - doorPos[2]) / 2 - (doorPos[2] != 0 ? settings.doorWidth / 4 : 0), settings.wallHeight / 2,
	        -size.y / 2 + settings.wallThickness / 2);
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");
	    wall = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
	    wall.parent = transform;
	    wall.localScale = new Vector3(size.x * doorPos[3] - (doorPos[3] != 0 ? settings.doorWidth / 2 : 0),
	        settings.wallHeight, settings.wallThickness);
	    wall.localPosition = new Vector3(
	        -size.x * (1 - doorPos[3]) / 2 - (doorPos[3] != 0 ? settings.doorWidth / 4 : 0), settings.wallHeight / 2,
	        size.y / 2 - settings.wallThickness / 2);
	    wall.localRotation = Quaternion.identity;
	    wall.GetComponent<MeshRenderer>().sharedMaterial = settings.wallMaterial;
	    wall.gameObject.layer = LayerMask.NameToLayer("Obstacles");

	    var lev = Mathf.Max(level - (Random.value < 0.4 ? 1 : 0), 0);
	    for (int i = 0; i < 4; ++i) {
	        if (neighbors[i] == null) continue;

	        var p = Vector3.zero;
	        switch (i) {
	            case 0:
	                p = new Vector3(
	                    -size.x / 2,
	                    settings.doorHeight / 2,
	                    size.y * (doorPos[0]) - size.y / 2);
	                break;
	            case 1:
	                p = new Vector3(
	                    size.x / 2,
	                    settings.doorHeight / 2,
	                    size.y * (doorPos[1]) - size.y / 2);
	                break;
	            case 2:
	                p = new Vector3(
	                    size.x * (doorPos[2]) - size.x / 2,
	                    settings.doorHeight / 2,
	                    -size.y / 2);
	                break;
	            case 3:
	                p = new Vector3(
	                    size.x * (doorPos[3]) - size.x / 2,
	                    settings.doorHeight / 2,
	                    size.y / 2);
	                break;
	        }

	        var already = FindObjectsOfType<Door>().SingleOrDefault(d => Vector3.Distance(p + floor.position, d.transform.position) < 1);
	        Transform door;
	        if (already == null) {
	            door = Instantiate(settings.door).transform;
	            door.parent = transform;
	            door.localScale = new Vector3(settings.doorWidth, settings.doorHeight, settings.wallThickness);
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

    public void Populate() {

    }

    public void Spawn(Transform t, int l, int n = 1) {
        if (!spawns.ContainsKey(t)) {
            spawns[t] = l;
            amounts[t] = n;
        } else {
            amounts[t] += n;
        }
    }

    public void Iterate(Action<Room> action) {
        if (neighbors == null || rLog.Contains(this))
            return;
        rLog.Add(this);
        action(this);

        foreach (var neighbor in neighbors) {
            if (neighbor == null || rLog.Contains(neighbor)) continue;
            neighbor.Iterate(action);
        }
    }

    public List<Room> Path(Room other) {
        rLog.Clear();
        var result = new List<Room>();
        IterateP(this, other, result);
        return result;
    }

    private bool IterateP(Room room, Room end, List<Room> path) {
        if (room == null || room.neighbors == null || rLog.Contains(room)) {
            return false;
        }
        rLog.Add(room);

        path.Add(room);
        if (room == end) return true;

        foreach (var neighbor in room.neighbors) {
            if (neighbor == null || rLog.Contains(neighbor)) continue;

            var v = IterateP(neighbor, end, path);
            if (v) return true;
        }
        path.Remove(room);
        return false;
    }


    public Room Furthest() {
        rLog.Clear();
        Room result;
        IterateF(this, out result, 0);
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

        foreach (var neighbor in room.neighbors) {
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

    public void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
            other.GetComponent<Player>().roomLevel = level;
    }

    public static Room Create(Vector2 position, Vector2 size){
        var room = new GameObject("Room "+0, typeof(Room)).GetComponent<Room>();
        room.position = position;
        room.size = size;
        return room;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position+Vector3.up*(level+2), new Vector3(0.2f, (level+2)*2, 0.2f));
    }
}
