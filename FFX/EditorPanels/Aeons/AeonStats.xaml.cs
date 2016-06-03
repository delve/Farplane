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

namespace Farplane.FFX.EditorPanels.Aeons
{
    /// <summary>
    /// Interaction logic for PartyStats.xaml
    /// </summary>
    public partial class AeonStats : UserControl
    {
        private const int StatsLength = 0x94;
        private int _statsBase;
        private int _nameBase;
        private int _characterIndex;

        public AeonStats()
        {
            InitializeComponent();
        }

        public void Refresh(int characterIndex)
        {
            _characterIndex = characterIndex;
            _statsBase = Offsets.GetOffset(OffsetType.PartyStatsBase) + StatsLength * _characterIndex;
            _nameBase = Offsets.GetOffset(OffsetType.AeonNames) + Offsets.AeonNames[_characterIndex-8];
            var statBytes = MemoryReader.ReadBytes(_statsBase, StatsLength);
            var nameBytes = MemoryReader.ReadBytes(_nameBase, 8);

            TextAeonName.Text = StringConverter.ToString(nameBytes);

            TextCurrentHP.Text = BitConverter.ToUInt32(statBytes, (int) PartyStatOffset.HPCurrent).ToString();
            TextMaxHP.Text = BitConverter.ToUInt32(statBytes, (int)PartyStatOffset.HPMax).ToString();
            TextCurrentMP.Text = BitConverter.ToUInt32(statBytes, (int)PartyStatOffset.MPCurrent).ToString();
            TextMaxMP.Text = BitConverter.ToUInt32(statBytes, (int)PartyStatOffset.MPMax).ToString();

            TextOverdrive.Text = statBytes[(int)PartyStatOffset.OverdriveLevel].ToString();
            TextOverdriveMax.Text = statBytes[(int)PartyStatOffset.OverdriveMax].ToString();

            TextBaseStrength.Text = statBytes[(int)PartyStatOffset.Strength].ToString();
            TextBaseDefense.Text = statBytes[(int)PartyStatOffset.Defense].ToString();
            TextBaseMagic.Text = statBytes[(int)PartyStatOffset.Magic].ToString();
            TextBaseMagicDef.Text = statBytes[(int)PartyStatOffset.MagicDefense].ToString();
            TextBaseAgility.Text = statBytes[(int)PartyStatOffset.Agility].ToString();
            TextBaseLuck.Text = statBytes[(int)PartyStatOffset.Luck].ToString();
            TextBaseEvasion.Text = statBytes[(int)PartyStatOffset.Evasion].ToString();
            TextBaseAccuracy.Text = statBytes[(int)PartyStatOffset.Accuracy].ToString();
        }

        private void ButtonMaxOverdrive_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || _statsBase <= 0) return;

            try
            {
                switch ((sender as TextBox).Name)
                {
                    case "TextAeonName":
                        var nameBytes = StringConverter.ToFFXBytes(TextAeonName.Text);
                        var writeBuffer = new byte[9];
                        nameBytes.CopyTo(writeBuffer,0);
                        MemoryReader.WriteBytes(_nameBase, writeBuffer);
                        AeonsPanel.UpdateTabs();
                        break;
                    case "TextCurrentHP":
                        MemoryReader.WriteBytes(_statsBase + (int) PartyStatOffset.HPCurrent,
                            BitConverter.GetBytes(uint.Parse(TextCurrentHP.Text)));
                        break;
                    case "TextMaxHP":
                        MemoryReader.WriteBytes(_statsBase + (int)PartyStatOffset.HPMax,
                            BitConverter.GetBytes(uint.Parse(TextMaxHP.Text)));
                        break;
                    case "TextCurrentMP":
                        MemoryReader.WriteBytes(_statsBase + (int)PartyStatOffset.MPCurrent,
                            BitConverter.GetBytes(uint.Parse(TextCurrentMP.Text)));
                        break;
                    case "TextMaxMP":
                        MemoryReader.WriteBytes(_statsBase + (int)PartyStatOffset.MPMax,
                            BitConverter.GetBytes(uint.Parse(TextMaxMP.Text)));
                        break;
                    case "TextOverdrive":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.OverdriveLevel, byte.Parse(TextOverdrive.Text));
                        break;
                    case "TextOverdriveMax":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.OverdriveMax, byte.Parse(TextOverdriveMax.Text));
                        break;
                    case "TextBaseStrength":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.Strength, byte.Parse(TextBaseStrength.Text));
                        break;
                    case "TextBaseDefense":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.Defense, byte.Parse(TextBaseDefense.Text));
                        break;
                    case "TextBaseMagic":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.Magic, byte.Parse(TextBaseMagic.Text));
                        break;
                    case "TextBaseMagicDef":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.MagicDefense, byte.Parse(TextBaseMagicDef.Text));
                        break;
                    case "TextBaseAgility":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.Agility, byte.Parse(TextBaseAgility.Text));
                        break;
                    case "TextBaseLuck":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.Luck, byte.Parse(TextBaseLuck.Text));
                        break;
                    case "TextBaseEvasion":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.Evasion, byte.Parse(TextBaseEvasion.Text));
                        break;
                    case "TextBaseAccuracy":
                        MemoryReader.WriteByte(_statsBase + (int)PartyStatOffset.Accuracy, byte.Parse(TextBaseAccuracy.Text));
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Error parsing input", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
        }
    }
}
