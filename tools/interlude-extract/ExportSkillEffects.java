import com.rost.cruma.core.unreal.SkillEffects;
import com.rost.cruma.core.unreal.SkillEffects.Action;
import com.rost.cruma.core.unreal.SkillEffects.Phase;
import com.rost.cruma.core.unreal.SkillEffects.Skill;
import com.rost.cruma.core.unreal.UArchive;
import com.rost.cruma.core.unreal.UArchiveLoader;
import com.rost.cruma.core.unreal.UArchiveLoader.USearchConfig;
import java.io.BufferedWriter;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.Locale;

public final class ExportSkillEffects {
    private static String attachName(int attachOn) {
        return switch (attachOn) {
            case SkillEffects.AM_NONE -> "none";
            case SkillEffects.AM_LOCATION -> "location";
            case SkillEffects.AM_RH -> "rh";
            case SkillEffects.AM_LH -> "lh";
            case SkillEffects.AM_RA -> "ra";
            case SkillEffects.AM_LA -> "la";
            case SkillEffects.AM_WING -> "wing";
            case SkillEffects.AM_BONE_SPECIFIED -> "bone";
            case SkillEffects.AM_ALIAS_SPECIFIED -> "alias";
            case SkillEffects.AM_TRAIL -> "trail";
            case SkillEffects.AM_AGATHION -> "agathion";
            case SkillEffects.AM_RIGHT_FOOT -> "right_foot";
            case SkillEffects.AM_LEFT_FOOT -> "left_foot";
            default -> Integer.toString(attachOn);
        };
    }

    private static String esc(String s) {
        if (s == null) return "";
        return s.replace("\\", "\\\\").replace("\t", "\\t").replace("\n", "\\n").replace("\r", "");
    }

    private static String fmt(float v) {
        if (v == (int) v) return Integer.toString((int) v);
        return String.format(Locale.ROOT, "%.6f", v).replaceFirst("0+$", "").replaceFirst("\\.$", ".0");
    }

    public static void main(String[] args) throws Exception {
        if (args.length < 2) {
            System.err.println("Usage: ExportSkillEffects <client-root> <out.tsv>");
            System.exit(2);
        }
        Path root = Path.of(args[0]).toAbsolutePath();
        Path out = Path.of(args[1]).toAbsolutePath();
        var loader = new UArchiveLoader(root, List.of(new USearchConfig("Animations", "usk")));
        UArchive archive = loader.loadArchiveIfPresent(SkillEffects.VISUAL_PACKAGE);
        if (archive == null) {
            System.err.println("Failed to load Animations/skill.usk under " + root);
            System.exit(1);
        }
        List<Skill> skills = SkillEffects.listVisuals(archive);
        Files.createDirectories(out.getParent());
        int actionRows = 0;
        int skillsWithActions = 0;
        try (BufferedWriter w = Files.newBufferedWriter(out, StandardCharsets.UTF_8)) {
            w.write("skill_id\tclass_name\tflying_time\tphase\teffect\tattach_on\tattach_name\tbone\ton_target\ton_multi_target\trelative_to_cylinder\toffset_x\toffset_y\toffset_z\tdelay\tabsolute\n");
            for (Skill skill : skills) {
                boolean any = false;
                for (Phase phase : Phase.values()) {
                    for (Action action : skill.actions(phase)) {
                        any = true;
                        actionRows++;
                        var off = action.offset();
                        w.write(
                                skill.skillId()
                                        + "\t" + esc(skill.getFullName())
                                        + "\t" + fmt(skill.flyingTime())
                                        + "\t" + phase.name().toLowerCase(Locale.ROOT)
                                        + "\t" + esc(action.effect())
                                        + "\t" + action.attachOn()
                                        + "\t" + attachName(action.attachOn())
                                        + "\t" + esc(action.bone())
                                        + "\t" + (action.onTarget() ? "1" : "0")
                                        + "\t" + (action.onMultiTarget() ? "1" : "0")
                                        + "\t" + (action.relativeToCylinder() ? "1" : "0")
                                        + "\t" + fmt(off.getX())
                                        + "\t" + fmt(off.getY())
                                        + "\t" + fmt(off.getZ())
                                        + "\t" + fmt(action.delay())
                                        + "\t" + (action.absolute() ? "1" : "0")
                                        + "\n");
                    }
                }
                if (!any) {
                    w.write(
                            skill.skillId()
                                    + "\t" + esc(skill.getFullName())
                                    + "\t" + fmt(skill.flyingTime())
                                    + "\t\t\t\t\t\t\t\t\t\t\t\t\t\n");
                } else {
                    skillsWithActions++;
                }
            }
        }
        System.err.printf(
                "Wrote %d skills (%d with actions, %d action rows) to %s%n",
                skills.size(), skillsWithActions, actionRows, out);
    }
}
