using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class PlayerModels : MonoBehaviour
{
    public Transform m_headAimTarget;

    [SerializeField] Transform m_PickaxeSlot;
    [SerializeField] Animator m_Animator;

    public Animator GetAnimator() => m_Animator;
    public Transform GetPickaxeSlot() => m_PickaxeSlot;
}
