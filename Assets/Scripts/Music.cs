using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {

    public AudioSource safeSource, tenseSource, chaseSource;

    public int level;
    public float lerpSpeed = 0.05f;
    public float volume = 0.15f;
    private float intensity;

	private void Update () {
	    intensity = Mathf.Lerp(intensity, level, lerpSpeed);

	    safeSource.volume = Mathf.Clamp(1 - Mathf.Abs(0 - intensity), 0, volume);
	    tenseSource.volume = Mathf.Clamp(1 - Mathf.Abs(1 - intensity), 0, volume);
	    chaseSource.volume = Mathf.Clamp(1 - Mathf.Abs(2 - intensity), 0, volume);
	}
}
