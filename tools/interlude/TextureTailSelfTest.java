// Self-test for TextureUtils.findMip against synthetic UE2 UTexture exports.
// Runs in scripts/check-interlude-javac.sh: a layout the parser must accept,
// and corrupted/mismatched layouts it must reject — never a silent first fit.
import io.github.l2terrain.utils.TextureUtils;

import java.io.ByteArrayOutputStream;
import java.util.Arrays;

public class TextureTailSelfTest {
    static int failures;

    public static void main(String[] args) {
        roundTripCompactIndex();

        // A: INT32 count, zeroed 29-byte property prefix, one 2x2 G16 mip.
        byte[] a = build(29, (byte) 0, true, new int[][]{{2, 2}}, 10, 1, 1);
        TextureUtils.TextureMip mip = TextureUtils.findMip(a, 2, 2, 10, 1, 1, new StringBuilder());
        check(mip != null, "A: mip not found");
        if (mip != null) {
            check(mip.wideCount, "A: want int-count, got byte-count");
            check(mip.mipCount == 1, "A: mipCount " + mip.mipCount);
            check(mip.size == 8, "A: size " + mip.size);
            check(mip.arrayStart == 29, "A: arrayStart " + mip.arrayStart);
            check((a[mip.dataOffset] & 0xFF) == 0x10 && (a[mip.dataOffset + 7] & 0xFF) == 0x17,
                "A: data offset " + mip.dataOffset);
        }

        // B: BYTE count, same layout otherwise.
        byte[] b = build(29, (byte) 0, false, new int[][]{{2, 2}}, 10, 1, 1);
        mip = TextureUtils.findMip(b, 2, 2, 10, 1, 1, new StringBuilder());
        check(mip != null && !mip.wideCount, "B: byte-count layout not found");

        // C: two-mip chain, props describe mip0 (4x4).
        byte[] c = build(7, (byte) 0, true, new int[][]{{4, 4}, {2, 2}}, 10, 2, 2);
        mip = TextureUtils.findMip(c, 4, 4, 10, 2, 2, new StringBuilder());
        check(mip != null && mip.mipCount == 2 && mip.size == 32,
            "C: mip chain " + (mip == null ? "null" : mip.mipCount + "/" + mip.size));

        // D: tail Format corrupted — no candidate may validate.
        byte[] d = build(29, (byte) 0, true, new int[][]{{2, 2}}, 10, 1, 1);
        d[d.length - 19] = 9;
        check(TextureUtils.findMip(d, 2, 2, 10, 1, 1, new StringBuilder()) == null,
            "D: corrupted tail accepted");

        // E: export truncated by one byte — the walk must not consume it exactly.
        byte[] e = Arrays.copyOf(a, a.length - 1);
        check(TextureUtils.findMip(e, 2, 2, 10, 1, 1, new StringBuilder()) == null,
            "E: truncated export accepted");

        // F: properties disagree with the mip dims — reject, never first-fit.
        check(TextureUtils.findMip(a, 4, 4, 10, 2, 2, new StringBuilder()) == null,
            "F: dim mismatch accepted");

        // G: junk-filled prefix — starts inside the junk must not validate.
        byte[] g = build(53, (byte) 0xAB, true, new int[][]{{2, 2}}, 10, 1, 1);
        mip = TextureUtils.findMip(g, 2, 2, 10, 1, 1, new StringBuilder());
        check(mip != null && mip.arrayStart == 53,
            "G: junk prefix " + (mip == null ? "null" : "start " + mip.arrayStart));

        // H: 256x256 G16 — the compact size index is 3 bytes (40 80 10).
        byte[] h = build(29, (byte) 0, true, new int[][]{{256, 256}}, 10, 8, 8);
        mip = TextureUtils.findMip(h, 256, 256, 10, 8, 8, new StringBuilder());
        check(mip != null && mip.size == 131072 && mip.dataOffset == 29 + 4 + 4 + 3,
            "H: 256x256 " + (mip == null ? "null" : mip.size + "@" + mip.dataOffset));

        if (failures > 0) {
            System.err.println("TextureTailSelfTest: " + failures + " failure(s)");
            System.exit(1);
        }
        System.out.println("TextureTailSelfTest OK");
    }

    static void check(boolean ok, String label) {
        if (!ok) {
            failures++;
            System.err.println("  FAIL " + label);
        }
    }

    /**
     * Serialize a synthetic export: prefix, count, per mip [INT32 widthOffset,
     * compact size, data, INT32 USize, INT32 VSize, BYTE UBits, BYTE VBits],
     * then the 19-byte tail (Format, UBits, VBits, USize, VSize, UClamp, VClamp).
     * Props and tail describe mips[0]; UBits/VBits are log2 of mips[0] dims.
     */
    static byte[] build(int prefixLen, byte prefixFill, boolean wide, int[][] mips,
                        int format, int uBits, int vBits) {
        int w0 = mips[0][0];
        int h0 = mips[0][1];
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        for (int i = 0; i < prefixLen; i++) out.write(prefixFill);
        if (wide) writeInt32(out, mips.length);
        else out.write(mips.length);
        for (int m = 0; m < mips.length; m++) {
            int w = mips[m][0];
            int h = mips[m][1];
            int size = w * h * 2;
            writeInt32(out, 100000 + m); // widthOffset: not needed to read pixels
            out.writeBytes(compact(size));
            for (int i = 0; i < size; i++) out.write(0x10 + m * 8 + (i % 8));
            writeInt32(out, w);
            writeInt32(out, h);
            out.write(log2(w));
            out.write(log2(h));
        }
        out.write(format);
        out.write(uBits);
        out.write(vBits);
        writeInt32(out, w0);
        writeInt32(out, h0);
        writeInt32(out, w0); // UClamp
        writeInt32(out, h0); // VClamp
        return out.toByteArray();
    }

    /** UE2 compact index encoder: 6 value bits + 0x40 more on byte 0, then 7 + 0x80. */
    static byte[] compact(int value) {
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        int v = value;
        boolean first = true;
        while (true) {
            if (first) {
                int b = v & 0x3F;
                v >>>= 6;
                if (v != 0) b |= 0x40;
                out.write(b);
                first = false;
            } else {
                int b = v & 0x7F;
                v >>>= 7;
                if (v != 0) b |= 0x80;
                out.write(b);
            }
            if (v == 0) break;
        }
        return out.toByteArray();
    }

    static void roundTripCompactIndex() {
        int[] values = {0, 1, 63, 64, 127, 128, 8191, 8192, 65535, 131072, 524288};
        for (int v : values) {
            byte[] enc = compact(v);
            check(TextureUtils.readCompactIndex(enc, 0) == v,
                "compact index round-trip " + v + " -> " + TextureUtils.readCompactIndex(enc, 0));
            check(TextureUtils.compactIndexLength(v) == enc.length,
                "compact index length " + v + " -> " + enc.length);
        }
        // 131072 must be 40 80 10 — the tail of the old G16 marker 00 40 80 10.
        byte[] enc = compact(131072);
        check(enc.length == 3 && (enc[0] & 0xFF) == 0x40 && (enc[1] & 0xFF) == 0x80 && (enc[2] & 0xFF) == 0x10,
            "compact(131072) bytes " + Arrays.toString(enc));
    }

    static int log2(int v) {
        int n = 0;
        while ((1 << n) < v) n++;
        return n;
    }

    static void writeInt32(ByteArrayOutputStream out, int v) {
        out.write(v & 0xFF);
        out.write((v >>> 8) & 0xFF);
        out.write((v >>> 16) & 0xFF);
        out.write((v >>> 24) & 0xFF);
    }
}
