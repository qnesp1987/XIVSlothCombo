using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Statuses;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class DRK
    {
        public const byte JobID = 32;

        public const uint
            HardSlash = 3617,
            Unleash = 3621,
            SyphonStrike = 3623,
            Souleater = 3632,
            SaltedEarth = 3639,
            AbyssalDrain = 3641,
            CarveAndSpit = 3643,
            Delirium = 7390,
            Quietus = 7391,
            Bloodspiller = 7392,
            FloodOfDarkness = 16466,
            EdgeOfDarkness = 16467,
            StalwartSoul = 16468,
            FloodOfShadow = 16469,
            EdgeOfShadow = 16470,
            LivingShadow = 16472,
            SaltAndDarkness = 25755,
            Oblation = 25754,
            Shadowbringer = 25757,
            Plunge = 3640,
            BloodWeapon = 3625,
            Unmend = 3624,
            TheBlackestNight = 7393;

        public static class Buffs
        {
            public const ushort
                BloodWeapon = 742,
                Darkside = 751,
                BlackestNight = 1178,
                Delirium = 1972,
                SaltedEarth = 749;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 1;
        }

        public static class Config
        {
            public static UserInt
                DRK_KeepPlungeCharges = new("DrkKeepPlungeCharges"),
                DRK_MPManagement = new("DrkMPManagement"),
                DRK_VariantCure = new("DRKVariantCure");
        }

        internal class DRK_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_ST_AdvancedMode;
            internal static DRKOpenerLogic DRKOpener = new();
            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is HardSlash)
                {
                    DRKGauge? gauge = GetJobGauge<DRKGauge>();
                    int plungeChargesRemaining = Config.DRK_KeepPlungeCharges;
                    int mpRemaining = Config.DRK_MPManagement;

                    if (IsEnabled(CustomComboPreset.DRK_Variant_Cure) && IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.DRK_VariantCure))
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.DRK_RangedUptime) &&
                        LevelChecked(Unmend) && !InMeleeRange() && HasBattleTarget())
                        return Unmend;

                    // Opener for DRK
                    if (IsEnabled(CustomComboPreset.DRK_ST_Opener))
                    {
                        if (DRKOpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    // oGCDs
                    if (CanWeave(actionID))
                    {
                        Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);
                        if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum) &&
                            IsEnabled(Variant.VariantUltimatum) && IsOffCooldown(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        //Mana Features
                        if (IsEnabled(CustomComboPreset.DRK_ManaOvercap))
                        {
                            if ((CombatEngageDuration().TotalSeconds < 7 && gauge.DarksideTimeRemaining == 0) ||
                                CombatEngageDuration().TotalSeconds >= 6)
                            {
                                if (IsEnabled(CustomComboPreset.DRK_EoSPooling) &&
                                    GetCooldownRemainingTime(Delirium) >= 40 &&
                                    (gauge.HasDarkArts || LocalPlayer.CurrentMp > (mpRemaining + 3000)) &&
                                    LevelChecked(EdgeOfDarkness))
                                    return OriginalHook(EdgeOfDarkness);

                                if (gauge.HasDarkArts ||
                                    LocalPlayer.CurrentMp > 8500 ||
                                    (gauge.DarksideTimeRemaining < 10000 && LocalPlayer.CurrentMp >= 3000))
                                {
                                    if (LevelChecked(EdgeOfDarkness))
                                        return OriginalHook(EdgeOfDarkness);

                                    if (LevelChecked(FloodOfDarkness) && !LevelChecked(EdgeOfDarkness))
                                        return FloodOfDarkness;
                                }
                            }
                        }

                        //oGCD Features
                        if (gauge.DarksideTimeRemaining > 1)
                        {
                            if (IsEnabled(CustomComboPreset.DRK_MainComboCDs_Group) &&
                                IsEnabled(CustomComboPreset.DRK_LivingShadow) &&
                                LevelChecked(LivingShadow) && gauge.Blood >= 50 &&
                                (GetCooldownRemainingTime(LivingShadow) <= GetCooldownRemainingTime(OriginalHook(HardSlash)) + 0.25 || ActionReady(LivingShadow)))
                                return LivingShadow;

                            if (IsEnabled(CustomComboPreset.DRK_MainComboBuffs_Group))
                            {
                                if (IsEnabled(CustomComboPreset.DRK_BloodWeapon) && LevelChecked(BloodWeapon) &&
                                    (GetCooldownRemainingTime(BloodWeapon) <= GetCooldownRemainingTime(OriginalHook(HardSlash)) + 0.25 || ActionReady(BloodWeapon)))
                                    return BloodWeapon;

                                if (IsEnabled(CustomComboPreset.DRK_Delirium) && LevelChecked(Delirium) &&
                                    (GetCooldownRemainingTime(Delirium) <= GetCooldownRemainingTime(OriginalHook(HardSlash)) + 0.25 || ActionReady(Delirium)))
                                    return Delirium;
                            }

                            if (IsEnabled(CustomComboPreset.DRK_MainComboCDs_Group))
                            {
                                if (IsEnabled(CustomComboPreset.DRK_SaltedEarth) && LevelChecked(SaltedEarth))
                                {
                                    if ((IsOffCooldown(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                        (HasEffect(Buffs.SaltedEarth) && ActionReady(SaltAndDarkness) && IsOnCooldown(SaltedEarth) && GetBuffRemainingTime(Buffs.SaltedEarth) < 9)) //Salt and Darkness
                                        return OriginalHook(SaltedEarth);
                                }

                                if (IsEnabled(CustomComboPreset.DRK_Shadowbringer) && LevelChecked(Shadowbringer))
                                {
                                    if ((IsNotEnabled(CustomComboPreset.DRK_ShadowbringerBurst) &&
                                        (GetCooldownRemainingTime(Shadowbringer) <= GetCooldownRemainingTime(OriginalHook(HardSlash)) + 0.25 || ActionReady(Shadowbringer))) ||
                                        (IsEnabled(CustomComboPreset.DRK_ShadowbringerBurst) &&
                                        (GetCooldownRemainingTime(Shadowbringer) <= GetCooldownRemainingTime(OriginalHook(HardSlash)) + 0.25 || ActionReady(Shadowbringer)) &&
                                        gauge.ShadowTimeRemaining > 1 && IsOnCooldown(Delirium))) //burst feature
                                        return Shadowbringer;
                                }

                                if (IsEnabled(CustomComboPreset.DRK_CarveAndSpit) && LevelChecked(CarveAndSpit) &&
                                    (GetCooldownRemainingTime(CarveAndSpit) <= GetCooldownRemainingTime(OriginalHook(HardSlash)) + 0.25 || ActionReady(CarveAndSpit)))
                                    return CarveAndSpit;

                                if (IsEnabled(CustomComboPreset.DRK_Plunge) &&
                                    LevelChecked(Plunge) && !IsMoving && (GetRemainingCharges(Plunge) > plungeChargesRemaining))
                                {
                                    if (IsNotEnabled(CustomComboPreset.DRK_MeleePlunge) ||
                                        (IsEnabled(CustomComboPreset.DRK_MeleePlunge) && GetTargetDistance() <= 1 &&
                                        IsOnCooldown(Delirium) && IsOnCooldown(BloodWeapon)))
                                        return Plunge;
                                }
                            }
                        }
                    }

                    //Delirium Features
                    if (IsEnabled(CustomComboPreset.DRK_Bloodspiller) &&
                        IsEnabled(CustomComboPreset.DRK_MainComboCDs_Group) &&
                        LevelChecked(Delirium) && gauge.DarksideTimeRemaining > 1)
                    {
                        //Regular Delirium
                        if (IsNotEnabled(CustomComboPreset.DRK_DelayedBloodspiller) &&
                            GetBuffStacks(Buffs.Delirium) > 0)
                            return Bloodspiller;

                        //Delayed Delirium
                        if (IsEnabled(CustomComboPreset.DRK_DelayedBloodspiller) &&
                            GetBuffStacks(Buffs.Delirium) > 0 && IsOnCooldown(BloodWeapon) && GetBuffStacks(Buffs.BloodWeapon) < 2)
                            return Bloodspiller;

                        //Blood management before Delirium
                        if (IsEnabled(CustomComboPreset.DRK_Delirium) &&
                            ((gauge.Blood >= 60 && GetCooldownRemainingTime(BloodWeapon) is > 0 and < 3) ||
                            (gauge.Blood >= 50 && GetCooldownRemainingTime(Delirium) > 37 && !HasEffect(Buffs.Delirium))))
                            return Bloodspiller;
                    }

                    // 1-2-3 combo
                    if (comboTime > 0)
                    {
                        if (lastComboMove is HardSlash && LevelChecked(SyphonStrike))
                            return SyphonStrike;

                        if (lastComboMove is SyphonStrike && LevelChecked(Souleater))
                        {
                            if (IsEnabled(CustomComboPreset.DRK_BloodGaugeOvercap) &&
                                LevelChecked(Bloodspiller) && gauge.Blood >= 90)
                                return Bloodspiller;
                            return Souleater;
                        }
                    }
                    return HardSlash;
                }
                return actionID;
            }
        }

        internal class DRK_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Unleash)
                {
                    DRKGauge? gauge = GetJobGauge<DRKGauge>();
                    Status? sustainedDamage = FindTargetEffect(Variant.Debuffs.SustainedDamage);

                    if (IsEnabled(CustomComboPreset.DRK_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= GetOptionValue(Config.DRK_VariantCure))
                        return Variant.VariantCure;


                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.DRK_Variant_SpiritDart) &&
                            IsEnabled(Variant.VariantSpiritDart) &&
                            (sustainedDamage is null || sustainedDamage?.RemainingTime <= 3))
                            return Variant.VariantSpiritDart;

                        if (IsEnabled(CustomComboPreset.DRK_Variant_Ultimatum) &&
                            ActionReady(Variant.VariantUltimatum))
                            return Variant.VariantUltimatum;

                        if (IsEnabled(CustomComboPreset.DRK_AoE_ManaOvercap) &&
                            LevelChecked(FloodOfDarkness) &&
                            (gauge.HasDarkArts ||
                            LocalPlayer.CurrentMp > 8500 ||
                            (gauge.DarksideTimeRemaining < 10 && LocalPlayer.CurrentMp >= 3000)))
                            return OriginalHook(FloodOfDarkness);

                        if (gauge.DarksideTimeRemaining > 1)
                        {
                            if (IsEnabled(CustomComboPreset.DRK_AoE_BloodWeapon) && LevelChecked(BloodWeapon) &&
                                (GetCooldownRemainingTime(BloodWeapon) <= GetCooldownRemainingTime(OriginalHook(Unleash)) + 0.25 || ActionReady(BloodWeapon)))
                                return BloodWeapon;

                            if (IsEnabled(CustomComboPreset.DRK_AoE_Delirium) && LevelChecked(Delirium) &&
                                (GetCooldownRemainingTime(Delirium) <= GetCooldownRemainingTime(OriginalHook(Unleash)) + 0.25 || ActionReady(Delirium)))
                                return Delirium;

                            if (IsEnabled(CustomComboPreset.DRK_AoE_LivingShadow) &&
                                LevelChecked(LivingShadow) && gauge.Blood >= 50 &&
                                (GetCooldownRemainingTime(LivingShadow) <= GetCooldownRemainingTime(OriginalHook(Unleash)) + 0.25 || ActionReady(LivingShadow)))
                                return LivingShadow;

                            if ((IsEnabled(CustomComboPreset.DRK_AoE_SaltedEarth) &&
                                ActionReady(SaltedEarth) && !HasEffect(Buffs.SaltedEarth)) || //Salted Earth
                                (HasEffect(Buffs.SaltedEarth) && ActionReady(SaltAndDarkness) && IsOnCooldown(SaltedEarth))) //Salt and Darkness
                                return OriginalHook(SaltedEarth);

                            if (IsEnabled(CustomComboPreset.DRK_AoE_AbyssalDrain) &&
                                LevelChecked(AbyssalDrain) && PlayerHealthPercentageHp() <= 60 &&
                                (GetCooldownRemainingTime(AbyssalDrain) <= GetCooldownRemainingTime(OriginalHook(Unleash)) + 0.25 || ActionReady(AbyssalDrain)))
                                return AbyssalDrain;

                            if (IsEnabled(CustomComboPreset.DRK_AoE_Shadowbringer) && LevelChecked(Shadowbringer) &&
                                 (GetCooldownRemainingTime(Shadowbringer) <= GetCooldownRemainingTime(OriginalHook(Unleash)) + 0.25 || ActionReady(Shadowbringer)))
                                return Shadowbringer;
                        }
                    }

                    if (IsEnabled(CustomComboPreset.DRK_Delirium) &&
                        LevelChecked(Delirium) && HasEffect(Buffs.Delirium) && gauge.DarksideTimeRemaining > 1)
                        return Quietus;

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Unleash && LevelChecked(StalwartSoul))
                        {
                            if (IsEnabled(CustomComboPreset.DRK_Overcap) && gauge.Blood >= 90 && LevelChecked(Quietus))
                                return Quietus;

                            return StalwartSoul;
                        }
                    }
                    return Unleash;
                }
                return actionID;
            }
        }

        internal class DRK_oGCD : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DRK_oGCD;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                DRKGauge? gauge = GetJobGauge<DRKGauge>();

                if ((actionID is CarveAndSpit || actionID is AbyssalDrain) && gauge.DarksideTimeRemaining > 0)
                {
                    if (gauge.Blood >= 50 && ActionReady(LivingShadow))
                        return LivingShadow;

                    if (ActionReady(SaltedEarth))
                        return SaltedEarth;

                    if (IsOffCooldown(CarveAndSpit) && LevelChecked(AbyssalDrain))
                        return actionID;

                    if (ActionReady(SaltAndDarkness) && HasEffect(Buffs.SaltedEarth))
                        return SaltAndDarkness;

                    if (IsEnabled(CustomComboPreset.DRK_Shadowbringer_oGCD) &&
                        ActionReady(Shadowbringer))
                        return Shadowbringer;
                }
                return actionID;
            }
        }
    }
}