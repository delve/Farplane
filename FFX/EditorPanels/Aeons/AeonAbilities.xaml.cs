﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Farplane.Common;
using Farplane.FFX.Values;
using MahApps.Metro.Controls;

namespace Farplane.FFX.EditorPanels.Aeons
{
    /// <summary>
    /// Interaction logic for PartyAbilities.xaml
    /// </summary>
    public partial class AeonAbilities : UserControl
    {
        private ButtonGrid _gridSkill = new ButtonGrid(2, 22);
        private ButtonGrid _gridSpecial = new ButtonGrid(2, 22);
        private ButtonGrid _gridWhiteMagic = new ButtonGrid(2, 22);
        private ButtonGrid _gridBlackMagic = new ButtonGrid(2, 19);

        private readonly Ability[] _skills = Ability.Abilities.Where(a => a.Type == AbilityType.Skill).ToArray();
        private readonly Ability[] _specials = Ability.Abilities.Where(a => a.Type == AbilityType.Special).ToArray();
        private readonly Ability[] _wMagic = Ability.Abilities.Where(a => a.Type == AbilityType.WhiteMagic).ToArray();
        private readonly Ability[] _bMagic = Ability.Abilities.Where(a => a.Type == AbilityType.BlackMagic).ToArray();
        private int _characterIndex = -1;

        public AeonAbilities()
        {
            InitializeComponent();
            foreach(var tabItem in TabAbilities.Items)
                ControlsHelper.SetHeaderFontSize((TabItem)tabItem, 14);

            TabSkills.Content = _gridSkill;
            TabSpecial.Content = _gridSpecial;
            TabWhiteMagic.Content = _gridWhiteMagic;
            TabBlackMagic.Content = _gridBlackMagic;

            _gridSkill.ButtonClicked += ButtonSkill_Click;
            _gridSpecial.ButtonClicked += ButtonSpecial_Click;
            _gridWhiteMagic.ButtonClicked += ButtonWhiteMagic_Click;
            _gridBlackMagic.ButtonClicked += ButtonBlackMagic_Click;

            _gridSkill.ShowScrollBar = false;
            _gridSpecial.ShowScrollBar = false;
            _gridWhiteMagic.ShowScrollBar = false;
            _gridBlackMagic.ShowScrollBar = false;
        }
        
        private void ButtonSkill_Click(int buttonIndex)
        {
            ToggleSkill(AbilityType.Skill, buttonIndex);
            Refresh(_characterIndex);
        }

        private void ButtonSpecial_Click(int buttonIndex)
        {
            ToggleSkill(AbilityType.Special, buttonIndex);
            Refresh(_characterIndex);
        }

        private void ButtonWhiteMagic_Click(int buttonIndex)
        {
            ToggleSkill(AbilityType.WhiteMagic, buttonIndex);
            Refresh(_characterIndex);
        }

        private void ButtonBlackMagic_Click(int buttonIndex)
        {
            ToggleSkill(AbilityType.BlackMagic, buttonIndex);
            Refresh(_characterIndex);
        }

        private void ToggleSkill(AbilityType type, int buttonIndex)
        {
            if (_characterIndex == -1) return;

            Ability skill = null;
            switch (type)
            {
                case AbilityType.Skill:
                    skill = _skills[buttonIndex];
                    break;
                case AbilityType.Special:
                    skill = _specials[buttonIndex];
                    break;
                case AbilityType.WhiteMagic:
                    skill = _wMagic[buttonIndex];
                    break;
                case AbilityType.BlackMagic:
                    skill = _bMagic[buttonIndex];
                    break;
                default:
                    return;
            }
             
            var byteIndex = skill.BitOffset / 8;
            var bitIndex = skill.BitOffset % 8;
            var offset = Offsets.GetOffset(OffsetType.PartyStatsBase) + 0x94 * _characterIndex +
                         (int)PartyStatOffset.SkillFlags;
            var skillBytes =
                MemoryReader.ReadBytes(offset, 0x0C);

            var newByte = BitHelper.ToggleBit(skillBytes[byteIndex], bitIndex);
            skillBytes[byteIndex] = newByte;

            MemoryReader.WriteBytes(offset, skillBytes);
        }

        public void Refresh(int characterIndex)
        {
            if (characterIndex == -1) return;
            _characterIndex = characterIndex;
            var skillBytes =
                MemoryReader.ReadBytes(Offsets.GetOffset(OffsetType.PartyStatsBase) + 0x94* _characterIndex + (int) PartyStatOffset.SkillFlags,
                    0x0C);
            var skillArray = BitHelper.GetBitArray(skillBytes);

            for (int i = 0; i < _skills.Length; i++)
            {
                var button = (Button)_gridSkill.GridBase.Children[i];
                button.Content = $"{_skills[i].Name} - {skillArray[_skills[i].BitOffset]}";
            }

            
            for (int i = 0; i < _specials.Length; i++)
            {
                var button = (Button)_gridSpecial.GridBase.Children[i];
                button.Content = $"{_specials[i].Name} - {skillArray[_specials[i].BitOffset]}";
            }

            
            for (int i = 0; i < _wMagic.Length; i++)
            {
                var button = (Button)_gridWhiteMagic.GridBase.Children[i];
                button.Content = $"{_wMagic[i].Name} - {skillArray[_wMagic[i].BitOffset]}";
            }

            
            for (int i = 0; i < _bMagic.Length; i++)
            {
                var button = (Button)_gridBlackMagic.GridBase.Children[i];
                button.Content = $"{_bMagic[i].Name} - {skillArray[_bMagic[i].BitOffset]}";
            }
        }
    }
}
