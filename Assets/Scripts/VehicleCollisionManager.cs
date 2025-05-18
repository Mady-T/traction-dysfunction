using UnityEngine;

public class VehicleCollisionManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fuel"))
        {
            playerController.RefuelVehicle();
        }
    }
}
