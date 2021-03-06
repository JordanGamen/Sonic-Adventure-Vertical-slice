﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;

    private bool selected = false;

    public bool AudioGo { get; set; } = false;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            pauseUI.SetActive(false);
            if (GameManager.instance.cameraMode == GameManager.mode.AUTO)
            {
                if (Camera.main.GetComponent<AutoCamera>() != null)
                {
                    Camera.main.GetComponent<AutoCamera>().enabled = true;
                }
                else
                {
                    Camera.main.GetComponent<ThirdPersonCameraControl>().enabled = true;
                    transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Free camera";
                    GameManager.instance.cameraMode = GameManager.mode.FREE;
                }
                transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Auto camera";
            }
            else
            {
                Camera.main.GetComponent<ThirdPersonCameraControl>().enabled = true;
                transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Free camera";
            }
        }

        AudioGo = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2)
        {
            if (Input.GetButtonDown(Constants.Inputs.pause) && !GameManager.instance.Dying)
            {
                if (!pauseUI.activeSelf)
                {
                    pauseUI.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(pauseUI.transform.GetChild(1).GetChild(1).gameObject);
                    Time.timeScale = 0;
                    AudioManager.instance.Pause(AudioManager.instance.CurrSound.name);
                    if (Camera.main.GetComponent<AutoCamera>() != null)
                    {
                        Camera.main.GetComponent<AutoCamera>().enabled = false;
                    }
                    Camera.main.GetComponent<ThirdPersonCameraControl>().enabled = false;

                    if (GameManager.instance.cameraMode == GameManager.mode.AUTO)
                    {
                        transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Auto camera";
                    }
                    else
                    {
                        transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Free camera";
                    }

                }
                else
                {
                    Continue();
                }
            }
        }
        
    }

    public void CameraChange()
    {
        if (GameManager.instance.cameraMode == GameManager.mode.AUTO)
        {
            GameManager.instance.cameraMode = GameManager.mode.FREE;
            transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Free camera";
        }
        else
        {
            GameManager.instance.cameraMode = GameManager.mode.AUTO;
            transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Auto camera";
        }
    }

    public void Continue()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
        if (GameManager.instance.cameraMode == GameManager.mode.AUTO)
        {
            if (Camera.main.GetComponent<AutoCamera>() != null)
            {
                Camera.main.GetComponent<AutoCamera>().enabled = true;
            }
            else
            {
                Camera.main.GetComponent<ThirdPersonCameraControl>().enabled = true;
                transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "Free camera";
                GameManager.instance.cameraMode = GameManager.mode.FREE;
            }
        }
        else
        {
            Camera.main.GetComponent<ThirdPersonCameraControl>().enabled = true;
        }        
        AudioManager.instance.UnPause(AudioManager.instance.CurrSound.name);
    }

    public void Restart()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;        
        Camera.main.GetComponent<AutoCamera>().enabled = true;
        StartCoroutine(GameObject.FindGameObjectWithTag(Constants.Tags.player).GetComponent<PlayerDeath>().Die());
        AudioManager.instance.UnPause(AudioManager.instance.CurrSound.name);
        GameManager.instance.Timer = 0;
    }

    public void Quit()
    {
        Debug.Log("Quiting...");
        Application.Quit();
    }
}
