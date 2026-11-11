using UnityEngine;

public sealed partial class ItemDropLayerService
{
    public const string DropItemLayerName = "DropItem";
    int _dropItemLayer = int.MinValue;

    public int Layer
    {
        get
        {
            if (_dropItemLayer == int.MinValue)
            {
                _dropItemLayer = LayerMask.NameToLayer(DropItemLayerName);
                if (_dropItemLayer < 0)
                    _dropItemLayer = LayerMask.NameToLayer("EntityClick");
            }

            return _dropItemLayer;
        }
    }

    public LayerMask Mask
    {
        get
        {
            int layer = Layer;
            return layer >= 0 ? (1 << layer) : 0;
        }
    }







}
