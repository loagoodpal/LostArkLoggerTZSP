using System;
using System.Collections.Generic;
using Advanced_Combat_Tracker;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;

[assembly: AssemblyTitle("Lost Ark Plugin")]
[assembly: AssemblyDescription("Lost Ark DPS plugin")]
[assembly: AssemblyCopyright("shalzuth")]
[assembly: AssemblyVersion("0.0.0.1")]


namespace MLParsing_Plugin
{
    public class LostArk_Plugin : UserControl, IActPluginV1
    {

        #region Designer Created Code (Avoid editing)

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LostArk_Plugin
            // 
            this.Name = "LostArk_Plugin";
            this.Size = new System.Drawing.Size(75, 68);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        public LostArk_Plugin()
        {
            InitializeComponent();
        }
        private DateTime curActionTime = DateTime.MinValue;
        Label lblStatus = null;
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            ActGlobals.oFormActMain.LogPathHasCharName = false;
            ActGlobals.oFormActMain.LogFileFilter = "LostArk*.log";
            ActGlobals.oFormActMain.LogFileParentFolderName = "LostArkLogs";
            try { ActGlobals.oFormActMain.ResetCheckLogs(); } catch { }
            ActGlobals.oFormActMain.TimeStampLen = 19;
            ActGlobals.oFormActMain.GetDateTimeFromLog = new FormActMain.DateTimeLogParser(ParseDateTime);
            ActGlobals.oFormActMain.BeforeLogLineRead += new LogLineEventDelegate(oFormActMain_BeforeLogLineRead);
            RenameDamageTypes();

            // Set status text to successfully loaded
            lblStatus = pluginStatusText;
            lblStatus.Text = "Lost Ark ACT plugin loaded";
        }
        private void RenameDamageTypes()
        {
            //CombatantData.ColumnDefs.Clear();
            CombatantData.OutgoingDamageTypeDataObjects = new Dictionary<string, CombatantData.DamageTypeDef>
            {
			    {"Outgoing Damage", new CombatantData.DamageTypeDef("Outgoing Damage", 0, Color.Orange)},
                {"Healed (Out)", new CombatantData.DamageTypeDef("Healed (Out)", 1, Color.Blue)},
                {"Shielded (Out)", new CombatantData.DamageTypeDef("Shielded (Out)", 1, Color.Teal)},
                {"All Outgoing (Ref)", new CombatantData.DamageTypeDef("All Outgoing (Ref)", 0, Color.Black)}
            };
            CombatantData.IncomingDamageTypeDataObjects = new Dictionary<string, CombatantData.DamageTypeDef>
            {
                {"Incoming Damage", new CombatantData.DamageTypeDef("Incoming Damage", -1, Color.Red)},
                {"Healed (Inc)",new CombatantData.DamageTypeDef("Healed (Inc)", 1, Color.LimeGreen)},
                {"Shielded (Inc)",new CombatantData.DamageTypeDef("Healed (Inc)", 1, Color.GreenYellow)},
                {"All Incoming (Ref)",new CombatantData.DamageTypeDef("All Incoming (Ref)", 0, Color.Black)}
            };
            CombatantData.SwingTypeToDamageTypeDataLinksOutgoing = new SortedDictionary<int, List<string>>
            {
                {(int) SwingTypeEnum.Melee, new List<string> { "Outgoing Damage" } },
                {(int) SwingTypeEnum.Healing, new List<string> { "Healed (Out)" } },
                {(int) SwingTypeEnum.PowerHealing, new List<string> { "Shielding (Out)" } },
            };
            CombatantData.SwingTypeToDamageTypeDataLinksIncoming = new SortedDictionary<int, List<string>>
            {
                {(int) SwingTypeEnum.Melee, new List<string> { "Incoming Damage" } },
                {(int) SwingTypeEnum.Healing, new List<string> { "Healed (Inc)" } },
                {(int) SwingTypeEnum.PowerHealing, new List<string> { "Shielding (Inc)" } },
            };

            

            CombatantData.DamageSwingTypes = new List<int> { 1, 2 };
            CombatantData.HealingSwingTypes = new List<int> { 3 };

            CombatantData.DamageTypeDataNonSkillDamage = "Auto-Attack (Out)";
            CombatantData.DamageTypeDataOutgoingDamage = "Outgoing Damage";
            CombatantData.DamageTypeDataOutgoingHealing = "Healed (Out)";
            CombatantData.DamageTypeDataIncomingDamage = "Incoming Damage";
            CombatantData.DamageTypeDataIncomingHealing = "Healed (Inc)";
            if (EncounterData.ColumnDefs.ContainsKey("PowerDrain"))
                EncounterData.ColumnDefs.Remove("PowerDrain");
            if (MasterSwing.ColumnDefs.ContainsKey("PowerDrain"))
                MasterSwing.ColumnDefs.Remove("PowerDrain");
            ActGlobals.oFormActMain.ValidateLists();
            ActGlobals.oFormActMain.ValidateTableSetup();
        }



        UInt64 currentcharID = 0xdeadbeef;
        void oFormActMain_BeforeLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            if (logInfo.detectedTime != curActionTime) curActionTime = logInfo.detectedTime;

            var split = logInfo.logLine.Split(new String[] { "|" }, StringSplitOptions.None);
            var eventType = Int16.Parse(split[0]);
            try
            {
                switch (eventType)
                {
                    case 1:
                        var CharSwitch = new CharSwitch(logInfo);
                        currentcharID = CharSwitch.charID;
                        break;
                    case 3:
                        var Ally = new AllyParse(logInfo);
                        if (Ally.charID == currentcharID)
                        {
                            ActGlobals.charName = Ally.name;
                        }
                        break;
                    case 5:
                        var Death = new DeathParse(logInfo);
                        ProcessDeathAction(Death);
                        break;
                    case 8:
                        var Dam = new DamageParse(logInfo);
                        ProcessCombatAction(Dam);
                        break;
                    case 9:
                        var Heal = new HealParse(logInfo);
                        ProcessHealAction(Heal);
                        break;
                    case 10:
                        var Shield = new ShieldParse(logInfo);
                        ProcessShieldAction(Shield);
                        break;
                    case 11:
                        var Counter = new CounterParse(logInfo);
                        ProcessCounterAction(Counter);
                        break;
                    case 12:
                        var Zone = new ZoneParse(logInfo);
                        ActGlobals.oFormActMain.ChangeZone(Zone.zone);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) { ActGlobals.oFormActMain.WriteExceptionLog(e, logInfo.logLine); };
        }

        private void ProcessCounterAction(CounterParse parse)
        {
            parse.logInfo.detectedType = Color.Gray.ToArgb();
            if (ActGlobals.oFormActMain.SetEncounter(parse.logInfo.detectedTime, parse.attacker, parse.victim))
            {
                ActGlobals.oFormActMain.AddCombatAction((int)SwingTypeEnum.Melee, false, "Counter", parse.attacker, "Counter", 1, parse.logInfo.detectedTime, parse.ts, parse.victim, "Counter");
            }
        }
        private void ProcessShieldAction(ShieldParse parse)
        {
            if (parse.amount > 0)
            {
                parse.logInfo.detectedType = Color.Gray.ToArgb();
                if (ActGlobals.oFormActMain.SetEncounter(parse.logInfo.detectedTime, parse.attacker, parse.victim))
                {
                    ActGlobals.oFormActMain.AddCombatAction((int)SwingTypeEnum.Healing, false, "Shield", parse.attacker, parse.ability, parse.amount, parse.logInfo.detectedTime, parse.ts, parse.victim, "");
                }
            }
        }
        private void ProcessHealAction(HealParse parse)
        {
            parse.logInfo.detectedType = Color.Gray.ToArgb();
            if (ActGlobals.oFormActMain.SetEncounter(parse.logInfo.detectedTime, parse.name, parse.name))
            {
                MasterSwing ms = new MasterSwing((int)SwingTypeEnum.Healing, false, "Heal", parse.heal, parse.logInfo.detectedTime, parse.ts, "Healing", parse.name, "Hitpoints", parse.name);
                ActGlobals.oFormActMain.AddCombatAction(ms);
            }
        }

        private void ProcessDeathAction(DeathParse parse)
        {
            parse.logInfo.detectedType = Color.Gray.ToArgb();
            if (ActGlobals.oFormActMain.SetEncounter(parse.logInfo.detectedTime, parse.attacker, parse.victim))
            {
                MasterSwing ms = new MasterSwing((int)SwingTypeEnum.Melee, false, Dnum.Death, parse.logInfo.detectedTime, parse.ts, "Death", parse.attacker, "", parse.victim);
                ActGlobals.oFormActMain.AddCombatAction(ms);
            }
        }
        private void ProcessCombatAction(DamageParse l)
        {
            l.logInfo.detectedType = Color.Gray.ToArgb();
            if (ActGlobals.oFormActMain.SetEncounter(l.logInfo.detectedTime, l.attacker, l.victim))
            {
                ActGlobals.oFormActMain.AddCombatAction((int)SwingTypeEnum.Melee, l.critical, "", l.attacker, l.ability, l.damage, l.logInfo.detectedTime, l.ts, l.victim, l.back ? "Back" : l.front ? "Front" : "");
            }

        }
        private DateTime ParseDateTime(string FullLogLine)
        {
            string[] logs = FullLogLine.Split('|');
            return DateTime.ParseExact(logs[1], "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
        }
        public void DeInitPlugin()
        {
            ActGlobals.oFormActMain.BeforeLogLineRead -= oFormActMain_BeforeLogLineRead;
            lblStatus.Text = "Lost Ark ACT plugin unloaded";
        }
    }

    internal class CharSwitch
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public UInt64 charID;

        public CharSwitch(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            charID = UInt64.Parse(split[2], System.Globalization.NumberStyles.HexNumber);
        }
    }
    internal class ZoneParse
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public string zone;

        public ZoneParse(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            zone = split[2];
        }
    }

    internal class AllyParse
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public String name, charClass;
        public int level, gearLevel;
        public UInt64 charID;

        public AllyParse(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            name = split[3];
            charID = UInt64.Parse(split[2], System.Globalization.NumberStyles.HexNumber);
            //charClass = split[5];
            //level = Int32.Parse(split[6]);
            //gearLevel = Int32.Parse(split[7]);
        }

    }
    internal class CounterParse
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public String attacker, victim;
        public CounterParse(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            victim = split[3];
            attacker = split[5];
        }
    }
    internal class DeathParse
    {

        public LogLineEventArgs logInfo;
        public int ts;
        public String attacker, victim;

        public DeathParse(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            attacker = split[5];
            victim = split[3];
        }
    }
    internal class ShieldParse
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public String attacker, victim, ability;
        public Int32 amount;

        public ShieldParse(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            attacker = split[3];
            ability = split[5];
            victim = split[8];
            amount = Int32.Parse(split[9]);
        }
    }
    internal class HealParse
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public String name;
        public Int32 heal, currentHP;

        public HealParse(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            name = split[3];
            heal = Int32.Parse(split[4]);
            currentHP = Int32.Parse(split[5]);
        }
    }
    internal class DamageParse
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public String attacker, victim, ability;
        public Boolean critical, back, front;
        public Int32 damage;
        [Flags]
        public enum DamageModifierFlags // 0b**FBKD*C with F: front attack, B: Back attack, K: bleed crit (dots ?), D: bleed not crit (dots ?), C: crit
        {
            None = 0,
            SkillCrit = 1,
            UnkModifier1 = 2,
            DotNoCrit = 4,
            DotCrit = 8,
            BackAttack = 0x10,
            FrontAttack = 0x20,
            UnkModifier2 = 0x40,
            UnkModifier3 = 0x80
        }
        public DamageParse(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split('|');
            attacker = split[3];
            victim = split[9];
            ability = split[5];
            try
            {
                damage = Int32.Parse(split[10]);
            }
            catch
            {
                damage = 0;
            }
            var modifier = Int16.Parse(split[11], System.Globalization.NumberStyles.HexNumber);
            critical =
                    ((DamageModifierFlags)modifier &
                     (DamageModifierFlags.DotCrit |
                      DamageModifierFlags.SkillCrit)) > 0;
            back = ((DamageModifierFlags)modifier & (DamageModifierFlags.BackAttack)) > 0;
            front = ((DamageModifierFlags)modifier & (DamageModifierFlags.FrontAttack)) > 0;
        }
    }

    internal class ParsedLine
    {
        public LogLineEventArgs logInfo;
        public int ts;
        public String attacker, victim, ability;
        public Boolean critical, back, front;
        public Int32 damage;

        public ParsedLine(LogLineEventArgs log)
        {
            logInfo = log;
            ts = ++ActGlobals.oFormActMain.GlobalTimeSorter;
            var split = logInfo.logLine.Split(new String[] { "|" }, StringSplitOptions.None);
            attacker = split[1];
            victim = split[2];
            ability = split[3];
            damage = Int32.Parse(split[4]);
            critical = split[5] == "1" ? true : false;
            back = split[6] == "1" ? true : false;
            front = split[7] == "1" ? true : false;
        }
    }
}