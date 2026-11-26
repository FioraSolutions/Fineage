class HeroTowerWnd extends UIScript;

function OnLoad()
{
	RegisterEvent(880);
	RegisterEvent(890);
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x60
	if(__NFUN_154__(Event_ID, 880))
	{
		Clear();
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("HeroTowerWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("HeroTowerWnd");
		HandleCheckAmIHero();
		HandleHeroShowList(param);
	}
	return;
}

function OnDBClickListCtrlRecord(string strID)
{
	switch(strID)
	{
		// End:0x1C
		case "lstHero":
			HandleShowDiary();
			// End:0x1F
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x21
		case "btnShowDiary":
			HandleShowDiary();
			// End:0x8F
			break;
		// End:0x8C
		case "btnReg":
			Class'NWindow.HeroTowerAPI'.static.RequestWriteHeroWords(Class'NWindow.UIAPI_EDITBOX'.static.GetString("HeroTowerWnd.txtDiary"));
			Class'NWindow.UIAPI_EDITBOX'.static.SetString("HeroTowerWnd.txtDiary", "");
			// End:0x8F
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleShowDiary()
{
	local LVDataRecord Record;
	local string strTmp;

	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("HeroTowerWnd.lstHero");
	// End:0x74
	if(__NFUN_151__(Record.nReserved1, 0))
	{
		strTmp = __NFUN_112__(__NFUN_112__("_diary?class=", string(Record.nReserved1)), "&page=1");
		RequestBypassToServer(strTmp);
	}
	return;
}

function HandleCheckAmIHero()
{
	local bool bHero;

	bHero = Class'NWindow.UIDATA_PLAYER'.static.IsHero();
	// End:0x6C
	if(bHero)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("HeroTowerWnd.txtDiary");
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("HeroTowerWnd.btnReg");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("HeroTowerWnd.txtDiary");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("HeroTowerWnd.btnReg");
	}
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("HeroTowerWnd.lstHero");
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("HeroTowerWnd.txtDiary", "");
	return;
}

function HandleHeroShowList(string param)
{
	local int i, nMax;
	local string strName;
	local int ClassID;
	local string strPledgeName;
	local int PledgeCrestID;
	local string strAllianceName;
	local int AllianceCrestID, WinCount;
	local Texture texPledge, texAlliance;
	local LVDataRecord Record, recordClear;

	ParseInt(param, "Max", nMax);
	i = 0;
	J0x1C:

	// End:0x5E0 [Loop If]
	if(__NFUN_150__(i, nMax))
	{
		strName = "";
		ClassID = 0;
		strPledgeName = "";
		PledgeCrestID = 0;
		strAllianceName = "";
		AllianceCrestID = 0;
		WinCount = 0;
		ParseString(param, __NFUN_112__("Name_", string(i)), strName);
		ParseInt(param, __NFUN_112__("ClassId_", string(i)), ClassID);
		ParseString(param, __NFUN_112__("PledgeName_", string(i)), strPledgeName);
		ParseInt(param, __NFUN_112__("PledgeCrestId_", string(i)), PledgeCrestID);
		ParseString(param, __NFUN_112__("AllianceName_", string(i)), strAllianceName);
		ParseInt(param, __NFUN_112__("AllianceCrestId_", string(i)), AllianceCrestID);
		ParseInt(param, __NFUN_112__("WinCount_", string(i)), WinCount);
		texPledge = GetPledgeCrestTexFromPledgeCrestID(PledgeCrestID);
		texAlliance = GetAllianceCrestTexFromAllianceCrestID(AllianceCrestID);
		Record = recordClear;
		Record.LVDataList.Length = 5;
		Record.LVDataList[0].szData = strName;
		Record.LVDataList[1].szData = GetClassType(ClassID);
		Record.LVDataList[2].szData = strPledgeName;
		// End:0x485
		if(__NFUN_151__(AllianceCrestID, 0))
		{
			// End:0x3A8
			if(__NFUN_151__(PledgeCrestID, 0))
			{
				Record.LVDataList[2].arrTexture.Length = 2;
				Record.LVDataList[2].arrTexture[0].objTex = texAlliance;
				Record.LVDataList[2].arrTexture[0].X = 10;
				Record.LVDataList[2].arrTexture[0].Y = 0;
				Record.LVDataList[2].arrTexture[0].Width = 8;
				Record.LVDataList[2].arrTexture[0].Height = 12;
				Record.LVDataList[2].arrTexture[0].U = 0;
				Record.LVDataList[2].arrTexture[0].V = 4;
				Record.LVDataList[2].arrTexture[1].objTex = texPledge;
				Record.LVDataList[2].arrTexture[1].X = 18;
				Record.LVDataList[2].arrTexture[1].Y = 0;
				Record.LVDataList[2].arrTexture[1].Width = 16;
				Record.LVDataList[2].arrTexture[1].Height = 12;
				Record.LVDataList[2].arrTexture[1].U = 0;
				Record.LVDataList[2].arrTexture[1].V = 4;				
			}
			else
			{
				Record.LVDataList[2].arrTexture.Length = 1;
				Record.LVDataList[2].arrTexture[0].objTex = texPledge;
				Record.LVDataList[2].arrTexture[0].X = 10;
				Record.LVDataList[2].arrTexture[0].Y = 0;
				Record.LVDataList[2].arrTexture[0].Width = 8;
				Record.LVDataList[2].arrTexture[0].Height = 12;
				Record.LVDataList[2].arrTexture[0].U = 0;
				Record.LVDataList[2].arrTexture[0].V = 4;
			}			
		}
		else
		{
			// End:0x56A
			if(__NFUN_151__(PledgeCrestID, 0))
			{
				Record.LVDataList[2].arrTexture.Length = 1;
				Record.LVDataList[2].arrTexture[0].objTex = texPledge;
				Record.LVDataList[2].arrTexture[0].X = 10;
				Record.LVDataList[2].arrTexture[0].Y = 0;
				Record.LVDataList[2].arrTexture[0].Width = 16;
				Record.LVDataList[2].arrTexture[0].Height = 12;
				Record.LVDataList[2].arrTexture[0].U = 0;
				Record.LVDataList[2].arrTexture[0].V = 4;
			}
		}
		Record.LVDataList[3].szData = strAllianceName;
		Record.LVDataList[4].szData = string(WinCount);
		Record.nReserved1 = ClassID;
		Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("HeroTowerWnd.lstHero", Record);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x1C;
	}
	return;
}
