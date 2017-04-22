using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

[RequireComponent(typeof(Human))]
public class Player : MonoBehaviour
{
    public Camera Camera;
    public float activateDistance;

    public Material nodeMaterial;
    public Material planeMaterial;
    public Material wallMaterial;
    public Transform nodeLabel;
    public AudioClip teleClip;

    public bool invisible;

    private Color initialColor;
    private HackMode hacker;
    private Networkable hack;
    public Networkable Hack
    {
        set
        {
            hack = value;
            if (hack != null && hacker == null) {
                hacker = new GameObject("Hacker").AddComponent<HackMode>();
                hacker.nodeMaterial = nodeMaterial;
                hacker.planeMaterial = planeMaterial;
                hacker.nodeLabel = nodeLabel;
                hacker.Origin = value;
            }
            if(hack == null && hacker != null)
                Destroy(hacker.gameObject);
        }
        get { return hack; }
    }

    private RaycastHit hit;
    private Human human;
    private GameObject[] switches;

    private void Start() {
        initialColor = GetComponentInChildren<Renderer>().material.color;

        if (Guard.player == null)
            Guard.player = GameObject.FindGameObjectWithTag("Player").transform;
        if (Guard.playerr == null)
            Guard.playerr = Guard.player.GetComponent<Rigidbody>();

        if (Camera == null) Camera = Camera.main;
        human = GetComponent<Human>();
        human.idleLook = () =>
        {
            if (Hack == null)
            {
                if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    var m = hit.point + transform.right * 0.25f;
                    m.y = transform.position.y;
                    transform.LookAt(m); // TODO lerp
                }
            }
        };
        human.forward = Camera.transform.forward;
        human.forward.y = 0;
        human.right = Camera.transform.right;
        human.right.y = 0;
        switches = GameObject.FindGameObjectsWithTag("Switch");

        GetComponent<AudioSource>().PlayOneShot(teleClip, 0.8f);
    }

    private void Update()
    {
        if (Hack == null)
        {
            human.inputControl = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            human.inputFire = Input.GetButtonDown("Fire1") && Input.GetButton("Fire2") && false;
            human.inputAim = Input.GetButton("Fire2") && false;
            if (Input.GetKeyDown(KeyCode.E))
                invisible = !invisible;
            if (human.inputControl.magnitude > 0) invisible = false;

            Camera.transform.position = transform.position - Camera.transform.forward*40;
            if (Input.GetAxisRaw("Fire2") > 0) human.idleLook();

            Cursor.lockState = CursorLockMode.None;
        }
        else {
            invisible = false;
            Camera.transform.position += human.right*Input.GetAxisRaw("Mouse X") +
                                         human.forward*Input.GetAxisRaw("Mouse Y");

            Cursor.lockState = CursorLockMode.Locked;
        }

        var render = GetComponentInChildren<Renderer>();
        var targetColor = wallMaterial.color;
        targetColor.a = 0f;
        render.material.color = Color.Lerp(render.material.color, invisible ? targetColor : initialColor, 0.05f);

        if (Label.instance != null)
            Label.instance.Target = null;
        var s = switches.OrderBy(h => Vector3.Distance(transform.position, h.transform.position)).FirstOrDefault();
        if (s != null && Vector3.Distance(transform.position, s.transform.position) <= activateDistance)
        {
            var item = s.GetComponent<Networkable>();
            if (item != null)
            {
                if (Label.instance != null && Hack == null)
                {
                    Label.instance.Target = s.transform;
                    Label.instance.text.text = item.Name + "\nLevel " + NetNode.Roman(item.level) + "\n" + item.action;
                }
                if (Input.GetButtonDown("Action"))
                {
                    s.transform.SendMessage("Activate", human.Creds);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (Label.instance != null)
            Label.instance.Target = null;
        Hack = null;
        Destroy(FindObjectOfType<Hud>());
    }
}
