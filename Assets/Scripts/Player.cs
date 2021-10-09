using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float jumpAmount = 60f;

    private Animator animator;
    private Rigidbody2D rigidbody2d;
    public event EventHandler OnDeath;
    private static Player instance;
    private bool playerDead = false;
    public static Player GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = FindObjectOfType<Animator>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && !playerDead)
        {
            rigidbody2d.velocity = Vector2.up * jumpAmount;
            Debug.Log("Jump " + context.phase);
            SoundManager.PlaySound(SoundManager.SoundType.playerJump);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rigidbody2d.bodyType = RigidbodyType2D.Static;
        playerDead = true;
        animator.GetComponent<Animator>().enabled = false;
        if (OnDeath != null) OnDeath(this, EventArgs.Empty);
        SoundManager.PlaySound(SoundManager.SoundType.Lose);
    }
}
