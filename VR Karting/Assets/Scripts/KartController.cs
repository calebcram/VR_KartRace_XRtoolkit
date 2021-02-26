using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;

public class KartController : MonoBehaviour
{
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    public List<ParticleSystem> primaryParticles = new List<ParticleSystem>();
    public List<ParticleSystem> secondaryParticles = new List<ParticleSystem>();

    float speed, currentSpeed;
    float rotate, currentRotate;
    int driftDirection;
    float driftPower;
    int driftMode = 0;
    bool first, second, third;
    Color c;

    [Header("INPUT")]
    public float turnInput;
    public bool speedInput;
    public bool driftInput;

    [Header("Bools")]
    public bool drifting;

    [Header("Parameters")]

    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;
    public float steerAnimationSpeed = 0.1f;
    public float steerAnimationAmount = 10;
    public float driftSteer = 2;
    public float driftKartAnimationSpeed = 0.1f;
    public float driftKartRotationAdd = 15;
    public float driftRecoverRotationDuration = 1;
    [Header("Model Parts")]

    public Transform[] frontWheels;
    public Transform[] backWheels;
    public Transform steeringWheel;

    [Header("Particles")]
    public Transform wheelParticles;
    public Transform flashParticles;
    public Color[] turboColors;

    void Start()
    {

        for (int i = 0; i < wheelParticles.GetChild(0).childCount; i++)
        {
            primaryParticles.Add(wheelParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < wheelParticles.GetChild(1).childCount; i++)
        {
            primaryParticles.Add(wheelParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }

        foreach(ParticleSystem p in flashParticles.GetComponentsInChildren<ParticleSystem>())
        {
            secondaryParticles.Add(p);
        }
    }

    void Update()
    {        
        //Follow Collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        //Accelerate
        if (speedInput)
            speed = acceleration;

        //Steer
        if (turnInput != 0)
        {
            int dir = turnInput > 0 ? 1 : -1;
            float amount = Mathf.Abs((turnInput));
            Steer(dir, amount);
        }

        //Drift
        if (driftInput && !drifting && turnInput != 0)
        {
            drifting = true;
            driftDirection = turnInput > 0 ? 1 : -1;

            foreach (ParticleSystem p in primaryParticles)
            {
                p.startColor = Color.clear;
                p.Play();
            }

            //kartModel.parent.DOComplete();
            //kartModel.parent.DOPunchPosition(transform.up * .2f, .3f, 5, 1);

        }

        if (drifting)
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(turnInput, -1, 1, 0, driftSteer) : ExtensionMethods.Remap(turnInput, -1, 1, driftSteer, 0);
            float powerControl = (driftDirection == 1) ? ExtensionMethods.Remap(turnInput, -1, 1, .2f, 1) : ExtensionMethods.Remap(turnInput, -1, 1, 1, .2f);
            Steer(driftDirection, control);
            driftPower += powerControl;

            ColorDrift();
        }

        if (driftInput && drifting)
        {
            Boost();
        }

        //currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        //currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;
        currentSpeed = speed;speed = 0;
        currentRotate = rotate;rotate = 0;
        //Animations    

        //a) Kart
        if (!drifting)
        {
            kartModel.localEulerAngles = Vector3.Lerp(kartModel.localEulerAngles, new Vector3(0, 90 + (turnInput * steerAnimationAmount), kartModel.localEulerAngles.z), steerAnimationSpeed);
        }
        else
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(turnInput, -1, 1, .5f, driftSteer) : ExtensionMethods.Remap(turnInput, -1, 1, driftSteer, .5f);
            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y,(control * driftKartRotationAdd) * driftDirection, driftKartAnimationSpeed), 0);
        }

        //b) Wheels
        foreach (var item in frontWheels)
        {
            item.localEulerAngles = new Vector3(0, (turnInput * steering), item.localEulerAngles.z);
            item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);
        }
        foreach (var item in backWheels)
        {
            item.localEulerAngles += new Vector3(sphere.velocity.magnitude / 2, 0, 0);
        }

        //c) Steering Wheel
        if(steeringWheel)
            steeringWheel.localEulerAngles = new Vector3(-25, 90, (turnInput * 45));
    }

    private void FixedUpdate()
    {
        //Forward Acceleration
        if(!drifting)
            sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        //Gravity
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        //Steering
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up*.1f), Vector3.down, out hitOn, 1.1f,layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f)   , Vector3.down, out hitNear, 2.0f, layerMask);

        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Boost()
    {
        drifting = false;

        if (driftMode > 0)
        {
            DOVirtual.Float(currentSpeed * 3, currentSpeed, .3f * driftMode, Speed);
            DOVirtual.Float(0, 1, .5f, ChromaticAmount).OnComplete(() => DOVirtual.Float(1, 0, .5f, ChromaticAmount));
            kartModel.Find("Tube001").GetComponentInChildren<ParticleSystem>().Play();
            kartModel.Find("Tube002").GetComponentInChildren<ParticleSystem>().Play();
        }

        driftPower = 0;
        driftMode = 0;
        first = false; second = false; third = false;

        foreach (ParticleSystem p in primaryParticles)
        {
            p.startColor = Color.clear;
            p.Stop();
        }

        kartModel.parent.DOLocalRotate(Vector3.zero, driftRecoverRotationDuration).SetEase(Ease.OutBack);

    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }

    public void ColorDrift()
    {
        if(!first)
            c = Color.clear;

        if (driftPower > 50 && driftPower < 100-1 && !first)
        {
            first = true;
            c = turboColors[0];
            driftMode = 1;

            PlayFlashParticle(c);
        }

        if (driftPower > 100 && driftPower < 150- 1 && !second)
        {
            second = true;
            c = turboColors[1];
            driftMode = 2;

            PlayFlashParticle(c);
        }

        if (driftPower > 150 && !third)
        {
            third = true;
            c = turboColors[2];
            driftMode = 3;

            PlayFlashParticle(c);
        }

        foreach (ParticleSystem p in primaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
        }

        foreach(ParticleSystem p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
        }
    }

    void PlayFlashParticle(Color c)
    {
        return;
        GameObject.Find("CM vcam1").GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        foreach (ParticleSystem p in secondaryParticles)
        {
            var pmain = p.main;
            pmain.startColor = c;
            p.Play();
        }
    }

    public void SetAcceleration(float x)
    {
        acceleration = x;
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }

    private void Speed(float x)
    {
        currentSpeed = x;
    }

    void ChromaticAmount(float x)
    {
        postProfile.GetSetting<ChromaticAberration>().intensity.value = x;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    //}
}
