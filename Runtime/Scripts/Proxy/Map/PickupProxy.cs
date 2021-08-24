using System;
using UnityEngine;

public enum PickupType
{
	Custom = -2,
	Null = -1,
	SniperRifle = 0,
	RedDot = 1,
	Shield = 2,
	Binoculars = 3,
	Banana = 4,
	SMG = 5,
	Pistol = 6,
	Grenade = 7,
	SmokeGrenade = 8,
	CSGrenade = 9,
	LaserSensor = 10,
	RemoteCharge = 11,
	CardboardDecoy = 12,
	SMGAmmox30 = 13,
	PistolAmmox15 = 14,
	SniperAmmox5 = 15,
	Shotgun = 16,
	ShotgunAmmox6 = 17
}

public class PickupProxy : MonoBehaviour
{
	public ItemProxy pickupItem;
	public PickupType pickupType;

	public int addedAmmo = -1; //Ammo added to the weapons ammo stash
	public int loadedAmmo = -1; //Ammo loaded in the weapon if you pick it up, use only if you want to replace the current magazine in the weapon
	public string pickupMessage = ""; //Custom message for the pick up if you change things about it, like the ammo amount, to avoid default messages
	public float respawnTime = -1; //Pick up will respawn after this amount of time
	public Activator activatorToActivate; //Activate an activator when you pick up this pick up
}