﻿using System.Collections;
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
    public GameObject particulas;

    private bool veoPlayer = false;
    private bool ultimaVezVisto = false;
    private bool dashActivo = true;
    private bool explosion= false;

    public string nombreEscena;

    //V
    private Vector3 velocidad;

    //VD
    private Vector3 velocidadDeseada;

    [SerializeField] GameObject[] puntos;

    public float radioPunto = 10f;
    public float speed = 4f;
    public float rotationMax = 1f;
    public float masa = 15f;

    float distancia;

    private int contador;

    private bool perseguir = false;

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

        RaycastHit rayHit;
 
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out rayHit, 8)){
            
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
    private void cambiarPunto()
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
        }
        else
        {
            velocidadDeseada = velocidadDeseada.normalized * speed;
        }

        //Limitamos la velocidad de rotación.
        Vector3 steering = Vector3.ClampMagnitude(velocidadDeseada - velocidad, rotationMax) / masa;


        //Limitamos velocidad.
        velocidad = Vector3.ClampMagnitude(velocidad + steering, speed);
        print(velocidad);

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
        distancia = Vector3.Distance(player.transform.position, this.transform.position);
        if (distancia <= 4 && distancia > 1.01 && dashActivo)
        {
            print("dash");
            agent.speed *= 5;
            dashActivo = false;
        }
        else if (distancia > 4)
        {
            agent.speed = 4;
            dashActivo = true;
        }
        if (distancia <= 1.01)
        {
            Destroy(this.gameObject, 1);
            Instantiate(particulas, this.transform.position, Quaternion.identity);
            explosion = true;
        }
    }

    private void UltimaPosPlayer()
    {
        //Calculamos la dirección en la que se vió por última vez al enemigo.
        /*velocidadDeseada = (lastPlayerPos - this.transform.position).normalized * speed;

        //Si entra en el radio, reduce la velocidad.
        distancia = Vector3.Distance(lastPlayerPos, this.transform.position);

        if (distancia < radioPunto)
        {
            velocidadDeseada = velocidadDeseada.normalized * speed * (distancia / radioPunto);          
            perseguir = false;
            ultimaVezVisto = false;
            cambiarPunto();
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
        Debug.DrawRay(this.transform.position, velocidad, Color.green);
        Debug.DrawRay(this.transform.position, velocidadDeseada, Color.white);*/

        agent.SetDestination(lastPlayerPos);

        distancia = Vector3.Distance(lastPlayerPos, this.transform.position);

        if (distancia < radioPunto)
        {     
            perseguir = false;
            ultimaVezVisto = false;
            Destroy(this.gameObject, 1);
            Instantiate(particulas, this.transform.position, Quaternion.identity);
            explosion = true;
            print("ª");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && veoPlayer == true)
        {
            if (explosion){
                SceneManager.LoadScene(nombreEscena);
            }
            perseguir = true;  
        }         
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && veoPlayer == true)
        {
            if (explosion){
                SceneManager.LoadScene(nombreEscena);
            }
            perseguir = true;  
        } 
    }
    private void OnTriggerExit(Collider other)
    {
        if (veoPlayer == true)
        {
            lastPlayerPos = player.transform.position;
            target = lastPlayerPos;
            ultimaVezVisto = true; 
        }         
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player"){
            print("Te pillé puto");
            SceneManager.LoadScene(nombreEscena);
        }
    }
}
