using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModels : MonoBehaviour
{
    [SerializeField] Transform m_PickaxeSlot;
    [SerializeField] Animator m_Animator;

    public Animator GetAnimator() => m_Animator;
    public Transform GetPickaxeSlot() => m_PickaxeSlot;
}
