using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this to use UI components

public class Order : MonoBehaviour
{
    private ClientMovement clientInRange;
    public PlayerState currentState = PlayerState.Free;
    private ClientMovement currentClient;
    [SerializeField] private Animator animator; // Reference to the Animator component
    private float orderStartTime; // Time when the order was taken
    [SerializeField] private Text moneyText; // Reference to the UI Text component
    [SerializeField] private Text goalMoney;
    private float goal;
    public GameObject winUI;
    float money = 0; // Changed money to float
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip winSound; 

    void Start()
    {
        goal = UnityEngine.Random.Range(10f, 20f);
        UpdateMoneyUI();
    }
    public enum PlayerState
    {
        Free,
        HasOrder,
        HasFood
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryTakeOrder();
            TryDeliverFood();
        }          
    }

    void TryTakeOrder()
    {
        if (currentState != PlayerState.Free)
            return;
        if (clientInRange == null)
            return;
        if (clientInRange.currentState != ClientMovement.ClientStates.WaitingToOrder)
            return;

        currentClient = clientInRange;
        SetPlayerState(PlayerState.HasOrder);

        clientInRange.TakeOrder();
        Debug.Log("Pedido tomado correctamente");

        // Record the time when the order was taken
        orderStartTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ClientMovement client = collision.GetComponent<ClientMovement>();

        if (client != null)
        {
            clientInRange = client;
        }

        if (collision.CompareTag("Kitchen"))
        {
            TryCook();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ClientMovement client = collision.GetComponent<ClientMovement>();

        if (client != null && client == clientInRange)
        {
            clientInRange = null;
        }
    }

    void TryCook()
    {
        if (currentState != PlayerState.HasOrder)
            return;
        
        SetPlayerState(PlayerState.HasFood);
        Debug.Log("Pedido preparado");
    }

    void TryDeliverFood()
    {
        if (currentState != PlayerState.HasFood)
            return;

        if (clientInRange == null)
            return;

        if (clientInRange != currentClient)
            return;

        clientInRange.ReceiveFood();

        currentClient = null;
        SetPlayerState(PlayerState.Free);
        Debug.Log("Comida entregada");

        // Calculate and log the time taken to deliver the food
        float timeTaken = Time.time - orderStartTime;
        Debug.Log($"Time taken to deliver food: {timeTaken} seconds");

        // Calculate money increment inversely proportional to time taken
        float baseReward = 10f;
        float timePenalty = Mathf.Clamp(1 / timeTaken, 0.1f, 1f); // Avoid division by zero and cap the penalty
        float reward = baseReward * timePenalty;
        money += reward;

        Debug.Log($"Reward based on time: {reward:F2}");

        // Update the money UI
        UpdateMoneyUI();
        CheckWinCondition();
    }

    void SetPlayerState(PlayerState newState)
    {
        currentState = newState;

        if (animator != null)
        {
            animator.SetBool("Pedido", currentState == PlayerState.HasFood);
        }
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $":{money:F2}$"; // Display money with two decimal places
        }
        if (goalMoney != null)
        {
            goalMoney.text = $"{goal:F2}$";
        }       
    }

    void CheckWinCondition()
    {
        if (money >= goal)
        {
            Debug.Log("Ganaste");
            Time.timeScale = 0f;
            if (winUI != null)
            {
                winUI.SetActive(true);
            }

            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.PlayOneShot(winSound);

        }
    }
}
