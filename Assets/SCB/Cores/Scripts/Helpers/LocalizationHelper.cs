using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace SCB.Cores
{
    public static class LocalizationHelper
    {
        public static void ChangeNextLocale()
        {
            int IndexLocale = 0;

            LocalizationSettings settings = new LocalizationSettings();
            var AvailableLocales = settings.GetAvailableLocales().Locales;
            for (int i = 0; i < AvailableLocales.Count; i++)
            {
                var locale = settings.GetSelectedLocale();
                if (locale.Identifier == AvailableLocales[i].Identifier)
                {
                    IndexLocale = i;
                    break;
                }
            }
            IndexLocale += 1;
            if (IndexLocale >= AvailableLocales.Count)
                IndexLocale = 0;

            LocalizationSettings.SelectedLocale = AvailableLocales[IndexLocale];
        }
    }
}
