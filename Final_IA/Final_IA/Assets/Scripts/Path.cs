using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    Vector3 target;

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

    bool huir = false;

    // Start is called before the first frame update
    void Start()
    {
        contador = 0;
        target = puntos[contador].transform.position;
    }

    void Update()
    {
        if (huir == false)
            IrAPunto();
        if (huir == true)
            Huir();
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

        //Si entra en el radio, reduce la velocidad.
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
    private void Huir()
    {
        //Calculamos la dirección a la que quiere ir el enemigo.
        velocidadDeseada = (target - this.transform.position).normalized * speed;

        //Si entra en el radio, reduce la velocidad.
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
        this.transform.position -= velocidad * Time.deltaTime;
        transform.forward = -velocidad.normalized;


        //Guía para ver si los vectores de velocidad y velocidadDeseada se comportan como deberían.
        Debug.DrawRay(this.transform.position, velocidad, Color.magenta);
        Debug.DrawRay(this.transform.position, velocidadDeseada, Color.cyan);

    }
    private void OnTriggerEnter(Collider other)
    {
        huir = true;
    }
    private void OnTriggerExit(Collider other)
    {
        huir = false;
    }
}
