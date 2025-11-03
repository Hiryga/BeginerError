using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseScroller : MonoBehaviour
{
    [SerializeField] private RawImage noiseImage;
    [SerializeField] private float speed = 0.05f;

    private void Update()
    {
        noiseImage.uvRect = new Rect(
            noiseImage.uvRect.x + speed * Time.deltaTime,
            noiseImage.uvRect.y + speed * 0.5f * Time.deltaTime,
            1, 1
        );
    }
}