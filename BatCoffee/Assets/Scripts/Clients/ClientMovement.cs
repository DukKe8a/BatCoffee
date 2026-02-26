using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClientMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Animator animator; // Reference to the Animator component
    public ClientStates currentState;
    private Transform originalSpawnPoint;
    private Spawner spawner;
    private Transform targetPoint;

    public enum ClientStates
    {
        walking,
        WaitingToOrder,
        Ordered,
        Eating,
        Leaving
    }

    void Start()
    {
        currentState = ClientStates.walking;
        if (animator != null)
        {
            animator.SetBool("Entrando", true); // Set "Entrando" to true when starting
        }
    }

    void Update()
    {
        if (currentState == ClientStates.walking && target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) < 0.05f)
                OnArrived();
        }

        if (currentState == ClientStates.Leaving)
        {
            // Invert the animation horizontally
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            if (animator != null)
        {
            animator.SetBool("Entrando", true); // Set "Entrando" to true when starting
        }
            transform.position = Vector3.MoveTowards(transform.position, originalSpawnPoint.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, originalSpawnPoint.position) < 0.05f)
                LeaveCoffe();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetPoint = newTarget;
    }

    public void OnArrived()
    {
        currentState = ClientStates.WaitingToOrder;
        Debug.Log("Llego a la mesa");
        target = null;

        if (animator != null)
        {
            animator.SetBool("Entrando", false); // Set "Entrando" to false when arriving
        }
    }

    public void TakeOrder()
    {
        if (currentState != ClientStates.WaitingToOrder)
            return;

        currentState = ClientStates.Ordered;
        Debug.Log("Pedido tomado");
    }

    public void ReceiveFood()
    {
        if (currentState != ClientStates.Ordered)
            return;

        currentState = ClientStates.Eating;
        Debug.Log("Cliente recibio comida");

        StartCoroutine(EatingRoutine());
    }

    IEnumerator EatingRoutine()
    {
        yield return new WaitForSeconds(3f);

        currentState = ClientStates.Leaving;
        Debug.Log("Cliente se va");
    }

    public void SetSpawnPoint(Transform spawn)
    {
        originalSpawnPoint = spawn;
    }

    void LeaveCoffe()
    {
        Debug.Log("Cliente sale del Cafe");

        if (spawner != null)
        {
            spawner.FreeSpawnPoint(targetPoint);
        }

        Destroy(gameObject);
    }

    public void SetSpawner(Spawner s)
    {
        spawner = s;
    }
}
