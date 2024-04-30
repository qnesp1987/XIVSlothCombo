using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class WAROpenerLogic : PvE.WAR
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Onslaught) < 3)
                return false;
            if (CustomComboFunctions.GetRemainingCharges(Infuriate) < 2)
                return false;
            if (!CustomComboFunctions.ActionReady(InnerRelease))
                return false;
            if (!CustomComboFunctions.ActionReady(Upheaval))
                return false;

            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 1;

        public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && LevelChecked;

        private OpenerState currentState = OpenerState.PrePull;

        public OpenerState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull)
                    {
                        Svc.Log.Debug($"Entered PrePull Opener");
                    }
                    if (value == OpenerState.InOpener) OpenerStep = 1;
                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (CanOpener && PrePullStep == 0)
            {
                PrePullStep = 1;
            }

            if (!HasCooldowns())
            {
                PrePullStep = 0;
            }

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.WasLastAction(Tomahawk) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = Tomahawk;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            PrePullStep = 0;
            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(Infuriate) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = Infuriate;

                if (CustomComboFunctions.WasLastAction(HeavySwing) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = HeavySwing;

                if (CustomComboFunctions.WasLastAction(Maim) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Maim;

                if (CustomComboFunctions.WasLastAction(StormsEye) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = StormsEye;

                if (CustomComboFunctions.WasLastAction(InnerRelease) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = InnerRelease;

                if (CustomComboFunctions.WasLastAction(InnerChaos) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = InnerChaos;

                if (CustomComboFunctions.WasLastAction(Upheaval) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = Upheaval;

                if (CustomComboFunctions.WasLastAction(Onslaught) && CustomComboFunctions.GetRemainingCharges(Onslaught) == 2 && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = Onslaught;

                if (CustomComboFunctions.WasLastAction(PrimalRend) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = PrimalRend;

                if (CustomComboFunctions.WasLastAction(Infuriate) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Infuriate;

                if (CustomComboFunctions.WasLastAction(InnerChaos) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = InnerChaos;

                if (CustomComboFunctions.WasLastAction(Onslaught) && CustomComboFunctions.GetRemainingCharges(Onslaught) == 1 && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Onslaught;

                if (CustomComboFunctions.WasLastAction(FellCleave) && CustomComboFunctions.GetBuffStacks(Buffs.InnerRelease) == 2 && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(Onslaught) && CustomComboFunctions.GetRemainingCharges(Onslaught) == 0 && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Onslaught;

                if (CustomComboFunctions.WasLastAction(FellCleave) && CustomComboFunctions.GetBuffStacks(Buffs.InnerRelease) == 1 && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(FellCleave) && CustomComboFunctions.GetBuffStacks(Buffs.InnerRelease) == 0 && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(HeavySwing) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = HeavySwing;

                if (CustomComboFunctions.WasLastAction(Maim) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Maim;

                if (CustomComboFunctions.WasLastAction(StormsPath) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = StormsPath;

                if (CustomComboFunctions.WasLastAction(FellCleave) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(Infuriate) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Infuriate;

                if (CustomComboFunctions.WasLastAction(InnerChaos) && OpenerStep == 22) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 22) actionID = InnerChaos;

                if (((actionID == InnerRelease && CustomComboFunctions.IsOnCooldown(InnerRelease)) ||
                    (actionID == Upheaval && CustomComboFunctions.IsOnCooldown(Upheaval)) ||
                    (actionID == Infuriate && CustomComboFunctions.GetRemainingCharges(Infuriate) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (CustomComboFunctions.GetResourceCost(actionID) > CustomComboFunctions.LocalPlayer.CurrentMp && ActionWatching.TimeSinceLastAction.TotalSeconds >= 2)
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            return false;
        }

        private bool DoOpenerSimple(ref uint actionID)
        {
            if (!LevelChecked) return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(Infuriate) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = Infuriate;

                if (CustomComboFunctions.WasLastAction(HeavySwing) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = HeavySwing;

                if (CustomComboFunctions.WasLastAction(Maim) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Maim;

                if (CustomComboFunctions.WasLastAction(StormsEye) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = StormsEye;

                if (CustomComboFunctions.WasLastAction(InnerRelease) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = InnerRelease;

                if (CustomComboFunctions.WasLastAction(InnerChaos) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = InnerChaos;

                if (CustomComboFunctions.WasLastAction(Upheaval) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = Upheaval;

                if (CustomComboFunctions.WasLastAction(Onslaught) && CustomComboFunctions.GetRemainingCharges(Onslaught) == 2 && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = Onslaught;

                if (CustomComboFunctions.WasLastAction(PrimalRend) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = PrimalRend;

                if (CustomComboFunctions.WasLastAction(Infuriate) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Infuriate;

                if (CustomComboFunctions.WasLastAction(InnerChaos) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = InnerChaos;

                if (CustomComboFunctions.WasLastAction(Onslaught) && CustomComboFunctions.GetRemainingCharges(Onslaught) == 1 && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = Onslaught;

                if (CustomComboFunctions.WasLastAction(FellCleave) && CustomComboFunctions.GetBuffStacks(Buffs.InnerRelease) == 2 && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(Onslaught) && CustomComboFunctions.GetRemainingCharges(Onslaught) == 0 && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = Onslaught;

                if (CustomComboFunctions.WasLastAction(FellCleave) && CustomComboFunctions.GetBuffStacks(Buffs.InnerRelease) == 1 && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(FellCleave) && CustomComboFunctions.GetBuffStacks(Buffs.InnerRelease) == 0 && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(HeavySwing) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = HeavySwing;

                if (CustomComboFunctions.WasLastAction(Maim) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = Maim;

                if (CustomComboFunctions.WasLastAction(StormsPath) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = StormsPath;

                if (CustomComboFunctions.WasLastAction(FellCleave) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = FellCleave;

                if (CustomComboFunctions.WasLastAction(Infuriate) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = Infuriate;

                if (CustomComboFunctions.WasLastAction(InnerChaos) && OpenerStep == 22) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 22) actionID = InnerChaos;

                if (((actionID == InnerRelease && CustomComboFunctions.IsOnCooldown(InnerRelease)) ||
                    (actionID == Upheaval && CustomComboFunctions.IsOnCooldown(Upheaval)) ||
                    (actionID == Infuriate && CustomComboFunctions.GetRemainingCharges(Infuriate) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (CustomComboFunctions.GetResourceCost(actionID) > CustomComboFunctions.LocalPlayer.CurrentMp && ActionWatching.TimeSinceLastAction.TotalSeconds >= 2)
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID, bool simpleMode)
        {
            if (!LevelChecked) return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID)) return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (simpleMode)
                {
                    if (DoOpenerSimple(ref actionID)) return true;
                }
                else
                {
                    if (DoOpener(ref actionID)) return true;
                }
            }

            if (!CustomComboFunctions.InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }


            return false;
        }
    }
}