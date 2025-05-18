using UnityEngine;

public class WheelParticleController : MonoBehaviour
{
    public ParticleSystem dirtParticlePrefab;
    ParticleSystem dirtParticles;
    Rigidbody2D wheelRb;
    float minimumRelativeVelocity = 0.02f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dirtParticles = Instantiate(dirtParticlePrefab);
        wheelRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Surface")){
            dirtParticles.Play();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Surface")){
            ContactPoint2D groundContact = collision.GetContact(0);
            dirtParticles.transform.position = groundContact.point;
            var dirtShape = dirtParticles.shape;
            if (wheelRb.angularVelocity <= 0){
                dirtShape.rotation = new Vector3(0, 0, 160 + Mathf.Abs(Mathf.Acos(groundContact.normal.x)));
            } else {
                dirtShape.rotation = new Vector3(0, 0, 20 + Mathf.Abs(Mathf.Acos(groundContact.normal.x)));
                // dirtParticles.transform.LookAt(new Vector3(Mathf.Sin(groundContact.normal.x), Mathf.Cos(groundContact.normal.y), 0));
            }
            var dirtMain = dirtParticles.main;
            // dirtMain.startSpeedMultiplier = groundContact.relativeVelocity.magnitude;
            if (groundContact.relativeVelocity.magnitude < minimumRelativeVelocity){
                dirtMain.startLifetime = 0;
                // Debug.Log(groundContact.relativeVelocity.magnitude);
            } else {
                dirtMain.startLifetime = 1;
            }


        }
        
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Surface") && dirtParticles != null){
            dirtParticles.Stop();
        }
    }
}
