using UnityEngine;

public partial class UserGear : Gear
{
    protected override Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = transform.FindRecursive("Weapon_L_Bone");
        }

        return _leftHandBone;
    }

    protected override Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = transform.FindRecursive("Weapon_R_Bone");
        }
        return _rightHandBone;
    }

    protected override Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = transform.FindRecursive("Shield_L_Bone");
        }
        return _shieldBone;
    }

    public void EquipHair(GameObject hair1Piece, GameObject hair2Piece)
    {
        if (Hair1 != null)
            DestroyImmediate(Hair1);
        if (Hair2 != null)
            DestroyImmediate(Hair2);

        Hair1 = hair1Piece;
        Hair2 = hair2Piece;
        Transform tr = _container.transform;
        if (Hair1 != null)
            Hair1.transform.SetParent(tr, false);
        if (Hair2 != null)
            Hair2.transform.SetParent(tr, false);

        _skinnedMeshSync?.SyncMesh();
    }
}
