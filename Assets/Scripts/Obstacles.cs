using UnityEngine;

public class Obstacles : MonoBehaviour
{
    // declarar las variables como publicas permite modificarlas desde unity

    // valores de tamaño
    public float minSize = 0.5f;
    public float maxSize = 2.0f;

    // valores de velocidad
    public float minSpeed = 50f;
    public float maxSpeed = 150f;

    // valor de giro maximo
    public float maxSpinSpeed = 10f;

    // efecto de impacto / rebote
    public GameObject bounceEffectPrefab;

    // referencia a Rigidbody2D como 'rb'
    Rigidbody2D rb;
    public AudioClip bounceSound;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // declarar numero de punto flotante
        // generar un numero aleatorio en un rango de 0.5 - 2.0
        // el valor aleatorio cambiara las dimensiones x,y de "Obstacle"

        // Este script se ejecuta independientemente para cada prefab de "Obstacle"

        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        // dar velocidad aleatoria a la cual multiplicar la fuerza inicial
        // al dividir entre randomSize, el tamaño de obstacle determina su velocidad

        float randomSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;

        // GetComponent toma un componente especifico del gameObject, para asi poder manipularlo
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // agrega fuerza hacia la derecha al rigidbody
        // la fuerza se multiplica por el valor de randomSpeed
        // tambien se puede aplicar Vector2.up / down/ left

        // randomDirection es instancia de Vector2, al que se le asigna una direccion random
        Vector2 randomDirection = Random.insideUnitCircle;
        rb.AddForce(randomDirection * randomSpeed);

        // dar giro aleatorio a obstacle
        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);
        
        // dar velocidad al obstacle al chocar
        float AddRandomSpeed = Random.Range(minSpeed, maxSpeed);
        rb.AddForce(rb.linearVelocity.normalized * AddRandomSpeed);

        // audio rebote
        AudioSource.PlayClipAtPoint(bounceSound, transform.position, 0.6f);

        // Destroy the effect after 1 second
        Destroy(bounceEffect, 1f);
        
    }
    // Update is called once per frame
    void Update()
    {



    }
}
