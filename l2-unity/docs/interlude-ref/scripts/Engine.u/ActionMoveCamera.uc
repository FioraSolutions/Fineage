class ActionMoveCamera extends MatAction
	native
	config;

enum EPathStyle
{
	PATHSTYLE_Linear,               // 0
	PATHSTYLE_Bezier                // 1
};

var(Path) config ActionMoveCamera.EPathStyle PathStyle;
