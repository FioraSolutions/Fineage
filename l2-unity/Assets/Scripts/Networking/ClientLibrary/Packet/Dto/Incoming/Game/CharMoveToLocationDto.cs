using System;
using System.Collections.Generic;
using UnityEngine;

public class CharMoveToLocationDto : IWireDto
{

    //l2j
    // buffer.writeInt(_objectId);
	//	buffer.writeInt(_xDst);
	//	buffer.writeInt(_yDst);
	//	buffer.writeInt(_zDst);
	//	buffer.writeInt(_x);
	//	buffer.writeInt(_y);
	//	buffer.writeInt(_z);
    public Vector3 NewPosition { get; private set; }
    public Vector3 OldPosition { get; private set; }
    public int ObjId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public CharMoveToLocationDto()
    {
        CreatedAt = DateTime.Now;
    }

    /// <summary>Unity-space positions (already converted). Used by domain / ZoneV2.</summary>
    public static CharMoveToLocationDto FromUnity(int objId, Vector3 destination, Vector3 origin)
    {
        CharMoveToLocationDto dto = new CharMoveToLocationDto();
        dto.ObjId = objId;
        dto.NewPosition = destination;
        dto.OldPosition = origin;
        return dto;
    }

    /// <summary>L2j world ints → Unity via <see cref="VectorUtils"/>.</summary>
    public static CharMoveToLocationDto FromL2(int objId, int destX, int destY, int destZ, int originX, int originY, int originZ)
    {
        return FromUnity(
            objId,
            VectorUtils.ConvertPosToUnity(new Vector3(destX, destY, destZ)),
            VectorUtils.ConvertPosToUnity(new Vector3(originX, originY, originZ)));
    }

    public void ReadFrom(PacketReader reader)
    {
        ObjId = reader.ReadI();
        

        int xDst = reader.ReadI();
        int yDst = reader.ReadI();
        int zDst = reader.ReadI();
        NewPosition = VectorUtils.ConvertPosToUnity(new Vector3(xDst , yDst , zDst));

        int x = reader.ReadI();
        int y = reader.ReadI();
        int z = reader.ReadI();
        OldPosition = VectorUtils.ConvertPosToUnity(new Vector3(x, y, z));
        
    }

  
}
