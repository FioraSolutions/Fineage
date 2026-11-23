package net.shrimpworks.unreal.packages.entities.objects.geometry;

import java.util.Arrays;

import net.shrimpworks.unreal.packages.PackageReader;

/**
 * A Node. I'm not sure what this is actually for.
 * <p>
 * Data format:
 * <pre>
 *   - [Plane] plane
 *   - [long] zone mask
 *   - [byte] node flags
 *   - [compact int] vert(ex?) pool
 *   - [compact int] surf(ace?)
 *   - [compact int] front
 *   - [compact int] back
 *   - [compact int] plane (?)
 *   - [compact int] collision bound
 *   - [compact int] render bound
 *   - [[byte, byte]] zone
 *   - [byte] num vertices
 *   - [[int, int]] leaf (?)
 * </pre>
 * <p>
 * Lineage 2 (package 123, licensee 19) writes a longer node. {@link #readL2}
 * follows that order: plane, zone mask, flags, then compact vert pool, surf,
 * <em>back</em>, <em>front</em>, plane, collision bound, render bound, an
 * unknown vector and id, connectivity and visibility masks, two compact zone
 * indices (0 = back, 1 = front), vertex count, two leaf ints, and 12 unknown
 * bytes. A fixed stride cannot describe it because the indices are compact.
 */

//		Plane := Read_Struct_Plane(owner, buffer,name+'.Plane');                  xxx Plane
//		ZoneMask := owner.read_qword(buffer,utdmAsValue,name+'.ZoneMask');        xxx QWORD
//		NodeFlags := owner.read_byte(buffer,utdmAsValue,name+'.NodeFlags');       xxx DWORD
//		iVertPool := owner.read_idx(buffer,utdmAsValue,name+'.iVertPool');        xxx DWORD
//		iSurf := owner.read_idx(buffer,utdmAsValue,name+'.iSurf');                xxx FLOAT
//		iFront := owner.read_idx(buffer,utdmAsValue,name+'.iFront');              xxx FLOAT
//		iBack := owner.read_idx(buffer,utdmAsValue,name+'.iBack');                xxx FLOAT
//		iPlane := owner.read_idx(buffer,utdmAsValue,name+'.iPlane');              xxx FLOAT
//		iCollisionBound := owner.read_idx(buffer,utdmAsValue,name+'.iCollisionBound'); xxx WORD
//		iRenderBound := owner.read_idx(buffer,utdmAsValue,name+'.iRenderBound');  xxx INDEX
//		iZone[0,1] := owner.read_byte(buffer,utdmAsValue,name+'.iZone[1]');       xxx QWORD, QWORD
//		NumVertices := owner.read_byte(buffer,utdmAsValue,name+'.NumVertices');   xxx DWORD
//		iLeaf[0,1] := owner.read_dword(buffer,utdmUnknown,name+'.iLeaf[1]');
//		{ Version 117 (UT2003)
//		Plane
//		QWORD
//		2x DWORD
//		4x FLOAT
//		4x DWORD
//		WORD
//		BYTE/INDEX
//		2x QWORD
//		DWORD
//		}

public class Node {

	public final Plane plane;
	public final long zoneMask;
	public final byte nodeFlags;

	public final int vertPool;
	public final int surf;
	public final int front;
	public final int back;
	public final int iPlane;
	public final int collisionBound;
	public final int renderBound;
	public final int[] zone;
	public final byte numVertices;
	public final int[] leaf;

	public Node(
			Plane plane, long zoneMask, byte nodeFlags, int vertPool, int surf, int front, int back, int iPlane,
			int collisionBound, int renderBound, int[] zone, byte numVertices, int[] leaf) {
		this.plane = plane;
		this.zoneMask = zoneMask;
		this.nodeFlags = nodeFlags;
		this.vertPool = vertPool;
		this.surf = surf;
		this.front = front;
		this.back = back;
		this.iPlane = iPlane;
		this.collisionBound = collisionBound;
		this.renderBound = renderBound;
		this.zone = zone;
		this.numVertices = numVertices;
		this.leaf = leaf;
	}

	public Node(PackageReader reader) {
		this(new Plane(reader),
			 reader.readLong(), reader.readByte(),
			 reader.readIndex(), reader.readIndex(), reader.readIndex(), reader.readIndex(), reader.readIndex(), reader.readIndex(),
			 reader.readIndex(),
			 new int[] { reader.readByte() & 0xff, reader.readByte() & 0xff }, reader.readByte(),
			 new int[] { reader.readInt(), reader.readInt() }
		);
	}

	/**
	 * Lineage 2 BSP node (package version 123, licensee 16 through 36).
	 * Zone index 0 is the back side and zone index 1 is the front side.
	 */
	public static Node readL2(PackageReader reader) {
		reader.ensureRemaining(16);
		Plane plane = new Plane(reader);
		reader.ensureRemaining(9);
		long zoneMask = reader.readLong();
		byte nodeFlags = reader.readByte();
		int vertPool = reader.readIndex();
		int surf = reader.readIndex();
		int back = reader.readIndex();
		int front = reader.readIndex();
		int iPlane = reader.readIndex();
		int collisionBound = reader.readIndex();
		int renderBound = reader.readIndex();
		reader.ensureRemaining(12);
		new Vector(reader);
		reader.ensureRemaining(4);
		reader.readInt();
		reader.ensureRemaining(16);
		reader.readLong();
		reader.readLong();
		int zoneBack = reader.readIndex();
		int zoneFront = reader.readIndex();
		reader.ensureRemaining(21);
		byte numVertices = reader.readByte();
		int[] leaf = { reader.readInt(), reader.readInt() };
		for (int i = 0; i < 12; i++) reader.readByte();
		return new Node(plane, zoneMask, nodeFlags, vertPool, surf, front, back, iPlane, collisionBound, renderBound,
						new int[] { zoneBack, zoneFront }, numVertices, leaf);
	}

	@Override
	public String toString() {
		return String.format("[plane=%s, zoneMask=%s, nodeFlags=%s, vertPool=%s, surf=%s, front=%s, back=%s, iPlane=%s, " +
							 "collisionBound=%s, renderBound=%s, zone=%s, numVertices=%s, leaf=%s]",
							 plane, zoneMask, nodeFlags, vertPool, surf, front, back, iPlane,
							 collisionBound, renderBound, Arrays.toString(zone), numVertices, Arrays.toString(leaf));
	}
}
