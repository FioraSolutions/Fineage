import com.rost.cruma.core.unreal.ClassDefaults;
import com.rost.cruma.core.unreal.ClientSearchPaths;
import com.rost.cruma.core.unreal.SkillEffects;
import com.rost.cruma.core.unreal.UArchive;
import com.rost.cruma.core.unreal.UArchiveLoader;
import com.rost.cruma.core.unreal.UArchiveLoader.USearchConfig;
import com.rost.cruma.core.unreal.UObjectExport;
import com.rost.cruma.core.unreal.UObjectImport;
import com.rost.cruma.core.unreal.UProperty;
import com.rost.cruma.core.unreal.primitives.UColor;
import com.rost.cruma.core.unreal.primitives.URotator;
import java.io.BufferedWriter;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.Set;

/**
 * Lossless dump of every Unreal property on every export in a package,
 * plus every referenced object in other packages (no binary payloads).
 */
public final class ExportPackageProperties {
    static final class Json {
        final StringBuilder sb = new StringBuilder();
        int indent;
        boolean pendingComma;
        final boolean compact;

        Json() { this(false); }
        Json(boolean compact) { this.compact = compact; }

        void nl() {
            if (compact) return;
            sb.append('\n');
            for (int i = 0; i < indent; i++) sb.append("  ");
        }

        void startObj() {
            comma();
            sb.append('{');
            indent++;
            pendingComma = false;
        }

        void endObj() {
            indent--;
            nl();
            sb.append('}');
            pendingComma = true;
        }

        void startArr() {
            comma();
            sb.append('[');
            indent++;
            pendingComma = false;
        }

        void endArr() {
            indent--;
            nl();
            sb.append(']');
            pendingComma = true;
        }

        void comma() {
            if (pendingComma) sb.append(',');
            pendingComma = false;
        }

        void key(String k) {
            comma();
            nl();
            sb.append('"').append(esc(k)).append("\": ");
        }

        void raw(String v) { sb.append(v); pendingComma = true; }

        void valNull() { comma(); sb.append("null"); pendingComma = true; }

        void val(String v) {
            comma();
            if (v == null) sb.append("null");
            else sb.append('"').append(esc(v)).append('"');
            pendingComma = true;
        }

        void val(boolean v) { comma(); sb.append(v ? "true" : "false"); pendingComma = true; }

        void val(long v) { comma(); sb.append(v); pendingComma = true; }

        void val(double v) {
            comma();
            if (v == (long) v) sb.append(String.format(Locale.ROOT, "%.1f", v));
            else sb.append(String.format(Locale.ROOT, "%.9g", v));
            pendingComma = true;
        }

        void keyVal(String k, String v) { key(k); val(v); }
        void keyVal(String k, boolean v) { key(k); val(v); }
        void keyVal(String k, long v) { key(k); val(v); }
        void keyVal(String k, double v) { key(k); val(v); }
        void keyNull(String k) { key(k); valNull(); }

        static String esc(String s) {
            StringBuilder o = new StringBuilder();
            for (int i = 0; i < s.length(); i++) {
                char c = s.charAt(i);
                switch (c) {
                    case '\\' -> o.append("\\\\");
                    case '"' -> o.append("\\\"");
                    case '\n' -> o.append("\\n");
                    case '\r' -> o.append("\\r");
                    case '\t' -> o.append("\\t");
                    default -> {
                        if (c < 0x20) o.append(String.format(Locale.ROOT, "\\u%04x", (int) c));
                        else o.append(c);
                    }
                }
            }
            return o.toString();
        }

        @Override public String toString() { return sb.toString(); }
    }

    static String resolve(UArchive a, int index) {
        if (index > 0 && index <= a.getExportMap().size()) {
            UObjectExport e = a.getExportMap().get(index - 1);
            return "export:" + a.getName() + ":" + (index - 1) + ":" + e.getClassName() + ":" + e.getObjectName();
        }
        if (index < 0 && (-index) <= a.getImportMap().size()) {
            UObjectImport e = a.getImportMap().get(-index - 1);
            String pkg = e.getClassPackage() == null ? "" : e.getClassPackage();
            return "import:" + pkg + ":" + e.getClassName() + ":" + e.getObjectName();
        }
        return index == 0 ? "none" : ("index:" + index);
    }

    static String importClassName(UArchive a, int index) {
        if (index >= 0 || (-index) > a.getImportMap().size()) return null;
        return a.getImportMap().get(-index - 1).getObjectName();
    }

    static String importClassPackage(UArchive a, int index) {
        if (index >= 0 || (-index) > a.getImportMap().size()) return null;
        // classPackage on a Class import is often "Core"; packageIndex points at the package import
        UObjectImport imp = a.getImportMap().get(-index - 1);
        int pkg = imp.getPackageIndex();
        if (pkg < 0 && (-pkg) <= a.getImportMap().size()) {
            return a.getImportMap().get(-pkg - 1).getObjectName();
        }
        return imp.getClassPackage();
    }

    static List<UProperty> extract(UArchive a, UObjectExport exp) {
        a.setPosition(exp.getSerialOffset());
        a.setObjectEnd(exp.getSerialOffset() + exp.getSerialSize());
        return new ArrayList<>(a.getPropertyExtractor().extractProperties());
    }

    static void writeProps(Json j, UArchive a, List<UProperty> props) {
        j.startArr();
        for (UProperty p : props) writeProp(j, a, p);
        j.endArr();
    }

    static void writeProp(Json j, UArchive a, UProperty p) {
        j.comma();
        j.nl();
        j.startObj();
        j.keyVal("name", p.getName());
        String type = p.getType() == null ? "Unknown" : p.getType().name();
        j.keyVal("type", type);
        if (p.isArray()) j.keyVal("array_index", p.getArrayIndex());
        if (p.getStructName() != null && !p.getStructName().isEmpty()) j.keyVal("struct", p.getStructName());
        switch (type) {
            case "Bool" -> j.keyVal("value", p.isTrue());
            case "Float" -> j.keyVal("value", p.getFloatValue());
            case "Int" -> {
                j.keyVal("value", p.getInt32Value());
                String n = a.resolveName(p.getInt32Value());
                if (n != null && !n.isEmpty() && !"None".equalsIgnoreCase(n)) j.keyVal("name_value", n);
            }
            case "Byte" -> j.keyVal("value", p.getUInt8Value());
            case "Name" -> {
                if (p.getStringValue() != null) j.keyVal("value", p.getStringValue());
                else {
                    String n = a.resolveName(p.getIndexValue());
                    if (n != null) j.keyVal("value", n);
                    else j.keyNull("value");
                    j.keyVal("name_index", p.getIndexValue());
                }
            }
            case "Str", "Class" -> j.keyVal("value", p.getStringValue());
            case "Object" -> {
                j.keyVal("index", p.getIndexValue());
                j.keyVal("ref", resolve(a, p.getIndexValue()));
                if (p.getStringValue() != null) j.keyVal("value", p.getStringValue());
            }
            case "Vector" -> writeVec(j, "value", p.getVectorValue());
            case "Rotator" -> writeRot(j, "value", p.getRotatorValue());
            case "Struct" -> {
                if (p.getRotatorValue() != null) writeRot(j, "rotator", p.getRotatorValue());
                if (p.getVectorValue() != null) writeVec(j, "vector", p.getVectorValue());
                if (p.getColorValue() != null) writeColor(j, "color", p.getColorValue());
            }
            case "Array", "FixedArray" -> {
                j.keyVal("array_size", p.getArraySize());
                if (p.getIntArray() != null && p.getIntArray().length > 0) {
                    j.key("int_array");
                    j.startArr();
                    for (int v : p.getIntArray()) { j.comma(); j.nl(); j.val((long) v); }
                    j.endArr();
                }
                if (p.getObjectArray() != null && p.getObjectArray().length > 0) {
                    j.key("object_array");
                    j.startArr();
                    for (int idx : p.getObjectArray()) {
                        j.comma();
                        j.nl();
                        j.startObj();
                        j.keyVal("index", idx);
                        j.keyVal("ref", resolve(a, idx));
                        j.endObj();
                    }
                    j.endArr();
                }
            }
            default -> {
                if (p.getStringValue() != null) j.keyVal("value", p.getStringValue());
                if (p.getDataValue() != null && p.getDataValue().length > 0 && p.getDataValue().length <= 64) {
                    j.keyVal("data_len", p.getDataValue().length);
                } else if (p.getDataValue() != null && p.getDataValue().length > 64) {
                    j.keyVal("data_len", p.getDataValue().length);
                    j.keyVal("data_omitted", "binary payload not stored");
                }
            }
        }
        if (p.getSubProperties() != null && !p.getSubProperties().isEmpty()) {
            j.key("subproperties");
            j.startArr();
            for (Map<String, UProperty> map : p.getSubProperties()) {
                j.comma();
                j.nl();
                writeProps(j, a, new ArrayList<>(map.values()));
            }
            j.endArr();
        }
        j.endObj();
    }

    static void writeVec(Json j, String key, glm_.vec3.Vec3 v) {
        if (v == null) { j.keyNull(key); return; }
        j.key(key);
        j.startObj();
        j.keyVal("x", v.getX());
        j.keyVal("y", v.getY());
        j.keyVal("z", v.getZ());
        j.endObj();
    }

    static void writeRot(Json j, String key, URotator r) {
        if (r == null) { j.keyNull(key); return; }
        j.key(key);
        j.startObj();
        j.keyVal("pitch", r.pitch());
        j.keyVal("yaw", r.yaw());
        j.keyVal("roll", r.roll());
        j.endObj();
    }

    static void writeColor(Json j, String key, UColor c) {
        if (c == null) { j.keyNull(key); return; }
        j.key(key);
        j.startObj();
        try {
            var r = c.getClass().getMethod("r");
            var g = c.getClass().getMethod("g");
            var b = c.getClass().getMethod("b");
            var a = c.getClass().getMethod("a");
            j.keyVal("r", ((Number) r.invoke(c)).longValue());
            j.keyVal("g", ((Number) g.invoke(c)).longValue());
            j.keyVal("b", ((Number) b.invoke(c)).longValue());
            j.keyVal("a", ((Number) a.invoke(c)).longValue());
        } catch (Exception e) {
            j.keyVal("to_string", String.valueOf(c));
        }
        j.endObj();
    }

    static boolean isClassExport(UObjectExport exp) {
        String cn = exp.getClassName();
        return cn == null || cn.isEmpty() || "None".equalsIgnoreCase(cn) || "Class".equalsIgnoreCase(cn);
    }

    static List<UProperty> extractSmart(UArchive a, UObjectExport exp, String[] sourceOut) {
        if (isClassExport(exp)) {
            try {
                List<UProperty> defs = ClassDefaults.findAll(a, exp.getObjectName());
                if (defs != null) {
                    sourceOut[0] = "class_defaults";
                    return new ArrayList<>(defs);
                }
            } catch (Throwable t) {
                sourceOut[0] = "class_defaults_error:" + t;
                return List.of();
            }
            sourceOut[0] = "class_defaults_empty";
            return List.of();
        }
        // Map/actor exports start with a native Unreal header; raw tagged
        // extraction hits a name-index error. Cruma's object loader skips it.
        try {
            var loader = a.getObjectLoader();
            if (loader != null) {
                List<UProperty> props = loader.exportProperties(exp);
                sourceOut[0] = "object_loader_export_properties";
                return props == null ? List.of() : new ArrayList<>(props);
            }
        } catch (Throwable t) {
            sourceOut[0] = "object_loader_error:" + t.getClass().getSimpleName();
        }
        sourceOut[0] = "tagged_properties";
        return extract(a, exp);
    }

    static void writeExport(Json j, UArchive a, int i, UObjectExport exp, List<UProperty> props, String source) {
        j.comma();
        j.nl();
        j.startObj();
        j.keyVal("export_index", i);
        j.keyVal("class", exp.getClassName());
        j.keyVal("name", exp.getObjectName());
        j.keyVal("super", exp.getSuperName());
        j.keyVal("outer_index", exp.getPackageIndex());
        j.keyVal("outer", resolve(a, exp.getPackageIndex()));
        j.keyVal("serial_offset", exp.getSerialOffset());
        j.keyVal("serial_size", exp.getSerialSize());
        if (source != null) j.keyVal("properties_source", source);
        j.key("properties");
        writeProps(j, a, props);
        attachNativeExtras(j, a, exp);
        j.endObj();
    }

    static void attachNativeExtras(Json j, UArchive a, UObjectExport exp) {
        String cls = exp.getClassName();
        if (cls == null) return;
        if ("Sound".equalsIgnoreCase(cls)) {
            j.keyVal("wave_serial_size", exp.getSerialSize());
            try {
                var obj = a.getObjectLoader().exportObject(exp);
                if (obj instanceof com.rost.cruma.core.unreal.USound snd) {
                    byte[] data = snd.getData();
                    if (data != null) j.keyVal("wave_bytes", data.length);
                    // payload intentionally omitted
                }
            } catch (Throwable ignored) {
            }
            return;
        }
        if ("MeshAnimation".equalsIgnoreCase(cls)) {
            try {
                writeMeshAnimationSequences(j, a, exp);
            } catch (Throwable t) {
                j.keyVal("mesh_animation_error", String.valueOf(t));
            }
            return;
        }
        if (isModelClass(cls) || isPolysClass(cls) || isVolumeActorClass(cls) || "Brush".equalsIgnoreCase(cls)) {
            try {
                writeGeometryExtras(j, a, exp);
            } catch (Throwable t) {
                j.keyVal("geometry_error", String.valueOf(t));
            }
        }
    }

    static boolean isModelClass(String cls) {
        return cls != null && "Model".equalsIgnoreCase(cls);
    }

    static boolean isPolysClass(String cls) {
        return cls != null && "Polys".equalsIgnoreCase(cls);
    }

    /** Volume actor types whose official bounds come from a Brush/Model (not point location). */
    static boolean isVolumeActorClass(String cls) {
        if (cls == null) return false;
        return "MusicVolume".equalsIgnoreCase(cls)
            || "WaterVolume".equalsIgnoreCase(cls)
            || "PhysicsVolume".equalsIgnoreCase(cls)
            || "DefaultPhysicsVolume".equalsIgnoreCase(cls)
            || "BlockingVolume".equalsIgnoreCase(cls)
            || "AmbientVolume".equalsIgnoreCase(cls)
            || "Volume".equalsIgnoreCase(cls);
    }

    static void writeGeometryExtras(Json j, UArchive a, UObjectExport exp) {
        String cls = exp.getClassName();
        var loader = a.getObjectLoader();
        if (loader == null) {
            j.keyVal("geometry_error", "object loader is null");
            return;
        }
        var obj = loader.exportObject(exp);
        if (isModelClass(cls)) {
            if (obj instanceof com.rost.cruma.core.unreal.Bsp.UModel model) {
                writeModelGeometry(j, a, model);
            } else {
                j.keyVal("geometry_error", "expected Bsp.UModel, got "
                    + (obj == null ? "null" : obj.getClass().getName()));
            }
            return;
        }
        if (isPolysClass(cls)) {
            if (obj instanceof com.rost.cruma.core.unreal.UPolys polys) {
                writePolys(j, a, "polys", polys);
            } else {
                j.keyVal("geometry_error", "expected UPolys, got "
                    + (obj == null ? "null" : obj.getClass().getName()));
            }
            return;
        }
        if (obj instanceof com.rost.cruma.core.unreal.UActor actor) {
            writeVolumeActorGeometry(j, a, actor);
            return;
        }
        // DefaultPhysicsVolume is not loaded as UActor in Cruma; tagged properties still ship.
        if (isVolumeActorClass(cls) || "Brush".equalsIgnoreCase(cls)) {
            writeVolumeFromTaggedProperties(j, a, loader.exportProperties(exp));
            return;
        }
        j.keyVal("geometry_error", "expected volume UActor, got "
            + (obj == null ? "null" : obj.getClass().getName()));
    }

    static void writeVolumeFromTaggedProperties(Json j, UArchive a, java.util.List<UProperty> props) {
        if (props == null) return;
        UProperty loc = null, pivot = null, brush = null;
        for (UProperty p : props) {
            String n = p.getName();
            if ("Location".equals(n)) loc = p;
            else if ("PrePivot".equals(n)) pivot = p;
            else if ("Brush".equals(n)) brush = p;
        }
        j.key("volume");
        j.startObj();
        if (loc != null && loc.getVectorValue() != null) writeVec(j, "location", loc.getVectorValue());
        if (pivot != null && pivot.getVectorValue() != null) writeVec(j, "prepivot", pivot.getVectorValue());
        if (brush != null) {
            j.keyVal("brush_index", brush.getIndexValue());
            j.keyVal("brush_ref", resolve(a, brush.getIndexValue()));
        }
        j.endObj();
    }

    static void writeModelGeometry(Json j, UArchive a, com.rost.cruma.core.unreal.Bsp.UModel model) {
        j.key("model");
        j.startObj();
        writeBox(j, "bounding_box", model.getBoundingBox());
        writeSphere(j, "bounding_sphere", model.getBoundingSphere());
        writeVecArray(j, "points", model.getPoints());
        writeVecArray(j, "vectors", model.getVectors());
        if (model.getBounds() != null) {
            j.key("bounds");
            j.startArr();
            for (var box : model.getBounds()) {
                j.comma();
                j.nl();
                writeBoxValue(j, box);
            }
            j.endArr();
        }
        var pref = model.getPolys();
        if (pref != null && !pref.isNull()) {
            j.keyVal("polys_index", pref.getIndex());
            j.keyVal("polys_ref", resolve(a, pref.getIndex()));
            try {
                var polys = pref.get();
                if (polys != null) writePolys(j, a, "polys", polys);
            } catch (Throwable t) {
                j.keyVal("polys_error", String.valueOf(t));
            }
        }
        j.endObj();
    }

    static void writeVolumeActorGeometry(Json j, UArchive a, com.rost.cruma.core.unreal.UActor actor) {
        j.key("volume");
        j.startObj();
        writeVec(j, "location", actor.getLocation());
        writeVec(j, "prepivot", actor.getPrePivot());
        writeVec(j, "scale", actor.getScale());
        writeVec(j, "draw_scale_3d", actor.getDrawScale3d());
        writeRot(j, "rotation", actor.getRotation());
        if (actor instanceof com.rost.cruma.core.unreal.UActor.BrushActor brushActor) {
            var bref = brushActor.getBrush();
            if (bref != null && !bref.isNull()) {
                j.keyVal("brush_index", bref.getIndex());
                j.keyVal("brush_ref", resolve(a, bref.getIndex()));
                try {
                    var model = bref.get();
                    if (model != null) writeModelGeometry(j, a, model);
                } catch (Throwable t) {
                    j.keyVal("brush_error", String.valueOf(t));
                }
            }
        }
        if (actor instanceof com.rost.cruma.core.unreal.UActor.MusicVolumeActor music) {
            j.keyVal("music_id", music.getMusicId());
            j.keyVal("has_night_music", music.isHasNightMusic());
            j.keyVal("night_music_id", music.getNightMusicId());
            j.keyVal("loop_music", music.isLoopMusic());
            j.keyVal("force_play_music", music.isForcePlayMusic());
            var states = music.getZoneMusicStates();
            if (states != null && !states.isEmpty()) {
                j.key("zone_music_states");
                j.startArr();
                for (var st : states) {
                    j.comma();
                    j.nl();
                    j.startObj();
                    j.keyVal("state", st.state());
                    j.keyVal("music_id", st.musicId());
                    j.endObj();
                }
                j.endArr();
            }
        }
        j.endObj();
    }

    static void writePolys(Json j, UArchive a, String key, com.rost.cruma.core.unreal.UPolys polys) {
        var list = polys.getPolygons();
        j.key(key);
        j.startArr();
        if (list != null) {
            for (var poly : list) {
                j.comma();
                j.nl();
                j.startObj();
                writeVec(j, "base", poly.base());
                writeVec(j, "normal", poly.normal());
                writeVec(j, "texture_u", poly.textureU());
                writeVec(j, "texture_v", poly.textureV());
                if (poly.vertices() != null) {
                    j.key("vertices");
                    j.startArr();
                    for (var v : poly.vertices()) {
                        j.comma();
                        j.nl();
                        writeVecValue(j, v);
                    }
                    j.endArr();
                }
                j.keyVal("flags", poly.flags());
                if (poly.itemName() != null && !poly.itemName().isEmpty())
                    j.keyVal("item_name", poly.itemName());
                j.keyVal("link", poly.link());
                j.keyVal("brush_polygon_index", poly.brushPolygonIndex());
                j.endObj();
            }
        }
        j.endArr();
    }

    static void writeBox(Json j, String key, com.rost.cruma.core.unreal.primitives.UBox box) {
        j.key(key);
        writeBoxValue(j, box);
    }

    static void writeBoxValue(Json j, com.rost.cruma.core.unreal.primitives.UBox box) {
        if (box == null) { j.valNull(); return; }
        j.startObj();
        writeVec(j, "min", box.min());
        writeVec(j, "max", box.max());
        j.keyVal("valid", box.isValid());
        j.endObj();
    }

    static void writeSphere(Json j, String key, com.rost.cruma.core.unreal.primitives.USphere sphere) {
        j.key(key);
        if (sphere == null) { j.valNull(); return; }
        j.startObj();
        writeVec(j, "location", sphere.location());
        j.keyVal("radius", sphere.radius());
        j.endObj();
    }

    static void writeVecArray(Json j, String key, glm_.vec3.Vec3[] arr) {
        if (arr == null) return;
        j.key(key);
        j.startArr();
        for (var v : arr) {
            j.comma();
            j.nl();
            writeVecValue(j, v);
        }
        j.endArr();
    }

    static void writeVecValue(Json j, glm_.vec3.Vec3 v) {
        if (v == null) { j.valNull(); return; }
        j.startObj();
        j.keyVal("x", v.getX());
        j.keyVal("y", v.getY());
        j.keyVal("z", v.getZ());
        j.endObj();
    }


    static void writeMeshAnimationSequences(Json j, UArchive a, UObjectExport exp) {
        MeshAnimationNotifyReader.Animation anim = MeshAnimationNotifyReader.read(a, exp);
        j.keyVal("mesh_animation_version", anim.version);
        j.keyVal("sequence_count", anim.sequences.size());
        j.keyVal("bone_count", (long) anim.boneCount);
        j.key("sequences");
        j.startArr();
        for (var seq : anim.sequences) {
            j.comma();
            j.nl();
            j.startObj();
            j.keyVal("name", seq.name);
            j.keyVal("start_frame", (long) seq.startFrame);
            j.keyVal("frame_count", (long) seq.frameCount);
            j.keyVal("rate", seq.rate);
            j.keyVal("shot_time", seq.shotTime);
            j.keyVal("pre_shot_time", seq.preShotTime);
            writeFloatList(j, "shot_times", seq.shotTimes);
            writeFloatList(j, "pre_shot_times", seq.preShotTimes);
            writeFloatList(j, "channel_times", seq.channelTimes);
            j.key("notifies");
            j.startArr();
            for (var n : seq.notifies) {
                j.comma();
                j.nl();
                j.startObj();
                j.keyVal("time", n.time);
                if (n.function != null && !n.function.isEmpty() && !"None".equals(n.function))
                    j.keyVal("function", n.function);
                if (!n.notifyClass.isEmpty()) j.keyVal("class", n.notifyClass);
                if (!n.notifyName.isEmpty()) j.keyVal("name", n.notifyName);
                if (!n.objectRef.isEmpty() && !"none".equals(n.objectRef))
                    j.keyVal("object", n.objectRef);
                j.endObj();
            }
            j.endArr();
            // Keyframe tracks omitted (binary motion, not properties).
            j.endObj();
        }
        j.endArr();
    }

    static void writeFloatList(Json j, String key, java.util.List<Float> list) {
        if (list == null || list.isEmpty()) return;
        j.key(key);
        j.startArr();
        for (int i = 0; i < list.size(); i++) {
            j.comma();
            j.nl();
            j.val((double) list.get(i));
        }
        j.endArr();
    }

    static void writeFloatList(Json j, String key, org.eclipse.collections.api.list.primitive.ImmutableFloatList list) {
        if (list == null || list.isEmpty()) return;
        j.key(key);
        j.startArr();
        for (int i = 0; i < list.size(); i++) {
            j.comma();
            j.nl();
            j.val((double) list.get(i));
        }
        j.endArr();
    }

    static void writeImport(Json j, UArchive a, int i, UObjectImport imp) {
        j.comma();
        j.nl();
        j.startObj();
        j.keyVal("import_index", i);
        j.keyVal("object_name", imp.getObjectName());
        j.keyVal("class_name", imp.getClassName());
        j.keyVal("class_package", imp.getClassPackage());
        j.keyVal("package_index", imp.getPackageIndex());
        j.keyVal("package", resolve(a, imp.getPackageIndex()));
        j.endObj();
    }

    static UArchive load(UArchiveLoader loader, String name) {
        UArchive a = loader.loadArchiveIfPresent(name);
        if (a == null && name != null) {
            a = loader.loadArchiveIfPresent(name.toLowerCase(Locale.ROOT));
        }
        if (a == null && name != null && !name.isEmpty()) {
            a = loader.loadArchiveIfPresent(Character.toUpperCase(name.charAt(0)) + name.substring(1));
        }
        return a;
    }

    static void collectRefs(UArchive a, List<UProperty> props, Map<String, Set<String>> needed) {
        for (UProperty p : props) {
            if (p.getType() != null && "Object".equals(p.getType().name())) {
                rememberImport(a, p.getIndexValue(), needed);
            }
            if (p.getObjectArray() != null) {
                for (int idx : p.getObjectArray()) rememberImport(a, idx, needed);
            }
            if (p.getSubProperties() != null) {
                for (var map : p.getSubProperties()) collectRefs(a, new ArrayList<>(map.values()), needed);
            }
        }
    }

    static void rememberImport(UArchive a, int index, Map<String, Set<String>> needed) {
        if (index >= 0) return;
        String cls = importClassName(a, index);
        String pkg = importClassPackage(a, index);
        if (cls == null || cls.isEmpty()) return;
        if (pkg == null || pkg.isEmpty() || "Core".equalsIgnoreCase(pkg) || "Engine".equalsIgnoreCase(pkg)) {
            // still try lineageeffect for EffectClass imports (class package is often Core, outer is lineageeffect)
            UObjectImport imp = a.getImportMap().get(-index - 1);
            int walk = imp.getPackageIndex();
            String outer = null;
            int guard = 0;
            while (walk < 0 && (-walk) <= a.getImportMap().size() && guard++ < 8) {
                UObjectImport o = a.getImportMap().get(-walk - 1);
                outer = o.getObjectName();
                if ("Class".equalsIgnoreCase(o.getClassName()) || "Package".equalsIgnoreCase(o.getClassName())
                    || "Core".equalsIgnoreCase(outer) || "Engine".equalsIgnoreCase(outer)) {
                    walk = o.getPackageIndex();
                    if ("Package".equalsIgnoreCase(o.getClassName()) && !"Core".equalsIgnoreCase(outer) && !"Engine".equalsIgnoreCase(outer)) {
                        pkg = outer;
                        break;
                    }
                    continue;
                }
                walk = o.getPackageIndex();
            }
            if (pkg == null || "Core".equalsIgnoreCase(pkg) || "Engine".equalsIgnoreCase(pkg)) {
                if (outer != null && !"Core".equalsIgnoreCase(outer) && !"Engine".equalsIgnoreCase(outer)) pkg = outer;
            }
        }
        if (pkg == null || pkg.isEmpty()) pkg = "lineageeffect";
        needed.computeIfAbsent(pkg, k -> new LinkedHashSet<>()).add(cls);
    }

    static void dumpArchive(Path out, String logicalName, String sourceHint, UArchive a) throws IOException {
        dumpArchive(out, logicalName, sourceHint, a, false);
    }

    static void dumpArchive(Path out, String logicalName, String sourceHint, UArchive a, boolean compact) throws IOException {
        Files.createDirectories(out.getParent());
        Json j = new Json(compact);
        j.startObj();
        j.keyVal("package", a.getName() == null ? logicalName : a.getName());
        j.keyVal("source", sourceHint);
        j.keyVal("tool", "tools/interlude-extract/ExportPackageProperties.java");
        j.keyVal("export_count", a.getExportMap().size());
        j.keyVal("import_count", a.getImportMap().size());
        j.key("imports");
        j.startArr();
        List<UObjectImport> imports = a.getImportMap();
        for (int i = 0; i < imports.size(); i++) writeImport(j, a, i, imports.get(i));
        j.endArr();
        j.key("exports");
        j.startArr();
        List<UObjectExport> exports = a.getExportMap();
        int failed = 0;
        for (int i = 0; i < exports.size(); i++) {
            UObjectExport exp = exports.get(i);
            List<UProperty> props;
            String[] source = new String[] { "tagged_properties" };
            try {
                props = extractSmart(a, exp, source);
            } catch (Throwable t) {
                failed++;
                j.comma();
                j.nl();
                j.startObj();
                j.keyVal("export_index", i);
                j.keyVal("class", exp.getClassName());
                j.keyVal("name", exp.getObjectName());
                j.keyVal("serial_size", exp.getSerialSize());
                j.keyVal("extract_error", String.valueOf(t));
                j.endObj();
                continue;
            }
            writeExport(j, a, i, exp, props, source[0]);
        }
        j.endArr();
        j.keyVal("extract_failures", failed);
        j.endObj();
        j.sb.append('\n');
        Files.writeString(out, j.toString(), StandardCharsets.UTF_8);
        System.err.printf(Locale.ROOT, "Wrote %s exports=%d imports=%d failures=%d -> %s (%d bytes)%n",
            a.getName(), a.getExportMap().size(), a.getImportMap().size(), failed, out, Files.size(out));
    }

    public static void main(String[] args) throws Exception {
        if (args.length < 2) {
            System.err.println("Usage: ExportPackageProperties <client-root> <out-dir>");
            System.exit(2);
        }
        Path root = Path.of(args[0]).toAbsolutePath();
        Path outDir = Path.of(args[1]).toAbsolutePath();
        Files.createDirectories(outDir);

        var entries = ClientSearchPaths.read(root);
        if (entries == null || entries.isEmpty()) entries = ClientSearchPaths.DEFAULT;
        List<USearchConfig> configs = new ArrayList<>();
        for (var e : entries) configs.add(new USearchConfig(e.directory(), e.extension()));
        var loader = new UArchiveLoader(root, configs);

        UArchive skill = load(loader, SkillEffects.VISUAL_PACKAGE);
        if (skill == null) {
            System.err.println("Failed to load Animations/skill.usk under " + root);
            System.exit(1);
        }
        Path skillOut = outDir.resolve("skill.usk.json");
        dumpArchive(skillOut, "skill", "Animations/skill.usk", skill);

        Map<String, Set<String>> needed = new LinkedHashMap<>();
        for (UObjectExport exp : skill.getExportMap()) {
            try {
                collectRefs(skill, extract(skill, exp), needed);
            } catch (Throwable ignored) {
            }
        }

        Path refsOut = outDir.resolve("skill.usk.references.json");
        Json rj = new Json();
        rj.startObj();
        rj.keyVal("from", "skill.usk");
        rj.key("packages");
        rj.startObj();
        for (var e : needed.entrySet()) {
            rj.key(e.getKey());
            rj.startArr();
            for (String c : e.getValue()) { rj.comma(); rj.nl(); rj.val(c); }
            rj.endArr();
        }
        rj.endObj();
        rj.endObj();
        rj.sb.append('\n');
        Files.writeString(refsOut, rj.toString(), StandardCharsets.UTF_8);

        // Always dump the full lineageeffect package (referenced emitters + class defaults).
        UArchive le = load(loader, "lineageeffect");
        if (le == null) le = load(loader, "LineageEffect");
        if (le == null) {
            System.err.println("WARNING: lineageeffect.u was not loaded; referenced effect class defaults are missing.");
        } else {
            dumpArchive(outDir.resolve("lineageeffect.u.json"), "lineageeffect", "system/lineageeffect.u", le);
        }

        // Dump any other referenced packages that are not engine/core (except skill itself).
        for (String pkg : needed.keySet()) {
            if (pkg == null) continue;
            String low = pkg.toLowerCase(Locale.ROOT);
            if (low.equals("skill") || low.equals("lineageeffect") || low.equals("core") || low.equals("engine")
                || low.equals("class") || low.equals("object") || low.equals("enum")) continue;
            if (Files.exists(outDir.resolve(pkg + ".json"))) continue;
            UArchive extra = load(loader, pkg);
            if (extra == null) {
                System.err.println("WARNING: referenced package not loaded: " + pkg);
                continue;
            }
            dumpArchive(outDir.resolve(pkg + ".u.json"), pkg, "client package " + pkg, extra);
        }

        System.err.println("Done. Packages referenced from skill.usk: " + needed.keySet());
    }
}
