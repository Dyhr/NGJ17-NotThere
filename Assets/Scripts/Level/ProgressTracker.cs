using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ProgressTracker : MonoBehaviour
{

    public static ProgressTracker instance;

    public int level = 0;
    public static int[] seeds;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StatusCheck());

        var now = System.DateTime.Now;
        Random.seed = now.Millisecond * now.Minute + now.Second * now.Hour;

        if(seeds == null)
            seeds = new[]
            {
                Random.Range(int.MinValue, int.MaxValue),
                Random.Range(int.MinValue, int.MaxValue),
                Random.Range(int.MinValue, int.MaxValue),
            };

        Rebuild();
    }

    public void Finish()
    {
        StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1.5f);
        level++;
        FindObjectOfType<Level>().rooms += 6;
        if (level < seeds.Length)
            Rebuild();
        else
            Winner();
    }

    private void Winner()
    {
        // TODO win the game
    }

    private void Rebuild()
    {
        if (seeds.Length < level) return;
        SoftReset();
        Random.seed = seeds[level];
        FindObjectOfType<Level>().Remake();
        FindObjectOfType<AstarPath>().Scan();
        foreach (var guard in FindObjectsOfType<Guard>())
            guard.Interrupt();
        foreach (var civ in FindObjectsOfType<Civilian>())
            civ.Interrupt();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Restart"))
        {
            HardReset();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void OnLevelWasLoaded(int level)
    {
        Rebuild();
    }

    private void HardReset()
    {
        SoftReset();
        Label.instance = null;
        Application.LoadLevel(0);
    }
    private void SoftReset()
    {
        Guard.switches = null;
        Guard.player = null;
        Guard.playerr = null;
        Guard.guards = null;
        Guard.Patrols.Clear();
    }

    IEnumerator StatusCheck()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            yield return new WaitForSeconds(1/2f);
            if (GameObject.FindWithTag("Player") == null) {
                yield return new WaitForSeconds(3);
                HardReset();
                yield return new WaitForSeconds(5);
            }
            // TODO show death screen
        }
    }
}
