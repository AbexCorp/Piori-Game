using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System.IO;

public class Services : Singleton<Services>
{
    async void Start()
    {
        await UnityServices.InitializeAsync();
        CheckConsent();
    }

    private string _consentPath = "consent.txt";
    private void CheckConsent()
    {
        if (File.Exists(_consentPath))
        {
            ReadConsent();
            return;
        }
        else
            AnalyticsService.Instance.StopDataCollection();
    }
    private void ReadConsent()
    {
        if (!File.Exists(_consentPath))
        {
            using (StreamReader sr = new StreamReader(_consentPath))
            {
                switch ((char)sr.Read())
                {
                    case '1':
                        AnalyticsService.Instance.StartDataCollection();
                        return;

                    case '0':
                    default:
                        AnalyticsService.Instance.StopDataCollection();
                        return;
                }
            }
        }
    }
    public void SetConsent(bool value)
    {
        if (value)
            AnalyticsService.Instance.StartDataCollection();
        else
            AnalyticsService.Instance.StopDataCollection();

        using (StreamWriter sw = new StreamWriter(_consentPath, false))
        {
            sw.WriteLine(value ? "1" : "0");
        }
    }
}
