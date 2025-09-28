using BepInEx;
using SilklessLib;
using System.Collections;
using UnityEngine;

[BepInPlugin("kuba44.Deathlink", "Deathlink for silklesscoop", "1.1.0")]
[BepInDependency("SilklessLib")]
public class Deathlink : BaseUnityPlugin
{
    private bool _canDie = true;
    private float _deathCooldown = 10f;
    private bool _cheatedDeath = false;

    private void Awake()
    {
        if (!SilklessAPI.Init())
        {
            Logger.LogError("Silkless plugin not found! Disabling Deathlink plugin.");
            Destroy(this);
        }
        else
        {
            Logger.LogInfo("Deathlink plugin is loaded!");
        }

        ModConfig.Bind(Config);
        SilklessAPI.AddHandler<DeathlinkPacket>(OnDeathlinkPacket);
    }

    private void Update()
    {
        if (HeroController.instance == null)
            return;

        if (!_canDie)
            return;

        if (HeroController.instance.playerData.health == 0)
        {
            LogUtil.LogInfo("You have died", true);

            SendDeathlinkPacket();

            StartCoroutine(DeathCooldown(_deathCooldown));
        }

        if (_cheatedDeath && !HeroController.instance.playerData.isInvincible)
        {
            LogUtil.LogInfo("You have cheated death", true);

            HeroController.instance.DamageSelf(100);
            StartCoroutine(DeathCooldown(_deathCooldown));

            _cheatedDeath = false;
        }
    }


    public class DeathlinkPacket : SilklessPacket
    {
        public string playerName;
    }

    private void OnDeathlinkPacket(DeathlinkPacket packet)
    {
        if (!_canDie)
            return;

        LogUtil.LogInfo($"Player {packet.playerName} has died", true);

        if (HeroController.instance.playerData.isInvincible)
        {
            if (ModConfig.RemoveInvincibility)
            {
                HeroController.instance.playerData.isInvincible = false;
            }
            else
            {
                _cheatedDeath = true;
                return;
            }
        }

        HeroController.instance.DamageSelf(100);
        StartCoroutine(DeathCooldown(_deathCooldown));
    }

    private void SendDeathlinkPacket()
    {
        SilklessAPI.SendPacket(new DeathlinkPacket
        {
            playerName = SilklessAPI.GetUsername(),
        });
    }

    IEnumerator DeathCooldown(float cooldown)
    {
        _canDie = false;

        yield return new WaitForSeconds(cooldown);

        _canDie = true;
    }
}