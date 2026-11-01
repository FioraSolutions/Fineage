// Dumps BSP Brush actors (CSG_Add / CSG_Subtract) of an Interlude map (.unr) as UnrealEd-style T3D
// "Begin Actor Class=Brush" blocks that L2T3DInfoParser.ParseBrushInfo / L2BrushBuilder understand.
// Interlude-baked tiles only had StaticMeshActors in their t3d -> BSP walk surfaces (e.g. Elven Village
// bridge decks on top of O_E_Brg) were missing. shrimpworks' Model/Polys parsers fail on L2 (ver 123/licensee)
// data, so Model->Polys ref and the FPoly list are read raw:
//   Model (brush):  None(1) BoundingBox(25) BoundingSphere(16) 5x empty TArray(5) NumSharedSides(4) NumZones(4) [zones] Polys(compact)
//   Polys:          None(1) Num(int) Max(int) then FPoly*Num:
//   FPoly:          NumVerts(compact) Base Normal TexU TexV (4x FVector) Verts[n](FVector) PolyFlags(int)
//                   Actor(compact) Texture(compact) ItemName(compact) iLink(compact) iBrushPoly(compact) LightMapScale(float) ?(int)
// Run: java -cp L2TerrainExtractor-all.jar BrushDump.java <map.unr> <out.t3d>
import io.github.l2terrain.crypto.L2Decryptor;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.*;
import net.shrimpworks.unreal.packages.entities.properties.*;
import java.nio.*; import java.nio.file.*; import java.util.*;

public class BrushDump {
    static byte[] raw; static int pos;
    static int compact() {
        int b0 = raw[pos++] & 0xff; boolean neg = (b0 & 0x80) != 0; int v = b0 & 0x3f;
        if ((b0 & 0x40) != 0) { int sh = 6; int b; do { b = raw[pos++] & 0xff; v |= (b & 0x7f) << sh; sh += 7; } while ((b & 0x80) != 0 && sh < 32); }
        return neg ? -v : v;
    }
    static int i32() { int v = ByteBuffer.wrap(raw, pos, 4).order(ByteOrder.LITTLE_ENDIAN).getInt(); pos += 4; return v; }
    static float f32() { return Float.intBitsToFloat(i32()); }
    static float[] vec() { return new float[]{f32(), f32(), f32()}; }
    static String fv(float[] v) { return String.format(Locale.ROOT, "%+013.6f,%+013.6f,%+013.6f", v[0], v[1], v[2]); }

    static String fullName(net.shrimpworks.unreal.packages.Package p, int ref) {
        if (ref == 0) return null;
        try {
            Named n = new ObjectReference(p, ref).get();
            List<String> parts = new ArrayList<>(); parts.add(n.name().name);
            if (n instanceof Import im) {
                ObjectReference o = im.packageIndex; int guard = 0;
                while (o != null && o.index != 0 && guard++ < 6) { Named on = o.get(); parts.add(0, on.name().name); if (on instanceof Import oi) o = oi.packageIndex; else break; }
            }
            return String.join(".", parts);
        } catch (Exception e) { return null; }
    }

    public static void main(String[] a) throws Exception {
        Path tmp = Files.createTempFile("l2bsp_", ".u");
        StringBuilder out = new StringBuilder();
        int brushes = 0, polys = 0, skipped = 0;
        try {
            L2Decryptor.decryptFile(Path.of(a[0]), tmp);
            raw = Files.readAllBytes(tmp);
            try (var p = new net.shrimpworks.unreal.packages.Package(new PackageReader(tmp))) {
                for (ExportedObject e : p.objects) {
                    if (e == null || !"Brush".equals(e.classIndex.get().name().name)) continue;
                    float[] L = {0, 0, 0}, P = {0, 0, 0}; float[] ms = {1, 1, 1}, ps = {1, 1, 1};
                    int csg = 0, flags = 0; ExportedObject model = null;
                    for (Property pr : p.object(e).properties) {
                        String nm = pr.name.name;
                        if (pr instanceof StructProperty.VectorProperty v) {
                            if (nm.equals("Location")) L = new float[]{v.x, v.y, v.z};
                            if (nm.equals("PrePivot")) P = new float[]{v.x, v.y, v.z};
                        } else if (pr instanceof ByteProperty b && nm.equals("CsgOper")) csg = b.value;
                        else if (pr instanceof IntegerProperty ip && nm.equals("PolyFlags")) flags = ip.value;
                        else if (pr instanceof ObjectProperty op && nm.equals("Brush")) { try { model = (ExportedObject) op.value.get(); } catch (Exception ignored) { } }
                        else if (nm.equals("MainScale") || nm.equals("PostScale")) {
                            var m = java.util.regex.Pattern.compile("(?:x|X)=(-?[0-9.Ee+-]+).*?(?:y|Y)=(-?[0-9.Ee+-]+).*?(?:z|Z)=(-?[0-9.Ee+-]+)").matcher(pr.toString());
                            if (m.find()) { float[] s = {Float.parseFloat(m.group(1)), Float.parseFloat(m.group(2)), Float.parseFloat(m.group(3))}; if (nm.equals("MainScale")) ms = s; else ps = s; }
                        }
                    }
                    if (model == null || csg == 0 /*CSG_Active*/ ) { skipped++; continue; }
                    try {
                        pos = model.pos + 1 + 25 + 16;
                        for (int k = 0; k < 5; k++) if (compact() != 0) throw new IllegalStateException("non-empty brush model arrays");
                        i32(); int nz = i32(); if (nz != 0) throw new IllegalStateException("zones " + nz);
                        int polysRef = compact();
                        ExportedObject pe = (ExportedObject) new ObjectReference(p, polysRef).get();
                        pos = pe.pos + 1; int num = i32(); i32();
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < num; i++) {
                            int nv = compact(); float[] base = vec(), nrm = vec(), tu = vec(), tv = vec();
                            float[][] vs = new float[nv][]; for (int k = 0; k < nv; k++) vs[k] = vec();
                            int pf = i32() | flags; compact(); int tex = compact(); compact(); compact(); compact(); f32(); i32();
                            String tn = fullName(p, tex);
                            sb.append("          Begin Polygon").append(tn != null ? " Texture=" + tn : "").append(" Flags=").append(pf).append("\n");
                            // bake MainScale*PostScale (brush-local, around PrePivot) so the Unity builder (Location-PrePivot+v) stays right
                            final float[] PP = P.clone(); final float[] sc = {ms[0] * ps[0], ms[1] * ps[1], ms[2] * ps[2]};
                            java.util.function.Function<float[], float[]> S = v -> new float[]{PP[0] + (v[0] - PP[0]) * sc[0], PP[1] + (v[1] - PP[1]) * sc[1], PP[2] + (v[2] - PP[2]) * sc[2]};
                            sb.append("             Origin   ").append(fv(S.apply(base))).append("\n");
                            sb.append("             Normal   ").append(fv(nrm)).append("\n");
                            sb.append("             TextureU ").append(fv(tu)).append("\n");
                            sb.append("             TextureV ").append(fv(tv)).append("\n");
                            for (float[] v : vs) sb.append("             Vertex   ").append(fv(S.apply(v))).append("\n");
                            sb.append("          End Polygon\n");
                            polys++;
                        }
                        out.append("Begin Actor Class=Brush Name=").append(e.name.name).append("\n");
                        out.append("    CsgOper=").append(csg == 1 ? "CSG_Add" : csg == 2 ? "CSG_Subtract" : "CSG_" + csg).append("\n");
                        if (flags != 0) out.append("    PolyFlags=").append(flags).append("\n");
                        out.append(String.format(Locale.ROOT, "    Location=(X=%f,Y=%f,Z=%f)\n", L[0], L[1], L[2]));
                        out.append("    Begin Brush Name=").append(model.name.name).append("\n       Begin PolyList\n").append(sb).append("       End PolyList\n    End Brush\n");
                        out.append(String.format(Locale.ROOT, "    PrePivot=(X=%f,Y=%f,Z=%f)\n", P[0], P[1], P[2]));
                        out.append("End Actor\n");
                        brushes++;
                    } catch (Exception ex) { System.err.println("WARN " + e.name.name + ": " + ex); skipped++; }
                }
            }
        } finally { Files.deleteIfExists(tmp); }
        Files.writeString(Path.of(a[1]), out.toString());
        System.out.println("Brushes: " + brushes + " (" + polys + " polys), skipped " + skipped + " -> " + a[1]);
    }
}
