using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusimojiBullet : MonoBehaviour
{
    public float moveSpeed = 1f;

    public SpriteRenderer renderer;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position += transform.up * (moveSpeed * Time.deltaTime);
    }

    public void SetSprite(Sprite sprite)
    {
        renderer.sprite = sprite;
    }
}
