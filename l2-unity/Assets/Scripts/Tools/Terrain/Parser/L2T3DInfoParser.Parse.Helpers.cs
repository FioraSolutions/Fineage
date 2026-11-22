#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class L2T3DInfoParser
{
    public static List<L2InterpolationPoint> ParseCameraInfo(string t3dPath) {
        if (terrainInfo == null) {
            terrainInfo = new L2TerrainInfo();
        }

        terrainInfo.interpolationPoints = new List<L2InterpolationPoint>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin Actor Class=InterpolationPoint Name=")) {
                    L2InterpolationPoint interpolationPoint = new L2InterpolationPoint();
                    interpolationPoint.name = line.Replace("Begin Actor Class=InterpolationPoint Name=", "");
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();
                        
                        if (line.StartsWith("Location=")) {
                            interpolationPoint.position = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Rotation=")) {
                            interpolationPoint.eulerAngles = L2MetaDataUtils.ParseRotation(line);
                        }
                    }

                    terrainInfo.interpolationPoints.Add(interpolationPoint);

                }
            }
        }

        // Debug.Log($"Loaded {terrainInfo.interpolationPoints.Count} interpolation points.");

        return terrainInfo.interpolationPoints;
    }

    /// <summary>
    /// UE2 <c>Class=Camera</c> actors (Lobby.unr viewport cameras). Unlike
    /// InterpolationPoint these carry Tag (U2Viewport*) and optionally FovAngle.
    /// Rotation is raw UE2 pitch/yaw/roll, same convention as ParseCameraInfo.
    /// </summary>
    public static List<L2T3DCameraInfo> ParseLobbyCameras(string t3dPath) {
        List<L2T3DCameraInfo> cameras = new List<L2T3DCameraInfo>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (!line.StartsWith("Begin Actor Class=Camera Name=")) {
                    continue;
                }

                L2T3DCameraInfo camera = new L2T3DCameraInfo();
                camera.name = line.Replace("Begin Actor Class=Camera Name=", "");
                while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                    line = line.Trim();

                    if (line.StartsWith("Location=")) {
                        camera.position = L2MetaDataUtils.ParseVector3(line);
                    } else if (line.StartsWith("Rotation=")) {
                        camera.eulerAngles = L2MetaDataUtils.ParseRotation(line);
                    } else if (line.StartsWith("Tag=")) {
                        camera.tag = line.Replace("Tag=", "");
                    } else if (line.StartsWith("FovAngle=")) {
                        camera.fovAngleDeg = L2MetaDataUtils.ParseFloatFromInfo(line);
                    }
                }

                cameras.Add(camera);
            }
        }

        return cameras;
    }

    public static List<AmbientSound> ParseAmbientSounds(string t3dPath) {
        if (terrainInfo == null) {
            terrainInfo = new L2TerrainInfo();
        }

        terrainInfo.ambientSounds = new List<AmbientSound>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin LevelObject Class=AmbientSoundObject Name=")) {
                    AmbientSound ambientSound = new AmbientSound();
                    ambientSound.name = line.Replace("Begin LevelObject Class=AmbientSoundObject Name=", "");

                    while ((line = reader.ReadLine()) != null && !line.Contains("End LevelObject")) {
                        line = line.Trim();

                        if (line.StartsWith("AmbientRandom=")) {
                            ambientSound.ambientRandom = L2MetaDataUtils.ParseIntFromInfo(line);
                            // Debug.Log(ambientSound.ambientRandom);
                        } else if (line.StartsWith("AmbientSoundStartTime=")) {
                            ambientSound.ambientSoundStartTime = L2MetaDataUtils.ParseFloatFromInfo(line);
                        } else if (line.StartsWith("AmbientSound=")) {
                            string ambientSoundName = line.Replace("AmbientSound=Sound'", "").Replace("'", "");
                            ambientSound.ambientSoundName = ambientSoundName;
                        } else if (line.StartsWith("SoundRadius=")) {
                            ambientSound.soundRadius = L2MetaDataUtils.ParseFloatFromInfo(line);
                        } else if (line.StartsWith("SoundVolume=")) {
                            ambientSound.soundVolume = L2MetaDataUtils.ParseIntFromInfo(line);
                        } else if (line.StartsWith("SoundPitch=")) {
                            ambientSound.soundPitch = L2MetaDataUtils.ParseIntFromInfo(line);
                        } else if (line.StartsWith("Group=")) {
                            ambientSound.groupName = line.Replace("Group=", "").Replace("\"", "");
                        } else if (line.StartsWith("Location=")) {
                            ambientSound.position = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("AmbientSoundType=")) {
                            ambientSound.ambientSoundType = line.Replace("AmbientSoundType=", "");                   
                        }
                    }

                    if(ambientSound.ambientSoundName != null && ambientSound.ambientSoundName.Length > 0) {
                        terrainInfo.ambientSounds.Add(ambientSound);
                    }

                    if(ambientSound.ambientSoundType == null || ambientSound.ambientSoundType.Length == 0) {
                        ambientSound.ambientSoundType = "AST1_Always";
                    }
                }
            }
        }

        // Debug.Log($"Loaded {terrainInfo.ambientSounds.Count} ambientsound(s).");

        return terrainInfo.ambientSounds;
    }
}
#endif
