using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class DRKOpenerLogic : PvE.DRK
    {
        private static bool HasCooldowns()
        {
            if (CustomComboFunctions.GetRemainingCharges(Plunge) < 2)
                return false;

            if (CustomComboFunctions.GetRemainingCharges(Shadowbringer) < 2)
                return false;

            if (!CustomComboFunctions.ActionReady(Delirium))
                return false;

            if (!CustomComboFunctions.ActionReady(LivingShadow))
                return false;

            if (!CustomComboFunctions.ActionReady(CarveAndSpit))
                return false;

            return true;
        }

        private static uint OpenerLevel => 90;

        public uint PrePullStep = 0;

        public uint OpenerStep = 0;

        private static uint[] StandardOpener = [
            HardSlash,
            EdgeOfShadow,
            Delirium,
            SyphonStrike,
            Souleater,
            LivingShadow,
            HardSlash,
            SaltedEarth,
            EdgeOfShadow,
            Bloodspiller,
            Shadowbringer,
            EdgeOfShadow,
            Bloodspiller,
            CarveAndSpit,
            Plunge,
            Bloodspiller,
            Shadowbringer,
            EdgeOfShadow,
            SyphonStrike,
            Souleater,
            Plunge,
            HardSlash];

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
                    if (value == OpenerState.InOpener) OpenerStep = 0;
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
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0)
                PrePullStep = 1;

            if (!HasCooldowns())
                PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (CustomComboFunctions.HasEffect(Buffs.BloodWeapon) && PrePullStep == 1)
                    PrePullStep++;

                else if (PrePullStep == 1)
                    actionID = BloodWeapon;

                if (CustomComboFunctions.WasLastAction(TheBlackestNight) && PrePullStep == 2)
                    currentState = OpenerState.InOpener;

                else if (PrePullStep == 2)
                    actionID = TheBlackestNight;

                if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }

            PrePullStep = 0;
            return false;
        }

    private bool DoOpener(uint[] OpenerActions, ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (CustomComboFunctions.WasLastAction(OpenerActions[OpenerStep]))
                    OpenerStep++;

                if (OpenerStep > 19 && CustomComboFunctions.HasEffect(Buffs.Darkside))
                    actionID = EdgeOfShadow;

                else if (OpenerStep == OpenerActions.Length)
                    CurrentState = OpenerState.OpenerFinished;

                else actionID = OpenerActions[OpenerStep];

                if (CustomComboFunctions.InCombat() && ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == Plunge && CustomComboFunctions.GetRemainingCharges(Plunge) == 0) ||
                                   (actionID == Delirium && CustomComboFunctions.IsOnCooldown(Delirium)) ||
                                   (actionID == LivingShadow && CustomComboFunctions.IsOnCooldown(LivingShadow)) ||
                                   (actionID == CarveAndSpit && CustomComboFunctions.IsOnCooldown(CarveAndSpit)) ||
                                   (actionID == Shadowbringer && CustomComboFunctions.GetRemainingCharges(Shadowbringer) == 0)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
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

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
            {
                if (DoOpener(StandardOpener, ref actionID))
                    return true;
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