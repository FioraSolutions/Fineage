// Read the height the level's TerrainInfo actually references.
//   java -cp L2TerrainExtractor-all.jar ExportLevelHeight.java <clientRoot> <map.unr> <outDir>
//
// TerrainMap is an object reference. A positive index is a texture export in the
// map. A negative index is an import: the .utx whose file name is that package,
// and the texture whose name (and group) the reference names. The first G16 in
// the package is not used. The TerrainInfo export bytes are written beside the
// G16 so the vertex grid in that export can replace a stale texture.
import io.github.l2terrain.crypto.L2Decryptor;
import io.github.l2terrain.utils.UnrealPackageUtils;
import net.shrimpworks.unreal.packages.Package;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.Export;
import net.shrimpworks.unreal.packages.entities.ExportedEntry;
import net.shrimpworks.unreal.packages.entities.ExportedObject;
import net.shrimpworks.unreal.packages.entities.Import;
import net.shrimpworks.unreal.packages.entities.Name;
import net.shrimpworks.unreal.packages.entities.Named;
import net.shrimpworks.unreal.packages.entities.ObjectReference;
import net.shrimpworks.unreal.packages.entities.objects.Texture;
import net.shrimpworks.unreal.packages.entities.objects.TextureBase;
import net.shrimpworks.unreal.packages.entities.properties.ObjectProperty;
import net.shrimpworks.unreal.packages.entities.properties.Property;
import net.shrimpworks.unreal.packages.entities.properties.StructProperty;

import java.awt.image.BufferedImage;
import java.awt.image.DataBufferUShort;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.security.MessageDigest;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class ExportLevelHeight {
    public static void main(String[] args) throws Exception {
        if (args.length < 3) {
            System.err.println("usage: ExportLevelHeight <clientRoot> <map.unr> <outDir>");
            System.exit(2);
        }
        Path client = Path.of(args[0]);
        Path map = Path.of(args[1]);
        Path out = Path.of(args[2]);
        Files.createDirectories(out);
        String mapFile = map.getFileName().toString();
        String mapStem = mapFile.toLowerCase(Locale.ROOT).endsWith(".unr")
            ? mapFile.substring(0, mapFile.length() - 4) : mapFile;
        Path mapTmp = Files.createTempFile("l2map_", ".unr");
        try {
            L2Decryptor.decryptFile(map, mapTmp);
            try (Package pkg = new Package(new PackageReader(mapTmp))) {
                Export terrain = null;
                for (Export exp : pkg.exports) {
                    if ("TerrainInfo".equals(exp.classIndex.get().name().name)) {
                        if (terrain != null) {
                            fail("more than one TerrainInfo in " + map.getFileName());
                        }
                        terrain = exp;
                    }
                }
                if (terrain == null) fail("no TerrainInfo in " + map.getFileName());
                ExportedObject terrainObj = asObject(terrain);
                var loaded = pkg.object(terrainObj);
                ObjectProperty terrainMap = null;
                StructProperty.VectorProperty scale = null;
                StructProperty.VectorProperty location = null;
                for (Property prop : loaded.properties) {
                    if (prop instanceof ObjectProperty op && "TerrainMap".equals(prop.name.name)) terrainMap = op;
                    if (prop instanceof StructProperty.VectorProperty vp && "TerrainScale".equals(prop.name.name)) scale = vp;
                    if (prop instanceof StructProperty.VectorProperty vp && "Location".equals(prop.name.name)) location = vp;
                }
                if (terrainMap == null) fail("TerrainInfo has no TerrainMap");
                if (scale == null || location == null) fail("TerrainInfo is missing TerrainScale or Location");
                int index = terrainMap.value.index;
                String qualified = fullName(pkg, index, mapStem);
                String[] parts = qualified.split("\\.");
                if (parts.length < 2) fail("TerrainMap path " + qualified + " has no package.name");
                String packageName = parts[0];
                String objectName = parts[parts.length - 1];
                String group = parts.length == 2 ? "" : String.join(".", java.util.Arrays.copyOfRange(parts, 1, parts.length - 1));
                String kind = index > 0 ? "export" : "import";
                System.out.println("[TerrainMap] " + qualified + " index " + index + " (" + kind
                    + ") group=" + (group.isEmpty() ? "(none)" : group));

                Package texPkg = pkg;
                Path utx = map;
                Path texTmp = null;
                int exportIndex = index > 0 ? index - 1 : -1;
                try {
                    if (index < 0) {
                        utx = findUtx(client, packageName);
                        String stem = utx.getFileName().toString();
                        stem = stem.substring(0, stem.length() - 4);
                        if (!stem.equalsIgnoreCase(packageName)) {
                            fail("TerrainMap " + qualified + " is package " + packageName
                                + ", resolved to " + utx);
                        }
                        texTmp = Files.createTempFile("l2tex_", ".utx");
                        L2Decryptor.decryptFile(utx, texTmp);
                        texPkg = new Package(new PackageReader(texTmp));
                    } else if (!packageName.equalsIgnoreCase(mapStem)) {
                        fail("TerrainMap " + qualified + " index " + index
                            + " is a level export, but its package is " + packageName
                            + " rather than " + mapStem);
                    }
                    Export tex = findTexture(texPkg, objectName, group, packageName);
                    exportIndex = tex.index;
                    ExportedObject texObj = asObject(tex);
                    var loadedTex = texPkg.object(texObj);
                    if (!(loadedTex instanceof Texture texture)) {
                        fail("TerrainMap " + qualified + " export " + exportIndex + " is not a Texture");
                    }
                    if (texture.format() != TextureBase.Format.G16) {
                        fail("TerrainMap " + qualified + " export " + exportIndex
                            + " format " + texture.format() + " is not G16");
                    }
                    Texture.MipMap[] mips = texture.mipMaps();
                    if (mips.length == 0) fail("TerrainMap " + qualified + " has no mip");
                    BufferedImage image = mips[0].get();
                    if (image.getType() != BufferedImage.TYPE_USHORT_GRAY) {
                        fail("TerrainMap " + qualified + " mip is not 16-bit gray");
                    }
                    short[] pixels = ((DataBufferUShort) image.getRaster().getDataBuffer()).getData();
                    int width = image.getWidth();
                    int height = image.getHeight();
                    if (pixels.length != width * height) {
                        fail("TerrainMap " + qualified + " mip " + width + "x" + height
                            + " has " + pixels.length + " samples");
                    }
                    byte[] raw = new byte[pixels.length * 2];
                    ByteBuffer.wrap(raw).order(ByteOrder.LITTLE_ENDIAN).asShortBuffer().put(pixels);
                    Path g16 = out.resolve("g16.raw");
                    Files.write(g16, raw);
                    byte[] info = UnrealPackageUtils.readExportData(pkg, terrainObj);
                    Files.write(out.resolve("terrain-info.bin"), info);
                    String sha = sha256(raw);
                    String json = ownerJson(map.getFileName().toString(), qualified, packageName, group,
                        objectName, kind, index, utx.toString(), exportIndex, width, height,
                        location, scale, sha);
                    Files.writeString(out.resolve("owner.json"), json, StandardCharsets.UTF_8);
                    System.out.println("[TerrainMap] wrote " + g16.getFileName() + " " + width + "x" + height
                        + " export " + exportIndex + " sha256 " + sha
                        + " terrain-info " + info.length + " bytes from " + utx.getFileName());
                } finally {
                    if (texPkg != pkg) texPkg.close();
                    if (texTmp != null) Files.deleteIfExists(texTmp);
                }
            }
        } finally {
            Files.deleteIfExists(mapTmp);
        }
    }

    static ExportedObject asObject(Export exp) {
        if (exp instanceof ExportedObject eo) return eo;
        if (exp instanceof ExportedEntry ee) return ee.asObject();
        fail("export " + exp.name.name + " has no object");
        return null;
    }

    static Export findTexture(Package pkg, String name, String group, String packageName) {
        List<String> hits = new ArrayList<>();
        Export found = null;
        for (Export exp : pkg.exports) {
            if (!"Texture".equals(exp.classIndex.get().name().name)) continue;
            if (!exp.name.name.equalsIgnoreCase(name)) continue;
            String gotGroup = groupOf(exp);
            String label = (gotGroup.isEmpty() ? "" : gotGroup + ".") + exp.name.name
                + " export " + exp.index;
            hits.add(label);
            if (!gotGroup.equalsIgnoreCase(group)) continue;
            if (found != null) {
                fail("TerrainMap texture " + name + " group '" + group + "' matches more than one export: " + hits);
            }
            found = exp;
        }
        if (found == null) {
            fail("TerrainMap texture " + (group.isEmpty() ? name : group + "." + name)
                + " is not in " + packageName + (hits.isEmpty() ? "" : " (same name in other groups: " + hits + ")"));
        }
        if (hits.size() > 1) {
            System.out.println("[TerrainMap] chose " + (group.isEmpty() ? "(none)" : group) + "." + name
                + " export " + found.index + " over " + hits);
        }
        return found;
    }

    static String groupOf(Export exp) {
        if (exp.groupIndex == null || exp.groupIndex.index == 0) return "";
        Name group = exp.groupName();
        if (group == null || group.name == null || group.name.isEmpty() || group.name.equalsIgnoreCase("None")) return "";
        return group.name;
    }

    static String fullName(Package pkg, int index, String mapPackage) {
        Named named = new ObjectReference(pkg, index).get();
        List<String> parts = new ArrayList<>();
        parts.add(named.name().name);
        if (named instanceof Import imp) {
            ObjectReference outer = imp.packageIndex;
            int guard = 0;
            while (outer != null && outer.index != 0 && guard++ < 8) {
                Named parent = outer.get();
                parts.add(0, parent.name().name);
                outer = parent instanceof Import pi ? pi.packageIndex : null;
            }
        } else {
            parts.add(0, mapPackage);
        }
        return String.join(".", parts);
    }

    static Path findUtx(Path client, String packageName) throws Exception {
        List<Path> roots = List.of(client.resolve("textures"), client.resolve("systextures"), client.resolve("Textures"), client.resolve("SysTextures"));
        List<Path> hits = new ArrayList<>();
        for (Path root : roots) {
            if (!Files.isDirectory(root)) continue;
            try (var stream = Files.list(root)) {
                for (Path file : stream.toList()) {
                    String fn = file.getFileName().toString();
                    if (fn.equalsIgnoreCase(packageName + ".utx")) hits.add(file);
                }
            }
        }
        if (hits.isEmpty()) {
            fail("TerrainMap package " + packageName + " has no " + packageName + ".utx under " + client);
        }
        if (hits.size() > 1) {
            fail("TerrainMap package " + packageName + " matches more than one utx: " + hits);
        }
        return hits.get(0);
    }

    static String sha256(byte[] raw) throws Exception {
        byte[] dig = MessageDigest.getInstance("SHA-256").digest(raw);
        StringBuilder sb = new StringBuilder();
        for (byte b : dig) sb.append(String.format("%02x", b));
        return sb.toString();
    }

    static String ownerJson(String map, String qualified, String packageName, String group, String name,
                            String kind, int index, String utx, int exportIndex, int width, int height,
                            StructProperty.VectorProperty location, StructProperty.VectorProperty scale,
                            String sha) {
        return "{\n"
            + "  \"map\": " + json(map) + ",\n"
            + "  \"reference\": " + json(qualified) + ",\n"
            + "  \"package\": " + json(packageName) + ",\n"
            + "  \"group\": " + json(group) + ",\n"
            + "  \"name\": " + json(name) + ",\n"
            + "  \"kind\": " + json(kind) + ",\n"
            + "  \"index\": " + index + ",\n"
            + "  \"utx\": " + json(utx) + ",\n"
            + "  \"utx_export_index\": " + exportIndex + ",\n"
            + "  \"width\": " + width + ",\n"
            + "  \"height\": " + height + ",\n"
            + "  \"location\": [" + f(location.x) + ", " + f(location.y) + ", " + f(location.z) + "],\n"
            + "  \"scale\": [" + f(scale.x) + ", " + f(scale.y) + ", " + f(scale.z) + "],\n"
            + "  \"g16\": \"g16.raw\",\n"
            + "  \"terrain_info\": \"terrain-info.bin\",\n"
            + "  \"g16_sha256\": " + json(sha) + "\n"
            + "}\n";
    }

    static String f(float v) {
        return String.format(Locale.ROOT, "%.6f", v);
    }

    static String json(String s) {
        return "\"" + s.replace("\\", "\\\\").replace("\"", "\\\"") + "\"";
    }

    static void fail(String message) {
        System.err.println("ERROR " + message);
        System.exit(1);
    }
}
