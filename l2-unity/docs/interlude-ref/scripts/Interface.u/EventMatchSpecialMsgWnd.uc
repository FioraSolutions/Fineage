class EventMatchSpecialMsgWnd extends UICommonAPI;

const TIMERID_Hide = 1;

var TextureHandle MessageTex;

function OnLoad()
{
	MessageTex = TextureHandle(GetHandle("MsgTex"));
	RegisterEvent(2270);
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 2270:
			HandleEventMatchGMMessage(a_Param);
			// End:0x23
			break;
		// End:0xFFFF
		default:
			// End:0x23
			break;
			break;
	}
	return;
}

function OnTimer(int a_TimerID)
{
	switch(a_TimerID)
	{
		// End:0x2D
		case 1:
			m_hOwnerWnd.HideWindow();
			m_hOwnerWnd.KillTimer(1);
			// End:0x33
			break;
		// End:0xFFFF
		default:
			// End:0x33
			break;
			break;
	}
	return;
}

function HandleEventMatchGMMessage(string a_Param)
{
	local int Type;
	local string Message, TextureName;
	local int TextureWidth, TextureHeight;

	ParseInt(a_Param, "Type", Type);
	ParseString(a_Param, "Message", Message);
	switch(byte(Type))
	{
		// End:0x82
		case 1:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_finish";
			TextureWidth = 512;
			TextureHeight = 256;
			// End:0x28E
			break;
		// End:0xCB
		case 2:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_start";
			TextureWidth = 512;
			TextureHeight = 512;
			// End:0x28E
			break;
		// End:0x117
		case 3:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_gameover";
			TextureWidth = 512;
			TextureHeight = 256;
			// End:0x28E
			break;
		// End:0x161
		case 4:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_count1";
			TextureWidth = 256;
			TextureHeight = 256;
			// End:0x28E
			break;
		// End:0x1AB
		case 5:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_count2";
			TextureWidth = 256;
			TextureHeight = 256;
			// End:0x28E
			break;
		// End:0x1F5
		case 6:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_count3";
			TextureWidth = 256;
			TextureHeight = 256;
			// End:0x28E
			break;
		// End:0x23F
		case 7:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_count4";
			TextureWidth = 256;
			TextureHeight = 256;
			// End:0x28E
			break;
		// End:0x289
		case 8:
			TextureName = "L2UI_CH3.BroadcastObs.br_msg1_count5";
			TextureWidth = 256;
			TextureHeight = 256;
			// End:0x28E
			break;
		// End:0xFFFF
		default:
			return;
			break;
	}
	MessageTex.SetWindowSize(TextureWidth, TextureHeight);
	MessageTex.SetTextureSize(TextureWidth, TextureHeight);
	MessageTex.SetTexture(TextureName);
	m_hOwnerWnd.ShowWindow();
	m_hOwnerWnd.KillTimer(1);
	m_hOwnerWnd.SetTimer(1, 5000);
	return;
}
