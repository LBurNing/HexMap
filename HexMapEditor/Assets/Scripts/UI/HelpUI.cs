using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour
{
    private Button btnClose;

    private void Awake()
    {
        btnClose = transform.Find("Close").GetComponent<Button>();
    }
    void Start()
    {
        btnClose.onClick.AddListener(()=> 
        {
            ViewManager.instance.HideUI("HelpUI");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
