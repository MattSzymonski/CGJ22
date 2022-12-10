using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mighty;

public class Player : MonoBehaviour
{
    private const string ENEMY = "Enemy";
    private const string PORTAL = "Portal";

    Copernicus copernicus;

    public const int controllerNr = 1;
    public float movementSpeed = 100.0f;
    private Vector2 movementDirection;
    //private Vector2 lookDirection;
    //private Vector2 previousLookDirection;

    public float currentSkillRadius;
    public float maxSkillRadius = 4.0f;
    public float skillChargeSpeed = 1.0f;
    public float skillCooldownTimeout = 2.0f;
    public float skillChargeTimeout = 1.0f;

    public float helpingDistance = 10.0f;

    private Rigidbody2D rb;

    private MightyTimer skillCooldownTimer;
    private MightyTimer skillChargeTimer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        skillCooldownTimer = MightyTimersManager.Instance.CreateTimer("PlayerSkillCooldownTimer", skillCooldownTimeout, 1f, false, true);
        skillCooldownTimer.finished = true;
        skillChargeTimer = MightyTimersManager.Instance.CreateTimer("PlayerSkillChargeTimer", skillChargeTimeout, 1f, false, true);
        copernicus = GameObject.FindGameObjectWithTag("Copernicus").GetComponent<Copernicus>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        PlayerAction();
        PlayerHelp();
        //Rotation();
    }
    
    void Move() //Interpreting player controllers input
    {
        movementDirection = new Vector2(Input.GetAxis("Controller" + controllerNr + " Left Stick Horizontal"),  -Input.GetAxis("Controller" + controllerNr + " Left Stick Vertical")) * movementSpeed;
        DebugExtension.DebugArrow(transform.position, movementDirection);
        rb.velocity = new Vector2(movementDirection.x, movementDirection.y);
    }

    void PlayerAction()
    {
        if (Input.GetButton("Controller" + controllerNr + " X") && skillCooldownTimer.finished && currentSkillRadius < maxSkillRadius)
        {
            //Debug.Log("Charging progress: " + currentSkillRadius);
            // charge the radius 
            currentSkillRadius += skillChargeSpeed * Time.deltaTime;
            DebugExtension.DebugCircle(Mighty.MightyUtilites.Vec3ToVec2(transform.position), Vector3.forward, currentSkillRadius);

        } else if ((Input.GetButtonUp("Controller" + controllerNr + " X") && skillCooldownTimer.finished)
                    || skillChargeTimer.finished || currentSkillRadius > maxSkillRadius) 
        {
            // use the skill
            // if area overlaps portals or enemies destroy them essa
            Debug.Log("Released");

            var objects = Physics2D.OverlapCircleAll(transform.position, currentSkillRadius);
            foreach (var obj in objects)
            {
                if (obj.CompareTag(ENEMY))
                    obj.GetComponent<Enemy>().Die();
                else if (obj.CompareTag(PORTAL))
                    Debug.Log("NOT IPMLEMENTED PORTAL");
            }
            currentSkillRadius = 0f;
            skillCooldownTimer.RestartTimer();
        }
    }

    void PlayerHelp()
    {
        if (Vector2.Distance(Mighty.MightyUtilites.Vec3ToVec2(transform.position), Mighty.MightyUtilites.Vec3ToVec2(copernicus.transform.position)) < helpingDistance)
        {
            copernicus.beingHelped = true;
        }
        else
        {
            copernicus.beingHelped = false;
        }
    }

    /*
    void Rotation() // Calculating angle between player joystick right stick declension
    {
        lookDirection = new Vector2(Input.GetAxis("Controller" + controllerNumber + " Right Stick Horizontal"),  -Input.GetAxis("Controller" + controllerNumber + " Right Stick Vertical")).normalized;
        if (lookDirection == Vector2.zero)
        {
            if (previousLookDirection == Vector2.zero) //for fixing Zero roation quat
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(previousLookDirection, Vector2.up);
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector2.up);
            previousLookDirection = lookDirection;
        }

        DebugExtension.DebugArrow(transform.position, lookDirection * 10, Color.yellow);
    }
    */
}
