// Dumps WaterVolume brushes of an Interlude map (.unr) to JSON so Unity can build visible water surfaces.
// L2 pools (e.g. Mother Tree basin, 21_19) are NOT StaticMeshes: the surface is the top face of a WaterVolume
// brush, so staticmeshes.json never contained them -> dry basins after bake.
// Run (Java 11+ single-file):  java -cp L2TerrainExtractor-all.jar WaterVolumeDump.java <map.unr> <out.json>
// World (UE units) = Location + (brushLocal - PrePivot)  (volumes in L2 maps are unrotated/unscaled).
// Emits the brush bounding box (min/max); Unity draws the water surface as the top face (max.z).
// The shrimpworks Model parser fails on many L2 brushes, so the FBox is also read raw: it is the first
// field after the (empty) property list, i.e. at export.pos+1: 6 floats + bValid byte.
import io.github.l2terrain.crypto.L2Decryptor;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.ExportedObject;
import net.shrimpworks.unreal.packages.entities.objects.Model;
import net.shrimpworks.unreal.packages.entities.properties.ObjectProperty;
import net.shrimpworks.unreal.packages.entities.properties.Property;
import net.shrimpworks.unreal.packages.entities.properties.StructProperty;
import java.nio.file.*;
import java.util.*;

public class WaterVolumeDump {
    public static void main(String[] a) throws Exception {
        Path tmp = Files.createTempFile("l2wv_", ".u");
        StringBuilder js = new StringBuilder("{\n  \"volumes\": [");
        int n = 0, skipped = 0;
        try {
            L2Decryptor.decryptFile(Path.of(a[0]), tmp);
            byte[] raw = Files.readAllBytes(tmp);
            try (var p = new net.shrimpworks.unreal.packages.Package(new PackageReader(tmp))) {
                Map<String, ExportedObject> by = new HashMap<>();
                for (ExportedObject x : p.objects) if (x != null) by.put(x.name.name, x);
                for (ExportedObject e : p.objects) {
                    if (e == null || !"WaterVolume".equals(e.classIndex.get().name().name)) continue;
                    float[] L = new float[3], P = new float[3];
                    ExportedObject brush = null;
                    for (Property pr : p.object(e).properties) {
                        String nm = pr.name.name;
                        if (pr instanceof StructProperty.VectorProperty v) {
                            if (nm.equals("Location")) { L[0] = v.x; L[1] = v.y; L[2] = v.z; }
                            if (nm.equals("PrePivot")) { P[0] = v.x; P[1] = v.y; P[2] = v.z; }
                        }
                        if (nm.equals("Brush") && pr instanceof ObjectProperty op) {
                            try { brush = by.get(op.value.get().name().name); } catch (Exception ignored) { }
                        }
                    }
                    float[] bb = null;
                    try {
                        Object mo = brush == null ? null : p.object(brush);
                        if (mo instanceof Model m && m.boundingBox != null) {
                            String b = m.boundingBox.toString();
                            var mm = java.util.regex.Pattern.compile("=(-?[0-9.Ee+-]+)").matcher(b);
                            float[] f = new float[6]; int k = 0;
                            while (mm.find() && k < 6) f[k++] = Float.parseFloat(mm.group(1));
                            if (k == 6) bb = f;
                        }
                    } catch (Exception ignored) { }
                    if (bb == null && brush != null && brush.size > 26) {
                        var buf = java.nio.ByteBuffer.wrap(raw, brush.pos + 1, 25).order(java.nio.ByteOrder.LITTLE_ENDIAN);
                        float[] f = new float[6];
                        for (int k = 0; k < 6; k++) f[k] = buf.getFloat();
                        boolean ok = raw[brush.pos + 25] == 1 && f[0] <= f[3] && f[1] <= f[4] && f[2] <= f[5] && Math.abs(f[3] - f[0]) < 200000;
                        if (ok) bb = f;
                    }
                    if (bb == null) {
                        System.err.println("WARN " + e.name.name + ": brush bounds unreadable, skipped");
                        skipped++;
                        continue;
                    }
                    String box = String.format(Locale.ROOT, "\"min\": [%.2f,%.2f,%.2f], \"max\": [%.2f,%.2f,%.2f]",
                        L[0] + bb[0] - P[0], L[1] + bb[1] - P[1], L[2] + bb[2] - P[2],
                        L[0] + bb[3] - P[0], L[1] + bb[4] - P[1], L[2] + bb[5] - P[2]);
                    js.append(n++ == 0 ? "\n" : ",\n").append("    {\"name\": \"").append(e.name.name)
                      .append("\", ").append(box).append("}");
                }
            }
        } finally { Files.deleteIfExists(tmp); }
        js.append("\n  ]\n}\n");
        Files.writeString(Path.of(a[1]), js.toString());
        System.out.println("WaterVolumes: " + n + " written, " + skipped + " skipped -> " + a[1]);
    }
}
