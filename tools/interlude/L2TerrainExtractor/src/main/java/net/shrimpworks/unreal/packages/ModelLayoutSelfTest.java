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
		System.out.println("ModelLayoutSelfTest ok");
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
