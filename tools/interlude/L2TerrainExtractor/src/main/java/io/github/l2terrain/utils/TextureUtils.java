package io.github.l2terrain.utils;

import java.awt.image.BufferedImage;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

/**
 * Shared utility methods for texture extraction and decompression.
 * 
 * <p>Provides DXT1/DXT3/DXT5 decompression, compact index parsing,
 * and other common texture operations used by multiple extractors.</p>
 */
public final class TextureUtils {
    
    private TextureUtils() {
        // Utility class - prevent instantiation
    }
    
    // ==================== Compact Index ====================
    
    /**
     * Read a compact index from a byte array.
     * Compact index is Unreal's variable-length integer encoding.
     * 
     * @param data the byte array
     * @param offset the offset to start reading from
     * @return the decoded integer value
     */
    public static int readCompactIndex(byte[] data, int offset) {
        if (offset >= data.length) return 0;
        
        int result = 0;
        int shift = 0;
        boolean negative = false;
        
        for (int i = 0; i < 5 && offset + i < data.length; i++) {
            int b = data[offset + i] & 0xFF;
            
            if (i == 0) {
                negative = (b & 0x80) != 0;
                result = b & 0x3F;
                if ((b & 0x40) == 0) {
                    return negative ? -result : result;
                }
                shift = 6;
            } else {
                result |= (b & 0x7F) << shift;
                if ((b & 0x80) == 0) {
                    return negative ? -result : result;
                }
                shift += 7;
            }
        }
        
        return negative ? -result : result;
    }
    
    /**
     * Get the byte length of a compact index encoding.
     * 
     * @param value the integer value
     * @return the number of bytes needed to encode this value
     */
    public static int compactIndexLength(int value) {
        if (value < 0) value = -value;
        if (value < 0x40) return 1;
        if (value < 0x2000) return 2;
        if (value < 0x100000) return 3;
        if (value < 0x8000000) return 4;
        return 5;
    }
    
    // ==================== Texture Data Location ====================
    
    /**
     * Find the texture data offset by searching for the size indicator.
     * 
     * @param data the raw export data
     * @param expectedSize the expected texture data size in bytes
     * @return the offset where texture data starts, or -1 if not found
     */
    public static int findTextureData(byte[] data, int expectedSize) {
        // Try to find compact index that matches expected size
        for (int i = 0; i < data.length - expectedSize - 10; i++) {
            int size = readCompactIndex(data, i);
            if (size == expectedSize && i + 5 + expectedSize <= data.length) {
                int indexLen = compactIndexLength(size);
                return i + indexLen;
            }
        }
        
        // Fallback: look for the data near the end
        int searchStart = Math.max(0, data.length - expectedSize - 100);
        for (int i = searchStart; i < data.length - expectedSize; i++) {
            int size = readCompactIndex(data, i);
            if (size == expectedSize) {
                int indexLen = compactIndexLength(size);
                return i + indexLen;
            }
        }
        
        return -1;
    }
    
    // ==================== DXT Decompression ====================
    
    /**
     * Decompress DXT1 texture data to an ARGB image.
     * 
     * @param dxtData the compressed DXT1 data
     * @param width texture width (must be multiple of 4)
     * @param height texture height (must be multiple of 4)
     * @return the decompressed image
     */
    public static BufferedImage decompressDXT1(byte[] dxtData, int width, int height) {
        BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
        ByteBuffer buffer = ByteBuffer.wrap(dxtData).order(ByteOrder.LITTLE_ENDIAN);
        
        int blockWidth = width / 4;
        int blockHeight = height / 4;
        
        for (int by = 0; by < blockHeight; by++) {
            for (int bx = 0; bx < blockWidth; bx++) {
                int c0 = buffer.getShort() & 0xFFFF;
                int c1 = buffer.getShort() & 0xFFFF;
                int indices = buffer.getInt();
                
                int[] colors = new int[4];
                colors[0] = rgb565ToArgb(c0);
                colors[1] = rgb565ToArgb(c1);
                
                if (c0 > c1) {
                    colors[2] = interpolateColor(colors[0], colors[1], 2, 1);
                    colors[3] = interpolateColor(colors[0], colors[1], 1, 2);
                } else {
                    colors[2] = interpolateColor(colors[0], colors[1], 1, 1);
                    colors[3] = 0x00000000; // transparent
                }
                
                for (int py = 0; py < 4; py++) {
                    for (int px = 0; px < 4; px++) {
                        int idx = (indices >> ((py * 4 + px) * 2)) & 0x3;
                        int x = bx * 4 + px;
                        int y = by * 4 + py;
                        if (x < width && y < height) {
                            image.setRGB(x, y, colors[idx]);
                        }
                    }
                }
            }
        }
        
        return image;
    }
    
    /**
     * Decompress DXT3 texture data to an ARGB image.
     * 
     * @param dxtData the compressed DXT3 data
     * @param width texture width (must be multiple of 4)
     * @param height texture height (must be multiple of 4)
     * @return the decompressed image
     */
    public static BufferedImage decompressDXT3(byte[] dxtData, int width, int height) {
        BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
        ByteBuffer buffer = ByteBuffer.wrap(dxtData).order(ByteOrder.LITTLE_ENDIAN);
        
        int blockWidth = width / 4;
        int blockHeight = height / 4;
        
        for (int by = 0; by < blockHeight; by++) {
            for (int bx = 0; bx < blockWidth; bx++) {
                // DXT3: 8 bytes explicit alpha, then 8 bytes DXT1 color block
                long alphaBlock = buffer.getLong();
                
                int c0 = buffer.getShort() & 0xFFFF;
                int c1 = buffer.getShort() & 0xFFFF;
                int indices = buffer.getInt();
                
                int[] colors = new int[4];
                colors[0] = rgb565ToRgb(c0);
                colors[1] = rgb565ToRgb(c1);
                colors[2] = interpolateColorNoAlpha(colors[0], colors[1], 2, 1);
                colors[3] = interpolateColorNoAlpha(colors[0], colors[1], 1, 2);
                
                for (int py = 0; py < 4; py++) {
                    for (int px = 0; px < 4; px++) {
                        int idx = (indices >> ((py * 4 + px) * 2)) & 0x3;
                        int alphaIdx = py * 4 + px;
                        int alpha = (int) ((alphaBlock >> (alphaIdx * 4)) & 0xF) * 17; // Scale 0-15 to 0-255
                        
                        int x = bx * 4 + px;
                        int y = by * 4 + py;
                        if (x < width && y < height) {
                            int argb = (alpha << 24) | colors[idx];
                            image.setRGB(x, y, argb);
                        }
                    }
                }
            }
        }
        
        return image;
    }
    
    /**
     * Decompress DXT5 texture data to an ARGB image.
     * 
     * @param dxtData the compressed DXT5 data
     * @param width texture width (must be multiple of 4)
     * @param height texture height (must be multiple of 4)
     * @return the decompressed image
     */
    public static BufferedImage decompressDXT5(byte[] dxtData, int width, int height) {
        BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
        ByteBuffer buffer = ByteBuffer.wrap(dxtData).order(ByteOrder.LITTLE_ENDIAN);
        
        int blockWidth = width / 4;
        int blockHeight = height / 4;
        
        for (int by = 0; by < blockHeight; by++) {
            for (int bx = 0; bx < blockWidth; bx++) {
                // DXT5: 2 bytes alpha endpoints + 6 bytes alpha indices + 8 bytes color
                int alpha0 = buffer.get() & 0xFF;
                int alpha1 = buffer.get() & 0xFF;
                
                // Read 6 bytes of alpha indices (48 bits for 16 pixels, 3 bits each)
                long alphaIndices = 0;
                for (int i = 0; i < 6; i++) {
                    alphaIndices |= ((long)(buffer.get() & 0xFF)) << (i * 8);
                }
                
                // Build alpha lookup table
                int[] alphas = new int[8];
                alphas[0] = alpha0;
                alphas[1] = alpha1;
                if (alpha0 > alpha1) {
                    for (int i = 1; i <= 6; i++) {
                        alphas[i + 1] = ((7 - i) * alpha0 + i * alpha1) / 7;
                    }
                } else {
                    for (int i = 1; i <= 4; i++) {
                        alphas[i + 1] = ((5 - i) * alpha0 + i * alpha1) / 5;
                    }
                    alphas[6] = 0;
                    alphas[7] = 255;
                }
                
                int c0 = buffer.getShort() & 0xFFFF;
                int c1 = buffer.getShort() & 0xFFFF;
                int indices = buffer.getInt();
                
                int[] colors = new int[4];
                colors[0] = rgb565ToRgb(c0);
                colors[1] = rgb565ToRgb(c1);
                colors[2] = interpolateColorNoAlpha(colors[0], colors[1], 2, 1);
                colors[3] = interpolateColorNoAlpha(colors[0], colors[1], 1, 2);
                
                for (int py = 0; py < 4; py++) {
                    for (int px = 0; px < 4; px++) {
                        int idx = (indices >> ((py * 4 + px) * 2)) & 0x3;
                        int alphaIdx = (int) ((alphaIndices >> ((py * 4 + px) * 3)) & 0x7);
                        
                        int x = bx * 4 + px;
                        int y = by * 4 + py;
                        if (x < width && y < height) {
                            image.setRGB(x, y, (alphas[alphaIdx] << 24) | colors[idx]);
                        }
                    }
                }
            }
        }
        
        return image;
    }
    
    // ==================== Other Format Extraction ====================
    
    /**
     * Extract RGBA8 texture data to an image.
     * 
     * @param exportData the raw export data
     * @param width texture width
     * @param height texture height
     * @return the extracted image, or null if data not found
     */
    public static BufferedImage extractRGBA8(byte[] exportData, int width, int height) {
        int rgba8Size = width * height * 4;
        int dataOffset = findTextureData(exportData, rgba8Size);
        if (dataOffset < 0) return null;
        
        BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
        
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int idx = dataOffset + (y * width + x) * 4;
                int b = exportData[idx] & 0xFF;
                int g = exportData[idx + 1] & 0xFF;
                int r = exportData[idx + 2] & 0xFF;
                int a = exportData[idx + 3] & 0xFF;
                image.setRGB(x, y, (a << 24) | (r << 16) | (g << 8) | b);
            }
        }
        
        return image;
    }
    
    /**
     * Extract P8 (palette 8-bit) texture data to a grayscale image.
     * 
     * @param exportData the raw export data
     * @param width texture width
     * @param height texture height
     * @return the extracted image, or null if data not found
     */
    public static BufferedImage extractP8(byte[] exportData, int width, int height) {
        int p8Size = width * height;
        int dataOffset = findTextureData(exportData, p8Size);
        if (dataOffset < 0) return null;
        
        BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_BYTE_GRAY);
        
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int gray = exportData[dataOffset + y * width + x] & 0xFF;
                int rgb = (gray << 16) | (gray << 8) | gray;
                image.setRGB(x, y, rgb);
            }
        }
        
        return image;
    }
    
    /**
     * Extract G16 (16-bit grayscale) texture data to an image.
     * 
     * @param exportData the raw export data
     * @param width texture width
     * @param height texture height
     * @return the extracted image, or null if data not found
     */
    public static BufferedImage extractG16(byte[] exportData, int width, int height) {
        int g16Size = width * height * 2;
        int dataOffset = findTextureData(exportData, g16Size);
        if (dataOffset < 0) return null;
        
        BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_USHORT_GRAY);
        ByteBuffer buffer = ByteBuffer.wrap(exportData, dataOffset, g16Size);
        buffer.order(ByteOrder.LITTLE_ENDIAN);
        
        short[] raster = new short[width * height];
        for (int i = 0; i < raster.length; i++) {
            raster[i] = buffer.getShort();
        }
        image.getRaster().setDataElements(0, 0, width, height, raster);
        
        return image;
    }
    
    /**
     * Extract DXT1 texture data to an image.
     * 
     * @param exportData the raw export data
     * @param width texture width
     * @param height texture height
     * @return the extracted image, or null if data not found
     */
    public static BufferedImage extractDXT1(byte[] exportData, int width, int height) {
        int dxt1Size = (width / 4) * (height / 4) * 8;
        int dataOffset = findTextureData(exportData, dxt1Size);
        if (dataOffset < 0) return null;
        
        byte[] dxtData = new byte[dxt1Size];
        System.arraycopy(exportData, dataOffset, dxtData, 0, dxt1Size);
        
        return decompressDXT1(dxtData, width, height);
    }
    
    /**
     * Extract DXT3 texture data to an image.
     * 
     * @param exportData the raw export data
     * @param width texture width
     * @param height texture height
     * @return the extracted image, or null if data not found
     */
    public static BufferedImage extractDXT3(byte[] exportData, int width, int height) {
        int dxt3Size = (width / 4) * (height / 4) * 16;
        int dataOffset = findTextureData(exportData, dxt3Size);
        if (dataOffset < 0) return null;
        
        byte[] dxtData = new byte[dxt3Size];
        System.arraycopy(exportData, dataOffset, dxtData, 0, dxt3Size);
        
        return decompressDXT3(dxtData, width, height);
    }
    
    /**
     * Extract DXT5 texture data to an image.
     * 
     * @param exportData the raw export data
     * @param width texture width
     * @param height texture height
     * @return the extracted image, or null if data not found
     */
    public static BufferedImage extractDXT5(byte[] exportData, int width, int height) {
        int dxt5Size = (width / 4) * (height / 4) * 16;
        int dataOffset = findTextureData(exportData, dxt5Size);
        if (dataOffset < 0) return null;
        
        byte[] dxtData = new byte[dxt5Size];
        System.arraycopy(exportData, dataOffset, dxtData, 0, dxt5Size);
        
        return decompressDXT5(dxtData, width, height);
    }
    
    // ==================== UE2 UTexture mip tail ====================

    /** Bytes after the last mip: Format, UBits, VBits (1 each), USize, VSize, UClamp, VClamp (4 each). */
    private static final int UTEXTURE_TAIL_BYTES = 19;

    /** One validated mip-0 level inside a UE2 UTexture export. */
    public static final class TextureMip {
        /** Pixel data offset inside the export bytes. */
        public final int dataOffset;
        /** Pixel data byte count. */
        public final int size;
        public final int width;
        public final int height;
        /** Total mip levels the export carries. */
        public final int mipCount;
        /** Offset of the mip-count field (diagnostics). */
        public final int arrayStart;
        /** true: the count is an INT32; false: a single BYTE. Meaningless for layout "chain". */
        public final boolean wideCount;
        /** Which validated family matched: "int-count", "byte-count", or "chain". */
        public final String layout;

        private TextureMip(int dataOffset, int size, int width, int height, int mipCount, int arrayStart, boolean wideCount, String layout) {
            this.dataOffset = dataOffset;
            this.size = size;
            this.width = width;
            this.height = height;
            this.mipCount = mipCount;
            this.arrayStart = arrayStart;
            this.wideCount = wideCount;
            this.layout = layout;
        }
    }

    /**
     * Locate mip0 inside a UE2 UTexture export.
     *
     * <p>Two families are tried at every plausible start offset. "Count": a
     * mip-array count (INT32 or BYTE), then per mip INT32 widthOffset,
     * compact-index byte size, pixel data, INT32 USize, INT32 VSize, BYTE
     * UBits, BYTE VBits, closed by the 19-byte Format/UBits/VBits/USize/
     * VSize/UClamp/VClamp tail. "Chain" (pkg.version 123, the Interlude
     * layout): no count and no offset table — [compact-index size][data]
     * [USize][VSize][UBits][VBits] repeated to EOF. A candidate is accepted
     * only when it consumes the export exactly and every field matches the
     * texture properties (and, for the chain, the halved mip dims and the
     * format's size law). Zero or several survivors is a null result with
     * the reasons in diag — never a silent first fit.</p>
     */
    public static TextureMip findMip(byte[] export, int width, int height, int formatByte,
                                     int uBits, int vBits, StringBuilder diag) {
        int limit = Math.min(export.length, 4096);
        TextureMip found = null;
        for (int start = 0; start < limit; start++) {
            for (int mode = 0; mode < 3; mode++) {
                TextureMip mip;
                if (mode == 2) {
                    mip = walkMipChain(export, start, width, height, formatByte, uBits, vBits);
                } else {
                    mip = walkMipArray(export, start, mode == 0, width, height, formatByte, uBits, vBits);
                }
                if (mip == null) continue;
                if (found == null) {
                    found = mip;
                } else if (found.dataOffset != mip.dataOffset || found.size != mip.size) {
                    diag.append("findMip: ambiguous — a second layout validates at export+")
                        .append(start).append(' ').append(mip.layout).append('\n');
                    return null;
                }
            }
        }
        if (found == null) {
            diag.append("findMip: no mip array layout validates against USize=").append(width)
                .append(" VSize=").append(height).append(" Format=").append(formatByte)
                .append(" UBits=").append(uBits).append(" VBits=").append(vBits)
                .append(" export bytes=").append(export.length).append('\n');
        }
        return found;
    }

    /**
     * Walk one candidate mip array. Every check is structural; the caller
     * enumerates starts and count widths, so a null here just means the next
     * candidate is tried.
     */
    private static TextureMip walkMipArray(byte[] export, int start, boolean wideCount,
                                           int width, int height, int formatByte, int uBits, int vBits) {
        int p = start;
        long count;
        if (wideCount) {
            if (p + 4 > export.length) return null;
            count = readUInt32(export, p);
            p += 4;
        } else {
            if (p >= export.length) return null;
            count = export[p] & 0xFF;
            p += 1;
        }
        if (count < 1 || count > 64) return null;
        int dataOffset = -1;
        int dataSize = -1;
        for (int m = 0; m < count; m++) {
            if (p + 4 > export.length) return null;
            p += 4; // widthOffset: absolute file offset of the data end; pixel read does not need it
            if (p >= export.length) return null;
            int size = readCompactIndex(export, p);
            if (size <= 0) return null;
            int indexLen = compactIndexLength(size);
            if (p + indexLen + size + 10 > export.length) return null;
            p += indexLen;
            int off = p;
            p += size;
            int mipW = readInt32(export, p);
            int mipH = readInt32(export, p + 4);
            p += 10; // + UBits + VBits bytes
            if (m == 0) {
                if (mipW != width || mipH != height) return null;
                dataOffset = off;
                dataSize = size;
            } else if (mipW < 1 || mipH < 1 || mipW > width || mipH > height) {
                return null;
            }
        }
        if (p != export.length - UTEXTURE_TAIL_BYTES) return null;
        if ((export[p] & 0xFF) != formatByte) return null;
        if ((export[p + 1] & 0xFF) != uBits) return null;
        if ((export[p + 2] & 0xFF) != vBits) return null;
        if (readInt32(export, p + 3) != width) return null;
        if (readInt32(export, p + 7) != height) return null;
        return new TextureMip(dataOffset, dataSize, width, height, (int) count, start, wideCount,
            wideCount ? "int-count" : "byte-count");
    }

    /**
     * Walk the pkg-123 bare mip chain: no array count and no per-mip offset
     * table — just [compact-index byte size][pixel data][INT32 USize]
     * [INT32 VSize][BYTE UBits][BYTE VBits] repeated until the export ends.
     * Proven against live Interlude exports (e.g. Lobby_terrain_t.Height:
     * 55-byte native prefix after the None tag, then a single 256x256 G16
     * block whose size index is the 3 bytes 40 80 10, dims after the data,
     * EOF right after VBits). Every block must match the halved mip dims and,
     * for formats with a known size law, the exact byte count; the chain must
     * consume the export exactly. Anything unexplained is a null here, never
     * a partial fit.
     */
    private static TextureMip walkMipChain(byte[] export, int start,
                                           int width, int height, int formatByte, int uBits, int vBits) {
        int p = start;
        int dataOffset = -1;
        int dataSize = -1;
        int count = 0;
        for (int m = 0; m < 64; m++) {
            int mipW = Math.max(1, width >> m);
            int mipH = Math.max(1, height >> m);
            if (p >= export.length) return null;
            int size = readCompactIndex(export, p);
            if (size <= 0) return null;
            int indexLen = compactIndexLength(size);
            int expect = expectedMipBytes(mipW, mipH, formatByte);
            if (expect >= 0 && size != expect) return null;
            if (p + indexLen + size + 10 > export.length) return null;
            p += indexLen;
            int off = p;
            p += size;
            if (readInt32(export, p) != mipW) return null;
            if (readInt32(export, p + 4) != mipH) return null;
            int mipUBits = export[p + 8] & 0xFF;
            int mipVBits = export[p + 9] & 0xFF;
            if (mipUBits != log2(mipW) || mipVBits != log2(mipH)) return null;
            p += 10;
            if (m == 0) {
                if (mipUBits != uBits || mipVBits != vBits) return null;
                dataOffset = off;
                dataSize = size;
            }
            count++;
            if (p == export.length) {
                return new TextureMip(dataOffset, dataSize, width, height, count, start, false, "chain");
            }
            if (mipW == 1 && mipH == 1) return null; // trailing bytes after the 1x1 mip are unexplained
        }
        return null;
    }

    /**
     * Serialized bytes of one mip level for an ETextureFormat byte, or -1
     * when the format has no enforced size law (NODATA or an enum value this
     * parser does not know — the chain then validates structurally).
     */
    private static int expectedMipBytes(int mipW, int mipH, int formatByte) {
        switch (formatByte) {
            case 0:  // TXF_P8
            case 9:  // TXF_L8
                return mipW * mipH;
            case 1:  // TXF_RGBA7
            case 2:  // TXF_RGB16
            case 10: // TXF_G16
            case 11: // TXF_R16f
                return mipW * mipH * 2;
            case 4:  // TXF_RGB8
                return mipW * mipH * 3;
            case 5:  // TXF_RGBA8
                return mipW * mipH * 4;
            case 3:  // TXF_DXT1
                return Math.max(1, (mipW + 3) / 4) * Math.max(1, (mipH + 3) / 4) * 8;
            case 7:  // TXF_DXT3
            case 8:  // TXF_DXT5
                return Math.max(1, (mipW + 3) / 4) * Math.max(1, (mipH + 3) / 4) * 16;
            default:
                return -1;
        }
    }

    private static int log2(int v) {
        int n = 0;
        while ((1 << n) < v) n++;
        return n;
    }

    /** Head and tail hex dump of an export for a failed parse. */
    public static String dumpHeadTail(byte[] data, int head, int tail) {
        StringBuilder sb = new StringBuilder();
        sb.append("head ").append(Math.min(head, data.length)).append(" bytes: ")
            .append(hex(data, 0, Math.min(head, data.length))).append('\n');
        int from = Math.max(0, data.length - tail);
        sb.append("tail ").append(data.length - from).append(" bytes at +").append(from).append(": ")
            .append(hex(data, from, data.length - from));
        return sb.toString();
    }

    public static String hex(byte[] data, int offset, int length) {
        StringBuilder sb = new StringBuilder(length * 3);
        for (int i = 0; i < length; i++) {
            if (i > 0) sb.append(' ');
            sb.append(Character.forDigit((data[offset + i] >> 4) & 0xF, 16));
            sb.append(Character.forDigit(data[offset + i] & 0xF, 16));
        }
        return sb.toString();
    }

    public static int readInt32(byte[] data, int offset) {
        return (data[offset] & 0xFF)
            | ((data[offset + 1] & 0xFF) << 8)
            | ((data[offset + 2] & 0xFF) << 16)
            | ((data[offset + 3] & 0xFF) << 24);
    }

    public static long readUInt32(byte[] data, int offset) {
        return readInt32(data, offset) & 0xFFFFFFFFL;
    }

    // ==================== Color Conversion Helpers ====================
    
    /**
     * Convert RGB565 color to ARGB (with full alpha).
     */
    public static int rgb565ToArgb(int color) {
        int r = ((color >> 11) & 0x1F) * 255 / 31;
        int g = ((color >> 5) & 0x3F) * 255 / 63;
        int b = (color & 0x1F) * 255 / 31;
        return 0xFF000000 | (r << 16) | (g << 8) | b;
    }
    
    /**
     * Convert RGB565 color to RGB (no alpha).
     */
    public static int rgb565ToRgb(int color) {
        int r = ((color >> 11) & 0x1F) * 255 / 31;
        int g = ((color >> 5) & 0x3F) * 255 / 63;
        int b = (color & 0x1F) * 255 / 31;
        return (r << 16) | (g << 8) | b;
    }
    
    /**
     * Interpolate between two ARGB colors.
     */
    public static int interpolateColor(int c0, int c1, int w0, int w1) {
        int total = w0 + w1;
        int r = (((c0 >> 16) & 0xFF) * w0 + ((c1 >> 16) & 0xFF) * w1) / total;
        int g = (((c0 >> 8) & 0xFF) * w0 + ((c1 >> 8) & 0xFF) * w1) / total;
        int b = ((c0 & 0xFF) * w0 + (c1 & 0xFF) * w1) / total;
        return 0xFF000000 | (r << 16) | (g << 8) | b;
    }
    
    /**
     * Interpolate between two RGB colors (no alpha).
     */
    public static int interpolateColorNoAlpha(int c0, int c1, int w0, int w1) {
        int total = w0 + w1;
        int r = (((c0 >> 16) & 0xFF) * w0 + ((c1 >> 16) & 0xFF) * w1) / total;
        int g = (((c0 >> 8) & 0xFF) * w0 + ((c1 >> 8) & 0xFF) * w1) / total;
        int b = ((c0 & 0xFF) * w0 + (c1 & 0xFF) * w1) / total;
        return (r << 16) | (g << 8) | b;
    }
}
