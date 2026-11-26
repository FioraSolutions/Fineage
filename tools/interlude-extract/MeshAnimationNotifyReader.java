import com.rost.cruma.core.unreal.UArchive;
import com.rost.cruma.core.unreal.UObjectExport;
import com.rost.cruma.core.unreal.UObjectImport;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

/**
 * Reads MeshAnimation native sequence data, including every AnimNotify binding.
 * Cruma's UMeshAnimation keeps only AttackShot / AttackPreShot / Channeling times
 * and camera marks; this reader keeps the full (time, function, object) list.
 *
 * Layout matches {@code com.rost.cruma.core.unreal.UMeshAnimSequences#read}
 * and {@code UMeshAnimation#deserialize}. Keyframe motion tracks are skipped,
 * not decoded.
 */
public final class MeshAnimationNotifyReader {
    static final long RF_HAS_STACK = 33554432L;

    public static final class Notify {
        public final float time;
        public final String function;
        public final int objectIndex;
        public final String notifyClass;
        public final String notifyName;
        public final String objectRef;

        Notify(float time, String function, int objectIndex,
               String notifyClass, String notifyName, String objectRef) {
            this.time = time;
            this.function = function == null ? "" : function;
            this.objectIndex = objectIndex;
            this.notifyClass = notifyClass == null ? "" : notifyClass;
            this.notifyName = notifyName == null ? "" : notifyName;
            this.objectRef = objectRef == null ? "" : objectRef;
        }
    }

    public static final class Sequence {
        public final String name;
        public final int startFrame;
        public final int frameCount;
        public final float rate;
        public final float shotTime;
        public final float preShotTime;
        public final List<Float> shotTimes;
        public final List<Float> preShotTimes;
        public final List<Float> channelTimes;
        public final List<Notify> notifies;

        Sequence(String name, int startFrame, int frameCount, float rate,
                 float shotTime, float preShotTime,
                 List<Float> shotTimes, List<Float> preShotTimes, List<Float> channelTimes,
                 List<Notify> notifies) {
            this.name = name;
            this.startFrame = startFrame;
            this.frameCount = frameCount;
            this.rate = rate;
            this.shotTime = shotTime;
            this.preShotTime = preShotTime;
            this.shotTimes = shotTimes;
            this.preShotTimes = preShotTimes;
            this.channelTimes = channelTimes;
            this.notifies = notifies;
        }
    }

    public static final class Animation {
        public final String name;
        public final int version;
        public final int boneCount;
        public final List<Sequence> sequences;

        Animation(String name, int version, int boneCount, List<Sequence> sequences) {
            this.name = name;
            this.version = version;
            this.boneCount = boneCount;
            this.sequences = sequences;
        }
    }

    private MeshAnimationNotifyReader() {}

    public static Animation read(UArchive archive, UObjectExport exp) {
        int saved = archive.getPosition();
        int savedEnd = archive.getObjectEnd();
        try {
            archive.setPosition(exp.getSerialOffset());
            archive.setObjectEnd(exp.getSerialOffset() + exp.getSerialSize());
            if ((exp.getObjectFlags() & RF_HAS_STACK) != 0L) {
                skipStateFrame(archive);
            }
            archive.getPropertyExtractor().extractProperties();
            int version = archive.readInt();
            int boneCount = readBoneCount(archive);
            skipMotionChunks(archive);
            int sequenceCount = archive.readIndex();
            requireCount("sequences", sequenceCount, exp);
            List<Sequence> sequences = new ArrayList<>(sequenceCount);
            for (int i = 0; i < sequenceCount; i++) {
                sequences.add(readSequence(archive, exp));
            }
            return new Animation(exp.getObjectName(), version, boneCount, sequences);
        } finally {
            archive.setObjectEnd(savedEnd);
            archive.setPosition(saved);
        }
    }

    static int readBoneCount(UArchive archive) {
        int count = archive.readIndex();
        requireCount("bones", count, null);
        for (int i = 0; i < count; i++) {
            archive.readName();
            archive.readInt();
            archive.readInt();
        }
        return count;
    }

    static void skipMotionChunks(UArchive archive) {
        var header = archive.getHeader();
        if (header.getFileVersion() > 122 && header.getLicenseVersion() > 24) {
            int end = archive.readInt();
            int count = archive.readIndex();
            requireCount("motion chunks", count, null);
            for (int i = 0; i < count; i++) {
                int chunkEnd = archive.readInt();
                archive.setPosition(chunkEnd);
            }
            archive.setPosition(end);
            return;
        }
        int count = archive.readIndex();
        requireCount("motion chunks", count, null);
        for (int i = 0; i < count; i++) {
            skipMotion(archive);
        }
    }

    static void skipMotion(UArchive archive) {
        archive.skipBytes(24);
        skipArray(archive, 4);
        int tracks = archive.readIndex();
        requireCount("tracks", tracks, null);
        for (int i = 0; i <= tracks; i++) {
            archive.readInt();
            skipArray(archive, 16);
            skipArray(archive, 12);
            skipArray(archive, 4);
        }
    }

    static void skipArray(UArchive archive, int elementSize) {
        int count = archive.readIndex();
        requireCount("keys", count, null);
        archive.skipBytes(Math.multiplyExact(count, elementSize));
    }

    static Sequence readSequence(UArchive archive, UObjectExport exp) {
        short fileVersion = archive.getHeader().getFileVersion();
        if (fileVersion >= 115) {
            archive.readFloat(); // bookmark
        }
        String name = archive.readName();
        int groupCount = archive.readIndex();
        requireCount("animation groups", groupCount, exp);
        for (int g = 0; g < groupCount; g++) {
            archive.readName();
        }
        int startFrame = archive.readInt();
        int sequenceFrames = archive.readInt();
        int notifyCount = archive.readIndex();
        requireCount("animation notifies", notifyCount, exp);
        float shotTime = -1.0f;
        float preShotTime = -1.0f;
        List<Float> shotTimes = new ArrayList<>();
        List<Float> preShotTimes = new ArrayList<>();
        List<Float> channelTimes = new ArrayList<>();
        List<Notify> notifies = new ArrayList<>(notifyCount);
        for (int n = 0; n < notifyCount; n++) {
            float time = archive.readFloat();
            String function = archive.readName();
            int objectIndex = 0;
            String notifyClass = "";
            String notifyName = "";
            String objectRef = "";
            if (fileVersion >= 112) {
                objectIndex = archive.readIndex();
                Resolved ref = resolveObject(archive, objectIndex);
                notifyClass = ref.className;
                notifyName = ref.objectName;
                objectRef = ref.ref;
                if ("AnimNotify_AttackShot".equalsIgnoreCase(notifyClass)) {
                    shotTime = time;
                    shotTimes.add(time);
                } else if ("AnimNotify_AttackPreShot".equalsIgnoreCase(notifyClass)) {
                    preShotTime = preShotTime < 0.0f ? time : Math.min(preShotTime, time);
                    preShotTimes.add(time);
                } else if ("AnimNotify_Channeling".equalsIgnoreCase(notifyClass)) {
                    channelTimes.add(time);
                }
            }
            if (fileVersion > 130) {
                int characters = archive.readInt();
                if (characters > 0) {
                    archive.skipBytes(Math.multiplyExact(characters, 2));
                }
            }
            notifies.add(new Notify(time, function, objectIndex, notifyClass, notifyName, objectRef));
        }
        float rate = archive.readFloat();
        skipLicenseeFields(archive);
        shotTimes.sort(Float::compareTo);
        preShotTimes.sort(Float::compareTo);
        channelTimes.sort(Float::compareTo);
        return new Sequence(
            name, startFrame, sequenceFrames, rate,
            shotTime, preShotTime, shotTimes, preShotTimes, channelTimes, notifies);
    }

    static void skipLicenseeFields(UArchive archive) {
        short licensee = archive.getHeader().getLicenseVersion();
        if (licensee <= 0) return;
        if (licensee == 1) {
            archive.readInt();
            archive.readInt();
            archive.readIndex();
            return;
        }
        archive.readInt();
        archive.readInt();
        archive.readInt();
        archive.readIndex();
        if (licensee >= 20) archive.readInt();
        if (licensee >= 25) archive.readInt();
        if (licensee == 26) {
            skipArray(archive, 8);
        } else if (licensee > 26) {
            archive.readByte();
            skipArray(archive, 8);
            int groupCount = archive.readIndex();
            requireCount("animation sequence pair groups", groupCount, null);
            for (int group = 0; group < groupCount; group++) {
                archive.readInt();
                skipArray(archive, 8);
            }
            archive.readInt();
            archive.readInt();
            skipArray(archive, 8);
        }
    }

    static final class Resolved {
        final String className;
        final String objectName;
        final String ref;
        Resolved(String className, String objectName, String ref) {
            this.className = className;
            this.objectName = objectName;
            this.ref = ref;
        }
    }

    static Resolved resolveObject(UArchive archive, int index) {
        if (index > 0 && index <= archive.getExportMap().size()) {
            UObjectExport e = archive.getExportMap().get(index - 1);
            String cls = e.getClassName() == null ? "" : e.getClassName();
            String name = e.getObjectName() == null ? "" : e.getObjectName();
            String pkg = archive.getName() == null ? "" : archive.getName();
            String ref = "export:" + pkg + ":" + (index - 1) + ":" + cls + ":" + name;
            return new Resolved(cls, name, ref);
        }
        if (index < 0 && (-index) <= archive.getImportMap().size()) {
            UObjectImport e = archive.getImportMap().get(-index - 1);
            String cls = e.getClassName() == null ? "" : e.getClassName();
            String name = e.getObjectName() == null ? "" : e.getObjectName();
            String pkg = e.getClassPackage() == null ? "" : e.getClassPackage();
            String ref = "import:" + pkg + ":" + cls + ":" + name;
            return new Resolved(cls, name, ref);
        }
        if (index == 0) return new Resolved("", "", "none");
        return new Resolved("", "", "index:" + index);
    }

    static void skipStateFrame(UArchive archive) {
        // UObject.StateFrame: package-private. Mirror the common UE2 layout
        // (name, node, probe mask, latent, offset) via reflection when present.
        try {
            Class<?> sf = Class.forName("com.rost.cruma.core.unreal.UObject$StateFrame");
            var m = sf.getDeclaredMethod("deserialize", UArchive.class);
            m.setAccessible(true);
            m.invoke(null, archive);
        } catch (ReflectiveOperationException e) {
            throw new IllegalStateException("Cannot skip MeshAnimation state frame: " + e, e);
        }
    }

    static void requireCount(String label, int count, UObjectExport exp) {
        if (count < 0 || count > 4_000_000) {
            String who = exp == null ? "" : (" " + exp.getObjectName());
            throw new IllegalStateException("Malformed MeshAnimation" + who + ": invalid " + label + " count " + count);
        }
    }

    public static String formatTime(float t) {
        return String.format(Locale.ROOT, "%.9g", t);
    }
}
