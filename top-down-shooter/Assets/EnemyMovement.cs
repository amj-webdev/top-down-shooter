using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    private Transform player; // Referencja do gracza
    [SerializeField] private float maxSpeed = 5f; // Maksymalna pr�dko��
    [SerializeField] private float acceleration = 2f; // Jak szybko przeciwnik przyspiesza
    private float currentSpeed = 0f; // Aktualna pr�dko�� przeciwnika
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Znajd� gracza po tagu
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

            // Zwi�ksz pr�dko�� przeciwnika stopniowo do maxSpeed
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);

            // Przesu� przeciwnika za pomoc� Rigidbody
            Vector3 velocity = direction * currentSpeed;
            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);

            // Obr�t przeciwnika w stron� gracza
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(targetRotation);
        }
    }
}
