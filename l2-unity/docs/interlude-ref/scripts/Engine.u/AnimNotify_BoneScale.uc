class AnimNotify_BoneScale extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );

}

enum L2PartialBoneScalerType
{
	LPBST_LINEAR,                   // 0
	LPBST_SIN_SOUT,                 // 1
	LPBST_SIN_FOUT,                 // 2
	LPBST_FIN_SOUT                  // 3
};

var() int BoneIndex;
var() float StartScale;
var() float EndScale;
var() float StartFrameIndex;
var() float EndFrameIndex;
var() bool ApplySubBones;
var() AnimNotify_BoneScale.L2PartialBoneScalerType BoneScalerType;
