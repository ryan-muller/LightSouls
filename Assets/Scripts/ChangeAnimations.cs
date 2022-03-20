using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimations : MonoBehaviour
{
    private Animator animator;
    public AnimatorOverrideController OneHand_WeaponController;
    private AnimatorOverrideController originalAnimator;

    public GameObject sword;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        originalAnimator = new AnimatorOverrideController(animator.runtimeAnimatorController);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //animator.ResetTrigger("EquipWeapon");
            sword.SetActive(true);
            animator.runtimeAnimatorController = OneHand_WeaponController;
            
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            sword.SetActive(false);
            animator.runtimeAnimatorController = originalAnimator;
            //animator.ResetTrigger("HolsterWeapon");
        }


    }
    
}
