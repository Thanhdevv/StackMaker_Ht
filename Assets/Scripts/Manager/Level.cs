using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform startPos;

    public Transform StartPos { get => startPos; set => startPos = value; }
}
