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

        // I: the live Lobby_terrain_t.Height layout (pkg.version 123) — the 59
        // head bytes below are byte-identical to the Mac dump: a None tag, the
        // 55-byte native prefix, then the size index 40 80 10 at +56. No array
        // count, no per-mip offset, dims after the data, EOF after VBits.
        byte[] i = buildChain(LOBBY_HEIGHT_HEAD, new int[][]{{256, 256}}, 10, 8, 8);
        check(i.length == 131141, "I: fixture bytes " + i.length);
        mip = TextureUtils.findMip(i, 256, 256, 10, 8, 8, new StringBuilder());
        check(mip != null && "chain".equals(mip.layout) && mip.dataOffset == 59
                && mip.size == 131072 && mip.mipCount == 1,
            "I: lobby chain " + (mip == null ? "null" : mip.layout + " " + mip.size + "@" + mip.dataOffset));

        // J: chain with three mips down to 1x1, junk prefix, EOF end.
        byte[] j = buildChain(new byte[]{(byte) 0xFF, (byte) 0xFF, (byte) 0xFF, (byte) 0xFF,
                (byte) 0xFF, (byte) 0xFF, (byte) 0xFF}, new int[][]{{4, 4}, {2, 2}, {1, 1}}, 10, 2, 2);
        mip = TextureUtils.findMip(j, 4, 4, 10, 2, 2, new StringBuilder());
        check(mip != null && "chain".equals(mip.layout) && mip.mipCount == 3 && mip.size == 32,
            "J: 3-mip chain " + (mip == null ? "null" : mip.mipCount + "/" + mip.size));

        // K: the lobby bytes with the wrong format must not validate (P8 size
        // law wants 65536, the chain says 131072).
        check(TextureUtils.findMip(i, 256, 256, 0, 8, 8, new StringBuilder()) == null,
            "K: P8 claim on a G16 chain accepted");

        // L: one trailing byte after VBits — no family may consume it exactly.
        byte[] l = Arrays.copyOf(i, i.length + 1);
        check(TextureUtils.findMip(l, 256, 256, 10, 8, 8, new StringBuilder()) == null,
            "L: trailing byte accepted");

        // M: chain dims disagree with the properties — reject, never first-fit.
        check(TextureUtils.findMip(i, 128, 128, 10, 7, 7, new StringBuilder()) == null,
            "M: dim mismatch on chain accepted");

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

    /** Head of the live Lobby_terrain_t.Height export (Mac dump 2026-11-23):
     * None tag + 55-byte native prefix + the 131072 size index 40 80 10. */
    static final byte[] LOBBY_HEIGHT_HEAD = {
        0x00, 0x22, (byte) 0xf3, (byte) 0xfa, 0x23, 0x77, 0x00, (byte) 0xa2,
        0x01, 0x42, 0x03, 0x70, 0x41, 0x0c, 0x01, 0x0a,
        0x06, 0x01, 0x08, 0x05, 0x01, 0x08, 0x07, 0x22,
        0x00, 0x01, 0x00, 0x00, 0x03, 0x22, 0x00, 0x01,
        0x00, 0x00, 0x08, 0x22, 0x00, 0x01, 0x00, 0x00,
        0x04, 0x22, 0x00, 0x01, 0x00, 0x00, 0x01, 0x00,
        0x00, 0x00, 0x00, 0x01, 0x44, (byte) 0xb0, 0x46, 0x00,
        0x40, (byte) 0x80, 0x10,
    };

    /**
     * Serialize a pkg-123 bare chain export: the head bytes (which already end
     * at mip0's size index for the single-mip case are NOT assumed — the head
     * is followed by the full chain), then per mip [compact size][data]
     * [INT32 USize][INT32 VSize][BYTE UBits][BYTE VBits], EOF. When the head
     * already ends with mip0's size index (the lobby bytes), that index is
     * used; otherwise one is appended per mip. Pixel bytes repeat bd 4f —
     * plausible G16 heights that form no valid candidate inside the data.
     */
    static byte[] buildChain(byte[] head, int[][] mips, int format, int uBits, int vBits) {
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        out.writeBytes(head);
        for (int m = 0; m < mips.length; m++) {
            int w = mips[m][0];
            int h = mips[m][1];
            int size = w * h * 2;
            if (!(m == 0 && endsWithSizeIndex(head, size))) out.writeBytes(compact(size));
            for (int k = 0; k < size; k++) out.write((k & 1) == 0 ? 0xbd : 0x4f);
            writeInt32(out, w);
            writeInt32(out, h);
            out.write(log2(w));
            out.write(log2(h));
        }
        return out.toByteArray();
    }

    /** True when the head's trailing bytes are exactly compact(size). */
    static boolean endsWithSizeIndex(byte[] head, int size) {
        byte[] want = compact(size);
        if (head.length < want.length) return false;
        for (int k = 0; k < want.length; k++) {
            if (head[head.length - want.length + k] != want[k]) return false;
        }
        return true;
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
