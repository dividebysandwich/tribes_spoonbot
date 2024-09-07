
$SPOONBOT::Version = "1.3";

function Spoonbot::ScreenSize() {
	%res = $pref::videoFullScreenRes;
	if ($pref::VideoFullScreen) {
		%posRes = $Spoonbot::screenSize[%res];
		if (%posRes != "")
			return %posRes;
		}
	return $Spoonbot::screenSize["640x480"];
	}


function Spoonbot::AddNoticeLine(%line) {
	$Spoonbot::notice = $Spoonbot::notice @ " " @ %line @ "\n";
	}

//function MainMenuGui::onOpen() {
//	if ($Spoonbot::ShowStatus == false)
//		return;

if (isObject(MainMenuGui)) {
	$Spoonbot::screenSize["320x240(V)"] = "320 240";
	$Spoonbot::screenSize["400x300(V)"] = "400 300";
	$Spoonbot::screenSize["480x360(V)"] = "480 360";
	$Spoonbot::screenSize["512x384"] = "512 384";
	$Spoonbot::screenSize["640x400"] = "640 400";
	$Spoonbot::screenSize["640x480"] = "640 480";
	$Spoonbot::screenSize["800x600"] = "800 600";
	$Spoonbot::screenSize["1024x768"] = "1024 768";

	if ((isFile("config\\spoonbot.cs")) && ($Spoonbot::SPOONBOTCSLOADED))
	{
		$Spoonbot::status = "Installation verified.";
		Spoonbot::AddNoticeLine("<f2>Welcome to SPOONBOT!<f0>");
		Spoonbot::AddNoticeLine("I strongly recommend you to");
		Spoonbot::AddNoticeLine("read the <f1>README.TXT<f0> in");
		Spoonbot::AddNoticeLine("case any questions arise\n");
		Spoonbot::AddNoticeLine("Don't forget, you can set");
		Spoonbot::AddNoticeLine("your preferences in the file");
		Spoonbot::AddNoticeLine("<f1>config\\spoonbot.cs<f0>");
	}
	else
	{
		$Spoonbot::status = "Installation ERROR!";
		Spoonbot::AddNoticeLine("<f2>ERROR IN SPOONBOT INSTALL<f0>");
		Spoonbot::AddNoticeLine("The file <f1>SPOONBOT.CS<f0> was");
		Spoonbot::AddNoticeLine("not found at\n");
		Spoonbot::AddNoticeLine("<f1>Tribes\config\spoonbot.cs<f0>");
		Spoonbot::AddNoticeLine("This file is needed for");
		Spoonbot::AddNoticeLine("SPOONBOT to run correctly!");
	}








	%screenSize = Spoonbot::ScreenSize();
	%width = getWord(%screenSize,0);
	%height = getWord(%screenSize, 1);

	%boxHeight = 125;

	%height = %height - 200; //hack to place boxes at the top of the screen, thus not overlapping PrestoPack's boxes.

	if (!isObject(SpoonbotStatus))
		newObject(SpoonbotStatus, FearGui::FearGuiBox, 50,%height-45 - %boxHeight, 200,%boxHeight);
	if (!isObject(SpoonbotStatusText))
		newObject(SpoonbotStatusText, FearGuiFormattedText, 1,0,190,400);
	AddToSet(SpoonbotStatus, SpoonbotStatusText);
	AddToSet(MainMenuGui, SpoonbotStatus);

	Control::SetValue(SpoonbotStatusText, 
		"<jc><f0>SPOONBOT <f1>v" @ $SPOONBOT::Version @ "<jl>\n  " @ $Spoonbot::status);

	if ($Spoonbot::notice != "") {
		if (!isObject(SpoonbotNotice))
			newObject(SpoonbotNotice, FearGui::FearGuiBox, %width - 250,%height-45 - %boxHeight, 200,%boxHeight);
		if (!isObject(SpoonbotNoticeText))
			newObject(SpoonbotNoticeText, FearGuiFormattedText, 1,0,190,400);
		AddToSet(SpoonbotNotice, SpoonbotNoticeText);
		AddToSet(MainMenuGui, SpoonbotNotice);
		Control::SetValue(SpoonbotNoticeText, $Spoonbot::notice);
		}
	}

