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
  [SerializeField] private float speed = 50f;
  [SerializeField] private float maxSpeed = 15f;
  [SerializeField] private float spinCooldown = .2f;

  [SerializeField] private Vector2 zBounds;
  [SerializeField] private Vector2 xBounds;

  private float movementX = 0f;
  private float movementZ = 0f;
  private float nextSpin = 0f;

  #region Monobehaviour methods
  void Start()
  {
    body = GetComponent<Rigidbody>();
    body.maxAngularVelocity = 500f;
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
    pos.x = Mathf.Clamp(transform.position.x, xBounds.x, xBounds.y);
    pos.z = Mathf.Clamp(transform.position.z, zBounds.x, zBounds.y);
    transform.position = pos;
  }

  void Spin()
  {
    transform.localScale = new Vector3(6f, 1f, 3f);
    transform.DOScaleX(3f, .8f).SetEase(Ease.OutBack);
    transform.DOScaleZ(1f, .8f).SetEase(Ease.OutBack);
    transform.DORotate(new Vector3(0, 180f, 0), .8f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
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
