@ -320,57 +320,44 @@ namespace XIVSlothCombo.Combos.PvE
                    }

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
                    // Astral Fire
if (gauge.InAstralFire)
{
    // Manafont to Flare
    if (LevelChecked(Flare))
    {
        if (ActionReady(Manafont) && currentMP is 0)
            return Manafont;

                        // Blizzard to Umbral Ice
                        if ((currentMP is 0 && WasLastAction(Flare)) ||
                            (currentMP < MP.FireAoE && !LevelChecked(Flare)))
                            return OriginalHook(Blizzard2);
        if (WasLastAction(Manafont))
            return Flare;
    }

                        if (currentMP >= MP.AllMPSpells)
                        {
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

                            if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare) &&
                                (gauge.UmbralHearts is 1 || currentMP < MP.FireAoE) &&
                                ActionReady(Triplecast) && !HasEffect(Buffs.Triplecast))
                                return Triplecast;
    // Polyglot usage
    if (LevelChecked(Foul) && gauge.HasPolyglotStacks() && WasLastAction(OriginalHook(Flare)))
        return Foul;

                            if (LevelChecked(Flare) && HasEffect(Buffs.EnhancedFlare) &&
                                (gauge.UmbralHearts is 1 || currentMP < MP.FireAoE))
                                return Flare;
    // Blizzard to Umbral Ice
    if ((currentMP is 0 && WasLastAction(Flare)) ||
        (currentMP < MP.FireAoE && !LevelChecked(Flare)))
        return OriginalHook(Blizzard2);

                            if (currentMP > MP.FireAoE)
                                return OriginalHook(Fire2);
                        }
                    }
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
