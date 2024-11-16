using XIVSlothCombo.CustomComboNS;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class BRDPvP
    {
        public const byte ClassID = 5;
        public const byte JobID = 23;

        public const uint
            PowerfulShot = 29391,
            ApexArrow = 29393,
            SilentNocturne = 29395,
            HarmonicArrow = 41464, 
            RepellingShot = 29399,
            WardensPaean = 29400,
            PitchPerfect = 29392,
            EncoreofLight = 41467, // Added Encore of Light
            BlastArrow = 29394;

        public static class Buffs
        {
            public const ushort
                FrontlinersMarch = 3138,
                FrontlinersForte = 3140,
                Repertoire = 3137,
                EncoreofLightReady = 4312, // Added Encore of Light buff
                BlastArrowReady = 3142;
        }

        internal class BRDPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                // Check if Encore of Light is ready and prioritize it
                if (HasEffect(Buffs.EncoreofLightReady))
                {
                    return OriginalHook(EncoreofLight);
                }

                // Handle GCD: PowerfulShot and related actions
                if (actionID == PowerfulShot)
                {
                    if (HasEffect(Buffs.BlastArrowReady))
                        return OriginalHook(BlastArrow);

                    if (HasEffect(Buffs.Repertoire))
                        return OriginalHook(PowerfulShot);

                    if (!GetCooldown(ApexArrow).IsCooldown)
                        return OriginalHook(ApexArrow);

                    return OriginalHook(PowerfulShot);
                }

                
                var harmonicCooldown = GetCooldown(HarmonicArrow);
                PluginLog.Log($"HarmonicArrow Cooldown: IsCooldown={harmonicCooldown.IsCooldown}, RemainingCharges={harmonicCooldown.RemainingCharges}");
                if (!harmonicCooldown.IsCooldown && harmonicCooldown.RemainingCharges == 4)
                {
                    return OriginalHook(HarmonicArrow);
                }

                if (IsEnabled(CustomComboPreset.BRDPvP_SilentNocturne) && !GetCooldown(SilentNocturne).IsCooldown)
                {
                    return OriginalHook(SilentNocturne);
                }

                
                return actionID;
            }
        }
    }
}
