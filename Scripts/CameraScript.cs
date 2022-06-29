using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform player;
    public Transform camerafollow;

    void Start()
    {
        camerafollow = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y, camerafollow.transform.position.z);
        camerafollow.transform.position = playerPos;
    }
}
