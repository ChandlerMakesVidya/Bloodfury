using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum weaponType
{
    Melee, Projectile, Custom, Hitscan, HitscanSlowMoProjectile
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int weaponID;
    public weaponType type;
    public GameObject worldPrefab;
    [Space]

    float baseDamage;
    float attackCooldown;
    float switchDowntime;

    float reach;

    GameObject projectile;
    int bulletsPerShot;
    int ammo;
    int reserve;
    float spread;
    bool automatic;

    #region Editor
#if UNITY_EDITOR

    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Weapon weapon = (Weapon)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Common Properties", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Damage", GUILayout.MaxWidth(80));
            weapon.baseDamage = EditorGUILayout.FloatField(weapon.baseDamage);

            EditorGUILayout.LabelField("Attack Cooldown", GUILayout.MaxWidth(100));
            weapon.attackCooldown = EditorGUILayout.FloatField(weapon.attackCooldown);

            EditorGUILayout.LabelField("Switch Downtime", GUILayout.MaxWidth(100));
            weapon.switchDowntime = EditorGUILayout.FloatField(weapon.switchDowntime);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Type Properties", EditorStyles.boldLabel);
            switch (weapon.type)
            {
                case weaponType.Melee:
                    weapon.reach = EditorGUILayout.FloatField("Reach", weapon.reach);
                    break;
                case weaponType.HitscanSlowMoProjectile:
                case weaponType.Projectile:
                    weapon.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", weapon.projectile, typeof(GameObject), true);
                    weapon.bulletsPerShot = EditorGUILayout.IntField("Bullets Per Shot", weapon.bulletsPerShot);
                    weapon.ammo = EditorGUILayout.IntField("Ammo", weapon.ammo);
                    weapon.reserve = EditorGUILayout.IntField("Reserve", weapon.reserve);
                    weapon.spread = EditorGUILayout.FloatField("Spread", weapon.spread);
                    weapon.automatic = EditorGUILayout.Toggle("Automatic", weapon.automatic);
                    break;
                case weaponType.Hitscan:
                    weapon.bulletsPerShot = EditorGUILayout.IntField("Bullets Per Shot", weapon.bulletsPerShot);
                    weapon.ammo = EditorGUILayout.IntField("Ammo", weapon.ammo);
                    weapon.reserve = EditorGUILayout.IntField("Reserve", weapon.reserve);
                    weapon.spread = EditorGUILayout.FloatField("Spread", weapon.spread);
                    weapon.automatic = EditorGUILayout.Toggle("Automatic", weapon.automatic);
                    break;
                case weaponType.Custom:
                    EditorGUILayout.LabelField("Put custom weapon behavior in the PlayerLoadout class, make appropriate checks for the selected weapon based on Weapon ID.", EditorStyles.textArea);
                    break;
                default:
                    break;
            }
        }
    }
#endif
    #endregion
}
