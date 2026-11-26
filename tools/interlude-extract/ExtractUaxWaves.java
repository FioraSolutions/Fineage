import com.rost.cruma.core.unreal.ClientSearchPaths;
import com.rost.cruma.core.unreal.UArchive;
import com.rost.cruma.core.unreal.UArchiveLoader;
import com.rost.cruma.core.unreal.UArchiveLoader.USearchConfig;
import com.rost.cruma.core.unreal.UObjectExport;
import com.rost.cruma.core.unreal.USound;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

/**
 * Writes official Sound wave payloads from a .uax into WAV/OGG files.
 * Does not copy the package binary. Payload format is preserved.
 *
 * Usage: ExtractUaxWaves &lt;client-root&gt; &lt;out-dir&gt; &lt;package&gt;...
 */
public final class ExtractUaxWaves {
    public static void main(String[] args) throws Exception {
        if (args.length < 3) {
            System.err.println("Usage: ExtractUaxWaves <client-root> <out-dir> <package>...");
            System.exit(2);
        }
        Path root = Path.of(args[0]).toAbsolutePath();
        Path outDir = Path.of(args[1]).toAbsolutePath();
        Files.createDirectories(outDir);
        List<String> names = new ArrayList<>();
        for (int i = 2; i < args.length; i++) names.add(args[i]);

        var entries = ClientSearchPaths.read(root);
        if (entries == null || entries.isEmpty()) entries = ClientSearchPaths.DEFAULT;
        List<USearchConfig> configs = new ArrayList<>();
        for (var e : entries) configs.add(new USearchConfig(e.directory(), e.extension()));
        var loader = new UArchiveLoader(root, configs);

        int ok = 0, missing = 0, failed = 0, waves = 0;
        for (String raw : names) {
            String logical = ExportClientPackages.logicalName(raw);
            UArchive a;
            try {
                a = ExportClientPackages.loadPathOrName(loader, root, raw);
            } catch (Throwable t) {
                failed++;
                System.err.printf(Locale.ROOT, "ERROR load %s: %s%n", raw, t);
                continue;
            }
            if (a == null) {
                missing++;
                System.err.printf(Locale.ROOT, "MISSING package: %s%n", raw);
                continue;
            }
            Path pkgDir = outDir.resolve(logical);
            Files.createDirectories(pkgDir);
            int wrote = 0;
            for (UObjectExport exp : a.getExportMap()) {
                if (exp.getClassName() == null || !"Sound".equalsIgnoreCase(exp.getClassName()))
                    continue;
                try {
                    var obj = a.getObjectLoader().exportObject(exp);
                    if (!(obj instanceof USound snd)) {
                        System.err.printf(Locale.ROOT, "ERROR %s.%s: not USound%n", logical, exp.getObjectName());
                        continue;
                    }
                    byte[] data = snd.getData();
                    if (data == null || data.length == 0) {
                        System.err.printf(Locale.ROOT, "ERROR %s.%s: empty wave payload%n", logical, exp.getObjectName());
                        continue;
                    }
                    String ext = extensionFor(data);
                    String fileName = exp.getObjectName() + "." + ext;
                    String group = groupName(a, exp);
                    Path out = group.isEmpty() ? pkgDir.resolve(fileName) : pkgDir.resolve(group).resolve(fileName);
                    Files.createDirectories(out.getParent());
                    Files.write(out, data);
                    wrote++;
                    waves++;
                } catch (Throwable t) {
                    System.err.printf(Locale.ROOT, "ERROR %s.%s: %s%n", logical, exp.getObjectName(), t);
                }
            }
            System.err.printf(Locale.ROOT, "Wrote %s waves=%d -> %s%n", logical, wrote, pkgDir);
            ok++;
        }
        System.err.printf(Locale.ROOT, "Done. packages ok=%d missing=%d failed=%d waves=%d%n", ok, missing, failed, waves);
        if (ok == 0) System.exit(1);
    }

    static String groupName(UArchive a, UObjectExport exp) {
        int index = exp.getPackageIndex();
        if (index > 0 && index <= a.getExportMap().size()) {
            String n = a.getExportMap().get(index - 1).getObjectName();
            if (n != null && !n.isEmpty() && !"None".equalsIgnoreCase(n))
                return n;
        }
        return "";
    }

    static String extensionFor(byte[] data) {
        if (data.length >= 4 && data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F')
            return "wav";
        if (data.length >= 4 && data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S')
            return "ogg";
        if (data.length >= 3 && (data[0] & 0xff) == 0xff && (data[1] & 0xe0) == 0xe0)
            return "mp3";
        return "bin";
    }
}
