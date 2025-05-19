using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamWarning : MonoBehaviour
{
    public float fadeSpeed = 3f;
    private SpriteRenderer sr;

    void Start() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        float alpha = Mathf.PingPong(Time.time * fadeSpeed, 1f);
        sr.color = new Color(1, 0, 0, alpha);
    }
}