using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copernicus : MonoBehaviour
{
    public GameObject telescopeObject;
    public GameObject bookshelfObject;
    public GameObject writingStandObject;
    public GameObject toiletObject;
    [Header("Copernicus states")]
    public bool isStanding = false;
    public bool isWorking = false;
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
    public float scoreBarHeight = 100f;

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

    private float interestDecreaseRate = 30f;
    // Start is called before the first frame update
    void Start()
    {
        innerScoreBar = mainProgressBarObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        outerScoreBar = mainProgressBarObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        outerScoreBar.sizeDelta = new Vector2(scoreTarget, scoreBarHeight);
        // Set inital progress bar
        float proportionFilled = score / scoreTarget;
        // fill by the percentage of current maxSize
        innerScoreBar.GetComponent<RectTransform>().sizeDelta =
        new Vector2(proportionFilled * outerScoreBar.sizeDelta.x, scoreBarHeight);
    }

    // Update is called once per frame
    void Update()
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
            new Vector2(proportionFilled * outerScoreBar.sizeDelta.x, scoreBarHeight);

            // Append score while working
            score += (beingHelped ? scoreIncreaseWhileWorkingHelped : scoreIncreaseWhileWorking) * Time.deltaTime;

            // Interest decreasing for work station currently attended
            if (workstationName == "telescope")
                telescopeInterest -= Time.deltaTime * interestDecreaseRate;
            else if (workstationName == "bookshelf")
                bookshelfInterest -= Time.deltaTime * interestDecreaseRate;
            else if (workstationName == "writingstand")
                writingStandInterest -= Time.deltaTime * interestDecreaseRate;
            else if (workstationName == "toilet")
                toiletInterest -= Time.deltaTime * interestDecreaseRate;

        }

        if (score >= scoreTarget)
        {
            Debug.Log("Player won, copernicus big brained the nocna solucja!");
            // TODO ENTER GAME END: PLAYER VICTORY
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGERRED");
        if (isStanding)
        {
            if (collision.gameObject.CompareTag("telescope"))
            {
                // Currently looking through the telescope, progress the progress bar
                isWorking = true;
                workstationName = "telescope";
            }
            else if (collision.gameObject.CompareTag("bookshelf"))
            {
                // Currently reading at the bookshelf, progress the progress bar
                isWorking = true;
                workstationName = "bookshelf";
            }
            else if (collision.gameObject.CompareTag("writingstand"))
            {
                // Currently writing notes at the writing stand, progress the progress bar
                isWorking = true;
                workstationName = "writingstand";
            }
            else if (collision.gameObject.CompareTag("toilet"))
            {
                // Currently relieving himself on the toilet, progress the progress bar
                isWorking = true;
                workstationName = "toilet";
            }
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("TRIGERRED EXIT");
        // TODO might want to check if is standing or not
        if (collision.gameObject.CompareTag("telescope"))
        {
            // No longer looking through the telescope
            isWorking = false;
            workstationName = "";
        }
        else if (collision.gameObject.CompareTag("bookshelf"))
        {
            // No longer reading at the bookshelf, progress the progress bar
            isWorking = false;
            workstationName = "";
        }
        else if (collision.gameObject.CompareTag("writingstand"))
        {
            // No longer writing notes at the writing stand, progress the progress bar
            isWorking = false;
            workstationName = "";
        }
        else if (collision.gameObject.CompareTag("toilet"))
        {
            // No longer relieving himself on the toilet, progress the progress bar
            isWorking = false;
            workstationName = "";
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
