using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UI._ConsoleUI;
using Kingmaker.View;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;

namespace RNGTest
{
#if DEBUG
    [EnableReloading]
#endif
    public class Main
    {
        public static uint NumberOfTests = 1000000;
        static UnityModManager.ModEntry ModEntry;
        static void Error(string text)
        {
            ModEntry?.Logger.Error(text);
        }
        static void Error(Exception ex)
        {
            ModEntry?.Logger.Error(ex.ToString());
        }
        static void Log(string text)
        {
            ModEntry?.Logger.Log(text);
        }
        static void Load(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            ModEntry.OnGUI = OnGUI;
            ModEntry.OnUpdate = OnUpdate;
#if DEBUG
            modEntry.OnUnload = Unload;
#endif
        }
#if DEBUG
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            return true;
        }
#endif
        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
            {
                RunTest();
            }
        }
        public static UnitEntityData GetUnitUnderMouse()
        {
            if (Game.Instance.IsControllerMouse)
            {
                Game instance = Game.Instance;
                UnitEntityView unitEntityView = (instance != null) ? instance.UI.SelectionManagerPC.HoverUnit : null;
                if (unitEntityView != null)
                {
                    return unitEntityView.EntityData;
                }
            }
            Vector2 v = Game.Instance.IsControllerGamepad ? RewiredCursorController.CursorPos : new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Camera camera = Game.GetCamera();
            foreach (RaycastHit raycastHit in Physics.RaycastAll(camera.ScreenPointToRay(v), camera.farClipPlane, 2118913))
            {
                GameObject gameObject = raycastHit.collider.gameObject;
                if (gameObject.CompareTag("SecondarySelection"))
                {
                    while (!gameObject.GetComponent<UnitEntityView>() && gameObject.transform.parent)
                    {
                        gameObject = gameObject.transform.parent.gameObject;
                    }
                }
                UnitEntityView component = gameObject.GetComponent<UnitEntityView>();
                if (component)
                {
                    return component.EntityData;
                }
            }
            return null;
        }
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Number of rolls per test");
            if (uint.TryParse(GUILayout.TextField(NumberOfTests.ToString()), out var result))
            {
                NumberOfTests = result;
            };
            GUILayout.EndHorizontal();
            GUILayout.Label($"To run a test hover over a NPC with the mouse and press Shift+T. Results will be written to {modEntry.Path}");
            UnitEntityData unit = GetUnitUnderMouse();
            GUILayout.Label($"Selected NPC: {unit?.CharacterName ?? "NULL"}");
        }

        static void RunTest()
        {
            using (new DisableCombatText())
            {
                var initiator = Game.Instance.Player.MainCharacter.Value;
                var npc = GetUnitUnderMouse();
                if (npc == null)
                {
                    Error($"No NPC selected");
                    return;
                }
                if (initiator.GetFirstWeapon() == null)
                {
                    Error($"{initiator.CharacterName} has no weapon");
                    return;
                }
                if (npc.GetFirstWeapon() == null)
                {
                    Error($"{npc.CharacterName} has no weapon");
                    return;
                }
                RunTest("Player", initiator, npc);
                RunTest("NPC", npc, initiator);
            }

        }
        static void RunTest(string name, UnitEntityData initiator, UnitEntityData target)
        {
            var weapon = initiator.GetFirstWeapon();
            var rolls = new ushort[NumberOfTests];
            var resultBuckets = new uint[Enum.GetNames(typeof(AttackResult)).Length];
            var weaponStats = new RuleCalculateWeaponStats(initiator, weapon, null);
            Rulebook.Trigger(weaponStats);
            using (var sw = new StreamWriter($"{ModEntry.Path}/{name}_rolls.txt"))
            {
                for (int i = 0; i < NumberOfTests; i++)
                {
                    var rule = new RuleAttackRoll(initiator, target, weaponStats, 0);
                    rule.SuspendCombatLog = true;
                    var roll = Rulebook.Trigger(rule);
                    if (roll.Roll.Value > 20 || roll.Roll.Value < 1)
                    {
                        Error("Roll out of range");
                        return;
                    }
                    rolls[i] = (ushort)roll.Roll.Value;
                    resultBuckets[(int)roll.Result] += 1;
                    sw.WriteLine("Roll: {0} Result: {1}", roll.Roll.Value, roll.Result);
                }
            }
            var buckets = new ulong[20];
            ulong sum = 0;
            var max1Seq = new SequenceCounter(SequenceType.LessThen, 2);
            var max20Seq = new SequenceCounter(SequenceType.GreaterThen, 19);
            var maxHighSeq = new SequenceCounter(SequenceType.GreaterThen, 13);
            var maxLowSeq = new SequenceCounter(SequenceType.LessThen, 8);
            foreach (var roll in rolls)
            {
                buckets[roll - 1] += 1;
                var prevSum = sum;
                sum += roll;
                if(sum < prevSum)
                {
                    Error("Overflow while calculating sum");
                    break;
                }
                max1Seq.Add(roll);
                max20Seq.Add(roll);
                maxHighSeq.Add(roll);
                maxLowSeq.Add(roll);
            }
            var maxBucket = buckets.Max();
            var minBucket = buckets.Min();
            var bucketDifference = maxBucket - minBucket;
            var average = sum / (double)NumberOfTests;
            using (var sw = new StreamWriter($"{ModEntry.Path}/{name}_summary.txt"))
            {
                sw.WriteLine("Initiator: {0}", initiator.CharacterName);
                sw.WriteLine("Target: {0}", target.CharacterName);
                sw.WriteLine("Weapon: {0}", weapon.Name);
                sw.WriteLine("Number of rolls: {0}", NumberOfTests);
                sw.WriteLine("Sum: {0}", sum);
                sw.WriteLine("Average: {0}", average);
                for(int i = 0; i < 20; i++)
                {
                    sw.WriteLine("Number of {0}: {1}", i + 1, buckets[i]);
                }
                sw.WriteLine("Highest count in set {0}", maxBucket);
                sw.WriteLine("Lowest count in set {0}", minBucket);
                sw.WriteLine("Difference {0}", bucketDifference);
                sw.WriteLine("Max 1 in a row: {0}", max1Seq.MaxLength);
                sw.WriteLine("Max 20 in a row: {0}", max20Seq.MaxLength);
                sw.WriteLine("Max > 13 in a row: {0}", maxHighSeq.MaxLength);
                sw.WriteLine("Max < 8 in a row: {0}", maxLowSeq.MaxLength);
                var resultNames = Enum.GetNames(typeof(AttackResult));
                for (int i =0; i < resultNames.Length; i++)
                {
                    sw.WriteLine("{0} count: {1} ({2}%)", resultNames[i], resultBuckets[i], resultBuckets[i] / (float)NumberOfTests * 100f);
                }

                var rule = new RuleAttackRoll(initiator, target, weaponStats, 0);
                rule.SuspendCombatLog = true;
                var roll = Rulebook.Trigger(rule);
                sw.WriteLine("AttackBonus: {0}", roll.AttackBonus);
                sw.WriteLine("IsTargetFlatFooted: {0}", roll.IsTargetFlatFooted);
                sw.WriteLine("TargetAC: {0}", roll.TargetAC);
                sw.WriteLine("IsSneakAttack: {0}", roll.IsSneakAttack);
                sw.WriteLine("Target.IsFlanked: {0}", roll.Target.CombatState.IsFlanked);
                sw.WriteLine("Weapon.CriticalEdge: {0}", roll.WeaponStats.CriticalEdge);
                sw.WriteLine("ImmuneToCriticalHit: {0}", roll.ImmuneToCriticalHit);
                sw.WriteLine("ImmuneToSneakAttack: {0}", roll.ImmuneToSneakAttack);
                sw.WriteLine("TargetUseFortification: {0}", roll.TargetUseFortification);
            }
            var imageSize = (int)Math.Sqrt(NumberOfTests);
            if (imageSize > 2800) imageSize = 2800;
            if (imageSize > 0 && imageSize <= 2800) {
                var texture = new Texture2D(imageSize, imageSize);
                int pixelIndex = 0;
                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        texture.SetPixel(x, y, rolls[pixelIndex++] > 10 ? Color.white : Color.black);
                    }
                }
                var data = texture.EncodeToPNG();
                File.WriteAllBytes($"{ModEntry.Path}/{name}_image.png", data);
            }
        }
    }
}
