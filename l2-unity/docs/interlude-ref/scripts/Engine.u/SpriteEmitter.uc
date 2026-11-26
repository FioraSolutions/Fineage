class SpriteEmitter extends ParticleEmitter
	native
	editinlinenew;

enum EParticleDirectionUsage
{
	PTDU_None,                      // 0
	PTDU_Up,                        // 1
	PTDU_Right,                     // 2
	PTDU_Forward,                   // 3
	PTDU_Normal,                    // 4
	PTDU_UpAndNormal,               // 5
	PTDU_RightAndNormal,            // 6
	PTDU_Scale                      // 7
};

var(Sprite) SpriteEmitter.EParticleDirectionUsage UseDirectionAs;
var(Sprite) Vector ProjectionNormal;
var(Sprite) int Refraction;
var(Sprite) float RefrUScale;
var(Sprite) float RefrVScale;
var transient Vector RealProjectionNormal;

defaultproperties
{
	ProjectionNormal=(X=0.0000000,Y=0.0000000,Z=1.0000000)
	RefrUScale=0.0600000
	RefrVScale=0.0600000
}