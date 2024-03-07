using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class Sign : MonoBehaviour
{
    private PlayerInputContorl playerInput;
    private Animator anim;
    public Transform playerTran;
    public GameObject signSprite;
    private IInteractable targetItem;
    private bool canPress;

    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputContorl();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInput.Gameplay.confirm.started += OnConfirm;
    }

    private void OnDestroy()
    {
        canPress = false;
    }


    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled=canPress;
        signSprite.transform.localScale = playerTran.localScale;
    } 

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (canPress)
        {
            targetItem.TrggerAction();
            GetComponent<AudioDefination>()?.PlaAudioClip();
        }
    }

    //切换设备同时切换动画
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            // Debug.Log(((InputAction)obj).activeControl.device);

            var d = ((InputAction)obj).activeControl.device;

            switch (d.device)
            {
                case Keyboard:
                    anim.Play("keyboard");
                    break;
                case DualShockGamepad:
                    anim.Play("ps");
                    break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = collision.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPress = false;
    }
}

