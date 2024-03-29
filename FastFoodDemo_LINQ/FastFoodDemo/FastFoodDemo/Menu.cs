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
using System.IO;

namespace FastFoodDemo
{
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        int ThanhTien = 0;
        int TongGiaSP = 0;
        BLSanPham blSP = new BLSanPham();
        BLHoaDon blHD = new BLHoaDon();
        BLChiTiet_HD blCT = new BLChiTiet_HD();
        PicItem[] listPi = new PicItem[200];
        List<Item> listItem = new List<Item>();
        List<SANPHAM> dsSP = new List<SANPHAM>();
        List<HOADON> dsHD = new List<HOADON>();
        public Image ConvertImage(byte[] b)
        {
            MemoryStream ms = new MemoryStream(b, 0, b.Length);
            ms.Write(b, 0, b.Length);
            return Image.FromStream(ms, true);
        }
        private void Menu_Load(object sender, EventArgs e)
        {
            pnThanhToan.Visible = false;

            dsSP = blSP.dsSanPham();

            for (int i = 0; i < dsSP.Count; i++)
            {
                if (dsSP[i].TT_Ban == false)
                    continue;
                listPi[i] = new PicItem()
                {
                    Tag = dsSP[i].MaSP,
                    TenSP = dsSP[i].TenSP,
                    GiaBan = string.Format("{0:n0}", dsSP[i].GiaBan) + " &đ",
                    PicSP = ConvertImage((byte[])dsSP[i].HinhSP.ToArray()),
                    Count = 0,
                };
                listPi[i].Click += Menu_Click;
                AddControl(listPi[i]);
            }

        }



        public void AddControl(PicItem pi)
        {
            int Ma = (int)pi.Tag;
            if (Ma >= 100 && Ma < 200)
                flpBurger.Controls.Add(pi);
            else if (Ma >= 200 && Ma < 300)
                flpChicken.Controls.Add(pi);
            else if (Ma >= 300 && Ma < 400)
                flpChickenSet.Controls.Add(pi);
            else if (Ma >= 400 && Ma < 500)
                flpCombo.Controls.Add(pi);
            else if (Ma >= 500 && Ma < 600)
                flpValue.Controls.Add(pi);
            else if (Ma >= 600 && Ma < 700)
                flpSet.Controls.Add(pi);
            else if (Ma >= 700 && Ma < 800)
                flpDessert.Controls.Add(pi);
            else
                flpDrink.Controls.Add(pi);
        }

        private void Menu_Click(object sender, EventArgs e)
        {
            pnThanhToan.Visible = true;

            PicItem pi = sender as PicItem;
            pi.Count++;
            listItem.Remove(listItem.Find(i => i.Tag == pi.Tag));

            Item item = new Item()
            {
                Tag = pi.Tag,
                PicSP = pi.PicSP,
                TenSP = pi.TenSP,
                GiaBan = "× " + pi.GiaBan,
                SoLuong = pi.Count,
            };

            listItem.Add(item);

            flpSanPham.Controls.Clear();
            listItem.ForEach(i => flpSanPham.Controls.Add(i));

            TinhTien();

            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].Click += MenuItemClick;
            }
        }

        private void TinhTien()
        {
            TongGiaSP = 0;
            ThanhTien = 0;
            for (int i = 0; i < listItem.Count; i++)
            {
                int value = (int)(listItem[i].Tag);
                SANPHAM sp = dsSP.Find(x => x.MaSP == value);
                ThanhTien += (int)sp.GiaBan * listItem[i].SoLuong;
                TongGiaSP += (int)sp.GiaSP * listItem[i].SoLuong;
            }
            if (ThanhTien == 0)
                pnThanhToan.Visible = false;
            else
                lblThanhTien.Text = "× " + string.Format("{0:n0}", ThanhTien) + " &đ";
        }
        private void MenuItemClick(object sender, EventArgs e)
        {
            flpSanPham.Controls.Clear();
            Item item = sender as Item;
            for (int i = 0; i < dsSP.Count; i++)
            {
                int valuePi = (int)(listPi[i].Tag);
                int valueI = (int)(item.Tag);
                if (valuePi == valueI)
                {
                    listPi[i].Count = 0;
                    break;
                }

            }
            listItem.Remove(item);
            listItem.ForEach(i => flpSanPham.Controls.Add(i));
            TinhTien();
        }

        private int MaxHD()
        {
            dsHD = blHD.dsHoaDon();
            int max = (from hd in dsHD
                       select hd.MaHD).ToList().Max();
            return max;
        }
        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            string message;
            HOADON hd = new HOADON();
            hd.MaHD = MaxHD() + 1;
            hd.TongTien = ThanhTien;
            hd.TongGiaSP = TongGiaSP;
            hd.Ngay = DateTime.Now.Day;
            hd.Thang = DateTime.Now.Month;
            hd.Nam = DateTime.Now.Year;
            hd.TT_HD = true;
            blHD.Insert(hd, out message);

            for (int i = 0; i < listItem.Count; i++)
            {
                int value = (int)(listItem[i].Tag);
                CHITIET_HD ct = new CHITIET_HD();
                ct.MaHD = hd.MaHD;
                ct.MaSP = value;
                ct.SoLuong = listItem[i].SoLuong;
                blCT.Insert(ct, out message);
            }
            MessageBox.Show(message);
            listItem.Clear();
            flpSanPham.Controls.Clear();
            for (int i = 0; i < dsSP.Count; i++)
            {
                if (listPi[i] == null)
                    break;
                listPi[i].Count = 0;
            }
            pnThanhToan.Visible = false;
        }
    }
}
