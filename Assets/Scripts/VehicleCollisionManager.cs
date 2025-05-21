using UnityEngine;
using UnityEngine.Rendering;

public class VehicleCollisionManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;
    private float lowerBound = -50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OutOfBoundsCheck();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fuel"))
        {
            playerController.RefuelVehicle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish Line"))
        {
            gameManager.FinishRace();
        }
    }

    private void OutOfBoundsCheck()
    {
        if (transform.position.y < lowerBound)
        {
            gameManager.FinishRace();
        }
    }
}
