using UnityEngine;
using UnityEngine.InputSystem;

public class EngineSoundController : MonoBehaviour
{
    public float maxEnginePitch = 3f;
    public GameObject vehicle;
    private PlayerController playerControllerScript;
    private AudioSource engineSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        engineSound = GetComponent<AudioSource>();
        playerControllerScript = vehicle.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerScript.GetFuel() > 0)
        {
            engineSound.pitch = 1 + (maxEnginePitch - 1) * playerControllerScript.GetRevFraction();    
        } else {
            engineSound.pitch = 0;
        }
    }

}
