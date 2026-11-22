import com.rost.cruma.core.unreal.ClientSearchPaths;
import com.rost.cruma.core.unreal.UArchive;
import com.rost.cruma.core.unreal.UArchiveLoader;
import com.rost.cruma.core.unreal.UArchiveLoader.USearchConfig;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.Locale.Category;

/**
 * Lossless property dump of named Unreal packages (maps, sounds, animations, effects).
 * No binary payloads. Reuses ExportPackageProperties helpers.
 *
 * Usage: ExportClientPackages &lt;client-root&gt; &lt;out-dir&gt; [--pretty] &lt;package-name-or-path&gt;...
 */
public final class ExportClientPackages {
    public static void main(String[] args) throws Exception {
        if (args.length < 3) {
            System.err.println("Usage: ExportClientPackages <client-root> <out-dir> [--pretty] <package>...");
            System.exit(2);
        }
        Path root = Path.of(args[0]).toAbsolutePath();
        Path outDir = Path.of(args[1]).toAbsolutePath();
        Files.createDirectories(outDir);
        boolean compact = true;
        List<String> names = new ArrayList<>();
        for (int i = 2; i < args.length; i++) {
            if ("--pretty".equals(args[i])) compact = false;
            else if ("--compact".equals(args[i])) compact = true;
            else names.add(args[i]);
        }
        if (names.isEmpty()) {
            System.err.println("No packages given.");
            System.exit(2);
        }

        var entries = ClientSearchPaths.read(root);
        if (entries == null || entries.isEmpty()) entries = ClientSearchPaths.DEFAULT;
        List<USearchConfig> configs = new ArrayList<>();
        for (var e : entries) configs.add(new USearchConfig(e.directory(), e.extension()));
        var loader = new UArchiveLoader(root, configs);

        int ok = 0;
        int missing = 0;
        int failed = 0;
        for (String raw : names) {
            String logical = logicalName(raw);
            UArchive a = null;
            try {
                a = loadPathOrName(loader, root, raw);
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
            String extHint = extensionHint(raw, a);
            Path out = outDir.resolve(fileName(logical, extHint));
            try {
                ExportPackageProperties.dumpArchive(out, logical, raw, a, compact);
                ok++;
            } catch (Throwable t) {
                failed++;
                System.err.printf(Locale.ROOT, "ERROR dump %s: %s%n", raw, t);
                t.printStackTrace(System.err);
            }
        }
        System.err.printf(Locale.ROOT, "Done. ok=%d missing=%d failed=%d of %d%n", ok, missing, failed, names.size());
        if (ok == 0) System.exit(1);
    }

    static UArchive loadPathOrName(UArchiveLoader loader, Path root, String raw) {
        Path p = Path.of(raw);
        if (p.isAbsolute() || raw.contains("/") || raw.contains("\\") || raw.contains(".")) {
            Path abs = p.isAbsolute() ? p : root.resolve(raw);
            if (Files.isRegularFile(abs)) {
                // Prefer the search-path name so imports resolve; also try stem.
                String stem = stripExt(abs.getFileName().toString());
                UArchive a = ExportPackageProperties.load(loader, stem);
                if (a != null) return a;
            }
        }
        return ExportPackageProperties.load(loader, logicalName(raw));
    }

    static String logicalName(String raw) {
        String s = raw.replace('\\', '/');
        int slash = s.lastIndexOf('/');
        if (slash >= 0) s = s.substring(slash + 1);
        return stripExt(s);
    }

    static String stripExt(String s) {
        int dot = s.lastIndexOf('.');
        if (dot > 0) {
            String ext = s.substring(dot + 1).toLowerCase(Locale.ROOT);
            if (ext.equals("unr") || ext.equals("uax") || ext.equals("ukx")
                || ext.equals("u") || ext.equals("usx") || ext.equals("utx")
                || ext.equals("usk") || ext.equals("umx") || ext.equals("uix")) {
                return s.substring(0, dot);
            }
        }
        return s;
    }

    static String extensionHint(String raw, UArchive a) {
        String s = raw.replace('\\', '/');
        int dot = s.lastIndexOf('.');
        if (dot > 0) {
            String ext = s.substring(dot + 1).toLowerCase(Locale.ROOT);
            if (ext.length() <= 4) return ext;
        }
        String n = a.getName() == null ? "" : a.getName();
        return "";
    }

    static String fileName(String logical, String ext) {
        if (ext == null || ext.isEmpty()) return logical + ".json";
        return logical + "." + ext + ".json";
    }
}
