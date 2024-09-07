dbecho(1,"Precaching Bot AI Functions");

$SPOONBOT::Version = "1.3";

//exec("NastyGun"); //Enable these if you want really nasty weapons...

SoundData SoundBotRepairItem
{
   wavFileName = "repair.wav";
   profile = Profile3dNear;
};


exec("BotGear");
exec("BotThink");
exec("BotTypes");
exec("BotFuncs");
exec("BotSpawn");
exec("BotTree");
exec("BotHUD");
exec("BotMove");
exec("JetToPos");
exec("BotPilot");
exec("admin");		// Why must I run this ? Cant suss this out? Wicked69.
exec("vehicle");	// That's a bug in TRIBES itself. But running too many of the standard scripts here results in mod incompatibility.



//
// AI support functions.
//

//
// This function creates an AI player using the supplied group of markers 
//    for locations.  The first marker in the group gives the starting location 
//    of the the AI, and the remaining markers specify the path to follow.  
//
// Example call:  
// 
//    createAI( guardNumberOne, "MissionGroup\\Teams\\team0\\guardPath", larmor );
//

//globals
//--------
// path type
// 0 = circular
// 1 = oneWay
// 2 = twoWay
$AI::defaultPathType = 1; //run oneWay paths. Changed from TwoWayPath by Werewolf

//armor types
//light = larmor
//medium = marmor
//heavy = harmor
$AI::defaultArmorType = "larmor";



//---------------------------------
//createAI()
//---------------------------------
//modified by Werewolf
function createAI( %aiName, %markerGroup, %armorType, %name )
{
   %group = nameToID( %markerGroup );
   %voice = "male2";
   if( %group == -1 || Group::objectCount(%group) == 0 )
   {
      dbecho(1, %aiName @ "Couldn't create AI: " @ %markerGroup @ " empty or not found." );
      return -1;
   }
   else
   {
      %spawnMarker = Group::getObject(%group, 0);

//    %spawnMarker = AI::pickRandomSpawn(%team); //Much more convenient

      %spawnPos = GameBase::getPosition(%spawnMarker);
      %spawnRot = GameBase::getRotation(%spawnMarker);

      if(String::findSubStr(%aiName, "Female") >= 0)        //All the IFs are from Werewolf
      {
		  $AI::defaultArmorType = "lfemale";
		  %voice = "female2";
      }
      else
      {
      $AI::defaultArmorType = "larmor";
      %voice = "male2";
      }

    if((String::findSubStr(%aiName, "Guard") >= 0) || (String::findSubStr(%aiName, "Mortar") >= 0))
    {
    if(String::findSubStr(%aiName, "Female") >= 0)
      {
      $AI::defaultArmorType = "harmor";
      %voice = "female5";
      }
      else
      {
      $AI::defaultArmorType = "harmor";
      %voice = "male5";
      }
    }


    if(String::findSubStr(%aiName, "Demo") >= 0)
    {
    if(String::findSubStr(%aiName, "Female") >= 0)
      {
      $AI::defaultArmorType = "mfemale";
      %voice = "female4";
      }
      else
      {
      $AI::defaultArmorType = "marmor";
      %voice = "female2";
      }
    }


      if( AI::spawn( %aiName, $AI::defaultArmorType, %spawnPos, %spawnRot, %aiName, %voice ) != "false" )
      {
	$Spoonbot::NumBots = $Spoonbot::NumBots + 1;
         // The order number is used for sorting waypoints, and other directives.
         // Set to two so it won't fuck up my precious chasing code :-P
  BotFuncs::InitVars( %aiId );      // Wicked69

  Client::setSkin(%aiId, $Server::teamSkin[Client::getTeam(%aiId)]);  // Werewolf

if (BotTypes::IsMedic(%newName))    //As of yet only one Medic can work in the Object Repair Task Queue. (This means repairing Turrets, etc)
{
if (%teamnum == 0)
 $Spoonbot::Team0Medic = %aiId;
if (%teamnum == 1)
 $Spoonbot::Team1Medic = %aiId;
}


  schedule("BotThink::Think(" @ %aiId @ ", True);", 3);      // Wicked69


         %orderNumber = 2;
         
         for(%i = 1; %i < Group::objectCount(%group); %i = %i + 1)
         {
             
            %spawnMarker = Group::getObject(%group, %i);
            %spawnPos = GameBase::getPosition(%spawnMarker);

            
//            AI::DirectiveWaypoint( %aiName, %spawnPos, %orderNumber );
            
//            %orderNumber++;
         }

      }
      else{
         dbecho( 1, "Failure spawning: " @ %aiName );
      }
   }
}

//-----------------------------------
// AI::initDrones()
//-----------------------------------
function AI::initDrones(%team, %numAi)
{
	dbecho(1, "spawning team " @ %team @ " ai...");
   for(%guard = 0; %guard < %numAi; %guard++)
   {
      //check for internal data
      %tempSet = 	nameToID("MissionGroup\\Teams\\team" @ %team @ "\\AI");
      %tempItem = Group::getObject(%tempSet, %guard);
      %aiName = Object::getName(%tempItem);
      
      %set = nameToID("MissionGroup\\Teams\\team" @ %team @ "\\AI\\" @ %aiName);
      %numPts = Group::objectCount(%set);
      
      if(%numPts > 0)
      {



         createAI(%aiName, %set, $AI::defaultArmorType, %aiName);

         %aiId = ai::GetId( %aiName );
         GameBase::setTeam(%aiId, %team);
         AI::setVar( %aiName,  iq,  60 );
         AI::setVar( %aiName,  attackMode, 1);
         AI::setVar( %aiName,  pathType, $AI::defaultPathType);
//      	 schedule("AI::setWeapons(" @ %aiName @ ");", 1);
	 %aiId = ai::GetId(%aiName);
      	 schedule("AI::setWeapons(" @ %aiId @ ");", 1);
      }
      else
         dbecho(1, "no info to spawn ai...");
   }
}


//------------------------------------------------------------------
//functions to test and move AI players.
//
//------------------------------------------------------------------

//
//This function will spawn an AI player about 5 units away from the 
//player that is passed to the function(%commandIssuer).
//
//
$numAI = 0;
function AI::helper(%aiName, %armorType, %commandIssuer)
{
   %spawnMarker = GameBase::getPosition(%commandIssuer);
   %xPos = getWord(%spawnMarker, 0) + floor(getRandom() * 15);
   %yPos = getword(%spawnMarker, 1) + floor(getRandom() * 10);
   %zPos = getWord(%spawnMarker, 2) + 5;
   %rPos = GameBase::getRotation(%commandIssuer);
   
   dbecho(2, "Spawning AI helper at position " @ %xPos @ " " @ %yPos @ " " @ %zPos);
   dbecho(2, "Current Issuer rotation: " @ %rPos);
      
   %aiSpawnPos = %xPos @ "  " @ %yPos @ "  " @ %zPos;
   %newName = %aiName @ $numAI;
   $numAI++;
   Ai::spawn(%newName, %armorType, %aiSpawnPos, %rPos,  %newName, %voice );
   $Spoonbot::NumBots = $Spoonbot::NumBots + 1;
   return ( %newName );
}

//
//This function will move an AI player to the position of an object
//that the players LOS is hitting(terrain included). Must be `	within 50 units.
//
//
function AI::moveToLOS(%aiName, %commandIssuer) 
{
   %issuerRot = GameBase::getRotation(%commandIssuer);
   %playerObj = Client::getOwnedObject(%commandIssuer);
   %playerPos = GameBase::getPosition(%commandIssuer);
      
   //check within max dist
   if(GameBase::getLOSInfo(%playerObj, 100, %issuerRot))
   { 
      %newIssuedVec = $LOS::position;
	  %distance = Vector::getDistance(%playerPos, %newIssuedVec);
	  dbecho(2, "Command accepted, AI player(s) moving....");
	  dbecho(2, "distance to LOS: " @ %distance);
//	  AI::DirectiveWaypoint( %aiName, %newIssuedVec, 2, 2 );
   }
   else
      dbecho(2, "Distance to far.");
      
   dbecho(2, "LOS point: " @ $LOS::position);
}

//This function will move an AI player to a position directly in front of
//the player passed, at a distance that is specified.
function  AI::moveAhead(%aiName, %commandIssuer, %distance) 
{
   
   %issuerRot = GameBase::getRotation(%commandIssuer);
   %commPos  = GameBase::getPosition(%commandIssuer);
//   dbecho(2, "Commanders Position: " @ %commPos);
   
   //get commanders x and y positions
   %comm_x = getWord(%commPos, 0);
   %comm_y = getWord(%commPos, 1);
   
   //get offset x and y positions
   %offSetPos = Vector::getFromRot(%issuerRot, %distance);
   %off_x = getWord(%offSetPos, 0);
   %off_y = getWord(%offSetPos, 1);
   
   //calc new position
   %new_x = %comm_x + %off_x;
   %new_y = %comm_y + %off_y;
   %newPos = %new_x  @ " " @ %new_y @ " 0";
  
   //move AI player
//   dbecho(2, "AI moving to " @ %newPos);
//   AI::DirectiveWaypoint(%aiName, %newPos, 2, 2);
}  

//
// OK, this is the complete command callback - issued for any command sent
//    to an AI. 
//
function AI::onCommand ( %name, %commander, %command, %waypoint, %targetId, %cmdText, 
         %cmdStatus, %cmdSequence )
{
   %aiId = BotFuncs::GetId( %name );
if (%aiId==0)
	return;
	
   %T = GameBase::getTeam( %aiId );
   %groupId = nameToID("MissionGroup\\Teams\\team" @ %T @ "\\AI\\" @ %name ); 
  	%nodeCount = Group::objectCount( %groupId );
//   dbecho(2, "checking drone information...." @ " number of nodes: " @ %nodeCount);
//   dbecho(2, "AI id: " @ %aiId @ " groupId: " @ %groupId);
   
 	   if( %command == 1 )  //Attack
	   {
	      // must convert waypoint location into world location.  waypoint location
	      //    is given in range [0-1023, 0-1023].  
	      %worldLoc = WaypointToWorld ( %waypoint );

//	      AI::DirectiveRemove( %name, 2 );   //Crude way to clear all directives. Needs to be done because else the bots won't
                                                 //respond to any new orders!

              Vehicle::passengerJump(0,%aiId,0);  //Crude way to make passengers hop off vehicles :-P
//              AI::Jump(%aiId);                    //Just jumps. Much more convenient


              %BotRot = GameBase::getRotation(%aiId);
              if(GameBase::getLOSInfo(Client::getOwnedObject(%aiId), 150, %BotRot))    //Test if AI is within firing range
                {
//	          AI::DirectiveTargetPoint( %name, %worldLoc, 2);  //Fucks up AI::DirectiveList
                }
                else
                {
//	          AI::DirectiveWaypoint( %name, %worldLoc, 2);
                }



	          AI::DirectiveWaypoint( %name, %worldLoc, 3000);
	          schedule ("AI::DirectiveRemove(" @ %name @ ", 3000);",10);

	   }





	   if( %command == 2 )  //Defend
	   {
	      // must convert waypoint location into world location.  waypoint location
	      //    is given in range [0-1023, 0-1023].  
	      %worldLoc = WaypointToWorld ( %waypoint );

//	      AI::DirectiveRemove( %name, 2 );   //Crude way to clear all directives. Needs to be done because else the bots won't
                                                 //respond to any new orders!



        if (getWord(%cmdText, 0) == "Deploy") //Deploy
         {
            if (getWord(%cmdText, 1) == "pulse") //Deploy Pulse Sensor
          {
              AI::DeployItem(%aiId, PulseSensorPack);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }

            if (getWord(%cmdText, 1) == "sensor") //Deploy Sensor Jammer
          {
              AI::DeployItem(%aiId, DeployableSensorJammerPack);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }

            if (getWord(%cmdText, 1) == "motion") //Deploy Motion Sensor
          {
              AI::DeployItem(%aiId, MotionSensorPack);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }

            if (getWord(%cmdText, 1) == "camera") //Deploy Camera
          {
              AI::DeployItem(%aiId, CameraPack);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }

            if (getWord(%cmdText, 1) == "Ammo") //Deploy Ammo Station
          {
              AI::DeployItem(%aiId, DeployableAmmoPack);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }

            if (getWord(%cmdText, 1) == "Inventory") //Deploy Inventory Station
          {
              AI::DeployItem(%aiId, DeployableInvPack);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }

            if (getWord(%cmdText, 1) == "Turret") //Deploy Turret
          {
              AI::DeployItem(%aiId, TurretPack);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }

            if (getWord(%cmdText, 1) == "beacon") //Deploy Beacon
          {
              AI::DeployItem(%aiId, Beacon);
              schedule("Vehicle::passengerJump(0," @ %aiId @ ",0);", 1);
          }
         }
          else
         {


//           %xPos = getWord(%worldLoc, 0);
//           %yPos = getword(%worldLoc, 1);
//           %zPos = getWord(%worldLoc, 2);

//           newObject("AI", SimGroup);
//           newObject(%name, SimGroup);
//           newObject("Marker1", Marker, PathMarker,0,%xPos,%yPos,%zPos,0,0,0);
//           addToSet(%name, "Marker1");
//           addToSet("AI", %name);
//           addToSet("MissionGroup\\Teams\\team" @ %T, AI);

             AI::DirectiveFollow( %name, %commander, 0,3000);
             schedule ("AI::DirectiveRemove(" @ %name @ ", 3000);",10);

//	     dbecho ( 2, %name @ " IS PROCEEDING TO LOCATION " @ %worldLoc );
         }

  	 if( %command == 3 )  //Repair
	 {
	     return;
	 }


      }


//	   dbecho( 1, "AI::OnCommand() issued to  " @ %name @ "  with parameters: " );
//	   dbecho( 1, "Cmdr:        " @ %commander );
//	   dbecho( 1, "Command:     " @ %command );
//	   dbecho( 1, "Waypoint:    " @ %waypoint );
//	   dbecho( 1, "TargetId:    " @ %targetId );
//	   dbecho( 1, "cmdText:     " @ %cmdText );
//	   dbecho( 1, "cmdStatus:   " @ %cmdStatus );
//	   dbecho( 1, "cmdSequence: " @ %cmdSequence );

 
}


// Play the given wave file FROM %source to %DEST.  The wave name is JUST the basic wave
// name without voice base info (which it will grab for you from the source client Id).  
// Basically does some string fiddling for you.  
//
// Example:
//    Ai::soundHelper( 2051, cheer3 );
//
function Ai::soundHelper( %sourceId, %destId, %waveFileName )
{
   %wName = strcat( "~w", Client::getVoiceBase( %sourceId ) );
   %wName = strcat( %wName, ".w" );
   %wName = strcat( %wName, %waveFileName );
   %wName = strcat( %wName, ".wav" );
   
   dbecho( 2, "Trying to play " @ %wName );
   
   Client::sendMessage( %destId, 0, %wName );
}


function Ai::messageHelper(%targetId, %msg )
{
    Client::sendMessage( %targetId, 0, %msg );
}



// Default periodic callback.  [Note by default it isn't called unless a frequency 
//    is set up using AI::CallbackPeriodic().  Type in that command to see how 
//    it works].  
function AI::onPeriodic( %aiName )
{
//   dbecho(2, "onPeriodic() called with " @ %aiName );
}



//The following callbacks are responsible for the bot's chasing behaviour.
//modified by Werewolf

function AI::onDroneKilled(%aiName)
{
   $Spoonbot::NumBots = $Spoonbot::NumBots - 1;
   if( ! $SinglePlayer )
   {

    %aiId = BotFuncs::GetId(%aiName);
    %team = GameBase::getTeam(%aiId);
    $Spoonbot::BotStatus[%aiId] = "Dead";
    %curTarget = ai::getTarget( %aiName );
    %targetName = Client::getName(%curTarget);

    $BotThink::Definitive_Attackpoint[%aiId] = "";  // ERROR: I think there's no player ID for a dead bot.
    $BotThink::ForcedOfftrack[%aiId] = true;

    if ($Spoonbot::BotChat)
	{
	    %chatdelay = floor(getRandom() * (10 - 0.1));
	    schedule("AI::RandomSuckMsg(" @ %aiName @ ", " @ %team @ ");", %chatdelay );
	}


    if (%aiId != $DoNotRespawnAI)
    {
      // Delay $RespawnDelay seconds before respawning
      if ($Spoonbot::RespawnDelay == 0)
      {
          $Spoonbot::RespawnDelay = 30;    //No respawn delay set in spoonbot.cs ?? Ok, then assume 30 seconds.
      }
      schedule("AI::spawnAdditionalBot(" @ %aiName @ ", " @ %team @ ", False);", $Spoonbot::RespawnDelay );
      $DoNotRespawnAI = 0;
     }
   }
   else
   {
    // just in case:
    dbecho( 2, "Non training callback called from Training" );
   }




}

//these AI function callbacks can be very useful!

function AI::onTargetDied(%aiName, %idNum)
{


   %aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;
   %curTarget = %idNum;

   %team=Client::getTeam(%aiId);
    if ($Spoonbot::BotChat)
	{
           %chatdelay = floor(getRandom() * (10 - 0.1));
	   schedule("AI::RandomCheerMsg(" @ %aiName@ ", " @ %team @ ");", %chatdelay );
	}

       %chance  = floor(getRandom() * (10-0.1));
       if (%chance > 2)
       {
           %animation = radnomItems(8, $PlayerAnim::Celebration1, $PlayerAnim::Celebration2, $PlayerAnim::Celebration3, $PlayerAnim::Taunt1, $PlayerAnim::Taunt2, $PlayerAnim::Wave, $PlayerAnim::OverHere, $PlayerAnim::Salute);
	   BotFuncs::Animation(%aiId, %animation);
       }

   BotFuncs::DelAttackerFromAll(%aiId);	// Wicked69
   $BotThink::Definitive_Attackpoint[%aiId] = "";
   $BotThink::ForcedOfftrack[%aiId] = True;

   if(%curTarget == -1)
   {
   	return;
   }
      
   $Spoonbot::BotStatus[%aiId] = "Idle";
}                                 

function AI::onTargetLOSAcquired(%aiName, %idNum)
{
%aiId = BotFuncs::GetId(%aiName);               //Sometimes, switching teams make your own bots chase you like an enemy. This is very
if (%aiId==0)
	return;
%aiTeam = Client::getTeam(%aiId);         //strange, since switching to Observer mode and THEN to an other team does NOT produce this error.
%targetTeam = Client::getTeam(%idNum);    //This is a quick hack so bots won't continue hunting you if you're in the same team
if (%targetTeam != %aiTeam)
  {
//   dbecho(1, %aiName @ " just spotted an enemy");
   AI::HuntTarget(%aiName, %idNum, 1);
  }
}

function AI::onTargetLOSLost(%aiName, %idNum)
{
%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;
%aiTeam = Client::getTeam(%aiId);
%targetTeam = Client::getTeam(%idNum);
if (%targetTeam != %aiTeam)
  {
//   dbecho(1, %aiName @ " lost eye contact to enemy");
   AI::HuntTarget(%aiName, %idNum, 1);

if ($Spoonbot::BotJetting[%aiId] != 1)
  AI::JetSimulation(%aiId, 0);


  }
}

function AI::onTargetLOSRegained(%aiName, %idNum)
{
%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;
%aiTeam = Client::getTeam(%aiId);
%targetTeam = Client::getTeam(%idNum);
if (%targetTeam != %aiTeam)
  {
//   dbecho(1, %aiName @ " found the enemy again");
   AI::HuntTarget(%aiName, %idNum, 1);
   $SPOONBOT::AbortAIJet = %aiId;
  }
}




function AI::HuntTarget(%aiName, %idNum, %Follow) //If %Follow is 0 then waypoint will be updated. If 1 then Bot will follow regardless of LOS.
{
   %aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;
   %curTarget = %idNum;

   if(%curTarget == -1)
   {
   	return;
   }
      
//   dbecho(1, %aiName @ " target: " @ %curTarget);	
   
   %targLoc = GameBase::getPosition(Client::getOwnedObject(%curTarget));
   %aiLoc = GameBase::getPosition(Client::getOwnedObject(%aiId));
   %targetDist = Vector::getDistance(%aiLoc, %targLoc);
//   dbecho(2, "distance to target: " @ %targetDist @ " targetPosition: " @ targLoc @ " aiLocation: " @ %aiLoc);


 if(String::findSubStr(%aiName, "Sniper") >= 0)
 {
   if(String::findSubStr(%aiName, "CMD") == 0)
	{
           AI::SmartFollow (%aiName, %idNum, %targLoc, 0, %targetDist, 80, 2);
	}
	else
	{
           AI::SmartStayAway (%aiName, %idNum, %targLoc, 0, %targetDist, 80, 2);
	}
 }

 if(String::findSubStr(%aiName, "Painter") >= 0)
 {
   if(String::findSubStr(%aiName, "CMD") == 0)
	{
           AI::SmartFollow (%aiName, %idNum, %targLoc, 0, %targetDist, 80, 2);
	}
	else
	{
           AI::SmartStayAway (%aiName, %idNum, %targLoc, 0, %targetDist, 80, 2);
	}
 }

 if(String::findSubStr(%aiName, "Guard") >= 0)
 {
   AI::SmartFollow (%aiName, %idNum, %targLoc, %Follow, %targetDist, 80, 2);
 }

 if(String::findSubStr(%aiName, "Mortar") >= 0)
 {
   AI::SmartFollow (%aiName, %idNum, %targLoc, %Follow, %targetDist, 80, 2);
 }

 if(String::findSubStr(%aiName, "Demo") >= 0)
 {
   AI::SmartFollow (%aiName, %idNum, %targLoc, %Follow, %targetDist, 50, 2);
 }

 if(String::findSubStr(%aiName, "Medic") >= 0)
 {
   AI::SmartFollow (%aiName, %idNum, %targLoc, %Follow, %targetDist, 60, 2);
 }

 if(String::findSubStr(%aiName, "Miner") >= 0)
 {
   AI::SmartFollow (%aiName, %idNum, %targLoc, %Follow, %targetDist, 30, 2);
 }


}




//added by Werewolf
//This is for "smart" chasing/attack of enemies.
//If %Follow = 0 the AI will try to keep a minimum distance of %mintargetDist. If the distance is greater, it will come closer.
//If %Follow = 1 then the AI will follow the target %idNum to oblivion
//%order is the order number of the directive, so you can have orders OVERWRITE each other instead of stacking

function AI::SmartFollow (%aiName, %idNum, %targLoc, %Follow, %targetDist, %mintargetDist, %order)
{
   if (%Follow == 0)
    {
      if(%targetDist > %minTargetDist)
        {
          AI::DirectiveWaypoint( %aiName, %targLoc, %order, 0 ); 

        }
        else
        {
//          AI::DirectiveRemove( %aiName, %order);
        }

    }
    else
    {
      if(%targetDist > %minTargetDist)
        {
	  AI::DirectiveFollow( %aiName, %idNum, 0, %order );

        }
        else
        {
//          AI::DirectiveRemove( %aiName, %order);
        }
    }
}


//added by Werewolf
//This is for "smart" chasing/attack of enemies.
//The AI will evade slightly until distance to enemy is < %mintargetDist.
//%order is the order number of the directive, so you can have orders OVERWRITE each other instead of stacking

function AI::SmartStayAway (%aiName, %idNum, %targLoc, %Follow, %targetDist, %mintargetDist, %order)
{
   if (%Follow == 0)
    {
      if(%targetDist < %minTargetDist)
        {
          AI::DirectiveWaypoint( %aiName, %targLoc, %order, 0 ); 
        }
        else
        {
//          AI::DirectiveRemove( %aiName, %order);
        }
    }
    else
    {
      if(%targetDist < %minTargetDist)
        {
    	  AI::DirectiveFollow( %aiName, %idNum, 0, %order );     
        }
        else
        {
//          AI::DirectiveRemove( %aiName, %order);
        }
    }

}




function AI::pickRandomSpawn(%team)
{
   %group = nameToID("MissionGroup/Teams/team" @ %team @ "/DropPoints/Random");
   %count = Group::objectCount(%group);
   if(!%count)
      return -1;

   %spawnIdx = floor(getRandom() * (%count - 0.1));
   %value = %count;
   for(%i = %spawnIdx; %i < %value; %i++) {
      %set = newObject("set",SimSet);
      %obj = Group::getObject(%group, %i);
      if(containerBoxFillSet(%set,$SimPlayerObjectType|$VehicleObjectType,GameBase::getPosition(%obj),2,2,4,0) == 0) 
         return %obj;
      if(%i == %count - 1) {
         dbecho(1, "pickRandomSpawn error: You forgot to set Random Drop points in your map!");
         %i = -1;
         %value = %spawnIdx;
      }
      deleteObject(%set);
   }
   return false;
}








//The following Callback isn't called - only God knows why..
//modified by Werewolf

function AI::onCollision (%aiId, %object)
{

   %targLoc = GameBase::getPosition(Client::getOwnedObject(%object));
   %aiLoc = GameBase::getPosition(Client::getOwnedObject(%aiId));
   %aiRotation = GameBase::GetRotation(Client::getOwnedObject(%aiId)); 

   dbecho(1, "obstacle location" @ %targLoc);
   dbecho(1, "ai location" @ %aiLoc);
   dbecho(1, "ai rotation" @ %aiLoc);

   dbecho(1, "onCollision called in ai.cs");
   Vehicle::passengerJump(0,%aiId,0);  //Crude way to make players avoid obstacles
   Vehicle::passengerJump(0,%object,0);


}









function AI::DeployItem(%aiId,%desc)  //added by Werewolf. Yay I'm proud of this piece of code ;-)
{

// List of deployable Items:
//--------------------------
//  DeployableAmmoPack
//  DeployableInvPack
//  TurretPack
//  CameraPack
//  DeployableSensorJammerPack
//  PulseSensorPack
//  MotionSensorPack
//  Beacon
//??mineammo??


        %item = %desc;
	%player = %aiId;

//echo(1, "Item description: \"" @ %desc @ "\"");
//echo(1, "item \"" @ %item @ "\"");

	// --------------------------------------
	// This doesn't work, so we have to do it manually.
	// --------------------------------------
	// Player::setItemCount(%aiId, %item, 1);
	// Player::deployItem(%aiId,%item);
	// --------------------------------------

        if (%item == "DeployableInvPack")
            {
                %client = Player::getClient(%aiId);
                %inv = newObject("ammounit_remote","StaticShape","DeployableInvStation",true);
                addToSet("MissionCleanup", %inv);
				%rot = GameBase::getRotation(%aiId); 
				GameBase::setTeam(%inv,GameBase::getTeam(%aiId));
				GameBase::setPosition(%inv,GameBase::getPosition(%aiId));
				GameBase::setRotation(%inv,%rot);
				Gamebase::setMapName(%inv,%name);
				Client::sendMessage(%client,0,"Inventory Station deployed");
				playSound(SoundPickupBackpack, GameBase::getPosition(%aiId));
				$TeamItemCount[GameBase::getTeam(%inv) @ "DeployableInvPack"]++;
				echo("MSG: ",%client," deployed an Inventory Station");
            }


        if (%item == "DeployableAmmoPack")
            {
                %client = Player::getClient(%aiId);
                %inv = newObject("ammounit_remote","StaticShape","DeployableAmmoStation",true);
                addToSet("MissionCleanup", %inv);
				%rot = GameBase::getRotation(%aiId); 
				GameBase::setTeam(%inv,GameBase::getTeam(%aiId));
				GameBase::setPosition(%inv,GameBase::getPosition(%aiId));
				GameBase::setRotation(%inv,%rot);
				Gamebase::setMapName(%inv,%name);
				Client::sendMessage(%client,0,"Ammo Station deployed");
				playSound(SoundPickupBackpack, GameBase::getPosition(%aiId));
				$TeamItemCount[GameBase::getTeam(%inv) @ "DeployableAmmoPack"]++;
				echo("MSG: ",%client," deployed an Ammo Station");
            }

        if (%item == "MotionSensorPack")
            {
                %client = Player::getClient(%aiId);
                %inv = newObject("","Sensor","DeployableMotionSensor",true);
                addToSet("MissionCleanup", %inv);
				%rot = GameBase::getRotation(%aiId); 
				GameBase::setTeam(%inv,GameBase::getTeam(%aiId));
				GameBase::setPosition(%inv,GameBase::getPosition(%aiId));
				GameBase::setRotation(%inv,%rot);
				Gamebase::setMapName(%inv,"Motion Sensor");
				Client::sendMessage(%client,0,"Motion Sensor deployed");
				playSound(SoundPickupBackpack, GameBase::getPosition(%aiId));
				$TeamItemCount[GameBase::getTeam(%inv) @ "MotionSensorPack"]++;
				echo("MSG: ",%client," deployed a Motion Sensor");
            }

        if (%item == "PulseSensorPack")
            {
                %client = Player::getClient(%aiId);
                %inv = newObject("","Sensor","DeployablePulseSensor",true);
                addToSet("MissionCleanup", %inv);
		%rot = GameBase::getRotation(%aiId); 
		GameBase::setTeam(%inv,GameBase::getTeam(%aiId));
		GameBase::setPosition(%inv,GameBase::getPosition(%aiId));
		GameBase::setRotation(%inv,%rot);
		Gamebase::setMapName(%inv,"Pulse Sensor");
		Client::sendMessage(%client,0,"Pulse Sensor deployed");
		playSound(SoundPickupBackpack, GameBase::getPosition(%aiId));
		$TeamItemCount[GameBase::getTeam(%inv) @ "PulseSensorPack"]++;
		echo("MSG: ",%client," deployed a Pulse Sensor");
            }


        if (%item == "DeployableSensorJammerPack")
            {
                %client = Player::getClient(%aiId);
                %inv = newObject("","Sensor","DeployableSensorJammer",true);
                addToSet("MissionCleanup", %inv);
		%rot = GameBase::getRotation(%aiId); 
		GameBase::setTeam(%inv,GameBase::getTeam(%aiId));
		GameBase::setPosition(%inv,GameBase::getPosition(%aiId));
		GameBase::setRotation(%inv,%rot);
		Gamebase::setMapName(%inv,"Sensor Jammer");
		Client::sendMessage(%client,0,"Sensor Jammer deployed");
		playSound(SoundPickupBackpack, GameBase::getPosition(%aiId));
		$TeamItemCount[GameBase::getTeam(%inv) @ "DeployableSensorJammerPack"]++;
		echo("MSG: ",%client," deployed a Sensor Jammer");
            }


        if (%item == "CameraPack")
            {
                %client = Player::getClient(%aiId);
		%camera = newObject("Camera","Turret",CameraTurret,true);
	   	addToSet("MissionCleanup", %camera);
		GameBase::setTeam(%camera,GameBase::getTeam(%aiId));
		GameBase::setRotation(%camera,%rot);
		GameBase::setPosition(%camera,GameBase::getPosition(%aiId));
		Gamebase::setMapName(%camera,"Camera#"@ $totalNumCameras++ @ " " @ Client::getName(%client));
		Client::sendMessage(%client,0,"Camera deployed");
		playSound(SoundPickupBackpack,GameBase::getPosition(%aiId));
		$TeamItemCount[GameBase::getTeam(%camera) @ "CameraPack"]++;
		echo("MSG: ",%client," deployed a Camera");
            }


        if (%item == "TurretPack")
            {
                %client = Player::getClient(%aiId);




	if($TeamItemCount[GameBase::getTeam(%aiId) @ %item] < $TeamItemMax[%item]) {
			%obj = getObjectType($los::object);
	    		%set = newObject("set",SimSet);
				%num = containerBoxFillSet(%set,$StaticObjectType,$los::position,$TurretBoxMaxLength,$TurretBoxMaxWidth,$TurretBoxMaxHeight,0);
				%num = CountObjects(%set,"DeployableTurret",%num);
				deleteObject(%set);
				if($MaxNumTurretsInBox > %num) {
		    		%set = newObject("set",SimSet);
					%num = containerBoxFillSet(%set,$StaticObjectType,$los::position,$TurretBoxMinLength,$TurretBoxMinWidth,$TurretBoxMinHeight,0);
					%num = CountObjects(%set,"DeployableTurret",%num);
					deleteObject(%set);
					if(0 == %num) {
						







                %rot = GameBase::getRotation(%aiId); 
		%turret = newObject("remoteTurret","Turret",DeployableTurret,true);
	        addToSet("MissionCleanup", %turret);
		GameBase::setTeam(%turret,GameBase::getTeam(%aiId));
		GameBase::setPosition(%turret,GameBase::getPosition(%aiId));
		GameBase::setRotation(%turret,%rot);
		Gamebase::setMapName(%turret,"RMT Turret#" @ $totalNumTurrets++ @ " " @ Client::getName(%client));
		Client::sendMessage(%client,0,"Remote Turret deployed");
		playSound(SoundPickupBackpack,GameBase::getPosition(%aiId));
		$TeamItemCount[GameBase::getTeam(%aiId) @ "TurretPack"]++;
		echo("MSG: ",%client," deployed a Remote Turret");



					} 
					else
						Client::sendMessage(%client,0,"Frequency Overload - Too close to other remote turrets");
				}
			   else 
					Client::sendMessage(%client,0,"Interference from other remote turrets in the area");
	}
	else																						  
	 	Client::sendMessage(%client,0,"Deployable Item limit reached for " @ %item.description @ "s");

	return false;








            }


        if (%item == "Beacon")
            {
                %client = Player::getClient(%aiId);
                %beacon = newObject("Target Beacon", "StaticShape", "DefaultBeacon", true);
		addToSet("MissionCleanup", %beacon);
		GameBase::setTeam(%beacon,GameBase::getTeam(%aiId));
		GameBase::setRotation(%beacon,%rot);
		GameBase::setPosition(%beacon,GameBase::getPosition(%aiId));
		Gamebase::setMapName(%beacon,"Target Beacon");
		Beacon::onEnabled(%beacon);
		Client::sendMessage(%client,0,"Beacon deployed");
		$TeamItemCount[GameBase::getTeam(%beacon) @ "Beacon"]++;
            }

}




function AI::RemoveBot(%aiName, %commandissuer)
{
  %aiId = BotFuncs::GetId( %aiName );
if (%aiId==0)
	return;
  $DoNotRespawnAI = %aiId;
  Player::kill(%aiId);
}


function AI::buildGraph()
{
	echo("building AI Graph...");
   %nodeGroup = nameToID("MissionGroup\\AIGraph");
   %numNodes  = Group::ObjectCount(%nodeGroup);
   
   echo("nodeGroup: " @ %nodeGroup @ " number of Nodes: " @ %numNodes); 
   
   for(%i = 0; %i < %numNodes; %i++)
   {
   	%node    = Group::getObject(%nodeGroup, %i);
      %nodePos = GameBase::getPosition(%node);
      %name = "Node " @ %i;
      //echo("name of node:" @ %name);
      //echo("adding node: " @ %node @ " at position: " @ %nodePos);
      if(name != "")
      	Graph::AddNode(%nodePos, %name);
      else 
         Graph::AddNode(%nodePos);
   }
   if(Graph::buildGraph() == -1)
    	echo("Can't create adjacent lists for graph nodes.");
   else
   	echo("Graph build complete.");
}


//Render lines to show AI Graph
function drawGraph()
{
	newObject("graphRender", GraphPathRender);
}


function AI::RandomCheerMsg(%aiName, %team)
{
%perChance = floor(getRandom() * (100 - 0.1));
if (%perChance <= 10)
 {
	%aiId = BotFuncs::GetId( %aiName );
if (%aiId==0)
	return;
	%enemyTeam = 1;
	if (%team == 1)
	{
	  %enemyTeam = 0;
	}
	
	
		%msgIdx = floor(getRandom() * (8 - 0.1));
		%msgIdx1 = floor(getRandom() * (128 - 0.1));
	for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
	  {
		Ai::soundHelper( %aiId, %cl, $vcheerList[%msgIdx] );
		Client::sendMessage(%cl, 2, %aiName @ ": " @ $cheerList[%msgIdx1], %aiId);
	  }
 }
}



function AI::RandomSuckMsg(%aiName, %team)
{
%perChance = floor(getRandom() * (10 - 0.1));
if (%perChance >= 8 )
 {
	%aiId = BotFuncs::GetId( %aiName );
//if (%aiId==0)
//	return;
	%enemyTeam = 1;
	if (%team == 1)
	{
	  %enemyTeam = 0;
	}
	
		%msgIdx = floor(getRandom() * (8 - 0.1));
		%msgIdx1 = floor(getRandom() * (48 - 0.1));
	for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))
	  {
		Ai::soundHelper( %aiId, %cl, $vsuckList[%msgIdx] );
			Client::sendMessage(%cl, 2, %aiName @ ": " @ $suckList[%msgIdx1], %aiId);
	  }
	
 }
}


function AI::ProcessAutoSpawn()		// A *VERY* dirty way to process the spoonbot.cs file... I know ;-)
{
if (($Spoonbot::AutoSpawn) && (!$Spoonbot::BotTree_Design)) // Not more bots in design mode.
 {
	if ($Spoonbot::Bot1Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot1Name @ ", " @ $Spoonbot::Bot1Team @ ", 0);",0.5);
	if ($Spoonbot::Bot2Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot2Name @ ", " @ $Spoonbot::Bot2Team @ ", 0);",1);
	if ($Spoonbot::Bot3Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot3Name @ ", " @ $Spoonbot::Bot3Team @ ", 0);",1.5);
	if ($Spoonbot::Bot4Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot4Name @ ", " @ $Spoonbot::Bot4Team @ ", 0);",2);
	if ($Spoonbot::Bot5Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot5Name @ ", " @ $Spoonbot::Bot5Team @ ", 0);",2.5);
	if ($Spoonbot::Bot6Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot6Name @ ", " @ $Spoonbot::Bot6Team @ ", 0);",3);
	if ($Spoonbot::Bot7Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot7Name @ ", " @ $Spoonbot::Bot7Team @ ", 0);",3.5);
	if ($Spoonbot::Bot8Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot8Name @ ", " @ $Spoonbot::Bot8Team @ ", 0);",4);
	if ($Spoonbot::Bot9Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot9Name @ ", " @ $Spoonbot::Bot9Team @ ", 0);",4.5);
	if ($Spoonbot::Bot10Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot10Name @ ", " @ $Spoonbot::Bot10Team @ ", 0);",5);
	if ($Spoonbot::Bot11Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot11Name @ ", " @ $Spoonbot::Bot11Team @ ", 0);",5.5);
	if ($Spoonbot::Bot12Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot12Name @ ", " @ $Spoonbot::Bot12Team @ ", 0);",6);
	if ($Spoonbot::Bot13Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot13Name @ ", " @ $Spoonbot::Bot13Team @ ", 0);",6.5);
	if ($Spoonbot::Bot14Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot14Name @ ", " @ $Spoonbot::Bot14Team @ ", 0);",7);
	if ($Spoonbot::Bot15Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot15Name @ ", " @ $Spoonbot::Bot15Team @ ", 0);",7.5);
	if ($Spoonbot::Bot16Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot16Name @ ", " @ $Spoonbot::Bot16Team @ ", 0);",8);
	if ($Spoonbot::Bot17Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot17Name @ ", " @ $Spoonbot::Bot17Team @ ", 0);",8.5);
	if ($Spoonbot::Bot18Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot18Name @ ", " @ $Spoonbot::Bot18Team @ ", 0);",9);
	if ($Spoonbot::Bot19Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot19Name @ ", " @ $Spoonbot::Bot19Team @ ", 0);",9.5);
	if ($Spoonbot::Bot20Name != "0")
		schedule("AI::spawnAdditionalBot(" @ $Spoonbot::Bot20Name @ ", " @ $Spoonbot::Bot20Team @ ", 0);",10);

//	AI::spawnAdditionalBot($Spoonbot::Bot2Name, $Spoonbot::Bot2Team, 0);
 }
}

function AI::Jet(%aiId)
{
	%passenger = %aiId;
	%armor = Player::getArmor(%passenger);

	if(%armor == "larmor" || %armor == "lfemale") {
		%height = 15;
		%velocity = 70;
		%zVec = 40;
	}
	else if(%armor == "marmor" || %armor == "mfemale") {
		%height = 19;
		%velocity = 100;
		%zVec = 60;
	}
	else if(%armor == "harmor") {
		%height = 22;
		%velocity = 140;
		%zVec = 90;
	}

	%pos = GameBase::getPosition(%passenger);
	%posX = getWord(%pos,0);
	%posY	= getWord(%pos,1);
	%posZ	= getWord(%pos,2);

	if(GameBase::testPosition(%passenger,%posX @ " " @ %posY @ " " @ (%posZ + %height))) {	
		%rotZ = getWord(GameBase::getRotation(%passenger),2);
		GameBase::setRotation(%passenger, "0 0 " @ %rotZ);
		GameBase::setPosition(%passenger,%posX @ " " @ %posY @ " " @ (%posZ + %height));
		%jumpDir = Vector::getFromRot(GameBase::getRotation(%passenger),%velocity,%zVec);
		Player::applyImpulse(%passenger,%jumpDir);
	}

}

function AI::Jump(%aiId)      //This function makes the AI jump. If %jet=1 then it Calls the Jetpack Routine
{
    %passenger = %aiId;
	%armor = Player::getArmor(%passenger);
	if(%armor == "larmor" || %armor == "lfemale") {
		%height = 2;
		%velocity = 70;
		%zVec = 70;
	}
	else if(%armor == "marmor" || %armor == "mfemale") {
		%height = 2;
		%velocity = 100;
		%zVec = 100;
	}
	else if(%armor == "harmor") {
		%height = 2;
		%velocity = 140;
		%zVec = 110;
	}

	%pos = GameBase::getPosition(%passenger);
	%posX = getWord(%pos,0);
	%posY	= getWord(%pos,1);
	%posZ	= getWord(%pos,2);

	if(GameBase::testPosition(%passenger,%posX @ " " @ %posY @ " " @ (%posZ + %height))) {	
		%rotZ = getWord(GameBase::getRotation(%passenger),2);
		GameBase::setRotation(%passenger, "0 0 " @ %rotZ);
		GameBase::setPosition(%passenger,%posX @ " " @ %posY @ " " @ (%posZ + %height));
		%jumpDir = Vector::getFromRot(GameBase::getRotation(%passenger),%velocity,%zVec);
		Player::applyImpulse(%passenger,%jumpDir);
	}
}






// These vector operations are something I am quite proud of.
// They're still in AI.CS and not in BotFuncs.cs because they can be applied on human players too.
// Werewolf

function AI::shove(%aiId, %velocity, %zVec, %rotX, %rotY, %rotZ)
{
		%passenger = %aiId;
		%OldrotX = getWord(GameBase::getRotation(%passenger),0);
		%OldrotY = getWord(GameBase::getRotation(%passenger),1);
		%OldrotZ = getWord(GameBase::getRotation(%passenger),2);
		%rotation = (%OldrotX + %rotX) @ " " @ (%OldrotY + %rotY) @ " " @ (%OldrotZ + %rotZ);
		GameBase::setRotation(%passenger, %rotation);
		%jumpDir = Vector::getFromRot(%rotation, %velocity, %zVec);
		Player::applyImpulse(%passenger,%jumpDir);
}




function AI::EvadeUp(%aiId)         //For climbing towers, one powerful jump back, and one forward after 1 second
{
%velocity = -100;
%zVec = 200;
AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
%velocity = 120;
%zVec = 200;
//AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
schedule("AI::shove(" @ %aiId @ ", " @ %velocity @ ", " @ %zVec @ ", 0, 0, 0);", 1);
}


function AI::EvadeBackLeft(%aiId)   //Turn back, then left and try again.
{
%velocity = -100;
%zVec = 100;
AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
%velocity = 150;
%zVec = 100;
schedule("AI::shove(" @ %aiId @ ", " @ %velocity @ ", " @ %zVec @ ", 0, 0, -1.6);", 1);
}

function AI::EvadeBackRight(%aiId)  //Turn back, then right and try again.
{
%velocity = -100;
%zVec = 100;
AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
%velocity = 150;
%zVec = 100;
schedule("AI::shove(" @ %aiId @ ", " @ %velocity @ ", " @ %zVec @ ", 0, 0, 1.6);", 1);
}

function AI::EvadeLeft(%aiId)       //Jump left
{
%velocity = 150;
%zVec = 100;
AI::shove(%aiId, %velocity, %zVec, 0, 0, -1.6);
}

function AI::EvadeRight(%aiId)      //Jump left
{
%velocity = 150;
%zVec = 100;
AI::shove(%aiId, %velocity, %zVec, 0, 0, 1.6);
}




function AI::RandomEvade(%aiId)      //Do anything of the above, randomly.
{
if ($Spoonbot::DebugMode)
 echo ("CALL AI::RandomEvade(" @ %aiId @ ");");

%evadeIdx = floor(getRandom() * (5 - 0.1));  // five possibilities

if ($Spoonbot::DebugMode)
 echo ("STATUS AI::RandomEvade = EvadeIdx=" @ %evadeIdx );


if (%evadeIdx == 0)
 {
AI::EvadeBackLeft(%aiId);
 }
else if (%evadeIdx == 1)
 {
AI::EvadeBackRight(%aiId);
 }
else if (%evadeIdx == 2)
 {
AI::EvadeUp(%aiId);
 }
else if (%evadeIdx == 3)
 {
AI::EvadeLeft(%aiId);
 }
else if (%evadeIdx == 4)
 {
AI::EvadeRight(%aiId);
 }
else if (%evadeIdx == 5)
 {
AI::EvadeBackLeft(%aiId);
 }
else if (%evadeIdx == 6)
 {
AI::EvadeBackRight(%aiId);
 }

return;
}



//The following function comes very close to human players jetting physics. This function calls itself over and over,
//each time applying a small amount of upward thrust to the AI. The variable "phase" is being increased with each call.
//The jetting time is limited, just like for human players.
//To make an AI start jetting, issue this function with "phase" set to 0. Then, this procedure will call itself every 0.5 seconds,
//each time increasing the "phase" variable.
//The AI will jet until "phase" = 6. Then, the AI will begin descending, while the descend speed is kept nominal by short thrusts 
//every 1 second. This prevents injuries.


//Note that once the jetting has started, the whole sequence is being executed. To STOP a jetting maneuver, set the 
//variable $SPOONBOT::StopAIJet to the aiId of the bot you want to stop jetting. If the jetting is being aborted, this function will
//set the "phase" variable to 6, thus starting to descend.


//What's the magic with AbortJet? Or better yet: What's the difference between STOPPING and ABORTING a jet maneuver?
//Well, let's assume you  want to issue a jet command, and don't know if the AI is already in the process of jetting. 
//You need to ABORT the first jetting, and schedule the NEW jetting command 1 or 2 seconds afterwards.
//This keeps the bots from climbing into the stratosphere.


function AI::JetSimulation(%aiId, %phase)         // Makes an AI jet like a real player.
{
if ($Spoonbot::DebugMode)
 echo ("STATUS AI::JetSimulation = in Phase " @ %phase @ " for bot " @ %aiId @ ". BotJetting is " @ $Spoonbot::BotJetting[%aiId]);

if ($SPOONBOT::StopAIJet == %aiId)  //To stop a jet, set $SPOONBOT::StopAIJet to the aiId you want to stop jetting
 {
 if (%phase < 6 )
   %phase = 6;			     //Just skip the climb phases, and start to descend
 $SPOONBOT::StopAIJet = 0;
 }

if ($SPOONBOT::AbortAIJet == %aiId)  //AbortJet is similar to StopJet, only there will be no controlled descend afterwards
 {
 $SPOONBOT::AbortAIJet = 0;
 return;			     //When Aborting, the function will kill itself immediately.
 }

$Spoonbot::BotJetting[%aiId] = 1;

%velocity = 20;
%zVec = 100;
AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
%phase = %phase + 1;

if (%phase < 6)
 {
 $Spoonbot::BotJettingHeat[%aiId] = 1;
 schedule("AI::JetSimulation(" @ %aiId @ ", " @ %phase @ ");", 0.5);  //First 6 phases: climb
 return;
 }

$Spoonbot::BotJettingHeat[%aiId] = 0;  //The BotJettingHeat is only 1 if we're in climbing phase. This is for rocket turrets.

if (%phase < 10)
 {
 schedule("AI::JetSimulation(" @ %aiId @ ", " @ %phase @ ");", 1);    //After climbing, do a controlled descend.
 return;
 }

$Spoonbot::BotJetting[%aiId] = -1;

// if "phase" exceeds 10, then this function will simply stop keeping itself alive.
return;
}

function AI::JetToHeight(%aiId, %height, %phase)         // Makes an AI jet to a specified height. Here the phase variable is only for INTERNAL use!
{

	$Spoonbot::BotJetting[%aiId] = -1;
	$Spoonbot::BotJettingHeat[%aiId] = 0;

	%AiPos = GameBase::getPosition(%aiId);
	%zPos = getWord(%AiPos, 2);


					//If we reached our height, stop jetting and abort this function.
					//I tried a new trick here: I stop the jetting sooner if the bot already has a great upward speed.

	if (%phase >=8)
		{
		if ((%zPos+6) >= %height)
			{
			%phase = 0;
			%velocity = 50;
			%zVec = 50;
			AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
			return;
			}
		}

	if (%phase >=6)
		{
		if ((%zPos+5) >= %height)
			{
			%phase = 0;
			%velocity = 50;
			%zVec = 50;
			AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
			return;
			}
		}

	if (%phase >=4)
		{
		if ((%zPos+4) >= %height)
			{
			%phase = 0;
			%velocity = 50;
			%zVec = 50;
			AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
			return;
			}
		}


	$Spoonbot::BotJetting[%aiId] = 1;
	$Spoonbot::BotJettingHeat[%aiId] = 1;  //These vars make turrets track the jetting bots, and avoid "double-jetting"

	%velocity = 0;  //This is the forward velocity.

	%zVec = 150;

	AI::shove(%aiId, %velocity, %zVec, 0, 0, 0);
	%phase = %phase + 1;

	schedule("AI::JetToHeight(" @ %aiId @ ", " @ %height @ ", " @ %phase @ ");", 0.3); 
	
	//Reschedule this until bot reaches %height.

	return;
}
