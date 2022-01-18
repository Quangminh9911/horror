using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontdestroy : MonoBehaviour
{
    [HideInInspector]
    public string objectId;

    private void Awake()
    {
        objectId = name + transform.position.ToString() + transform.eulerAngles.ToString();
    }


    void Start()
    {
      for (int i =0; i < Object.FindObjectsOfType<dontdestroy>().Length; i++)
        {
            if(Object.FindObjectsOfType<dontdestroy>()[i] != this)
            {
                if (Object.FindObjectsOfType<dontdestroy>()[i].objectId == objectId)
                {
                    Destroy(gameObject);
                }
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    
}
