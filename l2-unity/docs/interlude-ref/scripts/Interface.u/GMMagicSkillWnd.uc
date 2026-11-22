class GMMagicSkillWnd extends MagicSkillWnd;

var bool bShow;

function OnLoad()
{
	RegisterEvent(2300);
	RegisterEvent(2310);
	bShow = false;
	return;
}

function OnShow()
{
	return;
}

function OnHide()
{
	return;
}

function ShowMagicSkill(string a_Param)
{
	// End:0x0E
	if(__NFUN_122__(a_Param, ""))
	{
		return;
	}
	// End:0x37
	if(bShow)
	{
		Clear();
		m_hOwnerWnd.HideWindow();
		bShow = false;		
	}
	else
	{
		Class'NWindow.GMAPI'.static.RequestGMCommand(3, a_Param);
		bShow = true;
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x18
		case 2300:
			HadleGMObservingSkillListStart();
			// End:0x31
			break;
		// End:0x2E
		case 2310:
			HadleGMObservingSkillList(a_Param);
			// End:0x31
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HadleGMObservingSkillListStart()
{
	Clear();
	m_hOwnerWnd.ShowWindow();
	m_hOwnerWnd.SetFocus();
	return;
}

function HadleGMObservingSkillList(string a_Param)
{
	HandleSkillList(a_Param);
	return;
}

function OnClickItem(string strID, int Index)
{
	return;
}

defaultproperties
{
	m_WindowName="GMMagicSkillWnd.InnerWnd"
}