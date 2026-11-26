class FileLog extends Info
	native
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var int LogAr;
var string LogFileName;

// Export UFileLog::execOpenLog(FFrame&, void* const)
native final function OpenLog(string FName);

// Export UFileLog::execCloseLog(FFrame&, void* const)
native final function CloseLog();

// Export UFileLog::execLogf(FFrame&, void* const)
native final function Logf(string LogString);

event Destroyed()
{
	CloseLog();
	return;
}
