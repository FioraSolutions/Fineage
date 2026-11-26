package net.shrimpworks.unreal.packages;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.channels.SeekableByteChannel;
import java.util.List;

import net.shrimpworks.unreal.packages.entities.objects.Model;
import net.shrimpworks.unreal.packages.entities.objects.geometry.Node;

/**
 * One L2 BSP node (version 123) round-trips through {@link Node#readL2}, and a
 * two-node walk returns the front leaf only when back is serialized before front.
 */
public final class ModelLayoutSelfTest {

	public static void main(String[] args) throws Exception {
		byte[] parent = node(1f, 0f, 0f, 0f, -1, 1, 9, 8);
		byte[] leaf = node(0f, 0f, 1f, 0f, -1, -1, 4, 3);
		byte[] all = new byte[parent.length + leaf.length];
		System.arraycopy(parent, 0, all, 0, parent.length);
		System.arraycopy(leaf, 0, all, parent.length, leaf.length);

		PackageReader reader = new PackageReader(new Bytes(all));
		reader.version = 123;
		reader.moveTo(0);
		Node n0 = Node.readL2(reader);
		Node n1 = Node.readL2(reader);
		if (reader.currentPosition() != all.length) {
			fail("consumed " + reader.currentPosition() + " of " + all.length);
		}
		if (n0.back != -1 || n0.front != 1) fail("node0 back/front " + n0.back + "/" + n0.front);
		if (n0.zone[0] != 9 || n0.zone[1] != 8) fail("node0 zones " + n0.zone[0] + "," + n0.zone[1]);
		if (n1.zone[0] != 4 || n1.zone[1] != 3) fail("node1 zones " + n1.zone[0] + "," + n1.zone[1]);

		List<Node> nodes = List.of(n0, n1);
		int onFront = Model.pointZone(nodes, 1, 0, 0);
		if (onFront != 3) fail("point on +X returned zone " + onFront + " (expected 3, the child leaf's front zone)");
		int onBack = Model.pointZone(nodes, -1, 0, 0);
		if (onBack != 9) fail("point on -X returned zone " + onBack + " (expected 9, node0's back zone)");
		if (!Model.usesL2Layout(123, 19) || !Model.usesL2Layout(123, 28)) {
			fail("version 123 licensees 19 and 28 must use the L2 UModel layout");
		}
		if (Model.usesL2Layout(123, 15) || Model.usesL2Layout(123, 37) || Model.usesL2Layout(126, 28)) {
			fail("UModel layout range accepted a package outside version 123 licensee 16..36");
		}
		emptyTail();
		oneSectionTail();
		try {
			Model.readL2Remainder(readerOf(new byte[]{0x01, 0x00}), 2, 0);
			fail("a non-empty index with no section frame was accepted");
		} catch (IllegalStateException expected) {
			if (expected.getMessage() == null || !expected.getMessage().contains("lightmap")) {
				fail("unexpected tail error: " + expected.getMessage());
			}
		}
		System.out.println("ModelLayoutSelfTest ok");
	}

	/** Two compact zeros, the brush-model tail. */
	static void emptyTail() {
		PackageReader reader = readerOf(new byte[]{0, 0});
		Model.readL2Remainder(reader, 2, 0);
		if (reader.currentPosition() != 2) fail("empty tail consumed " + reader.currentPosition());
	}

	/**
	 * Index 2, one header byte, section count 1, one section (marker 3, the
	 * ints 0 and 1, one payload byte), then the closing ints 0 and 1.
	 */
	static void oneSectionTail() {
		ByteBuffer buf = ByteBuffer.allocate(64).order(ByteOrder.LITTLE_ENDIAN);
		buf.put((byte) 2); // index count
		buf.put((byte) 0x11); // header
		buf.put((byte) 1); // one section
		buf.put((byte) 3); // unknown-struct count
		buf.put((byte) 2); // index count again
		buf.putInt(0);
		buf.putInt(1);
		buf.put((byte) 0x5A); // payload
		buf.put((byte) 2);
		buf.putInt(0);
		buf.putInt(1);
		byte[] raw = new byte[buf.position()];
		buf.flip();
		buf.get(raw);
		PackageReader reader = readerOf(raw);
		Model.readL2Remainder(reader, raw.length, 3);
		if (reader.currentPosition() != raw.length) fail("section tail consumed " + reader.currentPosition());
	}

	static PackageReader readerOf(byte[] raw) {
		PackageReader reader = new PackageReader(new Bytes(raw));
		reader.version = 123;
		reader.moveTo(0);
		return reader;
	}

	static byte[] node(float nx, float ny, float nz, float w, int back, int front, int zoneBack, int zoneFront) {
		ByteBuffer buf = ByteBuffer.allocate(256).order(ByteOrder.LITTLE_ENDIAN);
		buf.putFloat(nx);
		buf.putFloat(ny);
		buf.putFloat(nz);
		buf.putFloat(w);
		buf.putLong(0);
		buf.put((byte) 0);
		index(buf, 0);
		index(buf, 0);
		index(buf, back);
		index(buf, front);
		index(buf, 0);
		index(buf, 0);
		index(buf, 0);
		buf.putFloat(0);
		buf.putFloat(0);
		buf.putFloat(0);
		buf.putInt(0);
		buf.putLong(0);
		buf.putLong(0);
		index(buf, zoneBack);
		index(buf, zoneFront);
		buf.put((byte) 0);
		buf.putInt(-1);
		buf.putInt(-1);
		buf.put(new byte[12]);
		byte[] out = new byte[buf.position()];
		buf.flip();
		buf.get(out);
		return out;
	}

	static void index(ByteBuffer buf, int v) {
		if (v >= 0 && v < 64) {
			buf.put((byte) v);
			return;
		}
		if (v < 0 && v > -64) {
			buf.put((byte) (0x80 | -v));
			return;
		}
		throw new IllegalStateException("test index out of one-byte range: " + v);
	}

	static void fail(String msg) {
		System.err.println("ERROR " + msg);
		System.exit(1);
	}

	static final class Bytes implements SeekableByteChannel {
		final byte[] data;
		long pos;
		boolean open = true;

		Bytes(byte[] data) { this.data = data; }

		@Override public int read(ByteBuffer dst) {
			if (pos >= data.length) return -1;
			int n = Math.min(dst.remaining(), (int) (data.length - pos));
			dst.put(data, (int) pos, n);
			pos += n;
			return n;
		}

		@Override public int write(ByteBuffer src) { throw new UnsupportedOperationException(); }

		@Override public long position() { return pos; }

		@Override public SeekableByteChannel position(long newPosition) {
			pos = newPosition;
			return this;
		}

		@Override public long size() { return data.length; }

		@Override public SeekableByteChannel truncate(long size) { throw new UnsupportedOperationException(); }

		@Override public boolean isOpen() { return open; }

		@Override public void close() { open = false; }
	}
}
