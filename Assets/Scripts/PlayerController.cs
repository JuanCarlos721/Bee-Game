using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.InputManagerEntry;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/*
    PLAYERPREFS

    PlayerPrefs es un sistema de guardado simple y persistente de Unity.
    Los datos se almacenan fuera de las escenas y permanecen aunque el
    juego se cierre o se cambie de escena.
    Los datos se guardan con un nombre clave y que valor se asigna
    El dato se recupera con Get...
    PlayerPrefs.SetInt("nombreClave", 5);

    TIPOS DE DATOS QUE PUEDE GUARDAR:
    - int
    - float
    - string

    EJEMPLOS DE USO:

    // Guardar datos
    PlayerPrefs.SetInt("Vidas", 3);
    PlayerPrefs.SetFloat("HighScore", 1500f);
    PlayerPrefs.SetString("NombreJugador", "Charlie");

    // Leer datos
    int vidas = PlayerPrefs.GetInt("Vidas", 3);
    float highScore = PlayerPrefs.GetFloat("HighScore", 0);
    string nombre = PlayerPrefs.GetString("NombreJugador", "Jugador");

    CUÁNDO USAR PlayerPrefs:
    - High Score
    - Opciones de sonido (volumen)
    - Sensibilidad de controles
    - Idioma seleccionado
    - Configuraciones simples del juego

    CUÁNDO NO USAR PlayerPrefs:
    - Inventarios grandes
    - Progreso complejo
    - Clases, listas o arrays
    - Datos importantes o sensibles

    NOTAS IMPORTANTES:
    - PlayerPrefs NO es seguro (el jugador puede modificarlo).
    - No debe usarse en Update(), solo cuando el valor cambia.
    - Es ideal para datos pequeños y simples.
*/

public class Player : MonoBehaviour
{
    // fuerza de empuje
    public float thrustForce = 7f;
    public float maxSpeed = 10f;
    
    // instanciar Rigidbody2D
    Rigidbody2D rb;

    // sprite de fuego
    public GameObject boosterFlame;

    // efecto de explosion
    public GameObject explosionEffect;

    // contador de tiempo
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;

    // variable que guarda el highscore
    private float high = 0f;

    // para agregar el recurso UIDocument a player en unity
    public UIDocument uiDocument;
    // variable tipo Label para modificar el puntaje
    private Label scoreText;

    // insntanciar Button
    private Button restartButton;

    private Button HighScore;

    // sonido de muerte
    public AudioClip deathSound;
    private AudioSource audioSource; // contenedor <>

    // bordes
    public GameObject borderParent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Cargar el High Score guardado en el dispositivo.
        // Si no existe aún, se usa 0 como valor inicial.
        high = PlayerPrefs.GetFloat("HighScore", 0);

        // dar acceso al rigidbody2d
        rb = GetComponent<Rigidbody2D>();

        // dar acceso al AudioSource
        audioSource = GetComponent<AudioSource>();

        // se pasan referencias con el 'query system' de unity para modificarlas con c#
        // dar acceso al contenedor tipo <Label> en e uiDocument -> ScoreLabel
        // .rootVisualElement gives you access to the top - level container of the UI layout.
        //.Q<Label>("ScoreLabel") uses Unity’s query system to find the first element of type Label with the name ScoreLabel.
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");

        // dar acceso al contenedor <Button> RestartButton
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        HighScore= uiDocument.rootVisualElement.Q<Button>("HighScore");


        // ocultar boton de restart
        restartButton.style.display = DisplayStyle.None;
        HighScore.style.display = DisplayStyle.None;

        // al presionar restartButton
        // += actua como un event listener (click == True) y suscribe la funcion ReloadScene
        restartButton.clicked += ReloadScene;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        MovePlayer();
    }


    // Cuando el jugador colisiona con un obstaculo
    private void OnCollisionEnter2D(Collision2D collision)
    {

        // Instantiate() crea la copia del prefab ExplosionEffect
        // transform.position pone el ExplosionEffect en donde esta el GameObject
        // transform.rotation pone la rotacion de la explosion igual a la de la nave (irrelevante en este caso)
        // las particulas aparecen en el ultimo frame en el que existio el gameObject
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // mostrar restartButton
        restartButton.style.display = DisplayStyle.Flex;
        HighScore.style.display = DisplayStyle.Flex;
        // Actualizar HighScore y mostrar
        HighScoreUpdate();

        // reproducir audio
        AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // destruir gameobject al frame siguiente
        Destroy(gameObject);

        borderParent.SetActive(false);
    }



    private void UpdateScore()
    {
        // contador de puntos
        elapsedTime += Time.deltaTime;
        //Debug.Log("Elapsed time: " + elapsedTime);

        scoreText.text = "Score: " + score;

        // redondear a entero con Math.FloorToInt()
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        Debug.Log("score: " + score);
    }

    private void MovePlayer()
    {

        // Flama ON
        if (Mouse.current.leftButton.wasPressedThisFrame)
            boosterFlame.SetActive(true);

        // Flama OFF
        if (Mouse.current.leftButton.wasReleasedThisFrame)
            boosterFlame.SetActive(false);

        // Movimiento solo mientras se presiona
        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);

            Vector2 direction = (mousePos - transform.position).normalized;
            transform.up = direction;

            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // Recarga la escena actual desde cero.
    // Unity destruye todos los GameObjects de la escena y la vuelve a cargar,
    // reiniciando posiciones, variables y el estado completo del juego.
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //
    void HighScoreUpdate()
    {
        if (score > high)
        {
            high = score;
            // Guardar el nuevo récord de forma persistente
            PlayerPrefs.SetFloat("HighScore", high);
        }

        HighScore.text = "High Score: " + high;
    }
}

