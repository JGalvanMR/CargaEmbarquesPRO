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

namespace CargaEmbarques
{
    [Activity(Label = "DETALLE SPLIT", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class DetalleSplit : Activity
    {
        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        SqlDataAdapter da;
        DataSet ds = new DataSet();
        SqlCommand cmnd = new SqlCommand();
        SqlCommand cmnd1 = new SqlCommand();
        SqlDataReader reader1;
        public static string ordenventa, tipoorden;
        string query = "", prod_clave = "", folio = "", tipo = "", cadena = "", prod_nombre = "";
        int tarima = 0, caja = 0, tarimaf = 0;
        bool find = false;
        ArrayAdapter<System.String> comboAdapter;
        System.String[] strFrutas;
        public string tb_tabla = "tb_mstr_pedidos_nal";
        public string tipoembarque = "NAL";

        //INFORMACION PARA LA CANCELACION DE LA TARIMA ACTUAL, QUITAR LINEA
        public string recibocancelar = "";
        public string productocancelar = "";
        public string tarimacancelar = "";
        public string tiporecibocancelar = "";
        public string cajascancelar = "";
        public string seccioncancelar = "";
        public string Normalcancelar = "";

        TextView pedido;
        TextView splittotales;

        RadioButton splitpendiente;
        RadioButton splittodos;

        EditText et;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FrmDetalleSplit);


            //Declaracion de los id de cada elemento
            pedido = FindViewById<TextView>(Resource.Id.pedidoactual);
            splittotales = FindViewById<TextView>(Resource.Id.splitcantidad);

            splitpendiente = FindViewById<RadioButton>(Resource.Id.radio_pendiente);
            splittodos = FindViewById<RadioButton>(Resource.Id.radio_todos);

            splitpendiente.Click += Splitpendiente_Click;

            splittodos.Click += Splittodos_Click;


            ordenventa = Intent.GetStringExtra("ordenventa");
            tipoorden = Intent.GetStringExtra("tipoorden");

            pedido.Text = "Orden De Venta: " + ordenventa;


            List<FlimStarInfo> lstFlimStar = detalle_split(ordenventa, tipoorden);
            var gvObject = FindViewById<GridView>(Resource.Id.gvsplit);
            gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Detalle de Split";
        }

        private void Splittodos_Click(object sender, EventArgs e)
        {
            var gvObject = FindViewById<GridView>(Resource.Id.gvsplit);
            gvObject.Adapter = new myGVitemAdapter(this, null);
            gvObject.Adapter = null;
            List<FlimStarInfo> lstFlimStar = detalle_split(ordenventa, tipoorden);
            gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);
        }

        private void Splitpendiente_Click(object sender, EventArgs e)
        {
            var gvObject = FindViewById<GridView>(Resource.Id.gvsplit);
            gvObject.Adapter = new myGVitemAdapter(this, null);
            gvObject.Adapter = null;
            List<FlimStarInfo> lstFlimStar = detalle_split(ordenventa, tipoorden);
            gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);
        }

        List<FlimStarInfo> listItem = new List<FlimStarInfo>();
        List<FlimStarInfo> detalle_split(string mped, string mov)
        {
            int splitanterior = 0;
            int sptotales = 0;

            int surtidos = 0;
            listItem.Clear();
            thisConnection.Open();
            if (splitpendiente.Checked == true)
            {
                cadena = "Select  tarima, nom_prod, prod_clave, SUM(cajas) AS CAJAS, NOM_CAPSPLIT from   tb_det_split WHERE emb_folio = '" + ordenventa + "' AND estatus = 'A' GROUP BY tarima, nom_prod, prod_clave, NOM_CAPSPLIT";
            }
            else if (splittodos.Checked == true)
            {
                cadena = "Select  tarima, nom_prod, prod_clave, SUM(cajas) AS CAJAS, NOM_CAPSPLIT, ESTATUS from   tb_det_split WHERE emb_folio = '" + ordenventa + "'  AND estatus != 'C' GROUP BY tarima, nom_prod, prod_clave, NOM_CAPSPLIT, ESTATUS";
            }

            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                if (splitanterior != Convert.ToInt32(Info["tarima"].ToString().Trim()))
                {
                    sptotales++;
                    splitanterior = Convert.ToInt32(Info["tarima"].ToString().Trim());
                }

                if (splitpendiente.Checked == true)
                {
                    listItem.Add(new FlimStarInfo()
                    {
                        Name = Info["prod_clave"].ToString().Trim() + " - " + Info["nom_prod"].ToString().Trim(),
                        Age = "Split: " + Info["tarima"].ToString().Trim() + "|" + Info["CAJAS"].ToString().Trim() + "|" + Info["NOM_CAPSPLIT"].ToString().Trim(),
                        ImageID = Resource.Drawable.logo_splittrailers_rojo
                    });
                }
                else
                {
                    if (Info["Estatus"].ToString().Trim() == "A")
                    {
                        listItem.Add(new FlimStarInfo()
                        {
                            Name = Info["prod_clave"].ToString().Trim() + " - " + Info["nom_prod"].ToString().Trim(),
                            Age = "Split: " + Info["tarima"].ToString().Trim() + "|" + Info["CAJAS"].ToString().Trim() + "|" + Info["NOM_CAPSPLIT"].ToString().Trim(),
                            ImageID = Resource.Drawable.logo_splittrailers_rojo
                        });
                    }
                    else
                    {
                        listItem.Add(new FlimStarInfo()
                        {
                            Name = Info["prod_clave"].ToString().Trim() + " - " + Info["nom_prod"].ToString().Trim(),
                            Age = "Split: " + Info["tarima"].ToString().Trim() + "|" + Info["CAJAS"].ToString().Trim() + "|" + Info["NOM_CAPSPLIT"].ToString().Trim(),
                            ImageID = Resource.Drawable.logo_splittrailers_verde
                        });
                    }
                }
            }
            thisConnection.Close();
            splittotales.Text = "Split Totales: " + sptotales;
            return listItem;
            //262517
            //UNIDADES POR TARIMAS
        }
    }
}