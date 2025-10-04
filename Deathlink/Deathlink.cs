using BepInEx;
using SilklessLib;
using System.Collections;
using UnityEngine;

[BepInPlugin("kuba44.Deathlink", "Deathlink for silklesscoop", "1.1.1")]
[BepInDependency("SilklessLib")]
public class Deathlink : BaseUnityPlugin
{
    private bool _canDie = true;
    private float _deathCooldown = 5f;
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
        HeroController hornet = HeroController.instance;
        if (hornet == null)
            return;
        PlayerData playerData = hornet.playerData;

        if (!_canDie)
            return;

        if (ModConfig.EnableKillKey && Input.GetKeyDown(ModConfig.KillKey) && !playerData.isInventoryOpen && !playerData.isInvincible)
        {
            playerData.TakeHealth(100, false, true);
            hornet.DamageSelf(1);
        }

        if (playerData.health == 0)
        {
            LogUtil.LogInfo("You have died", true);

            SendDeathlinkPacket();

            StartCoroutine(DeathCooldown(_deathCooldown));
        }

        if (_cheatedDeath && !playerData.isInvincible && !playerData.isInventoryOpen && !playerData.travelling)
        {
            LogUtil.LogInfo("You have cheated death, now you die", true);

            KillPlayer();

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

        PlayerData playerData = HeroController.instance.playerData;

        LogUtil.LogInfo($"Player {packet.playerName} has died", true);

        if (playerData.isInvincible)
        {
            if (ModConfig.RemoveInvincibility)
            {
                playerData.isInvincible = false;
            }
            else
            {
                _cheatedDeath = true;
                return;
            }
        }

        if(playerData.isInventoryOpen)
        {
            _cheatedDeath = true;
            return;
        }

        if (playerData.travelling)
        {
            _cheatedDeath = true;
            return;
        }

        KillPlayer();
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

    private void KillPlayer()
    {
        HeroController.instance.playerData.TakeHealth(100, false, true);
        HeroController.instance.DamageSelf(1);

        StartCoroutine(DeathCooldown(_deathCooldown));
    }
}