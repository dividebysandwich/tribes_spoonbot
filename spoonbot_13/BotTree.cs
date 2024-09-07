
$BotTree::T_Count = 0;

function BotTree::Add_T(%id, %pos)
{

// check to see if an exiting point is within 1 unit, if so ignore request.

	%add_tp = -1;

	%minAdj = 99999;

	for(%x = 1; %x < $BotTree::T_Count; %x++)
		if(Vector::getDistance(%pos, $BotTree::T_Pos[%x]) < 1)
			%add_tp = %x;

	if(%add_tp == -1)
	{
		$BotTree::T_Pos[$BotTree::T_Count] = %pos;

		BotFuncs::TeamMessage(GameBase::getTeam(%id), "Added Tree point " @ $BotTree::T_Count @ " at " @ %pos);

if ($Spoonbot::DebugMode)
		dbecho(1,"Added Tree point " @ $BotTree::T_Count @ " at " @ %pos);
if ($Spoonbot::DebugMode)
		dbecho(1,"Point " @ $BotTree::T_Count @ " can see " @ BotTree::GetLosView(%pos));

		$BotTree::T_Count = $BotTree::T_Count + 1;

		BotTree::Save_Tree();

	}
}

// Function to see if a point can see all points in list

function BotTree::CanSeeAllLOS(%Point,%Points)
{
	%Result = true;
	%PointCount = BotThink::getWordCount(%points);
	%Pos = $BotTree::T_Pos[%Point];
	%Cansee = "";
	for(%x = 0; %x < %PointCount; %x++)
	{
		if	((%Point != getWord(%points,%x)) &&
			(BotTree::CheckLOS(%Pos,$BotTree::T_Pos[getWord(%points,%x)],200) == False))
			%Result = False;
		else
			%cansee = %cansee @ getWord(%points,%x) @ " ";
	}

	return %Result;
}

//  Function filter:
// 1 = SimTerrainObjectType | SimInteriorObjectType
// 2 = StaticObjectType
// 4 = MoveableObjectType
// 8 = VehicleObjectType
// 16 = PlayerObjectType
// 32 = MineObjectType
// 64 = ItemObjectType
//
// The function returns true if it hit something, the details are placed in the
// global variables $los::position, $los::normal, $los::object (same as the
// GameBase LOS function).

function BotTree::CheckLOS(%pos1,%pos2,%MaxDist)
{
	%Result = False;

	if(Vector::getDistance(%pos1,%pos2) < %MaxDist)
		if(GetLosInfo(%pos1, %pos2, 1) == 0)
			%Result = True;
	return %Result;
}

function BotTree::CheckItemLOS(%pos1,%pos2,%MaxDist)
{
	%Result = False;
	if ((%MaxDist<=0) || (%MaxDist=""))
		%MaxDist=300;

	%xPos = getWord(%pos1, 0);                   //Now add some height to coordinates to make sure we get a correct LOS
	%yPos = getword(%pos1, 1);
	%zPos = getWord(%pos1, 2) + 1.5;
	%pos1 = %xPos @ "  " @ %yPos @ "  " @ %zPos;

	%xPos = getWord(%pos2, 0);
	%yPos = getword(%pos2, 1);
	%zPos = getWord(%pos2, 2) + 0.5;
	%pos2 = %xPos @ "  " @ %yPos @ "  " @ %zPos;


	if(Vector::getDistance(%pos1,%pos2) < %MaxDist)
		if(GetLosInfo(%pos1, %pos2, 1) == 0)
			%Result = True;
	return %Result;
}

function BotTree::GetLosView(%pos)
{
	%Result = "";

	for(%x = 1; %x < $BotTree::T_Count; %x++)
		if(	($BotTree::T_Pos[%x] != %Pos) &&
				BotTree::CheckLOS(%pos,$BotTree::T_Pos[%x],200))
			%Result = %Result @ %x @ " ";
	
	return %Result;
}



function BotTree::AutoTree(%id)
{

// Check for startup vars - no nulls please!.

	if($BotTree::LastGoodPos == "")
	{
		$BotTree::LastGoodLosView = BotTree::GetLosView(Vector::Add(GameBase::getPosition(%id),"0 0 2"));
		$BotTree::LastGoodPos = Vector::Add(GameBase::getPosition(%id),"0 0 0.01");
if ($Spoonbot::DebugMode)
		dbecho(1,"Init tree search engine");
	}

// Get current position

	$BotTree::CurrentPos = Vector::Add(GameBase::getPosition(%id),"0 0 2");

	if($BotTree::LastGoodPos != $BotTree::CurrentPos)
	{

		$BotTree::CurrentPos = Vector::Add(GameBase::getPosition(%id),"0 0 0.01");
		$BotTree::CurrentLosView = BotTree::GetLosView($BotTree::CurrentPos);
		
		if($BotTree::CurrentLosView == $BotTree::LastGoodLosView) // LOS!
		{
			$BotTree::LastGoodPos = $BotTree::CurrentPos;
			$BotTree::LastGoodLosView = $BotTree::CurrentLosView;
		}
		else // No LOS!
		{


if ($Spoonbot::DebugMode)
			dbecho(1,"Los change.");
if ($Spoonbot::DebugMode)
			dbecho(1,"Last    = " @ $BotTree::LastGoodLosView);
if ($Spoonbot::DebugMode)
			dbecho(1,"Current = " @ $BotTree::CurrentLosView);

// Lost contact with tree points so add a new one at last good point.

			if ((BotThink::getWordCount($BotTree::CurrentLosView) == 0) &&
				(BotThink::getWordCount($BotTree::LastGoodLosView) > 0))
			{
				if ($Spoonbot::DebugMode)
				   dbecho(1,"Type 1: Added point at last good pos");

//				if ($BotTree::AutoTree==True)
					BotTree::Add_T(%id, Vector::Add($BotTree::LastGoodPos,"0 0 2"));
//				else
//					messageAll(0, "I suggest adding a Treepoint. "@ $BotTree::LastGoodPos);
			}

// Gained contact with tree points. Add tree point at current position
// Should never happen as in design mode it should not be possible to lose sight in tree points.

//Comment by Werewolf: It happens, when you switch teams. I always switch teams to map the other team's base, so this IS a common situation!

			else if((BotThink::getWordCount($BotTree::CurrentLosView) > 0) &&
					(BotThink::getWordCount($BotTree::LastGoodLosView) == 0))
			{
				
//				if ($BotTree::AutoTree==True)
					BotTree::Add_T(%id,Vector::Add( $BotTree::CurrentPos,"0 0 2"));
//				else
//					messageAll(0, "I suggest adding a Treepoint. "@ $BotTree::CurrentPos);

				if ($Spoonbot::DebugMode)
					dbecho(1,"Type 2: Added point at current pos");
			}

			$BotTree::CurrentLosView = BotTree::GetLosView($BotTree::CurrentPos);
					
			$BotTree::LastGoodPos = $BotTree::CurrentPos;
			$BotTree::LastGoodLosView = $BotTree::CurrentLosView;

if ($Spoonbot::DebugMode)
			dbecho(1,"New Los.");
if ($Spoonbot::DebugMode)
			dbecho(1,"NewLast    = " @ $BotTree::LastGoodLosView);
if ($Spoonbot::DebugMode)
			dbecho(1,"NewCurrent = " @ $BotTree::CurrentLosView);

		}
	}


	$BOTTREE::Target_Loc = "<" @ $BotTree::LastGoodLosView @ ">";
	$BOTTREE::Unresolved_Objects = "<" @ $BotTree::CurrentLosView @ ">";
	$BOTTREE::User_Loc = $BotTree::CurrentPos;

// Reschedule after a couple of  milli secs!
	schedule("BotTree::AutoTree(" @ %Id@ ");", 0.1);
}

// initialise any startup variables and load from Tree File

function BotTree::Init_Tree()
{

deleteVariables("$BotTree::*");
deleteVariables("$BT::*");

// Reset Tree
$BotTree::T_Count = 0;
$BotTree::T_Count_Calc = 0;
$BotTree::T_Exits = "";
$BotTree::T_ReSupplyPoint = "";
$BotTree::T_CommandPoint = "";
$BotTree::T_FlagPoint = "";
$BotTree::T_FirePoint = "";
$BotTree::T_GeneratorPoint = "";
$numSniperSpots[0]=0;
$numSniperSpots[1]=0;

// Load pos details on objects to be linked into Tree system, gens, guns etc.



// Set Tree Filename based on current mission

	%Filename = "BT_" @ $missionName @".cs";

// Load Tree if Exists else create blank one.

	exec("Vehicle");	//For some reason, vehicle.cs doesn't get loaded sometimes. This fixes it.

	$Spoonbot::UseTreefiles = False;
	if(isFile("config\\" @ %Filename))
	$Spoonbot::UseTreefiles = True;

	if(isFile("config\\" @ %Filename))
	{
		exec(%filename);
	}
	else
		if($Spoonbot::BotTree_Design)
			BotTree::Save_Tree();

	//Old version detected, convert the file
	if(isFile("config\\" @ %Filename))
	if ((!$Spoonbot::BotTree_Design) && ($BotTree::T_Version != 2))
	{
		deleteVariables("$BotTree::T_A*");
		deleteVariables("$BotTree::T_R*");
		BotTree::Calculate_Tree_Routes();
		BotTree::Save_Tree();
		messageall(0, "Successfully converted Treefile to Version 2");
	}

	dbecho(1,"BotTree::Init_Tree: Loaded " @ $BotTree::T_Count @ " Tree Point");

	if ($BotTree::T_Count > 0)
                messageAll(0, "Successfully loaded Treefile with " @ $BotTree::T_Count @ " points");
	else
                messageAll(1, "No valid Treefile found! Please make a treefile for this map!");


	if($Spoonbot::BotTree_Design)
	{
                messageAll(1, "WARNING: Treefile Design Mode active!");

		%manager = 2048;

		%found = false;

		while((%manager < 2070) && (%found == false))
			if (Client::getName(%manager) == "")
				%manager++;
			else if (Player::isAIControlled(%manager) == "true")
				%manager++;
			else
				%found = true;

		if (%manager == 2070)
		{
			dbecho(1,"No player to use as master owner of tree points, try again in 5 secs");

			schedule("BotTree::Init_Tree();", 2);

			$BotTree::Manager = %manager;
				
			return;
		}

		dbecho(1, "manager = " @ %manager @ ", name = '" @ Client::getName(%manager) @ "'");

//If there are not points add one by player.

//		if($BotTree::T_Count == 0)
//			BotTree::Add_T(%id, Vector::Add(GameBase::getPosition(%id),"0 0 3"));

		%id=%manager;
		%pos=GameBase::getPosition(%manager);

		$BotTree::T_Pos[$BotTree::T_Count] = %pos;

//		BotFuncs::TeamMessage(GameBase::getTeam(%id), "Added Tree point " @ $BotTree::T_Count @ " at " @ %pos);

//if ($Spoonbot::DebugMode)
//		dbecho(1,"Added Tree point " @ $BotTree::T_Count @ " at " @ %pos);

//if ($Spoonbot::DebugMode)
//		dbecho(1,"Point " @ $BotTree::T_Count @ " can see " @ BotTree::GetLosView(%pos));


		$BotTree::T_Count = $BotTree::T_Count + 1;

		BotTree::Save_Tree();
//		if ($BotTree::AutoTree==True)
			BotTree::AutoTree(%Manager);

	}
}

function BotTree::Save_Tree()
{
	%Filename = "config\\BT_" @ $missionName @ ".cs";
	$BotTree::T_Version = 2;
	export("$BotTree::T_*", %Filename, false);
}

function BotTree::Calculate_Tree_Routes()
{
	// The main calculation algorithm has been changed to use Dijkstra's
	// algorithm.

	// Remove any unwanted points


//	BotTree::Optimise_Points();

	BotTree::ProcessPoints();

	BotTree::Save_Tree();
}


// Callable functions for tree selection.

function BotTree::FindNearestTreebyId(%id)
{
	%PlayerPosition = Vector::Add(GameBase::getPosition(%id),"0 0 2");

	%Nearest = -1;

	%Distance = 999999;

	for(%x = 1; %x < $BotTree::T_Count; %x++)
	{
		%TreePosition = $BotTree::T_Pos[%x];

		%NomDistance = Vector::getDistance(%PlayerPosition, %TreePosition);

		if ((%NomDistance < %Distance) &&
			(BotTree::CheckLOS(%PlayerPosition, %TreePosition,20000) == True))
		{
			%Nearest = %x;
			%Distance = %NomDistance;
		}
	}
 
	if (%Nearest != -1)
		return %Nearest;
	else if (($BotThink::LastPassedTreepoint[%id]!="") && ($BotThink::LastPassedTreepoint[%id]>0))
	{
		echo ("WARNING (nonfatal): BotTree::FindNearestTreebyId can't find a treepoint with LOS!");
		echo ("...returning LastPassedTreepoint " @ $BotThink::LastPassedTreepoint[%id]);
		return $BotThink::LastPassedTreepoint[%id];
	}
	else
	{
		echo ("WARNING (FATAL): BotTree::FindNearestTreebyPos can't find a treepoint with LOS!");
		echo ("...returning NearestTreebyPosNOLOS " @ BotTree::FindNearestTreebyPosNOLOS(%PlayerPosition));
		return BotTree::FindNearestTreebyPosNOLOS(%PlayerPosition);
	}
}



function BotTree::FindNearestTreebyPos(%PlayerPosition)
{
	%Nearest = -1;

	%PlayerPosition = Vector::Add(%PlayerPosition,"0 0 2");

	%Distance = 999999;

	for(%x = 1; %x < $BotTree::T_Count; %x++)
	{
		%TreePosition = $BotTree::T_Pos[%x];

		%NomDistance = Vector::getDistance(%PlayerPosition, %TreePosition);

		//dbecho(1, "nomdistance = " @ %NomDistance);

		if ((%NomDistance < %Distance) && (BotTree::CheckLOS(%PlayerPosition, %TreePosition,20000) == True))
		{
			%Nearest = %x;
			%Distance = %NomDistance;
		}
	}
 
	$LosWarning=False;
	if (%Nearest != -1)
		return %Nearest;
	else
	{
		$LosWarning=True;
		echo ("WARNING: BotTree::FindNearestTreebyPos can't find a treepoint with LOS!");
		return BotTree::FindNearestTreebyPosNOLOS(%PlayerPosition);
	}
 
}


//Same as above, but without LOS check!
function BotTree::FindNearestTreebyPosNOLOS(%PlayerPosition)
{
	%Nearest = -1;

	%Distance = 999999;

	for(%x = 1; %x < $BotTree::T_Count; %x++)
	{
		%TreePosition = $BotTree::T_Pos[%x];

		%NomDistance = Vector::getDistance(%PlayerPosition, %TreePosition);

		//dbecho(1, "nomdistance = " @ %NomDistance);

		if (%NomDistance < %Distance)
		{
			%Nearest = %x;
			%Distance = %NomDistance;
		}
	}

	return %Nearest;
 
}


function BotTree::CheckLosToObjects(%TreePosition, %EnemyTeam)
{
	%foundObjects=0;
	for (%i = 0; %i<$BotFuncs::AllCount; %i++)
	{
		%curobj = $BotFuncs::AllList[%i];
		%NomDistance = Vector::getDistance(GameBase::getPosition(%curobj), %TreePosition);
		if ((GameBase::getTeam(%curobj) == %EnemyTeam) && (%NomDistance > 150) && (BotTree::CheckLOS(%PlayerPosition, %TreePosition,1000) == True))
			%foundObjects++;
		if (%foundObjects > 3)
			return;
	}


}

function BotTree::isOutside(%TreePosition)
{
	%Result = False;

	%xPos = getWord(%TreePosition, 0);                   //Now add some height to coordinates to make sure we get a correct LOS
	%yPos = getword(%TreePosition, 1);
	%zPos = getWord(%TreePosition, 2) + 5;
	%pos1 = %xPos @ "  " @ %yPos @ "  " @ %zPos;

	%xPos = getWord(%TreePosition, 0);
	%yPos = getword(%TreePosition, 1);
	%zPos = getWord(%TreePosition, 2) + 10;
	%pos2 = %xPos @ "  " @ %yPos @ "  " @ %zPos;

	%xPos = getWord(%TreePosition, 0);
	%yPos = getword(%TreePosition, 1);
	%zPos = getWord(%TreePosition, 2) + 15;
	%pos3 = %xPos @ "  " @ %yPos @ "  " @ %zPos;

	%xPos = getWord(%TreePosition, 0);
	%yPos = getword(%TreePosition, 1);
	%zPos = getWord(%TreePosition, 2) + 20;
	%pos4 = %xPos @ "  " @ %yPos @ "  " @ %zPos;

	if(GetLosInfo(%TreePosition, %pos1, 1) == 0)
	if(GetLosInfo(%TreePosition, %pos2, 1) == 0)
	if(GetLosInfo(%TreePosition, %pos3, 1) == 0)
	if(GetLosInfo(%TreePosition, %pos4, 1) == 0)
		%Result = True;
	return %Result;

}


function BotTree::FindSniperPos(%aiId)
{
	%Nearest = -1;

	%aiTeam = Client::getTeam(%aiId);
	if(%aiTeam == 0)
	  %EnemyTeam = 1;
	else
	  %EnemyTeam = 0;

	if ($numSniperSpots[%aiTeam]<=0)
	{
		%numSpots = 0;
		for(%x = 1; %x < $BotTree::T_Count; %x++)
		{
			%TreePosition = $BotTree::T_Pos[%x];
			if (BotTree::CheckLosToObjects(%TreePosition, %EnemyTeam) >= 2)
			{
				if (BotTree::isOutside(%TreePosition))
				{
					$foundSniperSpots[%aiTeam, %numSpots] = %TreePosition;
					%numSpots++;
				}
			}
		}
		$numSniperSpots[%aiTeam] = %numSpots;
	}
if ($Spoonbot::DebugMode)
		dbecho(1,"Found " @ $numSniperSpots[%aiTeam] @ " Sniper spots");
 
	%index = floor(getRandom() * $numSniperSpots[%aiTeam]);
if ($Spoonbot::DebugMode)
		dbecho(1,"Using spot No." @ %index @ " at pos " @ $foundSniperSpots[%aiTeam, %index]);
	return $foundSniperSpots[%aiTeam, %index];

}






function BotTree::ProcessPoints()
{
   for (%point1=0;%point1<$BotTree::T_Count;%point1++)
   {
        %PointInfo = "";
        for (%point2=0 ;%point2<$BotTree::T_Count;%point2++)
        {
            if (%point1 != %point2)
            {
                %los = false;
                %coord1 = $BotTree::T_Pos[%point1];
                %coord2 = $BotTree::T_Pos[%point2];

                if (BotTree::CheckLOS(%coord1,%coord2,2000))
                {
                  %PointInfo = %PointInfo @ %point2 @ " " ;
                }
            }
        }
   $BotTree::T_Edge[%point1] = %PointInfo;
  }
}

//---------------------------------------------------------------------------------------------------------------------------------
//
//---------------------------------------------------------------------------------------------------------------------------------
// Find the path from point1 to point2
// Will not report error when path is not available! Need to add this check yourself.
// This is just the basic algorithm

function BotTree::FindPath(%Point1, %Point2)
{

//          messageall(0, %point1 @ "=Point1 ");
//          messageall(0, %point2 @ "=Point2 ");
        if (%point1 == %point2) return (%point1);

        %cashpath = $BT::CASHPATH[%Point1, %Point2];
        if ((%cashpath != -1) && (%cashpath != ""))
        {
//          messageall(0, "= !!! CASHED PATH !!!! ");
          return (%cashpath);
          break;
        }

        %PointString = %Point2;
        %Used[%Point2] = true;
        %found=false;

        for (%PointIndex=0;%found==false; %PointIndex++)
        {
                %LookupPoint = getWord(%PointString, %PointIndex);
                //messageall(0, %LookupPoint @ "=Look upstring ");
                if  ((%LookupPoint == -1) || (%LookupPoint == ""))
                {
                  break;
                }

                %TempString = $BotTree::T_Edge[%LookupPoint];
                //messageall(0, %TempString @ "=%TempString ");
                for (%PointNumber=0; %found==false; %PointNumber++)
                {
                        %Point = GetWord(%TempString,%PointNumber);

                        if ((%Point == -1) || (%Point == ""))
                        {
                          break;
                        }
                        if (%Used[%Point] == "")
                        {
                                %Parent[%Point] = %LookupPoint;
                                //messageall(0, %Point @ "=Parent ");
                                //messageall(0, %LookupPoint @ "=POINT ");

                                %Used[%Point] = true;
                                %PointString = %PointString @ " " @ %Point;
                                if (%Point == %Point1){ %found = true;}
                        }
                }
        }

        %path = %Point1;
        %Point = %Point1;
	$LastNumPoints = 0;
        for (%nI =0; (%nI < 80) && (%Point !=""); %nI++)
        {
                %Point = %Parent[%Point];
                %path = %path @ " " @ %Point;
		$LastNumPoints++;
        }
        $BT::CASHPATH[%Point1, %Point2] = %path;

return (%path);
}


function BotTree::GetMeToPos(%aiId, %Pos, %FinalPoint)
{

// if nearest tree point to us is directly routeable to TreePoint then use it directly
// else route to nearest exit point to tree point.

	if (($BotThink::ForcedOfftrack[%aiId]==false) && ($BotThink::LastPoint[%aiId]==false))
	{
		if (%Pos == $CurrentTargetPos[%aiId])
			return;
	}
	else
		$CurrentTargetPos[%aiId] = %Pos;

	$BotThink::Definitive_Attackpos[%aiId] = %Pos;

	BotFuncs::DeleteAllAttackPointsByPrio(%aiId, 0); //Really delete ALL attackpoints

	$BotThink::ForcedOfftrack[%aiId]=false;
	$BotThink::LastPoint[%aiId]=false;

	%AiPos = GameBase::getPosition(%aiId);

	if ($Spoonbot::UseTreefiles == False)
	{
		%aiName = Client::getName(%aiId);
		AI::DirectiveWaypoint(%aiName, %Pos, 1024);
		return;
	}


// Nearest point to destination


	if(BotTree::checklos(%aipos, %pos,200) == True) // Go directly if last point set.
	{
		if ($Spoonbot::DebugMode)
		dbecho(1, %aiId @ " can see target!" );

//		if (%FinalPoint)
			BotFuncs::AddAttacker( %aiId, %Pos,3,4);
	}
	else
	{

		%TreePoint = BotTree::FindNearestTreebyPos(%Pos);

// Nearest point to us

		%Nearest = BotTree::FindNearestTreebyId(%aiId);

// if nearest point = nearest point to target go directly to it

		if(%Nearest == %TreePoint)
		{


			if ($Spoonbot::DebugMode)
				dbecho(1, "(2)" @ %aiid @ " can see via one point target, go via point " );

			%Location = $BotTree::T_Pos[%Nearest];

			BotFuncs::AddAttacker( %aiId, %Location,3,4);
//			if (%FinalPoint)
				BotFuncs::AddAttacker( %aiId, %Pos,3,4);


		}

// else route.
		else 
			%tempPath = BotTree::FindPath(%Nearest, %Treepoint);

		if ($LastNumPoints > 0)
		{
			for(%x = 0; %x < $LastNumPoints; %x++)
			{
				%Point = getWord(%tempPath, %x);
					if ($Spoonbot::DebugMode)
						dbecho(1, %aiId @ " next point " @ %Point);
					%Location = $BotTree::T_Pos[%Point];
					BotFuncs::AddAttacker( %aiId, %Location,3,4); // Must be strict with reaching treepoints
											//or else we could hang somewhere!
			}

			if ($Spoonbot::DebugMode)
				dbecho(1, %aiId @ " Final Point " @ %Pos);

			if (%FinalPoint)
				BotFuncs::AddAttacker( %aiId, %Pos,3,4);

			return True;
		}
		
		if ($LastNumPoints==0)
		{
if ($Spoonbot::DebugMode)
			dbecho(1,"Can't get " @ %aiid @ " to " @ %pos );
			return False;
		}
	}
}


function BotTree::Stop(%aiId)
{
	BotFuncs::DeleteAllAttackPointsByPrio(%aiId, 4);
	$BotThink::Definitive_Attackpoint[%aiId] = -1;
	$BotThink::ForcedOfftrack[%aiId] = false;
}


function BotTree::StopTeamBots(%aiId)
{

   %myTeam = GameBase::getTeam(%aiId);
   
   for(%i = 0; %i < $AttackerIndexCount; %i++)
      if(GameBase::getTeam($AttackerIndexIds[%i]) == %myteam)
		BotTree::Stop(%id);
}

function BotTree::CalculateRouteDistance(%Start,%Finish)
{
	%StartPos = $BotTree::T_Pos[%Start];
	%FinishPos = $BotTree::T_Pos[%Finish];
	%Route = $BotTree::T_R[%Start,%Finish];
	%HopCount = BotThink::getWordCount(%Route) - 1;


	if(%Route == "n")
		%Distance = 99999999;

	else if (%Route == "")
		%Distance = Vector::getDistance(%StartPos,%FinishPos);

	else
	{
		%Distance = Vector::getDistance(%StartPos,$BotTree::T_Pos[getWord(%Route,0)]);

		for(%x = 0; %x < %HopCount; %x++)
			%Distance = %Distance +
					Vector::getDistance($BotTree::T_Pos[getWord(%Route,%x)],$BotTree::T_Pos[getWord(%Route,%x+1)]);

		%Distance = %Distance + Vector::getDistance(%EndPos,$BotTree::T_Pos[getWord(%Route,%HopCount)]);
	}

	//dbecho(1,"Distance between " @ %Start @ " and " @ %Finish @ " via '" @ %Route @ "' = " @ %Distance);

	return %Distance;
}


// Removes tree points if other's can already perform function.

function BotTree::Optimise_Points()
{
	for(%x = 1; %x < $BotTree::T_Count; %x++)
	{
		if($BotTree::T_Pos[%x] != "")
		{
			%LosView = BotTree::GetLosView($BotTree::T_Pos[%x]);

			%WordCount = BotThink::getWordCount(%LosView);
		
			%DeletePoint = False;

			for(%y = 0; ((%y < %WordCount) && (%DeletePoint == False)); %y++)
			{
				%Point = getWord(%LosView,%x);
			
				if(BotTree::CanSeeAllLOS(%Point,%LosView) == True)
				{
					%DeletePoint = True;

if ($Spoonbot::DebugMode)
					dbecho(1,%x @ " redundant, " @ %Point @ " can see " @ %LosView);
	
					$BotTree::T_Pos[%x] = "";
				}
			}
		}
	}
}

