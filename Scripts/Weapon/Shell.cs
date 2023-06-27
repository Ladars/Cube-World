using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRB;
    public float forceMin;
    public float forceMax;

    float lifeTime = 4;
    float fadeTime = 2;
    private void Start()
    {
        float force = Random.Range(forceMin, forceMax);
        myRB.AddForce(transform.right * force);
        myRB.AddTorque(Random.insideUnitSphere*force);
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        float percent = 0;
        float fadeSpeed = 1 / fadeTime;
        Material mat = GetComponent<Renderer>().material;
        Color initialColour = mat.color;
        while(percent < 1)
        {
            percent += Time.deltaTime*fadeSpeed;
            mat.color = Color.Lerp(initialColour, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
