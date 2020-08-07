using Kingmaker.UI.SettingsUI;
using System;
using static Kingmaker.UI.SettingsUI.SettingsEntityDropdownState;

namespace RNGTest
{
    public class DisableCombatText : IDisposable
    {
        public DropdownState ShowAvoidOnCombatText;
        public DropdownState ShowMissOnCombatText;
        public DropdownState ShowAttackOfOpportunityOnCombatText;
        public DropdownState ShowCriticalHitOnCombatText;
        public DropdownState ShowSneakAttackOnCombatText;
        public DropdownState ShowDamageOnCombatText;
        public DropdownState ShowSavesInCombatText;
        public DropdownState ShowPartyActions;
        public DropdownState ShowEnemyActions;
        public DropdownState ShowPartyHP;
        public DropdownState ShowEnemyHP;
        public DisableCombatText()
        {
            ShowAvoidOnCombatText = SettingsRoot.Instance.ShowAvoidOnCombatText.CurrentState;
            ShowMissOnCombatText = SettingsRoot.Instance.ShowMissOnCombatText.CurrentState;
            ShowAttackOfOpportunityOnCombatText = SettingsRoot.Instance.ShowAttackOfOpportunityOnCombatText.CurrentState;
            ShowCriticalHitOnCombatText = SettingsRoot.Instance.ShowCriticalHitOnCombatText.CurrentState;
            ShowSneakAttackOnCombatText = SettingsRoot.Instance.ShowSneakAttackOnCombatText.CurrentState;
            ShowDamageOnCombatText = SettingsRoot.Instance.ShowDamageOnCombatText.CurrentState;
            ShowSavesInCombatText = SettingsRoot.Instance.ShowSavesInCombatText.CurrentState;
            ShowPartyActions = SettingsRoot.Instance.ShowPartyActions.CurrentState;
            ShowEnemyActions = SettingsRoot.Instance.ShowEnemyActions.CurrentState;
            ShowPartyHP = SettingsRoot.Instance.ShowPartyHP.CurrentState;
            ShowEnemyHP = SettingsRoot.Instance.ShowEnemyHP.CurrentState;
            SettingsRoot.Instance.ShowAvoidOnCombatText.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowMissOnCombatText.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowAttackOfOpportunityOnCombatText.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowCriticalHitOnCombatText.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowSneakAttackOnCombatText.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowDamageOnCombatText.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowSavesInCombatText.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowPartyActions.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowEnemyActions.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowPartyHP.CurrentState = DropdownState.None;
            SettingsRoot.Instance.ShowEnemyHP.CurrentState = DropdownState.None;
        }

        public void Dispose()
        {
            SettingsRoot.Instance.ShowAvoidOnCombatText.CurrentState = ShowAvoidOnCombatText;
            SettingsRoot.Instance.ShowMissOnCombatText.CurrentState = ShowMissOnCombatText;
            SettingsRoot.Instance.ShowAttackOfOpportunityOnCombatText.CurrentState = ShowAttackOfOpportunityOnCombatText;
            SettingsRoot.Instance.ShowCriticalHitOnCombatText.CurrentState = ShowCriticalHitOnCombatText;
            SettingsRoot.Instance.ShowSneakAttackOnCombatText.CurrentState = ShowSneakAttackOnCombatText;
            SettingsRoot.Instance.ShowDamageOnCombatText.CurrentState = ShowDamageOnCombatText;
            SettingsRoot.Instance.ShowSavesInCombatText.CurrentState = ShowSavesInCombatText;
            SettingsRoot.Instance.ShowPartyActions.CurrentState = ShowPartyActions;
            SettingsRoot.Instance.ShowEnemyActions.CurrentState = ShowEnemyActions;
            SettingsRoot.Instance.ShowPartyHP.CurrentState = ShowPartyHP;
            SettingsRoot.Instance.ShowEnemyHP.CurrentState = ShowEnemyHP;
        }
    }
}
