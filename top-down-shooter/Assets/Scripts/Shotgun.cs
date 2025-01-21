using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class Shotgun : MonoBehaviour
{
    enum WeaponType
    {
        Gun,    // Opcja 1
        Shotgun,  // Opcja 2
        Sniper    // Opcja 3
    }

    [SerializeField] WeaponType type; 

    [Header("Fields")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform player;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] LineRenderer lineRenderer;
    

    [Header("Gun properties")]
    [SerializeField] float fireRate = 1f;
    [SerializeField] float recoilSmoothTime = 0.6f;
    [SerializeField] float maxRecoilDistance = 0.8f;
    [SerializeField] float recoilMultiplier = 1.8f;


    [Header("Shotgun properties")]
    [SerializeField] float coneAngle = 90f;
    [SerializeField] float damageRadius = 10f;

    [Header("Sniper propertier")]
    [SerializeField] float maxSniperRange = 10f;
    [SerializeField] string[] blockTags;


    private float nextFireTime = 0f;
    private float recoilTime = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Ustawienia LineRenderer
        lineRenderer.positionCount = 2; // Promieñ ma dwa punkty (pocz¹tek i koniec)
        lineRenderer.startWidth = 0.04f;  // Szerokoœæ promienia na pocz¹tku
        lineRenderer.endWidth = 0.04f;    // Szerokoœæ promienia na koñcu

    }

    void Update()
    {

        switch (type)
        {
            case WeaponType.Gun:
                maxRecoilDistance = 0.1f;
                fireRate = 10f;
                GameObject.Find("Player").GetComponent<PlayerMovement>().moveSpeed = 12;
                break;
            case WeaponType.Shotgun:
                maxRecoilDistance = 0.9f;
                fireRate = 1f;
                GameObject.Find("Player").GetComponent<PlayerMovement>().moveSpeed = 5f;
                break;
            case WeaponType.Sniper:
                maxRecoilDistance = 0.5f;
                fireRate = 0.4f;
                GameObject.Find("Player").GetComponent<PlayerMovement>().moveSpeed = 7f;
                break;
                
        }

        if (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame)
        {
            if (type == WeaponType.Shotgun)
                ShootShootgun();
            else
                Shoot();
        }

        ApplyRecoil();

        if(type == WeaponType.Sniper)
        {
            lineRenderer.positionCount = 2;

            Vector3 startPosition = firePoint.position; // Pocz¹tek promienia
            RaycastHit hit;
            Vector3 endPosition = startPosition + transform.right * maxSniperRange;

            lineRenderer.SetPosition(0, startPosition); // Ustaw pocz¹tek promienia
            lineRenderer.SetPosition(1, endPosition);   // Ustaw koniec promienia

            if (Physics.Raycast(startPosition, transform.right, out hit, maxSniperRange))
            {
                // Sprawdzanie tagu trafionego obiektu
                foreach (string tag in blockTags)
                {
                    if (hit.collider.CompareTag(tag))
                    {
                        endPosition = hit.point;
                        break;
                    }
                }
            }

            lineRenderer.SetPosition(0, startPosition); 
            lineRenderer.SetPosition(1, endPosition);   

            if (Time.time < nextFireTime)
            {
                lineRenderer.positionCount = 0;
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }

        

    }

    void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            Vector3 spawnPosition = firePoint.position;
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, firePoint.rotation);

            bullet.transform.Rotate(0, 90f, 0);

            nextFireTime = Time.time + 1f / fireRate;

            recoilTime = 0f;
        }
    }

    void ShootShootgun()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            // Play muzzle flash
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            // Deal damage in a cone
            DealDamageInCone();
            recoilTime = 0f;
        }
    }

    void ApplyRecoil()
    {
        if (recoilTime < recoilSmoothTime)
        {
            recoilTime += Time.deltaTime;

            float t = recoilTime / recoilSmoothTime;

            float recoilFactor = Mathf.Pow(1 - t, 3) * maxRecoilDistance;

            float forwardSpeed = Mathf.Abs(rb.velocity.z);

            if (forwardSpeed > 0.1)
                recoilFactor *= recoilMultiplier;

            Vector3 recoilDirection = -transform.right * recoilFactor;
            rb.AddForce(recoilDirection, ForceMode.VelocityChange);
        }
    }

    void DealDamageInCone()
    {
        if(type == WeaponType.Shotgun)
        { 
            Collider[] hitColliders = Physics.OverlapSphere(firePoint.position, damageRadius);

                foreach (var collider in hitColliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        Vector3 directionToTarget = collider.transform.position - firePoint.position;
                        float distanceToTarget = directionToTarget.magnitude;
                        float angleToTarget = Vector3.Angle(firePoint.right, directionToTarget);

                        // Obliczenie minimalnego dystansu dla rozszerzonego sto¿ka
                        float baseRadius = 1f; // Szerokoœæ podstawy sto¿ka (mo¿esz ustawiæ w inspektorze)
                        float minDistance = baseRadius / Mathf.Tan(Mathf.Deg2Rad * coneAngle);

                        // SprawdŸ, czy cel mieœci siê w rozszerzonym sto¿ku
                        if (distanceToTarget >= minDistance && angleToTarget <= coneAngle)
                        {
                            if (Physics.Raycast(firePoint.position, directionToTarget.normalized, out RaycastHit hit, damageRadius))
                            {
                                

                                if (hit.collider.CompareTag("Enemy"))
                                {
                                    Destroy(collider.gameObject);
                                }
                                else if (hit.collider.CompareTag("Wall"))
                                {
                                    Debug.Log($"{collider.name} jest os³oniêty przez {hit.collider.name}");
                                }
                            }
                        }
                    }
                }
        }
        else
        {

        }
    }



    //Draw helping gizmos with shotgun range and radius
    void OnDrawGizmos()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.red;
        Vector3 forward = firePoint.right;

        Vector3 leftBoundary = Quaternion.AngleAxis(-coneAngle / 2, firePoint.up) * forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(coneAngle / 2, firePoint.up) * forward;

        Gizmos.DrawLine(firePoint.position, firePoint.position + leftBoundary * damageRadius);
        Gizmos.DrawLine(firePoint.position, firePoint.position + rightBoundary * damageRadius);


    }
}
