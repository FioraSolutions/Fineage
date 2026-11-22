// Resolves which BSP zone each Camera actor of an Interlude map (.unr) sits in.
// The login vista fog is the fog of Camera25's zone, and Lobby has three
// ZoneInfo actors with disagreeing ranges, so a per-map heuristic cannot pick
// one — only the BSP point-zone query the engine itself runs is authoritative.
//
// The level's UModel serializes (UE2 order — UE2/UE3 UModel::Serialize and the
// L2-specific L2MapExtract template agree; BrushDump relies on the same header
// for brushes):
//   None(1) BoundingBox(25) BoundingSphere(16)
//   TArray Vectors | TArray Points | TArray Nodes | TArray Surfs | TArray Verts
//   NumSharedSides(4) NumZones(4) Zones[nz] Polys(compact)
// Nodes are walked: d = dot(plane, P) - w; descend iFront/iBack while the child
// index is >= 0, else the leaf zone is iZone[side].
//
// The FBspNode layout is not taken on faith: three candidate families are
// parsed and each is validated by querying every ZoneInfo-family actor at its
// own stored Location — UnrealEd places that point inside its zone, so a
// correct layout must return a valid, distinct leaf zone per actor:
//   SEQ   L2-custom variable nodes (L2MapExtract unreal_package.bt): compact
//         indices, a mid fixed block, a trailing pad — both size-scanned.
//   MAP_A classic UE1-432 serialized FBspNode (64 bytes, iZone as bytes).
//   MAP_B UE2-style FBspNode with INT iZone[2] right after ZoneMask.
// Zone index -> actor name pairs each distinct leaf index with the ZoneInfo
// whose Location produced it, so the Zones[] tail never needs parsing.
// No candidate passing => hard error with diagnostics, never a guessed zone.
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

    static final class NodeRec {
        float nx, ny, nz, w;
        int front, back;           // child node indices, <0 = leaf
        int zoneFront, zoneBack;   // leaf zone indices per side
    }

    static final class Layout {
        String desc; NodeRec[] nodes; int nodesEnd;
        Layout(String d, NodeRec[] n, int e) { desc = d; nodes = n; nodesEnd = e; }
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
                // Vectors(12/item), Points(12/item), Nodes (UE2 array order:
                // Nodes come before Surfs and Verts — the classic mistake is
                // reading the node count as the vert count).
                int[] P = {model.pos + 1 + 25 + 16};
                int nv = compactAt(P); P[0] += nv * 12;
                int np = compactAt(P); P[0] += np * 12;
                int nn = compactAt(P);
                int nodesStart = P[0];
                long modelEnd = (long) model.pos + model.size;
                if (nn <= 0 || nn > 500000 || nodesStart >= modelEnd)
                    throw new IllegalStateException("bad node count " + nn + " at " + nodesStart);

                Layout hit = null;
                boolean inv = false;
                List<String> attempts = new ArrayList<>();

                // Family SEQ: L2-custom variable-size nodes (L2MapExtract).
                // plane(16) zone_mask(8) node_flags(1) vert_pool(c) surf(c)
                // back(c) front(c) i_plane(c) collision_bound(c) render_bound(c)
                // mid[MID] zone[0](c) zone[1](c) num_vertices(1) leaf0(4) leaf1(4) trail[TRAIL]
                outer:
                for (int mid : new int[]{32, 0, 16}) {
                    for (int trail : new int[]{12, 0, 4, 8, 16, 20, 24}) {
                        for (boolean flip : new boolean[]{false, true}) {
                            Layout L = parseSeq(nodesStart, nn, mid, trail, modelEnd);
                            if (L == null) continue;
                            String why = selfCheck(L, zones, flip);
                            if (why == null) { hit = L; inv = flip; break outer; }
                            if (attempts.size() < 16) attempts.add("seq mid=" + mid + " trail=" + trail + (flip ? " inv" : "") + ": " + why);
                        }
                    }
                }
                // Family MAP_A: classic 432 serialized FBspNode, iZone bytes at 52/53.
                if (hit == null) outer:
                for (int stride : new int[]{64, 60, 68, 72, 80}) {
                    for (boolean flip : new boolean[]{false, true}) {
                        Layout L = parseFixedA(nodesStart, nn, stride, modelEnd);
                        if (L == null) continue;
                        String why = selfCheck(L, zones, flip);
                        if (why == null) { hit = L; inv = flip; break outer; }
                        if (attempts.size() < 16) attempts.add("mapA stride=" + stride + (flip ? " inv" : "") + ": " + why);
                    }
                }
                // Family MAP_B: UE2-style FBspNode, INT iZone[2] after ZoneMask.
                if (hit == null) outer:
                for (int stride = 48; stride <= 80; stride += 4) {
                    for (boolean flip : new boolean[]{false, true}) {
                        Layout L = parseFixedB(nodesStart, nn, stride, modelEnd);
                        if (L == null) continue;
                        String why = selfCheck(L, zones, flip);
                        if (why == null) { hit = L; inv = flip; break outer; }
                        if (attempts.size() < 16) attempts.add("mapB stride=" + stride + (flip ? " inv" : "") + ": " + why);
                    }
                }
                if (hit == null) {
                    System.err.println("ERROR no FBspNode layout passed the ZoneInfo self-check.");
                    System.err.println("vectors=" + nv + " points=" + np + " nodes=" + nn
                            + " modelPos=" + model.pos + " modelSize=" + model.size);
                    for (String s : attempts) System.err.println("  " + s);
                    System.exit(2);
                }

                // Tail sanity: the Surfs array count must follow the nodes.
                int[] T = {hit.nodesEnd};
                if (T[0] >= modelEnd) {
                    System.err.println("ERROR layout " + hit.desc + " ends past the model payload");
                    System.exit(2);
                }
                int ns = compactAt(T);
                if (ns < 0 || ns > 500000) {
                    System.err.println("ERROR layout " + hit.desc + " leaves an implausible surfs count " + ns);
                    System.exit(2);
                }

                Map<Integer, String> zoneNames = new LinkedHashMap<>();
                for (Pt z : zones) zoneNames.putIfAbsent(query(hit, z.x, z.y, z.z, inv), z.name);

                StringBuilder out = new StringBuilder("point\tx\ty\tz\tzone_actor\n");
                for (Pt c : cams) {
                    int zi = query(hit, c.x, c.y, c.z, inv);
                    String zn = zoneNames.get(zi);
                    out.append(c.name).append('\t')
                       .append(fmt(c.x)).append('\t').append(fmt(c.y)).append('\t').append(fmt(c.z)).append('\t')
                       .append(zn != null ? zn : "zone#" + zi).append('\n');
                }
                Files.writeString(Path.of(a[1]), out.toString());
                System.out.println("layout: " + hit.desc + (inv ? ", inverted sides" : "") + ", surfs=" + ns);
                System.out.println("zones self-checked: " + zoneNames.values());
                System.out.print(out);
            }
        } finally { Files.deleteIfExists(tmp); }
    }

    static String fmt(double v) { return String.format(Locale.ROOT, "%.3f", v); }

    // Family SEQ: variable-size L2 nodes; null when the sequential parse
    // desyncs (runs past the payload before all nodes are read).
    static Layout parseSeq(int start, int nn, int mid, int trail, long modelEnd) {
        NodeRec[] nodes = new NodeRec[nn];
        int[] P = {start};
        for (int i = 0; i < nn; i++) {
            if (P[0] + 25 + 9 + mid + 2 + 1 + 8 + trail > modelEnd) return null;
            NodeRec n = new NodeRec();
            n.nx = f32At(P); n.ny = f32At(P); n.nz = f32At(P); n.w = f32At(P);
            P[0] += 8;      // zone_mask
            P[0] += 1;      // node_flags
            compactAt(P);   // vert_pool
            compactAt(P);   // surf
            n.back = compactAt(P);
            n.front = compactAt(P);
            compactAt(P);   // i_plane
            compactAt(P);   // collision_bound
            compactAt(P);   // render_bound
            P[0] += mid;
            n.zoneBack = compactAt(P);
            n.zoneFront = compactAt(P);
            P[0] += 1;      // num_vertices
            P[0] += 8;      // leaf[2]
            P[0] += trail;
            nodes[i] = n;
        }
        return new Layout("seq mid=" + mid + " trail=" + trail, nodes, P[0]);
    }

    // Family MAP_A: classic 432 FBspNode (64 bytes): plane(16) zoneMask(8)
    // iVertPool(4) iSurf(4) iBack(4) iFront(4) iPlane(4) iCollisionBound(4)
    // iRenderBound(4) iZone u8[2] NumVertices u8 NodeFlags u8 iLeaf[2](8).
    static Layout parseFixedA(int start, int nn, int stride, long modelEnd) {
        if (stride < 64 || start + (long) nn * stride > modelEnd) return null;
        NodeRec[] nodes = new NodeRec[nn];
        for (int i = 0; i < nn; i++) {
            int[] P = {start + i * stride};
            NodeRec n = new NodeRec();
            n.nx = f32At(P); n.ny = f32At(P); n.nz = f32At(P); n.w = f32At(P);
            P[0] = start + i * stride + 32;
            n.back = i32At(P); n.front = i32At(P);
            P[0] = start + i * stride + 52;
            n.zoneBack = raw[P[0]] & 0xff;
            n.zoneFront = raw[P[0] + 1] & 0xff;
            nodes[i] = n;
        }
        return new Layout("mapA stride=" + stride, nodes, start + nn * stride);
    }

    // Family MAP_B: UE2-style FBspNode: plane(16) zoneMask(8) iZone INT[2]
    // iVertPool(4) iSurf(4) iFront(4) iBack(4) ... (stride-scanned tail).
    static Layout parseFixedB(int start, int nn, int stride, long modelEnd) {
        if (stride < 48 || start + (long) nn * stride > modelEnd) return null;
        NodeRec[] nodes = new NodeRec[nn];
        for (int i = 0; i < nn; i++) {
            int[] P = {start + i * stride + 16};
            NodeRec n = new NodeRec();
            int[] Q = {start + i * stride};
            n.nx = f32At(Q); n.ny = f32At(Q); n.nz = f32At(Q); n.w = f32At(Q);
            P[0] += 8; // zoneMask
            n.zoneFront = i32At(P);   // "Index of the zones in front, in back"
            n.zoneBack = i32At(P);
            P[0] += 8; // iVertPool, iSurf
            n.front = i32At(P);
            n.back = i32At(P);
            nodes[i] = n;
        }
        return new Layout("mapB stride=" + stride, nodes, start + nn * stride);
    }

    // Walk nodes from the root; leaf zone index, or -1 when the walk leaves the model.
    static int query(Layout L, double x, double y, double z, boolean inv) {
        int idx = 0;
        for (int guard = 0; guard < 100000; guard++) {
            if (idx < 0 || idx >= L.nodes.length) return -1;
            NodeRec n = L.nodes[idx];
            double d = n.nx * x + n.ny * y + n.nz * z - n.w;
            boolean front = (d >= 0) != inv;
            int child = front ? n.front : n.back;
            if (child >= 0) {
                idx = child;
            } else {
                return front ? n.zoneFront : n.zoneBack;
            }
        }
        return -1;
    }

    // Every ZoneInfo at its own Location must resolve to a valid leaf zone
    // (UE2 caps zones at MAX_MODELZONES=64), and a multi-zone map must yield
    // more than one distinct index — a wrong node layout collapses the walks
    // into out-of-range indices or a single degenerate leaf.
    static String selfCheck(Layout L, List<Pt> zones, boolean inv) {
        Set<Integer> seen = new HashSet<>();
        for (Pt z : zones) {
            int zi = query(L, z.x, z.y, z.z, inv);
            if (zi < 0) return z.name + " walk failed";
            if (zi >= 64) return z.name + " zone index " + zi + " out of range";
            seen.add(zi);
        }
        if (zones.size() >= 2 && seen.size() < 2) return "all zone queries collapsed to zone " + seen;
        return null;
    }
}
