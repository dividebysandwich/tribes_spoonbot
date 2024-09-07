

//This function enables the bots to cap the flag even though they're not exactly touching it.

function BotMove::CheckForCappedFlags(%aiId)
{

	%aiName = Client::getName(%aiId);
	%aiTeam = Client::getTeam(%aiId);
	%AiPos = GameBase::getPosition(%aiId);

	if(%aiTeam == 0)
		%EnemyTeam = 1;
	else
		%EnemyTeam = 0;


%player = Client::getOwnedObject(%aiId);

%ourflag = BotFuncs::GetFlagId(%aiTeam);
%theirflag = BotFuncs::GetFlagId(%EnemyTeam);

%ourflagpos = GameBase::GetPosition(%ourflag);
%theirflagpos = GameBase::GetPosition(%theirflag);

if (Vector::getDistance(%AiPos, %ourflagpos) < 5 )
	Flag::onCollision(%ourflag, %player);


if (Vector::getDistance(%AiPos, %theirflagpos) < 5 )
	Flag::onCollision(%theirflag, %player);

}







function BotMove::Move(%aiId)
{


BotFuncs::TidyAttackerList(%aiId);

	if ($Spoonbot::DebugMode)
		echo ("CALL BotMove::Move("@ %aiId @ ");");


//Sanity check. Check if Bot is alive. If not then clear Vars and abort functiom
//----------------------------------------
//Werewolf

if ($Spoonbot::BotStatus[%aiId] == "Dead")
	return;

%player = Client::getOwnedObject(%aiId);

if (Player::isDead(%player))
	{
		BotFuncs::ClearVars(%aiId);
		return;
	}

    %aiName = Client::getName(%aiId);

//The following needs to be done in case
//IsDead doesn't work in this version

    %newId = BotFuncs::GetId(%aiName);
if (%newId==0)
	return;
    if (%newId != %aiId)
    {
		BotFuncs::ClearVars(%aiId);
		return;
    }


	//If we were damaged recently, jet to make it harder for the enemy
	if (GameBase::getDamageLevel(Client::getOwnedObject(%aiId)) != $OldDamage[%aiId])
	{
		if ($Spoonbot::BotJetting[%aiId] != 1)
			AI::JetSimulation(%aiId, 0);
		$OldDamage[%aiId] = GameBase::getDamageLevel(Client::getOwnedObject(%aiId));
	}



if (BotTypes::isCMD(%aiName) == 1)
	return;



// Generic variables

	%aiName = Client::getName(%aiId);
	%aiTeam = Client::getTeam(%aiId);
	%AiPos = GameBase::getPosition(%aiId);

	if(%aiTeam == 0)
		%EnemyTeam = 1;
	else
		%EnemyTeam = 0;



BotMove::CheckForCappedFlags(%aiId);

if ($Spoonbot::PainterTarget[%aiId]!=-1)
{
	AI::SetVar(%aiName, triggerPct, 1000 );
	BotFuncs::PaintTarget(%aiName, $Spoonbot::PainterTarget[%aiId]);
}

if (($Spoonbot::MedicBusy[%aiId] == 1 ) && (BotTypes::IsMedic(%aiName) == 1))
{
	playSound(SoundBotRepairItem ,GameBase::getPosition(%aiId));
}



if (($Spoonbot::MortarBusy[%aiId] == 1 ) || ($Spoonbot::MedicBusy[%aiId] == 1))
{
  schedule("BotMove::Move(" @ %aiId @ " );", 5);
  return;
}




BotPilot::Check(%aiId);




//
//
// Bot "stuck" check
//
//
	if ($Spoonbot::DebugMode)
		echo ("Status BotMove::Move = StuckCheck");

// Now we're checking of the AI is stuck somewhere by comparing the actual position with the previous one.
// Then we either issue a RandomEvade to try to solve this problem, or use the nearest treepoint and go from there.

	%xPos = getWord(%AiPos, 0);
	%yPos = getWord(%AiPos, 1);
	%zPos = getWord(%AiPos, 2);
	%LastxPos = getWord($Spoonbot::lastPosition[%aiId], 0);
	%LastyPos = getWord($Spoonbot::lastPosition[%aiId], 1);
	%LastzPos = getWord($Spoonbot::lastPosition[%aiId], 2);

	if ($Spoonbot::DebugMode)
		echo("PREVIOUS POSITION of ID " @ %aiId @ " Name " @ %aiName @ " is " @ $Spoonbot::lastPosition[%aiId]);
	if ($Spoonbot::DebugMode)
		echo("ACTUAL POSITION of ID " @ %aiId @ " Name " @ %aiName @ " is " @ %AiPos);
	if ($Spoonbot::DebugMode)
		echo("STUCK GRACE = " @ $Spoonbot::StuckGracePeriod[%aiId]);

	%stuck = False;
	%MinPositionDelta = 1;
	%MaxGracePeriod = 5;

	if (BotFuncs::Delta(%xPos, %LastxPos) < %MinPositionDelta)
		if (BotFuncs::Delta(%yPos, %LastyPos) < %MinPositionDelta)
			if (BotFuncs::Delta(%zPos, %LastzPos) < %MinPositionDelta)  //If the bot hasn't moved recently, it may be stuck.
				%stuck = True;

	
	if (!%stuck)
		$Spoonbot::StuckGracePeriod[%aiId] = 0;


	if ((%stuck) && (!$BotThink::LastPoint[%aiId]))
	{

	    $Spoonbot::StuckGracePeriod[%aiId]++;

	    if (($Spoonbot::StuckGracePeriod[%aiId] == 3) && ($Spoonbot::BotJettingHeat[%aiId] == 1) && ($BotThink::StuckTries[%aiId]<3))
		{
				$BotThink::PassedTreepoints=0;
				$BotThink::StuckTries[%aiId]++;
				$BotThink::ForcedOfftrack[%aiId] = True;
				$Spoonbot::StuckGracePeriod[%aiId] = 0;
				$Spoonbot::BotJettingHeat[%aiId] = 0;
				$JetToPos[%aiId] = "break";
		}

	    if ($Spoonbot::StuckGracePeriod[%aiId] >= %MaxGracePeriod)
	    {

			$Spoonbot::StuckGracePeriod[%aiId] = 0;

			if($Spoonbot::MedicBusy[%aiId] == 0)
			{

//				%nearest = BotTree::FindNearestTreebyPos(GameBase::GetPosition(%aiId));
//				if (%nearest != -1)	//Get to the nearest visible treepoint
//				{
//					if($Spoonbot::BotJettingHeat[%aiId]==0)
//						JetToPos(%aiId, $BotTree::T_Pos[%nearest]);
//					AI::RandomEvade(%aiId);
//				}
//				else	//we're getting desperate, so attack anything in sight
//				{



		  %foundTarget=False;

		  if (($Spoonbot::AlreadyLookedForTargets[%aiId] == False) && (!((Player::getMountedItem(%player, $FlagSlot) != -1) || (Player::getMountedItem(%aiId, $FlagSlot) != -1))) )
			//If bot is stuck, check for visible nearby targets and attack them.
		  {

			if ($Spoonbot::DebugMode)
				dbecho(1,"Searching for new targets in " @ $BotFuncs::AllCount);

			%foundTarget=False;
			for(%object = 0; (%object <= $BotFuncs::AllCount); %object++)
			{
				%potentialTarget = $BotFuncs::AllList[%object];
				%PTname = GameBase::getDataName(%potentialTarget);
				%PTteam = GameBase::getTeam(%potentialTarget);
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"potentialTarget= "@ %potentialTarget);
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"PTname= "@ %PTname);
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"PTteam= "@ %PTteam);
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"LOS is "@ BotFuncs::CheckForItemLOS(%aiId, %potentialTarget));
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"Distance= "@ Vector::getDistance(GameBase::getPosition(%aiId), GameBase::getPosition(%potentialTarget)));

				if ( (BotFuncs::CheckForItemLOS(%aiId, %potentialTarget) ) && ( Vector::getDistance(GameBase::getPosition(%aiId), GameBase::getPosition(%potentialTarget)) <= 40 ) && (%foundTarget==false))
				if ((%PTname == Generator) && (%enemyteam == %PTteam))
				{
					if (%potentialTarget!=0)
					{
					if ($Spoonbot::DebugMode)
						dbecho(1,"Found new target: GENERATOR!");
					  $Spoonbot::Target[%aiId]=%potentialTarget;
					  BotFuncs::AttackObject(%aiName, %potentialTarget);
					  %objectpos = GameBase::getPosition(%potentialTarget);
					  $BotThink::ForcedOfftrack[%aiId] = true;
					  BotTree::Getmetopos(%aiid,%objectpos, false);
					  $BotThink::Definitive_Attackpoint[%aiId] = %potentialTarget;
					  %foundTarget=true;
					}
				}
				else if ((%PTname == VehicleStation) && (%enemyteam == %PTteam))
				{
					if (%potentialTarget!=0)
					{
					if ($Spoonbot::DebugMode)
						dbecho(1,"Found new target: VEHICLE STATION!");
					  $Spoonbot::Target[%aiId]=%potentialTarget;
					  BotFuncs::AttackObject(%aiName, %potentialTarget);
					  %objectpos = GameBase::getPosition(%potentialTarget);
					  $BotThink::ForcedOfftrack[%aiId] = true;
					  BotTree::Getmetopos(%aiid,%objectpos, false);
					  $BotThink::Definitive_Attackpoint[%aiId] = %potentialTarget;
					  %foundTarget=true;
					}
				}
				else if ((%PTname == VehiclePad) && (%enemyteam == %PTteam))
				{
					if (%potentialTarget!=0)
					{
					if ($Spoonbot::DebugMode)
						dbecho(1,"Found new target: VEHICLE PAD!");
					  $Spoonbot::Target[%aiId]=%potentialTarget;
					  BotFuncs::AttackObject(%aiName, %potentialTarget);
					  %objectpos = GameBase::getPosition(%potentialTarget);
					  $BotThink::ForcedOfftrack[%aiId] = true;
					  BotTree::Getmetopos(%aiid,%objectpos, false);
					  $BotThink::Definitive_Attackpoint[%aiId] = %potentialTarget;
					  %foundTarget=true;
					}
				}
				else if ((%PTname == InventoryStation) && (%enemyteam == %PTteam))
				{
					if (%potentialTarget!=0)
					{
					if ($Spoonbot::DebugMode)
						dbecho(1,"Found new target: INVENTORY STATION!");
					  $Spoonbot::Target[%aiId]=%potentialTarget;
					  BotFuncs::AttackObject(%aiName, %potentialTarget);
					  %objectpos = GameBase::getPosition(%potentialTarget);
					  $BotThink::ForcedOfftrack[%aiId] = true;
					  BotTree::Getmetopos(%aiid,%objectpos, false);
					  $BotThink::Definitive_Attackpoint[%aiId] = %potentialTarget;
					  %foundTarget=true;
					}
				}
				else if ((%PTname == CommandStation) && (%enemyteam == %PTteam))
				{
					if (%potentialTarget!=0)
					{
					if ($Spoonbot::DebugMode)
						dbecho(1,"Found new target: COMMAND STATION!");
					  $Spoonbot::Target[%aiId]=%potentialTarget;
					  BotFuncs::AttackObject(%aiName, %potentialTarget);
					  %objectpos = GameBase::getPosition(%potentialTarget);
					  $BotThink::ForcedOfftrack[%aiId] = true;
					  BotTree::Getmetopos(%aiid,%objectpos, false);
					  $BotThink::Definitive_Attackpoint[%aiId] = %potentialTarget;
					  %foundTarget=true;
					}
				}
				else if ((%PTname == AmmoStation) && (%enemyteam == %PTteam))
				{
					if (%potentialTarget!=0)
					{
					if ($Spoonbot::DebugMode)
						dbecho(1,"Found new target: AMMO STATION!");
					  $Spoonbot::Target[%aiId]=%potentialTarget;
					  BotFuncs::AttackObject(%aiName, %potentialTarget);
					  %objectpos = GameBase::getPosition(%potentialTarget);
					  $BotThink::ForcedOfftrack[%aiId] = true;
					  BotTree::Getmetopos(%aiid,%objectpos, false);
					  $BotThink::Definitive_Attackpoint[%aiId] = %potentialTarget;
					  %foundTarget=true;
					}
				}
				else if ((%PTname == DeployableTurret) && (%enemyteam == %PTteam))
				{
					if (%potentialTarget!=0)
					{
					if ($Spoonbot::DebugMode)
						dbecho(1,"Found new target: DEPLOYABLE TURRET!");
					  $Spoonbot::Target[%aiId]=%potentialTarget;
					  BotFuncs::AttackObject(%aiName, %potentialTarget);
					  %objectpos = GameBase::getPosition(%potentialTarget);
					  $BotThink::ForcedOfftrack[%aiId] = true;
					  BotTree::Getmetopos(%aiid,%objectpos, false);
					  $BotThink::Definitive_Attackpoint[%aiId] = %potentialTarget;
					  %foundTarget=true;
					}
				}
			}

			if (%foundTarget == True)
			{
				$Spoonbot::AlreadyLookedForTargets[%aiId] = False;
				$Spoonbot::StuckGracePeriod[%aiId] = -10;
				$JetToPos[%aiId]="break";
				schedule ("$JetToPos[%aiId]=\"\"", 0.3);
				$BotThink::ForcedOfftrack[%aiId] = True;
			} else {
				$Spoonbot::AlreadyLookedForTargets[%aiId] = True;
				$Spoonbot::StuckGracePeriod[%aiId] = %MaxGracePeriod;
			}
		  }
		}


		if (($Spoonbot::AlreadyLookedForTargets[%aiId] == True) || (%foundTarget == False))
		{

				$Spoonbot::AlreadyLookedForTargets[%aiId] = False;
				if ($Spoonbot::DebugMode)
					echo("STUCK ID " @ %aiId @ " Name " @ %aiName @ " at " @ %AiPos);
				$BotThink::ForcedOfftrack[%aiId] = True;
				$BotThink::StuckTries[%aiId]=0;
				$Spoonbot::StuckGracePeriod[%aiId] = 0;
				$Spoonbot::BotJettingHeat[%aiId] = 0;
				$JetToPos[%aiId] = "break";
				if ($Spoonbot::UseTreefiles == False)
					AI::RandomEvade(%aiId);
				else if ($BotFuncs::AttackerCount[%aiId]!=0)
					WarpMyAss(%aiId, BotFuncs::NearestAttacker(%aiId, 99999, 4));

				
		}

//				} //End of emergency "look for target" code
	      }
	}

	$Spoonbot::lastPosition[%aiId] = %AiPos;









// When JetToPos is in use we don't need extra cpu overhead of the BotMove::Move function.
if($Spoonbot::BotJettingHeat[%aiId]== 1)
{
  schedule("BotMove::Move(" @ %aiId @ " );", $Spoonbot::MovementInterval);
  return;
}
// Nor do we want redundant calls of JetToPos pushed on the stack.












	if ($Spoonbot::DebugMode)
	{
		echo ("STATUS BotMove::AttackerCount = " @ $BotFuncs::AttackerCount[%aiId]);
		echo ("STATUS BotMove::NearestAttacker= " @ BotFuncs::NearestAttacker(%aiId, 99999, 4));
		echo ("STATUS BotMove LOS to Attacker = " @ BotFuncs::CheckForLOS(%aiId, BotFuncs::NearestAttacker(%aiId, 99999, 4)));
	}





	//What? No waypoints at all?? Rethink immediately!
	if ($BotFuncs::AttackerCount[%aiId]==0)
	{
		$hasFlag[%aiId]=False;
		$BotThink::ForcedOfftrack[%aiId] = True;
		$CurrentTargetPos[%aiId] = 0;
		BotThink::Think(%aiId,False);
	}






// Tree point movement. That means that we have to walk them off step by step.

	%AIRequest = BotFuncs::NearestAttacker(%aiId, 99999, 4);

	if (%AIRequest != 0)
	{

		if ($Spoonbot::DebugMode)
			echo ("STATUS BotMove::Move = Tree point movement");

		%nearest = BotTree::FindNearestTreebyPos(GameBase::GetPosition(%aiId));

		if ($Spoonbot::DebugMode)
			echo ("STATUS BotMove::Move Nearest Treepoint = " @ %nearest);

		if ($BotThink::Definitive_Attackpoint[%aiId]==-1)
		{
			$Spoonbot::PainterTarget[%aiId]=-1;
			$Spoonbot::Target[%aiId]=-1;
		}
		if($BotThink::ForcedOfftrack[%aiId])	// If forced off track by enemy intervention
							// Attacked, weak etc then we must recalculate
							// our destination.
		{

			// Clear current treepoint route entries. Were
			// Potentially lost and the current route could
			// be invalid!

			BotFuncs::DeleteAllAttackPointsByPrio(%aiId, 0);
			if ($Spoonbot::DebugMode)
				echo ("Forced off track!");

			BotTree::Getmetopos(%aiid,$BotThink::Definitive_Attackpos[%aiId], false);
//			$BotThink::ForcedOfftrack[%aiId] = false;
//			$BotThink::LastPoint[%aiId] = false;

		}
		else
		{
			if ($Spoonbot::DebugMode)
				echo ("Treefile Movement code running...");

//			%Distance = Vector::getDistance(GameBase::getPosition(%aiId), GameBase::getPosition($BotThink::Definitive_Attackpoint[%aiId]));
//			%Distance2 = Vector::getDistance(GameBase::getPosition(%aiId), %AIRequest);
//			if ($Spoonbot::DebugMode)
//			echo("SELF GOAL MOVE " @ %aiid @ " team = " @ %aiteam @ ", enemyteam = " @ %enemyteam @ ", attackpoint = " @ $BotThink::Definitive_Attackpoint[%aiId] @ ", attackpoint team = " @ GameBase::getTeam($BotThink::Definitive_Attackpoint[%aiId]));
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"Distance to tree point (" @ %AIRequest @ ") = " @ %Distance2 );
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"Distance to target (" @ $BotThink::Definitive_Attackpoint[%aiId] @ ") = " @ %Distance );
//			if ($Spoonbot::DebugMode)
//			dbecho(1,"Is Last Point? " @ $BotThink::LastPoint[%aiId]);



		if (BotFuncs::CheckForItemLOS(%aiId, $BotThink::Definitive_Attackpoint[%aiId]))
		{
			if ($Spoonbot::DebugMode)
				dbecho(1,"LOS found, attacking... " @ $BotThink::Definitive_Attackpoint[%aiId]);

			if (($BotThink::LastPoint[%aiId] == False) && (%enemyteam == GameBase::getTeam($BotThink::Definitive_Attackpoint[%aiId])))
			{
				if ($Spoonbot::DebugMode)
					echo("Target Object Type: " @ getObjectType($BotThink::Definitive_Attackpoint[%aiId]));
				if (GameBase::getDataName($BotThink::Definitive_Attackpoint[%aiId])=="flag")
				{
					AI::DirectiveRemove(%aiName, 1024);
					AI::DirectiveWaypoint(%aiName, %AIRequest, 1024);
				}
				else
					BotFuncs::Attack(%aiId, $BotThink::Definitive_Attackpoint[%aiId]);
			}
			else
			{
				AI::DirectiveRemove(%aiName, 1024);
				AI::DirectiveWaypoint(%aiName, %AIRequest, 1024);
			}

//			if (($BotThink::LastPoint[%aiId] == False) && (%aiteam == GameBase::getTeam($BotThink::Definitive_Attackpoint[%aiId])))
//			{
//				AI::DirectiveWaypoint(%aiName, GameBase::getPosition($BotThink::Definitive_Attackpoint[%aiId]), 1024);
//			}

		}
		else 
		{
			if ($Spoonbot::DebugMode)
			dbecho(1,"No LOS found. Following next treepoint ");
			AI::DirectiveRemove(%aiName, 1024);
			AI::DirectiveWaypoint(%aiName, %AIRequest, 1024);
		}


			if((%Distance < 2) && ($BotThink::LastPoint[%aiId] == True) && ($Spoonbot::MedicBusy[%aiId]==0)) // reached target, now look for next target
			{
				$BotThink::Definitive_Attackpoint[%aiId] = -1;
				$BotThink::ForcedOfftrack[%aiId] = False;
			}


// New code by Dewy
			%TargPosX = getWord(%AIRequest,0);
			%TargPosY = getWord(%AIRequest,1);
			%TargPosZ = getWord(%AIRequest,2);
			%BotPos = GameBase::getPosition(%aiId);
			%BotPosX = getWord(%BotPos,0);
			%BotPosY = getWord(%BotPos,1);
			%BotPosZ = getWord(%BotPos,2);
			%dist = Vector::getDistance(%AIRequest, %BotPos);
			%distX = Vector::getDistance(%TargPosX, %BotPosX);
			%distY = Vector::getDistance(%TargPosY, %BotPosY);
			%distZ = Vector::getDistance(%TargPosZ, %BotPosZ);

			if(%distX <= %distZ*1.5)
			{
				if(%distY <= %distZ*1.5)
				{
						if ($Spoonbot::DebugMode)
						echo("Calling: JetToPos("@%aiId@", "@%AIRequest@");");
						JetToPos(%aiId, %AIRequest);
				}
			}
	
	}


	schedule("BotMove::Move(" @ %aiId @ " );", $Spoonbot::MovementInterval);
	return;
    }


// User Requested Move

	else if (BotFuncs::NearestAttacker(%aiId, 500, 2) != 0)
	{

		if ($Spoonbot::DebugMode)
			echo ("STATUS BotMove::Move = User requested move");

		$BotThink::ForcedOfftrack[%aiId] = True;

		%UserRequest = BotFuncs::NearestAttacker(%aiId, 500, 2);

		if (%UserRequest == 0)

//Nearest request too far away... So delete it

		{
			%UserRequest = BotFuncs::NearestAttacker(%aiId, 99999, 2);
			BotFuncs::DelAttacker(%aiId,%UserRequest);
		}
		else

		// Go to User Requeued Location

			AI::HuntTarget(%aiName, %UserRequest, 1);
			$Spoonbot::BotStatus[%aiId] = "Moving";
	}


// AI Team Mate Requested Move

	else if (BotFuncs::NearestAttacker(%aiId, 500, 3) != 0)
	{

		if ($Spoonbot::DebugMode)
			echo ("STATUS BotMove::Move = AI Teammate requested move");

		$BotThink::ForcedOfftrack[%aiId] = false;

		%AIRequest = BotFuncs::NearestAttacker(%aiId, 500, 3);  //To Wicked69: You had a typo here ;-)
 
		if (%AIRequest == 0)

//Nearest request too far away... So delete it

		{
			%AIRequest = BotFuncs::NearestAttacker(%aiId, 99999, 3);
			BotFuncs::DelAttacker(%aiId,%AIRequest);
		}
		else
		{
// Go to AI Requeted Location

			$Spoonbot::BotStatus[%aiId] = "Helping AI";
			AI::HuntTarget(%aiName, %AIRequest, 1);
		}
	}


  schedule("BotMove::Move(" @ %aiId @ " );", $Spoonbot::MovementInterval);
}

