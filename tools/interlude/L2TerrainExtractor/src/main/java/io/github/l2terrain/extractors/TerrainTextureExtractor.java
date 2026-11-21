package io.github.l2terrain.extractors;

import io.github.l2terrain.crypto.L2Decryptor;
import io.github.l2terrain.utils.TextureUtils;
import io.github.l2terrain.utils.UnrealPackageUtils;
import net.shrimpworks.unreal.packages.Package;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.Export;
import net.shrimpworks.unreal.packages.entities.ExportedEntry;
import net.shrimpworks.unreal.packages.entities.ExportedObject;
import net.shrimpworks.unreal.packages.entities.objects.Texture;
import net.shrimpworks.unreal.packages.entities.objects.TextureBase;
import net.shrimpworks.unreal.packages.entities.properties.IntegerProperty;
import net.shrimpworks.unreal.packages.entities.properties.Property;

import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.*;
import java.util.stream.Collectors;

public class TerrainTextureExtractor {

    /** Ground diffuse referenced from tile metadata (splatmap → Texture= ref). */
    public record GroundTextureRef(String textureName, String packageHint) {
        public String key() {
            return textureName.toLowerCase(Locale.ROOT);
        }
    }

    public static class TextureInfo {
        public final String name;
        public final String sourcePackage;
        public final BufferedImage image;
        public final int width;
        public final int height;

        public TextureInfo(String name, String sourcePackage, BufferedImage image, int width, int height) {
            this.name = name;
            this.sourcePackage = sourcePackage;
            this.image = image;
            this.width = width;
            this.height = height;
        }
    }

  private static final class TexturePackageIndex {
    private final List<Path> allUtx = new ArrayList<>();
    private final Map<String, Path> byPackageStem = new TreeMap<>();

    void build(Path texturesDir) throws IOException {
      allUtx.clear();
      byPackageStem.clear();
      try (var stream = Files.list(texturesDir)) {
        for (Path p : stream.toList()) {
          String fileName = p.getFileName().toString();
          if (!fileName.toLowerCase(Locale.ROOT).endsWith(".utx")) {
            continue;
          }
          allUtx.add(p);
          String stem = fileName.substring(0, fileName.length() - 4);
          byPackageStem.put(stem.toLowerCase(Locale.ROOT), p);
        }
      }
      allUtx.sort(Comparator.comparing(p -> p.getFileName().toString().toLowerCase(Locale.ROOT)));
      System.out.printf(
          "Indexed %d texture package(s) under %s%n", allUtx.size(), texturesDir);
    }
  }

  /**
   * Resolve all requested ground textures: package hint first, then scan every *.utx for the name.
   */
  public Map<String, TextureInfo> resolveGroundTextures(Path texturesDir, Collection<GroundTextureRef> refs)
      throws IOException {
    if (refs == null || refs.isEmpty()) {
      return Collections.emptyMap();
    }
    TexturePackageIndex index = new TexturePackageIndex();
    index.build(texturesDir);

    Map<String, TextureInfo> results = new TreeMap<>();
    Map<String, GroundTextureRef> pending = new LinkedHashMap<>();
    for (GroundTextureRef ref : refs) {
      pending.putIfAbsent(ref.key(), ref);
    }

    // Pass 1: honor Unreal package ref from TerrainInfo (Package.TextureName).
    List<GroundTextureRef> hintOrdered = pending.values().stream()
        .filter(r -> r.packageHint() != null && !r.packageHint().isBlank())
        .toList();
    for (GroundTextureRef ref : hintOrdered) {
      Path pkg = index.byPackageStem.get(normalizePackageHint(ref.packageHint()));
      if (pkg == null) {
        continue;
      }
      TextureInfo info = extractNamedTexture(pkg, ref.textureName());
      if (info != null) {
        results.put(ref.key(), info);
        pending.remove(ref.key());
        System.out.printf(
            "  Resolved ground texture %s from %s (TerrainInfo package ref %s)%n",
            ref.textureName(), pkg.getFileName(), ref.packageHint());
      }
    }

    // Pass 2: name search across every client textures/*.utx.
    if (!pending.isEmpty()) {
      Set<String> want = pending.values().stream().map(r -> r.textureName().toLowerCase(Locale.ROOT))
          .collect(Collectors.toCollection(HashSet::new));
      for (Path pkg : index.allUtx) {
        if (want.isEmpty()) {
          break;
        }
        extractMatchingFromPackage(pkg, results, want, pending);
      }
    }

    return results;
  }

  private static String normalizePackageHint(String hint) {
    String h = hint.trim();
    if (h.toLowerCase(Locale.ROOT).endsWith(".utx")) {
      h = h.substring(0, h.length() - 4);
    }
    return h.toLowerCase(Locale.ROOT);
  }

  private void extractMatchingFromPackage(
      Path packagePath,
      Map<String, TextureInfo> results,
      Set<String> wantNamesLower,
      Map<String, GroundTextureRef> pending) throws IOException {
    String pkgFile = packagePath.getFileName().toString();
    Path tempFile = Files.createTempFile("l2tex_", ".utx");
    try {
      L2Decryptor.decryptFile(packagePath, tempFile);
      try (Package pkg = new Package(new PackageReader(tempFile))) {
        for (Export export : pkg.exports) {
          String className = export.classIndex.get().name().name;
          if (!className.equals("Texture")) {
            continue;
          }
          String texName = export.name.name;
          String texKey = texName.toLowerCase(Locale.ROOT);
          if (!wantNamesLower.contains(texKey) || results.containsKey(texKey)) {
            continue;
          }
          TextureInfo info = decodeExport(pkg, export, pkgFile, texName);
          if (info != null) {
            results.put(texKey, info);
            wantNamesLower.remove(texKey);
            GroundTextureRef ref = pending.remove(texKey);
            String refName = ref != null ? ref.textureName() : texName;
            System.out.printf(
                "  Resolved ground texture %s from %s (name index)%n", refName, pkgFile);
          }
        }
      }
    } finally {
      Files.deleteIfExists(tempFile);
    }
  }

  private TextureInfo extractNamedTexture(Path packagePath, String textureName) throws IOException {
    String want = textureName.toLowerCase(Locale.ROOT);
    String pkgFile = packagePath.getFileName().toString();
    Path tempFile = Files.createTempFile("l2tex_", ".utx");
    try {
      L2Decryptor.decryptFile(packagePath, tempFile);
      try (Package pkg = new Package(new PackageReader(tempFile))) {
        for (Export export : pkg.exports) {
          String className = export.classIndex.get().name().name;
          if (!className.equals("Texture")) {
            continue;
          }
          if (!export.name.name.equalsIgnoreCase(textureName)) {
            continue;
          }
          return decodeExport(pkg, export, pkgFile, export.name.name);
        }
      }
    } finally {
      Files.deleteIfExists(tempFile);
    }
    return null;
  }

  private TextureInfo decodeExport(Package pkg, Export export, String pkgFile, String texName) {
    try {
      ExportedObject obj = null;
      if (export instanceof ExportedObject eo) {
        obj = eo;
      } else if (export instanceof ExportedEntry ee) {
        obj = ee.asObject();
      }
      if (obj == null) {
        return null;
      }
      var texObj = pkg.object(obj);
      if (!(texObj instanceof Texture tex)) {
        return null;
      }
      TextureBase.Format format = tex.format();
      int width = 256, height = 256;
      for (Property prop : tex.properties) {
        if (prop.name.name.equals("USize") && prop instanceof IntegerProperty ip) {
          width = ip.value;
        } else if (prop.name.name.equals("VSize") && prop instanceof IntegerProperty ip) {
          height = ip.value;
        }
      }
      byte[] exportData = UnrealPackageUtils.readExportData(tex, obj);
      BufferedImage image = extractTextureByFormat(exportData, format, width, height);
      if (image == null) {
        return null;
      }
      return new TextureInfo(texName, pkgFile, image, width, height);
    } catch (Exception e) {
      return null;
    }
  }

  public Set<GroundTextureRef> collectGroundTextureRefsFromMetadata(Path extractedFolder) throws IOException {
    Set<GroundTextureRef> refs = new LinkedHashSet<>();
    try (var stream = Files.walk(extractedFolder)) {
      stream.filter(p -> p.getFileName().toString().endsWith("_metadata.txt")).forEach(metaFile -> {
        try {
          for (String line : Files.readAllLines(metaFile)) {
            if (!line.startsWith("splatmap_") || !line.contains(",")) {
              continue;
            }
            String groundRef = line.substring(line.indexOf(',') + 1).trim();
            GroundTextureRef parsed = parseGroundTextureRef(groundRef);
            if (parsed != null) {
              refs.add(parsed);
            }
          }
        } catch (IOException e) {
          System.err.println("Error reading " + metaFile + ": " + e.getMessage());
        }
      });
    }
    return refs;
  }

  static GroundTextureRef parseGroundTextureRef(String groundRef) {
    if (groundRef == null || groundRef.isBlank()) {
      return null;
    }
    int dot = groundRef.indexOf('.');
    if (dot > 0 && dot < groundRef.length() - 1) {
      return new GroundTextureRef(groundRef.substring(dot + 1), groundRef.substring(0, dot));
    }
    return new GroundTextureRef(groundRef, null);
  }

  /** Extract every Texture export from every *.utx under the client textures folder. */
  public Map<String, TextureInfo> extractAllFromClient(Path texturesDir) throws IOException {
    TexturePackageIndex index = new TexturePackageIndex();
    index.build(texturesDir);
    Map<String, TextureInfo> results = new TreeMap<>();
    for (Path pkg : index.allUtx) {
      extractAllTexturesFromPackage(pkg, results);
    }
    return results;
  }

  private void extractAllTexturesFromPackage(Path packagePath, Map<String, TextureInfo> results) throws IOException {
    String pkgFile = packagePath.getFileName().toString();
    Path tempFile = Files.createTempFile("l2tex_", ".utx");
    try {
      L2Decryptor.decryptFile(packagePath, tempFile);
      try (Package pkg = new Package(new PackageReader(tempFile))) {
        for (Export export : pkg.exports) {
          String className = export.classIndex.get().name().name;
          if (!className.equals("Texture")) {
            continue;
          }
          String texName = export.name.name;
          String texKey = texName.toLowerCase(Locale.ROOT);
          if (results.containsKey(texKey)) {
            continue;
          }
          TextureInfo info = decodeExport(pkg, export, pkgFile, texName);
          if (info != null) {
            results.put(texKey, info);
          }
        }
      }
    } finally {
      Files.deleteIfExists(tempFile);
    }
  }

  public Set<String> collectTextureNamesFromMetadata(Path extractedFolder) throws IOException {
    Set<String> names = new HashSet<>();
    for (GroundTextureRef ref : collectGroundTextureRefsFromMetadata(extractedFolder)) {
      names.add(ref.key());
    }
    return names;
  }

  private BufferedImage extractTextureByFormat(byte[] exportData, TextureBase.Format format, int width, int height) {
    return switch (format) {
      case DXT1 -> TextureUtils.extractDXT1(exportData, width, height);
      case DXT3 -> TextureUtils.extractDXT3(exportData, width, height);
      case DXT5 -> TextureUtils.extractDXT5(exportData, width, height);
      case RGBA8 -> TextureUtils.extractRGBA8(exportData, width, height);
      case PALETTE_8_BIT -> TextureUtils.extractP8(exportData, width, height);
      default -> null;
    };
  }
}