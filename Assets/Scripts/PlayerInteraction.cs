using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerInteraction : MonoBehaviour
{

    private Sign currentSign;
    public Sign CurrentSign { get { return currentSign; } }
    private GameObject interactPrompt;
    public Light2D flashLight;
    private static PlayerInteraction _instance;
    private GameObject currentHealthShop;
    AudioManager audioManager;

    public static PlayerInteraction Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerInteraction>();
            }
            return _instance;
        }
    }

    void Start()
    {
        flashLight.enabled = false;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }


        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }



    #region update
    void Update()
    {
        if (InputManager.downWasPressed)
        {
            if (currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }

        //vertical = Input.GetAxis("Vertical");
        vertical = InputManager.movement.y;

        if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }

        if (InputManager.InteractionWasPressed)
        {
            if (currentTeleporter != null)
            {
                audioManager.PlaySFX(audioManager.dorr);
                transform.position = currentTeleporter.GetComponent<Dorr>().GetDestination().position;
            }

            if (currentHealthShop != null) 
            {
                TryBuyHealth();
            }
        }

        if (InputManager.FlashLightWasPressed)
        {
            flashLight.enabled = !flashLight.enabled;
        }

        if (InputManager.InteractionWasPressed && currentSign != null)
        {
            currentSign.DisplayInfo();
        }

    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f; 
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed); 
        }
        else
        {
            rb.gravityScale = 1f; // Restore gravity
        }
    }

    #endregion

    #region 2Way Platform

    private GameObject currentOneWayPlatform;
    [SerializeField] private BoxCollider2D playerCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("oneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("oneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    #endregion

    #region Ladder/teleport

    private float vertical;
    private float speed = 8f;
    private bool isLadder;
    private bool isClimbing;
    
    [SerializeField] private Rigidbody2D rb;
    private GameObject currentTeleporter;
    public TrailRenderer tr;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HealthShop"))  // Detect the health shop area
        {
            currentHealthShop = collision.gameObject;
        }

        if (collision.CompareTag("Dorr"))
        {
            tr.emitting = false;
            currentTeleporter = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HealthShop"))  // Exit health shop area
        {
            if (collision.gameObject == currentHealthShop)
            {
                currentHealthShop = null;
            }
        }

        if (collision.CompareTag("Dorr"))
        {
            if (collision.gameObject == currentTeleporter)
            {
                tr.emitting = true;
                currentTeleporter = null;
            }
        }
    }

    #endregion

    #region HealthShop

    public void InteractWithHealthShop(HealthShop shop)
    {
        currentHealthShop = shop.gameObject;
        TryBuyHealth();
    }

    public void TryBuyHealth()
    {
        GameController gameController = GameController.Instance;

        Debug.Log("money: " + gameController.amountOfMoney);
        Debug.Log("health: " + gameController.playerHealth.currentHealth + " / " + gameController.playerHealth.maxHealth);
        Debug.Log("FullHealth? " + gameController.playerHealth.IsFullHealth);
        Debug.Log("Dead? " + gameController.playerHealth.IsDead);

        if (gameController.amountOfMoney >= 5 &&
            gameController.playerHealth != null &&
            !gameController.playerHealth.IsFullHealth &&
            !gameController.playerHealth.IsDead)
        {
            gameController.amountOfMoney -= 5;
            gameController.coinText.text = $": {gameController.amountOfMoney}";
            audioManager.PlaySFX(audioManager.healthShop);

            gameController.playerHealth.AddHealth(1);
            Debug.Log("Bought 1 health point.");
        }
        else
        {
            Debug.Log("BuyHealth: amount = " + gameController.amountOfMoney);
            Debug.Log("BuyHealth: currentHealth = " + gameController.playerHealth.currentHealth + " / " + gameController.playerHealth.maxHealth);
            Debug.Log("BuyHealth: storedPlayerHealth = " + GameController.Instance.storedPlayerHealth);

        }
    }

    #endregion

    public void SetCurrentSign(Sign sign)
    {
        currentSign = sign;
    }

    // Called by the Sign script when the player leaves its trigger.
    public void ClearCurrentSign()
    {
        currentSign = null;
    }
}
