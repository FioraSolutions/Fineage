// List every export in an L2 package as name, class, and immediate group.
//   java -cp L2TerrainExtractor-all.jar DescribeExports.java <pkg.utx>
// Group is empty when the export has no group (group index 0 or name None).
import io.github.l2terrain.crypto.L2Decryptor;
import net.shrimpworks.unreal.packages.PackageReader;
import net.shrimpworks.unreal.packages.entities.Export;
import net.shrimpworks.unreal.packages.entities.Name;

import java.nio.file.Files;
import java.nio.file.Path;

public class DescribeExports {
    public static void main(String[] a) throws Exception {
        if (a.length < 1) {
            System.err.println("usage: DescribeExports <pkg.utx>");
            System.exit(2);
        }
        Path utx = Path.of(a[0]);
        Path tmp = Files.createTempFile("l2desc_", ".utx");
        try {
            L2Decryptor.decryptFile(utx, tmp);
            try (var pkg = new net.shrimpworks.unreal.packages.Package(new PackageReader(tmp))) {
                for (Export export : pkg.exports) {
                    String cls = export.classIndex.get().name().name;
                    String group = "";
                    if (export.groupIndex != null && export.groupIndex.index != 0) {
                        Name g = export.groupName();
                        if (g != null && g.name != null && !g.name.isEmpty() && !g.name.equals("None")) {
                            group = g.name;
                        }
                    }
                    System.out.println(export.name.name + "\t" + cls + "\t" + group);
                }
            }
        } finally {
            Files.deleteIfExists(tmp);
        }
    }
}
