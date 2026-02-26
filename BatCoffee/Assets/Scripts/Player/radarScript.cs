using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this to use UI components

public class radarScript : MonoBehaviour
{
    private Transform pulseTransform;
    private float range;
    private float rangeMax;
    private bool isPulsing = false;
    private CircleCollider2D circle;

    public KeyCode sonarKey = KeyCode.Space;
    public float stressPerSonar = 0.2f;     // cuánto sube cada grito
    public float stressDecay = 0.3f;        // cuánto baja por segundo
    public Image stressBar;

    private float currentStress = 0f;
    private float stressDelayTimer = 0f; // Timer to track delay before stress decay
    private bool stressDecayDelayed = true; // Flag to control stress decay delay
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip GameOverSound;
    [SerializeField] private AudioClip ScreamSound;

    private void Awake()
    {
        pulseTransform = transform.Find("Pulse");
        rangeMax = 10f; 

        circle = pulseTransform.GetComponent<CircleCollider2D>();
        circle.radius = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(sonarKey))
        {
            UseSonar();
            isPulsing = true;
            range = 0f; // Reset the pulse range when sonar key is pressed

            // Reset the stress decay delay
            stressDecayDelayed = true;
            stressDelayTimer = 0f;
        }

        if (isPulsing)
        {
            float rangeSpeed = 5f;
            range += Time.deltaTime * rangeSpeed;

            if (range > rangeMax)
            {
                isPulsing = false; // Stop pulsing when the range exceeds the maximum
            }
            pulseTransform.localScale = new Vector3(range, range);
            circle.radius = range / 2;
        }

        if (!stressDecayDelayed)
        {
            // Ensure the stress bar decreases over time
            currentStress -= stressDecay * Time.deltaTime;
            currentStress = Mathf.Clamp01(currentStress);
        }
        else
        {
            stressDelayTimer += Time.deltaTime;
            if (stressDelayTimer >= 4f) // Correct delay time
            {
                stressDecayDelayed = false; // Allow stress decay after the delay
            }
        }

        // Update the stress bar to reflect the current stress
        stressBar.fillAmount = currentStress;

        if (currentStress >= 1f)
        {
            GameOver();
        }
    }

    void UseSonar()
    {
        Debug.Log("Sonar usado");
        audioSource.PlayOneShot(ScreamSound); 

        currentStress += stressPerSonar;
        currentStress = Mathf.Clamp01(currentStress);
    }

    public GameObject gameOverUI;

    void GameOver()
    {
        Debug.Log("Clientes asustados. Perdiste.");

        
        Time.timeScale = 0f;

        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        currentStress = 0f;
        // Stop the current music before playing the Game Over sound
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Play the Game Over sound
        audioSource.PlayOneShot(GameOverSound);
            
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        Fade fade = obj.GetComponent<Fade>();

        if (fade != null)
        {
            fade.FadeReveal();
        }
    }
}
