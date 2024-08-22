using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class BLM
    {
        public const byte ClassID = 7;
        public const byte JobID = 25;

        public const uint
            Fire = 141,
            Blizzard = 142,
            Thunder = 144,
            Fire2 = 147,
            Transpose = 149,
            Fire3 = 152,
            Thunder3 = 153,
            Blizzard3 = 154,
            AetherialManipulation = 155,
            Scathe = 156,
            Manafont = 158,
            Freeze = 159,
            Flare = 162,
            LeyLines = 3573,
            Sharpcast = 3574,
            Blizzard4 = 3576,
            Fire4 = 3577,
            BetweenTheLines = 7419,
            Thunder4 = 7420,
            Triplecast = 7421,
            Foul = 7422,
            Thunder2 = 7447,
            Despair = 16505,
            UmbralSoul = 16506,
            Xenoglossy = 16507,
            Blizzard2 = 25793,
            HighFire2 = 25794,
            HighBlizzard2 = 25795,
            Amplifier = 25796,
            Paradox = 25797,
            HighThunder = 36986,
            HighThunder2 = 36987,
            FlareStar = 36989;

        public static class Buffs
        {
            public const ushort
                Thundercloud = 164,
                Firestarter = 165,
                LeyLines = 737,
                CircleOfPower = 738,
                Sharpcast = 867,
                Triplecast = 1211,
                Thunderhead = 3870,
                EnhancedFlare = 2960;
        }

        public static class Debuffs
        {
            public const ushort
                Thunder = 161,
                Thunder2 = 162,
                Thunder3 = 163,
                Thunder4 = 1210,
                HighThunder = 3871,
                HighThunder2 = 3872; 
        }

        public static class Traits
        {
            public const uint
                EnhancedFreeze = 295,
                EnhancedPolyglot = 297,
                AspectMasteryIII = 459,
                EnhancedFoul = 461,
                EnhancedManafont = 463,
                Enochian = 460,
                EnhancedPolyglotII = 615;
        }

        public static class MP
        {
            public const int MaxMP = 10000;

            public const int AllMPSpells = 800; //"ALL MP" spell. Only caring about the absolute minimum.
            public static int ThunderST => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(Thunder));
            public static int ThunderAoE => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(Thunder2));
            public static int FireI => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(Fire));
            public static int FireAoE => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(Fire2));
            public static int FireIII => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(Fire3));
            public static int BlizzardAoE => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(Blizzard2));
            public static int BlizzardI => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(Blizzard));
            public static int Freeze => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(BLM.Freeze));
            public static int Despair => CustomComboFunctions.GetResourceCost(CustomComboFunctions.OriginalHook(BLM.Despair));
        }

        // Debuff Pairs of Actions and Debuff
        public static readonly Dictionary<uint, ushort>
            ThunderList = new()
            {
                { Thunder,  Debuffs.Thunder  },
                { Thunder2, Debuffs.Thunder2 },
                { Thunder3, Debuffs.Thunder3 },
                { Thunder4, Debuffs.Thunder4 },
                { HighThunder, Debuffs.HighThunder },
                { HighThunder2, Debuffs.HighThunder2 }
            };

        public static class Config
        {
            public static UserBool
                BLM_Adv_Xeno_Burst = new("BLM_Adv_Xeno_Burst");

            public static UserBoolArray
                BLM_Adv_Cooldowns_Choice = new("BLM_Adv_Cooldowns_Choice"),
                BLM_AoE_Adv_Cooldowns_Choice = new("BLM_AoE_Adv_Cooldowns_Choice"),
                BLM_Adv_Movement_Choice = new("BLM_Adv_Movement_Choice");

            public static UserInt
                BLM_VariantCure = new("BLM_VariantCure"),
                BLM_Adv_Cooldowns = new("BLM_Adv_Cooldowns"),
                BLM_Adv_Thunder = new("BLM_Adv_Thunder"),
                BLM_Adv_Rotation_Options = new("BLM_Adv_Rotation_Options"),
                BLM_Advanced_OpenerSelection = new("BLM_Advanced_OpenerSelection"),
                BLM_ST_Adv_ThunderHP = new("BLM_ST_Adv_ThunderHP"),
                BLM_AoE_Adv_ThunderHP = new("BLM_AoE_Adv_ThunderHP"),
                BLM_AoE_Adv_ThunderUptime = new("BLM_AoE_Adv_ThunderUptime"),
                BLM_Adv_ThunderCloud = new("BLM_Adv_ThunderCloud"),
                BLM_Adv_InitialCast = new("BLM_Adv_InitialCast");

            public static UserFloat
                BLM_AstralFire_Refresh = new("BLM_AstralFire_Refresh");
        }

        internal class BLM_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_SimpleMode;
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == Fire)
                {
                    var gauge = GetJobGauge<BLMGauge>();
                    var maxPolyglot = TraitLevelChecked(Traits.EnhancedPolyglotII) ? 3 : TraitLevelChecked(Traits.EnhancedPolyglot) ? 2 : 1;
                    var maxPolyglotCD = maxPolyglot * 30000;
                    var remainingPolyglotCD = Math.Max(0, (maxPolyglot - gauge.PolyglotStacks) * 30000 + (gauge.EnochianTimer - 30000));
                    var curMp = LocalPlayer.CurrentMp;
                    int nextMpGain = gauge.UmbralIceStacks switch
                    {
                        0 => 0,
                        1 => 2500,
                        2 => 5000,
                        3 => 10000,
                        _ => 0
                    };
                    var thunderDebuff = FindEffectOnMember(ThunderList[OriginalHook(Thunder)], CurrentTarget);
                    var elementTimer = gauge.ElementTimeRemaining / 1000f;
                    var gcdsInTimer = Math.Floor(elementTimer / GetActionCastTime(Fire));
                    var canSwiftB3 = IsOffCooldown(All.Swiftcast) || ActionReady(Triplecast) || GetBuffStacks(Buffs.Triplecast) > 0;

                    if (HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1)
                    {
                        if (thunderDebuff is null || thunderDebuff.RemainingTime < 3)
                            return OriginalHook(Thunder);
                    }

                    if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && CanSpellWeave(ActionWatching.LastSpell))
                        return Amplifier;

                    if (remainingPolyglotCD < 6000 && gcdsInTimer > 2 && gauge.HasPolyglotStacks())
                        return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;

                    if (IsMoving)
                    {
                        if (ActionReady(Amplifier) && gauge.PolyglotStacks < maxPolyglot)
                            return Amplifier;

                        if (gauge.HasPolyglotStacks())
                            return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;
                    }

                    if (CanSpellWeave(actionID) && ActionReady(LeyLines))
                        return LeyLines;

                    if (gauge.InAstralFire)
                    {

                        if (gauge.IsParadoxActive && gcdsInTimer < 2 && curMp >= MP.FireI)
                            return Paradox;

                        if (HasEffect(Buffs.Firestarter))
                        {
                            if (gcdsInTimer < 2 || curMp < MP.FireI || WasLastAbility(Transpose))
                                return Fire3;
                        }

                        if (curMp < MP.FireI && Despair.LevelChecked() && curMp >= MP.Despair)
                        {
                            if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0)
                                return Triplecast;

                            return Despair;
                        }

                        if (curMp == 0 && FlareStar.LevelChecked() && gauge.AstralSoulStacks == 6)
                            return FlareStar;

                        if (Fire4.LevelChecked())
                        {
                            if (gcdsInTimer > 1 && curMp >= MP.FireI)
                                return Fire4;
                        }

                        if (curMp >= MP.FireI)
                            return Fire;

                        if (ActionReady(Manafont))
                            return Manafont;

                        if (ActionReady(Blizzard3) && !canSwiftB3)
                            return Blizzard3;

                        if (ActionReady(Transpose))
                            return Transpose;

                    }
                    if (gauge.InUmbralIce)
                    {
                        if (ActionReady(Blizzard3) && gauge.UmbralIceStacks < 3)
                        {
                            if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0)
                                return Triplecast;

                            if (GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast))
                                return All.Swiftcast;

                            if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                                return Blizzard3;
                        }

                        if (Blizzard4.LevelChecked() && gauge.UmbralHearts < 3)
                            return Blizzard4;

                        if (gauge.IsParadoxActive)
                            return Paradox;

                        if (gauge.HasPolyglotStacks())
                            return Xenoglossy.LevelChecked() ? Xenoglossy : Foul;

                        if (curMp + nextMpGain >= 7500 && (LocalPlayer.CastActionId == Blizzard || WasLastSpell(Blizzard) || WasLastSpell(Blizzard4)))
                        {
                            if (Fire3.LevelChecked())
                                return Fire3;

                            return Fire;
                        }

                        if ((curMp + nextMpGain <= 10000 || curMp < 7500))
                            return Blizzard;

                        if (ActionReady(Transpose) && CanSpellWeave(ActionWatching.LastSpell))
                            return Transpose;

                        if (Fire3.LevelChecked())
                            return Fire3;
                    }

                    if (Blizzard3.LevelChecked())
                        return Blizzard3;

                }
                return actionID;
            }
        }

        internal class BLM_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                uint currentMP = LocalPlayer.CurrentMp;
                BLMGauge? gauge = GetJobGauge<BLMGauge>();

                if (actionID is Blizzard2 or HighBlizzard2)
                {
                    // 2xHF2 Transpose with Freeze [A7]
                    if (gauge.ElementTimeRemaining <= 0)
                        return OriginalHook(Blizzard2);

                    if (gauge.ElementTimeRemaining > 0)
                    {
                        if (CurrentTarget is null)
                        {
                            if (gauge.InAstralFire && LevelChecked(Transpose))
                                return Transpose;

                            if (gauge.InUmbralIce && LevelChecked(UmbralSoul))
                                return UmbralSoul;
                        }

                        if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                            IsEnabled(Variant.VariantCure) &&
                            PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                            return Variant.VariantCure;

                        if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                            IsEnabled(Variant.VariantRampart) &&
                            IsOffCooldown(Variant.VariantRampart) &&
                            CanSpellWeave(actionID))
                            return Variant.VariantRampart;
                    }

                    // Astral Fire
                    // Astral Fire
if (gauge.InAstralFire)
{
    // Manafont to Flare
    if (LevelChecked(Flare))
    {
        if (ActionReady(Manafont) && currentMP is 0)
            return Manafont;

        if (WasLastAction(Manafont))
            return Flare;
    }

    // Polyglot usage
    if (LevelChecked(Foul) && gauge.HasPolyglotStacks() && WasLastAction(OriginalHook(Flare)))
        return Foul;

    // Blizzard to Umbral Ice
    if ((currentMP is 0 && WasLastAction(Flare)) ||
        (currentMP < MP.FireAoE && !LevelChecked(Flare)))
        return OriginalHook(Blizzard2);

    if (currentMP >= MP.AllMPSpells)
    {
        // Flare Usage in the AoE Rotation
        if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare) &&
            (gauge.UmbralHearts is 1 || currentMP < MP.FireAoE) &&
            ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast))
            return Triplecast;

        if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare) &&
            (gauge.UmbralHearts is 1 || currentMP < MP.FireAoE))
            return Flare;

        if (currentMP > MP.FireAoE)
            return OriginalHook(Fire2);
    }
}

                    // Umbral Ice
                    if (gauge.InUmbralIce)
                    {
                        if (gauge.UmbralHearts < 3 && LevelChecked(Freeze) && TraitLevelChecked(Traits.EnhancedFreeze) && currentMP >= MP.Freeze)
                            return Freeze;

                        // Thunder II/IV uptime
                        if (!ThunderList.ContainsKey(lastComboMove) && currentMP >= MP.ThunderAoE)
                        {
                            // Check for High Thunder II if level is 92 or above
                            if (LevelChecked(HighThunder2) && Svc.ClientState.LocalPlayer.Level >= 92 && GetDebuffRemainingTime(Debuffs.HighThunder2) <= 4)
                                return HighThunder2;

                            // Fallback to Thunder IV if under level 92
                            if (LevelChecked(Thunder4) && Svc.ClientState.LocalPlayer.Level < 92 && GetDebuffRemainingTime(Debuffs.Thunder4) <= 4)
                                return Thunder4;

                            // Fallback to Thunder II if Thunder IV is unavailable and the debuff needs refreshing
                            if (LevelChecked(Thunder2) && !LevelChecked(Thunder4) && GetDebuffRemainingTime(Debuffs.Thunder2) <= 4)
                                return Thunder2;
                        }

                        if (currentMP < 9400 && !TraitLevelChecked(Traits.EnhancedFreeze) && Freeze.LevelChecked() && currentMP >= MP.Freeze)
                            return Freeze;

                        if (currentMP >= 9400 && !TraitLevelChecked(Traits.AspectMasteryIII))
                            return Transpose;

                        if ((gauge.UmbralHearts is 3 || currentMP == MP.MaxMP) && TraitLevelChecked(Traits.AspectMasteryIII))
                            return OriginalHook(Fire2);
                    }
                }

                return actionID; // Ensure method returns a value
            }
        }

        internal class BLM_Variant_Raise : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Variant_Raise;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level) =>
                (actionID is All.Swiftcast && HasEffect(All.Buffs.Swiftcast) && IsEnabled(Variant.VariantRaise))
                ? Variant.VariantRaise
                : actionID;
        }

        internal class BLM_Scathe_Xeno : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Scathe_Xeno;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                (actionID is Scathe && LevelChecked(Xenoglossy) && GetJobGauge<BLMGauge>().HasPolyglotStacks())
                ? Xenoglossy
                : actionID;
        }

        internal class BLM_Blizzard_1to3 : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard_1to3;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Blizzard && LevelChecked(Freeze) && !GetJobGauge<BLMGauge>().InUmbralIce)
                    return Blizzard3;

                if (actionID is Freeze && !LevelChecked(Freeze))
                    return Blizzard2;

                return actionID;
            }
        }

        internal class BLM_Fire_1to3 : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_1to3;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                (actionID is Fire && ((LevelChecked(Fire3) && !GetJobGauge<BLMGauge>().InAstralFire) || HasEffect(Buffs.Firestarter)))
                ? Fire3
                : actionID;
        }

        internal class BLM_Between_The_LeyLines : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Between_The_LeyLines;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is LeyLines && HasEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
        }

        internal class BLM_Aetherial_Manipulation : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Aetherial_Manipulation;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is AetherialManipulation &&
                ActionReady(BetweenTheLines) &&
                HasEffect(Buffs.LeyLines) &&
                !HasEffect(Buffs.CircleOfPower) &&
                !IsMoving
                ? BetweenTheLines
                : actionID;
        }

        internal class BLM_UmbralSoul : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_UmbralSoul;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
                actionID is Transpose && GetJobGauge<BLMGauge>().InUmbralIce && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
        }
    }
}