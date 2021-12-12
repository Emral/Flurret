using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ProjectileType
{
    P_Thin, P_Long, P_Beam,

    E_Small, E_Big, E_Sine
}

public enum PowerupStage { Off, Low, Mid, High, Ultra }

public enum EffectType
{
    EnemyImpact, PlayerImpact,

    EnemyDeath, PlayerDeath
}

[System.Serializable]
public class ProjectileReference
{
    public ProjectileType type;
    public Projectile prefab;
}

[System.Serializable]
public class EffectReference
{
    public EffectType type;
    public ParticleSystem prefab;
}

[System.Serializable]
public class TrainPieceReference
{
    public char type;
    public Entity prefab;
}

[System.Serializable]
public class WaveConfig
{
    public string sceneName;
    public string flavourName;
    public Color backgroundTint;
    public int countIncreaseAfterBeat;
    public float hpIncreaseAfterBeat;
    public float addToScoreMultiplierAfterBeat;
}

[CreateAssetMenu(menuName = "Create References")]
public class References : ScriptableObject
{
    public List<WaveConfig> waves;
    public List<string> waveRepeatNamePrefix;
    public List<string> waveRepeatNameSuffix;

    public List<ProjectileReference> projectileMap;
    public List<EffectReference> effectMap;
    public List<SoundEffect> sounds;
    public TextEffect scorePointsText;

    public Projectile GetProjectile(ProjectileType type)
    {
        return projectileMap.First(t => t.type == type).prefab;
    }

    public ParticleSystem GetEffect(EffectType type)
    {
        return effectMap.First(t => t.type == type).prefab;
    }

    public Color colorGrey;
    public Color colorOrange;

    public List<TrainPieceReference> trainEntityPrefabs;

    public List<Collectible> collectibles;

    public Entity GetTrainPiece(char type)
    {
        return trainEntityPrefabs.First(t => t.type == type).prefab;
    }
}