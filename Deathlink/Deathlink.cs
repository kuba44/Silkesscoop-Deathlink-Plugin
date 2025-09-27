using BepInEx;
using SilklessLib;
using System.Collections;
using UnityEngine;

[BepInPlugin("com.kuba44.Deathlink", "Deathlink for silklesscoop", "1.0.1")]
[BepInDependency("SilklessLib")]
public class Deathlink : BaseUnityPlugin
{
    private bool _canDie = true;
    private float _deathCooldown = 10f;

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

        SilklessAPI.AddHandler<DeathlinkPacket>(OnDeathlinkPacket);
    }

    private void Update()
    {
        if (HeroController.instance == null)
            return;

        if (_canDie && HeroController.instance.playerData.health == 0)
        {
            SendDeathlinkPacket();

            StartCoroutine(DeathCooldown(_deathCooldown));
        }
    }

    public class DeathlinkPacket : SilklessPacket
    {

    }

    private void OnDeathlinkPacket(DeathlinkPacket packet)
    {
        HeroController.instance.DamageSelf(100);
        LogUtil.LogInfo($"Player {packet.ID} has died", true);
    }

    private void SendDeathlinkPacket()
    {
        SilklessAPI.SendPacket(new DeathlinkPacket { });
    }

    IEnumerator DeathCooldown(float cooldown)
    {
        _canDie = false;

        yield return new WaitForSeconds(cooldown);

        _canDie = true;
    }
}