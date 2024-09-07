// JetToPos function 0.3 by Dewy
// Not yet considering heat/energy factors!!! 'Tis just a beta function.
// The role of this function is primarily just to be a educational refference for scripters. (But it does work.)
// Coordinates have to be set, else returns False.
// $ options: ex. --->$JetToPos = Show;<---
//  	Show = Show operations.
//		Break = Bailout of the operation before it recycles.

function JetToPos(%client, %targetPos)
{
if (Player::getArmor(%client) == False) // If the client is dead then clean up here and quit looping on it.
	{
	   $JetToPos[%client] = ""; // Clean up dead entries.
	   %name = Client::getName(%client);
//	   echo(%name@" died, stopping JetToPos.");
	   return False;  // Bail out.
	   break;
	}
	if(%targetPos != "")
	{
	   Item::setVelocity(%client, "0 0 0");	//Stop the client, so inherited velocity doesn't affect JetToPos results.
	   deleteVariables($JetToPos[""@%client@""]); // If there's new coordinates then delete the old ones.
	   $JetToPos[%client] = %targetPos; // Set the new ones.
	}
   else
   {
	if(%targetPos == "")
	{
	   %targetPos = $JetToPos[%client]; // If there's no coordinants then look in the client's table.
	}
   }

	if(%targetPos == "break"){return False;break;}

	if((%targetPos==0) || (%targetPos==-1)) // If none are found 
	{
//	   echo("No Coordinates specified. JetToPos ABORTED");
	   $JetToPos[%client] = ""; // Purge coordinates.
	   return False;
	   break;
	}






// Get client position.
		%clientPos = GameBase::getPosition(%client);
		%clientX = getWord(%clientPos,0);
		%clientY = getWord(%clientPos,1);
		%clientZ = getWord(%clientPos,2);
// Setup Speed controls.
		%speed = Item::getVelocity(%client);
		%speedX = getWord(%speed, 0);
		%speedY = getWord(%speed, 1);
		%speedZ = getWord(%speed, 2);
// Setup Impulse info
		%setPosX = getWord(%targetPos,0);
		%setPosY = getWord(%targetPos,1);
		%setPosZ = getWord(%targetPos,2);
		%mathX = Vector::neg(%clientX);
		%mathY = Vector::neg(%clientY);
		%mathZ = Vector::neg(%clientZ);
		%destPosX = %setPosX+%mathX;
		%destPosY = %setPosY+%mathY;
		%destPosZ = %setPosZ+%mathZ;
		%reverseX = Vector::neg(%destPosX);
		%reverseY = Vector::neg(%destPosY);
		%reverseZ = Vector::neg(%destPosZ);
		
// Get Distance.
		%dist = Vector::getDistance(%clientPos, %targetPos);
		%Xdist = Vector::getDistance(%clientX, %setPosX);
		%Ydist = Vector::getDistance(%clientY, %setPosY);
		%Zdist = Vector::getDistance(%clientZ, %setPosZ);
// Emergency Brakes.
		%brakeX = Vector::neg(%speedX); //*0.25;
		%brakeY = Vector::neg(%speedY); //*0.25;

// Finite Vertical Navigation.
		%parachuteVal = Vector::neg(%speedZ)*0.15;
		%vertAdjust = %setPosZ-%clientZ+3;

	if(%setPosZ >= %clientZ)
	{
		if(%dist <= 25)
		{
			if(%vertAdjust <= 5)
			{
				Player::ApplyImpulse(%client, "0 0 "@%vertAdjust);
			}
			else
			{
				if(%destPosZ >= 0)
				{
					Player::ApplyImpulse(%client, "0 0 10");
				}
			}
		}
	}

	if(%speedX >= 50 || %speedY >= 50 || %speedZ >= 50) // Error Trap.
	{
		echo("JetToPos calculation ERROR!!! Aborting...");
			   $JetToPos[%client] = ""; // Purge coordinates.
		break;
	}

	
 // Don't make the client go "FWING!!!" (In any direction.) :P :)
	if(%dist >= 25) // Coarse Flight.
	{
		if(%speedX <=  -10){if(%destPosX <= -1){%destPosX = -0.1;}} //%destPosX*0.010;}}
		if(%speedY <= -10){if(%destPosY <= -1){%destPosY = -0.1;}} //%destPosY*0.010;}}
		if(%speedZ <= -15){if(%destPosZ <= -1){%destPosZ = %destPosZ*0.010;}}
		if(%speedZ <= -0.000015){if(%destPosZ <= -0.000015){%destPosZ = %destPosZ+Vector::neg(%destPosZ);}}

		if(%speedX >= 10){if(%destPosX >= 1){%destPosX = %destPosX*-0.010;}} //0.1;}}
		if(%speedY >= 10){if(%destPosY >= 1){%destPosY = %destPosY*-0.010;}} //0.1;}}
		if(%speedZ >= 15){if(%destPosZ >= 1){%destPosZ = %destPosZ*-0.010;}}
		if(%speedZ >= 0.000015){if(%destPosZ >= 0.000015){%destPosZ = %destPosZ+Vector::neg(%destPosZ);}}

		if(%speedX <= -15){%destPosX = %brakeX;}
		if(%speedY <= -15){%destPosY = %brakeY;}
		if(%speedZ <= -0.1){Player::ApplyImpulse(%client, "0 0 "@%parachuteVal);}

		if(%speedX >= 15){%destPosX = %brakeX;} 
		if(%speedY >= 15){%destPosY = %brakeY;} 
		if((%speedZ <= 15)&&(%speedZ>-0.1)){Player::ApplyImpulse(%client, "0 0 "@%vertAdjust);} //parachuteVal);} 
		if(%speedZ <= 5){Player::ApplyImpulse(%client, "0 0 "@%vertAdjust);} //parachuteVal);} 
	}
	if(%dist < 25) // The Alignment.
	{
		if(%dist >= 10)
		{
			if(%speedX <=  -10){if(%destPosX <= -5){%destPosX = %destPosX*0.010;}}
			if(%speedY <= -10){if(%destPosY <= -5){%destPosY = %destPosY*0.010;}}
			if(%speedZ <= -10){if(%destPosZ <= -5){%destPosZ = %destPosZ*0.010;}}
			if(%speedX >= 10){if(%destPosX >= 5){%destPosX = %destPosX*-0.010;}}
			if(%speedY >= 10){if(%destPosY >= 5){%destPosY = %destPosY*-0.010;}}
			if(%speedZ >= 10){if(%destPosZ >= 5){%destPosZ = %destPosZ*-0.010;}}
			if(%speedX >= 15){%destPosX = %brakeX;} 
			if(%speedX <= -15){%destPosX = %brakeX;}
			if(%speedY >= 15){%destPosY = %brakeY;} 
			if(%speedY <= -15){%destPosY = %brakeY;}
			if(%speedZ <= -0.1){Player::ApplyImpulse(%client, "0 0 "@%parachuteVal);}
			if(%speedZ <= 10) {Player::ApplyImpulse(%client, "0 0 "@%vertAdjust);}
		}
	}
	if(%dist <= 10) // The Landing.
	{
			if(%speedX <=  -5){if(%destPosX <= -1){%destPosX = %destPosX*0.10;}}
			if(%speedY <= -5){if(%destPosY <= -1){%destPosY = %destPosY*0.10;}}
			if(%speedZ <= -5){if(%destPosZ <= -1){%destPosZ = %destPosZ*0.10;}}
			if(%speedX >= 5){if(%destPosX >= 1){%destPosX = %destPosX*-0.10;}}
			if(%speedY >= 5){if(%destPosY >= 1){%destPosY = %destPosY*-0.10;}}
			if(%speedZ >= 5){if(%destPosZ >= 0.11){%destPosZ = %destPosZ*-0.20;}}
			if(%speedX >= 3){%destPosX = %brakeX;} 
			if(%speedX <= -3){%destPosX = %brakeX;}
			if(%speedY >= 3){%destPosY = %brakeY;} 
			if(%speedY <= -3){%destPosY = %brakeY;}
			if(%speedZ >= 15){%destPosZ = %parachuteVal;} 
			if(%speedZ <= -0.1){%destPosZ = %parachuteVal;}
	}

	// Finite Horizontal Navigation.
	if(%Xdist <= 15)
	{
		if(%Zdist >= 0) // Here we tell JetToPos to hold X&Y until Z is in the clear.
		{
			if(%Zdist <= 1)
			{
				if(%Xdist <= 20){if(%speedX <= 3){%lockX = false;}}
			}
			if(%dist <= 0)
			{
				if(%Xdist >= -20){if(%speedX >= -3){%lockX = false;}}
			}
			if(%Zdist >= 1)
			{
			   if(%Xdist <= 20){%lockX = true;}
			   if(%Xdist >= -20){%lockX = true;}
			   if(%speedZ >= 3){Player::ApplyImpulse(%client, "0 0 "@%parachuteVal);} 
			   if(%speedZ <= -0.1){Player::ApplyImpulse(%client, "0 0 "@%parachuteVal);}
			   %destPos = ""@%destPosX@" "@%destPosY@" "@%destPosZ@"";
			   Player::ApplyImpulse(%client, %destPos);
			}
		}
	}
	if(%Ydist <= 15)
	{
		if(%Zdist >= 0) // Here we tell JetToPos to hold X&Y until Z is in the clear.
		{
			   if(%Zdist <= 1)
			   {
				if(%Ydist <= 20){if(%speedY <= 3){%lockY = false;}}
			   }
			   if(%dist <= 0)
			   {
				if(%Ydist >= -20){if(%speedY >= -3){%lockY = false;}}
			   }
				if(%Zdist >= 1)
				{
				   if(%Ydist <= 20){%lockY = true;}
				   if(%Ydist >= -20){%lockY = true;}
				}
				if(%speedZ >= 3){Player::ApplyImpulse(%client, "0 0 "@%parachuteVal);} 
				if(%speedZ <= -0.1){Player::ApplyImpulse(%client, "0 0 "@%parachuteVal);}
				%destPos = ""@%destPosX@" "@%destPosY@" "@%destPosZ@"";
				Player::ApplyImpulse(%client, %destPos);
		}
	}
if(%lockX == true)
	{
		%destPosX = %brakeX;
	}
	if(%lockY == true)
	{
		%destPosY = %brakeY;
	}
// Are we stuck? Hmm, lets try to fix it. (For field obsticals.)
	if(%dist >= 10)
	{
		if(%speedY <= 0.1){if(%speedY >= 0){if(%lockX != true){%destPosX = %reverseX*0.001; %stuck = true; %freeUp = "+Y axis. Reversing X to "@%reverseX;}}}
		if(%speedY >= -0.1){if(%speedY <= 0){if(%lockX != true){%destPosX = %reverseX*0.001; %stuck = true; %freeUp = "-Y axis. Reversing X to "@%reverseX;}}}
		if(%speedX <= 0.1){if(%speedX >= 0){if(%lockY != true){%destPosY = %reverseY*0.001; %stuck = true; %freeUp = "+X axis. Reversing Y to "@%reverseY;}}}
		if(%speedX >= -0.1){if(%speedX <= 0){if(%lockY != true){%destPosY = %reverseY*0.001; %stuck = true; %freeUp = "-X axis. Reversing Y to "@%reverseY;}}}
		if(%speedZ <= 0.1){if(%speedZ >= 0){%destPosZ = %reverseZ; %stuck = true; %freeUp = "+Z axis. Reversing "@%reverseZ;Player::ApplyImpulse(%client,"0 0 200");}}
		if(%speedZ >= -0.1){if(%speedZ <= 0){%destPosZ = %reverseZ; %stuck = true; %freeUp = "-Z axis. Reversing "@%reverseZ;}}
	}
	if(%dist >=25)
	{
		%destPos = ""@%destPosX@" "@%destPosY@" "@%destPosZ@""; //0";
	}
	else
	{
		%destPos = ""@%destPosX@" "@%destPosY@" "@%destPosZ@"";
	}
	if(%dist <= 4.5)
	{
		GameBase::setPosition(%client,%clientPos);
	}
	if($JetToPos[%client] == show)
	{
	cls();  // clear the console screen.

	%name = Client::getName(%client);
	if(%stuck == true){echo("Stuck on "@%freeUp);}
	
	echo("Used by = "@%name);
	echo("User ID = "@%client);
	echo("speed = "@%speed);
	echo("Current Pos = "@%clientPos);
	echo("TargPos = "@%targetPos);
	echo("Applied value = "@%destPos);
	echo("Distance = "@%dist);
	echo("Xdist = "@%Xdist);
	echo("Ydist = "@%Ydist);
	echo("Zdist = "@%Zdist);
	echo("Vertical Adjust = "@%vertAdjust);
	echo("Paracute Value = "@%parachuteVal);
	echo();
	echo();
	echo();
	echo();
	echo();
	echo();
	}


//	if (((%speedX+%speedY+%speedZ) < 0.1) && (%dist >= 2)) // && (%dist <= 30))
//		JetToPosCheat(%client, %targetPos);



// Apply the results.	
	if(%dist >= 6) // A higher value for faster/sloppier destinations.
	{
		if($JetToPos[%client] != "break") // Override input. (Just in case.)
		{
			if(%destPos != "")
			{
//				echo("Speed: " @ %speed);
//				echo("Dista: " @ %dist);

				$Spoonbot::BotJettingHeat[%client]=1; //Make missile turrets lock on me
		//Player::ApplyImpulse(%client,"0 0 "@%vertAdjust*10);
				Player::ApplyImpulse(%client, %destPos);
				schedule("JetToPos("@%client@");",0.01); // Comment out this line if if your gonna call the function from elsewhere.
				// Note: You'll need your external function call to JetToPos repeatedly until goal is met.
				return %dist; // Tie-in for externals. (For use if external function decides when it should quit.)
			}
			else
			{
				echo("No Destination Found");
			}
		}
	}
	else
	{
		$Spoonbot::BotJettingHeat[%client]=0; //Missile turrets don't get a lock on me anymore
		if($JetToPos[%client] == show)
		{
			echo(%name@" made it to the destonation!!! WooHoo!!! :)");
			$JetToPos[%client] = ""; // Where all done with them.
		}
	}

}

function setJetToPos(%client){$setTargDest = GameBase::getPosition(%client);echo("Position = "@$setTargDest);}
function go(%client){JetToPos(%client,$setTargDest);}

function JetToPosCheat(%client, %targetPos)
{
	if ((%targetPos=="") || (%targetPos==0))
	{
		AI::RandomEvade(%client);
		return;
	}
	%dist = Vector::getDistance(GameBase::getPosition(%client), %targetPos);
	if ((%dist<=0) || (%dist>200))
	{
		AI::RandomEvade(%client);
		return;
	}

	$JetToPosCheatActivated[%client]=False;
	if ($Spoonbot::DebugMode)
	 echo ("CHEAT: Now cheating on JetToPos. Teleporting Player.");
	$Spoonbot::BotJettingHeat[%client]=0;
	GameBase::setPosition(%client,%targetPos);
	Item::setVelocity(%client, "0 0 0");
	$JetToPos[%client]="break";

}

function WarpMyAss(%client, %targetPos)
{
		if ((%targetPos=="") || (%targetPos==0))
		{
			AI::RandomEvade(%client);
			return;
		}
		%dist = Vector::getDistance(GameBase::getPosition(%client), %targetPos);
		if ((%dist<=0) || (%dist>200))
		{
			AI::RandomEvade(%client);
			return;
		}

		if ($Spoonbot::DebugMode)
		 echo ("CHEAT: Warping the ass of bot "@ %client);
		GameBase::setPosition(%client,%targetPos);
		$Spoonbot::BotJettingHeat[%client]=0;
		Item::setVelocity(%client, "0 0 0");
		$JetToPos[%client]="break";
		schedule ("$JetToPos[%client]=\"\"", 0.3);

}