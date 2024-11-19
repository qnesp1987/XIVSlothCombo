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
            EncoreofLight = 41467,
            BlastArrow = 29394;

        public static class Buffs
        {
            public const ushort
                FrontlinersMarch = 3138,
                FrontlinersForte = 3140,
                Repertoire = 3137,
                EncoreofLightReady = 4312,
                BlastArrowReady = 3142;
        }

        internal class BRDPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BRDPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (HasEffect(Buffs.EncoreofLightReady))
                {
                    return OriginalHook(EncoreofLight);
                }

                if (actionID == PowerfulShot)
                {
                    if (HasEffect(Buffs.BlastArrowReady))
                        return OriginalHook(BlastArrow);

                    if (HasEffect(Buffs.Repertoire))
                        return OriginalHook(PowerfulShot);

                    if (!GetCooldown(ApexArrow).IsCooldown)
                        return OriginalHook(ApexArrow);

                    var harmonicCooldown = GetCooldown(HarmonicArrow);
                    if (!harmonicCooldown.IsCooldown && harmonicCooldown.RemainingCharges == 4)
                    {
                        return OriginalHook(HarmonicArrow);
                    }

                    return OriginalHook(PowerfulShot);
                }

                if (actionID == RepellingShot || actionID == WardensPaean)
                {
                    if (IsEnabled(CustomComboPreset.BRDPvP_SilentNocturne) && !GetCooldown(SilentNocturne).IsCooldown)
                    {
                        return OriginalHook(SilentNocturne);
                    }
                }

                return actionID;
            }
        }
    }
}
