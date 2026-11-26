// Export every Texture from an already-decrypted UE2 package as PNG.
//   javac -cp <l2terrain-all.jar> ExportAllTextures.java
//   java  -cp <l2terrain-all.jar>:. ExportAllTextures <pkg.utx> <outDir>
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.Export;
import net.shrimpworks.unreal.packages.entities.ExportedObject;
import net.shrimpworks.unreal.packages.entities.objects.Texture;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.nio.file.Files;
import java.nio.file.Path;

public class ExportAllTextures {
    public static void main(String[] a) throws Exception {
        if (a.length < 2) {
            System.err.println("usage: ExportAllTextures <decrypted.utx> <outDir>");
            System.exit(2);
        }
        Path utx = Path.of(a[0]);
        Path out = Path.of(a[1]);
        Files.createDirectories(out);
        int wrote = 0, fail = 0;
        try (var pkg = new net.shrimpworks.unreal.packages.Package(new PackageReader(utx))) {
            for (Export exp : pkg.exports) {
                if (exp == null) continue;
                Object obj;
                try {
                    obj = (exp instanceof ExportedObject eo) ? eo.object() : null;
                } catch (Exception e) {
                    fail++;
                    continue;
                }
                if (!(obj instanceof Texture tex)) continue;
                try {
                    Texture.MipMap[] mips = tex.mipMaps();
                    BufferedImage img = mips.length == 0 ? null : mips[0].get();
                    // fall back: some APIs expose image() / getImage()
                    if (img == null) {
                        fail++;
                        continue;
                    }
                    String name = exp.name.name;
                    Path dest = out.resolve(name + ".png");
                    ImageIO.write(img, "png", dest.toFile());
                    wrote++;
                } catch (Exception e) {
                    fail++;
                }
            }
        }
        System.out.println("wrote=" + wrote + " fail=" + fail + " " + utx.getFileName());
    }
}
