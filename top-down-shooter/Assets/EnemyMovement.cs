using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    private Transform player; // Referencja do gracza
    [SerializeField] private float maxSpeed = 5f; // Maksymalna prêdkoœæ
    [SerializeField] private float acceleration = 2f; // Jak szybko przeciwnik przyspiesza
    private float currentSpeed = 0f; // Aktualna prêdkoœæ przeciwnika
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ZnajdŸ gracza po tagu
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Nie znaleziono gracza z tagiem 'Player'!");
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Oblicz kierunek do gracza
            Vector3 direction = (player.position - transform.position).normalized;

            // Zwiêksz prêdkoœæ przeciwnika stopniowo do maxSpeed
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);

            // Przesuñ przeciwnika za pomoc¹ Rigidbody
            Vector3 velocity = direction * currentSpeed;
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);

            // Obrót przeciwnika w stronê gracza
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(targetRotation);
        }
    }
}
