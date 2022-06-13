using UnityEngine;
using System.Collections;

public class viveHighlight : MonoBehaviour
{
    private GameObject currentObject; //The currently highlighted object
    private Color oldColor; //The old color of the currentObject
    private float highlightFactor = 0.5f; //How much should the object be highlighted

    // Update is called once per frame
    //void Update()
    //{
    //    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(r, out hit))
    //    { //IF we have hit something
    //        if (currentObject == null)
    //        { //IF we haved no current object
    //            currentObject = hit.transform.gameObject; //save the object
    //            HighlightCurrentObject(); //and highlight it
    //        }
    //        else if (hit.transform != currentObject.transform)
    //        { //ELSE IF we have hit a different object
    //            RestoreCurrentObject(); //THEN restore the old object
    //            currentObject = hit.transform.gameObject; //save the new object
    //            HighlightCurrentObject(); //and highlight it

    //        }
    //    }
    //    else //ELSE no object was hit
    //        RestoreCurrentObject(); //THEN restore the old object
    //}

    public void highlight(RaycastHit hit)
    {

        if (currentObject == null)
        { //IF we haved no current object
            currentObject = hit.transform.gameObject; //save the object
            HighlightCurrentObject(); //and highlight it
        }
        else if (hit.transform != currentObject.transform)
        { //ELSE IF we have hit a different object
            //RestoreCurrentObject(); //THEN restore the old object
            currentObject = hit.transform.gameObject; //save the new object
            HighlightCurrentObject(); //and highlight it

        }
        else
        {
            //ELSE no object was hit
            //RestoreCurrentObject(); //THEN restore the old object
        }
    }

    private void HighlightCurrentObject()
    {
        Debug.Log("highlight ran");
        Renderer r = currentObject.GetComponent(typeof(Renderer)) as Renderer;
        oldColor = r.material.GetColor("_Color");
        Color newColor = new Color(oldColor.r + highlightFactor, oldColor.g + highlightFactor, oldColor.b + highlightFactor, oldColor.a);
        r.material.SetColor("_Color", newColor);
    }

    //Restores the current object to it's formaer state.
    private void RestoreCurrentObject()
    {
        Debug.Log("Restore ran");
        if (currentObject != null)
        { //IF we actually have an object to restore
            Renderer r = currentObject.GetComponent(typeof(Renderer)) as Renderer;
            r.material.SetColor("_Color", oldColor);
            currentObject = null;
        }
    }
}