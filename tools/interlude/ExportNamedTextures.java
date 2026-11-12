// Export named UE2 Texture objects from an L2 .utx as PNG.
//   java -cp L2TerrainExtractor-all.jar ExportNamedTextures.java <pkg.utx> <outDir> name1,name2
import io.github.l2terrain.crypto.L2Decryptor;
import io.github.l2terrain.utils.TextureUtils;
import io.github.l2terrain.utils.UnrealPackageUtils;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.Export;
import net.shrimpworks.unreal.packages.entities.ExportedEntry;
import net.shrimpworks.unreal.packages.entities.ExportedObject;
import net.shrimpworks.unreal.packages.entities.objects.Texture;
import net.shrimpworks.unreal.packages.entities.objects.TextureBase;
import net.shrimpworks.unreal.packages.entities.properties.IntegerProperty;
import net.shrimpworks.unreal.packages.entities.properties.Property;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.HashSet;
import java.util.Locale;
import java.util.Set;

public class ExportNamedTextures {
    public static void main(String[] a) throws Exception {
        if (a.length < 3) {
            System.err.println("usage: ExportNamedTextures <pkg.utx> <outDir> name1,name2");
            System.exit(2);
        }
        Path utx = Path.of(a[0]);
        Path out = Path.of(a[1]);
        Files.createDirectories(out);
        Set<String> want = new HashSet<>();
        for (String s : a[2].split(",")) {
            if (!s.isBlank()) want.add(s.toLowerCase(Locale.ROOT));
        }
        Path tmp = Files.createTempFile("l2tex_", ".utx");
        int wrote = 0;
        try {
            L2Decryptor.decryptFile(utx, tmp);
            try (var pkg = new net.shrimpworks.unreal.packages.Package(new PackageReader(tmp))) {
                for (Export export : pkg.exports) {
                    String name = export.name.name;
                    if (!want.contains(name.toLowerCase(Locale.ROOT))) continue;
                    String cls = export.classIndex.get().name().name;
                    if (!"Texture".equals(cls)) {
                        System.out.println("SKIP class=" + cls + " " + name);
                        continue;
                    }
                    ExportedObject obj = export instanceof ExportedObject eo ? eo
                            : export instanceof ExportedEntry ee ? ee.asObject() : null;
                    if (obj == null) {
                        System.out.println("SKIP no-object " + name);
                        continue;
                    }
                    var texObj = pkg.object(obj);
                    if (!(texObj instanceof Texture tex)) {
                        System.out.println("SKIP not-texture " + name);
                        continue;
                    }
                    TextureBase.Format format = tex.format();
                    int width = 256, height = 256;
                    for (Property prop : tex.properties) {
                        if (prop instanceof IntegerProperty ip && prop.name.name.equals("USize")) width = ip.value;
                        else if (prop instanceof IntegerProperty ip && prop.name.name.equals("VSize")) height = ip.value;
                    }
                    byte[] exportData = UnrealPackageUtils.readExportData(tex, obj);
                    BufferedImage image = switch (format) {
                        case DXT1 -> TextureUtils.extractDXT1(exportData, width, height);
                        case DXT3 -> TextureUtils.extractDXT3(exportData, width, height);
                        case DXT5 -> TextureUtils.extractDXT5(exportData, width, height);
                        case RGBA8 -> TextureUtils.extractRGBA8(exportData, width, height);
                        case PALETTE_8_BIT -> TextureUtils.extractP8(exportData, width, height);
                        default -> null;
                    };
                    if (image == null) {
                        System.out.println("FAIL " + name + " format=" + format + " " + width + "x" + height);
                        continue;
                    }
                    Path png = out.resolve(name + ".png");
                    ImageIO.write(image, "PNG", png.toFile());
                    System.out.println("WROTE " + png.getFileName() + " " + format + " " + image.getWidth() + "x" + image.getHeight());
                    wrote++;
                }
            }
        } finally {
            Files.deleteIfExists(tmp);
        }
        System.out.println("done " + wrote + "/" + want.size() + " from " + utx.getFileName());
        if (wrote == 0) System.exit(1);
    }
}
