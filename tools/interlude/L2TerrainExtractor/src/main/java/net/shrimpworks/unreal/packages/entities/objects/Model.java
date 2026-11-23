package net.shrimpworks.unreal.packages.entities.objects;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import net.shrimpworks.unreal.packages.Package;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.Export;
import net.shrimpworks.unreal.packages.entities.ObjectReference;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Bound;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Leaf;
import net.shrimpworks.unreal.packages.entities.objects.geometry.LightMap;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Node;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Sphere;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Surface;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Vector;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Vert;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Zone;
import net.shrimpworks.unreal.packages.entities.properties.Property;

/**
 * Model class object representation.
 * <p>
 * Data format (refer to individual geometry types for their data formats):
 * <pre>
 *   - [Bound] bounding box
 *   - [Sphere] bounding sphere
 *   - [compact int] vector count
 *   - [[Vector, ...]] vectors
 *   - [compact int] point count
 *   - [[Vector, ...]] points
 *   - [compact int] node count
 *   - [[Node, ...]] nodes
 *   - [compact int] surface count
 *   - [[Surface, ...]] surfaces
 *   - [compact int] vert count
 *   - [[Vert, ...]] verts
 *   - [int] number of shared sides
 *   - [int] zone count
 *   - [[Zone, ...]] zones
 *   - [compact int] (object reference] ref to Polys object
 *   - [compact int] lightmap count
 *   - [[LightMap, ...]] lightmaps
 *   - [compact int] lightbit count (what's a lightbit?)
 *   - [[byte, ...]] lightbits
 *   - [compact int] bound count
 *   - [[Bound, ...]] bounds
 *   - [compact int] leaf hulls count (what's a leaf hull?!)
 *   - [[Int, ...]] leaf hulls
 *   - [compact int] leaves count
 *   - [[Leaf, ...]] leaves
 *   - [compact int] lights count
 *   - [[compact int, ...]] (object references) refs to Light objects
 *   - [int] root outside (??)
 *   - [int] linked (??)
 * </pre>
 */
public class Model extends Object {

	public final Bound boundingBox;
	public final Sphere boundingSphere;

	public final List<Vector> vectors;
	public final List<Vector> points;
	public final List<Node> nodes;
	public final List<Surface> surfaces;
	public final List<Vert> verts;

	public final int numSharedSides;

	public final List<Zone> zones;

	public final ObjectReference polys;

	public final List<LightMap> lightMaps;
	public final List<Byte> lightBits;
	public final List<Bound> bounds;
	public final List<Integer> leafHulls;
	public final List<Leaf> leaves;
	public final List<ObjectReference> lights;

	public final int rootOutside;
	public final int linked;

	public Model(Package pkg, PackageReader reader, Export export, ObjectHeader header, Collection<Property> properties, int dataStart) {
		super(pkg, reader, export, header, properties, dataStart);

		reader.moveTo(dataStart);
		reader.ensureRemaining(64);

		this.boundingBox = new Bound(reader);
		this.boundingSphere = new Sphere(reader);

		// L2 Interlude UModel order (version 123, licensee 16..36, which covers
		// the 19 and 28 packages the client ships): Vectors, Points, Nodes,
		// Surfs, Verts, then numSharedSides, numZones, ZoneProperties, Polys,
		// bounds, leaf hulls, leaves, lights, rootOutside, linked. No light-map
		// array. Nodes are variable length, so a stride probe cannot read them.
		// UEViewer splits a later L2 material layout at licensee 37; that body
		// is not this one.
		boolean l2 = usesL2Layout(pkg.version, pkg.license);
		if (pkg.version >= 117 && !l2) {
			throw new IllegalStateException(
					"UModel for package version " + pkg.version + " licensee " + pkg.license
					+ " is not implemented. The L2 layout is version 123, licensee 16 through 36"
					+ " (Interlude ships 19 and 28). Refusing to guess an FBspNode stride.");
		}

		int vectorCount = arrayCount(reader, "vectors");
		this.vectors = new ArrayList<>(vectorCount);
		for (int i = 0; i < vectorCount; i++) {
			reader.ensureRemaining(16);
			vectors.add(new Vector(reader));
		}

		int pointCount = arrayCount(reader, "points");
		this.points = new ArrayList<>(pointCount);
		for (int i = 0; i < pointCount; i++) {
			reader.ensureRemaining(16);
			points.add(new Vector(reader));
		}

		int nodeCount = arrayCount(reader, "nodes");
		this.nodes = new ArrayList<>(nodeCount);
		for (int i = 0; i < nodeCount; i++) {
			try {
				nodes.add(l2 ? Node.readL2(reader) : new Node(reader));
			} catch (RuntimeException ex) {
				throw new IllegalStateException(
						"Model node " + i + " of " + nodeCount + ": " + ex.getMessage(), ex);
			}
		}

		int surfCount = arrayCount(reader, "surfs");
		this.surfaces = new ArrayList<>(surfCount);
		for (int i = 0; i < surfCount; i++) {
			try {
				surfaces.add(l2 ? Surface.readL2(pkg, reader) : new Surface(pkg, reader));
			} catch (RuntimeException ex) {
				throw new IllegalStateException(
						"Model surface " + i + " of " + surfCount + ": " + ex.getMessage(), ex);
			}
		}

		int vertCount = arrayCount(reader, "verts");
		this.verts = new ArrayList<>(vertCount);
		for (int i = 0; i < vertCount; i++) {
			reader.ensureRemaining(8);
			verts.add(new Vert(reader));
		}

		reader.ensureRemaining(8);
		this.numSharedSides = reader.readInt();

		int zoneAt = reader.currentPosition();
		int zoneCount = reader.readInt();
		if (zoneCount < 0 || zoneCount > 64) {
			throw new IllegalStateException("Model num_zones " + zoneCount + " at offset " + zoneAt);
		}
		this.zones = new ArrayList<>(zoneCount);
		for (int i = 0; i < zoneCount; i++) {
			reader.ensureRemaining(24);
			zones.add(new Zone(pkg, reader));
			// UE1 (<63) and L2 ZoneProperties both store a float after the masks.
			if (pkg.version < 63 || l2) reader.readFloat();
			else if (pkg.version >= 117) reader.readInt();
		}

		this.polys = new ObjectReference(pkg, reader.readIndex());

		if (l2) {
			this.lightMaps = List.of();
			this.lightBits = List.of();
		} else {
			int lightMapCount = arrayCount(reader, "lightMaps");
			this.lightMaps = new ArrayList<>(lightMapCount);
			for (int i = 0; i < lightMapCount; i++) {
				reader.ensureRemaining(32);
				lightMaps.add(new LightMap(pkg, reader));
			}

			int lightBitCount = arrayCount(reader, "lightBits");
			this.lightBits = new ArrayList<>(lightBitCount);
			for (int i = 0; i < lightBitCount; i++) {
				reader.ensureRemaining(1);
				lightBits.add(reader.readByte());
			}
		}

		int boundCount = arrayCount(reader, "bounds");
		this.bounds = new ArrayList<>(boundCount);
		for (int i = 0; i < boundCount; i++) {
			reader.ensureRemaining(25);
			bounds.add(new Bound(reader));
		}

		int leafHullsCount = arrayCount(reader, "leafHulls");
		this.leafHulls = new ArrayList<>(leafHullsCount);
		for (int i = 0; i < leafHullsCount; i++) {
			reader.ensureRemaining(4);
			leafHulls.add(reader.readInt());
		}

		int leavesCount = arrayCount(reader, "leaves");
		this.leaves = new ArrayList<>(leavesCount);
		for (int i = 0; i < leavesCount; i++) {
			reader.ensureRemaining(32);
			leaves.add(new Leaf(pkg, reader));
		}

		int lightsCount = arrayCount(reader, "lights");
		this.lights = new ArrayList<>(lightsCount);
		for (int i = 0; i < lightsCount; i++) {
			reader.ensureRemaining(2);
			lights.add(new ObjectReference(pkg, reader.readIndex()));
		}

		reader.ensureRemaining(8);

		this.rootOutside = reader.readInt();
		this.linked = reader.readInt();

		if (l2) {
			int end = reader.currentPosition();
			int limit = export.pos + export.size;
			if (end > limit) {
				throw new IllegalStateException(
						"Model " + export.name.name + " parse ended at " + end + ", export ends at " + limit);
			}
		}
	}

	/**
	 * Lineage 2 Interlude UModel body. Version 123 with licensee 16 through 36
	 * is the family UEViewer reads as one L2 layout (the next material body
	 * starts at licensee 37). Interlude maps are 123/28 and other client
	 * packages are 123/19; both use this body.
	 */
	public static boolean usesL2Layout(int version, int license) {
		return version == 123 && license >= 16 && license <= 36;
	}

	private static int arrayCount(PackageReader reader, String field) {
		int at = reader.currentPosition();
		int n = reader.readIndex();
		if (n < 0 || n > 500_000) {
			throw new IllegalStateException("Model " + field + " count " + n + " at offset " + at);
		}
		return n;
	}

	/**
	 * Zone number of the leaf containing the point. Front is
	 * {@code dot(plane, P) - w >= 0}. A negative child is a leaf; zone 1 is
	 * the front side and zone 0 is the back side.
	 */
	public static int pointZone(List<Node> nodes, double x, double y, double z) {
		if (nodes.isEmpty()) throw new IllegalStateException("Model has no BSP nodes");
		int idx = 0;
		for (int guard = 0; guard <= nodes.size(); guard++) {
			if (idx < 0 || idx >= nodes.size()) {
				throw new IllegalStateException(
						"BSP node " + idx + " is outside the node array (" + nodes.size() + ")");
			}
			Node node = nodes.get(idx);
			boolean front = node.plane.x * x + node.plane.y * y + node.plane.z * z - node.plane.w >= 0;
			int child = front ? node.front : node.back;
			if (child < 0) return node.zone[front ? 1 : 0];
			idx = child;
		}
		throw new IllegalStateException("BSP walk did not reach a leaf");
	}

	@Override
	public String toString() {
		return String.format(
				"Model [boundingBox=%s, boundingSphere=%s, vectors=%s, points=%s, nodes=%s, " +
				"surfaces=%s, export=%s, properties=%s]",
				boundingBox, boundingSphere, vectors, points, nodes, surfaces, export, properties);
	}
}
