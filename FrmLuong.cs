﻿using System;
using System.Windows.Forms;
using Nhom3_QuanLyNhanSu.Models;
using Nhom3_QuanLyNhanSu.Entities;

namespace Nhom3_QuanLyNhanSu
{
    public partial class FrmLuong : Form
    {
        #region Function
        private infoTab tab = null;
        public ChangeStateButton change;

        private Validate validate = new Validate();

        private bool BeforeInsert = false;


        public void setTab(infoTab tab)
        {
            this.tab = tab;

            this.tab.AddNew = new infoTab.EventButton(Them);
            this.tab.Cancel = new infoTab.EventButton(HuyBo);
            this.tab.Save = new infoTab.EventButton(LuuLai);
            this.tab.Delete = new infoTab.EventButton(Xoa);
            this.tab.Edit = new infoTab.EventButton(Sua);
            this.tab.Refresh = new infoTab.EventButton(CapNhat);
            this.tab.Print = new infoTab.EventButton(In);
        }

        private void changeStateButton(bool b1, bool b2, bool b3, bool b4, bool b5)
        {
            if (!isUpdate)
            {
                tab.Them = b1;
                tab.Sua = b2;
                tab.Luu = b3;
                tab.CapNhat = b4;
                tab.In = false;

                change();
            }
        }

        private void HiddenIconTextBox()
        {
            lblIconMaCV.Visible = false;
            lblIconTenCV.Visible = false;
        }

        private void EnableTextBox(bool b)
        {

            txtLuongCoBan.Enabled = b;
        }

        public void ShowEditAnDeleteButton()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                changeStateButton(true, true, false, true, true);
                EnableTextBox(false);
            }
            HiddenIconTextBox();

        }

        private void ShowDetail(DataGridViewRow row)
        {
            if (row != null && (tab != null && (tab.action == ActionForm.KHONG || tab.action == ActionForm.SUA)) || tab == null || BeforeInsert)
            {
                txtMaLuong.Text = row.Cells[0].Value.ToString();
                txtLuongCoBan.Text = string.Format("{0:0,0}", double.Parse(row.Cells[1].Value.ToString()));
                txtSoNV.Text = row.Cells[2].Value.ToString();
            }
            else {
                clearInsert();
            }
        }

        private void clearInsert() {
            txtMaLuong.Text = "";
            txtLuongCoBan.Text = "";
            txtSoNV.Text = "";
        }

        private void Them()
        {
            changeStateButton(false, false, true, false, true);
            tab.action = ActionForm.THEM;

            EnableTextBox(true);
            clearInsert();
            txtLuongCoBan.Focus();
            btnSearch.Enabled = false;
            dataGridView1.Enabled = false;
        }

        private void HuyBoFull()
        {
            changeStateButton(true, true, false, true, true);
            tab.action = ActionForm.KHONG;

            EnableTextBox(false);


            HiddenIconTextBox();
            BeforeInsert = false;
            btnSearch.Enabled = true;
            dataGridView1.Enabled = true;
        }

        private void HuyBo()
        {
            HuyBoFull();
            ShowDetail(dataGridView1.CurrentRow);
        }

        private void LuuLai()
        {
            switch (tab.action)
            {
                case ActionForm.THEM:
                    validate.Check(new ValidateParam(ValidateType.PRICE, txtLuongCoBan.Text, lblIconTenCV, "Lương phải có kiểu tiền tệ"));

                    if (!validate.Check(lblIconMaCV, lblIconTenCV))
                    {
                        MessageBox.Show("Có lỗi. Không thể lưu");
                        return;
                    }
                    BeforeInsert = true;
                    model.insert(new Luong() { BacLuong = validate.formatPrice(txtLuongCoBan.Text.Trim()) });
                    
                    HuyBoFull();
                    isChangeData = true;
                    
                    if(!isUpdate && lblMessage.Text.Equals("Insert Successfully"))
                    tab.UpdateData(3);
                    break;
                case ActionForm.SUA:
                    validate.Check(new ValidateParam(ValidateType.PRICE, txtLuongCoBan.Text, lblIconTenCV, "Lương phải có kiểu tiền tệ"));

                    if (!validate.Check(lblIconTenCV))
                    {
                        MessageBox.Show("Có lỗi. Không thể lưu");
                        return;
                    }
                    model.update(new Luong() { MaLuong = txtMaLuong.Text, BacLuong = validate.formatPrice(txtLuongCoBan.Text.Trim()), SoNV = int.Parse(txtSoNV.Text) });
                    HuyBoFull();
                    isChangeData = true;
                    if (!isUpdate && lblMessage.Text.EndsWith("row(s) affected"))
                        tab.UpdateData(3);
                    break;
            }
        }

        private void Sua()
        {
            changeStateButton(false, false, true, false, true);
            tab.action = ActionForm.SUA;

            EnableTextBox(true);
            txtLuongCoBan.Focus();
            btnSearch.Enabled = false;
        }

        private void Xoa()
        {
            if (tab.action == ActionForm.KHONG)
            {
                model.delete();
                if (isUpdate && lblMessage.Text.EndsWith("row(s) affected"))
                {
                    tab.UpdateData(3);
                }
            }
        }

        private void CapNhat()
        {
            if (tab.action == ActionForm.KHONG)
            {
                model.getAllData();
                ShowDetail(dataGridView1.CurrentRow);
            }
        }

        private void In()
        {
        }

        #endregion

        private LuongModel model = null;

        public FrmLuong()
        {
            InitializeComponent();
        }

        private bool isUpdate = false;

        public bool isChangeData = false;

        public string CurrentRowSelected = null;

        private void EnableButtonUpdate(bool b) {
            btnLuu.Enabled = !b;
            btnHuy.Enabled = !b;
            btnThem.Enabled = b;
            btnSua.Enabled = b;
        }

        public FrmLuong(string CurrentRowSelected)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            InitializeComponent();

            this.tab = new infoTab();
           

            this.isUpdate = true;
            this.CurrentRowSelected = CurrentRowSelected;
            EnableTextBox(false);
            HiddenIconTextBox();

            btnLuu.Visible = true;
            btnHuy.Visible = true;
            btnThem.Visible = true;
            btnSua.Visible = true;
            btnDong.Visible = true;

            EnableButtonUpdate(true);

            
        }

        private void txtLuongCoBan_Leave(object sender, EventArgs e)
        {
            validate.Check(new ValidateParam(ValidateType.PRICE, txtLuongCoBan.Text, lblIconTenCV, "Lương phải có kiểu tiền tệ"));
        }

        private void FrmLuong_Load(object sender, EventArgs e)
        {
            model = new LuongModel();
            model.setControl(dataGridView1, lblMessage);

            model.getAllData("LUONG_CO_BAN DESC");


            model.ShowDetail(ShowDetail);

            validate.SetTooltip(toolTip1);

            if (CurrentRowSelected != null)
            {
                dataGridView1.ClearSelection();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value.ToString().Equals(CurrentRowSelected))
                    {
                        row.Cells[0].Selected = true;
                    }
                }
                dataGridView1.DoubleClick += new EventHandler(dataGridView1_DoubleClick);
            }

            ShowDetail(dataGridView1.CurrentRow);
        }

        private void CloseForm()
        {
            if (dataGridView1.CurrentRow != null)
                CurrentRowSelected = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            else
                CurrentRowSelected = null;
            this.Dispose();
        }

        void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "LUONG_CO_BAN")
            {
                if (e.Value != null)
                {
                    e.Value = string.Format("{0:0,0}", double.Parse(e.Value.ToString()));
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            model.getAllData("LUONG_CO_BAN DESC", validate.formatPrice(txtKey.Text.Trim()));
            button1.Enabled = true;
            ShowDetail(dataGridView1.CurrentRow);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            model.getAllData("LUONG_CO_BAN DESC");
            button1.Enabled = false;
            ShowDetail(dataGridView1.CurrentRow);
        }

        private void txtKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            LuuLai();
            if (tab.action == ActionForm.KHONG)
                EnableButtonUpdate(true);
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            HuyBo();
            EnableButtonUpdate(true);
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Sua();
            EnableButtonUpdate(false);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
            EnableButtonUpdate(false);
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            CloseForm();
        }
    }
}
