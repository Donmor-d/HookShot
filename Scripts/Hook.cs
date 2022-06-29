using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    // Start is called before the first frame update

    public Rigidbody2D HookRB;
    public SpringJoint2D HookSpring;

    public BoxCollider2D HookCollider;

    public Rigidbody2D Owner;
    public GameObject LineOBJ;
    public LineRenderer Line;

    void Start()
    {
        HookRB = GetComponent<Rigidbody2D>();
        HookCollider = GetComponent<BoxCollider2D>();
        HookSpring = GetComponent<SpringJoint2D>();
        Owner = GameObject.Find("Player").GetComponent<Rigidbody2D>();

        LineOBJ = Instantiate(LineOBJ);
        Line = LineOBJ.GetComponent<LineRenderer>();

        HookSpring.enabled = false;
        HookSpring.connectedBody = Owner;

    }

    // Update is called once per frame
    void Update()
    {
        var Pos = new Vector3[2];

        Pos[0] = Owner.transform.position;
        Pos[1] = this.transform.position;

        Line.SetPositions(Pos);

        //Debug.Log(Mathf.Sqrt(Mathf.Pow((Pos[0].x - Pos[1].x), 2) + Mathf.Pow((Pos[0].y - Pos[1].y), 2)).ToString());

        if (Mathf.Sqrt( Mathf.Pow((Pos[0].x - Pos[1].x), 2) + Mathf.Pow((Pos[0].y - Pos[1].y), 2)) > 5.1) // se distancia for maior que 5, destruir. Cálculo de hipotenusa
        {
            Destroy(LineOBJ);
            Destroy(this.gameObject);
        }

        if (Player.IsHooked)
        {
            HookSpring.distance = Mathf.Clamp(HookSpring.distance + 0.02f * -Input.GetAxis("Vertical"), 1, 5);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HookSpring.enabled = true;
        HookRB.velocity = Vector2.zero;
        HookRB.bodyType = RigidbodyType2D.Kinematic;
        HookRB.transform.parent = collision.gameObject.transform;
        Player.IsHooked = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        HookRB.transform.parent = null;
    }

    private void OnDestroy()
    {
        Destroy(LineOBJ);
    }
}
