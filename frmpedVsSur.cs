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
    [Activity(Label = "EMBARQUE SURTIDO", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class frmpedVsSur : Activity
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

        TextView pedido;
        TextView cajaspedidas;
        TextView cajassurtidas;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PedvsSur);
            // Create your application here

            //Declaracion de los id de cada elemento
            pedido = FindViewById<TextView>(Resource.Id.ordenvnt);
            cajaspedidas = FindViewById<TextView>(Resource.Id.tcajasped);
            cajassurtidas = FindViewById<TextView>(Resource.Id.tcajassur);

            pedido.Text = "Orden De Venta: " + ordenventa;
            ordenventa = Intent.GetStringExtra("ordenventa");
            tipoorden = Intent.GetStringExtra("tipoorden");

            pedido.Text = "Orden De Venta: " + ordenventa;


            List<FlimStarInfo> lstFlimStar = detalle_pedido(ordenventa, tipoorden);
            var gvObject = FindViewById<GridView>(Resource.Id.LB1);
            gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);
            gvObject.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(OnGridView_ItemClicked);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Pedido Vs Surtido";
        }

        private void OnGridView_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {

        }

        List<FlimStarInfo> listItem = new List<FlimStarInfo>();
        List<FlimStarInfo> detalle_pedido(string mped, string mov)
        {
            int pedidos = 0;
            int surtidos = 0;
            listItem.Clear();
            thisConnection.Open();
            cadena = "Select DISTINCT PROD_CLAVE,CANT_PED,CANT_SUR,NOM_PROD from tb_ped_embarque Where emb_folio='" + mped + "' and NALEXP = '" + mov + "' order by NOM_PROD";
            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                if (Convert.ToInt32(Info["CANT_PED"].ToString().Trim()) - Convert.ToInt32(Info["CANT_SUR"].ToString().Trim()) == 0)
                {
                    listItem.Add(new FlimStarInfo()
                    {
                        Name = Info["NOM_PROD"].ToString().Trim(),
                        Age = "Pedidos: " + Info["CANT_PED"].ToString().Trim() + " Surtido: " + Info["CANT_SUR"].ToString().Trim() + " Faltante Por Armar: " + (Convert.ToInt32(Info["CANT_PED"].ToString().Trim()) - Convert.ToInt32(Info["CANT_SUR"].ToString().Trim())),
                        ImageID = Resource.Drawable.ProductoCompleto
                    });
                }
                else
                {
                    listItem.Add(new FlimStarInfo()
                    {
                        Name = Info["NOM_PROD"].ToString().Trim(),
                        Age = "Pedidos: " + Info["CANT_PED"].ToString().Trim() + " Surtido: " + Info["CANT_SUR"].ToString().Trim() + " Faltante Por Armar: " + (Convert.ToInt32(Info["CANT_PED"].ToString().Trim()) - Convert.ToInt32(Info["CANT_SUR"].ToString().Trim())),
                        ImageID = Resource.Drawable.ProductoIncompleto
                    });
                }

                pedidos = pedidos + Convert.ToInt32(Info["CANT_PED"].ToString().Trim());
                surtidos = surtidos + Convert.ToInt32(Info["CANT_SUR"].ToString().Trim());

            }
            thisConnection.Close();
            cajaspedidas.Text = "Cajas Pedidas: " + pedidos;
            cajassurtidas.Text = "Cajas Surtidas: " + surtidos;
            return listItem;
        }
    }
}