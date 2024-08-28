using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Attributes")]
    [SerializeField] private float smoothTime;

    private Vector3 offset = new Vector3 (0, 0, -10f);
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        target = Player.instance.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
