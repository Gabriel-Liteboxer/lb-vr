using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagModularity : MonoBehaviour
{
    Dictionary<string, GameObject> TaggedObjects = new Dictionary<string, GameObject>();

    public GameObject FindTaggedObject(string aTag)
    {
        if(TaggedObjects.ContainsKey(aTag))
        {
            Debug.Log("located GameObject with tag " + aTag + " in dictionary");

            return TaggedObjects[aTag];

        }
        else
        {
            GameObject tagObj = GameObject.FindGameObjectWithTag(aTag);

            

            if(tagObj != null)
            {
                TaggedObjects.Add(aTag, tagObj);

                Debug.Log("found GameObject with tag " + aTag + " and added to dictionary");

                return tagObj;
            }
            else
            {
                Debug.Log("no object was found with tag " + aTag);
                return null;

            }

        }

    }
}
