﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FastFoodDemo.BL_Layer;

namespace FastFoodDemo
{
    public partial class Products : UserControl
    {
        public Products()
        {
            InitializeComponent();
        }

        BLSanPham blSP = new BLSanPham();
        public List<SANPHAM> dsSP;
        private void Products_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            blSP = new BLSanPham();
            dsSP = blSP.dsSanPham();
            dgvSanPham.Rows.Clear();
            for (int i = 0; i < dsSP.Count; i++)
            {
                if (dsSP[i].TT_Ban == true)
                    dgvSanPham.Rows.Add(dsSP[i].MaSP, dsSP[i].TenSP, dsSP[i].GiaSP, dsSP[i].GiaBan);
            }
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            if (txtFind.Text == "")
                LoadData();
            else
            {
                dgvSanPham.Rows.Clear();
                for (int i = 0; i < dsSP.Count; i++)
                {
                    if (dsSP[i].TenSP.Contains(txtFind.Text)&& dsSP[i].TT_Ban == true)
                        dgvSanPham.Rows.Add(dsSP[i].MaSP, dsSP[i].TenSP, dsSP[i].GiaSP, dsSP[i].GiaBan);
                }
            }
        }

        int rowselect = 0;

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            rowselect = e.RowIndex;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.SelectedRows.Count > 0)
            {
                if (rowselect == -1 || rowselect >= dsSP.Count)
                    return;
                int vt = 0;
                for (int i = 0; i < dsSP.Count; i++)
                {
                    if (dsSP[i].MaSP == (int)(dgvSanPham.Rows[rowselect].Cells[0].Value))
                    {
                        vt = i;
                        break;
                    }
                }
                DataGridViewRow row = dgvSanPham.Rows[vt];
                int Ma = int.Parse(row.Cells[0].Value.ToString());
                SANPHAM sp = dsSP.Find(x => x.MaSP == Ma);
                DetailProduct detail = new DetailProduct(sp);
                var result = detail.ShowDialog();
                LoadData();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DetailProduct detail = new DetailProduct(true);
            var result = detail.ShowDialog();
            LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.SelectedRows.Count > 0)
            {
                if (rowselect == -1 || rowselect >= dsSP.Count)
                    return;
                int vt = 0;
                for (int i = 0; i < dsSP.Count; i++)
                {
                    if (dsSP[i].MaSP == (int)(dgvSanPham.Rows[rowselect].Cells[0].Value))
                    {
                        vt = i;
                        break;
                    }
                }
                DialogResult dialog = MessageBox.Show("Bạn có muốn xoá không?", "Xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    string message;
                    dsSP[vt].TT_Ban = false;
                    bool result = blSP.Delete(dsSP[vt], out message);
                    if (result == false)
                        MessageBox.Show(message);
                    LoadData();
                }
            }
        }
    }
}
