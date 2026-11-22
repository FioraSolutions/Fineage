// Resolves which BSP zone each Camera actor of an Interlude map (.unr) sits in.
// The login vista fog is the fog of Camera25's zone, and Lobby has three
// ZoneInfo actors with disagreeing ranges, so a per-map heuristic cannot pick
// one — only the BSP point-zone query the engine itself runs is authoritative.
//
// The level's UModel serializes (same layout BrushDump relies on for brushes):
//   None(1) BoundingBox(25) BoundingSphere(16)
//   TArray Vectors | TArray Points | TArray Verts | TArray Nodes | TArray Surfs
//   NumSharedSides(4) NumZones(4) Zones[nz](compact object refs) Polys(compact)
// Nodes are walked: d = dot(plane, P) - w; side front when d >= 0; descend
// iFront/iBack while >= 0, else the leaf zone is iZone[side].
//
// FVert and FBspNode serialized sizes are not taken on faith: the tool tries
// candidate sizes and self-checks by querying every ZoneInfo-family actor at
// its own stored Location — UnrealEd places that point inside its zone, so a
// correct layout must return the zone index that actor back-references. Zone
// index -> actor name comes from the model's Zones[] ref list, read after
// NumSharedSides/NumZones; finding it needs Surfs skipped, and FBspSurf has
// variable-length object refs, so instead the self-check pairs each distinct
// leaf index with the ZoneInfo whose Location produced it. No candidate
// passing => hard error with diagnostics, never a guessed zone.
//
// Run: java -cp L2TerrainExtractor-all.jar ZoneAtPoint.java <map.unr> <out.tsv>
import io.github.l2terrain.crypto.L2Decryptor;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.*;
import net.shrimpworks.unreal.packages.entities.properties.*;
import java.nio.*; import java.nio.file.*; import java.util.*;

public class ZoneAtPoint {
    static byte[] raw;

    static int compactAt(int[] P) {
        int b0 = raw[P[0]++] & 0xff; boolean neg = (b0 & 0x80) != 0; int v = b0 & 0x3f;
        if ((b0 & 0x40) != 0) { int sh = 6; int b; do { b = raw[P[0]++] & 0xff; v |= (b & 0x7f) << sh; sh += 7; } while ((b & 0x80) != 0 && sh < 32); }
        return neg ? -v : v;
    }
    static int i32At(int[] P) { int v = ByteBuffer.wrap(raw, P[0], 4).order(ByteOrder.LITTLE_ENDIAN).getInt(); P[0] += 4; return v; }
    static float f32At(int[] P) { return Float.intBitsToFloat(i32At(P)); }

    static final class Pt {
        final String name; final double x, y, z;
        Pt(String name, double x, double y, double z) { this.name = name; this.x = x; this.y = y; this.z = z; }
    }

    static final class Layout {
        int nodeStart, nodeCount, nodeStride;
        boolean invertSides;
    }

    public static void main(String[] a) throws Exception {
        Path tmp = Files.createTempFile("l2zone_", ".u");
        try {
            L2Decryptor.decryptFile(Path.of(a[0]), tmp);
            raw = Files.readAllBytes(tmp);
            try (var p = new net.shrimpworks.unreal.packages.Package(new PackageReader(tmp))) {
                // Level BSP model: the Model export with real payload (brush shells are tiny).
                ExportedObject model = null;
                for (ExportedObject e : p.objects)
                    if (e != null && "Model".equals(e.classIndex.get().name().name))
                        if (model == null || e.size > model.size) model = e;
                if (model == null) throw new IllegalStateException("no Model export in " + a[0]);

                List<Pt> zones = new ArrayList<>(), cams = new ArrayList<>();
                for (ExportedObject e : p.objects) {
                    if (e == null) continue;
                    String cls = e.classIndex.get().name().name;
                    boolean isZone = "ZoneInfo".equals(cls) || "SkyZoneInfo".equals(cls);
                    if (!isZone && !"Camera".equals(cls)) continue;
                    float[] L = null;
                    for (Property pr : p.object(e).properties)
                        if (pr instanceof StructProperty.VectorProperty v && pr.name.name.equals("Location"))
                            L = new float[]{v.x, v.y, v.z};
                    if (L == null) continue;
                    (isZone ? zones : cams).add(new Pt(e.name.name, L[0], L[1], L[2]));
                }
                if (zones.isEmpty()) throw new IllegalStateException("no ZoneInfo with Location found");
                if (cams.isEmpty()) System.err.println("WARN no Camera with Location found");

                // Header: None byte + BoundingBox + BoundingSphere, then
                // Vectors(12/item), Points(12/item), Verts(candidate), Nodes(candidate).
                int[] P = {model.pos + 1 + 25 + 16};
                int nv = compactAt(P); P[0] += nv * 12;
                int np = compactAt(P); P[0] += np * 12;
                int nverts = compactAt(P);
                int afterVertsPos = P[0];
                long modelEnd = (long) model.pos + model.size;

                int[] vertSizes = {12, 24, 16, 20};
                int[] nodeStrides = {59, 63, 67, 61, 55, 51};
                Layout hit = null;
                List<String> attempts = new ArrayList<>();
                for (int vs : vertSizes) {
                    int nodesAt = afterVertsPos + nverts * vs;
                    if (nodesAt + 1 >= modelEnd) { attempts.add("verts=" + vs + ": past model end"); continue; }
                    int[] Q = {nodesAt};
                    int nn = compactAt(Q);
                    if (nn <= 0 || nn > 500000) { attempts.add("verts=" + vs + ": bad node count " + nn); continue; }
                    for (int stride : nodeStrides) {
                        if (nodesAt + (long) nn * stride > modelEnd) continue;
                        for (boolean inv : new boolean[]{false, true}) {
                            Layout L = new Layout();
                            L.nodeStart = Q[0]; L.nodeCount = nn; L.nodeStride = stride; L.invertSides = inv;
                            String why = selfCheck(L, zones);
                            if (why == null) { hit = L; break; }
                            if (attempts.size() < 12) attempts.add("verts=" + vs + " stride=" + stride + (inv ? " inv" : "") + ": " + why);
                        }
                        if (hit != null) break;
                    }
                    if (hit != null) break;
                }
                if (hit == null) {
                    System.err.println("ERROR no FBspNode/FVert layout passed the ZoneInfo self-check.");
                    System.err.println("vectors=" + nv + " points=" + np + " verts=" + nverts);
                    for (String s : attempts) System.err.println("  " + s);
                    System.exit(2);
                }

                Map<Integer, String> zoneNames = new LinkedHashMap<>();
                for (Pt z : zones) zoneNames.putIfAbsent(query(hit, z.x, z.y, z.z), z.name);

                StringBuilder out = new StringBuilder("point\tx\ty\tz\tzone_actor\n");
                for (Pt c : cams) {
                    int zi = query(hit, c.x, c.y, c.z);
                    String zn = zoneNames.get(zi);
                    out.append(c.name).append('\t')
                       .append(fmt(c.x)).append('\t').append(fmt(c.y)).append('\t').append(fmt(c.z)).append('\t')
                       .append(zn != null ? zn : "zone#" + zi).append('\n');
                }
                Files.writeString(Path.of(a[1]), out.toString());
                System.out.println("zones self-checked: " + zoneNames.values() + " (node stride " + hit.nodeStride + (hit.invertSides ? ", inverted sides" : "") + ")");
                System.out.print(out);
            }
        } finally { Files.deleteIfExists(tmp); }
    }

    static String fmt(double v) { return String.format(Locale.ROOT, "%.3f", v); }

    // Walk nodes from the root; leaf zone index, or -1 when the walk leaves the model.
    static int query(Layout L, double x, double y, double z) {
        int idx = 0;
        for (int guard = 0; guard < 100000; guard++) {
            long off = (long) L.nodeStart + (long) idx * L.nodeStride;
            if (off < 0 || off + L.nodeStride > raw.length) return -1;
            int[] P = {(int) off};
            float nx = f32At(P), ny = f32At(P), nz = f32At(P), w = f32At(P);
            P[0] += 8; // ZoneMask
            int z0 = i32At(P), z1 = i32At(P);
            P[0] += 8; // iSurf, iVertPool
            int iFront = i32At(P), iBack = i32At(P);
            double d = nx * x + ny * y + nz * z - w;
            boolean front = d >= 0;
            if (L.invertSides) front = !front;
            int child = front ? iFront : iBack;
            if (child >= 0) {
                if (child >= L.nodeCount) return -1;
                idx = child;
            } else {
                return front ? z0 : z1;
            }
        }
        return -1;
    }

    // Every ZoneInfo at its own Location must resolve to a valid leaf zone
    // (UE2 caps zones at MAX_MODELZONES=64), and a multi-zone map must yield
    // more than one distinct index — a wrong node stride collapses the walks
    // into out-of-range indices or a single degenerate leaf.
    static String selfCheck(Layout L, List<Pt> zones) {
        Set<Integer> seen = new HashSet<>();
        for (Pt z : zones) {
            int zi = query(L, z.x, z.y, z.z);
            if (zi < 0) return z.name + " walk failed";
            if (zi >= 64) return z.name + " zone index " + zi + " out of range";
            seen.add(zi);
        }
        if (zones.size() >= 2 && seen.size() < 2) return "all zone queries collapsed to zone " + seen;
        return null;
    }
}
