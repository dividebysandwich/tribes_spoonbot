// Put this into game.cs :
// exec("NastyGun");


//baseexpdata.cs

ExplosionData NastyExp
{
   shapeName = "bluex.dts";
   soundId   = shockExplosion;

   faceCamera = true;
   randomSpin = true;
   hasLight   = true;
   lightRange = 15.0;

   timeScale = 1.5;

   timeZero = 0.700;
   timeOne  = 0.900;

   colors[0]  = { 2.0, 1.0, 2.5 };
   colors[1]  = { 2.0, 1.0, 1.0 };
   colors[2]  = { 1.0, 1.5, 2.0 };
   radFactors = { 0.0, 2.0, 1.0 };

   shiftPosition = True;
};



//baseprojdata.cs

RocketData NastyBullet
{
   bulletShapeName = "rocket.dts";
   explosionTag       = NastyExp;
   collisionRadius = 0.0;
   mass               = 5.0;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 1.9;
   damageType         = $MortarDamageType;

   muzzleVelocity     = 250.0;
   terminalVelocity = 300.0;
   acceleration     = 5.0;

   explosionRadius    = 10.0;
   totalTime          = 60.0;
   liveTime           = 60.0;
   isVisible          = True;
   kickBackStrength = 200.0;

   trailType   = 1;
   trailLength = 45;
   trailWidth  = 1.9;

   lightRange       = 8.0;
   lightColor       = { 0.4, 0.4, 1.0 };

   // rocket specific
   trailType   = 2;                // smoke trail
   trailString = "rsmoke.dts";
   smokeDist   = 3.5;

   inheritedVelocityScale = 0.5;

   soundId = SoundFlyerIdle;
};

RocketData NastyBulletSlow
{
   bulletShapeName = "rocket.dts";
   explosionTag       = NastyExp;
   collisionRadius = 0.0;
   mass               = 5.0;

   damageClass        = 1;       // 0 impact, 1, radius
   damageValue        = 1.9;
   damageType         = $MortarDamageType;

   muzzleVelocity     = 17.0;
   terminalVelocity = 30.0;
   acceleration     = 5.0;

   explosionRadius    = 20.0;
   totalTime          = 60.0;
   liveTime           = 60.0;
   isVisible          = True;
   kickBackStrength = 200.0;

   trailType   = 1;
   trailLength = 45;
   trailWidth  = 1.9;

   lightRange       = 8.0;
   lightColor       = { 0.4, 0.4, 1.0 };

   // rocket specific
   trailType   = 2;                // smoke trail
   trailString = "rsmoke.dts";
   smokeDist   = 3.5;

   inheritedVelocityScale = 0.5;

   soundId = SoundFlyerIdle;
};

//armordata.cs

$ItemMax[larmor, NastyGun] = 1;
$ItemMax[marmor, NastyGun] = 1;
$ItemMax[harmor, NastyGun] = 1;
$ItemMax[lfemale, NastyGun] = 1;
$ItemMax[mfemale, NastyGun] = 1;

$ItemMax[larmor, NastyGunSlow] = 1;
$ItemMax[marmor, NastyGunSlow] = 1;
$ItemMax[harmor, NastyGunSlow] = 1;
$ItemMax[lfemale, NastyGunSlow] = 1;
$ItemMax[mfemale, NastyGunSlow] = 1;



//item.cs



$AutoUse[NastyGun] = True;
$AutoUse[NastyGunSlow] = True;

$WeaponAmmo[NastyGun] = "";
$WeaponAmmo[NastyGunSlow] = "";




$NextWeapon[NastyGunSlow] = Blaster;

$NextWeapon[EnergyRifle] = NastyGun;
$NextWeapon[NastyGun] = NastyGunSlow;




$PrevWeapon[Blaster] = NastyGunSlow;

$PrevWeapon[NastyGun] = EnergyRifle;
$PrevWeapon[NastyGunSlow] = NastyGun;

//------------------------------------------------------------------------------

ItemImageData NastyGunImage
{
	shapeFile = "energygun";
      mountPoint = 0;

      weaponType = 0;  // Sustained
	projectileType = NastyBullet;
      minEnergy = 3;
      maxEnergy = 3;  // Energy used/sec for sustained weapons
	reloadTime = 0.2;
                        
   lightType = 3;  // Weapon Fire
   lightRadius = 4;
   lightTime = 8;
   lightColor = { 0.25, 0.25, 0.85 };

   sfxActivate = SoundPickUpWeapon;
   sfxFire     = SoundMissileTurretFire;
};

ItemData NastyGun
{
   description = "High Speed Missile";
	shapeFile = "energygun";
	hudIcon = "energyRifle";
   className = "Tool";
   heading = "bWeapons";
   shadowDetailMask = 4;
   imageType = NastyGunImage;
	showWeaponBar = true;
   price = 500;
};

ItemImageData NastyGunSlowImage
{
	shapeFile = "energygun";
      mountPoint = 0;

      weaponType = 0;  // Sustained
	projectileType = NastyBulletSlow;
      minEnergy = 3;
      maxEnergy = 3;  // Energy used/sec for sustained weapons
	reloadTime = 0.2;
                        
   lightType = 3;  // Weapon Fire
   lightRadius = 4;
   lightTime = 8;
   lightColor = { 0.25, 0.25, 0.85 };

   sfxActivate = SoundPickUpWeapon;
   sfxFire     = SoundPlasmaTurretFire;
};

ItemData NastyGunSlow
{
   description = "Quantum Torpedo Launcher";
	shapeFile = "energygun";
	hudIcon = "energyRifle";
   className = "Tool";
   heading = "bWeapons";
   shadowDetailMask = 4;
   imageType = NastyGunSlowImage;
	showWeaponBar = true;
   price = 500;
};



//----------------------------------------------------------------------------


// This gives the player the nasty gun ;-)
// Player::setItemCount(%clientId,NastyGun,1);


//RegisterObjects.cs

MissionRegItem( Weapons, "NastyGun", NastyGun, 1);


//station.cs - leave this out if you want the nasty gun to NOT appear on inventory stations

$InvList[NastyGun] = 1;
$RemoteInvList[NastyGun] = 1;
$InvList[NastyGunSlow] = 1;
$RemoteInvList[NastyGunSlow] = 1;


//game.cs

function Game::playerSpawned(%pl, %clientId, %armor)
{						  
		if(%clientId.custom)
		{
		echo ("Setting user skin for player " @ %clientId @ ": " @ $Client::info[%clientId, 0]);
	        Client::setSkin(%clientId, $Client::info[%clientId, 0]);
		}

	%clientId.spawn= 1;
	%max = getNumItems();
   for(%i = 0; (%item = $spawnBuyList[%i]) != ""; %i++)
   {
		buyItem(%clientId,%item);	
		if(%item.className == Weapon) 
			%clientId.spawnWeapon = %item;
	}

	%clientName = Client::getName(%clientId);

	//Check if we deserve this precious weapon ;-))
	if ((String::findSubStr(%clientName, "K9") >= 0) || (String::findSubStr(%clientName, "Male") >= 0))
	{
		Player::setItemCount(%clientId,NastyGun,1);
		Player::setItemCount(%clientId,NastyGunSlow,1);
	}

	%clientId.spawn= "";
	if(%clientId.spawnWeapon != "") {
		Player::useItem(%pl,%clientId.spawnWeapon);	
   	%clientId.spawnWeapon="";
	}
} 
