using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class WAR
    {
        public const byte ClassID = 3;
        public const byte JobID = 21;
        public const uint
            HeavySwing = 31,
            Maim = 37,
            Berserk = 38,
            Overpower = 41,
            StormsPath = 42,
            StormsEye = 45,
            Tomahawk = 46,
            InnerBeast = 49,
            SteelCyclone = 51,
            Infuriate = 52,
            FellCleave = 3549,
            Decimate = 3550,
            Upheaval = 7387,
            InnerRelease = 7389,
            RawIntuition = 3551,
            MythrilTempest = 16462,
            ChaoticCyclone = 16463,
            NascentFlash = 16464,
            InnerChaos = 16465,
            Orogeny = 25752,
            PrimalRend = 25753,
            Onslaught = 7386;

        public static class Buffs
        {
            public const ushort
                InnerRelease = 1177,
                SurgingTempest = 2677,
                NascentChaos = 1897,
                PrimalRendReady = 2624,
                Berserk = 86;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 1;
        }

        public static class Config
        {
            public static UserInt
                WAR_InfuriateRange = new("WarInfuriateRange"),
                WAR_SurgingRefreshRange = new("WarSurgingRefreshRange"),
                WAR_KeepOnslaughtCharges = new("WarKeepOnslaughtCharges"),
                WAR_VariantCure = new("WAR_VariantCure"),
                WAR_FellCleaveGauge = new("WAR_FellCleaveGauge"),
                WAR_DecimateGauge = new("WAR_DecimateGauge"),
                WAR_InfuriateSTGauge = new("WAR_InfuriateSTGauge"),
                WAR_InfuriateAoEGauge = new("WAR_InfuriateAoEGauge");
        }

        // Replace Storm's Path with Storm's Path combo and overcap feature on main combo to fellcleave
        internal class WAR_ST_StormsPath : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_StormsPath;
            internal static WAROpenerLogic WAROpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                WARGauge? gauge = GetJobGauge<WARGauge>();
                int surgingThreshold = Config.WAR_SurgingRefreshRange;
                int onslaughtChargesRemaining = Config.WAR_KeepOnslaughtCharges;
                int fellCleaveGaugeSpend = Config.WAR_FellCleaveGauge;
                int infuriateGauge = Config.WAR_InfuriateSTGauge;
                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                if (actionID is HeavySwing)
                {
                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_RangedUptime) &&
                        LevelChecked(Tomahawk) && !InMeleeRange() && HasBattleTarget())
                        return Tomahawk;

                    // Opener for DRK
                    if (IsEnabled(CustomComboPreset.WAR_ST_Opener))
                    {
                        if (WAROpener.DoFullOpener(ref actionID, false))
                            return actionID;
                    }

                    if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Infuriate) &&
                        ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && gauge.BeastGauge <= infuriateGauge && CanWeave(actionID))
                        return Infuriate;

                    //Sub Storm's Eye level check
                    if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_InnerRelease) &&
                        CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && !LevelChecked(StormsEye))
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest))
                    {
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) &&
                                IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_InnerRelease) &&
                                ActionReady(OriginalHook(Berserk)))
                                return OriginalHook(Berserk);

                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Upheaval) &&
                                ActionReady(Upheaval))
                                return Upheaval;

                            if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Onslaught) &&
                                LevelChecked(Onslaught) && GetRemainingCharges(Onslaught) > onslaughtChargesRemaining)
                            {
                                if (IsNotEnabled(CustomComboPreset.WAR_ST_StormsPath_Onslaught_MeleeSpender) ||
                                    (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_Onslaught_MeleeSpender) &&
                                    !IsMoving && GetTargetDistance() <= 1 && (GetCooldownRemainingTime(InnerRelease) > 40 || !LevelChecked(InnerRelease))))
                                    return Onslaught;
                            }
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend) &&
                            HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend))
                        {
                            if (IsNotEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange) ||
                                (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange) &&
                                !IsMoving && (GetTargetDistance() <= 1 || GetBuffRemainingTime(Buffs.PrimalRendReady) <= 10)))
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_FellCleave) &&
                            LevelChecked(InnerBeast))
                        {
                            if (HasEffect(Buffs.InnerRelease) || (HasEffect(Buffs.NascentChaos) && LevelChecked(InnerChaos)))
                                return OriginalHook(InnerBeast);

                            if (HasEffect(Buffs.NascentChaos) && !LevelChecked(InnerChaos))
                                return OriginalHook(Decimate);
                        }

                    }

                    if (comboTime > 0)
                    {
                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_FellCleave) &&
                            LevelChecked(InnerBeast) && (!LevelChecked(StormsEye) || HasEffectAny(Buffs.SurgingTempest)) && gauge.BeastGauge >= fellCleaveGaugeSpend)
                            return OriginalHook(InnerBeast);

                        if (lastComboMove is HeavySwing && LevelChecked(Maim))
                            return Maim;

                        if (lastComboMove is Maim && LevelChecked(StormsPath))
                        {
                            if ((GetBuffRemainingTime(Buffs.SurgingTempest) <= surgingThreshold) &&
                                LevelChecked(StormsEye))
                                return StormsEye;

                            return StormsPath;
                        }
                    }
                    return HeavySwing;
                }
                return actionID;
            }
        }

        internal class War_ST_StormsEye : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.War_ST_StormsEye;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID == StormsEye)
                {
                    if (comboTime > 0)
                    {
                        if (lastComboMove is HeavySwing && LevelChecked(Maim))
                            return Maim;

                        if (lastComboMove is Maim && LevelChecked(StormsEye))
                            return StormsEye;
                    }

                    return HeavySwing;
                }

                return actionID;
            }
        }

        internal class WAR_AoE_Overpower : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_AoE_Overpower;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                WARGauge? gauge = GetJobGauge<WARGauge>();
                int decimateGaugeSpend = Config.WAR_DecimateGauge;
                int infuriateGauge = Config.WAR_InfuriateAoEGauge;
                Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                if (actionID is Overpower)
                {
                    if (IsEnabled(CustomComboPreset.WAR_Variant_Cure) && 
                        IsEnabled(Variant.VariantCure) && 
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.WAR_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_Infuriate) &&
                        ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos) && gauge.BeastGauge <= infuriateGauge && CanWeave(actionID))
                        return Infuriate;

                    //Sub Mythril Tempest level check
                    if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_InnerRelease) &&
                        CanWeave(actionID) && ActionReady(OriginalHook(Berserk)) && !LevelChecked(MythrilTempest))
                        return OriginalHook(Berserk);

                    if (HasEffect(Buffs.SurgingTempest))
                    {
                        if (CanWeave(actionID))
                        {
                            if (IsEnabled(CustomComboPreset.WAR_Variant_SpiritDart) &&
                                IsEnabled(Variant.VariantSpiritDart) &&
                                (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                                return Variant.VariantSpiritDart;

                            if (IsEnabled(CustomComboPreset.WAR_Variant_Ultimatum) && 
                                IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                                return Variant.VariantUltimatum;

                            if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_InnerRelease) &&
                                ActionReady(OriginalHook(Berserk)))
                                return OriginalHook(Berserk);

                            if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_Orogeny) &&
                                ActionReady(Orogeny))
                                return Orogeny;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend) && 
                            HasEffect(Buffs.PrimalRendReady) && LevelChecked(PrimalRend))
                        {
                            if (IsNotEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange)||
                                (IsEnabled(CustomComboPreset.WAR_ST_StormsPath_PrimalRend_CloseRange) && 
                                (GetTargetDistance() <= 3 || GetBuffRemainingTime(Buffs.PrimalRendReady) <= 10)))
                                return PrimalRend;
                        }

                        if (IsEnabled(CustomComboPreset.WAR_AoE_Overpower_Decimate) && LevelChecked(SteelCyclone) && 
                            (gauge.BeastGauge >= decimateGaugeSpend || HasEffect(Buffs.InnerRelease) || HasEffect(Buffs.NascentChaos)))
                            return OriginalHook(SteelCyclone);
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Overpower && LevelChecked(MythrilTempest))
                            return MythrilTempest;
                    }
                    return Overpower;
                }
                return actionID;
            }
        }

        internal class WAR_NascentFlash : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_NascentFlash;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is NascentFlash)
                {
                    if (LevelChecked(NascentFlash))
                        return NascentFlash;

                    return RawIntuition;
                }

                return actionID;
            }
        }


        internal class WAR_ST_StormsPath_PrimalRend : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_ST_StormsPath_PrimalRend;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is InnerBeast || actionID is SteelCyclone)
                {

                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;

                    // Fell Cleave or Decimate
                    return OriginalHook(actionID);
                }

                return actionID;
            }
        }

        internal class WAR_InfuriateFellCleave : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_InfuriateFellCleave;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                WARGauge? rageGauge = GetJobGauge<WARGauge>();
                int rageThreshold = PluginConfiguration.GetCustomIntValue(Config.WAR_InfuriateRange);

                if (actionID is InnerBeast or FellCleave or SteelCyclone or Decimate)
                {
                    if (InCombat() && rageGauge.BeastGauge <= rageThreshold && ActionReady(Infuriate) && !HasEffect(Buffs.NascentChaos)
                    && ((!HasEffect(Buffs.InnerRelease)) || IsNotEnabled(CustomComboPreset.WAR_InfuriateFellCleave_IRFirst)))
                        return OriginalHook(Infuriate);
                }
                return actionID;
            }
        }

        internal class WAR_PrimalRend_InnerRelease : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WAR_PrimalRend_InnerRelease;

            protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
            {
                if (actionID is Berserk or InnerRelease)
                {
                    if (LevelChecked(PrimalRend) && HasEffect(Buffs.PrimalRendReady))
                        return PrimalRend;
                }
                return actionID;
            }
        }
    }
}
