// ======================================
// based on DynHud by KillerBunny
// --------------------------------------

// ======================================
// This is the bind to show the HUD, 
// it's set to CTRL+B right now
// but you can change it to whatever
// ======================================

editActionMap("playMap.sae");
bindCommand(keyboard0, make, control, $BotHUD::ToggleKey, TO, "BotHUD::Toggle();");
bindCommand(keyboard0, break, control, $BotHUD::ToggleKey, TO, "");

// ======================================
// BotHUD routines
// ======================================

$BotHUD::Name = "BotHUD";


function BotHUD::DrawHUD(%x2, %y2)
{
	%x1 = 6;
	%y1 = 85;
//	%x2 = 190;
//	%y2 = 230;

	$BotHUD[0] = newObject("BotHUD_Frame", FearGui::FearGuiMenu, %x1, %y1, %x2, %y2);
	$BotHUD[1] = newObject("BotHUD_Main", FearGuiFormattedText, (%x1+3), (%y1+3), (%x2-3), (%y2-3));

	for(%i = 0; $BotHUD[%i] != ""; %i++)
		addToSet(PlayGui, $BotHUD[%i]); 
	Control::setValue("BotHUD_Main", $BotHUD::StatusString);

}

function BotHUD::HideHUD()
{
	$BotHUD::StatusString = " ";
	Control::setValue("BotHUD_Main", $BotHUD::StatusString);

	for(%i = 0; $BotHUD[%i] != ""; %i++)
		deleteObject($BotHUD[%i]);
}



function BotHUD::Create()
{
	if($BotHUD::Loaded == 1)
		return;

	$BotHUD::Loaded = 1;

//	$BotHUD[0] = newObject("BotHUD_Frame", FearGui::FearGuiMenu, %x1, %y1, %x2, %y2);
//	$BotHUD[1] = newObject("BotHUD_Main", FearGuiFormattedText, (%x1+3), (%y1+3), (%x2-3), (%y2-3));
//	for(%i = 0; $BotHUD[%i] != ""; %i++)
//		addToSet(PlayGui, $BotHUD[%i]); 

	BotHUD::DrawHUD(190, 230);

	BotHUD::Heartbeat();
}


function BotHUD::CheckUserTeam()			//Clever method to find out the clients team number.
{
	%startCl = 2049;
	%endCl = %startCl + 90;
	for(%cl = %startCl; (%cl < %endCl); %cl++)
	{
		%name = Client::getName(%cl);
		if (%name == $PCFG::Name)
		{
		     %team = Client::getTeam(%cl);
		     return %team;
		}
	}

}








function BotHUD::UpdateHUD()
{
	if($BotHUD::Loaded != 1)
		return;

	BotHUD::HideHUD();
	%y = 0;
	$BotHUD::StatusString = "<f0>" @ $BotHUD::Name @ " <f3>v" @ $SPOONBOT::Version @ "\n";
	%y = %y + 26;


if ($Spoonbot::BotTree_Design == False)
{
	%startCl = 2049;
	%endCl = %startCl + 90;
	%BotsToDisplay = 0;
	for(%cl = %startCl; (%cl < %endCl); %cl++)
	{
	     %team = Client::getTeam(%cl);
	     %aiName = Client::getName(%cl);

	     if(Player::isAIControlled(%cl))
	     {
		if(%team == BotHUD::CheckUserTeam())			// Find out the team number of the client who plays this.
		{
		%BotsToDisplay = %BotsToDisplay + 1;
		$BotHUD::BotName = %aiName;
		$BotHUD::Status = $Spoonbot::BotStatus[%cl];
		%String = "%1%2";
		%append = "\n<f2>" @ $BotHUD::BotName @ "<f1>\n" @ $BotHUD::Status @ "\n";
		%y = %y + 39;
	        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 
		}
	     }
	}

	BotHUD::DrawHUD(190, %y);
}
else
{

// $BOTTREE::Tree_Count            -        Self Explanatory
// $BOTTREE::Unresolved_Routes     -        Count of unresolved Tree Paths (Fragmentations).
// $BOTTREE::Unresolved_Objects    -        Count of unresolved routes to objects.
// $BOTTREE::Target_Loc            -        Location of current object to be resolved
// $BOTTREE::User_Loc              -        Location of Player
// $BOTTREE::Target_Distance       -        Guess?
// $BOTTREE::Route_Updated         -        Indicates if tree route calculation up to date.


	%String = "%1%2";
	%append = "\n<f2>Tree_count:<f1> " @ $BOTTREE::Tree_count;
	%y = %y + 19;
        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 

	%String = "%1%2";
	%append = "\n<f2>Unresolved_Routes:<f1> " @ $BOTTREE::Unresolved_Routes;
	%y = %y + 19;
        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 

	%String = "%1%2";
	%append = "\n<f2>Unresolved_Objects:<f1> " @ $BOTTREE::Unresolved_Objects;
	%y = %y + 19;
        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 

	%String = "%1%2";
	%append = "\n<f2>Target_Loc:<f1> " @ $BOTTREE::Target_Loc;
	%y = %y + 19;
        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 

	%String = "%1%2";
	%append = "\n<f2>User_Loc:<f1> " @ $BOTTREE::User_Loc;
	%y = %y + 19;
        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 

	%String = "%1%2";
	%append = "\n<f2>Target_Distance:<f1> " @ $BOTTREE::Target_Distance;
	%y = %y + 19;
        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 

	%String = "%1%2";

//	%append = "\n<f2>Route_Updated:<f1> " @ $BOTTREE::Route_Updated;
//I assume you want a True/False display here instead.

	if ($BOTTREE::Route_Updated == 1)
	{
	 %append = "\n<f2>Route_Updated:<f1> True";
	}
	else
	{
	 %append = "\n<f2>Route_Updated:<f1> False";
	}
	%y = %y + 19;
        $BotHUD::StatusString = sprintf(%String,$BotHUD::StatusString,%append); 

	BotHUD::DrawHUD(190, %y);	//%y is the box height. I assumed 19 pixels per line. Could be too much, but it doesn't really matter.


}

}

function BotHUD::Heartbeat()
{
	if($BotHUD::Loaded == 0)
		return;

	schedule("BotHUD::Heartbeat();", 2);
	BotHUD::UpdateHUD();
}

function BotHUD::Remove()
{
	if($BotHUD::Loaded == 0)
		return;

	$BotHUD::Loaded = 0;

	$BotHUD::StatusString = " ";
	Control::setValue("BotHUD_Main", $BotHUD::StatusString);

	for(%i = 0; $BotHUD[%i] != ""; %i++)
		deleteObject($BotHUD[%i]);
}

function BotHUD::Toggle()
{
	if($BotHUD::Loaded == 1)
		BotHUD::Remove();
	else
		BotHUD::Create();
}

function BotHUD::Reset()
{
	$BotHUD::StatusString = $BotHUD::Name @ "\n";
}

