// Bot Generic Functions by Wicked69 (Paul@Lathope.demon.co.uk) Started 7/11/1999.

// Global Vars used by following functions.

$AttackerIndexCount = 0;



// Creates vars for AI and enters AI into master list. Wicked69


function BotFuncs::GetId(%aiName)
{

	if (%aiName == "")
	{
		return 0;
	}
	return ai::GetId(%aiName);
}

function BotFuncs::InitVars(%aiId)
{
	%AiPos = GameBase::getPosition(%aiId);
	$Pilot::DontCheck[%aiId] = 0;
	$Spoonbot::lastPosition[%aiId] = %AiPos;
	$Spoonbot::BotStatus[%aiId] = "Idle";
	$Spoonbot::Team0MedicTask = -1;
	$Spoonbot::Team1MedicTask = -1;
	$Spoonbot::BotJetting[%aiId] = -1;
	$Spoonbot::BotJettingHeat[%aiId] = 0;
	$BotThink::LastPoint[%aiId] = True;
	$BotFuncs::AttackerCount[%aiId] = 0;
	$BotFuncs::AttackerIndex[%aiId] = 0;
	$Spoonbot::RidingWith[%cl] = 0;
	BotFuncs::AddAttackerIndex(%aiId);
	$Spoonbot::MedicTask[%aiId] = -1;
	$Spoonbot::PainterTarget[%aiId]=-1;
	$Spoonbot::MortarTarget[%aiId]=-1;
	$Spoonbot::Target[%aiId]=-1;
	$Spoonbot::AlreadyLookedForTargets[%aiId] = False;
	$CurrentTargetPos[%aiId]=0;
	$Spoonbot::MortarBusy[%aiId]=0;
	$Spoonbot::MedicBusy[%aiId]=0;
	$BotThink::LastPoint[%aiId]=0;
}



// Resets variables for AI. Should be used on respawn etc. Wicked69

function BotFuncs::ClearVars(%aiId)
{
	$Spoonbot::BotStatus[%aiId] = "Idle";
	$Pilot::DontCheck[%aiId] = 0;
	$BotFuncs::AttackerCount[%aiId] = 0;
	$BotFuncs::AttackerIndex[%aiId] = 0;
	$Spoonbot::MedicTask[%aiId] = -1;
	$Spoonbot::PainterTarget[%aiId]=-1;
	$Spoonbot::Target[%aiId]=-1;
	$Spoonbot::MortarTarget[%aiId]=-1;
	$Spoonbot::AlreadyLookedForTargets[%aiId] = False;
	$CurrentTargetPos[%aiId]=0;
	$Spoonbot::MortarBusy[%aiId]=0;
	$Spoonbot::MedicBusy[%aiId]=0;
	$BotThink::LastPoint[%aiId]=0;
}



// Uses Attacker List to determine if AI is under attack.
// Under attack has prio of 1 e.g. defence entry. Also
// ignores attacker if dead.


function BotFuncs::IsUnderattack(%aiId)
{
	%count = 0;

	for(%i = 0; (%i < $BotFuncs::AttackerIndex[%aiId]); %i++)
		if (($BotFuncs::Attackers[%aiId,%i] != 0) &&
			($BotFuncs::Priority[%aiId,%i] == 1) &&
			(Player::isDead($BotFuncs::Attackers[%aiId,%i]) == False))
			%count++;
		
	return %count;
}



// Finds nearest attacker or entry to AI. Wicked69

function BotFuncs::NearestAttacker(%aiId, %MaxDistance, %SearchPrio)
{
	%BotPosition = GameBase::getPosition(%aiId);

	%AttackerDistance = 999999; //  Very Large Number !
	%AttackerId = %aiId;
	%AttackerPrio = 999999;

	for(%i = 0; %i < $BotFuncs::AttackerIndex[%aiId]; %i++)
	{
		if($BotFuncs::Attackers[%aiId,%i] != 0)
		{

			if ($BotFuncs::Priority[%aiId,%i] != 4)	//If it's not a Treepoint, get position
				%AttackerNominatePosition =	GameBase::getPosition($BotFuncs::Attackers[%aiId,%i]);
			else					//else, position is stored directly.
				%AttackerNominatePosition = $BotFuncs::Attackers[%aiId,%i];

			%AttackerNominateDistance = Vector::getDistance(%BotPosition,%AttackerNominatePosition);

			if (%SearchPrio!=4) //Looking for enemies
			{
				if (($BotFuncs::Priority[%aiId,%i] <= %AttackerPrio) &&
				(($BotFuncs::Priority[%aiId,%i] == %SearchPrio ) || (%SearchPrio == 0)) &&
				(%AttackerDistance > %AttackerNominateDistance) && // This one is nearer, if so use it!
				(!Player::isDead($BotFuncs::Attackers[%aiId,%i])))	//Bot Still Alive!
				{
					%AttackerDistance = %AttackerNominateDistance;
					%AttackerId = $BotFuncs::Attackers[%aiId,%i];
					%AttackerPrio = $BotFuncs::Priority[%aiId,%i];
				}
			}
			else	//Looking for Treepoints
			{
				//Just get the next waypoint from the list.
				if (	(%AttackerPrio == 999999) && ($BotFuncs::Attackers[%aiId,%i] != 0) &&
					($BotFuncs::Priority[%aiId,%i] == %SearchPrio ) )
				{
					%AttackerDistance = %AttackerNominateDistance;
					%AttackerId = $BotFuncs::Attackers[%aiId,%i];
					%AttackerPrio = $BotFuncs::Priority[%aiId,%i];
				}

//				if ((%AttackerDistance > %AttackerNominateDistance) &&
//				($BotFuncs::Priority[%aiId,%i] == %SearchPrio) &&
//				(BotTree::CheckLOS(%BotPosition, %AttackerNominatePosition, 200)) )
//				{
//					%AttackerDistance = %AttackerNominateDistance;
//					%AttackerId = $BotFuncs::Attackers[%aiId,%i];
//					%AttackerPrio = $BotFuncs::Priority[%aiId,%i];
//				}
			}
		}
	}
	
	if (%AttackerDistance > %MaxDistance)
		return 0;

	return %AttackerId;
}



// AddAttacker Function
// Params Description
//
// 1. %aiId = Id of Bot Being Attacked
//
// 2. %attackerid = id of attacking object
//
// 3. %DeleteDistance = Distance for checks against which this entry is made for deletion
//
// 4. %Priority = Priority for future searches. Should follow following protocol.
//
//    1 - Used for defence Purposes
//
//    2 - Manual Entry via Command. e.g. User Request
//
//    3 - Help requests from other bots.
//
//    4 - Used for bot Tree Routing
//
// Returns True is added, False if entry already exits.
//

function BotFuncs::AddAttacker(%aiId, %attackerId, %DeleteDistance, %Priority)
{
	if (%aiId != %attackerId)
	{
		%found = False;

		for(%i = 0; ((%i < $BotFuncs::AttackerIndex[%aiId]) && (%found == False)); %i++) // Stop attacker being added to
		{																				// Attack List Twice (Or More !)
			if ($BotFuncs::Attackers[%aiId,%i] == %attackerId)
				%found = True;
		}

// If Attacker in list already

		if (%found == True)
			return False;

// Otherwise Add
		else
		{
		
// If AttackerCount = Index then there are no free slots so do a simple append
			if($BotFuncs::AttackerIndex[%aiId] == $BotFuncs::AttackerIndex[%aiId])
			{
				$BotFuncs::Attackers[%aiId,$BotFuncs::AttackerCount[%aiId]] = %attackerId;
				$BotFuncs::DeleteDistance[%aiId,$BotFuncs::AttackerCount[%aiId]] = %DeleteDistance;
				$BotFuncs::Priority[%aiId,$BotFuncs::AttackerCount[%aiId]] = %Priority;

				$BotFuncs::AttackerCount[%aiId] = $BotFuncs::AttackerCount[%aiId] + 1;
				$BotFuncs::AttackerIndex[%aiId] = $BotFuncs::AttackerIndex[%aiId] + 1;
			}
			else
			{

// Find gap and replace

				%found = False;

				for(%i = 0; ((%i < $BotFuncs::AttackerIndex[%aiId]) && (%found == False)); %i++)
				{
					if($BotFuncs::Attackers[%aiId,%i] == 0)
					{
						%found = True;

						$BotFuncs::Attackers[%aiId,%i] = %attackerId;
						$BotFuncs::DeleteDistance[%aiId,%i] = %DeleteDistance;
						$BotFuncs::Priority[%aiId,%i] = %Priority;
						$BotFuncs::AttackerCount[%aiId] = $BotFuncs::AttackerCount[%aiId] + 1;
					}
				}
			}
		}
	}

	return True;
}



// Deletes an attacker from an attack list. Wicked69

function BotFuncs::DelAttacker(%aiId,%attackerId)
{
	%found = False;

	for(%i = 0; ((%i < $BotFuncs::AttackerIndex[%aiId]) && (%found == False)); %i++)
	{
		if($BotFuncs::Attackers[%aiId,%i] == %attackerId)
		{
			$BotFuncs::Attackers[%aiId,%i] = 0;
			$BotFuncs::AttackerCount[%aiId] = $BotFuncs::AttackerCount[%aiId] - 1;
			%found = True;
		}
	}

	return %found;
}



// Deletes an attacker from all AI attacker lists (Normally used on Player/AI Death). Wicked69

function BotFuncs::DelAttackerFromAll(%attackerId)
{
	for(%x = 0; %x < $AttackerIndexCount; %x++)
	{
		// Remove Entries from Bot Lists. Returns True if Actually Deleted.

		if(BotFuncs::DelAttacker($AttackerIndexIds[%x], %attackerId) == True)
			BotThink::Think($AttackerIndexIds[%x],False);
		
	}
}



// Adds an entry to the Master Attacker Index (Used for Mass Adds, Delete's etc. ). Wicked69

function BotFuncs::AddAttackerIndex(%aiId)
{
// Find out of entry exits already

	%found = False;

	for(%x = 0; ((%x < $AttackerIndexCount) && (%found == False)); %x++)
		if($AttackerIndexIds[%x] == %aiId)
			%found = True;
	if(%found == False)
	{
		$AttackerIndexIds[$AttackerIndexCount] = %aiId;
		$AttackerIndexCount = $AttackerIndexCount + 1;
	}
}


// finds nearest team AI member and returns ID. Uses AI master attack index for list.
// if nearest is father than max distance or AI not found (?) then 0 returned. Wicked69
// Optional parameters
//
// %excludeId = ID of AI not to be found (other than %aiId and alternate team members);
// can be used to find a second team member or to exclude an unhealthy one!
//
// %includeAIType = AI type to be found eg "Medic" or "Guard". Need Work.
//

function BotFuncs::FindNearestTeamAI(%aiId, %mindist, %maxdist, %excludeId, %includeAIType)
{
	%nearestdist = 999999;
	%nearestid = 0;
	%myTeam = Client::getTeam(%aiId);

	%BotPosition = GameBase::getPosition(%aiId);

// Look for AI

	for(%x = 0; %x < $AttackerIndexCount; %x++)
	{

		%nearestnominateid = $AttackerIndexIds[%x];
		%nearestnominatepos = GameBase::getPosition($AttackerIndexIds[%x]);
		%nearestnominatedist = Vector::getDistance(%BotPosition,%nearestnominatepos);
		%nearestnominateteam = Client::getTeam(%nearestnominateid);
		%nearestnominatename = Client::getName(%nearestnominateid);

		// Note: following if statement breaks AI type standard - direct coding!

		if	((%aiId != %nearestnominateid) &&
			 (%excludeId != %nearestnominateid) &&
			 ( %nearestnominatedist >= %mindist) &&
			 ( %nearestnominatedist < %nearestdist) &&
			 (%nearestnominateteam == %myTeam) &&
			 ((%includeAIType == "") || (String::findSubStr(%nearestnominatename, %includeAIType) >= 0)))
		{

			%nearestid = %nearestnominateid;
			%nearestdist = %nearestnominatedist;
		}
	}


// Look for Team Member	

   %numPlayers = getNumClients();
   for(%i = 0; %i < %numPlayers; %i++)
   {
      %id = getClientByIndex(%i);
      if(Client::getTeam(%id) == %MyTeam)
      {

		%nearestnominateid = %id;
		%nearestnominatepos = GameBase::getPosition(%id);
		%nearestnominatedist = Vector::getDistance(%BotPosition,%id);
		%nearestnominateteam = Client::getTeam(%id);
		%nearestnominatename = Client::getName(%id);

		if	((%excludeId != %nearestnominateid) &&
			 ( %nearestnominatedist >= %mindist) &&
			 ( %nearestnominatedist < %nearestdist) &&
			 (%nearestnominateteam == %myTeam))
		{
			%nearestid = %nearestnominateid;
			%nearestdist = %nearestnominatedist;
		}
      }
   }

	if(%nearestdist > %maxdist)
		return 0;

	return %nearestid;
}






function BotFuncs::NumAttackers(%aiId, %prio)
{
	%temp=0;
	for(%i = 0; (%i < $BotFuncs::AttackerIndex[%aiId]); %i++)
	{
		if (($BotFuncs::Attackers[%aiId,%i] != 0) && ($BotFuncs::Priority[%aiId,%i] == %prio))
			%temp++;
	}
return (%temp);
}



// Deletes any targets closer than min distance or dead.


function BotFuncs::TidyAttackerList(%aiId)
{
	%aipos = GameBase::getPosition(%aiId);

	for(%i = 0; (%i < $BotFuncs::AttackerIndex[%aiId]); %i++)
	{
		%PleaseDelete = False;

		if ($BotFuncs::Attackers[%aiId,%i] != 0)
		{
			if(Player::isDead($BotFuncs::Attackers[%aiId,%i]) == True) // Dead, So delete
				%PleaseDelete = True;

			%WordCount = BotThink::getWordCount(%Attackerpos);

			%Attackerpos = $BotFuncs::Attackers[%aiId,%i];

			//dbecho(1, "Wordcount for '" @ %Attackerpos @ "' = " @ %Wordcount );

			// if waypoint supplied then dont convert to waypoint
			if(%Wordcount == 1)
				%attackerpos = GameBase::getPosition($BotFuncs::Attackers[%aiId,%i]);

			%Attackerdist = Vector::getDistance(%aipos,%attackerpos);

			if(%Attackerdist <= $BotFuncs::DeleteDistance[%aiId,%i])
			{
				%PleaseDelete = True;
				%nearest = BotTree::FindNearestTreebyPos(%aipos);
				if (Vector::getDistance(%aipos, $BotTree::T_Pos[%nearest]) <= 4)
				{
					$BotThink::PassedTreepoints++;


					//Bots have to pass 4 treepoints before we consider ourselves "unstuck"
					if ($BotThink::PassedTreepoints>=4)
						$BotThink::StuckTries[%aiId] = 0;

					if ((%nearest>0) && (%nearest!=""))
						$BotThink::LastPassedTreepoint[%aiId] = %nearest;
					$JetToPos[%aiId] = "break";
					if (BotFuncs::NumAttackers(%aiId, 4) <=0)
					{
						$BotThink::LastPoint[%aiId] = True;
					}
				}
				
			}

			if(%PleaseDelete == True)
			{
				$BotFuncs::Attackers[%aiId,%i] = 0;
				$BotFuncs::AttackerCount[%aiId] = $BotFuncs::AttackerCount[%aiId] - 1;
			}
		}
	}
}



// Code to give one AI a repairkit from another. Wicked69
// I like this one!


function BotFuncs::GiveRepairKit(%source,%dest)
{
	%sourcecount = Player::GetItemCount(%source,RepairKit);
	%destcount = Player::GetItemCount(%dest,RepairKit);

// Cheat, Frig or whatever you want to call it. Wicked69

	%sourcecount = 1;

	%destcount = 1;

	Player::SetItemCount(%source,RepairKit, %sourcecount);
	Player::SetItemCount(%dest,RepairKit, %destcount);

	%Team = GameBase::getTeam(%source);
    BotFuncs::TeamMessage(%Team, Client::GetName(%source) @ " gave " @ Client::GetName(%dest) @ " a Health Kit");
}




// IsHealthy - Checks health of Bot!
// Returns boolean based on whether AI is healthy.

function BotFuncs::IsHealthy(%aiId)
{
	%player = Client::getOwnedObject(%aiId);
	%damage = GameBase::getDamageLevel(%player);

	if (%damage >= $Spoonbot::RetreatDamageLevel)
		return False;

	return True;
}


function BotFuncs::TeamMessage(%team, %message)
{
   %numPlayers = getNumClients();
   for(%i = 0; %i < %numPlayers; %i++)
   {
      %id = getClientByIndex(%i);
      if(Client::getTeam(%id) == %team)
      {
         Client::sendMessage(%id, 2, %message);
      }
   }
}


function BotFuncs::checkObstacle(%aiId)  //This function checks for nearby obstacles and makes the AI evade them. By Werewolf.
{                                  //This is by no means perfect. The AI still hangs pretty often.
        %passenger = %aiId;
        %armor = Player::getArmor(%passenger);
	if(%armor == "larmor" || %armor == "lfemale") {
		%height = 10;
		%velocity = 70;
		%zVec = 40;
	}
	else if(%armor == "marmor" || %armor == "mfemale") {
		%height = 15;
		%velocity = 100;
		%zVec = 80;
	}
	else if(%armor == "harmor") {
		%height = 20;
		%velocity = 140;
		%zVec = 100;
	}

	%pos = GameBase::getPosition(Client::getOwnedObject(%passenger));
	%posX = getWord(%pos,0);
	%posY	= getWord(%pos,1);
	%posZ	= getWord(%pos,2);

	%minObstacleDistance = 0.001;      //The minimum distance controls how close objects may get until the bot starts evading.
					   //This value still needs experimenting
	%evasionDistance = 0;

	%rotX = getWord(GameBase::getRotation(Client::getOwnedObject(%passenger)),0);
	%rotY = getWord(GameBase::getRotation(Client::getOwnedObject(%passenger)),1);
	%rotZ = getWord(GameBase::getRotation(%passenger),2);

	%jump = 0;



	//Checking for straight obstacles
	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),(%posX + %minObstacleDistance) @ " " @ %posY @ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}
	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),(%posX - %minObstacleDistance) @ " " @ %posY @ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}


	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),%posX @ " " @ (%posY + %minObstacleDistance) @ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}
	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),%posX @ " " @ (%posY - %minObstacleDistance) @ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}






	//Checking for diagonal obstacles
	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),(%posX + %minObstacleDistance) @ " " @ (%posY + %minObstacleDistance)@ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}
	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),(%posX - %minObstacleDistance) @ " " @ (%posY - %minObstacleDistance) @ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}


	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),(%posX - %minObstacleDistance) @ " " @ (%posY + %minObstacleDistance) @ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}
	if(!GameBase::testPosition(Client::getOwnedObject(%passenger),(%posX + %minObstacleDistance) @ " " @ (%posY - %minObstacleDistance) @ " " @ %posZ)) {	
		AI::RandomEvade(%passenger);
		return;
	}

}	


function BotFuncs::GetTheFlag(%aiName)   //by EMO1313. Makes %aiName to head for the flag and ignore all enemies.
{

if ($Spoonbot::DebugMode)
 echo ("CALL BotFuncs::GetTheFlag("@ %aiName @");");

			%aiId = BotFuncs::GetId( %aiName );
if (%aiId==0)
	return;
			%aiTeam = GameBase::getTeam( %aiId );

			%enemyTeam = 1;           //find out the enemy team's teamnumber
			if (%aiTeam == 1)
			{
			  %enemyTeam = 0;
			}

			%pos = ($teamFlag[%enemyTeam]).originalPosition;
			%xPos = getWord(%pos, 0);
			%yPos = getword(%pos, 1);
			%zPos = getWord(%pos, 2);
			%flagPos = %xPos @ "  " @ %yPos @ "  " @ %zPos;

			AI::DirectiveWaypoint( %aiName, %flagPos, 125);         //Head for the flag.
			AI::DirectiveWaypoint( %aiName, %flagPos, 2);         //Head for the flag.
			AI::SetVar(%aiName, spotDist, 100);
			AI::SetVar(%aiName, seekOff , 1);
			schedule("AI::SetVar(" @ %aiName @ "," @ spotDist @ ", 150);",60);
			schedule("AI::SetVar(" @ %aiName @ "," @ seekOff @ " , 0);",60);
			schedule("AI::DirectiveRemove(" @ %aiName @ ", 125);",60);

			$Spoonbot::BotStatus[%aiId] = "Getting Flag";
			if ($Spoonbot::DebugMode)
			  echo(%aiName @ " tries to capture the enemy's flag"); //on his objective.
			return;
}

function BotFuncs::ReturnTheFlag(%aiName)   //by Werewolf. Makes %aiName to head for the OWN flag and ignore all enemies.
{

if ($Spoonbot::DebugMode)
 echo ("CALL BotFuncs::ReturnTheFlag("@ %aiName @");");

			%aiId = BotFuncs::GetId( %aiName );
if (%aiId==0)
	return;
			%aiTeam = GameBase::getTeam( %aiId );

			%pos = ($teamFlag[%aiTeam]).originalPosition;
			%xPos = getWord(%pos, 0);
			%yPos = getword(%pos, 1);
			%zPos = getWord(%pos, 2);
			%flagPos = %xPos @ "  " @ %yPos @ "  " @ %zPos;

			AI::DirectiveWaypoint( %aiName, %flagPos, 125);         //Head for the flag.
			AI::DirectiveWaypoint( %aiName, %flagPos, 2);         //Head for the flag.
			AI::SetVar(%aiName, spotDist, 100);
			AI::SetVar(%aiName, seekOff , 1);
			schedule("AI::SetVar(" @ %aiName @ "," @ spotDist @ ", 150);",$Spoonbot::ThinkingInterval);
			schedule("AI::SetVar(" @ %aiName @ "," @ seekOff @ " , 0);",$Spoonbot::ThinkingInterval);
			schedule("AI::DirectiveRemove(" @ %aiName @ ", 125);",$Spoonbot::ThinkingInterval);
			schedule("AI::DirectiveRemove(" @ %aiName @ ", 2);",$Spoonbot::ThinkingInterval);

			$Spoonbot::BotStatus[%aiId] = "Returning Flag";
			if ($Spoonbot::DebugMode)
			  echo(%aiName @ " tries to capture the enemy's flag"); //on his objective.
			return;
}


function BotFuncs::checkForFlagrunner(%aiName)
{
if ($Spoonbot::DebugMode)
 echo ("CALL BotFuncs::CheckForFlagrunner("@ %aiName @");");

	%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;

	if (Player::getMountedItem(%aiId, $FlagSlot) != -1)  //if the ai carries a flag, make him run home!!
	{
		if ($Spoonbot::DebugMode)
		  echo ("STATUS BotFuncs::CheckForFlagrunner = Flagrunner " @ %flagrunnerName @ "has the flag");
//		BotFuncs::ReturnTheFlag(%aiName);
		return;
	}


	%T = GameBase::getTeam(%aiId);
	%startCl = 2049;
	%endCl = %startCl + 90;

	for(%cl = %startCl; (%cl < %endCl); %cl++)
	{
		%team = Client::getTeam(%cl);
		%flagrunnerName = Client::getName(%cl);

		if (Player::getMountedItem(%cl, $FlagSlot) != -1)
		{
		  if (%cl != %aiId)
		  {
			  if(%team != %T)	
			  {
				%player = Client::getOwnedObject(%cl);
				if (!Player::isDead(%player))
				{
				        if (%flagrunnerName != "")
					{
					AI::HuntTarget(%aiName, %cl, 1);
					if ($Spoonbot::DebugMode)
					  echo (%aiName @ " has decided to attack enemy flagrunner " @ %flagrunnerName);
					$Spoonbot::BotStatus[%aiId] = "Attacking Flagrunner";
					}

				}
			  }
		          else				    //else this is a friendly, so let's escort him to safety.
		          {
				%player = Client::getOwnedObject(%cl);
		 		if (!Player::isDead(%player))
				{
				        if (%flagrunnerName != "")
					{
				   	AI::HuntTarget(%aiName, %cl, 1);  //The procedure is the same, except the friendly bot won't fire on the flagrunner
					if ($Spoonbot::DebugMode)
				   	  echo(%aiName @ " is escorting the friendly flagrunner " @ %flagrunnerName);
					$Spoonbot::BotStatus[%aiId] = "Escorting Flagrunner";
				        }
		 	        }
		          }
		  }
		}
	}
}



function BotFuncs::getNearestPainter(%aiId)
{
	if ($Spoonbot::DebugMode)
	 echo ("CALL BotFuncs::GetNearestPainter("@ %aiId @");");

	%T = GameBase::getTeam(%aiId);
	%startCl = 2049;
	%endCl = %startCl + 90;
	%nearest = 99999999;
	%nearestPainter = -1;
	%aiPos = GameBase::getPosition(%aiId);
	for(%cl = %startCl; (%cl < %endCl); %cl++)
	{
		%team = Client::getTeam(%cl);
		if(%team == %T)
		{
			if (Player::getMountedItem(%cl,$WeaponSlot) == TargetingLaser)
			{
				%clPos = GameBase::getPosition(%cl);
				%curDist = Vector::getDistance(%aiPos, %clPos);
				if (%curdist<%nearest)
				{
					%nearest = %curdist;
					%nP = %cl;
				}
			}
		}
	}
	$NearestPainter[%aiId] = %nP;
}



function BotFuncs::checkForPaint(%aiName)					// savage
{										// savage
if ($Spoonbot::DebugMode)
 echo ("CALL BotFuncs::CheckForPaint("@ %aiName @");");

  if((String::findSubStr(%aiName, "Guard") >= 0) || (String::findSubStr(%aiName, "Mortar") >= 0))
  {
	%aiId = BotFuncs::GetId(%aiName);						// savage
	$Spoonbot::MortarBusy[%aiId] = 0;
	if (%aiId==0)
		return;
	if (!BotFuncs::CheckAlive(%aiId))
		return;

	if (!BotTree::isOutside(GameBase::getPosition(%aiId)))
		return;

	%T = GameBase::getTeam(%aiId);						// savage
	%startCl = 2049;
	%endCl = %startCl + 90;
//	for(%cl = Client::getFirst(); %cl != -1; %cl = Client::getNext(%cl))	// old loop made mortars fire on human lasers only
	for(%cl = %startCl; (%cl < %endCl); %cl++)				// new loop also reacts on AI targeting lasers.
	{
		%team = Client::getTeam(%cl);					// savage
		if(%team == %T)							// savage
		{								// savage
			if (Player::getMountedItem(%cl,$WeaponSlot) == TargetingLaser)
			{
				if (Vector::getDistance(GameBase::getPosition($Spoonbot::PainterTarget[%cl]), GameBase::getPosition(%aiId)) < 250)
				if (Player::isTriggered(%cl,$WeaponSlot))
				{
					$Spoonbot::BotStatus[%aiId] = "Mortaring";

					Player::setItemCount(%aiId, mortar, 1);
					Player::setItemCount(%aiId, mortarammo, 500);


					Player::mountItem(%aiId, mortar, 0);		// savage
					AI::SetVar(%aiName, triggerPct, 1000 );		// savage


   AI::callWithId(%aiId, Player::setItemCount, Mortar, 1);
	AI::callWithId(%aiId, Player::setItemCount, MortarAmmo, 10000);
   AI::callWithId(%aiId, Player::mountItem, mortar, 0);


					$Spoonbot::MortarBusy[%aiId] = 1;

					AI::DirectiveTargetLaser( %aiName, %cl, 3000 );					
										// let's only harass the painting player
					schedule ("AI::DirectiveRemove(" @ %aiName @ ", 3000 );", 5);
					schedule ("$Spoonbot::MortarBusy[" @ %aiId @ "] = 0;",5);

					Client::sendMessage(%cl, 3, %aiName @ ": Target acquired.~wtgtacq", %aiId);
					Ai::soundHelper( %aiId, %cl, "tgtacq" );
					return;					// First found painter wins
				}
				else
				{
//					AI::DirectiveRemove( %aiName, 3000 );	// Werewolf
//					$Spoonbot::MortarBusy[%aiId] = 0;
					return;
				}
			}							// savage
		}								// savage
	}									// savage
  }
}



function BotFuncs::AddRepairTask(%object)  //by Werewolf. Adds a destroyed object (turret, generator, etc) to the repair queue
{
if ($Spoonbot::DebugMode)
 dbecho (1, "CALL BotFuncs::AddRepairTask(" @ %object @");");

$Spoonbot::NumRepairTasks = $Spoonbot::NumRepairTasks + 1;

for(%task = 0; (%task < 99); %task++)
  {
	if ($Spoonbot::RepairTaskObject[%task] == 0)
	  {
		$Spoonbot::RepairTaskObject[%task] = %object;
		return;
	  }
  }
}




function BotFuncs::CheckIfBusyWithRepairs(%aiName)  //by Werewolf. Only updates a storage variable.
{

%aiId = BotFuncs::GetId( %aiName );
if (%aiId==0)
	return;

%task = -1;

%task = $Spoonbot::MedicTask[%aiId];

 if (%task == -1)
 {
	return 0;
 }
 else 
 {
	return 1;
 }


}




function BotFuncs::SetCurrentRepairTask(%aiName, %task)  //by Werewolf. Only updates a storage variable.
{

if ($Spoonbot::DebugMode)
 echo ("CALL BotFuncs::SetCurrentRepairTask(" @ %aiName @ ", " @ %task @ ");");


%aiId = BotFuncs::GetId( %aiName );
if (%aiId==0)
	return;

$Spoonbot::MedicTask[%aiId] = %task;



}





function BotFuncs::GetRepairTask(%aiName)  //by Werewolf. Returns the current repair Task for a Medic
{
	%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;
	%Team = GameBase::getTeam( %aiId );

	%Current_Repair = 0;
	%Task = -1;

// Generators

	//dbecho(1,"GetRepairTask:: GenCount = " @ $BotFuncs::gencount[%team]);

	for(%x = 0; %x < $BotFuncs::gencount[%team]; %x++)
	{

		//dbecho(1,"GetRepairTask:: Status for " @ $BotFuncs::genlist[%team,%x] @
		//	" = " @ GameBase::getDamageLevel($BotFuncs::genlist[%team,%x]));

		if(GameBase::getDamageLevel($BotFuncs::genlist[%team,%x]) > %current_repair)
		{
			%Task = $BotFuncs::genlist[%team,%x];
			%Current_Repair = GameBase::getDamageLevel($BotFuncs::genlist[%team,%x]);
		}
	}

// If duff generator found then repair - priority

	if (%Task != -1)
		return %Task;

// Sensors

	//dbecho(1,"GetRepairTask:: SensorCount = " @ $BotFuncs::sensorcount[%team]);

	for(%x = 0; %x < $BotFuncs::sensorcount[%team]; %x++)
	{

		//dbecho(1,"GetRepairTask:: Status for " @ $BotFuncs::sensorlist[%team,%x] @
		//	" = " @ GameBase::getDamageLevel($BotFuncs::sensorlist[%team,%x]));

		if(GameBase::getDamageLevel($BotFuncs::sensorlist[%team,%x]) > %current_repair)
		{
			%Task = $BotFuncs::sensorlist[%team,%x];
			%Current_Repair = GameBase::getDamageLevel($BotFuncs::sensorlist[%team,%x]);
		}
	}

// Guns

	//dbecho(1,"GetRepairTask:: GunCount = " @ $BotFuncs::guncount[%team]);

	for(%x = 0; %x < $BotFuncs::guncount[%team]; %x++)
	{

		//dbecho(1,"GetRepairTask:: Status for " @ $BotFuncs::gunlist[%team,%x] @
		//	" = " @ GameBase::getDamageLevel($BotFuncs::gunlist[%team,%x]));

		if(GameBase::getDamageLevel($BotFuncs::gunlist[%team,%x]) > %current_repair)
		{
			%Task = $BotFuncs::gunlist[%team,%x];
			%Current_Repair = GameBase::getDamageLevel($BotFuncs::gunlist[%team,%x]);
		}
	}

// Command Points

	//dbecho(1,"GetRepairTask:: ComCount = " @ $BotFuncs::comcount[%team]);

	for(%x = 0; %x < $BotFuncs::comcount[%team]; %x++)
	{

		//dbecho(1,"GetRepairTask:: Status for " @ $BotFuncs::comlist[%team,%x] @
		//	" = " @ GameBase::getDamageLevel($BotFuncs::comlist[%team,%x]));

		if(GameBase::getDamageLevel($BotFuncs::comlist[%team,%x]) > %current_repair)
		{
			%Task = $BotFuncs::comlist[%team,%x];
			%Current_Repair = GameBase::getDamageLevel($BotFuncs::comlist[%team,%x]);
		}
	}

// Inventory Points

	//dbecho(1,"GetRepairTask:: InvCount = " @ $BotFuncs::invcount[%team]);

	for(%x = 0; %x < $BotFuncs::invcount[%team]; %x++)
	{

		//dbecho(1,"GetRepairTask:: Status for " @ $BotFuncs::invlist[%team,%x] @
		//	" = " @ GameBase::getDamageLevel($BotFuncs::invlist[%team,%x]));

		if(GameBase::getDamageLevel($BotFuncs::invlist[%team,%x]) > %current_repair)
		{
			%Task = $BotFuncs::invlist[%team,%x];
			%Current_Repair = GameBase::getDamageLevel($BotFuncs::invlist[%team,%x]);
		}
	}

// Vehicle Points

	//dbecho(1,"GetRepairTask:: VehCount = " @ $BotFuncs::vehcount[%team]);

	for(%x = 0; %x < $BotFuncs::vehcount[%team]; %x++)
	{

		//dbecho(1,"GetRepairTask:: Status for " @ $BotFuncs::vehlist[%team,%x] @
		//	" = " @ GameBase::getDamageLevel($BotFuncs::vehlist[%team,%x]));

		if(GameBase::getDamageLevel($BotFuncs::vehlist[%team,%x]) > %current_repair)
		{
			%Task = $BotFuncs::vehlist[%team,%x];
			%Current_Repair = GameBase::getDamageLevel($BotFuncs::vehlist[%team,%x]);
		}
	}

	return %Task;
}





function BotFuncs::Animation(%aiId, %animation)  //by Werewolf. Makes a bot wave or salute or anything.
{

// Example usage for animation:
//	%animation = radnomItems(8, $PlayerAnim::Celebration1, $PlayerAnim::Celebration2, $PlayerAnim::Celebration3, $PlayerAnim::Taunt1, $PlayerAnim::Taunt2, $PlayerAnim::Wave, $PlayerAnim::OverHere, $PlayerAnim::Salute);
// See PlayerAnim definitions in Player.cs and ArmorData.cs

//	%player = Client::getControlObject(%aiId);
	%player = %aiId;
	Player::setAnimation(%player, %animation);
}



function BotFuncs::DeleteAllAttackPointsByPrio(%aiId, %Prio)
{
	for(%i = 0; (%i < $BotFuncs::AttackerIndex[%aiId]); %i++)
		if (($BotFuncs::Priority[%aiId,i] == %Prio) || (%Prio == 0))
		{
			$BotFuncs::Attackers[%aiId,%i] = 0;
			$BotFuncs::AttackerCount[%aiId]--;
		}
	if (%Prio == 0)
	{
		$BotFuncs::AttackerCount[%aiId] = 0;
		$BotFuncs::AttackerIndex[%aiId] = 0;
	}
}



// Scans the object tree and inserts all relevant objects into a separate array
// If you want to do such an array, be sure to check that the team id is NOT -1,
// AND the object Name is NOT " " or "False"

function BotFuncs::ScanObjectTree()
{


	if ($Spoonbot::DebugMode)
	dbecho (1, "SCANNING OBJECT TREE");

	$BotFuncs::flagcount[0] = 0;
	$BotFuncs::flagcount[1] = 0;
	$BotFuncs::sensorcount[0] = 0;
	$BotFuncs::sensorcount[1] = 0;
	$BotFuncs::guncount[0] = 0;
	$BotFuncs::guncount[1] = 0;
	$BotFuncs::gencount[0] = 0;
	$BotFuncs::gencount[1] = 0;
	$BotFuncs::comcount[0] = 0;
	$BotFuncs::comcount[1] = 0;
	$BotFuncs::vehcount[0] = 0;
	$BotFuncs::vehcount[1] = 0;
	$BotFuncs::invcount[0] = 0;
	$BotFuncs::invcount[1] = 0;
	$BotFuncs::switchcount = 0;
	$BotFuncs::AllCount = 0;

	for(%object = 8205; (%object <= 9000); %object++)
	{
		%name = GameBase::getDataName(%object);
		%team = GameBase::getTeam(%object);

		if (%name == "flag")
		{
			//dbecho(1,"Found flag for team " @ %team @ ", id = " @ %object );

			$BotFuncs::flaglist[%team,$BotFuncs::flagcount[%team]] = %object;
			$BotFuncs::flagcount[%team] = $BotFuncs::flagcount[%team] + 1;

			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;
		}

		else if ((%name == "PulseSensor") ||
				 (%name == "MediumPulseSensor") ||
				 (%name == "DeployableMotionSensor") ||
				 (%name == "LargeAntenna") ||
				 (%name == "ArrayAntenna") ||
				 (%name == "DeployableSensorJammer") ||
				 (%name == "CameraTurret"))
		{
			//dbecho(1,"Found sensor for team " @ %team @ ", id = " @ %object );

			$BotFuncs::sensorlist[%team,$BotFuncs::sensorcount[%team]] = %object;
			$BotFuncs::sensorcount[%team] = $BotFuncs::sensorcount[%team] + 1;

			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;
		}

		else if ((%name == "PlasmaTurret") ||
				 (%name == "ELFTurret") ||
				 (%name == "IndoorTurret") ||
				 (%name == "RocketTurret") ||
				 (%name == "DeployableTurret"))
		{
			//dbecho(1,"Found gun for team " @ %team @ ", id = " @ %object );

			$BotFuncs::gunlist[%team,$BotFuncs::guncount[%team]] = %object;
			$BotFuncs::guncount[%team] = $BotFuncs::guncount[%team] + 1;

			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;
		}

		else if ((%name == "Generator") || (%name == "SolarPanel"))
		{
			//dbecho(1,"Found generator for team " @ %team @ ", id = " @ %object );

			$BotFuncs::genlist[%team,$BotFuncs::gencount[%team]] = %object;
			$BotFuncs::gencount[%team] = $BotFuncs::gencount[%team] + 1;

			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;
		}

		else if ((%name == "AmmoStation") ||
				 (%name == "InventoryStation") ||
				 (%name == "DeployableInvStation") ||
				 (%name == "DeployableAmmoStation") ||
				 (%name == "VehicleStation"))
		{
			//dbecho(1,"Found station for team " @ %team @ ", id = " @ %object );

			$BotFuncs::invlist[%team,$BotFuncs::invcount[%team]] = %object;
			$BotFuncs::invcount[%team] = $BotFuncs::invcount[%team] + 1;

			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;
		}

		else if (%name == "CommandStation")
		{
			//dbecho(1,"Found command point for team " @ %team @ ", id = " @ %object );

			$BotFuncs::comlist[%team,$BotFuncs::comcount[%team]] = %object;
			$BotFuncs::comcount[%team] = $BotFuncs::comcount[%team] + 1;

			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;
		}

		else if (%name == "vehiclePad")
		{
			//dbecho(1,"Found vehicle point for team " @ %team @ ", id = " @ %object );

			$BotFuncs::vehlist[%team,$BotFuncs::vehcount[%team]] = %object;
			$BotFuncs::vehcount[%team] = $BotFuncs::vehcount[%team] + 1;

			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;
		}

		else if (%name == "DropPointMarker") { }
                else if (%name == "TowerSwitch")
                {
			$BotFuncs::switchlist[$BotFuncs::switchcount] = %object;
			$BotFuncs::switchcount++;
			$BotFuncs::AllList[$BotFuncs::AllCount++] = %object;

		}
		//else if ((%name != "False") && (%name != ""))
		//	dbecho (1, "BotFuncs::ScanObjectTree... Unmapped Object " @ %object @ " Name " @ %name @ " Team " @ %team);
	
	}

	if ($Spoonbot::DebugMode)
	dbecho(1,"Flag count for team 0 = " @ $BotFuncs::flagcount[0]);
	if ($Spoonbot::DebugMode)
	dbecho(1,"Flag count for team 1 = " @ $BotFuncs::flagcount[1]);
}

function BotFuncs::GetFlagId(%team)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Flag Count for team " @ %Team @ " = " @ $BotFuncs::flagcount[%team]);	

	if($BotFuncs::flagcount[%team] == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No flags available for team " @ %team );
		return -1;
	}

	%index = floor(getRandom() * $BotFuncs::flagcount[%team]);

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning flag " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::flaglist[%team,%index]);

	return $BotFuncs::flaglist[%team,%index];
}

function BotFuncs::GetSwitchId(%team, %BotPosition)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Switch Count = " @ $BotFuncs::switchcount);	
	if($BotFuncs::switchcount == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No Switches available");
		return -1;
	}

	%nominee = -1;
	%nearestDistance = 9999999;
	for (%i=0; %i<$BotFuncs::switchcount; %i++)
	{
		%curobj=$BotFuncs::switchlist[%i];
		if (%team == GameBase::getTeam(Group::getObject(GetGroup(%curobj), 0)))
		{
			%distance=Vector::getDistance(%BotPosition,GameBase::getPosition(%curobj));
			if (%distance<%nearestDistance)
			{
				BotTree::FindNearestTreebyPos(GameBase::getPosition(%curobj));
				if (!$LosWarning)
				{
				%nominee = %curobj;
				%nearestDistance = %distance;
				%index=%i;
				}
			}
		}
	}

	%index = floor(getRandom() * $BotFuncs::switchcount[%team]);

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning switch " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::switchlist[%index]);

	return $BotFuncs::switchlist[%index];
}



function BotFuncs::GetSensorId(%team, %BotPosition)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Sensor Count for team " @ %Team @ " = " @ $BotFuncs::sensorcount[%team]);	

	if($BotFuncs::sensorcount[%team] == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No sensors available for team " @ %team );
		return -1;
	}

	%nominee = -1;
	%nearestDistance = 9999999;
	for (%i=0; (%i<$BotFuncs::sensorcount[%team]); %i++)
	{
		%curobj=$BotFuncs::sensorlist[%team,%i];
		%health=GameBase::getDamageLevel(%curobj);
		if (%health<0.7)
		{
			%distance=Vector::getDistance(%BotPosition,GameBase::getPosition(%curobj));
			if (%distance<%nearestDistance)
			{
				BotTree::FindNearestTreebyPos(GameBase::getPosition(%curobj));
				if (!$LosWarning)
				{
				%nominee = %curobj;
				%nearestDistance = %distance;
				%index=%i;
				}
			}
		}
	}

//	%index = floor(getRandom() * $BotFuncs::sensorcount[%team]);

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning sensor " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::sensorlist[%team,%index]);

	return $BotFuncs::sensorlist[%team,%index];
}

function BotFuncs::GetGunId(%team, %BotPosition)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Gun Count for team " @ %Team @ " = " @ $BotFuncs::guncount[%team]);	

	if($BotFuncs::guncount[%team] == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No guns available for team " @ %team );
		return -1;
	}

	%nominee = -1;
	%nearestDistance = 9999999;
	for (%i=0; %i<$BotFuncs::guncount[%team]; %i++)
	{
		%curobj=$BotFuncs::gunlist[%team,%i];
		%health=GameBase::getDamageLevel(%curobj);
		if (%health<0.7)
		{
			%distance=Vector::getDistance(%BotPosition,GameBase::getPosition(%curobj));
			if (%distance<%nearestDistance)
			{
				BotTree::FindNearestTreebyPos(GameBase::getPosition(%curobj));
				if (!$LosWarning)
				{
				%nominee = %curobj;
				%nearestDistance = %distance;
				%index=%i;
				}
			}
		}
	}

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning gun " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::gunlist[%team,%index]);

	return $BotFuncs::gunlist[%team,%index];
}

function BotFuncs::GetInvId(%team, %BotPosition)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Inv Count for team " @ %Team @ " = " @ $BotFuncs::invcount[%team]);	

	if($BotFuncs::invcount[%team] == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No inventory points available for team " @ %team );
		return -1;
	}

	%nominee = -1;
	%nearestDistance = 9999999;
	for (%i=0; %i<$BotFuncs::invcount[%team]; %i++)
	{
		%curobj=$BotFuncs::invlist[%team,%i];
		%health=GameBase::getDamageLevel(%curobj);
		if (%health<0.7)
		{
			%distance=Vector::getDistance(%BotPosition,GameBase::getPosition(%curobj));
			if (%distance<%nearestDistance)
			{
				BotTree::FindNearestTreebyPos(GameBase::getPosition(%curobj));
				if (!$LosWarning)
				{
				%nominee = %curobj;
				%nearestDistance = %distance;
				%index=%i;
				}
			}
		}
	}

//	%index = floor(getRandom() * $BotFuncs::invcount[%team]);

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning inventory " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::invlist[%team,%index]);

	return $BotFuncs::invlist[%team,%index];
}

function BotFuncs::GetGenId(%team, %BotPosition)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Gen Count for team " @ %Team @ " = " @ $BotFuncs::gencount[%team]);	
	if($BotFuncs::gencount[%team] == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No generators available for team " @ %team );
		return -1;
	}

	%nominee = -1;
	%nearestDistance = 9999999;
	for (%i=0; %i<$BotFuncs::gencount[%team]; %i++)
	{
		%curobj=$BotFuncs::genlist[%team,%i];
		%health=GameBase::getDamageLevel(%curobj);
		if (%health<0.7)
		{
			%distance=Vector::getDistance(%BotPosition,GameBase::getPosition(%curobj));
			if (%distance<%nearestDistance)
			{
				BotTree::FindNearestTreebyPos(GameBase::getPosition(%curobj));
				if (!$LosWarning)
				{
				%nominee = %curobj;
				%nearestDistance = %distance;
				%index=%i;
				}
			}
		}
	}

//	%index = floor(getRandom() * $BotFuncs::gencount[%team]);

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning generator " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::genlist[%team,%index]);

	return $BotFuncs::genlist[%team,%index];
}

function BotFuncs::GetComId(%team, %BotPosition)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Com Count for team " @ %Team @ " = " @ $BotFuncs::comcount[%team]);	

	if($BotFuncs::comcount[%team] == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No command points available for team " @ %team );
		return -1;
	}

	%nominee = -1;
	%nearestDistance = 9999999;
	for (%i=0; %i<$BotFuncs::comcount[%team]; %i++)
	{
		%curobj=$BotFuncs::comlist[%team,%i];
		%health=GameBase::getDamageLevel(%curobj);
		if (%health<0.7)
		{
			%distance=Vector::getDistance(%BotPosition,GameBase::getPosition(%curobj));
			if (%distance<%nearestDistance)
			{
				BotTree::FindNearestTreebyPos(GameBase::getPosition(%curobj));
				if (!$LosWarning)
				{
				%nominee = %curobj;
				%nearestDistance = %distance;
				%index=%i;
				}
			}
		}
	}

//	%index = floor(getRandom() * $BotFuncs::comcount[%team]);

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning command point " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::comlist[%team,%index]);

	return $BotFuncs::comlist[%team,%index];
}

function BotFuncs::GetVehId(%team, %BotPosition)
{
	if ($Spoonbot::DebugMode)
		dbecho(1,"Veh Count for team " @ %Team @ " = " @ $BotFuncs::vehcount[%team]);	

	if($BotFuncs::vehcount[%team] == 0)
	{
	if ($Spoonbot::DebugMode)
		dbecho(1," No vehicle points available for team " @ %team );
		return -1;
	}

	%nominee = -1;
	%nearestDistance = 9999999;
	for (%i=0; %i<$BotFuncs::vehcount[%team]; %i++)
	{
		%curobj=$BotFuncs::vehlist[%team,%i];
		%health=GameBase::getDamageLevel(%curobj);
		if (%health<0.7)
		{
			%distance=Vector::getDistance(%BotPosition,GameBase::getPosition(%curobj));
			if (%distance<%nearestDistance)
			{
				BotTree::FindNearestTreebyPos(GameBase::getPosition(%curobj));
				if (!$LosWarning)
				{
				%nominee = %curobj;
				%nearestDistance = %distance;
				%index=%i;
				}
			}
		}
	}

//	%index = floor(getRandom() * $BotFuncs::vehcount[%team]);

	if ($Spoonbot::DebugMode)
	dbecho(1,"Returning vehicle point " @ %index @ " for team " @ %team @ ", " @ $BotFuncs::vehlist[%team,%index]);

	return $BotFuncs::vehlist[%team,%index];
}





function BotFuncs::Attack(%aiId, %object)  //The same as AttackObject, but it expects an ID
{
	%aiName = Client::GetName(%aiId);
	BotFuncs::AttackObject(%aiName, %object);

}

function BotFuncs::AttackObjectFunction(%aiName, %object)   // %object=Obj. ID of the structure (turret, station, etc) to fire at.
{
	%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;
	if ($Spoonbot::DebugMode)
		echo ("CALL BotFuncs::AttackObjectFunction ("@ %aiName @");");

		%target = GameBase::getPosition(%object);
		%damage = GameBase::getDamageLevel(%object);
		%team = GameBase::getTeam(%object);
		%name = GameBase::getDataName(%object);

		if ($Spoonbot::DebugMode)
		{
			echo ("STATUS BotFuncs::AttackObject object = "@ %object );
			echo ("STATUS BotFuncs::AttackObject name = "@ %name );
			echo ("STATUS BotFuncs::AttackObject location = "@ %target );
			echo ("STATUS BotFuncs::AttackObject damage = "@ %damage );
			echo ("STATUS BotFuncs::AttackObject team = "@ %team );
		}


	AI::DirectiveTargetPoint(%aiName, %target, 5000); //was 255
	schedule ("AI::DirectiveRemove(" @ %aiName @ ", 5000 );", 5);
	BotFuncs::CheckIfTargetDestroyed(%aiName, %object);

}

function BotFuncs::AttackObjectCeaseFire(%aiName)
{
	%aiId = BotFuncs::GetId(%aiName);
	if (%aiId==0)
	return;
	$Spoonbot::PainterTarget[%aiId]=-1;
	$Spoonbot::Target[%aiId]=-1;
}


function BotFuncs::CheckIfTargetDestroyed(%aiName, %object)
{
	%damage = GameBase::getDamageLevel(%object);
	if (%damage >= 0.7)		//Rocket Turrets have 0.75 if destroyed, having a 1.0 here resulted in the bots not stopping to attack a destroyed rocket Turret.
	{
		BotFuncs::AttackObjectCeaseFire(%aiName);
		return;
	}

//	if (BotTypes::isPainter(%aiName) == 1)
//	{
//		%aiId = BotFuncs::GetId( %aiName );
//		if (%aiId==0)
//			return;
//		Player::mountItem(%aiId, TargetingLaser, 0);
//		AI::SetVar(%aiName, triggerPct, 1000 );
//	}

	schedule("BotFuncs::CheckIfTargetDestroyed(" @ %aiName @ ", " @ %object @");", 2);
}

function BotFuncs::Delta(%a, %b)	//Returns the difference between two numbers, regardless if positive or negative
{
if (%a<0)	//Make both variables positive
 %a = 0 - %a;
if (%b<0)
 %b = 0 - %b;

if (%a >= %b)
 %diff = %a - %b;
if (%b > %a)
 %diff = %b - %a;

return %diff;
}



function BotFuncs::GetDistanceToFlag(%aiId, %flagId)
{
%curDistance = Vector::getDistance(GameBase::getPosition(%flagId), GameBase::getPosition(Client::getOwnedObject(%aiId)));
return %curDistance;
}



// Added for Tree Viewing - Wicked69

ItemImageData TreePointImage
 {
 shapeFile = "flag";
 //shapeFile = "sensor_small";
 mountPoint = 2;
 mountOffset = { 0, 0.15, -1 };
 mass = 2.5;
 firstPerson = false;
 };



ItemData TreePoint
{
 description = "Tree point vis";
 shapeFile = "flag";
 //shapeFile = "sensor_small";
 className = "TreePoints";
 heading = "dDeployables";
 imageType = TreePointImage;
 shadowDetailMask = 4;
 mass = 1.5;
 elasticity = 0.2;
 price = 600;
 hudIcon = "deployable";
 showWeaponBar = true;
 hiliteOnActive = true;
};

 function TreePoint::onUse(%player,%item) {
 if (Player::getMountedItem(%player,$BackpackSlot) != %item)
 {
  Player::mountItem(%player,%item,$BackpackSlot);
  }
   else { Player::deployItem(%player,%item);
  }
 }

 function TreePoint::onDeploy(%player,%item,%pos)
 {
  if (TreePoint::deployShape(%player,%item))
  {
   Player::decItemCount(%player,%item);
  }
 } 

function TreePoint::DeployShape(%player,%position)
{

 %client = Player::getClient(%player);
 %rot = GameBase::getRotation(%player);
 %dobj = newObject("","StaticShape",TreePointShape,true);
 addToSet("MissionCleanup", %dobj);
 GameBase::setTeam(%dobj,GameBase::getTeam(%player));

 GameBase::setPosition(%dobj,%position);
 GameBase::setRotation(%dobj,%rot);
 Gamebase::setMapName(%dobj,"Tree Point");
 GameBase::startFadeIn(%dobj);
 return true;
 }



ItemImageData TreePointRouteImage
 {
 shapeFile = "sensor_small";
 mountPoint = 2;
 mountOffset = { 0, 0.15, -1 };
 mass = 2.5;
 firstPerson = false;
 };



ItemData TreePointRoute
{
 description = "Tree Point Route vis";
 shapeFile = "flag";
 shapeFile = "sensor_small";
 className = "TreePointRoutes";
 heading = "dDeployables";
 imageType = TreePointRouteImage;
 shadowDetailMask = 4;
 mass = 1.5;
 elasticity = 0.2;
 price = 600;
 hudIcon = "deployable";
 showWeaponBar = true;
 hiliteOnActive = true;
};

 function TreePointRoute::onUse(%player,%item) {
 if (Player::getMountedItem(%player,$BackpackSlot) != %item)
 {
  Player::mountItem(%player,%item,$BackpackSlot);
  }
   else { Player::deployItem(%player,%item);
  }
 }

 function TreePoint::onDeploy(%player,%item,%pos)
 {
  if (TreePointRoute::deployShape(%player,%item))
  {
   Player::decItemCount(%player,%item);
  }
 } 


function TreePointRoute::DeployShape(%player,%position)
{

 %client = Player::getClient(%player);
 %rot = GameBase::getRotation(%player);
 %dobj = newObject("","StaticShape",TreePointRouteShape,true);
 addToSet("MissionCleanup", %dobj);
 GameBase::setTeam(%dobj,GameBase::getTeam(%player));

 GameBase::setPosition(%dobj,%position);
 GameBase::setRotation(%dobj,%rot);
 Gamebase::setMapName(%dobj,"Tree Point Route");
 GameBase::startFadeIn(%dobj);
 return true;
 }

 
function BotFuncs::BotsHopOn(%client)
{
 	if ($Spoonbot::DebugMode)
		echo ("CALL BotFuncs::BotsHopOn ("@ %client @");");
   %DriverName = Client::getName(%client);
   %DriverPos = GameBase::getPosition(%client);
   %startCl = 2049;
   %endCl = %startCl + 90;
   for(%cl = %startCl; %cl < %endCl; %cl = %cl + 1)
       if (Player::isAIControlled(%cl)) //Is this a bot?
       {
	%DriverTeam = Client::getTeam(%client);
	%AiTeam = Client::getTeam(%cl);
	if (%DriverTeam==%AiTeam)
	{
		%aiName = Client::getName(%cl);
		%AiPos = GameBase::getPosition(%cl);
		%Distance = Vector::getDistance(%AiPos,%DriverPos) ;
 	if ($Spoonbot::DebugMode)
		echo ("STATUS BotFuncs::BotsHopOn Driver "@ %DriverName @" position " @ %DriverPos );
 	if ($Spoonbot::DebugMode)
		echo ("STATUS BotFuncs::BotsHopOn Bot "@ %aiName @" position " @ %AiPos );
 	if ($Spoonbot::DebugMode)
		echo ("STATUS BotFuncs::BotsHopOn Distance between "@ %aiName @" and Driver " @ %client @ " is " @ %Distance );

		if (%Distance <= 200)  				//is bot within 200 units of human Driver?
		{
 	if ($Spoonbot::DebugMode)
		echo ("STATUS BotFuncs::BotsHopOn BOT "@ %aiName @" NOW HOPS ON" );

			BotFuncs::DeleteAllAttackPointsByPrio(%cl, 4);
			BotFuncs::AddAttacker(%cl, %client, 1, 3);

//			AI::DirectiveFollow(%AiName, %client, 0, 1024);

		$BotThink::Definitive_Attackpoint[%cl] = "";
		$BotThink::Definitive_Attackpos[%cl] = "";
		$BotThink::ForcedOfftrack[%aiId] = false;
		$BotThink::LastPoint[%cl] = false;

		$Spoonbot::RidingWith[%cl] = %client;	//Bot %cl is now riding together with human Driver %client
		}
	}
       }
}

function BotsHopOff(%client)
{
 	if ($Spoonbot::DebugMode)
		echo ("CALL BotFuncs::BotsHopOff ("@ %client @");");
   %DriverName = Client::getName(%client);
   %DriverPos = GameBase::getPosition(%client);
   %startCl = 2049;
   %endCl = %startCl + 90;
   for(%cl = %startCl; %cl < %endCl; %cl = %cl + 1)
       if (Player::isAIControlled(%cl)) //Is this a bot?
       {
	%Driver = $Spoonbot::RidingWith[%cl];
	if (%Driver == %client)			//Is the bot riding with the human Driver %client ?
	{
		%aiName = Client::getName(%cl);
		Vehicle::passengerJump(0,Client::getOwnedObject(%cl),0);	//Yes, so bail out.
		BotFuncs::DeleteAllAttackPointsByPrio(%cl, 3);

//		AI::DirectiveRemove(%AiName, 1024);

		$BotThink::Definitive_Attackpoint[%cl] = "";
		$BotThink::Definitive_Attackpos[%cl] = "";
		$BotThink::ForcedOfftrack[%aiId] = true;
		$BotThink::LastPoint[%cl] = True;

		$Spoonbot::RidingWith[%cl]=0;
	}
       }
}

function BotFuncs::CheckForLOS(%aiId, %object)
{
return BotTree::CheckLOS(GameBase::getPosition(%aiId), GameBase::getPosition(%object), 200);
}

function BotFuncs::CheckForItemLOS(%aiId, %object)
{
   %Marker = GameBase::getPosition(%object);
if (BotTree::CheckLOS(GameBase::getPosition(%aiId), GameBase::getPosition(%object), 500))
	return True;

%upMarker = Vector::Add(%Marker, "0 0 0.5");
if (BotTree::CheckLOS(GameBase::getPosition(%aiId), %upMarker, 500))
	return True;
%upMarker = Vector::Add(%Marker, "0 0 1");
if (BotTree::CheckLOS(GameBase::getPosition(%aiId), %upMarker, 500))
	return True;
%upMarker = Vector::Add(%Marker, "0 0 1.5");
if (BotTree::CheckLOS(GameBase::getPosition(%aiId), %upMarker, 500))
	return True;

return False;
}

function BotFuncs::PaintTarget(%aiName, %object)
{
%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;

	if ((!BotFuncs::CheckForItemLOS(%aiId, %object)) && (!BotFuncs::CheckForLOS(%aiId, %object))) //No LOS, so move towards target
	{
		$Spoonbot::BotStatus[%aiId] = "Approach "@ GameBase::getDataName(%object);

		Player::mountItem(%aiId, $Spoonbot::PainterClose, 0);
		AI::SetVar(%aiName, triggerPct, 1000 );

		BotTree::Getmetopos(%aiId,GameBase::GetPosition(%object), false);
		$BotThink::Definitive_Attackpoint[%aiId] = %object;
		$BotThink::Definitive_Attackpos[%aiId] = GameBase::GetPosition(%object);
		$Spoonbot::MedicBusy[%aiId] = 0;
	} else				//We have LOS, so delete previous move command
	{
		BotTree::Stop(%aiId);
		$Spoonbot::BotStatus[%aiId] = "Painting "@ GameBase::getDataName(%object);
		Player::mountItem(%aiId, TargetingLaser, 0);
		AI::SetVar(%aiName, triggerPct, 1000 );
		BotFuncs::AttackObjectFunction(%aiName, %object);	//Attack Target
		BotFuncs::CheckIfTargetDestroyed(%aiName, %object);	//Cease Fire if destroyed
		$Spoonbot::MedicBusy[%aiId] = 1;			//Hack to make the stuck-detection code stop working while painter is busy.
	}
}

function BotFuncs::AttackObject(%aiName, %object)   // %object=Obj. ID of the structure (turret, station, etc) to fire at.
{
	%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;
	if ($Spoonbot::DebugMode)
		echo ("CALL BotFuncs::AttackObject ("@ %aiName @");");
	if ($Spoonbot::DebugMode)
		echo ("BotFuncs::AttackObject Target="@ %object);
	if ($Spoonbot::DebugMode)
		echo ("BotFuncs::AttackObject Target Position="@ GameBase::GetPosition(%object));

	%distance=Vector::getDistance(GameBase::GetPosition(%aiId), GameBase::GetPosition(%object));

	if (!BotFuncs::CheckForItemLOS(%aiId, %object)) //No LOS, so move towards target
	{
		$Spoonbot::BotStatus[%aiId] = "Attacking " @ GameBase::getDataName(%object);
                if ($BotThink::Definitive_Attackpoint[%aiId] != %object)
			BotTree::Getmetopos(%aiId,GameBase::GetPosition(%object), false);
		$BotThink::Definitive_Attackpoint[%aiId] = %object;
		$BotThink::Definitive_Attackpos[%aiId] = GameBase::GetPosition(%object);
	}
	else 		//We have LOS, so delete previous move command
	{

		//Switch to plasma if we're too close (i.e. most likely inside a building)
		if ((BotTypes::isDemo(%aiName)) && (%distance <= 30))
			Player::mountItem(%aiId, $Spoonbot::DemoClose, 0);
		$Spoonbot::BotStatus[%aiId] = "Firing";
		BotFuncs::AttackObjectFunction(%aiName, %object);		//Attack Target
		BotFuncs::CheckIfTargetDestroyed(%aiName, %object);	//Cease Fire if destroyed
	}


}

function BotFuncs::CheckAlive(%aiId)
{
if (%aiId==0)
	return false;
if (%aiId==-1)
	return false;

if ($Spoonbot::BotStatus[%aiId] == "Dead")
	return false;

%player = Client::getOwnedObject(%aiId);

if (Player::isDead(%player))
    {
		return false;
    }

    %aiName = Client::getName(%aiId);

//The following needs to be done in case
//IsDead doesn't work in this version

    %newId = BotFuncs::GetId(%aiName);
    if (%newId != %aiId)
    {
		return false;
    }
    if (%newId == 0)
    {
		return false;
    }

return true;
}
