using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copernicus : MonoBehaviour
{
    public GameObject telescopeObject;
    public GameObject bookshelfObject;
    public GameObject writingStandObject;
    public GameObject toiletObject;
    //[Label("Copernicus states")]
    public bool isStanding = false;
    public bool isWorking = false;

    // Score Tracking
    public GameObject mainProgressBarObject;
    private RectTransform innerScoreBar;
    private RectTransform outerScoreBar;
    public float score = 0f;
    public float scoreIncreaseWhileWorking = 100f;
    public float scoreTarget = 1800f;
    public float scoreBarHeight = 100f;
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
        // TODO currently only scales bar while Copernicus is working
        if (isWorking)
        {
            float proportionFilled = score / scoreTarget;
            // fill by the percentage of current maxSize
            innerScoreBar.GetComponent<RectTransform>().sizeDelta =
            new Vector2(proportionFilled * outerScoreBar.sizeDelta.x, scoreBarHeight);

            // Append score while working
            score += Time.deltaTime * scoreIncreaseWhileWorking;
        }

        if (score >= scoreTarget)
        {
            Debug.Log("Player won, copernicus big brained the nocna solucja!");
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
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
