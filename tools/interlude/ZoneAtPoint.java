// Resolves which BSP zone a point sits in, using the level UModel.
// The reader is Model (package version 123, licensee 19): Vectors, Points,
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
                if (p.license != 19 || p.version < 117 || p.version >= 178) {
                    System.err.println("ERROR " + a[0] + " is package " + p.version + "/" + p.license
                            + "; UModel zone walk is implemented for licensee 19 (L2 Interlude is 123/19)");
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
                    if (loc == null || region == null) {
                        System.err.println("ERROR " + cls + " " + e.name.name + " is missing Location or Region");
                        System.exit(2);
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
                    boolean numberMismatch = got != pt.zoneNumber;
                    boolean nameMismatch = placed && (actor == null || pt.regionZone == null || !pt.regionZone.equals(actor));
                    if (numberMismatch || nameMismatch) {
                        errors.add(pt.cls + " " + pt.name + " at ("
                                + fmt(pt.x) + ", " + fmt(pt.y) + ", " + fmt(pt.z)
                                + ") BSP zone " + got + " (" + (actor == null ? "no actor in Model.zones" : actor) + ")"
                                + "; Region.ZoneNumber is " + pt.zoneNumber
                                + " and Region.Zone is " + (pt.regionZone == null ? "none" : pt.regionZone));
                    } else if (placed) {
                        out.append(pt.name).append('\t')
                           .append(fmt(pt.x)).append('\t').append(fmt(pt.y)).append('\t').append(fmt(pt.z)).append('\t')
                           .append(actor).append('\n');
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
                        + " nodes=" + model.nodes.size() + " zones=" + model.zones.size());
                System.out.print(out);
            }
        } catch (Exception ex) {
            String msg = ex.getMessage() == null ? ex.getClass().getSimpleName() : ex.getMessage();
            System.err.println("ERROR " + msg);
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
}
