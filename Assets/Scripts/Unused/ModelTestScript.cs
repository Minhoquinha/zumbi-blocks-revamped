using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ModelTestScript : MonoBehaviour
{
    public bool Physics;

    public bool FloorVisible;
    public Material [] FloorMaterials;
    private int MaterialNum;
    private Material CurrentMaterial;

    public ItemCollection ItemCollectionScript;
    public Rigidbody [] Models;

    void Start()
    {
        GetComponent<MeshRenderer>().material = FloorMaterials [0];
        MaterialNum = 0;

        Physics = false;
        for (int i = 0; i < Models.Length; i++)
        {
            if (ItemCollectionScript.ItemArray [i].GetComponent<Rigidbody>() != null)
            {
                Models [i] = ItemCollectionScript.ItemArray [i].GetComponent<Rigidbody>();
                Models [i].constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (MaterialNum < FloorMaterials.Length - 1)
            {
                if (FloorVisible)
                {
                    MaterialNum++;
                }
                else
                {
                    FloorVisible = true;
                }
            }
            else
            {
                MaterialNum = 0;
                FloorVisible = false;
            }

            CurrentMaterial = FloorMaterials [MaterialNum];
            GetComponent<MeshRenderer>().enabled = FloorVisible;
            GetComponent<MeshRenderer>().material = CurrentMaterial;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Physics = !Physics;

            if (Physics)
            {
                for (int i = 0; i < Models.Length; i++)
                {
                    Models [i].constraints = RigidbodyConstraints.None;
                }
            }
            else
            {
                for (int i = 0; i < Models.Length; i++)
                {
                    Models [i].constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }
    }
}
