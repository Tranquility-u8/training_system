using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberController : MonoBehaviour
{
    [SerializeField] [Range(45, 120)] private int value = 0;
    public int Value => value;
}
