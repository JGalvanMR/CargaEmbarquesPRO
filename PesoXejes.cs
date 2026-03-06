using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Lang;
using System.Data.SqlClient;
using System.Data;
using Android.Content;
using Android.Net.Wifi;
using Android.Text;
using Android.Views.InputMethods;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Java.Util;
using Org.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System;
using Android;
using Java.Net;
using Plugin.DeviceInfo;
using Android.Net;
using CargaEmbarques.Modal;
using System.Globalization;
using Math = System.Math;
using static Android.Hardware.Camera;

namespace CargaEmbarques
{
    [Activity(Label = "DETALLE PESO X EJES", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class PesoXejes : Activity
    {
        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        SqlDataAdapter da;
        DataSet ds = new DataSet();
        SqlCommand cmnd = new SqlCommand();
        SqlCommand cmnd1 = new SqlCommand();
        SqlDataReader reader1;
        public static string numerotrailer, fechatrailerx, emb_tipo;
        string query = "", prod_clave = "", folio = "", tipo = "", cadena = "", prod_nombre = "";
        int tarima = 0, caja = 0, tarimaf = 0;
        bool find = false;
        ArrayAdapter<System.String> comboAdapter;
        System.String[] strFrutas;
        public string tb_tabla = "tb_mstr_pedidos_nal";
        public string tipoembarque = "NAL";

        decimal Peso = 0;
        decimal pesoinicialeje1 = 0;
        decimal pesoinicialeje2 = 0;
        decimal pesoinicialeje3 = 0;

        //decimal largoTrailer = 0;
        decimal numeroEjes = 5;

        DataTable PesoProd = new DataTable("PesoProd");
        DataTable TbCar = new DataTable("TbCar");

        //INFORMACION PARA LA CANCELACION DE LA TARIMA ACTUAL, QUITAR LINEA
        public string recibocancelar = "";
        public string productocancelar = "";
        public string tarimacancelar = "";
        public string tiporecibocancelar = "";
        public string cajascancelar = "";
        public string seccioncancelar = "";
        public string Normalcancelar = "";


        TextView placas;
        TextView fecha;
        TextView pedorigen;
        TextView pesototalproducto;
        TextView pesototaltrailer;


        EditText et;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PesoXEjes);

            string producto = "", tipo = "", recibo = "", cajas = "", seccion = "";
            string contenido = "";
            string cadena = "";
            //Declaracion de los id de cada elemento
            //placas = FindViewById<TextView>(Resource.Id.trailerpla);
            //fecha = FindViewById<TextView>(Resource.Id.fectrai);
            //pedorigen = FindViewById<TextView>(Resource.Id.pediori);
            pesototalproducto = FindViewById<TextView>(Resource.Id.pestotalcarga);
            pesototaltrailer = FindViewById<TextView>(Resource.Id.pestotaltrailer);


            numerotrailer = Intent.GetStringExtra("no_trailer");
            fechatrailerx = Intent.GetStringExtra("horatrailer");

            thisConnection.Open();
            cadena = "SELECT pesoEje1, pesoEje2, pesoEje3  FROM tb_mstr_trailer WHERE no_trailer = '" + numerotrailer + "' AND hora_trailer = '" + fechatrailerx + "'";
            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                pesoinicialeje1 = Convert.ToDecimal(Info["pesoEje1"].ToString().Trim());
                pesoinicialeje2 = Convert.ToDecimal(Info["pesoEje2"].ToString().Trim());
                pesoinicialeje3 = Convert.ToDecimal(Info["pesoEje3"].ToString().Trim());
            }
            thisConnection.Close();


            placas.Text = "TRAILER: " + numerotrailer;
            fecha.Text = "FECHA: " + fechatrailerx;

            pedorigen.Text = " ";

            thisConnection.Open();
            cadena = "SELECT a.prod_clave, a.prod_nombre, a.prod_presentacion, B.env_peso FROM tb_cat_producto A, tb_cat_envases B WHERE A.prod_presentacion = b.env_clave";
            SqlDataAdapter da = new SqlDataAdapter(cadena, thisConnection);
            DataSet ds = new DataSet();
            da.Fill(ds, "PesoProd");
            PesoProd = ds.Tables["PesoProd"];

            TbCar.Columns.Add("Prod_Clave");
            TbCar.Columns.Add("tipo");
            TbCar.Columns.Add("recibo");
            TbCar.Columns.Add("cajas");
            TbCar.Columns.Add("seccion");
            thisConnection.Close();

            DataRow row = TbCar.NewRow();
            thisConnection.Open();
            cadena = "SELECT A.prod_clave, A.cajas, A.tipo_rec, A.recibo, A.seccion FROM tb_det_embarque AS A INNER JOIN tb_mstr_embarque AS B ON A.emb_folio = B.emb_folio AND A.emb_tipo = B.emb_tipo WHERE A.Estatus != 'C' AND B.no_trailer = '" + numerotrailer + "' AND B.hora_trailer = '" + fechatrailerx + "' ORDER BY A.seccion";
            cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                row = TbCar.NewRow();
                row["Prod_Clave"] = Info["prod_clave"].ToString().Trim();
                row["tipo"] = Info["tipo_rec"].ToString().Trim();
                row["recibo"] = Info["recibo"].ToString().Trim();
                row["cajas"] = Info["cajas"].ToString().Trim();
                row["seccion"] = Info["seccion"].ToString().Trim();
                TbCar.Rows.Add(row);
            }
            thisConnection.Close();

            decimal Pesoejes = 0;
            decimal pesototal = 0;
            int posiciones = 0;
            int posicionprimer14 = 0;
            int posicionanterior = 0;
            for (int i = 0; i < TbCar.Rows.Count; i++)
            {
                decimal mKilos = 0;
                producto = TbCar.Rows[i]["Prod_Clave"].ToString();
                tipo = TbCar.Rows[i]["tipo"].ToString();
                recibo = TbCar.Rows[i]["recibo"].ToString();
                cajas = TbCar.Rows[i]["cajas"].ToString();
                seccion = TbCar.Rows[i]["seccion"].ToString();
                if (posicionanterior != Convert.ToInt32(seccion))
                {
                    posicionanterior = Convert.ToInt32(seccion);
                    posiciones = posiciones + 1;
                }
                if (tipo.Trim() == "PTC")
                {
                    thisConnection.Open();
                    cadena = "SELECT A.rptd_peso_bruto, A.rptd_tara, A.rptd_cantidad, B.ENV_PESO,C.RPT_FECHA FROM tb_det_recepcion_pt A, tb_cat_envases B, TB_MSTR_RECEPCION_PT C  WHERE  A.rptd_estatus != 'C' AND A.RPT_RECIBO = '" + recibo + "' AND A.PROD_CLAVE = '" + producto + "' AND A.ENV_CLAVE = B.ENV_CLAVE AND A.RPT_RECIBO = C.RPT_RECIBO ";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        mKilos = mKilos + ((Convert.ToDecimal(Info["rptd_peso_bruto"].ToString().Trim()) - Convert.ToDecimal(Info["rptd_tara"].ToString().Trim())) / Convert.ToDecimal(Info["rptd_cantidad"].ToString().Trim())) * Convert.ToDecimal(cajas);
                        DateTime mF = DateTime.ParseExact(fechatrailerx.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (Convert.ToString(producto.Trim()) == "02002ML00" || Convert.ToString(producto.Trim()) == "02002BROFR" || Convert.ToString(producto.Trim()) == "02BRCO2025")
                        {
                            mKilos = mKilos + (Fn_PesoHielo(Convert.ToDecimal(8.5), Convert.ToDateTime(Info["RPT_FECHA"].ToString().Trim()), mF) * Convert.ToInt32(cajas));
                        }
                        else if (producto.ToString().Trim() == "02002BRHEB")
                        {
                            mKilos = mKilos + Fn_PesoHielo(Convert.ToDecimal(4), Convert.ToDateTime(Info["RPT_FECHA"].ToString().Trim()), mF);
                        }

                    }
                    thisConnection.Close();

                }
                else
                {
                    thisConnection.Open();
                    cadena = "SELECT B.PROD_CLAVE, C.PROD_NOMBRE, B.PROD_PESO_VAR,B.FODP_UNIDADES,C.PROD_PRESENTACION,D.ENV_PESO,E.HRP_PESO_NETO,E.HRP_NUM_UNIDADES,E.hrp_fecha FROM TB_DET_FINAL_ODP B, TB_CAT_PRODUCTO C,tb_cat_envases D, TB_HIST_RECEPCION E  WHERE B.PROD_CLAVE = '" + producto + "' AND B.ORDP_FOLIO = '" + recibo + "' AND B.PROD_CLAVE = C.PROD_CLAVE AND C.PROD_PRESENTACION = D.ENV_CLAVE AND B.ORDP_FOLIO = E.hrp_recibo AND B.PROD_CLAVE = E.PROD_CLAVE";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        decimal Mpe = 0;
                        if (Convert.ToDecimal(Info["PROD_PESO_VAR"].ToString().Trim()) > 0)
                        {
                            Mpe = Convert.ToDecimal(Info["PROD_PESO_VAR"]) * Convert.ToInt32(cajas) + (Convert.ToDecimal(Convert.ToDecimal(Info["ENV_PESO"])) * Convert.ToInt32(cajas));
                        }
                        else
                        {
                            Mpe = Convert.ToDecimal(Info["ENV_PESO"]) * Convert.ToInt32(cajas) + (Convert.ToDecimal(Info["HRP_PESO_NETO"]) / Convert.ToDecimal(Info["HRP_NUM_UNIDADES"]) * Convert.ToInt32(cajas));
                        }

                        decimal Muni = 0;
                        mKilos = mKilos + Mpe;
                        DateTime xcs = DateTime.ParseExact(fechatrailerx.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (producto == "02002ML00" || producto == "02002BROFR" || producto == "02BRCO2025")
                        {
                            xcs = DateTime.ParseExact(fechatrailerx.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture); ;
                            mKilos = mKilos + (Fn_PesoHielo(Convert.ToDecimal(8.5), Convert.ToDateTime(Info["hrp_fecha"]), xcs) * Convert.ToInt32(cajas));
                        }
                        else
                        {
                            mKilos = mKilos + Fn_PesoHielo(Convert.ToDecimal(4), Convert.ToDateTime((Info["hrp_fecha"])), xcs);
                        }
                    }
                    thisConnection.Close();
                }
                Pesoejes = Pesoejes + mKilos;
                pesototal = pesototal + mKilos;

                if (Convert.ToInt32(seccion) == 14)
                {
                    listItem.Add(new FlimStarInfo()
                    {
                        Name = "PESO EJE NUMERO 1:",
                        Age = Convert.ToString(Math.Round(Pesoejes + (20 * posiciones), 2)) + " | " + Convert.ToString(Math.Round(Pesoejes + (20 * posiciones) + pesoinicialeje2, 2)),
                        ImageID = Resource.Drawable.pesoEje1
                    });
                    posicionprimer14 = posiciones;
                    Pesoejes = 0;
                }
            }
            listItem.Add(new FlimStarInfo()
            {
                Name = "PESO EJE NUMERO 2:",
                Age = Convert.ToString((Math.Round(Pesoejes + (20 * (posiciones - posicionprimer14)), 2))) + " | " + Convert.ToString(Math.Round(Pesoejes + (20 * (posiciones - posicionprimer14)) + pesoinicialeje3, 2)),
                ImageID = Resource.Drawable.pesoeje2
            });
            pesototalproducto.Text = "PESO TOTAL: " + Math.Round(pesototal + (20 * posiciones), 2);
            pesototaltrailer.Text = "PESO TOTAL TRAILER: " + Math.Round(pesototal + (20 * posiciones) + pesoinicialeje2 + pesoinicialeje3, 2);


            List<FlimStarInfo> lstFlimStar = listItem;
            var gvObject = FindViewById<GridView>(Resource.Id.infopesos);
            gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);
        }

        private decimal Fn_PesoHielo(decimal peso, DateTime FecRec, DateTime FecEmb)
        {
            int Mdia = FecEmb.Subtract(FecRec).Days;
            decimal Mpeso = 0;
            switch (Mdia)
            {
                case 0:
                    Mpeso = peso;
                    break;
                case 1:
                    Mpeso = (peso * 85) / 100;
                    break;
                case 2:
                    Mpeso = (peso * 75) / 100;
                    break;
                case 3:
                    Mpeso = (peso * 50) / 100;
                    break;
                case 4:
                    Mpeso = (peso * 35) / 100;
                    break;
                case 5:
                    Mpeso = (peso * 20) / 100;
                    break;
                case 6:
                    Mpeso = (peso * 10) / 100;
                    break;
            }

            return Mpeso;
        }

        List<FlimStarInfo> listItem = new List<FlimStarInfo>();

        private decimal getInfoTrailer(string numerotrailer, string fechatrailerx, string emb_tipo)
        {
            decimal lengthAxles = 0;
            if (emb_tipo == "NAL")
            {
                thisConnection.Open();
                cadena = "SELECT largo FROM tb_mstr_trailer WHERE no_trailer = '" + numerotrailer + "' AND hora_trailer = '" + fechatrailerx + "'";
                SqlCommand cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                SqlDataReader Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    lengthAxles = Convert.ToDecimal(Info["largo"].ToString().Trim());
                }
                thisConnection.Close();

                return lengthAxles;
            }
            else
            {
                thisConnection.Open();
                cadena = "SELECT largo FROM tb_mstr_trailer WHERE no_trailer = '" + numerotrailer + "' AND hora_trailer = '" + fechatrailerx + "'";
                SqlCommand cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                SqlDataReader Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    lengthAxles = Convert.ToDecimal(Info["largo"].ToString().Trim());
                }
                thisConnection.Close();

                return lengthAxles;
            }
        }

        private decimal BridgeFormula(decimal lengthAxles, decimal numberAxles)
        {
            decimal weight = 0;

            if (lengthAxles > 0)
            {
                weight = 500 * ((lengthAxles * numberAxles) / (numberAxles - 1) + (12 * numberAxles) + 36);
            }
            
            return LibrasAKilos(weight);
        }

        private decimal LibrasAKilos(decimal libras)
        {
            decimal KG = 0;

            if (libras > 0)
            {
                KG = libras / Convert.ToDecimal(2.20462262);
            }

            return KG;
        }

        static int EncontrarUltimo14(int[] array)
        {
            // Recorre la lista en orden inverso y encuentra la última ocurrencia de 14
            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (array[i] == 14)
                {
                    return 14; // Devuelve el valor 14 encontrado
                }
            }

            return -1; // Devuelve -1 si no se encuentra el valor 14
        }

        private void ActualizaPesoPedido(string Placa, string Fecha)
        {
            SqlCommand cmd;
            thisConnection.Open();
            decimal PesoP = 0, PesoT = 0;
            string Cadena = "SELECT emb_tipo, emb_folio FROM tb_mstr_embarques WHERE no_trailer='" + Placa + "' AND hora_trailer = '" + Fecha + "'";
            SqlDataAdapter da = new SqlDataAdapter(Cadena, thisConnection);
            DataSet ds = new DataSet();
            da.Fill(ds, "Ped");
            DataTable Ped = ds.Tables["Ped"];
            foreach (DataRow row in Ped.Rows)
            {
                PesoP = Fn_Peso(row["emb_folio"].ToString(), row["emb_tipo"].ToString());
                PesoT += PesoP;
            }
        }

        public decimal Fn_Peso(string var_Folio, string var_Tipo)
        {
            decimal mKilos = 0;
            string mfec = DateTime.Now.ToString("dd/MM/yyyy");
            //thisConnecion.Open();
            string Cadena = "SELECT A.EMB_FOLIO,A.PROD_CLAVE,A.NO_LOTE,A.CAJAS,A.TARIMA,B.RPT_FECHA,C.env_clave,C.rptd_peso_bruto,C.rptd_tara,C.rptd_tarimas,C.rptd_cantidad FROM TB_DET_EMBARQUE A, TB_MSTR_RECEPCION_PT B,tb_det_recepcion_pt C WHERE " +
                            " A.TIPO_REC = 'PTC' AND A.ESTATUS = 'A' AND A.EMB_FOLIO = '" + var_Folio + "' AND A.EMB_TIPO = '" + var_Tipo + "' AND A.RECIBO = B.RPT_RECIBO AND A.RECIBO = C.RPT_RECIBO AND A.PROD_CLAVE = C.PROD_CLAVE ";
            SqlCommand cmd;
            cmd = new SqlCommand(Cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                //mKilos = mKilos + ((Convert.ToDecimal(Info["rptd_peso_bruto"]) - Convert.ToDecimal(Info["rptd_tara"]) - (Convert.ToDecimal(Info["rptd_tarimas"]) * 20)) / Convert.ToDecimal(Info["rptd_cantidad"])) * Convert.ToDecimal(Info["CAJAS"]);
                mKilos = mKilos + ((Convert.ToDecimal(Info["rptd_peso_bruto"]) - Convert.ToDecimal(Info["rptd_tara"])) / Convert.ToDecimal(Info["rptd_cantidad"])) * Convert.ToDecimal(Info["CAJAS"]);
                DateTime mF = Convert.ToDateTime(mfec);
                if (Convert.ToString(Info["PROD_CLAVE"]) == "02002ML00" || Convert.ToString(Info["PROD_CLAVE"]) == "02002BROFR" || Convert.ToString(Info["PROD_CLAVE"]) == "02BRCO2025")
                    mKilos = mKilos + Fn_PesoHielo(Convert.ToDecimal(8.5), Convert.ToDateTime(Info["RPT_FECHA"]), mF);
                if (Info["PROD_CLAVE"].ToString() == "02002BRHEB")
                    mKilos = mKilos + Fn_PesoHielo(Convert.ToDecimal(4), Convert.ToDateTime(Info["RPT_FECHA"]), mF);
            }
            Cadena = "SELECT A.EMB_FOLIO,A.PROD_CLAVE,A.NO_LOTE,A.CAJAS,A.TARIMA,B.PROD_PESO_VAR,B.FODP_UNIDADES,C.PROD_PRESENTACION,D.ENV_PESO,E.HRP_PESO_NETO,E. HRP_NUM_UNIDADES,E.hrp_fecha FROM TB_DET_EMBARQUE A, TB_DET_FINAL_ODP B, TB_CAT_PRODUCTO C,tb_cat_envases D, TB_HIST_RECEPCION E WHERE " +
                            " A.TIPO_REC = 'PTP' AND A.EMB_FOLIO = '" + var_Folio + "' AND A.EMB_TIPO = '" + var_Tipo + "' AND A.RECIBO = B.ORDP_FOLIO AND A.PROD_CLAVE = B.PROD_CLAVE AND B.PROD_CLAVE = C.PROD_CLAVE AND C.PROD_PRESENTACION = D.ENV_CLAVE  " +
                            " AND A.PROD_CLAVE = E.PROD_CLAVE AND A.RECIBO = E.HRP_RECIBO ORDER BY A.NO_LOTE";
            cmd = new SqlCommand(Cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            decimal mKilos2 = 0, mKilos3 = 0;
            while (Info.Read())
            {
                decimal Mpe = 0;
                if (Convert.ToDecimal(Info["PROD_PESO_VAR"]) > 0)
                    Mpe = Convert.ToDecimal(Info["PROD_PESO_VAR"]) * Convert.ToDecimal(Info["CAJAS"]) + Convert.ToDecimal(Info["ENV_PESO"]) * Convert.ToDecimal(Info["CAJAS"]);
                else
                    Mpe = ((Convert.ToDecimal(Info["HRP_PESO_NETO"]) / Convert.ToDecimal(Info["HRP_NUM_UNIDADES"])) * Convert.ToDecimal(Info["CAJAS"])) + Convert.ToDecimal(Info["ENV_PESO"]) * Convert.ToDecimal(Info["CAJAS"]);

                //mKilos = mKilos + ((Convert.ToDecimal(Info["HRP_PESO_NETO"]) / Convert.ToDecimal(Info["HRP_NUM_UNIDADES"])) * Convert.ToDecimal(Info["CAJAS"]) + Mpe);
                mKilos = mKilos + Mpe;
                mKilos2 += Mpe;
                if (Info["PROD_CLAVE"].ToString() == "02002ML00" || Info["PROD_CLAVE"].ToString() == "02002BROFR" || Info["PROD_CLAVE"].ToString() == "02BRCO2025")
                {
                    mKilos = mKilos + Fn_PesoHielo(Convert.ToDecimal(8.5), Convert.ToDateTime(Info["RPT_FECHA"]), Convert.ToDateTime(mfec));
                    mKilos3 += Fn_PesoHielo(Convert.ToDecimal(8.5), Convert.ToDateTime(Info["RPT_FECHA"]), Convert.ToDateTime(mfec));
                }
                if (Info["PROD_CLAVE"].ToString() == "02002BRHEB")
                {
                    mKilos = mKilos + Fn_PesoHielo(Convert.ToDecimal(4), Convert.ToDateTime(Info["RPT_FECHA"]), Convert.ToDateTime(mfec));
                    mKilos3 += Fn_PesoHielo(Convert.ToDecimal(4), Convert.ToDateTime(Info["RPT_FECHA"]), Convert.ToDateTime(mfec));
                }
            }
            Cadena = "SELECT A.EMB_FOLIO,A.PROD_CLAVE,A.NO_LOTE,A.CAJAS,A.TARIMA, A.SECCION FROM TB_DET_EMBARQUE A " +
                           " WHERE A.ESTATUS = 'A' AND A.EMB_FOLIO = '" + var_Folio + "' AND A.EMB_TIPO = '" + var_Tipo + "' ORDER BY A.SECCION";
            cmd = new SqlCommand(Cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            Int32 nTar = 0, ntot = 0;
            while (Info.Read())
            {
                if (Convert.ToInt32(Info["seccion"]) > nTar)
                {
                    nTar = Convert.ToInt32(Info["seccion"]);
                    ntot++;
                }
            }
            mKilos += (ntot * 20);
            //thisConnecion.Close();
            return mKilos;
        }
    }
}