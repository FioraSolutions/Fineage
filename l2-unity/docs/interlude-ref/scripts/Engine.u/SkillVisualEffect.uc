class SkillVisualEffect extends Object
	native
	hidecategories(Object);

cpptext
{
	// trigger the actions	
	void TriggerTransientEffect(class APawn *Owner, class AActor *Target, INT SkillID);
	void TriggerCasting(class APawn *Owner, class AActor *Target);
	void TriggerChanneling(class APawn *Owner, class AActor *Target);
	void TriggerPreshot(class APawn *Owner);
	void TriggerShot(class APawn* Owner);
	void TriggerExplosion(class ANSkillProjectile *Projectile);

}

struct SkillActionInfo
{
	var() editinlinenotify SkillAction Action;
	var() int SpecificStage;
};

var(General) name Desc;
var(Visual) array<SkillActionInfo> CastingActions;
var(Visual) array<SkillActionInfo> ChannelingActions;
var(Visual) array<SkillActionInfo> PreshotActions;
var(Visual) array<SkillActionInfo> ShotActions;
var(Visual) array<SkillActionInfo> ExplosionActions;
var(Visual) float FlyingTime;
