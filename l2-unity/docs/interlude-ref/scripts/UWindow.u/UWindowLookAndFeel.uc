class UWindowLookAndFeel extends UWindowBase;

var() Texture active;
var() Texture Inactive;
var() Texture ActiveS;
var() Texture InactiveS;
var() Texture Misc;
var() Region FrameTL;
var() Region FrameT;
var() Region FrameTR;
var() Region FrameL;
var() Region FrameR;
var() Region FrameBL;
var() Region FrameB;
var() Region FrameBR;
var() Color FrameActiveTitleColor;
var() Color FrameInactiveTitleColor;
var() Color HeadingActiveTitleColor;
var() Color HeadingInActiveTitleColor;
var() int FrameTitleX;
var() int FrameTitleY;
var() Region BevelUpTL;
var() Region BevelUpT;
var() Region BevelUpTR;
var() Region BevelUpL;
var() Region BevelUpR;
var() Region BevelUpBL;
var() Region BevelUpB;
var() Region BevelUpBR;
var() Region BevelUpArea;
var() Region MiscBevelTL[4];
var() Region MiscBevelT[4];
var() Region MiscBevelTR[4];
var() Region MiscBevelL[4];
var() Region MiscBevelR[4];
var() Region MiscBevelBL[4];
var() Region MiscBevelB[4];
var() Region MiscBevelBR[4];
var() Region MiscBevelArea[4];
var() Region ComboBtnUp;
var() Region ComboBtnDown;
var() Region ComboBtnDisabled;
var() int ColumnHeadingHeight;
var() Region HLine;
var() Color EditBoxTextColor;
var() int EditBoxBevel;
var() Region TabSelectedL;
var() Region TabSelectedM;
var() Region TabSelectedR;
var() Region TabUnselectedL;
var() Region TabUnselectedM;
var() Region TabUnselectedR;
var() Region TabBackground;
var() float Size_ScrollbarWidth;
var() float Size_ScrollbarButtonHeight;
var() float Size_MinScrollbarHeight;
var() float Size_TabAreaHeight;
var() float Size_TabAreaOverhangHeight;
var() float Size_TabSpacing;
var() float Size_TabXOffset;
var() float Pulldown_ItemHeight;
var() float Pulldown_VBorder;
var() float Pulldown_HBorder;
var() float Pulldown_TextBorder;

function Texture GetTexture(UWindowFramedWindow W)
{
	// End:0x36
	if(W.bStatusBar)
	{
		// End:0x2D
		if(W.IsActive())
		{
			return ActiveS;			
		}
		else
		{
			return InactiveS;
		}		
	}
	else
	{
		// End:0x51
		if(W.IsActive())
		{
			return active;			
		}
		else
		{
			return Inactive;
		}
	}
	return;
}

function Setup()
{
	return;
}

function FW_DrawWindowFrame(UWindowFramedWindow W, Canvas C)
{
	return;
}

function Region FW_GetClientArea(UWindowFramedWindow W)
{
	return;
}

function UWindowBase.FrameHitTest FW_HitTest(UWindowFramedWindow W, float X, float Y)
{
	return;
}

function FW_SetupFrameButtons(UWindowFramedWindow W, Canvas C)
{
	return;
}

function DrawClientArea(UWindowClientWindow W, Canvas C)
{
	return;
}

function Combo_SetupSizes(UWindowComboControl W, Canvas C)
{
	return;
}

function Combo_Draw(UWindowComboControl W, Canvas C)
{
	return;
}

function Combo_GetButtonBitmaps(UWindowComboButton W)
{
	return;
}

function Combo_SetupLeftButton(UWindowComboLeftButton W)
{
	return;
}

function Combo_SetupRightButton(UWindowComboRightButton W)
{
	return;
}

function Checkbox_SetupSizes(UWindowCheckbox W, Canvas C)
{
	return;
}

function Checkbox_Draw(UWindowCheckbox W, Canvas C)
{
	return;
}

function ComboList_DrawBackground(UWindowComboList W, Canvas C)
{
	return;
}

function ComboList_DrawItem(UWindowComboList Combo, Canvas C, float X, float Y, float W, float h, string Text, bool bSelected)
{
	return;
}

function Editbox_SetupSizes(UWindowEditControl W, Canvas C)
{
	return;
}

function Editbox_Draw(UWindowEditControl W, Canvas C)
{
	return;
}

function SB_SetupUpButton(UWindowSBUpButton W)
{
	return;
}

function SB_SetupDownButton(UWindowSBDownButton W)
{
	return;
}

function SB_SetupLeftButton(UWindowSBLeftButton W)
{
	return;
}

function SB_SetupRightButton(UWindowSBRightButton W)
{
	return;
}

function SB_VDraw(UWindowVScrollbar W, Canvas C)
{
	return;
}

function SB_HDraw(UWindowHScrollbar W, Canvas C)
{
	return;
}

function Tab_DrawTab(UWindowTabControlTabArea tab, Canvas C, bool bActiveTab, bool bLeftmostTab, float X, float Y, float W, float h, string Text, bool bShowText)
{
	return;
}

function Tab_GetTabSize(UWindowTabControlTabArea tab, Canvas C, string Text, out float W, out float h)
{
	return;
}

function Tab_SetupLeftButton(UWindowTabControlLeftButton W)
{
	return;
}

function Tab_SetupRightButton(UWindowTabControlRightButton W)
{
	return;
}

function Tab_SetTabPageSize(UWindowPageControl W, UWindowPageWindow P)
{
	return;
}

function Tab_DrawTabPageArea(UWindowPageControl W, Canvas C, UWindowPageWindow P)
{
	return;
}

function Menu_DrawMenuBar(UWindowMenuBar W, Canvas C)
{
	return;
}

function Menu_DrawMenuBarItem(UWindowMenuBar B, UWindowMenuBarItem i, float X, float Y, float W, float h, Canvas C)
{
	return;
}

function Menu_DrawPulldownMenuBackground(UWindowPulldownMenu W, Canvas C)
{
	return;
}

function Menu_DrawPulldownMenuItem(UWindowPulldownMenu M, UWindowPulldownMenuItem item, Canvas C, float X, float Y, float W, float h, bool bSelected)
{
	return;
}

function Button_DrawSmallButton(UWindowSmallButton B, Canvas C)
{
	return;
}

function PlayMenuSound(UWindowWindow W, UWindowBase.MenuSound S)
{
	return;
}

function ControlFrame_SetupSizes(UWindowControlFrame W, Canvas C)
{
	return;
}

function ControlFrame_Draw(UWindowControlFrame W, Canvas C)
{
	return;
}
