using BepInEx.Configuration;

public static class ModConfig
{
    public static bool RemoveInvincibility = false;

    public static void Bind(ConfigFile config)
    {
        RemoveInvincibility = config.Bind("General", "Remove Invincibility", false, "If true, players will not be able to avoid death by using invincibility. Not tested, could break a few things.").Value;
    }
}