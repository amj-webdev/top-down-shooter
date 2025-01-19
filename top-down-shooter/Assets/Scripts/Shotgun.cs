using UnityEngine;
using UnityEngine.InputSystem;

public class Shotgun : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform player;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform firePoint;

    [Header("Shotgun properties")]
    [SerializeField] float fireRate = 1f;
    [SerializeField] float recoilSmoothTime = 0.6f;
    [SerializeField] float maxRecoilDistance = 0.8f;
    [SerializeField] float recoilMultiplier = 1.8f;

    private float nextFireTime = 0f;
    private float recoilTime = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame)
        {
            Shoot();
        }

        ApplyRecoil();

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
}
