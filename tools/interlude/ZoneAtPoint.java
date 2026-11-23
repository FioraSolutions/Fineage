// Resolves which BSP zone a point sits in, using the level UModel.
// The reader is Model for L2 version 123, licensee 16..36 (Interlude ships
// 19 and 28): Vectors, Points,
// Nodes, Surfs, Verts, then the zone list. Node records are variable length,
// so this does not probe FVert or FBspNode sizes.
//
// The login shot does not read the TSV this writes. LoginVista.json zoneActor
// is InterpolationPoint5's stored Region.Zone. Camera25 (Tag U2Viewport2) is
// the UnrealEd viewport. A failed parse exits 2 and names the model; it does
// not invent a zone.
//
// Run: java -cp L2TerrainExtractor-all.jar ZoneAtPoint.java <map.unr> <out.tsv>
import io.github.l2terrain.crypto.L2Decryptor;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.ExportedObject;
import net.shrimpworks.unreal.packages.entities.objects.Model;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Zone;
import net.shrimpworks.unreal.packages.entities.properties.Property;
import net.shrimpworks.unreal.packages.entities.properties.StructProperty;

import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Locale;
import java.util.Map;

public class ZoneAtPoint {
    static final class Pt {
        final String cls;
        final String name;
        final double x, y, z;
        final int zoneNumber;
        final String regionZone;

        Pt(String cls, String name, double x, double y, double z, int zoneNumber, String regionZone) {
            this.cls = cls;
            this.name = name;
            this.x = x;
            this.y = y;
            this.z = z;
            this.zoneNumber = zoneNumber;
            this.regionZone = regionZone;
        }
    }

    public static void main(String[] a) {
        if (a.length != 2) {
            System.err.println("ERROR usage: ZoneAtPoint <map.unr> <out.tsv>");
            System.exit(2);
        }
        Path tmp = null;
        try {
            tmp = Files.createTempFile("l2zone_", ".u");
            L2Decryptor.decryptFile(Path.of(a[0]), tmp);
            try (var p = new net.shrimpworks.unreal.packages.Package(new PackageReader(tmp))) {
                if (!Model.usesL2Layout(p.version, p.license)) {
                    System.err.println("ERROR " + a[0] + " is package " + p.version + "/" + p.license
                            + "; UModel zone walk is the L2 layout for version 123, licensee 16 through 36"
                            + " (Interlude ships 19 and 28). Refusing to guess a node stride.");
                    System.exit(2);
                }
                ExportedObject modelExport = null;
                for (ExportedObject e : p.objects) {
                    if (e == null || !"Model".equals(e.classIndex.get().name().name)) continue;
                    if (modelExport == null || e.size > modelExport.size) modelExport = e;
                }
				if (modelExport == null) {
					System.err.println("ERROR no Model export in " + a[0]);
					System.exit(2);
				}
				for (ExportedObject e : p.objects) {
					if (e == null || !"Model".equals(e.classIndex.get().name().name)) continue;
					var any = p.object(e);
					if (!(any instanceof Model)) {
						System.err.println("ERROR Model " + e.name.name + " did not parse as UModel");
						System.exit(2);
					}
				}
				var loaded = p.object(modelExport);
                if (!(loaded instanceof Model)) {
                    System.err.println("ERROR Model " + modelExport.name.name + " did not parse as UModel");
                    System.exit(2);
                }
                Model model = (Model) loaded;
                if (model.nodes.isEmpty()) {
                    System.err.println("ERROR Model " + modelExport.name.name + " has no BSP nodes"
                            + " (vectors=" + model.vectors.size() + " points=" + model.points.size()
                            + " surfs=" + model.surfaces.size() + " verts=" + model.verts.size() + ")");
                    System.exit(2);
                }

                Map<String, Pt> byName = new LinkedHashMap<>();
                List<Pt> queried = new ArrayList<>();
                int unplaced = 0;
                for (ExportedObject e : p.objects) {
                    if (e == null) continue;
                    String cls = e.classIndex.get().name().name;
                    if (!cls.equals("ZoneInfo") && !cls.equals("SkyZoneInfo")
                            && !cls.equals("Camera") && !cls.equals("InterpolationPoint")) continue;
                    float[] loc = null;
                    StructProperty.PointRegionProperty region = null;
                    for (Property pr : p.object(e).properties) {
                        if (pr instanceof StructProperty.VectorProperty v && pr.name.name.equals("Location"))
                            loc = new float[]{v.x, v.y, v.z};
                        if (pr instanceof StructProperty.PointRegionProperty r && pr.name.name.equals("Region"))
                            region = r;
                    }
                    if (region == null) {
                        System.err.println("ERROR " + cls + " " + e.name.name + " is missing Region");
                        System.exit(2);
                    }
                    if (loc == null) {
                        // Editor browsers (TextureBrowser, MeshViewer, …) store no
                        // Location. Actor's default is the origin, which is not a
                        // placed lobby camera, so they are not zoned.
                        unplaced++;
                        continue;
                    }
                    String regionZone = region.zone.index == 0 ? null : region.zone.get().name().name;
                    Pt pt = new Pt(cls, e.name.name, loc[0], loc[1], loc[2], region.zoneNumber & 0xff, regionZone);
                    byName.put(pt.name, pt);
                    queried.add(pt);
                }
                if (queried.isEmpty()) {
                    System.err.println("ERROR no ZoneInfo, Camera, or InterpolationPoint with Location and Region in " + a[0]);
                    System.exit(2);
                }

                List<String> errors = new ArrayList<>();
                int placedRows = 0;
                for (int i = 0; i < model.zones.size(); i++) {
                    String owner = zoneActorName(model, i);
                    if (owner == null) continue;
                    Pt pt = byName.get(owner);
                    if (pt == null) {
                        errors.add("Model " + modelExport.name.name + " zone " + i
                                + " actor " + owner + " has no Location in the map");
                        continue;
                    }
                    int got = Model.pointZone(model.nodes, pt.x, pt.y, pt.z);
                    if (got != i) {
                        errors.add(owner + " at (" + fmt(pt.x) + ", " + fmt(pt.y) + ", " + fmt(pt.z)
                                + ") is Model zone " + i + " but the BSP walk returned " + got
                                + " (" + nameOrMissing(model, got) + ")");
                    }
                }

                StringBuilder out = new StringBuilder("point\tx\ty\tz\tzone_actor\n");
                for (Pt pt : queried) {
                    int got = Model.pointZone(model.nodes, pt.x, pt.y, pt.z);
                    String actor = zoneActorName(model, got);
                    boolean placed = pt.cls.equals("Camera") || pt.cls.equals("InterpolationPoint");
                    if (placed && (got < 0 || got >= model.zones.size() || actor == null)) {
                        errors.add(pt.cls + " " + pt.name + " at ("
                                + fmt(pt.x) + ", " + fmt(pt.y) + ", " + fmt(pt.z)
                                + ") BSP zone " + got + " (" + (actor == null ? "no actor in Model.zones" : actor) + ")");
                    } else if (placed) {
                        // PointRegion on this package is 13 bytes. The UE2 reader
                        // takes a compact index, an int, and one byte, then skips
                        // the other 7. That byte is 32..72 while NumZones is 5, and
                        // the index resolves to TerrainInfo0, so it is not the BSP
                        // slot. The slot is the walk, checked above against each
                        // zone actor's own Location.
                        out.append(pt.name).append('\t')
                           .append(fmt(pt.x)).append('\t').append(fmt(pt.y)).append('\t').append(fmt(pt.z)).append('\t')
                           .append(actor).append('\n');
                        placedRows++;
                    }
                }
                if (!errors.isEmpty()) {
                    System.err.println("ERROR BSP zone walk failed " + errors.size()
                            + " check(s) on Model " + modelExport.name.name
                            + " (nodes=" + model.nodes.size() + " zones=" + model.zones.size() + ")");
                    int shown = Math.min(errors.size(), 30);
                    for (int i = 0; i < shown; i++) System.err.println("ERROR " + errors.get(i));
                    if (errors.size() > shown) {
                        System.err.println("ERROR ... and " + (errors.size() - shown) + " more");
                    }
                    System.exit(2);
                }
                Files.writeString(Path.of(a[1]), out.toString());
                System.out.println("zones self-checked on Model " + modelExport.name.name
                        + " nodes=" + model.nodes.size() + " zones=" + model.zones.size()
                        + " placed=" + placedRows
                        + (unplaced == 0 ? "" : " unplaced=" + unplaced));
                System.out.print(out);
            }
        } catch (Exception ex) {
            System.err.println("ERROR " + causeChain(ex));
            ex.printStackTrace(System.err);
            System.exit(2);
        } finally {
            if (tmp != null) {
                try { Files.deleteIfExists(tmp); } catch (Exception ignored) { }
            }
        }
    }

    static String zoneActorName(Model model, int zi) {
        if (zi < 0 || zi >= model.zones.size()) return null;
        Zone zone = model.zones.get(zi);
        if (zone.zoneActor == null || zone.zoneActor.index == 0) return null;
        return zone.zoneActor.get().name().name;
    }

    static String nameOrMissing(Model model, int zi) {
        String name = zoneActorName(model, zi);
        return name == null ? "no actor in Model.zones[" + zi + "]" : name;
    }

    static String fmt(double v) { return String.format(Locale.ROOT, "%.3f", v); }

    static String causeChain(Throwable ex) {
        StringBuilder chain = new StringBuilder();
        Throwable cur = ex;
        while (cur != null) {
            if (chain.length() > 0) chain.append(" caused by ");
            String msg = cur.getMessage();
            chain.append(cur.getClass().getSimpleName());
            if (msg != null && !msg.isEmpty()) chain.append(": ").append(msg);
            cur = cur.getCause();
        }
        return chain.toString();
    }
}
