using TMPro;
using UnityEngine;


public class FindPlayerData : MonoBehaviour
{
    public TMP_InputField name;
    //public TMP_InputField playerid;
    public FetchData fetch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
 

    // Update is called once per frame
    void Update()
    {

    }
    public void SearchForPlayer()
    {
        if (name.text != "")
        {
            fetch.SetupPlayerSearchData(name.text);
        }
    }
}