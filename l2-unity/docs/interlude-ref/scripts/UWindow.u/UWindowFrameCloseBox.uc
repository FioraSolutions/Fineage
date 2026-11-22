class UWindowFrameCloseBox extends UWindowButton;

function Created()
{
	bNoKeyboard = true;
	super.Created();
	return;
}

function Click(float X, float Y)
{
	ParentWindow.Close();
	return;
}

function KeyDown(int Key, float X, float Y)
{
	return;
}
