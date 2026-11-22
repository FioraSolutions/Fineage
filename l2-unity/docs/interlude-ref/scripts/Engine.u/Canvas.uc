class Canvas extends Object
	native
	noexport;

var Font Font;
var float SpaceX;
var float SpaceY;
var float OrgX;
var float OrgY;
var float ClipX;
var float ClipY;
var float CurX;
var float CurY;
var float Z;
var byte Style;
var float CurYL;
var Color DrawColor;
var bool bCenter;
var bool bNoSmooth;
var const int SizeX;
var const int SizeY;
var Font TinyFont;
var Font SmallFont;
var Font MedFont;
var string TinyFontName;
var string SmallFontName;
var string MedFontName;
var const Viewport Viewport;
var const int pCanvasUtil;
var Texture pNormalLeftTex;
var Texture pNormalRightTex;
var Texture pTargetLeftTex;
var Texture pTargetRightTex;
var Texture pAttackLeftTex;
var Texture pAttackRightTex;
var Texture p2DQuestMarkTex;
var Texture pBlueTex;
var Texture pBlueCrossTex;
var Texture pBlueShieldTex;
var Texture pBlueSwordTex;
var Texture pRedTex;
var Texture pRedCrossTex;
var Texture pRedShieldTex;
var Texture pRedSwordTex;
var Texture pBlueAttLeaderTex;
var Texture pBlueLeaderTex;
var Texture pRedAttLeaderTex;
var Texture pRedLeaderTex;
var Texture pRadarTex;
var Texture pMyLocTex;
var Texture pPartyLocTex;
var Texture pTargetLocTex;
var Texture pTutorialLocTex1;
var Texture pTutorialLocTex2;
var Texture pRadarNorthTex;
var Texture pRadarSouthTex;
var Texture pRadarEastTex;
var Texture pRadarWestTex;
var bool m_bForeTutorial;
var bool m_bBroadcastObserverMode;
var bool m_bShowBroadcastObserverTargetName;
var bool m_IsClipped;
var float m_OldCurX;
var float m_OldCurY;
var float m_OldOrgX;
var float m_OldOrgY;
var float m_OldClipX;
var float m_OldClipY;
var Font m_L2Font[3];
var Texture pChatBack1_1;
var Texture pChatBack1_2;
var Texture pChatBack1_3;
var Texture pChatBack1_4;
var Texture pChatBack2_1;
var Texture pChatBack2_2;
var Texture pChatBack2_3;
var Texture pChatBack2_4;
var Texture m_pBroadcastObserverNumberTexRed[9];
var Texture m_pBroadcastObserverNumberTexBlue[9];
var const int m_pViewportWindowRenderTarget;
var float m_fRecordMarkTimer;
var Texture m_pReplayRecordTimerTex;
var float m_FontScaleModifier;
var float m_fTimePerFont;
var float m_fStartTime;
var int m_nTextIndex;
var bool m_bTextAnim;
var bool m_bTextEnd;
var bool m_bFirstTextAnim;

// Export UCanvas::execStrLen(FFrame&, void* const)
native(464) final function StrLen(coerce string String, out float XL, out float YL);

// Export UCanvas::execDrawText(FFrame&, void* const)
native(465) final function DrawText(coerce string Text, optional bool CR);

// Export UCanvas::execDrawTile(FFrame&, void* const)
native(466) final function DrawTile(Material mat, float XL, float YL, float U, float V, float UL, float VL);

// Export UCanvas::execDrawActor(FFrame&, void* const)
native(467) final function DrawActor(Actor A, bool Wireframe, optional bool ClearZ, optional float DisplayFOV);

// Export UCanvas::execDrawTileClipped(FFrame&, void* const)
native(468) final function DrawTileClipped(Material mat, float XL, float YL, float U, float V, float UL, float VL);

// Export UCanvas::execDrawTextClipped(FFrame&, void* const)
native(469) final function DrawTextClipped(coerce string Text, optional bool bCheckHotKey);

// Export UCanvas::execTextSize(FFrame&, void* const)
native(470) final function TextSize(coerce string String, out float XL, out float YL);

// Export UCanvas::execDrawPortal(FFrame&, void* const)
native(480) final function DrawPortal(int X, int Y, int Width, int Height, Actor CamActor, Vector CamLocation, Rotator CamRotation, optional int FOV, optional bool ClearZ);

// Export UCanvas::execWrapStringToArray(FFrame&, void* const)
native final function WrapStringToArray(string Text, out array<string> OutArray, float dx, string EOL);

// Export UCanvas::execDrawTileStretched(FFrame&, void* const)
native final function DrawTileStretched(Material mat, float XL, float YL);

// Export UCanvas::execDrawTileJustified(FFrame&, void* const)
native final function DrawTileJustified(Material mat, byte Justification, float XL, float YL);

// Export UCanvas::execDrawTileScaled(FFrame&, void* const)
native final function DrawTileScaled(Material mat, float XScale, float YScale);

// Export UCanvas::execDrawTextJustified(FFrame&, void* const)
native final function DrawTextJustified(coerce string String, byte Justification, float x1, float y1, float x2, float y2);

event Reset()
{
	Font = default.Font;
	SpaceX = default.SpaceX;
	SpaceY = default.SpaceY;
	OrgX = default.OrgX;
	OrgY = default.OrgY;
	CurX = default.CurX;
	CurY = default.CurY;
	Style = default.Style;
	DrawColor = default.DrawColor;
	CurYL = default.CurYL;
	bCenter = false;
	bNoSmooth = false;
	Z = 1.0000000;
	return;
}

final function SetPos(float X, float Y)
{
	CurX = X;
	CurY = Y;
	return;
}

final function SetOrigin(float X, float Y)
{
	OrgX = X;
	OrgY = Y;
	return;
}

final function SetClip(float X, float Y)
{
	ClipX = X;
	ClipY = Y;
	return;
}

final function DrawPattern(Material Tex, float XL, float YL, float Scale)
{
	__NFUN_466__(Tex, XL, YL, __NFUN_171__(__NFUN_175__(CurX, OrgX), Scale), __NFUN_171__(__NFUN_175__(CurY, OrgY), Scale), __NFUN_171__(XL, Scale), __NFUN_171__(YL, Scale));
	return;
}

final function DrawIcon(Texture Tex, float Scale)
{
	// End:0x6B
	if(__NFUN_119__(Tex, none))
	{
		__NFUN_466__(Tex, __NFUN_171__(float(Tex.USize), Scale), __NFUN_171__(float(Tex.VSize), Scale), 0.0000000, 0.0000000, float(Tex.USize), float(Tex.VSize));
	}
	return;
}

final function DrawRect(Texture Tex, float RectX, float RectY)
{
	__NFUN_466__(Tex, RectX, RectY, 0.0000000, 0.0000000, float(Tex.USize), float(Tex.VSize));
	return;
}

final function SetDrawColor(byte R, byte G, byte B, optional byte A)
{
	local Color C;

	C.R = R;
	C.G = G;
	C.B = B;
	// End:0x47
	if(__NFUN_154__(int(A), 0))
	{
		A = byte(255);
	}
	C.A = A;
	DrawColor = C;
	return;
}

static final function Color MakeColor(byte R, byte G, byte B, optional byte A)
{
	local Color C;

	C.R = R;
	C.G = G;
	C.B = B;
	// End:0x47
	if(__NFUN_154__(int(A), 0))
	{
		A = byte(255);
	}
	C.A = A;
	return C;
	return;
}

final function DrawVertical(float X, float Height)
{
	SetPos(X, CurY);
	DrawRect(Texture'Engine.WhiteSquareTexture', 2.0000000, Height);
	return;
}

final function DrawHorizontal(float Y, float Width)
{
	SetPos(CurX, Y);
	DrawRect(Texture'Engine.WhiteSquareTexture', Width, 2.0000000);
	return;
}

final function DrawLine(int Direction, float Size)
{
	local float X, Y;

	X = CurX;
	Y = CurY;
	switch(Direction)
	{
		// End:0x50
		case 0:
			SetPos(X, __NFUN_175__(Y, Size));
			DrawRect(Texture'Engine.WhiteSquareTexture', 2.0000000, Size);
			// End:0xC0
			break;
		// End:0x6C
		case 1:
			DrawRect(Texture'Engine.WhiteSquareTexture', 2.0000000, Size);
			// End:0xC0
			break;
		// End:0xA0
		case 2:
			SetPos(__NFUN_175__(X, Size), Y);
			DrawRect(Texture'Engine.WhiteSquareTexture', Size, 2.0000000);
			// End:0xC0
			break;
		// End:0xBD
		case 3:
			DrawRect(Texture'Engine.WhiteSquareTexture', Size, 2.0000000);
			// End:0xC0
			break;
		// End:0xFFFF
		default:
			break;
	}
	SetPos(X, Y);
	return;
}

final simulated function DrawBracket(float Width, float Height, float bracket_size)
{
	local float X, Y;

	X = CurX;
	Y = CurY;
	Width = float(__NFUN_250__(int(Width), 5));
	Height = float(__NFUN_250__(int(Height), 5));
	DrawLine(3, bracket_size);
	DrawLine(1, bracket_size);
	SetPos(__NFUN_174__(X, Width), Y);
	DrawLine(2, bracket_size);
	DrawLine(1, bracket_size);
	SetPos(__NFUN_174__(X, Width), __NFUN_174__(Y, Height));
	DrawLine(0, bracket_size);
	DrawLine(2, bracket_size);
	SetPos(X, __NFUN_174__(Y, Height));
	DrawLine(3, bracket_size);
	DrawLine(0, bracket_size);
	SetPos(X, Y);
	return;
}

final simulated function DrawBox(Canvas Canvas, float Width, float Height)
{
	local float X, Y;

	X = Canvas.CurX;
	Y = Canvas.CurY;
	Canvas.DrawRect(Texture'Engine.WhiteSquareTexture', 2.0000000, Height);
	Canvas.DrawRect(Texture'Engine.WhiteSquareTexture', Width, 2.0000000);
	Canvas.SetPos(__NFUN_174__(X, Width), Y);
	Canvas.DrawRect(Texture'Engine.WhiteSquareTexture', 2.0000000, Height);
	Canvas.SetPos(X, __NFUN_174__(Y, Height));
	Canvas.DrawRect(Texture'Engine.WhiteSquareTexture', __NFUN_174__(Width, float(1)), 2.0000000);
	Canvas.SetPos(X, Y);
	return;
}

defaultproperties
{
	Font=Font'Engine.DefaultFont'
	Z=1.0000000
	Style=1
	DrawColor=(R=127,G=127,B=127,A=255)
	TinyFontName="WarfareFonts.FontMono"
	SmallFontName="WarfareFonts.FontMono"
	MedFontName="WarfareFonts.FontMono800x600"
}