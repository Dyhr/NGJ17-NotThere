﻿using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Human : MonoBehaviour
{
    public float height;
    public float speed;

    public float hp;
    public float armor;
    public int level;

    public Creds Creds
    {
        get { return new Creds(name, level, this); }
    }

    public AnimationCurve aimCurve;
    public float aimTimeMul;

    [HideInInspector]
    public Vector3 inputControl;
    [HideInInspector]
    public bool inputAim;
    [HideInInspector]
    public bool inputFire;

    [HideInInspector]
    public Vector3 forward = Vector3.forward;
    [HideInInspector]
    public Vector3 right = Vector3.right;

    [HideInInspector]
    public Action idleLook;
    [HideInInspector]
    public bool lockRot;

    private Rigidbody _rigidbody;
    private float aimTime;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (hp <= 0) {
            if (GetComponent<Guard>()) GetComponent<Guard>().PleaseDie();
            Destroy(gameObject);
            return;
        }

        var move = (forward*inputControl.z + right*inputControl.x).normalized;
        if (inputControl.magnitude == 0)
        {
            if (idleLook != null) idleLook();
        }
        else if (!lockRot)
        {
            if(move.magnitude > 0)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move), 0.4f);
        }
        _rigidbody.velocity = move*speed;

        var gun = GetComponentInChildren<Gun>();
        if (gun != null) {
            aimTime += (inputAim ? -Time.deltaTime : Time.deltaTime) * aimTimeMul;
            aimTime = Mathf.Clamp01(aimTime);
            gun.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(90, 0, 0), Quaternion.Euler(0, 0, 0), aimCurve.Evaluate(aimTime));
            if (inputFire && aimTime == 0)
            {
                gun.Fire();
            }
        }

        inputControl = Vector3.zero;
        inputFire = false;
        inputAim = false;
        lockRot = false;
    }
}
