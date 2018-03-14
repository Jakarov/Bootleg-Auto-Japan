﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour {

    //Customizable Variables
    [Header("Customizable Aspects")]
    public int ammo = 200;
    public bool reloadWhileScoped = false;
    public float reloadTimer;
    [Range(0, 0.1f)]
    public float minRecoil = 0.1f;
    [Range(0, 0.5f)]
    public float maxRecoil = 0.1f;
    [Range(0, 1)]
    public float cameraShakeIntensity = 1f;
    public float cameraShakeAmount = 100;
    public float spread;
    public float originalCooldown;
    public int amount;
    public bool useCooldown;
    public bool semiAuto;

    //Reliabilities
    [Header("Reliabilites")]
    public ParticleSystem[] particleSystemArray;
    public Camera mainCamera;
    CameraController cameraController;
    public Text ammoText;
    public GameObject emChamber;

    //Local Variables
    Scope scope;
    bool reloading = false;
    float cooldown; //Saves original value of cooldown

    //Global Variables
    [HideInInspector]
    public bool scoped = false;

	// Use this for initialization
	void Start () {
        scope = gameObject.GetComponent<Scope>();//Acheive scope script
        cameraController = mainCamera.GetComponent<CameraController>(); //Getting CameraController
        cooldown = originalCooldown;
	}

    // Update is called once per frame
    void Update() {
        ScopeCheck(); //Checking for scope
        cooldown -= Time.deltaTime;
        if (!useCooldown)
        {
            if (Input.GetMouseButton(0) && ammo > 0 && !reloading) //Check if it's suitable to shoot
            {
                Shoot();
            }
            else
            {
                mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, 0); //Reseting camera shake
            }
        }
        else if(semiAuto)
        {

            if (Input.GetMouseButtonDown(0) && ammo > 0 && !reloading && cooldown < 0) //Check if it's suitable to shoot
            {
                Shoot();
            }
            else
            {
                mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, 0); //Reseting camera shake
            }
        } else if(!semiAuto)
        {

            if (Input.GetMouseButton(0) && ammo > 0 && !reloading && cooldown < 0) //Check if it's suitable to shoot
            {
                Shoot();
            }
            else
            {
                mainCamera.transform.eulerAngles = new Vector3(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, 0); //Reseting camera shake
            }

        }
        Reload();//Reload check
    }

    void ScopeCheck() //Go to scope script for more details
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            scope.ScopeIn();
            scoped = true;
        }else 
        {
            scope.ScopeOut();
            scoped = false;
        }

    }

    void Reload() //Go to reload script for more details
    {
        if (reloadWhileScoped) //Check if you can reload while scoped
        {
            if (Input.GetKeyDown(KeyCode.R) || ammo == 0 && !reloading) 
            {
                reloading = true;
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.R) || ammo == 0 && !reloading && !scoped)
            {
                reloading = true;
            }

        }

        if(reloading)
        {
            reloadTimer -= Time.deltaTime;
            ammoText.text = "Reloading...";
        }
        if(reloadTimer<= 0)
        {
            reloadTimer = 1f;
            reloading = false;
            ammo = 200;        //Reset everything
            ammoText.text = ammo.ToString();
        }
    }
    public void Shoot()
    {
        emChamber.GetComponent<Shoot>().TriggerPull(amount/*amount of bullets per shot*/, emChamber.transform/*emmision chamber*/, particleSystemArray[0]/*getting particle system in gun*/, spread);
        ammo -= amount; //Remove ammo
        cameraController.rotationY -= Random.Range(minRecoil * amount, maxRecoil * amount); //Adding recoil
        cameraController.rotationX += Random.Range(-minRecoil, minRecoil);
        mainCamera.transform.eulerAngles += new Vector3(0, 0, Random.Range(-cameraShakeAmount, cameraShakeAmount) * cameraShakeIntensity); //Camera shake, set intensity to 0

        ammoText.text = ammo.ToString(); //Setting ammo UI text
        cooldown = originalCooldown;
        particleSystemArray[0].Play();
    }
}
