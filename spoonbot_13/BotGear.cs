//=========================================================================================================================================
// 																AI::setWeapons()
//=========================================================================================================================================
//
//   Modified by Werewolf
//
//      The AI:setWeapons was removed to this file BotGear for the simple reason of makeing the bots more configurable in an easier manner.
// Mostly for developers of mods that contain bots. This way if you have different weapons and armor types...
//
//  								    The following contains the AI::setWeapons and AI::periodic
//
//  									Moved to the BotGear file for configurability by Emo1313...
//
//=========================================================================================================================================

function AI::setWeapons(%aiName)
{
	%aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;


	if (!BotFuncs::CheckAlive(%aiId))
	{
		return;
	}

	%aiName = Client::GetName(%aiId);

	AI::setVar(%aiName, iq, 90 );

	//== In case some moron ignorant mapmaker (just kidding ;-)) forgot to name the bots according to the readme,
	//== they'll spawn with the ugly-useless standard weapons.
	//== by Werewolf

	Player::setItemCount(%aiId, blaster, 1);
	Player::setItemCount(%aiId, disclauncher, 0);
	Player::setItemCount(%aiId, chaingun, 0);
	Player::setItemCount(%aiId, discammo, 0);
	Player::setItemCount(%aiId, bulletAmmo, 0);

	AI::SetVar(%aiName, triggerPct, 1000 );
	AI::setVar(%aiName, attackMode, 1);
	AI::setAutomaticTargets( %aiName );
//	AI::callbackPeriodic(%aiName, 5, ai::periodicWeaponChange);



	AI::periodicWeaponChange(%aiName);

//=========================================================== If Mission Type Death Match, Set Basics And Return

   if(Game::missionType == "DM")
   {	
        Player::setItemCount(%aiId, blaster, 1);
		Player::mountItem(%aiId, blaster, 0);
	    return;
   }
//=========================================================== Mortar Bot Set Up
	if(String::findSubStr(%aiName, "Mortar") >= 0)
	{
	    AI::setVar(%aiName, iq, 90 );
	   	   for(%i = 0; $Spoonbot::MortarGear[%i] != ""; %i++)
		   {
	 	        Player::setItemCount(%aiId, $Spoonbot::MortarGear[%i], $SpoonBot::MortarAmmo[%i]);
		   }
		   Player::mountItem(%aiId, $SpoonBot::MortarLong, 0);
	}
//=========================================================== Guard Bot Set Up
	if(String::findSubStr(%aiName, "Guard") >= 0)
	{
	    AI::setVar(%aiName, iq, 90 );
	   	   for(%i = 0; $Spoonbot::GuardGear[%i] != ""; %i++)
		   {
	 	        Player::setItemCount(%aiId, $Spoonbot::GuardGear[%i], $SpoonBot::GuardAmmo[%i]);
		   }
		   Player::mountItem(%aiId, $SpoonBot::GuardLong, 0);
	}
	
//=========================================================== Demo Bot Setup
	else if(String::findSubStr(%aiName, "Demo") >= 0)
	{
	   		for(%i = 0; $Spoonbot::DemoGear[%i] != ""; %i++)
			{
	 	        Player::setItemCount(%aiId, $Spoonbot::DemoGear[%i], $SpoonBot::DemoAmmo[%i]);
	        }
			Player::mountItem(%aiId, $SpoonBot::DemoLong, 0);
	}
//=========================================================== Painter Bot Setup
	else if(String::findSubStr(%aiName, "Painter") >= 0)
	{
	    AI::setVar(%aiName, iq, 200 );
   		for(%i = 0; $Spoonbot::PainterGear[%i] != ""; %i++)
 	       Player::setItemCount(%aiId, $Spoonbot::PainterGear[%i], $SpoonBot::PainterAmmo[%i]);
        Player::mountItem(%aiId, $SpoonBot::PainterLong, 0);
	
	    AI::SetVar(%aiName, spotDist, 50);
	    AI::SetVar(%aiName, seekOff , 1);   // Dunno what this does. Werewolf
	}
//=========================================================== Sniper Bot Setup	
	else if(String::findSubStr(%aiName, "Sniper") >= 0)
	{
	   AI::setVar(%aiName, iq, 80 );
	   	   for(%i = 0; $Spoonbot::SniperGear[%i] != ""; %i++)
	 	       Player::setItemCount(%aiId, $Spoonbot::SniperGear[%i], $SpoonBot::SniperAmmo[%i]);
		   Player::mountItem(%aiId, $SpoonBot::SniperLong, 0);
		   
	    AI::SetVar(%aiName, spotDist, 600);		// Make 20-20 vision for snipers ;-) Werewolf
	    AI::SetVar(%aiName, seekOff , 1);   // Dunno what this does. Werewolf
	}
//=========================================================== Medic Bot Setup
	else if(String::findSubStr(%aiName, "Medic") >= 0)
	{
	   AI::setVar(%aiName, iq, 70 );
	   	  for(%i = 0; $Spoonbot::MedicGear[%i] != ""; %i++)
	 	       Player::setItemCount(%aiId, $Spoonbot::MedicGear[%i], $SpoonBot::MedicAmmo[%i]);
	      Player::mountItem(%aiId, $SpoonBot::MedicLong, 0);
	}
//=========================================================== Miner Bot Setup
	else if(String::findSubStr(%aiName, "Miner") >= 0)
	{
	   	   AI::setVar(%aiName, iq, 70 );
	   	   for(%i = 0; $Spoonbot::MinerGear[%i] != ""; %i++)
	 	       Player::setItemCount(%aiId, $Spoonbot::MinerGear[%i], $SpoonBot::MinerAmmo[%i]);
	       Player::mountItem(%aiId, $SpoonBot::MinerLong, 0);
	}
//=========================================================== Standard Setup	
	else 
	{
	   	   AI::setVar(%aiName, iq, 50 );
	
	   	   for(%i = 0; %i = ""; %i++)
	 	       Player::setItemCount(%aiId, $Spoonbot::StandardGear[%i], $SpoonBot::StandardAmmo[%i]);
	       Player::mountItem(%aiId, $SpoonBot::StandardLong, 0);
	}
}


//=========================================================================================================================================
//							AI::periodicWeaponChange()
//=========================================================================================================================================
//modified by Werewolf

function ai::periodicWeaponChange(%aiName)
{
    %aiId = BotFuncs::GetId(%aiName);
if (%aiId==0)
	return;

	if (!BotFuncs::CheckAlive(%aiId))
	{
		return;
	}


  if((String::findSubStr(%aiName, "Guard") >= 0) || (String::findSubStr(%aiName, "Mortar") >= 0))
	BotFuncs::checkForPaint(%aiName);



if ($Spoonbot::MortarBusy[%aiId] == 1 )
{
  schedule("ai::periodicWeaponChange(" @ %aiName @ ");", 5);
  return;
}


    %curTarget = BotFuncs::NearestAttacker(%aiId, 300, 0);


    %targLoc = GameBase::getPosition(Client::getOwnedObject(%curTarget));
    %aiLoc = GameBase::getPosition(Client::getOwnedObject(%aiId));
    %targetDist = Vector::getDistance(%aiLoc, %targLoc);


   %player = Client::getOwnedObject(%aiId);
   %client = Player::GetClient(%aiId);
   %dlevel = GameBase::getDamageLevel(%player);

   %rk = Player::getItemCount(%client, repairkit);
   if   ((%rk > 0) && (%dlevel > 0.2))
   {
     if (GameBase::getAutoRepairRate(%player) == 0)
     {
       Player::decItemCount(%client,repairkit);
       GameBase::setAutoRepairRate(%player,0.15);
     }
   }else if (%dlevel == 0)
   {
     GameBase::setAutoRepairRate(%player,0);
   }


    if(%targetDist == -1)
    {
	%targetDist = 300;
    }

    if(%targetDist == 0)
    {
	%targetDist = 300;
    }

	
	//============================================================================ Mortar Weapons
	if((String::findSubStr(%aiName, "Mortar") >= 0) && ($Spoonbot::MortarBusy[%aiId] == 0))
	{
		   if(%targetDist > 50)
		   {
		
		      if(Player::isJetting(%curTarget))
		      {	
		         Player::mountItem(%aiId, $SpoonBot::MortarJet, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }
		      else
		      {
				Player::mountItem(%aiId, $SpoonBot::MortarLong, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }   
		
		   }   
		   else
		   {
		         Player::mountItem(%aiId, $SpoonBot::MortarClose, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
		   }
	}


	//============================================================================ Guard Weapons
	if((String::findSubStr(%aiName, "Guard") >= 0) && ($Spoonbot::MortarBusy[%aiId] == 0))
	{
		   if(%targetDist > 50)
		   {
		
		      if(Player::isJetting(%curTarget))
		      {	
		         Player::mountItem(%aiId, $SpoonBot::GuardJet, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }
		      else
		      {
				Player::mountItem(%aiId, $SpoonBot::GuardLong, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }   
		
		   }   
		   else
		   {
		         Player::mountItem(%aiId, $SpoonBot::GuardClose, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
		   }
	}
	
	//============================================================================ Demo Weapons
	else if(String::findSubStr(%aiName, "Demo") >= 0)
	{
		   if(%targetDist > 100)
		   {
		
		      if(Player::isJetting(%curTarget))
		      {	
		         Player::mountItem(%aiId, $SpoonBot::DemoJet, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }
		      else
		      {
				Player::mountItem(%aiId, $SpoonBot::DemoLong, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }   
		   }   
		   else
		   {
		         Player::mountItem(%aiId, $SpoonBot::DemoClose, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
		   }
	}
	
	//============================================================================ Painter Weapons
	else if(String::findSubStr(%aiName, "Painter") >= 0)
	{
		   if(%targetDist > 50)
		   {
		
		      if(Player::isJetting(%curTarget))
		      {	
		         Player::mountItem(%aiId, $SpoonBot::PainterJet, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }
		      else
		      {
		         Player::mountItem(%aiId, $SpoonBot::PainterLong, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }   
		
		   }   
		   else
		   {
		         Player::mountItem(%aiId, $SpoonBot::PainterClose, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
		   }
	}
	
	//============================================================================ Sniper Weapons
	else if(String::findSubStr(%aiName, "Sniper") >= 0)
	{
		   Player::setDetectParameters(%aiId, 0.0001, 0.00001);
	
		   if(%targetDist > 50)
		   {
	
		      if(Player::isJetting(%curTarget))
		      {	
			        Player::mountItem(%aiId, $SpoonBot::SniperJet, 0);
			        AI::SetVar(%aiName, triggerPct, 0.005 );
		      }
		      else
		      {
					Player::mountItem(%aiId, $SpoonBot::SniperLong, 0);
					AI::SetVar(%aiName, triggerPct, 0.005 );
		      }   
		
		   }   
		   else
		   {
		         Player::mountItem(%aiId, $SpoonBot::SniperClose, 0);  
		         AI::SetVar(%aiName, triggerPct, 1 );
		   }
	}
	
	//============================================================================ Miner Weapons
	else if(String::findSubStr(%aiName, "Miner") >= 0)
	{
		   if(%targetDist > 100)
		   {
		
		      if(Player::isJetting(%curTarget))
		      {	
		        	Player::mountItem(%aiId, $SpoonBot::MinerJet, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }
		      else
		      {
		        	Player::mountItem(%aiId, $SpoonBot::MinerLong, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }   
		
		   }   
		   else
		   {
		         Player::mountItem(%aiId, $SpoonBot::MinerClose, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
		   }
	}
	
	//============================================================================ Set Medic Weapons
	else if(String::findSubStr(%aiName, "Medic") >= 0)
	{
		   if(%targetDist > 20)
		   {
		      if(Player::isJetting(%curTarget))
		      {	
		         	Player::mountItem(%aiId, $SpoonBot::MedicJet, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }
		      else
		      {
		         	Player::mountItem(%aiId, $SpoonBot::MedicLong, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
		      }   
		   }   
		   else
		   {
		         Player::mountItem(%aiId, $SpoonBot::MedicClose, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
		   }
	}
	
	
	
	//============================================================================ Set Standard Weapons If Bot Is Not Any Of The Presets
	else 
	{	
	    if(%targetDist > 100)
	    {
	    	 Player::mountItem(%aiId, $SpoonBot::StandardLong, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
	    }   
	    else
	    {
	       if(Player::isJetting(%curTarget))
	       {	
	          Player::mountItem(%aiId, $SpoonBot::StandardJet, 0);
		         AI::SetVar(%aiName, triggerPct, 1000 );
	       }
	       else
	       {
	          Player::mountItem(%aiId, $SpoonBot::StandardLong, 0);  
		         AI::SetVar(%aiName, triggerPct, 1000 );
	       }   
	    }
	}

schedule("ai::periodicWeaponChange(" @ %aiName @ ");", 3);

}
//============================================================================================================== End Of Bot Setup Functions
