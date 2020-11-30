using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public delegate void onIPconnect();

public class guiHelper : MonoBehaviour
{
    public static string ipAddress;
    private string defaultIPaddr = "255.255.255.255";


    public static onIPconnect onIPconnectDelegate;


    private void Awake()
    {
       

        string ipAddrPref = PlayerPrefs.GetString("ipAddr");


        if (string.IsNullOrEmpty(ipAddrPref))
        {
            ipAddress = defaultIPaddr;
        }
        else ipAddress = ipAddrPref; 
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnConnect()
    {
        PlayerPrefs.SetString("ipAddr", ipAddress);
        if (onIPconnectDelegate == null) return;
        onIPconnectDelegate();
    }

    public void setIPaddr(string ipaddr)
    {
        ipAddress = ipaddr;
        Debug.Log($"{GetType()}: setIPaddr(): Setting   ip:{ipAddress}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
