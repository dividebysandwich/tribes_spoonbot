//=========================================================================================================================================
//This will spawn an AI player in a map that does not yet contain a bot
//by Werewolf
//=========================================================================================================================================
//
// The following function has been modified so that armors can be modifies in the shifter_v1/spoonbot .cs file... This is to allow the bots
// to be much more configurable by the system admin... Check the Spoonbot.cs file for more details on this!!!
//																																Emo1313
//=========================================================================================================================================

function AI::SpawnAdditionalBot(%aiName, %teamnum, %spawnFromAdmin)
{

	if (%aiName == "")
	{
	  return;
	}
	
	   $numAI = 0;
	
	
	   %spawnMarker = AI::pickRandomSpawn(%teamnum);
	   if(%spawnMarker == -1)
	   {
	      %spawnPos = "0 0 300";
	      %spawnRot = "0 0 0";
	   }
	   else
	   {
	      %spawnPos = GameBase::getPosition(%spawnMarker);
	      %spawnRot = GameBase::getRotation(%spawnMarker);
	   }
	
		 %rPos = %spawnRot;
	         %xPos = getWord(%spawnPos, 0);
	         %yPos = getword(%spawnPos, 1);
	         %zPos = getWord(%spawnPos, 2);
	         %aiSpawnPos = %xPos @ "  " @ %yPos @ "  " @ %zPos;
	
	//         dbecho(1, "Random Spawn Position is: " @ %xPos @ "  " @ %yPos @ "  " @ %zPos);
	
	
	//======================================================================================================================= Setup Bot Armors
	
	    if(String::findSubStr(%aiName, "Female") >= 0)        //All the IFs are from Werewolf
	    {
		      $AI::defaultArmorType = $SpoonBot::StandardFArmor;
	      %voice = "female1";
	    }
	    else
	    {
		      $AI::defaultArmorType = $SpoonBot::StandardMArmor;
		      %voice = "male1";
	    }
	
	
	    if(String::findSubStr(%aiName, "Mortar") >= 0)
	    {
		    if(String::findSubStr(%aiName, "Female") >= 0)
		    {
			      $AI::defaultArmorType = $SpoonBot::MortarFArmor;
			      %voice = "female5";
		    }
		    else
		    {
			      $AI::defaultArmorType = $SpoonBot::MortarMArmor;
			      %voice = "male5";
		    }
	    }


	    if(String::findSubStr(%aiName, "Guard") >= 0)
	    {
		    if(String::findSubStr(%aiName, "Female") >= 0)
		    {
			      $AI::defaultArmorType = $SpoonBot::GuardFArmor;
			      %voice = "female5";
		    }
		    else
		    {
			      $AI::defaultArmorType = $SpoonBot::GuardMArmor;
			      %voice = "male5";
		    }
	    }
	
	
	    if(String::findSubStr(%aiName, "Demo") >= 0)
	    {
		    if(String::findSubStr(%aiName, "Female") >= 0)
		    {
			      $AI::defaultArmorType = $SpoonBot::DemoFArmor;
			      %voice = "female4";
		    }
		    else
		    {
			      $AI::defaultArmorType = $SpoonBot::DemoMArmor;
			      %voice = "male4";
		    }
	    }
	

	    if(String::findSubStr(%aiName, "Medic") >= 0)
	    {
		    if(String::findSubStr(%aiName, "Female") >= 0)
		    {
			      $AI::defaultArmorType = $SpoonBot::MedicFArmor;
			      %voice = "female2";
		    }
		    else
		    {
			      $AI::defaultArmorType = $SpoonBot::MedicMArmor;
			      %voice = "male4";
		    }
	    }
	
	
	    if(String::findSubStr(%aiName, "Sniper") >= 0)
	    {
		    if(String::findSubStr(%aiName, "Female") >= 0)
		    {
			      $AI::defaultArmorType = $SpoonBot::SniperFArmor;
			      %voice = "female2";
		    }
		    else
		    {
			      $AI::defaultArmorType = $SpoonBot::SniperMArmor;
			      %voice = "male2";
		    }
	    }
	
	
	    if(String::findSubStr(%aiName, "Painter") >= 0)
	    {
		    if(String::findSubStr(%aiName, "Female") >= 0)
		    {
			      $AI::defaultArmorType = $SpoonBot::PainterFArmor;
			      %voice = "female1";
		    }
		    else
		    {
			      $AI::defaultArmorType = $SpoonBot::PainterMArmor;
			      %voice = "male1";
		    }
	    }
	
	
	    if(String::findSubStr(%aiName, "Miner") >= 0)
	    {
		    if(String::findSubStr(%aiName, "Female") >= 0)
		    {
			      $AI::defaultArmorType = $SpoonBot::MinerFArmor;
			      %voice = "female2";
		    }
		    else
		    {
			      $AI::defaultArmorType = $SpoonBot::MinerMArmor;
			      %voice = "male4";
		    }
	    }



//================================================================================================================== Done Setting Up Armors

	  if (%spawnFromAdmin == 1) //Are we spawning from the menu in admin.cs ?
	  {                         //If yes, we have to insert a bot count number into the name
	      %num = 1;
	      %newName = "T" @ %teamnum @ "N" @ %num @"_" @ %aiName;
	      %spawnSuccessfull = 0;
	      if( AI::spawn( %newName, $AI::defaultArmorType, %aiSpawnPos, %rPos, %newName, %voice  ) != "false" )
	      {
		$Spoonbot::NumBots = $Spoonbot::NumBots + 1;
	        %spawnSuccessfull = 1;
	      }
	      else
	      {
	        %num++;
	        %newName = "T" @ %teamnum @ "N" @ %num @"_" @ %aiName;
	        if( AI::spawn( %newName, $AI::defaultArmorType, %aiSpawnPos, %rPos, %newName, %voice  ) != "false" )
	        {
		$Spoonbot::NumBots = $Spoonbot::NumBots + 1;
	          %spawnSuccessfull = 1;
	        }
	        else
	        {
	          %num++;
	          %newName = "T" @ %teamnum @ "N" @ %num @"_" @ %aiName;
	          if( AI::spawn( %newName, $AI::defaultArmorType, %aiSpawnPos, %rPos, %newName, %voice  ) != "false" )
	          {
		$Spoonbot::NumBots = $Spoonbot::NumBots + 1;
	            %spawnSuccessfull = 1;
	          }
	          else
	          {
	            dbecho( 1, "Cannot spawn " @ %aiName @ ", only 3 bots per class allowed for each team");
	          }
	        }
	      }
	  }
	  else
	  {                      
	      %newName = %aiName;
	      if( AI::spawn( %newName, $AI::defaultArmorType, %aiSpawnPos, %rPos, %newName, %voice  ) != "false" )
	      {
		$Spoonbot::NumBots = $Spoonbot::NumBots + 1;
	         %spawnSuccessfull = 1;
	
			 %aiId = BotFuncs::GetId(%aiName);
	
			 $BotThink::BotHome[%aiId] = %spawnPos; // Set AI homepoint as spawnpoint. Wicked69
	
		     //dbecho( 1, "AutoSpawn successfull for " @ %aiName );
	      }
		  else
	      {
	         dbecho( 1, "AutoSpawn error: Cannot spawn " @ %aiName );
	      }
	    
	  }
	
	
	  newObject("AI", SimGroup);
	  newObject(%newName, SimGroup);
	  newObject("Marker1", Marker, PathMarker,0,%xPos,%yPos,%zPos,0,0,0);
	  addToSet(%newName, "Marker1");
	  addToSet("AI", %newName);
	  addToSet("MissionGroup\\Teams\\team" @ %teamnum, AI);
	
	
	
	
	// Above method allows spawning of 3 bots per class and team. The old method below only allowed one.
	// The problem was to try all names until a free name was found. That way you can control the number of bots the client can spawn...
	// ... and we don't wanna have a player spawning huge armies of bots ;-)
	
	//   Ai::spawn(%newName, $AI::defaultArmorType, %aiSpawnPos, %rPos, %newName, %voice );
	
	
	  %aiId = BotFuncs::GetId( %newName );
	  GameBase::setTeam(%aiId, %teamnum);
	  AI::setVar( %newName,  iq,  90 );
	  AI::setVar( %newName,  attackMode, 1);
	  AI::setVar( %newName,  pathType, $AI::defaultPathType);
	  

//	  schedule("AI::setWeapons(" @ %aiId @ ");", 1); 
	  schedule("AI::setWeapons(" @ %newName @ ");", 1); 
	
	// Add Bot think function to schedule - Wicked69
	
	  BotFuncs::InitVars( %aiId );      // Wicked69
	
	  Client::setSkin(%aiId, $Server::teamSkin[Client::getTeam(%aiId)]);  // Werewolf
	
		if (BotTypes::IsMedic(%newName))    //As of yet only one Medic can work in the Object Repair Task Queue. (This means repairing Turrets, etc)
		{
			if (%teamnum == 0)
				 $Spoonbot::Team0Medic = %aiId;
			if (%teamnum == 1)
				 $Spoonbot::Team1Medic = %aiId;
		}
	
	
    $BotThink::Definitive_Attackpoint[%aiId] = "";
    $BotThink::ForcedOfftrack[%aiId] = true;

	  schedule("BotThink::Think(" @ %aiId @ ", True);", 3);      // Wicked69
	  schedule("BotMove::Move(" @ %aiId @ " );", 3);      // Werewolf

	  return ( %newName );
}






























//----------------------------------
// AI::setupAI()
//
// Called from Mission::init() which is defined in Objectives.cs (or Dm.cs for
//    deathmatch missions).  
//----------------------------------   
//modified by Werewolf
function AI::setupAI(%key, %team)
{


    %aiName = %key;

    if (%aiName == "")
     {
      return;
     }

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


    if(String::findSubStr(%aiName, "Mortar") >= 0)
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


    if(String::findSubStr(%aiName, "Guard") >= 0)
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


   //if there is no key then they don't exist yet
   if(%key == "")
   {
      %aiFound = 0;
      for( %T = 0; %T < 8; %T++ )
      {
         %groupId = nameToID("MissionGroup\\Teams\\team" @ %T @ "\\AI" );
         if( %groupId != -1 )
         {
            %teamItemCount = Group::objectCount(%groupId);
            if( %teamItemCount > 0 )
            {
               AI::initDrones(%T, %teamItemCount);
               %aiFound += %teamItemCount;
            }
         }
      }
      if( %aiFound == 0 )
         dbecho(1, "No drones exist in map...");
      else
         dbecho(1, %aiFound @ " drones installed..." );
   }
   else     //respawning dead AI with original name and path
   {
      %group = nameToID("MissionGroup\\Teams\\team" @ %team @ "\\AI\\" @ %key);
      %num = Group::objectCount(%group);
      %aiId = BotFuncs::GetId(%key);

      if( %aiId <= 0) // Is it a pre-defined AI, or a dynamically spawned? If it is the latter, then...
      {
         %teamnum = %team;
         %newName = %key;



// New method for finding spawn points:
//------------------------------------
   %spawnMarker = AI::pickRandomSpawn(%teamnum);
   if(%spawnMarker == -1)
   {
      %spawnPos = "0 0 300";
      %spawnRot = "0 0 0";
   }
   else
   {
      %spawnPos = GameBase::getPosition(%spawnMarker);
      %spawnRot = GameBase::getRotation(%spawnMarker);
   }



         %xPos = getWord(%spawnPos, 0);
         %yPos = getword(%spawnPos, 1);
         %zPos = getWord(%spawnPos, 2);
//         %rPos = GameBase::getRotation(%commandIssuer);
         %aiSpawnPos = %xPos @ "  " @ %yPos @ "  " @ %zPos;

//         dbecho(1, "Random Spawn Position is: " @ %xPos @ "  " @ %yPos @ "  " @ %zPos);


         Ai::spawn(%newName, $AI::defaultArmorType, %aiSpawnPos, %spawnRot, %newName, %voice );
	 $Spoonbot::NumBots = $Spoonbot::NumBots + 1;
         %aiId = BotFuncs::GetId( %newName );
         GameBase::setTeam(%aiId, %teamnum);
         AI::setVar( %newName,  iq,  90 );
         AI::setVar( %newName,  attackMode, 1);
         AI::setVar( %newName,  pathType, $AI::defaultPathType);

  BotFuncs::InitVars( %aiId );      // Wicked69

  Client::setSkin(%aiId, $Server::teamSkin[Client::getTeam(%aiId)]);  // Werewolf

if (BotTypes::IsMedic(%newName))    //As of yet only one Medic can work in the Object Repair Task Queue. (This means repairing Turrets, etc)
{
if (%teamnum == 0)
 $Spoonbot::Team0Medic = %aiId;
if (%teamnum == 1)
 $Spoonbot::Team1Medic = %aiId;
}

  $BotThink::Definitive_Attackpoint[%aiId] = -1;
  $BotThink::ForcedOfftrack[%aiId] = true;

  schedule("BotThink::Think(" @ %aiId @ ", True);", 3);      // Wicked69

// Need moving to new code. wicked69.

         schedule("AI::setWeapons(" @ %aiId @ ");", 1);
         
      }
      //--------------------------------------------


      else  //else respawn like regular AIs
      {


      createAI(%key, %group, $AI::defaultArmorType, %key);
      %aiId = BotFuncs::GetId(%key);


      GameBase::setTeam(%aiId, %team);
      AI::setVar(%key, pathType, $AI::defaultPathType);

      AI::setWeapons(%aiId);
//      AI::setWeapons(%key);

   }
  }	
}

