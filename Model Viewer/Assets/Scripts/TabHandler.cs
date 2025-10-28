using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public struct TabInfo
{
    [SerializeField] public Button Button;
    [SerializeField] public GameObject Panel;
}

public class TabHandler : MonoBehaviour
{
    [SerializeField] private List<TabInfo> Tabs;

    private void Start()
    {
        foreach (TabInfo tabInfo in Tabs)
        {
            tabInfo.Button.onClick.AddListener(TabButtonClicked);
            tabInfo.Panel.SetActive(false);
            void TabButtonClicked()
            {
                foreach (TabInfo info in Tabs)
                {
                    info.Panel.SetActive(false);
                }

                tabInfo.Panel.SetActive(true);
            }
        }
    }
}