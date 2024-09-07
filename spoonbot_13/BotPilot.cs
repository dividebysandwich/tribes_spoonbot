$BotPilot::NumWaypointsA = 0;
$BotPilot::NumWaypointsB = 0;

if ($Pilot::WaypointEditor)
{
 editActionMap("actionMap.sae");
 bindCommand(keyboard0, make, "f5", TO, "BotPilot::AddWaypoint(2049);");
 bindCommand(keyboard0, break, "f5", TO, "");
 messageAll(1, "WARNING: Waypoint Editor active!");
}


function BotPilot::Init_Waypoints()
{
 deleteVariables("$BotPilot::*");
 $BotPilot::NumWaypointsA = 0;
 $BotPilot::NumWaypointsB = 0;
 %Filename = "BP_" @ $missionName @".cs";
 if(isFile("config\\" @ %Filename))
 {
   exec(%filename);
   if ($Pilot::DebugMode)
     echo("BOTPILOT> Successfully loaded waypoint data with " @ $BotPilot::NumWaypointsA+$BotPilot::NumWaypointsB @ " points.");
 }
}


function BotPilot::AddWaypoint(%client)
{
 %team = Client::getTeam(%client);

 if (%team == 0)
 {
  $BotPilot::RouteA[$BotPilot::NumWaypointsA] = GameBase::getPosition(Client::getControlObject(%client));
  $BotPilot::RotA[$BotPilot::NumWaypointsA] = GameBase::getRotation(Client::getControlObject(%client));
  $BotPilot::NumWaypointsA++;
  $BotPilot::RouteA[$BotPilot::NumWaypointsA] = "0 0 0";
  $BotPilot::RotA[$BotPilot::NumWaypointsA] = "0 0 0";
  $BotPilot::RouteA[$BotPilot::NumWaypointsA+1] = "0 0 0";
  $BotPilot::RotA[$BotPilot::NumWaypointsA+1] = "0 0 0";
  messageAll(1, "Placed Waypoint for Team 0");
 }
 if (%team == 1)
 {
  $BotPilot::RouteB[$BotPilot::NumWaypointsB] = GameBase::getPosition(Client::getControlObject(%client));
  $BotPilot::RotB[$BotPilot::NumWaypointsB] = GameBase::getRotation(Client::getControlObject(%client));
  $BotPilot::NumWaypointsB++;
  $BotPilot::RouteB[$BotPilot::NumWaypointsB] = "0 0 0";
  $BotPilot::RotB[$BotPilot::NumWaypointsB] = "0 0 0";
  $BotPilot::RouteB[$BotPilot::NumWaypointsB+1] = "0 0 0";
  $BotPilot::RotB[$BotPilot::NumWaypointsB+1] = "0 0 0";
  messageAll(1, "Placed Waypoint for Team 1");
 }

 %Filename = "config\\BP_" @ $missionName @ ".cs";
 export("$BotPilot::*", %Filename, false);


}




function bmount(%this)
{
 if ($Pilot::DebugMode)
  echo("BOTPILOT> " @ Client::getOwnedObject(2050) @ " Trying to mount vehicle");

// %newbugs = GameBase::GetPosition(%this);
// $BotThink::ForcedOfftrack[2050] = True;
// BotTree::Getmetopos(2050, %newbugs, false);
// $BotThink::Definitive_Attackpoint[2050] = %this;

 BotPilot::Mount(%this, 2050);

}



function bghost(%this)
{
 $Pilot::NextWaypoint[%this] = 0;
 BotPilot::Fly(%this, 0);
}




//Usage: BotPilot <Vehicle ID> <Pilot Client ID> <Team>
// The team argument determines which path the vehicle takes
//
function BotPilot::Fly(%this, %client, %team)
{

 %currentpos = GameBase::GetPosition(%this);

 if ((%currentpos == "0 0 0") || (%currentpos == "0") || (%currentpos == -1) || (%currentpos == 0) || (%currentpos == ""))
   return;

 %currentrot = GameBase::GetRotation(%this);

// echo(%currentpos);

//Switch to next waypoint if we reached the current one...

 if (%team == 0)
  if (Vector::getDistance(%currentpos, $BotPilot::RouteA[$Pilot::NextWaypoint[%this]]) < 5)
  {
   if ($BotPilot::RouteA[$Pilot::NextWaypoint[%this]+1] == "0 0 0") //Quit if we reached our target
   {
     schedule("$Pilot::DontCheck["@%client@"] = 0;", 45);
     %pl = Client::getOwnedObject(%client);
     if(%pl != -1) 
     {
	   schedule("Player::setMountObject("@%pl@", -1, 0);", 5);
   	   schedule("Client::setControlObject("@%client@", "@%pl@");", 5);
	   schedule("BotFuncs::BotsHopOff("@%client@");", 5);
     }
     schedule("$BotThink::ForcedOfftrack["@%client@"] = true;", 5);
     schedule("BotTree::GetMeToPos("@%client@", $CurrentTargetPos["@%client@"], true);", 5);
     schedule("GameBase::startFadeOut("@%this@");", 5);
     schedule("deleteObject(" @ %this @ ");", 7.5);
     schedule(%this @ ".fading = \"\";",7.5);
     return;
   }
   else
     $Pilot::NextWaypoint[%this]++;
  }


 if (%team == 1)
  if (Vector::getDistance(%currentpos, $BotPilot::RouteB[$Pilot::NextWaypoint[%this]]) < 5)
  {
   if ($BotPilot::RouteB[$Pilot::NextWaypoint[%this]+1] == "0 0 0") //Quit if we reached our target
   {
     schedule("$Pilot::DontCheck["@%client@"] = 0;", 45);
     %pl = Client::getOwnedObject(%client);
     if(%pl != -1) 
     {
	   schedule("Player::setMountObject("@%pl@", -1, 0);", 5);
   	   schedule("Client::setControlObject("@%client@", "@%pl@");", 5);
	   schedule("BotFuncs::BotsHopOff("@%client@");", 5);
     }
     schedule("$BotThink::ForcedOfftrack["@%client@"] = true;", 5);
     schedule("BotTree::GetMeToPos("@%client@", $CurrentTargetPos["@%client@"], true);", 5);
     schedule("GameBase::startFadeOut("@%this@");", 5);
     schedule("deleteObject(" @ %this @ ");", 7.5);
     schedule(%this @ ".fading = \"\";",7.5);
     return;
   }
   else
     $Pilot::NextWaypoint[%this]++;
  }

 %cp1 = getWord(%currentpos, 0);
 %cp2 = getWord(%currentpos, 1);
 %cp3 = getWord(%currentpos, 2);

 %cr1 = getWord(%currentrot, 0);
 %cr2 = getWord(%currentrot, 1);
 %cr3 = getWord(%currentrot, 2);

//get the next waypoint data

 if (%team == 0)
 {
  %np1 = getWord($BotPilot::RouteA[$Pilot::NextWaypoint[%this]], 0);
  %np2 = getWord($BotPilot::RouteA[$Pilot::NextWaypoint[%this]], 1);
  %np3 = getWord($BotPilot::RouteA[$Pilot::NextWaypoint[%this]], 2);
  %nr1 = getWord($BotPilot::RotA[$Pilot::NextWaypoint[%this]], 0);
  %nr2 = getWord($BotPilot::RotA[$Pilot::NextWaypoint[%this]], 1);
  %nr3 = getWord($BotPilot::RotA[$Pilot::NextWaypoint[%this]], 2);
//  echo("Next: " @ $BotPilot::RouteA[$Pilot::NextWaypoint[%this]]);
 }
 if (%team == 1)
 {
  %np1 = getWord($BotPilot::RouteB[$Pilot::NextWaypoint[%this]], 0);
  %np2 = getWord($BotPilot::RouteB[$Pilot::NextWaypoint[%this]], 1);
  %np3 = getWord($BotPilot::RouteB[$Pilot::NextWaypoint[%this]], 2);
  %nr1 = getWord($BotPilot::RotB[$Pilot::NextWaypoint[%this]], 0);
  %nr2 = getWord($BotPilot::RotB[$Pilot::NextWaypoint[%this]], 1);
  %nr3 = getWord($BotPilot::RotB[$Pilot::NextWaypoint[%this]], 2);
//  echo("Next: " @ $BotPilot::RouteB[$Pilot::NextWaypoint[%this]]);
 }

//Interpolate Movement (very crude)

 if (%cp1 < %np1)
   %cp1 = %cp1 + 3;
 else if (%cp1 > %np1 + 3)
   %cp1 = %cp1 - 3;
 else
   %cp1 = %np1;

 if (%cp2 < %np2)
   %cp2 = %cp2 + 3;
 else if (%cp2 > %np2 + 3)
   %cp2 = %cp2 - 3;
 else
   %cp2 = %np2;

 if (%cp3 < %np3)
   %cp3 = %cp3 + 3;
 else if (%cp3 > %np3 + 3)
   %cp3 = %cp3 - 3;
 else
   %cp3 = %np3;

//Interpolate rotation

 if (%cr1 < %nr1)
   %cr1 = %cr1 + 0.1;
 else if (%cr1 > %nr1 + 0.1)
   %cr1 = %cr1 - 0.1;
 else
   %cr1 = %nr1;

 if (%cr2 < %nr2)
   %cr2 = %cr2 + 0.1;
 else if (%cr2 > %nr2 + 0.1)
   %cr2 = %cr2 - 0.1;
 else
   %cr2 = %nr2;

 if (%cr3 < %nr3)
   %cr3 = %cr3 + 0.1;
 else if (%cr3 > %nr3 + 0.1)
   %cr3 = %cr3 - 0.1;
 else
   %cr3 = %nr3;

//Now set position and rotation

 %nextpos = %cp1 @ " " @ %cp2 @ " " @ %cp3;
 %nextrot = %cr1 @ " " @ %cr2 @ " " @ %cr3;

 GameBase::setPosition(%this, %nextpos);
 GameBase::setRotation(%this, %nextrot);

//Give a little speed so the movement is less jerky

 %acc = Vector::getFromRot(%nextrot, 100, 0);
 Item::setVelocity(%this, %acc);


//Reschedule regularely

 schedule("BotPilot::Fly("@%this@", "@%client@", "@%team@");",0.1);

}





function BotPilot::Mount(%this, %client)
{
	%object = Client::getOwnedObject(%client);
	%armor = Player::getArmor(%object);
	if ((%armor == "larmor" || %armor == "lfemale") && Vehicle::canMount (%this, %object))
	{

		%weapon = Player::getMountedItem(%object,$WeaponSlot);
		if(%weapon != -1) {
			%object.lastWeapon = %weapon;
			Player::unMountItem(%object,$WeaponSlot);
			}
		Player::setMountObject(%object, %this, 1);
		Client::setControlObject(%client, %this);
		playSound (GameBase::getDataName(%this).mountSound, GameBase::getPosition(%this));
		%object.driver= 1;
		%object.vehicle = %this;
		%this.clLastMount = %client;
		$Spoonbot::IsPilot[%client] = True;

		//If it's a HPC, issue an "APC ready" message and wait for 20 seconds
		//else fly away immediately

		if(GameBase::getDataName(%this) == Scout)
		{
			$Pilot::NextWaypoint[%this] = 0;
			BotPilot::Fly(%this, %client, Client::getTeam(%client));
		}
		else
		{
			BotFuncs::BotsHopOn(%client);
			schedule ("$Pilot::NextWaypoint["@%this@"] = 0;", 10);
			schedule ("BotPilot::Fly("@%this@", "@%client@", Client::getTeam("@%client@"));", 10);

		}

	}
	else if(GameBase::getDataName(%this) != Scout)
	{
	 	%mountSlot= Vehicle::findEmptySeat(%this,%client);
		if(%mountSlot)
		{
			%object.vehicleSlot = %mountSlot;
			%object.vehicle = %this;
			Player::setMountObject(%object, %this, %mountSlot);
			playSound (GameBase::getDataName(%this).mountSound, GameBase::getPosition(%this));
		}
	}

}


function BotPilot::Buy(%client, %item, %pad)
{
	%player = Client::getOwnedObject(%client);
	%obj = %pad;
	if(GameBase::isPowered(%obj) && GameBase::getDamageState(%obj) == "Enabled") {
		%markerPos = GameBase::getPosition(%obj);
		%set = newObject("set",SimSet);
		%mask = $VehicleObjectType | $SimPlayerObjectType | $ItemObjectType;
		%objInWay = containerBoxFillSet(%set,%mask,%markerPos,6,5,14,1);
		%station = %player.Station;
		if(%objInWay == 1) 
		{
			%object = Group::getObject(%set, 0);	
			%sName = GameBase::getDataName(%object);
			if(%sName.className == Vehicle) 
			{
				if(GameBase::getControlClient(%object) == -1) 
				{
					if(%station.fadeOut == "") 
					{
						if(%item != $VehicleToItem[%sname]) 
						{
							%object.fading = 1;
							%station.fadeOut=1;
							teamEnergyBuySell(%player,$VehicleToItem[%sName].price);
							$TeamItemCount[Client::getTeam(%client) @ ($VehicleToItem[%sName])]--;
							GameBase::startFadeOut(%object);
							schedule("deleteObject(" @ %object @ ");",2.5,%object);
							schedule(%object @ ".fading = \"\";",2.5,%object);
							schedule(%station @ ".fadeOut = \"\";",2.5,%station);
							%objInWay--;
						}
						else
							return 2;
					}
					else 
					{
						Client::SendMessage(%client,0,"ERROR - Vehicle creation pad busy"); 
						return 0;
					}
				}
				else 
				{
					Client::SendMessage(%client,0,"ERROR - Vehicle in creation area is mounted");
					return 0;
				}
			} 
		}
		if(!%objInWay) 
		{
			if (checkResources(%player,%item,1)) 
			{
		 		%vehicle = newObject("",flier,$DataBlockName[%item],true);
				Gamebase::setMapName(%vehicle,%item.description);
				%vehicle.clLastMount = %client;
				addToSet("MissionCleanup", %vehicle);
				%vehicle.fading = 1;
				GameBase::setTeam(%vehicle,Client::getTeam(%client));
				if(%object.fading) 
				{
						//after 2 seconds mount the vehicle
					$Pilot::DontCheck[%client] = 1;
					schedule("BotPilot::Mount(" @ %vehicle @ ", " @ %client @ ");", 4.5);
					schedule("GameBase::startFadeIn(" @ %vehicle @ ");",2.5,%vehicle);
					schedule("GameBase::setPosition(" @ %vehicle @ ",\"" @ %markerPos @ "\");",2.5,%vehicle);
					schedule("GameBase::setRotation(" @ %vehicle @ ",\"" @ GameBase::getRotation(%obj) @ "\");",2.5,%vehicle);
					schedule(%vehicle @ ".fading = \"\"; VehiclePad::checkSeq(" @ %obj @ "," @ %player.Station @ ");",5,%vehicle);
					%obj.busy = getSimTime() + 5;
				}
				else 
				{
						//after 2 seconds mount the vehicle
					$Pilot::DontCheck[%client] = 1;
					schedule("BotPilot::Mount(" @ %vehicle @ ", " @ %client @ ");", 2);
					GameBase::startFadeIn(%vehicle);
					GameBase::setPosition(%vehicle,%markerPos);
					GameBase::setRotation(%vehicle,GameBase::getRotation(%obj));
				 	schedule(%vehicle @ ".fading = \"\"; VehiclePad::checkSeq(" @ %obj @ "," @ %player.Station @ ");",3,%vehicle);
					%obj.busy = getSimTime() + 3;
				}
				deleteObject(%set);
				$TeamItemCount[Client::getTeam(%client) @ %item]++;
				return 1;
			}
		}
		else
			Client::SendMessage(%client,0,"ERROR - Object in vehicle creation area");
		deleteObject(%set);
	}	
	else
		Client::SendMessage(%client,0,"ERROR - Vehicle Pad Disabled");

	return 0;
}





//Check if a bot should buy and mount a vehicle
function BotPilot::Check(%client)
{

 if ($Pilot::DontCheck[%client] != 0)
   return;

 %player = Client::getOwnedObject(%client);

 %armor = Player::getArmor(%player);
 if (%armor != "larmor" && %armor != "lfemale")
    return;

 %aiTeam = Client::getTeam(%client);
 if(%aiTeam == 0)
  %EnemyTeam = 1;
 else
  %EnemyTeam = 0;
 %AttackDefend=%EnemyTeam;
 %BotPosition = GameBase::getPosition(%client);

//Is there a vehicle pad within 30 meters?

 %pad = BotFuncs::GetVehId(%aiTeam, %BotPosition);
 if (%pad == -1)
  return;


//Can I see the vehicle pad?

//echo("Distance=" @Vector::getDistance(%BotPosition, GameBase::getPosition(%pad)) );

 if (Vector::getDistance(%BotPosition, GameBase::getPosition(%pad)) > 30)
   return;


 if (!BotFuncs::CheckForItemLOS(%client, %pad))
   return;

// echo("Pad " @ %pad @ " is within range");

//If yes, buy a vehicle

%temp = floor(getRandom() * 3);
if (%temp == 0)
	%item = ScoutVehicle;
if (%temp == 1)
	%item = LAPCVehicle;
if (%temp == 2)
	%item = HAPCVehicle;
if (%temp == 3)
	%item = ScoutVehicle;

 echo("Attempt to buy " @ %item);

BotPilot::Buy(%client, %item, %pad);


}

