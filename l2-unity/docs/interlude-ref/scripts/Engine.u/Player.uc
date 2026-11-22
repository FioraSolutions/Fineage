class Player extends Object
	native
	config
	noexport;

const IDC_ARROW = 0;
const IDC_SIZEALL = 1;
const IDC_SIZENESW = 2;
const IDC_SIZENS = 3;
const IDC_SIZENWSE = 4;
const IDC_SIZEWE = 5;
const IDC_WAIT = 6;

var native const int vfOut;
var native const int vfExec;
var const transient PlayerController Actor;
var transient Interaction Console;
var const transient bool bWindowsMouseAvailable;
var bool bShowWindowsMouse;
var bool bSuspendPrecaching;
var const transient float WindowsMouseX;
var const transient float WindowsMouseY;
var int CurrentNetSpeed;
var globalconfig int ConfiguredInternetSpeed;
var globalconfig int ConfiguredLanSpeed;
var byte SelectedCursor;
var transient InteractionMaster InteractionMaster;
var transient array<Interaction> LocalInteractions;
var transient BaseGUIController GUIController;
