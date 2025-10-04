using BepInEx.Configuration;
using UnityEngine;

public static class ModConfig
{
    public static bool RemoveInvincibility;
    public static bool EnableKillKey;
    public static KeyCode KillKey;

    public static void Bind(ConfigFile config)
    {
        RemoveInvincibility = config.Bind("1. General", "Remove Invincibility", false, "If true, players will not be able to avoid death by using invincibility. Not tested, could break a few things.").Value;
        EnableKillKey = config.Bind("2. Debug", "Enable Kill Key", false, "If true, enables the option below").Value;
        KillKey = config.Bind("2. Debug", "Kill Key", KeyCode.K, "Press the key to kill your hornet. Used to debug the mod. Left in if you really want to troll your friends.").Value;
    }
}