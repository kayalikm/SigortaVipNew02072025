using System;
using System.Collections.Generic;
using System.Data;

namespace SigortaVip.Models
{
    internal class getAracTipList
    {
        dal mydal = new dal();

        public List<aracTip> getTipList()
        {
            List<aracTip> aracTips = new List<aracTip>();

            string sql = @"SELECT
                         [TipKodu]
                         ,[TipAdı]
                         FROM [sigortavipserver].[dbo].[Smarka] ";

            DataSet data = mydal.CommandExecuteReader(sql,mydal.myConnection);

            foreach (DataRow item in data.Tables[0].Rows)
            {
                aracTips.Add(new aracTip
                {
                    tipAdi = item["TipAdı"].ToString(),
                    tipKod = Convert.ToInt16(item["TipKodu"].ToString())
                });
            }
            return aracTips;
        }
        public List<aracTip> getTipListByMarka(string markaKodu)
        {
            List<aracTip> aracTips = new List<aracTip>();

            string sql = @"SELECT
                         [TipKodu]
                         ,[TipAdı]
                         FROM [sigortavipserver].[dbo].[Smarka]
                         Where MarkaKodu =" +markaKodu;
            DataSet data = mydal.CommandExecuteReader(sql, mydal.myConnection);

            foreach (DataRow item in data.Tables[0].Rows)
            {
                aracTips.Add(new aracTip
                {
                    tipAdi = item["TipAdı"].ToString(),
                    tipKod = Convert.ToInt16(item["TipKodu"].ToString())
                });
            }
            return aracTips;
        }
        public aracTip getTipListByMarkaVeTipKodu(string markaKodu,string tipKodu)
        {
            aracTip aracTip = new aracTip();

            string sql = @"SELECT *
                         FROM [sigortavipserver].[dbo].[Smarka]
                         Where MarkaKodu =" + markaKodu +
                         "and TipKodu =" + tipKodu;
            DataSet data = mydal.CommandExecuteReader(sql, mydal.myConnection);

            try
            {

                aracTip.markaAdi = data.Tables[0].Rows[0]["MarkaAdı"].ToString();
                aracTip.tipKod = Convert.ToInt16(data.Tables[0].Rows[0]["TipKodu"].ToString());
                aracTip.tipAdi = data.Tables[0].Rows[0]["TipAdı"].ToString();
            }
            catch (Exception)
            {
                return new aracTip();
            }

            return aracTip;
        }
    }

}
