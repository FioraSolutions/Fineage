class Inspector extends Actor
	native
	notplaceable;

// Export UInspector::execInspectorFunc001(FFrame&, void* const)
native function Coords InspectorFunc001(Mesh InMesh, int BoneIndex);

// Export UInspector::execInspectorFunc002(FFrame&, void* const)
native function int InspectorFunc002(Mesh InMesh);

// Export UInspector::execInspectorFunc003(FFrame&, void* const)
native function int InspectorFunc003(Mesh InMesh);

// Export UInspector::execInspectorFunc004(FFrame&, void* const)
native function int InspectorFunc004(Mesh InMesh);

// Export UInspector::execInspectorFunc005(FFrame&, void* const)
native function int InspectorFunc005(Mesh InMesh, int AnimSeqIndex);

// Export UInspector::execInspectorFunc006(FFrame&, void* const)
native function InspectorFunc006(Mesh InMesh, int AnimSeqIndex, int frame);

event int Inspection(int Param1, int Param2, int param3, int param4)
{
	return;
}
