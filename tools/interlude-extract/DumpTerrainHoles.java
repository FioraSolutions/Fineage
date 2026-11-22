import com.rost.cruma.core.unreal.ClientSearchPaths;
import com.rost.cruma.core.unreal.UArchive;
import com.rost.cruma.core.unreal.UArchiveLoader;
import com.rost.cruma.core.unreal.UArchiveLoader.USearchConfig;
import com.rost.cruma.core.unreal.UBitSet;
import com.rost.cruma.core.unreal.UTerrain;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

/**
 * Official TerrainInfo hole / size table via Cruma's typed UTerrainInfoActor.
 * The #1670 JSON dump only stores QuadVisibilityBitmap array_size, not the bits.
 *
 * Usage: DumpTerrainHoles &lt;client-root&gt; &lt;out.tsv&gt; [tile...]
 * Also writes sibling quadvisibility/&lt;tile&gt;.quadvisibility.txt (2048 signed words).
 */
public final class DumpTerrainHoles {
    static final String[] DEFAULT_TILES = {
        "16_24", "16_25", "16_26", "17_24", "17_25", "17_26", "18_25", "18_26",
        "19_21", "19_19", "21_19", "21_18", "21_20", "20_19", "20_22",
        "22_22", "22_21", "22_23", "21_21", "21_22", "21_23"
    };

    public static void main(String[] args) throws Exception {
        if (args.length < 2) {
            System.err.println("Usage: DumpTerrainHoles <client-root> <out.tsv> [tile...]");
            System.exit(2);
        }
        Path root = Path.of(args[0]).toAbsolutePath();
        Path out = Path.of(args[1]).toAbsolutePath();
        Path wordsDir = out.getParent().resolve("quadvisibility");
        List<String> tiles = new ArrayList<>();
        for (int i = 2; i < args.length; i++) tiles.add(args[i]);
        if (tiles.isEmpty()) tiles.addAll(List.of(DEFAULT_TILES));
        Files.createDirectories(wordsDir);

        var entries = ClientSearchPaths.read(root);
        if (entries == null || entries.isEmpty()) entries = ClientSearchPaths.DEFAULT;
        List<USearchConfig> configs = new ArrayList<>();
        for (var e : entries) configs.add(new USearchConfig(e.directory(), e.extension()));
        var loader = new UArchiveLoader(root, configs);

        StringBuilder sb = new StringBuilder();
        sb.append("tile\thm_x\thm_y\tcruma_scale\tquad_bits\tquad_hidden\tedge_bits\tedge_true\tlayers\tdeco\n");
        for (String tile : tiles) {
            UArchive a = ExportPackageProperties.load(loader, tile);
            if (a == null) {
                sb.append(tile).append("\tMISSING\n");
                System.err.println("missing " + tile);
                continue;
            }
            UTerrain.UTerrainInfoActor ti = null;
            for (var exp : a.getExportMap()) {
                if ("TerrainInfo".equals(exp.getClassName())) {
                    Object obj = a.getObjectLoader().exportObject(exp);
                    if (obj instanceof UTerrain.UTerrainInfoActor actor) {
                        ti = actor;
                        break;
                    }
                }
            }
            if (ti == null) {
                sb.append(tile).append("\tNO_TI\n");
                System.err.println("no TI " + tile);
                continue;
            }
            UBitSet q = ti.getQuadVisibilityBitmap();
            var ef = UTerrain.UTerrainInfoActor.class.getDeclaredField("edgeTurnBitmap");
            ef.setAccessible(true);
            UBitSet edge = (UBitSet) ef.get(ti);
            int hidden = count(q, false);
            int edgeTrue = count(edge, true);
            @SuppressWarnings("unchecked")
            List<?> layers = (List<?>) UTerrain.UTerrainInfoActor.class.getMethod("getLayers").invoke(ti);
            @SuppressWarnings("unchecked")
            List<?> deco = (List<?>) UTerrain.UTerrainInfoActor.class.getMethod("getDecoLayers").invoke(ti);
            sb.append(String.format(Locale.ROOT, "%s\t%d\t%d\t%s\t%d\t%d\t%d\t%d\t%d\t%d\n",
                tile, ti.getNativeHeightmapX(), ti.getNativeHeightmapY(),
                String.valueOf(ti.getScale()),
                q == null ? 0 : q.size(), hidden,
                edge == null ? 0 : edge.size(), edgeTrue,
                layers == null ? 0 : layers.size(),
                deco == null ? 0 : deco.size()));
            if (q != null) {
                writeWords(wordsDir.resolve(tile + ".quadvisibility.txt"), q);
            } else {
                System.err.println("ERROR no QuadVisibilityBitmap for " + tile);
            }
            System.err.printf(Locale.ROOT, "%s hidden=%d/%d edgeTrue=%d layers=%d deco=%d%n",
                tile, hidden, q == null ? 0 : q.size(), edgeTrue,
                layers == null ? 0 : layers.size(), deco == null ? 0 : deco.size());
        }
        Files.createDirectories(out.getParent());
        Files.writeString(out, sb.toString());
        System.err.println("wrote " + out);
    }

    /** Pack UBitSet into 2048 signed 32-bit words (bit 0 of word 0 is quad 0). */
    static void writeWords(Path out, UBitSet q) throws Exception {
        int size = q.size();
        if (size != 65536) {
            System.err.println("ERROR " + out.getFileName() + " QuadVisibilityBitmap size=" + size + " (need 65536)");
        }
        int words = (size + 31) / 32;
        StringBuilder sb = new StringBuilder(words * 8);
        for (int w = 0; w < words; w++) {
            int word = 0;
            for (int b = 0; b < 32; b++) {
                int i = w * 32 + b;
                if (i < size && q.get(i)) word |= (1 << b);
            }
            sb.append(word).append('\n');
        }
        Files.writeString(out, sb.toString());
    }

    static int count(UBitSet b, boolean wantTrue) {
        if (b == null) return 0;
        int n = 0;
        int size = b.size();
        for (int i = 0; i < size; i++) {
            if (b.get(i) == wantTrue) n++;
        }
        return n;
    }
}
