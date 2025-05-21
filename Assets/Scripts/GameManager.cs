using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using NUnit.Framework.Constraints;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private GameObject pauseMenu;
    private GameObject vehicle;
    private float totalSeconds;
    private PlayerController playerController;
    private bool isRacing;
    private InputAction menuAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0;
        StartCoroutine("StartCountdown");
        vehicle = GameObject.FindWithTag("Player");
        playerController = vehicle.GetComponent<PlayerController>();
        playerCamera.Follow = vehicle.transform.Find("Vehicle Body").transform;
        totalSeconds = 0;
        isRacing = true;
        menuAction = InputSystem.actions.FindAction("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        totalSeconds += Time.deltaTime;
        fuelText.text = "Fuel: " + playerController.GetFuel();
        if (menuAction.ReadValue<float>() == 1)
        {
            pauseMenu.SetActive(true);
        }
        if (isRacing)
        {
            timeText.text = "Time: " + TimeSpan.FromSeconds(totalSeconds).ToString("mm':'ss':'fff");
        }
    }

    public void FinishRace()
    {
        isRacing = false;
        StartCoroutine("PauseMenu");
    }

    private IEnumerator PauseMenu()
    {
        yield return new WaitForSeconds(3f);
        pauseMenu.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitLevel()
    {
        Application.Quit();
    }

    public void ResumeLevel()
    {
        pauseMenu.SetActive(false);

    }

    private IEnumerator StartCountdown()
    {
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1;

    }
}
