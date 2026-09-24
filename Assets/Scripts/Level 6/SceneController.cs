using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [Header("Unity Error Popups")]
    [SerializeField] private Image errorImage;
    [SerializeField] private Texture awarenessError;
    [SerializeField] private Texture missingObjectError;
    [SerializeField] private Texture invalidObjectError;

    [Header("Objects to be deleted/disabled")]
    [SerializeField] private GameObject debrisParent;
    [SerializeField] private GameObject experimentParent;
    [SerializeField] private GameObject facilityScifiWallsParent;
    [SerializeField] private GameObject facilityFurnitureParent;
    [SerializeField] private GameObject officeAreaParent;
    [SerializeField] private GameObject facilityWallsParent;
    [SerializeField] private GameObject facilitySetupParent;
    [SerializeField] private GameObject playerBody;

    [Header("Scripts to Manipulate")]
    [SerializeField] private PlayerMovement playerMovement;
    


    public void ShowAwarenessError()
    {
       //set error image to awarenesserror
    }

    public void ShowMissingObjectError()
    {

    }

    public void ShowInvalidObjectError()
    {

    }

    public void DisableFirstObject()
    {
        debrisParent.SetActive(false);
    }

    public void DisableSecondObject()
    {
        experimentParent.SetActive(false);
    }

    public void DisableThridObject()
    {
        facilityScifiWallsParent.SetActive(false);
    }

    public void EnableInitialObjects()
    {
        debrisParent.SetActive(true);
        experimentParent.SetActive(true);
        facilityScifiWallsParent.SetActive(true);
    }

    public void DisableWholeScene()
    {
        StartCoroutine(SceneDisable());
    }

    private IEnumerator SceneDisable()
    {
        facilityFurnitureParent.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        debrisParent.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        experimentParent.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        facilityScifiWallsParent.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        officeAreaParent.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        facilityWallsParent.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        facilitySetupParent.SetActive(false);
    }

    public void SlowPlayerMovement()
    {

    }

    public void DisablePlayerWASDMovement()
    {

    }

    public void EnablePlayerWASDMovement()
    {

    }

    public void DisablePlayerMouseLook()
    {

    }

    public void EnablePlayerMouseLook()
    {

    }    

    public void DisablePlayerBody()
    {

    }

    public void EnablePlayerBody()
    {

    }

    public void RemoveFacilityCompletely()
    {
        facilitySetupParent.SetActive(false);
    }

    public void LeaveEnding()
    {
        //screen goes black
        //"APPLICATION EXITING..."
        //game 'closes'
        //terminal window appears and displays "SEONCD_PERSON_STUDY, SUBJECT STATUS: ACTIVE"
        //terminal closes
        //game quits
    }

    public void RebuildNewWorld()
    {
        //start enabled parent objects of new world one by one
    }

    public void StayEnding()
    {
        //fade to black
        //terminal appears and shows "BUILDING..." 
        //then "BUILD FAILED"
        //protag says "oh."
        //exit to main menu
    }
}
