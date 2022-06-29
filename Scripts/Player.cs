using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 MousePos;

    public Material RocketMat;
    public Material AirMat;

    public Rigidbody2D PlayerRB;
    public Rigidbody2D Projectile;
    public Rigidbody2D HookProj = null;

    public LayerMask ProjectileLayer;

    public Transform Aim;

    public Transform GroundCheck;
    public Transform LeftWallCheck;
    public Transform RightWallCheck;
    public LayerMask GroundLayer;

    public bool Jump;
    public bool WallJump;
    public static bool IsHooked;

    public float MoveSpeed = 5f;
    public float JumpForce = 300f;
    public bool CanJump;

    //tem items
    public bool HasMagBoot;    //bota magnetica
    public bool HasRocketBoot; //bota foguete
    public bool HasAirBoot;    //bota de ar comprimido

    public bool RocketBootEnable;
    public bool AirBootEnable;

    public float MoveDirection = 0f;

    public bool Grounded;
    public bool OnRightWall;
    public bool OnLeftWall;

    private bool ShiftHeld = false;

    void Start()
    {
        PlayerRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        MousePos = Input.mousePosition;

        Grounded = Physics2D.OverlapCircle(GroundCheck.position, 0.9f, GroundLayer) && PlayerRB.velocity.y == 0;
        ShiftHeld = Input.GetKey(KeyCode.LeftShift);
        OnRightWall = Physics2D.OverlapBox(RightWallCheck.position, new Vector2(0.1f, 0.9f), 0, GroundLayer) && PlayerRB.velocity.x == 0 && HasMagBoot;
        OnLeftWall = Physics2D.OverlapBox(LeftWallCheck.position, new Vector2(0.1f, 0.9f), 0, GroundLayer) && PlayerRB.velocity.x == 0 && HasMagBoot;
        bool PlayerBox = Physics2D.OverlapBox(transform.position, new Vector2(0.5f, 1f), 0, ProjectileLayer);

        Inputting();
        ShootHook(PlayerBox);

        if (Input.GetKeyDown(KeyCode.E) && RocketBootEnable && AirBootEnable && Grounded && !IsHooked)
        {
            HasRocketBoot = !HasRocketBoot;
            HasAirBoot = !HasAirBoot;
        }

        if (HasRocketBoot)
        {
            GetComponent<SpriteRenderer>().material = RocketMat;
        }
        else if (HasAirBoot)
        {
            GetComponent<SpriteRenderer>().material = AirMat;
        }


    }

    public void ShootHook(bool PlayerBox)
    {
        Vector2 ShootDirection;

        ShootDirection = MousePos;
        ShootDirection = Camera.main.ScreenToWorldPoint(new Vector3(ShootDirection.x, ShootDirection.y, 0f));
        ShootDirection = new Vector2(ShootDirection.x - transform.position.x, ShootDirection.y - transform.position.y);

        // começa script pro crosshair

            var aimAngle = Mathf.Atan2(ShootDirection.y, ShootDirection.x);

            if (aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }

            var x = transform.position.x + 5f * Mathf.Cos(aimAngle);
            var y = transform.position.y + 5f * Mathf.Sin(aimAngle);

            Aim.transform.position = new Vector3(x, y, 0);

        //finaliza script do crosshair

        float H = 5 / Mathf.Sqrt(Mathf.Pow(ShootDirection.x, 2) + Mathf.Pow(ShootDirection.y, 2));

        if (Input.GetKeyDown(KeyCode.Mouse0) && HookProj == null)
        {
            HookProj = Instantiate(Projectile, transform.position, Quaternion.identity);
            HookProj.velocity = new Vector2(ShootDirection.x * H * MoveSpeed, ShootDirection.y * H * MoveSpeed);
        }
        if (!Input.GetKey(KeyCode.Mouse0) && HookProj != null)
        {
            Destroy(HookProj.gameObject);
            IsHooked = false;
        }
    }

    //debug collision hitboxes
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GroundCheck.position, 0.25f);
        Gizmos.DrawWireCube(LeftWallCheck.position, new Vector3(0.1f, 1f, 0.1f));
        Gizmos.DrawWireCube(RightWallCheck.position, new Vector3(0.1f, 1f, 0.1f));

        Gizmos.DrawWireCube(transform.position, new Vector3(0.5f, 1f, 0.1f));
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Inputting()
    {

        if (Input.GetButtonDown("Jump"))
        {
            if (Grounded)
            {
                if (ShiftHeld)
                {
                    CanJump = false;
                }
                Jump = true;
            }
            else if (CanJump && HasAirBoot)
            {
                Jump = true;
                CanJump = false;
            }
            else if (OnRightWall || OnLeftWall)
            {
                WallJump = true;
            }
        }

        if (IsHooked && ShiftHeld && HasRocketBoot)
        {
            PlayerRB.gravityScale = Mathf.Max(3f, PlayerRB.gravityScale + 2 * Time.deltaTime); //1 segundo para chegar no valor maximo
        }
        else
        {
            PlayerRB.gravityScale = 1f;
        }

        if (Input.GetButton("Jump") && HasRocketBoot && PlayerRB.velocity.y < -1 && !IsHooked)  //uso no futuro ao adicionar o gancho
        {
            PlayerRB.velocity = new Vector2(PlayerRB.velocity.x, -1f); //hover
        }
    }

    private void Move()
    {
        MoveDirection = Input.GetAxis("Horizontal");

        if ((OnRightWall || OnLeftWall) && !Grounded && !IsHooked)
        {
            PlayerRB.velocity = new Vector2(PlayerRB.velocity.x, 0);
        }
        if (Grounded)
        {
            CanJump = HasAirBoot;
            PlayerRB.velocity = new Vector2(MoveDirection * MoveSpeed * Mathf.Pow((ShiftHeld ? 2 : 1),(HasRocketBoot ? 1 : 0)), PlayerRB.velocity.y);
            //(IsSprinting ? 2 : 1) retorna 2 se verdadeiro e 1 se falso, (HasRocketBoots ? 1 : 0) retorna 1 se verdadeiro e 0 se falso -> 2^1 = 2 e 2^0 = 1
        }
        else if (!IsHooked)
        {
            if (MoveDirection > 0 && PlayerRB.velocity.x < MoveSpeed/2)
            {
                PlayerRB.velocity = new Vector2(Mathf.Min(MoveSpeed/2, PlayerRB.velocity.x + MoveSpeed * (MoveDirection/2) /* (1/PlayerRB.velocity.x)*/), PlayerRB.velocity.y);
            }
            else if (MoveDirection < 0 && PlayerRB.velocity.x > -MoveSpeed)
            {
                PlayerRB.velocity = new Vector2(Mathf.Max(-MoveSpeed/2, PlayerRB.velocity.x + MoveSpeed * (MoveDirection/2)), PlayerRB.velocity.y);
            }
        }

        if (WallJump)
        {
            if (OnRightWall)
            {
                PlayerRB.velocity = new Vector2(PlayerRB.velocity.x, 0);
                PlayerRB.AddForce(new Vector2(-200f, JumpForce * Mathf.Pow((ShiftHeld ? 2 : 1), (CanJump ? 1 : 0))));
                WallJump = false;
                if (CanJump)
                {
                    CanJump = !ShiftHeld;
                }
            }
            else
            {
                PlayerRB.velocity = new Vector2(PlayerRB.velocity.x, 0);
                PlayerRB.AddForce(new Vector2(200f, JumpForce * Mathf.Pow((ShiftHeld ? 2 : 1), (CanJump ? 1 : 0))));
                WallJump = false;
                if (CanJump)
                {
                    CanJump = !ShiftHeld;
                }
            }
        }

        if (Jump)
        {
            if (!IsHooked)
            {
            PlayerRB.velocity = new Vector2(PlayerRB.velocity.x, 0); // ao invés de somar a força ele reseta
            PlayerRB.AddForce(new Vector2(0f, JumpForce * Mathf.Pow((ShiftHeld ? 2 : 1), (CanJump ? 1 : 0)) ) );
            }
            Jump = false;
            if (CanJump)
            {
                CanJump = !ShiftHeld;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "airboot")
        {
            AirBootEnable = true;
            HasAirBoot = true;

            if (HasRocketBoot)
            {
                HasRocketBoot = false;
            }

        } else if (collision.name == "rocketboot")
        {
            RocketBootEnable = true;
            HasRocketBoot = true;

            if (HasAirBoot)
            {
                HasAirBoot = false;
            }
        }
        Destroy(collision.gameObject);
    }
}
