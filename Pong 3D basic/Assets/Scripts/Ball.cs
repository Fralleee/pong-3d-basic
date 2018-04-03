using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{

  private Rigidbody body;
  public float maxSpeed = 25f; // this should increase with game time

  void Start()
  {
    //float xPower = Random.Range(2, 4);
    //float zPower = Random.Range(10, 20);
    //body.AddForce(new Vector3(Random.Range(-xPower, xPower), 0, Random.Range(-zPower, zPower)));
    body = GetComponent<Rigidbody>();
  }


  void Update()
  {
    if (body.velocity.magnitude > maxSpeed) body.velocity = body.velocity.normalized * maxSpeed;
  }
}
