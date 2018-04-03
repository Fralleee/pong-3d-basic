using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/*
 *  Flytta ut basic grejer till egen komponent som både padcontroller och AI controller ärver ifrån
 * 
 */

[RequireComponent(typeof(Rigidbody))]
public class PadController : MonoBehaviour
{
  private Rigidbody body;
  private Quaternion originRotation;
  [SerializeField] private float speed = 50f;
  [SerializeField] private float torqueSpeed = 40f;
  [SerializeField] private float torqueRecoverySpeed = 25f;
  [SerializeField] private float maxSpeed = 15f;
  [SerializeField] private float spinCooldown = .2f;
  private float movementX = 0f;
  private float movementZ = 0f;
  private float nextSpin = 0f;

  #region Monobehaviour methods
  void Start()
  {
    body = GetComponent<Rigidbody>();
    body.maxAngularVelocity = 500f;
    originRotation = transform.rotation;
  }

  void Update()
  {
    movementX = Input.GetAxisRaw("Vertical") * -speed;
    movementZ = Input.GetAxisRaw("Horizontal") * speed;
    if (Input.GetButtonDown("Fire1") && Time.time > nextSpin)
    {
      Spin();
      nextSpin = Time.time + spinCooldown;
    }
  }


  void FixedUpdate()
  {
    body.AddForce(new Vector3(movementX, 0, movementZ), ForceMode.VelocityChange);
    if (body.velocity.magnitude > maxSpeed) body.velocity = body.velocity.normalized * maxSpeed;
  }

  void LateUpdate()
  {
    ClampPosition();
  }
  #endregion

  void ClampPosition()
  {
    var pos = transform.position;
    pos.x = Mathf.Clamp(transform.position.x, -13f, 13f);
    pos.z = Mathf.Clamp(transform.position.z, 20.5f, 29f);
    transform.position = pos;
  }

  void Spin()
  {
    transform.DORotate(new Vector3(0, 180f, 0), .4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
  }

  void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.tag == "Ball")
    {
      Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
      Vector3 newForce = new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
      ballRb.AddForce(newForce, ForceMode.VelocityChange);
    }
  }
}
