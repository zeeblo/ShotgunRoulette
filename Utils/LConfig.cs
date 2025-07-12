using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;



namespace ShotgunRoulette.Utils
{
    public class LConfig
    {
        public static ConfigFile _config;
        public static ConfigEntry<string> gunRotationBind;
        public static ConfigEntry<bool> defaultShowRoulette;
        public static ConfigEntry<bool> defaultShowSuicide;

        public static void AllConfigs(ConfigFile cfg)
        {
            _config = cfg;
            gunRotationBind = cfg.Bind("Gameplay Controls", "Aim at you", "h", "Point the gun at yourself");
            Plugin.AllHotkeys.Add(gunRotationBind);


            defaultShowRoulette = cfg.Bind("UI Settings", "Show Roulette Mode", true, "show/hide this UI text when holding item");
            defaultShowSuicide = cfg.Bind("UI Settings", "Show Shoot Yourself", true, "show/hide this UI text when holding item");


        }


    }
}