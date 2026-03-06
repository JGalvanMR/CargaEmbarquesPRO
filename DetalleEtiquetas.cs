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
using System.Net.Mail;
using System.Text.RegularExpressions;
using AlertDialog = Android.App.AlertDialog;

namespace CargaEmbarques
{
    [Activity(Label = "ETIQUETAS CAPTURADAS", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class DetalleEtiquetas : Activity
    {
        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        SqlDataAdapter da;
        DataSet ds = new DataSet();
        SqlCommand cmnd = new SqlCommand();
        SqlCommand cmnd1 = new SqlCommand();
        SqlDataReader reader1;
        public static string ordenventa, tipoorden, responsable;
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
        TextView cajasleidas;
        Android.Widget.Button QuitarLinea;

        EditText et;
        GridView gv;
        Android.Widget.Button btnAceptarEliminarTarima;
        int indiceSeleccionado;

        string SerialShippingContainerCode = "0000796631";
        string patron = @"^00007966310*([1-9]\d*).$";
        string FolioProducto = "";
        string TarimaProducto = "";
        string ProductoProducto = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DetalleEtiquetas);

            //Declaracion de los id de cada elemento
            pedido = FindViewById<TextView>(Resource.Id.ordenve);
            cajasleidas = FindViewById<TextView>(Resource.Id.totalcajasleidas);
            QuitarLinea = FindViewById<Button>(Resource.Id.QuitarLinea);


            //QuitarLinea.Visibility = ViewStates.Invisible;
            QuitarLinea.Click += BtnQuitarLinea_Click;


            ordenventa = Intent.GetStringExtra("ordenventa");
            tipoorden = Intent.GetStringExtra("tipoorden");
            responsable = Intent.GetStringExtra("responsable");

            pedido.Text = "Orden De Venta: " + ordenventa;

            List<FlimStarInfo> lstFlimStar = detalle_pedido(ordenventa, tipoorden);
            var gvObject = FindViewById<GridView>(Resource.Id.gvleido);
            gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);
            gvObject.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(OnGridView_ItemClicked);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Detalle de Etiquetas";
            //QuitarLinea.Visibility = ViewStates.Invisible;
            QuitarLinea.Enabled = false;

        }

        private void actualizarGrid()
        {
            List<FlimStarInfo> lstFlimStar = detalle_pedido(ordenventa, tipoorden);
            var gvObject = FindViewById<GridView>(Resource.Id.gvleido);
            gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);
            gvObject.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(OnGridView_ItemClicked);
        }

        private void BtnQuitarLinea_Click(object sender, EventArgs e)
        {
            #region Crear LayoutInflater apartir de la vista eliminarTarima.
            Android.Views.View view = LayoutInflater.Inflate(Resource.Layout.eliminarTarima, null);
            AlertDialog buidersxs = new AlertDialog.Builder(this).Create();
            buidersxs.SetView(view);
            buidersxs.SetCanceledOnTouchOutside(false);
            #endregion

            #region Enlazar los controles a la vista eliminarTarima
            #region Crear Elemento del GridView
            gv = view.FindViewById<GridView>(Resource.Id.gvTarima);
            List<FlimStarInfo> lstFlimStar = detalle_pedido(ordenventa, tipoorden);
            FlimStarInfo elementoEspecifico = lstFlimStar[indiceSeleccionado];
            List<FlimStarInfo> listaUnicoElemento = new List<FlimStarInfo>() { elementoEspecifico };
            gv.Adapter = new myGVitemAdapter(this, listaUnicoElemento);



            #endregion
            #region Enlazar control EditText input
            et = view.FindViewById<EditText>(Resource.Id.codigoTarima);
            et.LongClickable = false;
            et.RequestFocus();
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(et, ShowFlags.Implicit);
            #endregion
            #endregion

            #region Enlazar Controles Button para la vista eliminarTarima
            #region btnAceptar
            btnAceptarEliminarTarima = view.FindViewById<Android.Widget.Button>(Resource.Id.btnAceptar);
            btnAceptarEliminarTarima.Click += delegate
            {
                eliminarTarima();
                //QuitarLinea.Visibility = ViewStates.Invisible;
                QuitarLinea.Enabled = false;
                buidersxs.Dismiss();
                //if (et.Text != "" || et.Length() > 15)
                //{
                //    eliminarTarima();
                //    QuitarLinea.Visibility = ViewStates.Invisible;
                //    QuitarLinea.Enabled = false;
                //    buidersxs.Dismiss();
                //}
            };
            #endregion
            #region btnCancelar
            Android.Widget.Button btnCancelarEliminarTarima = view.FindViewById<Android.Widget.Button>(Resource.Id.btnCancelar);
            btnCancelarEliminarTarima.Click += delegate
            {
                gv.Adapter = new myGVitemAdapter(this, null);
                gv.Adapter = null;
                et.Text = "";
                //QuitarLinea.Visibility = ViewStates.Invisible;
                QuitarLinea.Enabled = false;
                buidersxs.Cancel();
                return;
            };
            #endregion
            #endregion
            buidersxs.Show();



            #region Eliminar Tarima/Linea OLD
            /*et = new EditText(this);
            et.InputType = Android.Text.InputTypes.ClassText;
            et.LongClickable = false;
            et.Hint = "Codigo";
            AlertDialog.Builder ad = new AlertDialog.Builder(this);
            ad.SetTitle("CONFIRMACION DE LINEA");
            ad.SetCancelable(false);
            //ad.SetView(et);
            ad.SetView(gv);
            ad.SetPositiveButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Eliminar Tarima</font>"), SaveName);
            ad.SetNegativeButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Cancelar</font>"), CancelAction);
            ad.Show();*/
            #endregion
        }

        private void eliminarTarima()
        {
            string V_Recibo = "", V_Prd = "", V_Existe = "", Mtipo = "", Fechacad = "", fecha_cad = "", diacad = "", mescad = "", prod_nombre = "";
            string v_Folio = "", Prd = "", Lote = "", Cajas = "", Temp = "", Pos = "", Cadena = "", Tar = "", NomPro = "", Sts = "", id_pallet = "";
            int L_Cad, V_Tamaño, wrkcen, intPositionEtiBlanca;
            string szSQL, mtar = "";


            string codigo = et.Text.Trim();
            codigo = codigo.Replace("HTTP://WWW.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=", "");
            codigo = codigo.Replace("HTTP://GAB.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=", "");
            codigo = codigo.Replace("http://www.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=", "");
            codigo = codigo.Replace("http://gab.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=", "");
            if ((codigo.Contains("HTTP://WWW.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=") == false || codigo.Contains("http://www.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") == false) || (codigo.Contains("HTTP://GAB.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=") == false || codigo.Contains("http://gab.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") || false))
            {
                if (codigo.Trim().Length == 12)
                {
                    string pti_famous = codigo.Trim();
                    if (codigo.StartsWith("0"))
                    {
                        pti_famous = codigo.TrimStart('0');
                    }

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_famous='" + pti_famous + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = sqlDataReader["tarima"].ToString().Trim();
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if (codigo.Contains(SerialShippingContainerCode) == true)
                {
                    Match match = Regex.Match(codigo, patron);
                    id_pallet = match.Groups[1].Value;

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where id_Pallet='" + id_pallet + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = sqlDataReader["tarima"].ToString().Trim();
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if (!Regex.IsMatch(codigo.Trim(), @"\s") && codigo.Length <= 20)
                {
                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_clave='" + codigo.Trim() + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = sqlDataReader["tarima"].ToString().Trim();
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if (codigo.Length > 10)
                {
                    if (codigo.Contains(" ") == true)
                    {
                        V_Tamaño = codigo.Length;
                        L_Cad = V_Tamaño - 9;
                        Mtipo = "PTP";
                        mtar = codigo.Substring(V_Tamaño - 3, 3);
                        V_Recibo = codigo.Substring(0, 6);
                        if (V_Recibo.Substring(0, 1) == "0")
                        {
                            Mtipo = "PTC";
                            V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                        }
                        V_Prd = codigo.Substring(6, L_Cad).ToUpper();
                    }
                    else
                    {
                        DataTable CatalogodeProducto = new DataTable();
                        if (thisConnection.State == ConnectionState.Closed)
                        {
                            thisConnection.Open();
                        }
                        string cade = "Select prod_clave,prod_nombre,prod_tipo from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC') order by LEN(prod_clave) DESC";
                        SqlDataAdapter da = new SqlDataAdapter(cade, thisConnection);
                        DataSet ds = new DataSet();
                        da.Fill(ds, "CatalogodeProducto");
                        CatalogodeProducto = ds.Tables["CatalogodeProducto"];
                        if (thisConnection.State == ConnectionState.Open)
                        {
                            thisConnection.Close();
                        }

                        for (int i = 0; i < CatalogodeProducto.Rows.Count; i++)
                        {
                            string producto_clave = CatalogodeProducto.Rows[i]["Prod_Clave"].ToString().Trim();
                            string producto_tipo = CatalogodeProducto.Rows[i]["prod_tipo"].ToString().Trim();
                            bool esta = codigo.Trim().Contains(producto_clave);

                            if (esta)
                            {
                                V_Prd = producto_clave;
                                Mtipo = producto_tipo;
                                break;
                            }
                        }



                        int posprod = codigo.Trim().IndexOf(V_Prd);
                        V_Recibo = codigo.Trim().Substring(0, posprod).Trim();

                        string restocaptura = codigo.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                        if (restocaptura.Length == 6)
                        {
                            //Mtipo = "PTC";
                            mtar = restocaptura.Substring(0, 3);
                        }
                        else if (restocaptura.Length == 9)
                        {
                            //Mtipo = "PTC";
                            //mcaj = restocaptura.Substring(6, 3);
                            mtar = restocaptura.Substring(0, 3);
                        }
                        else
                        {
                            //Mtipo = "PTC";
                            mtar = restocaptura.Substring(0, 2);
                        }
                    }

                }
            }
            else
            {
                if (codigo.Contains("=") == false)
                {
                    et.Text = "";
                    et.RequestFocus();
                    return;
                }
                codigo = codigo.Replace("HTTP://WWW.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=", "");
                codigo = codigo.Replace("HTTP://GAB.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=", "");
                codigo = codigo.Replace("http://www.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=", "");
                codigo = codigo.Replace("http://gab.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=", "");
                DataTable CatalogodeProducto = new DataTable();
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                //string cade = "Select prod_clave,prod_nombre from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC') order by LEN(prod_clave) DESC";
                string cade = "Select prod_clave,prod_nombre, prod_tipo from vwCatalogoProductos order by LEN(prod_clave) DESC";
                SqlDataAdapter da = new SqlDataAdapter(cade, thisConnection);
                DataSet ds = new DataSet();
                da.Fill(ds, "CatalogodeProducto");
                CatalogodeProducto = ds.Tables["CatalogodeProducto"];
                if (thisConnection.State == ConnectionState.Open)
                {
                    thisConnection.Close();
                }

                for (int i = 0; i < CatalogodeProducto.Rows.Count; i++)
                {
                    string producto_clave = CatalogodeProducto.Rows[i]["Prod_Clave"].ToString().Trim();
                    string producto_tipo = CatalogodeProducto.Rows[i]["prod_tipo"].ToString().Trim();
                    bool esta = codigo.Trim().Contains(producto_clave);

                    if (esta)
                    {
                        V_Prd = producto_clave;
                        Mtipo = producto_tipo;
                        break;
                    }
                }



                int posprod = codigo.Trim().IndexOf(V_Prd);
                V_Recibo = codigo.Trim().Substring(0, posprod).Trim();

                string restocaptura = codigo.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                if (restocaptura.Length == 6)
                {
                    //Mtipo = "PTC";
                    mtar = restocaptura.Substring(0, 3);
                }
                else if (restocaptura.Length == 9)
                {
                    //Mtipo = "PTC";
                    //mcaj = restocaptura.Substring(6, 3);
                    mtar = restocaptura.Substring(0, 3);
                }
                else
                {
                    //Mtipo = "PTC";
                    mtar = restocaptura.Substring(0, 2);
                }

                #region VALIDAR ETIQUETA VERDE A CANCELAR
                /*int tam = codigo.Length;
                string mCaj, Ent, mtarf;
                mCaj = "";
                Ent = "N";
                if (tam > 20) {
                    int valorfolio = 0;
                    valorfolio = Convert.ToInt32(codigo.Substring(0, 6));
                    if (valorfolio > 262113)
                    {
                        Ent = "S";
                    }
                }
                if (Ent == "N")
                {
                    mCaj = codigo.Substring(tam - 3, 3);
                    mtar = codigo.Substring(tam - 6, 3);
                    mtar = Convert.ToInt32(mtar).ToString();
                    int tam2 = tam - 6;
                    Mtipo = "PTP";
                    if (tam2 == 15)
                    {
                        V_Recibo = codigo.Substring(0, 5);
                        V_Prd = codigo.Substring(5, tam - 11);
                        Mtipo = "PTC";
                        et.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                    }
                    else if (tam2 <= 14)
                    {
                        V_Recibo = codigo.Substring(0, 4);
                        V_Prd = codigo.Substring(4, tam - 10);
                        Mtipo = "PTC";
                        et.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                    }
                    else
                    {
                        V_Recibo = codigo.Substring(0, 6);
                        V_Prd = codigo.Substring(6, tam - 12);
                        et.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                    }
                    string nombreproducto = "";
                    nombreproducto = traenom(V_Prd);

                    if (nombreproducto == "")
                    {
                        V_Recibo = codigo.Substring(0, 6);
                        V_Prd = codigo.Substring(6, tam - 12);
                        Mtipo = "PTP";
                        et.Text = V_Recibo + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                    }

                    nombreproducto = traenom(V_Prd);

                    if (nombreproducto == "")
                    {
                        mtar = codigo.Substring(tam - 4, 2);
                        V_Recibo = codigo.Substring(0, 6);
                        V_Prd = codigo.Substring(6, tam - 10);
                        Mtipo = "PTC";
                        et.Text = V_Recibo + V_Prd + mtar + mtar;
                    }

                    nombreproducto = traenom(V_Prd);

                    if (nombreproducto == "")
                    {
                        //mcaj = captura.Substring(tam - 3, 3);
                        mtar = codigo.Substring(tam - 6, 3);
                        V_Recibo = codigo.Substring(0, 5);
                        V_Prd = codigo.Substring(5, tam - 11);
                        Mtipo = "PTC";
                        et.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                    }
                }
                else {
                    mCaj = codigo.Substring(tam - 3, 3);
                    mtar = codigo.Substring(tam - 7, 2);
                    mtarf = codigo.Substring(tam - 5, 2);
                    V_Recibo = codigo.Substring(0, 6);
                    V_Prd = codigo.Substring(6, tam - 13);
                    Mtipo = "PTC";
                    et.Text = V_Recibo + V_Prd + mtar + mtarf;
                }*/
                #endregion
            }

            int mtarInt;
            if (int.TryParse(mtar, out mtarInt))
            {
                mtar = mtarInt.ToString();
            }
            else
            {
                mtar = "0"; // o cualquier otro valor predeterminado que desees
            }

            //mtar = Convert.ToInt32(mtar).ToString();

            if ((V_Recibo.Trim() == recibocancelar.Trim()) && (V_Prd.Trim() == productocancelar.Trim()) && mtar.Trim() == tarimacancelar.Trim())
            {
                Prd = productocancelar;
                Lote = recibocancelar;
                Tar = tarimacancelar;
                v_Folio = ordenventa;
                Pos = seccioncancelar;
                Cajas = cajascancelar;

                thisConnection.Open();
                szSQL = "UPDATE tb_det_embarque SET ESTATUS = 'C' WHERE emb_folio = '" + v_Folio + "' and emb_tipo = '" + tipoorden + "' and prod_clave = '" + Prd + "' and recibo = '" + Lote + "' and cajas = '" + Cajas + "' AND SECCION = '" + Pos + "' AND TARIMA = '" + Tar + "' AND ESTATUS = 'A'";
                SqlCommand cmd = new SqlCommand(szSQL, thisConnection);
                cmd.ExecuteNonQuery();
                thisConnection.Close();

                if (Mtipo == "PTC")
                {
                    //szSQL = "UPDATE tb_det_trazabilidad SET SURTIDO = SURTIDO - '" + Cajas + "', pti_estatus_sur = ' ' WHERE TIPO = 'PTC' AND RECIBO = '" + Lote + "' and prod_clave = '" + Prd + "' AND TARIMA = " + Tar;
                    szSQL = "UPDATE tb_det_trazabilidad SET SURTIDO = CASE WHEN (SURTIDO - '" + Cajas + "') < 0 OR (SURTIDO - '" + Cajas + "') > Etiqueta THEN SURTIDO ELSE SURTIDO - '" + Cajas + "' END, pti_estatus_sur = ' ' WHERE TIPO = 'PTC' AND RECIBO = '" + Lote + "' AND prod_clave = '" + Prd + "' AND TARIMA = " + Tar;
                }
                else
                {
                    //szSQL = "UPDATE TB_DET_ETI_FINAL SET CAJAS_SUR = CAJAS_SUR - '" + Cajas + "', estatus_sur = ' ' WHERE FOLIO = '" + Lote + "' and CVE_PROD = '" + Prd + "' AND TARIMA = " + Tar + "";
                    szSQL = "UPDATE TB_DET_ETI_FINAL SET CAJAS_SUR = CASE WHEN (CAJAS_SUR - '" + Cajas + "') < 0 OR (CAJAS_SUR - '" + Cajas + "') > num_cajas THEN CAJAS_SUR ELSE CAJAS_SUR - '" + Cajas + "' END, estatus_sur = ' ' WHERE FOLIO = '" + Lote + "' and CVE_PROD = '" + Prd + "' AND TARIMA = " + Tar + "";
                }

                thisConnection.Open();
                cmd = new SqlCommand(szSQL, thisConnection);
                cmd.ExecuteNonQuery();
                thisConnection.Close();

                //actualizacion del gridview
                List<FlimStarInfo> lstFlimStar = detalle_pedido(ordenventa, tipoorden);
                var gvObject = FindViewById<GridView>(Resource.Id.gvleido);
                gvObject.Adapter = new myGVitemAdapter(this, null);
                gvObject.Adapter = null;
                gvObject.Adapter = new myGVitemAdapter(this, lstFlimStar);
                gvObject.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(OnGridView_ItemClicked);


                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#41DF01' size = 10>Producto Eliminado</font>"));
                alertDialog.SetIcon(Resource.Drawable.no);
                alertDialog.SetCancelable(false);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>Tarima cancelada correctamente!</font>"));
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    et.Text = "";
                    alertDialog.Dispose();

                });
                alertDialog.Show();

                string body = setEmailCancelacion(v_Folio, Lote, getNombreProducto(Prd), mtar, Cajas, responsable);
                //SendMail("jgalvan@mrlucky.com.mx", body, "CANCELACION DE TARIMAS DE EMBARQUES");
                SendMail(getDestinatarios(), body, "CANCELACION DE TARIMAS DE EMBARQUES");
            }
            else
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>No corresponden las etiquetas</font>"));
                alertDialog.SetIcon(Resource.Drawable.no);
                alertDialog.SetCancelable(false);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Etiqueta Capturada no Corresponde a la Informacion Especificada, Favor de Verificarlo</font>"));
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    et.Text = "";
                    alertDialog.Dispose();
                });
                alertDialog.Show();

                string body = setEmailCancelacion(pedido.Text.Trim(), recibocancelar.Trim(), getNombreProducto(productocancelar.Trim()), tarimacancelar, cajascancelar, responsable.Trim());
                SendMail("jgalvan@mrlucky.com.mx", body, "CANCELACION DE TARIMAS DE EMBARQUES - ERROR");
                actualizarGrid();
                //SendMail(getDestinatarios(), body, "CANCELACION DE TARIMAS DE EMBARQUES");
            }
        }

        public string getNombreProducto(string prod_clave)
        {
            string prod_nombre;

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            SqlCommand command = new SqlCommand("SELECT dbo.getNombreProducto(@prod_clave)", thisConnection);
            command.Parameters.AddWithValue("@prod_clave", prod_clave);
            prod_nombre = (string)command.ExecuteScalar();
            //thisConnection.Close();

            return prod_nombre;

        }

        private void OnGridView_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Obtener datos del elemento seleccionado
            string[] data1 = e.View.FindViewById<TextView>(Resource.Id.txtName).Text.Split(" - ");
            string[] data2 = e.View.FindViewById<TextView>(Resource.Id.txtAge).Text.Split("|");

            // Asignar valores a variables
            AssignValues(data1, data2);

            // Verificar si el elemento puede ser seleccionado
            if (Normalcancelar.Trim() != "X")
            {
                indiceSeleccionado = e.Position;
                // Mostrar mensaje y habilitar botón
                ShowMessageAndButton();
            }
            else
            {
                // Ocultar botón y mostrar alerta
                HideButtonAndShowAlert();
            }
        }

        private void AssignValues(string[] data1, string[] data2)
        {
            recibocancelar = data2[0].Replace("|", "");
            cajascancelar = data2[1].Replace("|", "");
            tarimacancelar = data2[2].Replace("|", "");
            productocancelar = data2[3].Replace("|", "");
            tiporecibocancelar = data2[4].Replace("|", "");
            seccioncancelar = data1[1].Replace(" - ", "");
            Normalcancelar = data1[0].Replace(" - ", "");
        }

        private void ShowMessageAndButton()
        {
            //QuitarLinea.Visibility = ViewStates.Visible;
            QuitarLinea.Enabled = true;
            QuitarLinea.RequestFocus();
            Toast.MakeText(this, "LINEA SELECCIONADA... PROCEDA A BOTON QUITAR LINEA", ToastLength.Short).Show();
        }
        private void HideButtonAndShowAlert()
        {
            //QuitarLinea.Visibility = ViewStates.Invisible;
            QuitarLinea.Enabled = false;

            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Producto de Split Seleccionado</font>"));
            alertDialog.SetIcon(Resource.Drawable.no);
            alertDialog.SetCancelable(false);
            alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La seleccion actual no puede ser elegida debido a que forma parte de un split, favor de cancelar con la lectora de armadores</font>"));
            alertDialog.SetNeutralButton("Ok", delegate
            {
                ClearVariables();
                alertDialog.Dispose();
            });
            alertDialog.Show();
        }
        private void ClearVariables()
        {
            recibocancelar = "";
            productocancelar = "";
            tarimacancelar = "";
            tiporecibocancelar = "";
            cajascancelar = "";
            seccioncancelar = "";
            Normalcancelar = "";
        }

        List<FlimStarInfo> listItem = new List<FlimStarInfo>();
        List<FlimStarInfo> detalle_pedido(string mped, string mov)
        {
            int surtidos = 0;
            listItem.Clear();
            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }

            cadena = "Select A.recibo, A.cajas, A.PROD_CLAVE, B.PROD_NOMBRE, A.seccion, A.tarima, A.tipo_rec, A.ESTATUS, A.OpCap From tb_det_embarque A, TB_CAT_PRODUCTO B WHERE A.emb_folio = '" + ordenventa + "' AND A.ESTATUS = 'A' AND A.PROD_CLAVE = B.PROD_CLAVE order by a.seccion,b.prod_nombre";
            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                if (Info["OpCap"].ToString().Trim() == "N")
                {
                    listItem.Add(new FlimStarInfo()
                    {
                        Name = "N - " + Info["seccion"].ToString().Trim() + " - " + Info["PROD_NOMBRE"].ToString().Trim(),
                        Age = Info["recibo"].ToString().Trim() + "|" + Info["cajas"].ToString().Trim() + "|" + Info["tarima"].ToString().Trim() + "|" + Info["PROD_CLAVE"].ToString().Trim() + "|" + Info["tipo_rec"].ToString().Trim(),
                        ImageID = Resource.Drawable.cargasupervisor
                    });
                }
                else
                {
                    listItem.Add(new FlimStarInfo()
                    {
                        Name = "X - " + Info["seccion"].ToString().Trim() + " - " + Info["PROD_NOMBRE"].ToString().Trim(),
                        Age = Info["recibo"].ToString().Trim() + "|" + Info["cajas"].ToString().Trim() + "|" + Info["tarima"].ToString().Trim() + "|" + Info["PROD_CLAVE"].ToString().Trim() + "|" + Info["tipo_rec"].ToString().Trim(),
                        ImageID = Resource.Drawable.cargasplit
                    });
                }

                surtidos = surtidos + Convert.ToInt32(Info["cajas"].ToString().Trim());

            }
            thisConnection.Close();
            cajasleidas.Text = "Cajas Leidas: " + surtidos;
            return listItem;
        }

        public void SendMail(string Dest, string mBody, string mAsunto)
        {
            MailMessage msg = new MailMessage();
            MailMessage email = new MailMessage();

            string[] destinatarios = Dest.Split(';');
            foreach (string destinos in destinatarios)
            {
                email.To.Add(new MailAddress(destinos));
            }
            email.From = new MailAddress("jgalvan@mrlucky.com.mx");
            email.Subject = mAsunto;
            email.IsBodyHtml = true;
            email.Body = mBody;  //"Información de la factura";
            email.Priority = MailPriority.Normal;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "mail1.mrlucky.com.mx";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            //smtp.Credentials = new NetworkCredential("dmunoz", "GuIraSis003$1234");
            smtpClient.Credentials = new System.Net.NetworkCredential("jgalvan", "mnK3a2aN@1lQ21VV");

            try
            {
                smtpClient.Send(email);
                email.Dispose();
                RunOnUiThread(() => Toast.MakeText(this, "correo enviado exitosamente\r\n", ToastLength.Short).Show());
            }
            catch (System.Exception ex)
            {
                RunOnUiThread(() => Toast.MakeText(this, "correo no enviado\r\n" + ex.ToString(), ToastLength.Short).Show());
            }
        }

        public string getDestinatarios()
        {
            string destinatarios = "";

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            cadena = "select EMAIL_DEST from TB_MSTR_EMAIL where CNTE_CLAVE='CARGAEMB' and EMAIL_MOV='ELI'";
            SqlCommand cmd = new SqlCommand(cadena, thisConnection);
            destinatarios = Convert.ToString(cmd.ExecuteScalar());
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            return destinatarios;
        }

        public string setEmailCancelacion(string orden, string recibo, string producto, string tarima, string cajas, string responsable)
        {
            if (string.IsNullOrEmpty(orden))
            {
                orden = "N/A";
                recibo = "N/A";
                producto = "N/A";
                tarima = "N/A";
                cajas = "N/A";
                responsable = "N/A";
            }

            // Obtén el identificador de la imagen
            /*int resourceIdImagen1 = Resource.Drawable.LogosA;
            int resourceIdImagen2 = Resource.Drawable.logo;
            // Carga la imagen desde los recursos
            Bitmap bitmapImagen1 = BitmapFactory.DecodeResource(Resources, resourceIdImagen1);
            Bitmap bitmapImagen2 = BitmapFactory.DecodeResource(Resources, resourceIdImagen2);

            // Convierte la imagen a base64
            MemoryStream streamImagen1 = new MemoryStream();
            MemoryStream streamImagen2 = new MemoryStream();
            bitmapImagen1.Compress(Bitmap.CompressFormat.Png, 100, streamImagen1);
            bitmapImagen2.Compress(Bitmap.CompressFormat.Png, 100, streamImagen2);
            byte[] byteArrayImagen1 = streamImagen1.ToArray();
            byte[] byteArrayImagen2 = streamImagen2.ToArray();
            string base64Image1 = Convert.ToBase64String(byteArrayImagen1);
            string base64Image2 = Convert.ToBase64String(byteArrayImagen2);*/


            string body = @"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html dir='ltr' lang='es'><head><meta content='text/html; charset=UTF-8' http-equiv='Content-Type'><style>*,::after,::before{box-sizing:border-box}html{-moz-tab-size:4;tab-size:4}html{line-height:1.5;-webkit-text-size-adjust:100%}body{margin:0}table{text-indent:0;border-color:inherit}body{font-family:inherit;line-height:inherit}*,::after,::before{--tw-border-opacity:1;border-color:rgba(229,231,235,var(--tw-border-opacity))}body{font-family:ui-sans-serif,system-ui,-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,'Helvetica Neue',Arial,'Noto Sans',sans-serif,'Apple Color Emoji','Segoe UI Emoji','Segoe UI Symbol','Noto Color Emoji';background-color:#fff;text-align:center;color:#000;font-size:14px;line-height:20px;display:flex;margin-top:18px;flex-grow:1;overflow-x:hidden;overflow-y:hidden;align-items:center;border-radius:30px}h1{font-weight:600;font-size:15px;text-align:left}.imagenes{width:85em;height:auto;display:block;outline:0;border:none;text-decoration:none}.bg-gray{background:#e5e7eb;color:#374151;text-align:center}.miTabla{align-items:center;justify-content:center;font-size:14px;margin:0 auto;color:#333;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen,Ubuntu,Cantarell,Fira Sans,Droid Sans,Helvetica Neue,sans-serif;border-collapse:collapse}.brBottom{border-bottom:3px solid #e5e7eb}</style></head><div style='display:none;overflow:hidden;line-height:1px;opacity:0;max-height:0;max-width:0'><div></div></div><body style='background-color:#fff;color:#212121'><table align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='max-width:70em;padding:20px;margin:0 auto;background-color:#f2f2f2'><tbody><tr style='width:100%'><td><table align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background-color:#fff'><tbody><tr><td><table align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='background-color:#e5e7eb;padding:10px'><tbody><tr><td><img class='imagenes' src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAADGgAAACTCAYAAABoU868AAAACXBIWXMAAC4jAAAuIwF4pT92AAAgAElEQVR4nOzdd3RU1frG8e8+U1KpoYsFC4jt2rsiiooDAcvVK7af15aIYu8NFXvHmrF71WtXIBBB8dp7vRaUYgGRnkBCymTK2b8/Br3UZDI5k1Cez1pZKzln7+d9CS5i1jrv2QYREREREREREREREREREREREREREREREWkdw0v9uOwD7A1kLbv6I9j3KRkypxU7W3sVl24P7A90XnZlLvA+JYVTWq8pMK1ZXERERERERERERERERERERERERERERERkg1VcuhdQAuwAfAdUAD5gZ5LP+z+GtbcQHvJH6zW5Fikq3Q/DFcBAYDbwy7I7vYGuwItYziRcuLg12tOAhojIWqLgwkm9a6OJwXlZvt7GYCrr4tO7tc16Y9ZNA75t7d7WNh0vnLjX0kji4I55gU1icTdSF3O/7do2q/S3Gw+a29q9iYiIiIiIiIiIiIiIiIiIiIiIiIikpLi0EHgZmAj2AsCA2Riow7pTMM6xwGUkBw/CWHsX4SGzW7Hj1lNUuj+GK4FDgEnAzcCPQF/Agvs91tkLwz1AFOhPSeGClm5TAxoiIq2s350fZX3225Lr6+PuuUDU75gpLtbF0jfu2vZBvzNuo3bZl/x640FTW7vX1tbmvNf3rYsmRsZdOyDgM3Ndy88Bx2RH4u4OgMkN+q6dcNbuN/Xv06m1WxURERERERERERERERERERERERERWbOicd0w5htgPJZbMdwNDFpuxXzgPqx9CMxRGC4BugOPYrmNcOGc1mi7xRWN648xF5L83kwEbsWyAMPFwPFAYNnKOmA0lvsxvAn8SEnhUS3drgY0RERaUb87P8r+5NfFE2IJu1eW3zn7ysO2fPblr+f2qKqL+0/co+fMu9/65eDq+sRNrrXbZgec57q3zb5hQxzU6HHpm1vMq6q/x7V2cMBnvsgO+C68aMAWnz303m+9coO+amPMkjlLImdE4onrHGMeTTw4+LzW7llEREREREREREREREREREREREREZI2KS+8Ejga2A74CtljDykrgVuBB4HDgWmDjZV/fRknh+nmiRnHpvsANQD/gP8ClQDVwNXBcAzvvAV4B3id5isY7mW10RRrQEBFpRc6Z4x8DDmqb4x9YHYkPt3BU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fgWM3Wvuv684uMdr43oOqu/Z2pyjeUVIrrGNxrczpMur6NKtjGULmiCXwJjELOPbb99/3/2Td0RxH7mAxgddOvHkI8K4OAA4ObZRj8hGPbd2Dde4qyw85xr3Ac9JvHn4bkOfPb3/d+ONThg+xgMGAv5GRrxDw4jFW1uHiMjWmPzucmfoTS9cVAztb1yHCd3ry747tzH7pXwQXw14m12gHSVcZ8wuFYmHl7YUzgoie8rO3Nt1zJJMwh0J/LetEP01trZ/Zdo7u/mGoffszDpERERERERERERERERERERERERE5JPrlFOGHTJ16tQXAZNMJht69ep9zZtv/vdxoMLzPCorqwFIp9JTnnr6qWONMVvXRmM1a6237759X8zlcgcBYb9++55VLBbfmzlzxmPW2grHcaiurl0z3vFT+IlkmwG390GHpKqqS3XMtLsybmn9mnG7NL3NXrPuJ5FMrrm2bkCjaflSkqvDG7HjMWvA92kyFVtScgwcwahTn2f4vVCXvhz45bbc+3ZaAVwAvATcCRwCfI9Rp94GcNCQvuVHDOt+dP/DDqjt0qN2Vb44N/3q/7V1e+PRRac6xuvjuE7ON+4DXrriqnvv+dsGwjXt4yMf0NiQKx/9wsEr88sqMn75IYUoX+/gHBLExc33bQEc49qEm3rwmpMeum5H1yki0h46//CRo5e3Fe+2lkxFyr046bkvN7YVrw9je3xH17ajZRLuDXvWpq+a25g7Ix9E1xtjZqd995utNw59raNrExEREREREREREREREREREREREZFPDmut2XffvvfncrnTVl+KABfW62hh999//9PvuOOOcduz18knDx381ltvTVq9frx6Lx+grKx8TZDC8fymBYNG2mVhurEq6X4bY9zmQniTtfSJrSWK12ZEPMew2xu34S74z9p7wmAAx3WorKx+vwMI2R5DeLv2qK0pOQKeASqAg7b9zreJBW7DcjmGrwJXAu8AIxh16vTNTR49ejSz5y1KOol09JsrLw13dLEfy4CGiMinzW4/nlS9tKX4izi2Z3muebos6f7RMaamsS34YWzt4YDT0TW2F8eYrDFMrkx517Xkw3JjzE9ja7snXOfGH53Q4/e/OLVP0NE1ioiIiIiIiIiIiIiIiIiIiIiIiMgnz8mnnLLbu++8/UgUhf3ev5ZMpigrKweIqmtqb3h00iM/2tbu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'></td></tr></tbody></table><table align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='padding:25px 25px'><tbody><tr><td><h1 style='color:#333;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen,Ubuntu,Cantarell,Fira Sans,Droid Sans,Helvetica Neue,sans-serif;font-size:17px;font-weight:700;margin-bottom:15px'>El Sistema Supervisor de Embarques Notifica La Cancelación De Una Tarima En La Siguiente Orden</h1><table align='center' width='100%' border='0' cellpadding='3' cellspacing='0' role='presentation' class='miTabla'><thead><tr style='background-color:#f6f6f6'><th colspan='8' class='bg-gray'>CANCELACIÓN DE TARIMAS DE EMBARQUES</th></tr><tr align='center' style='background-color:#1f2937;color:#fff'><th>Orden</th><th>Recibo</th><th>Producto</th><th>Tarima</th><th>Cajas</th><th>Responsable</th></tr></thead><tbody><tr align='LEFT' class='brBottom'><td align='center'>" + orden + "</td><td align='center'>" + recibo + "</td><td align='right'>" + producto + "</td><td align='center'>" + tarima + "</td><td align='center'>" + cajas + "</td><td align='right'>" + responsable + "</td></tr></table></td></tr></tbody></table><h4>PUEDES VALIDAR ESTA INFORMACION CONSULTANDO EN EL MONITOR DE FECHAS DE CADUCIDADES DE SIPGAB</h4><hr style='width:100%;border:none;border-top:1px solid #eaeaea'><table align='center' width='100%' border='0' cellpadding='0' cellspacing='0' role='presentation' style='padding:25px 35px'><tbody><tr><td><p style='text-align:justify;font-weight:light;font-size:12px;line-height:24px;margin:0;color:#333;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen,Ubuntu,Cantarell,Fira Sans,Droid Sans,Helvetica Neue,sans-serif'>Este correo electronico contiene información referente a la cancelación de una tarima la cual fue eliminada por alguno de los siguientes motivos tal como sobrepeso, reacomodo de carga, cancelacion del pedido, etc. .</p></td></tr></tbody></table></td></tr></tbody></table><p style='text-align:justify;font-size:10px;line-height:24px;margin:24px 0;color:#333;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen,Ubuntu,Cantarell,Fira Sans,Droid Sans,Helvetica Neue,sans-serif;padding:0 20px'>Este mensaje fue producido y distribuido por CargaEmbarques una aplicación móvil de Mr. Lucky, Carretera Panamericana km 5 Colonia Rancho Grande CP 36544, Irapuato, Guanajuato, MX. © 2024, Mr. Lucky. Todos los derechos reservados. 'CargaEmbarques' es una marca registrada de Mr. Lucky. Consulte nuestra política de privacidad.<a href='http://mrlucky.com.mx' style='color:#2754c5;text-decoration:underline;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen,Ubuntu,Cantarell,Fira Sans,Droid Sans,Helvetica Neue,sans-serif;font-size:14px' target='_blank'>mrlucky.com.mx</a>, Inc. View our<a href='http://mrlucky.com.mx' style='color:#2754c5;text-decoration:underline;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen,Ubuntu,Cantarell,Fira Sans,Droid Sans,Helvetica Neue,sans-serif;font-size:14px' target='_blank'>privacy policy</a>.</p></td></tr></tbody></table></body></html>";



            return body;
        }
    }
}