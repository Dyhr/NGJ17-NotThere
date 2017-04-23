using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Music : MonoBehaviour {

    public AudioSource safeSource, tenseSource, chaseSource;

    public int level;
    public float lerpSpeed = 0.05f;
    public float volume = 0.15f;
    private float intensity;

    private Player player;
    private Guard[] guards;

    private void Start() {
        guards = FindObjectsOfType<Guard>();
        player = FindObjectOfType<Player>();
    }

	private void Update () {

	    // Determine the level
	    level = 0;
	    if (player.roomLevel > 0 || player.Hack != null) level = 1;
	    if (guards.Any(g => g.canSeePlayer)) level = 2;

	    // Apply the level
	    intensity = Mathf.Lerp(intensity, level, lerpSpeed);

	    safeSource.volume = Mathf.Clamp(1 - Mathf.Abs(0 - intensity), 0, volume);
	    tenseSource.volume = Mathf.Clamp(1 - Mathf.Abs(1 - intensity), 0, volume);
	    chaseSource.volume = Mathf.Clamp(1 - Mathf.Abs(2 - intensity), 0, volume);
	}
}
