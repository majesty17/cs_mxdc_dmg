using static System.Net.Mime.MediaTypeNames;

namespace cs_mxdc_dmg
{
    public partial class Form_main : Form
    {
        private bool initOK = false;


        public Form_main()
        {
            InitializeComponent();
        }

        // 
        private void Form_main_Load(object sender, EventArgs e)
        {
            // 初始化各种数值
            numericUpDown_4w_li.Value = 4;
            numericUpDown_4w_min.Value = 4;
            numericUpDown_4w_yun.Value = 200;
            numericUpDown_4w_zhi.Value = 4;

            numericUpDown_watk_wp.Value = 40;
            comboBox_watk_biao.SelectedIndex = 4;
            numericUpDown_watk_shoutao.Value = 2;

            numericUpDown_watk_link.Value = 1;
            numericUpDown_watk_poi.Value = 0;
            numericUpDown_me_lvl.Value = 30;
            checkBox_ski_shad.Checked = false;

            linkLabel_monster.LinkClicked += (s, args) =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://mxdc.dvg.cn/mob_list.php",
                    UseShellExecute = true
                });
            };

            initOK = true;
        }


        
        private void cal(object sender, EventArgs e)
        {
            if (!initOK) return;

            int w4_li = (int)numericUpDown_4w_li.Value;
            int w4_min = (int)numericUpDown_4w_min.Value;
            int w4_yun = (int)numericUpDown_4w_yun.Value;
            int w4_zhi = (int)numericUpDown_4w_zhi.Value;
            
            int watk_wp = (int)numericUpDown_watk_wp.Value;            int watk_biao = -1;
            string text = comboBox_watk_biao.Text;            int start = text.LastIndexOf('(');
            int end = text.LastIndexOf(')');

            if (start >= 0 && end > start)
            {
                watk_biao = int.Parse(text.Substring(start + 1, end - start - 1));
            }
            else
            {
                MessageBox.Show("请选择正确的飞镖类型！");
            }



                int watk_shoutao = (int)numericUpDown_watk_shoutao.Value;

            int watk_link = (int)numericUpDown_watk_link.Value;
            int watk_poi = (int)numericUpDown_watk_poi.Value;
            
            int me_lvl = (int)numericUpDown_me_lvl.Value;
            bool ski_shad = checkBox_ski_shad.Checked;

            // 总物理攻击加成
            int watk_all = watk_wp + watk_shoutao + watk_link + watk_biao + watk_poi;

            double atk_panel_max = (3.6 * w4_yun + w4_min + w4_li) / 100.0 * watk_all;
            double atk_panel_min = (3.2 * w4_yun * 0.6 + w4_min + w4_li) / 100.0 * watk_all;


            label_out_panel.Text = $"面板攻击: {Math.Ceiling(atk_panel_min)} ~ {Math.Ceiling(atk_panel_max)}";


            // 下面计算实际伤害

            int mon_wdef = (int)numericUpDown_mon_wdef.Value;
            int mon_hp = (int)numericUpDown_mon_hp.Value;
            int mon_lvl = (int)numericUpDown_mon_lvl.Value;

            // 计算原始攻击力范围
            double atk_raw_min = w4_yun * watk_all * 2.5 / 100;
            double atk_raw_max = w4_yun * watk_all * 5.0 / 100;

            // 计算等级差系数
            int lvl_diff = Math.Max(0, mon_lvl - me_lvl);
            double lvl_coeff = 1 - lvl_diff * 0.01  ;


            // 计算原始目标伤害，1.0是属性克制；
            double aim_dmg_min = atk_raw_min * 1.0 * lvl_coeff - Math.Floor(mon_wdef * 0.6);
            double aim_dmg_max = atk_raw_max * 1.0 * lvl_coeff - Math.Floor(mon_wdef * 0.5);

            double skill_multiplier = 1.5; // 技能倍率，假设为1.5倍
            double crit_multiplier = 1.0;  // 暴击倍率，假设为1.0倍

            // 计算最终伤害
            double final_dmg_min = aim_dmg_min * skill_multiplier;
            double final_dmg_max = aim_dmg_max * skill_multiplier;

            double final_dmg_crit_min = aim_dmg_min * (skill_multiplier + crit_multiplier);
            double final_dmg_crit_max = aim_dmg_max * (skill_multiplier + crit_multiplier);

            string out_dmg = $"实际伤害：\n单段普通伤害: {Math.Floor(final_dmg_min)} ~ {Math.Floor(final_dmg_max)}\n" +
                $"单段暴击伤害: {Math.Floor(final_dmg_crit_min)} ~ {Math.Floor(final_dmg_crit_max)}\n" +
                $"双飞普通伤害: {Math.Floor(final_dmg_min) * 2} ~ {Math.Floor(final_dmg_max) * 2}\n" +
                $"双飞暴击伤害: {Math.Floor(final_dmg_crit_min * 2)} ~ {Math.Floor(final_dmg_crit_max) * 2}\n";

            int all_min = (int)Math.Floor(final_dmg_min) * 2;
            int all_max = (int)Math.Floor(final_dmg_crit_max) * 2;

            // 如果影子分身技能被选中，计算影子分身的伤害
            if (ski_shad)
            {
                out_dmg = out_dmg + $"影子分身普通伤害: {Math.Floor(final_dmg_min) * 3} ~ {Math.Floor(final_dmg_max) * 3}\n" +
                    $"影子分身暴击伤害: {Math.Floor(final_dmg_crit_min * 3)} ~ {Math.Floor(final_dmg_crit_max * 3)}\n";
                all_min = (int)Math.Floor(final_dmg_min) * 3;
                all_max = (int)Math.Floor(final_dmg_crit_max) * 3;
            }

            out_dmg =out_dmg + $"总伤害范围: {all_min} ~ {all_max}";


            label_out_dmg.Text = out_dmg;
        }
    }
}
