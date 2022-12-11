using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Copernicus : MonoBehaviour
{
    public GameObject telescopeObject;
    public GameObject bookshelfObject;
    public GameObject writingStandObject;
    public GameObject toiletObject;
    [Header("Copernicus states")]
    public bool isStanding = false;
    public bool isWorking = false;
    public float maximumWorkingDistance = 3.0f;
    public string workstationName = "";

    // Score Tracking
    [Header("Score Tracking")]
    public GameObject mainProgressBarObject;
    private RectTransform innerScoreBar;
    private RectTransform outerScoreBar;
    public float score = 0f;
    public float scoreIncreaseWhileWorking = 100f;
    public float scoreIncreaseWhileWorkingHelped = 100f;
    public bool beingHelped;
    public float scoreTarget = 1800f;
    public float scoreBarHeightInner = 40f;
    public float scoreBarHeightOuter = 50f;

    // Interest levels
    [Header("Interest levels")]
    public float telescopeInterest = 0f;
    public float telescopeInterestMax = 100f;
    public float bookshelfInterest = 0f;
    public float bookshelfnterestMax = 100f;
    public float writingStandInterest = 0f;
    public float writingStandInterestMax = 100f;
    public float toiletInterest = 0f;
    public float toiletInterestMax = 100f;

    public float telescopeInterestIncreaseRate = 10f;
    public float bookshelfInterestIncreaseRate = 10f;
    public float writingStandInterestIncreaseRate = 10f;
    public float toiletInterestIncreaseRate = 10f;

    public float interestDecreaseRate = 30f;
    public float interestChangeMaxThreshold = 500f;
    [SerializeField]
    private string currentInterest = "";

    // Copernicus movement
    [Header("Copernicus Movement")]
    private AgentMovement agentMovement;
    private NavMeshAgent navMeshAgent;
    private float minVelocityToConsiderMoving = 0.1f;
    private Animator copernicusAnimator;
    private Vector3 lookDirection;

    // Enemy detection
    [Header("Enemy detection")]
    public GameObject currentRoom;
    private MainGameManager gameManager;

    // Copernicus Action hints
    [Header("Copernicus action hints")]
    public Sprite[] actionHintSprites;
    private SpriteRenderer actionHintRenderer;
    private float actionHintTime = 3.0f;

    // FoV
    [Header("Field of View")]
    private FieldOfView fov;

    // Start is called before the first frame update
    void Start()
    {
        fov = FindObjectOfType<FieldOfView>();

        copernicusAnimator = transform.GetChild(1).gameObject.GetComponent<Animator>();
        actionHintRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        innerScoreBar = mainProgressBarObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        outerScoreBar = mainProgressBarObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        outerScoreBar.sizeDelta = new Vector2(scoreTarget, scoreBarHeightOuter);
        // Set inital progress bar
        float proportionFilled = score / scoreTarget;
        // fill by the percentage of current maxSize
        innerScoreBar.GetComponent<RectTransform>().sizeDelta =
        new Vector2(proportionFilled * outerScoreBar.sizeDelta.x, scoreBarHeightInner);

        gameManager = GameObject.Find("GameManager").GetComponent<MainGameManager>();
        // Initialise interest at random
        Vector2 initialInterestRange = new Vector2(0f, 100f);
        telescopeInterest = Random.Range(initialInterestRange[0], initialInterestRange[1]);
        bookshelfInterest = Random.Range(initialInterestRange[0], initialInterestRange[1]);
        writingStandInterest = Random.Range(initialInterestRange[0], initialInterestRange[1]);
        toiletInterest = Random.Range(initialInterestRange[0], initialInterestRange[1]);

        // Initialise initial point of interest
        currentInterest = getMostInterestingWorkstation().Item1;

        // Initialise first movement targe 
        agentMovement = GetComponent<AgentMovement>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        setMovementTargetToCurrentInterest();
        displayCopernicusActionHint();
    }

    // Update is called once per frame
    void Update()
    {
        if (MainGameManager.Instance.notPlaying)
            return;
        /*	
        if (DetectEnemy())	
        {	
            Debug.Log("Sees enemy kurwa");	
            // TODO GAME OVER!	
            displayCopernicusActionHint(true);	
            return;	
        }	
        */
        // Check if moving
        if (navMeshAgent.velocity.magnitude > minVelocityToConsiderMoving)
        {
            lookDirection = navMeshAgent.velocity;
            isStanding = false;
            copernicusAnimator.SetBool("isWorking", false);
        }
        else
        {
            isStanding = true;
            copernicusAnimator.SetBool("isWorking", true);
        }
        checkIfWorking();
        updateInterests();
        checkIfInterestChanged();

        fov.SetOrigin(transform.position);
        fov.SetAimDirection(lookDirection);

        if (score >= scoreTarget)
        {
            Debug.Log("Player won, copernicus big brained the nocna solucja!");
            MainGameManager.Instance.notPlaying = true;
            MainGameManager.Instance.Victory();
        }
    }
    public void EnemySighted()
    {
        Debug.Log("Found ENEMY - DIE");
        MainGameManager.Instance.notPlaying = true;
        MainGameManager.Instance.GameOver();
    }
    public void PlayerSighted()
    {
        Debug.Log("Found PLAYER - ALSO DIE");
        MainGameManager.Instance.notPlaying = true;
        MainGameManager.Instance.GameOver();
    }


    private (string, float) getMostInterestingWorkstation()
    {
        string mostInterestingWorkstationName = Utils.TELESCOPE;
        float highestInterestValue = telescopeInterest;

        if (bookshelfInterest > highestInterestValue)
        {
            mostInterestingWorkstationName = Utils.BOOKSHELF;
            highestInterestValue = bookshelfInterest;
        }
        if (writingStandInterest > highestInterestValue)
        {
            mostInterestingWorkstationName = Utils.WRITING_STAND;
            highestInterestValue = writingStandInterest;
        }
        if (toiletInterest > highestInterestValue)
        {
            mostInterestingWorkstationName = Utils.TOILET;
            highestInterestValue = toiletInterest;
        }
        return (mostInterestingWorkstationName, highestInterestValue);
    }

    private float getCurrentWorkstationInterestLevel()
    {
        if (currentInterest == Utils.TELESCOPE)
        {
            return telescopeInterest;
        }
        if (currentInterest == Utils.BOOKSHELF)
        {
            return bookshelfInterest;
        }
        if (currentInterest == Utils.WRITING_STAND)
        {
            return writingStandInterest;
        }
        if (currentInterest == Utils.TOILET)
        {
            return toiletInterest;
        }
        // otherwise, no interest (0)
        return 0f;
    }

    private void setMovementTargetToCurrentInterest()
    {
        if (currentInterest == Utils.TELESCOPE)
        {
            agentMovement.SetTarget(Mighty.MightyUtilites.Vec3ToVec2(telescopeObject.transform.position));
        }
        else if (currentInterest == Utils.BOOKSHELF)
        {
            agentMovement.SetTarget(Mighty.MightyUtilites.Vec3ToVec2(bookshelfObject.transform.position));
        }
        else if (currentInterest == Utils.WRITING_STAND)
        {
            agentMovement.SetTarget(Mighty.MightyUtilites.Vec3ToVec2(writingStandObject.transform.position));
        }
        else if (currentInterest == Utils.TOILET)
        {
            agentMovement.SetTarget(Mighty.MightyUtilites.Vec3ToVec2(toiletObject.transform.position));
        }
    }

    private void checkIfWorking()
    {
        if (isStanding)
        {
            if (Vector2.Distance(Mighty.MightyUtilites.Vec3ToVec2(transform.position), Mighty.MightyUtilites.Vec3ToVec2(telescopeObject.transform.position)) < maximumWorkingDistance)
            {
                // Currently looking through the telescope, progress the progress bar
                isWorking = true;
                workstationName = Utils.TELESCOPE;
            }
            else if (Vector2.Distance(Mighty.MightyUtilites.Vec3ToVec2(transform.position), Mighty.MightyUtilites.Vec3ToVec2(bookshelfObject.transform.position)) < maximumWorkingDistance)
            {
                // Currently reading at the bookshelf, progress the progress bar
                isWorking = true;
                workstationName = Utils.BOOKSHELF;
            }
            else if (Vector2.Distance(Mighty.MightyUtilites.Vec3ToVec2(transform.position), Mighty.MightyUtilites.Vec3ToVec2(writingStandObject.transform.position)) < maximumWorkingDistance)
            {
                // Currently writing notes at the writing stand, progress the progress bar
                isWorking = true;
                workstationName = Utils.WRITING_STAND;
            }
            else if (Vector2.Distance(Mighty.MightyUtilites.Vec3ToVec2(transform.position), Mighty.MightyUtilites.Vec3ToVec2(toiletObject.transform.position)) < maximumWorkingDistance)
            {
                // Currently relieving himself on the toilet, progress the progress bar
                isWorking = true;
                workstationName = Utils.TOILET;
            }
            else
            {
                // No longer working, away from all stations
                isWorking = false;
                workstationName = "";
            }
        }
        else
        {
            // Currently walking so no working!
            isWorking = false;
            workstationName = "";
        }
    }

    private void updateInterests()
    {
        telescopeInterest += Time.deltaTime * telescopeInterestIncreaseRate;
        bookshelfInterest += Time.deltaTime * bookshelfInterestIncreaseRate;
        writingStandInterest += Time.deltaTime * writingStandInterestIncreaseRate;
        toiletInterest += Time.deltaTime * toiletInterestIncreaseRate;
        // TODO currently only scales bar while Copernicus is working
        if (isWorking)
        {
            float proportionFilled = score / scoreTarget;
            // fill by the percentage of current maxSize
            innerScoreBar.GetComponent<RectTransform>().sizeDelta =
            new Vector2(proportionFilled * outerScoreBar.sizeDelta.x, scoreBarHeightInner);

            // Append score while working
            score += (beingHelped ? scoreIncreaseWhileWorkingHelped : scoreIncreaseWhileWorking) * Time.deltaTime;

            // Interest decreasing for work station currently attended
            if (workstationName == Utils.TELESCOPE)
                telescopeInterest -= Time.deltaTime * interestDecreaseRate;
            else if (workstationName == Utils.BOOKSHELF)
                bookshelfInterest -= Time.deltaTime * interestDecreaseRate;
            else if (workstationName == Utils.WRITING_STAND)
                writingStandInterest -= Time.deltaTime * interestDecreaseRate;
            else if (workstationName == Utils.TOILET)
                toiletInterest -= Time.deltaTime * interestDecreaseRate;

        }
    }

    private void checkIfInterestChanged()
    {
        var mostInterestingVars = getMostInterestingWorkstation();
        string mostInterestingWorkstation = mostInterestingVars.Item1;
        float mostInterestingInterestLevel = mostInterestingVars.Item2;
        float currentWorkstationInterestLevel = getCurrentWorkstationInterestLevel();

        // If mostInterestingWorkstation is the current workstation, skip
        if (mostInterestingWorkstation == currentInterest)
            return;

        // if current workstation level difference to most interesting: get most interesting
        if (currentWorkstationInterestLevel + interestChangeMaxThreshold < mostInterestingInterestLevel)
        {
            currentInterest = mostInterestingWorkstation;
            setMovementTargetToCurrentInterest();
            displayCopernicusActionHint();

        }
    }

    private void displayCopernicusActionHint(bool seeEnemy = false)
    {
        CancelInvoke("CleanCopernicusActionHint");
        // Display a bubble above copernicus's head on his next action
        // TODO Display Warning when Big C sees an enemy
        if (seeEnemy)
        {
            // Copernicus saw the enemy ! he dead
            actionHintRenderer.sprite = actionHintSprites[4];
            actionHintRenderer.gameObject.SetActive(true);
            return;
        }
        if (currentInterest == Utils.TELESCOPE)
        {
            actionHintRenderer.sprite = actionHintSprites[0];
        }
        else if (currentInterest == Utils.BOOKSHELF)
        {
            actionHintRenderer.sprite = actionHintSprites[1];
        }
        else if (currentInterest == Utils.WRITING_STAND)
        {
            actionHintRenderer.sprite = actionHintSprites[2];
        }
        else if (currentInterest == Utils.TOILET)
        {
            actionHintRenderer.sprite = actionHintSprites[3];
        }

        Invoke("CleanCopernicusActionHint", actionHintTime);
        actionHintRenderer.gameObject.SetActive(true);
    }

    private void CleanCopernicusActionHint()
    {
        // Remove sprite instead of disabling?actionHintRenderer.sprite.
        actionHintRenderer.gameObject.SetActive(false);
    }

    private bool DetectEnemy()
    {
        if (!currentRoom)
        {
            Debug.LogError("Something Wrong, no room assigned to Big C");
            return false;
        }

        foreach (var room in gameManager.rooms)
        {
            if (room == currentRoom)
            {
                if (room.GetComponent<Room>().enemies.Count > 0)
                    return true;
            }
        }

        return false;
    }
}
