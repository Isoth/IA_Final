using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Path : MonoBehaviour
{
    Vector3 target;
    Vector3 playerPos;
    Vector3 lastPlayerPos;

    public GameObject player;    

    bool veoPlayer = false;
    bool ultimaVezVisto = false;

    public string nombreEscena;

    //V
    public static Vector3 velocidad;

    //VD
    public Vector3 velocidadDeseada;

    public GameObject[] puntos;

    public float radioPunto = 10f;
    public float speed = 4f;
    public float rotationMax = 1f;
    public float masa = 15f;

    float distancia;

    int contador;

    bool perseguir = false;

    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        contador = 0;
        target = puntos[contador].transform.position;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        playerPos = player.transform.position;

        if (perseguir == false && ultimaVezVisto == false)
        {
            IrAPunto();
        }          
        if (perseguir == true && ultimaVezVisto == false)
        {
            Perseguir();
        }  
        if (ultimaVezVisto == true)
        {
            UltimaPosPlayer();
        }

        LayerMask mask = LayerMask.GetMask("Player");

        RaycastHit rayHit;
 
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out rayHit, mask)){
            
            Debug.DrawLine(transform.position, rayHit.point, Color.red);
            
            if (rayHit.collider.tag == "Player"){
                print("Buenos días");
                veoPlayer = true;
            }
            else
            {
                veoPlayer = false;
            }
        }
    }

    // Update is called once per frame
    void cambiarPunto()
    {
        if (contador < puntos.Length)
        {
            contador++;
            target = puntos[contador].transform.position;
        }
        else
        {
            contador = 0;
            target = puntos[contador].transform.position;
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < puntos.Length; i++)
        {
            if (i + 1 < puntos.Length)
            {
                Debug.DrawLine(puntos[i].transform.position, puntos[i + 1].transform.position, Color.red);
            }
            if (i + 1 == puntos.Length)
            {
                Debug.DrawLine(puntos[i].transform.position, puntos[0].transform.position, Color.red);
            }
        }
    }

    private void IrAPunto()
    {
        //Calculamos la dirección a la que quiere ir el enemigo.
        velocidadDeseada = (target - this.transform.position).normalized * speed;

        //Distancia hasta target.
        distancia = Vector3.Distance(target, this.transform.position);

        if (distancia < radioPunto)
        {
            velocidadDeseada = velocidadDeseada.normalized * speed * (distancia / radioPunto);
            cambiarPunto();
            print("dentro");
        }
        else
        {
            velocidadDeseada = velocidadDeseada.normalized * speed;
            print("fuera");
        }

        //Limitamos la velocidad de rotación.
        Vector3 steering = Vector3.ClampMagnitude(velocidadDeseada - velocidad, rotationMax) / masa;


        //Limitamos velocidad.
        velocidad = Vector3.ClampMagnitude(velocidad + steering, speed);


        //Aplicamos vectores y velocidades.
        this.transform.position += velocidad * Time.deltaTime;
        transform.forward = velocidad.normalized;


        //Guía para ver si los vectores de velocidad y velocidadDeseada se comportan como deberían.
        Debug.DrawRay(this.transform.position, velocidad, Color.magenta);
        Debug.DrawRay(this.transform.position, velocidadDeseada, Color.cyan);

    }

    private void Perseguir()
    {
        agent.SetDestination(player.transform.position);

        //Dash o Placaje
        distancia = Vector3.Distance(target, this.transform.position);
        if (distancia < 2)
        {
            // PLACAR O DISPARAR
        }
    }

    private void UltimaPosPlayer()
    {
        //Calculamos la dirección a la que quiere ir el enemigo.
        velocidadDeseada = (lastPlayerPos - this.transform.position).normalized * speed;

        //Si entra en el radio, reduce la velocidad.
        distancia = Vector3.Distance(lastPlayerPos, this.transform.position);

        if (distancia < 1)
        {
            velocidadDeseada = velocidadDeseada.normalized * speed * (distancia / radioPunto);          
            perseguir = false;
            ultimaVezVisto = false;
            print("ª");
        }
        else
        {
            velocidadDeseada = velocidadDeseada.normalized * speed;
        }

        //Limitamos la velocidad de rotación.
        Vector3 steering = Vector3.ClampMagnitude(velocidadDeseada - velocidad, rotationMax) / masa;


        //Limitamos velocidad.
        velocidad = Vector3.ClampMagnitude(velocidad + steering, speed);


        //Aplicamos vectores y velocidades.
        this.transform.position += velocidad * Time.deltaTime;
        transform.forward = velocidad.normalized;


        //Guía para ver si los vectores de velocidad y velocidadDeseada se comportan como deberían.
        Debug.DrawRay(this.transform.position, velocidad, Color.magenta);
        Debug.DrawRay(this.transform.position, velocidadDeseada, Color.cyan);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && veoPlayer)
        {
            perseguir = true;  
        }         
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && veoPlayer)
        {
            perseguir = true;  
        } 
    }
    private void OnTriggerExit(Collider other)
    {
        lastPlayerPos = player.transform.position;
        ultimaVezVisto = true;        
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player"){
            print("Te pillé puto");
            SceneManager.LoadScene(nombreEscena);
        }
    }
}
