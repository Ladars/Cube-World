using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;

    private bool isActive = false;
    [SerializeField] private LineRenderer lineRenderer;
    public Transform laserOrigin;

    private void Awake()
    {
        //      lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
    }

    void Update()
    {
        if (isActive)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            lineRenderer.SetPosition(0, laserOrigin.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * 100f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void Activate()
    {
        isActive = true;
        lineRenderer.enabled = true;
    }
}
