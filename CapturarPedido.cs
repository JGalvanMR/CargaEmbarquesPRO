using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.Net;
using Android.Net.Wifi;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Fragment.App;
using CargaEmbarques.Modal;
using CargaEmbarques.Services;
using Java.Lang;
using Java.Net;
using Java.Util;
using Java.Util.Functions;
using Org.Json;
using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Android.Provider.ContactsContract.CommonDataKinds;
using AlertDialog = Android.App.AlertDialog;


namespace CargaEmbarques
{
    [Activity(Label = "INGRESAR PEDIDO", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class CapturarPedido : Activity, ILocationListener
    {
        #region VARIABLES
        public static string cadenaConexion = "Persist Security Info=False;user id=sa; password=Gabira2026$;Initial Catalog =GAB_Irapuato; server=tcp:189.206.160.206,2352; Connect Timeout = 130";
        //public static string cadenaConexion = "Persist Security Info=False;user id=sa; password=Gabira2026$;Initial Catalog =GAB_Irapuato; server=tcp:192.168.123.6,1433; Connect Timeout = 0";
        WSCargaEmbarques192.WebServiceEmbarques notificarFalloEtiquetasLocal = new WSCargaEmbarques192.WebServiceEmbarques();
        WSCargaEmbarques189.WebServiceEmbarques notificarFalloEtiquetas = new WSCargaEmbarques189.WebServiceEmbarques();
        WSCargaEmbarques192.WebServiceEmbarques notificarCargaExcesivaLocal = new WSCargaEmbarques192.WebServiceEmbarques();
        WSCargaEmbarques189.WebServiceEmbarques notificarCargaExcesiva = new WSCargaEmbarques189.WebServiceEmbarques();
        WSCargaEmbarques192.WebServiceEmbarques proxyLocal = new WSCargaEmbarques192.WebServiceEmbarques();
        WSCargaEmbarques189.WebServiceEmbarques proxy = new WSCargaEmbarques189.WebServiceEmbarques();
        public static int valido = 0, veces = 0;
        public static string cvvehiculo, cvresponsable, Version = "12.2";
        public static string vehiculo, responsable, pedidoEMB;
        public string Nombre = "", Mtipo = "", MProd = "", MTar = "", MFol = "", mUser = "", mAutoriza = "", user = "", motfolade = "", ALTA = "";
        public string cvecam = "", muser = "", mconcen = "1";
        public static string AutoPed = "N";
        public int proceso = 0;
        public static string EtiquetaExiste = "S", EtiquetaCapturada = "S", FechaCaducada = "S", OrdenExiste = "S";
        public static string HayExistencias = "S";
        public static string Surtidomayor = "S";
        public static string ValiFechacad = "S";
        public static string EstructuraEtiqueta = "S";
        string dondegenera = "";
        string ModificaPedido = "N";
        string tipopedido = "";
        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        SqlDataAdapter da;
        DataSet ds = new DataSet();
        SqlCommand cmnd = new SqlCommand();
        SqlCommand cmnd1 = new SqlCommand();
        SqlDataReader reader1;
        public static DataTable det_pedidos = new DataTable("det_pedidos");
        public static DataTable det_pedidos2 = new DataTable("det_pedidos2");
        public static DataTable productos_leidos = new DataTable("productos_leidos");
        string query = "", prod_clave = "", folio = "", tipo = "", cadena = "", prod_nombre = "";
        int tarima = 0, caja = 0, tarimaf = 0;
        bool find = false;
        ArrayAdapter<System.String> comboAdapter;
        System.String[] strFrutas;
        string motivoautorizafechaadelantada = "";

        public static DataTable det_producto_sin_blanca = new DataTable("det_producto_sin_blanca");

        public static DataTable TbPed = new DataTable("det_pedidos");

        public static Int32 foliocampo = 0;

        public Int32 pdn_diasmin = 12;

        public string tipoped = "";

        public static string imei = "";

        public int AndenValida = 0;

        static int PICK_CONTACT_REQUEST = 1;

        // FIX #3: requestCodes únicos por permiso (antes colisionaban en uno solo).
        const int REQ_LOCATION = 100;
        const int REQ_WRITE_STORAGE = 101;
        const int REQ_READ_STORAGE = 102;

        DataTable CatProd = new DataTable();

        //Declarar los datos de los items en el layout CapturarSplit
        EditText pedido;
        TextView peso;
        TextView fecha;
        TextView lugar;
        TextView Notrailer;
        TextView Anden;
        TextView horainicial;
        TextView Horafinal;
        Spinner Ordenes;
        EditText codigoetiqueta;
        EditText temperatura;
        Spinner TipoTar;
        EditText Posicion;
        EditText Cajas;
        Button iniarCarga;
        TextView LblFT;
        IMenu Mymenu;

        EditText confirmprod;

        EditText password;

        EditText et;
        //opcones del menu
        Button fotoevent;

        int Segundos = 0;

        private NfcAdapter _nfcAdapter;

        //Declaracion de variables de GEOLOCALIZACION y ubicacion
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
        string latitud = "";
        string longitud = "";

        //Termino Declaracion de items

        Int32 TotCaj;

        string valorfinal = "";

        string montacarguista = "";

        //Variables de solicitud al servidor si realiza o no guardado de datos de la bd interna a la bd del servidor antes de borrar

        Context context;
        Runnable listener;
        //private static string INFO_FILE = "http://192.168.123.4:81/EmbarquesApk/estado_respaldo.txt";
        private static string INFO_FILE = "http://189.206.160.206:81/EmbarquesApk/estado_respaldo.txt";
        private int respaldo_activo = 1;
        System.Timers.Timer Timer1 = new System.Timers.Timer();

        //Variables de Validacion***************************************************************************************************

        string FolioAtrasado = "";
        string FechaAtrasada = "";
        string TarimaAtrasada = "";
        string FolioLeido = "";
        string FechaLeido = "";
        string TarimaLeido = "";
        string Producto = "";
        string Productocve = "";
        string CajasDisp = "";
        string PenXAuto = "";
        string ConsultaInserFolioAdelantado = "";
        string CapturaSplitActiva = "0";
        string tipotarima = "";
        //Variables de Validacion***************************************************************************************************
        string SerialShippingContainerCode = "0000796631";
        string patron = @"^00007966310*([1-9]\d*).$";
        string patronPTI_Famous = @"^0+";
        string FolioProducto = "";
        string TarimaProducto = "";
        string ProductoProducto = "";

        int v_dif;

        string cnte_clave = "", cve_subcli = "", validaVidaAnaquel = "";
        DataTable ProductosVidaAnaquel = new DataTable();
        private TeamsNotifier notiTeams;

        // Reemplaza la inicialización con tipo de destino (C# 9.0+) por la sintaxis compatible con C# 8.0
        private readonly CargaEmbarques.Services.ATUService _atuService = new CargaEmbarques.Services.ATUService();
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LecturaCarga);

            //Declaracion de los id de cada elemento
            pedido = FindViewById<EditText>(Resource.Id.pedingres);
            peso = FindViewById<TextView>(Resource.Id.pesoactual);
            fecha = FindViewById<TextView>(Resource.Id.fechaactual);
            Ordenes = FindViewById<Spinner>(Resource.Id.cmbpedido);
            lugar = FindViewById<TextView>(Resource.Id.NalExp);
            Notrailer = FindViewById<TextView>(Resource.Id.NumTrailer);
            Anden = FindViewById<TextView>(Resource.Id.Andencapturado);
            horainicial = FindViewById<TextView>(Resource.Id.iniciocarga);
            Horafinal = FindViewById<TextView>(Resource.Id.fincarga);
            codigoetiqueta = FindViewById<EditText>(Resource.Id.codigoProducto);
            temperatura = FindViewById<EditText>(Resource.Id.Temp);
            TipoTar = FindViewById<Spinner>(Resource.Id.TipoTar);
            Posicion = FindViewById<EditText>(Resource.Id.txtpos);
            Cajas = FindViewById<EditText>(Resource.Id.cancaj);
            iniarCarga = FindViewById<Button>(Resource.Id.IniciarCarga);
            iniarCarga.Click += BtnIniciarCarga_Click;
            LblFT = FindViewById<TextView>(Resource.Id.LblFT);
            fotoevent = FindViewById<Button>(Resource.Id.Fotoevent);
            fotoevent.Click += Btnevent_Click;
            confirmprod = FindViewById<EditText>(Resource.Id.codigoconfirmProducto);
            confirmprod.Enabled = false;

            #region Deshabilitar el menú de pegado
            codigoetiqueta.CustomSelectionActionModeCallback = new NoPasteCallback();
            codigoetiqueta.LongClickable = false;
            codigoetiqueta.SetTextIsSelectable(false);

            confirmprod.CustomSelectionActionModeCallback = new NoPasteCallback();
            confirmprod.LongClickable = false;
            confirmprod.SetTextIsSelectable(false);
            #endregion


            thisConnection.Open();
            string CadenaTraerProductoSinBlanca = "SELECT prod_clave FROM Tb_Det_DiseñoEtiquetaXCliente";
            SqlDataAdapter daProductoSinBlanca = new SqlDataAdapter(CadenaTraerProductoSinBlanca, thisConnection);
            DataSet dsProductoSinBlanca = new DataSet();
            daProductoSinBlanca.Fill(dsProductoSinBlanca, "det_producto_sin_blanca");
            det_producto_sin_blanca = dsProductoSinBlanca.Tables["det_producto_sin_blanca"];
            thisConnection.Close();

            InitializeLocationManager();

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cmnd = thisConnection.CreateCommand();
            cmnd.CommandText = "select inicio_campo from Tb_folio_campo";
            foliocampo = Convert.ToInt32(cmnd.ExecuteScalar());
            ds.Clear();
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            cvresponsable = Intent.GetStringExtra("cvresponsable");
            responsable = Intent.GetStringExtra("responsable");
            imei = Intent.GetStringExtra("imei");

            AsignarAnden();
            #region PRUEBA CORREO CARGA EXCESIVA
            //string not = setBodyEmail("084451", "06006PM10I");
            //notificarCargaExcesiva.SendMail("jgalvan@mrlucky.com.mx", not, "CARGA EXCESIVA EN ORDEN DE EMBARQUES");
            #endregion
            Cajas.FocusChange += (Sender, args) =>
            {
                if (Cajas.HasFocus == true)
                {
                    InputMethodManager immx = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    immx.ShowSoftInput(Cajas, ShowFlags.Implicit);
                }
            };

            Posicion.KeyPress += (sender, e) =>
            {
                Cajas.Enabled = false;
                int UltPos = 0;
                int mtar = 0;
                if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter))
                {
                    int i = 0;
                    #region VALIDACIÓN - TIPO DE DATO DE LA POSCICION
                    if (int.TryParse(Posicion.Text.Trim(), out i) == false)
                    {
                        Toast.MakeText(this, "El dato debe ser numerico", ToastLength.Long).Show();
                        Posicion.Text = "";
                        Posicion.RequestFocus();
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ShowSoftInput(Posicion, ShowFlags.Implicit);
                        return;
                    }
                    #endregion
                    #region VALIDACION - LONGITUD DE LA POSICION
                    if (Posicion.Text.Trim().Length > 2)
                    {
                        Toast.MakeText(this, "El dato debe ser de 2 posiciones", ToastLength.Long).Show();
                        Posicion.Text = "";
                        Posicion.RequestFocus();
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ShowSoftInput(Posicion, ShowFlags.Implicit);
                        return;
                    }
                    #endregion
                    #region VALIDACION  - POSICION DEBE SER MENOR DE 30
                    if (Convert.ToInt32(Posicion.Text.Trim()) > 30)
                    {
                        Toast.MakeText(this, "La Posicion no puede ser mayor a 30", ToastLength.Long).Show();
                        Posicion.Text = "";
                        Posicion.RequestFocus();
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ShowSoftInput(Posicion, ShowFlags.Implicit);
                        return;
                    }
                    #endregion
                    #region VALIDACION - TIPO DE LA TARIMA
                    if (tipotarima == "TAR")
                    {
                        Toast.MakeText(this, "La Tarima es Invalida, Seleccionar correctamente", ToastLength.Long).Show();
                        TipoTar.RequestFocus();
                        return;
                    }
                    #endregion
                    // NO_TRAILER = '" + Notrailer.Text + "' and fecha = '" + fecha.Text + "'
                    #region VALIDACION - POSICIÓN SIGUIENTE
                    if (Convert.ToInt32(Posicion.Text) > 1)
                    {
                        if (thisConnection.State == ConnectionState.Closed)
                        {
                            thisConnection.Open();
                        }
                        query = "Select Id_Tarima, Pdn_Folio, Posicion, Pdn_Fecha From Tb_Det_Tar Where No_Trailer = '" + Notrailer.Text + "' and pdn_fecha = '" + fecha.Text + "'";
                        SqlCommand cmd = new SqlCommand(query);
                        cmd.Connection = thisConnection;
                        SqlDataReader Info;
                        Info = cmd.ExecuteReader();
                        while (Info.Read())
                        {
                            if (Convert.ToInt32(Info["Posicion"].ToString().Trim()) > UltPos)
                            {
                                UltPos = Convert.ToInt32(Info["Posicion"].ToString().Trim());
                            }
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                        if (UltPos + 1 < Convert.ToInt32(Posicion.Text))
                        {
                            Toast.MakeText(this, "Advertencia: Se esta saltando la posicion, la que sigue es la " + UltPos + 1, ToastLength.Long).Show();
                            return;
                        }
                    }
                    #endregion
                    #region VALIDACION - DESTINO DEL EMBARQUE
                    switch (lugar.Text.Trim())
                    {
                        case "Cancún":
                            tipopedido = "FC"; ;
                            break;
                        case "Guadalajara":
                            tipopedido = "FG"; ;
                            break;
                        case "Distrito Federal":
                            tipopedido = "FD"; ;
                            break;
                        case "Externos":
                            tipopedido = "FE"; ;
                            break;
                        case "Puerto Vallarta":
                            tipopedido = "FV"; ;
                            break;
                        case "Cuautitlan":
                            tipopedido = "FM"; ;
                            break;
                        case "Exportación":
                            tipopedido = "EXP"; ;
                            break;
                        case "Nacional":
                            tipopedido = "NAL"; ;
                            break;
                        case "Maquila":
                            tipopedido = "TRA"; ;
                            break;
                    }
                    #endregion

                    #region VALIDACION - ANTES DE ENTRAR A SPLIT ACTIVA O INACTIVA
                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string Cadenavalidaposicion = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO, obs) " +
                           "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" +
                           pedido.Text.Trim() + "','Embarque Posicion " + Posicion.Text + " Validar Embarque','CARGAEMB','" + pedido.Text.Trim() + "', 'Validacion Antes de entrar a split captura split activa = " + CapturaSplitActiva + " NAL/EXP = " + tipopedido.Trim() + "')";
                    SqlCommand cmdvalidaposicion = new SqlCommand(Cadenavalidaposicion, thisConnection);
                    cmdvalidaposicion.ExecuteNonQuery();
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                    #endregion

                    #region VALIDACION - CAPTURA DE SPLIT ACTIVA
                    if (CapturaSplitActiva == "1")
                    {
                        string hay = "N";

                        #region OBTENER INFORMACIÓN DE LA POSICION DE LA TARIMA DENTRO DEL EMBARQUE
                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        //DateTime fechasx = Convert.ToDateTime(fecha.Text);
                        string[] fechasx = fecha.Text.Split(' ');
                        string fechasxi = fechasx[0];
                        query = "Select Id_Tarima, Pdn_Folio, Posicion, Pdn_Fecha From Tb_Det_Tar Where Posicion = '" + Posicion.Text.Trim() + "' and No_Trailer = '" + Notrailer.Text + "' and pdn_fecha = '" + fechasxi + "'";
                        SqlCommand cmd = new SqlCommand(query);
                        cmd.Connection = thisConnection;
                        SqlDataReader Info;
                        Info = cmd.ExecuteReader();
                        while (Info.Read())
                        {
                            hay = "S";
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                        #endregion

                        #region OBTENER EL IDENTIFICADOR DE LA TARIMA DENTRO DEL CATALOGO DE TARIMAS
                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        query = "Select Id_Tarima, Nom_Tarima From Tb_Cat_Tarima Where Nom_Tarima = '" + tipotarima.Trim() + "'";
                        cmd = new SqlCommand(query);
                        cmd.Connection = thisConnection;
                        Info = cmd.ExecuteReader();
                        string CveTar = "";
                        while (Info.Read())
                        {
                            CveTar = Info["Id_Tarima"].ToString().Trim();
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                        #endregion

                        #region SI LA POSICION DE LA NUEVA TARIMA NO EXISTE DENTRO DEL EMBARQUE SE AÑADE
                        if (hay == "N")
                        {
                            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                            string cadena = "Insert into Tb_Det_Tar (Id_Tarima, Pdn_Folio, Posicion, Pdn_Fecha, No_Trailer, FechaCap, OPCAP) Values ('" + CveTar + "', '" + pedido.Text.Trim() + "', '" + Posicion.Text + "',' " + fecha.Text + "','" + Notrailer.Text + "','" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "','X')";
                            cmd = new SqlCommand(cadena, thisConnection);
                            cmd.ExecuteNonQuery();
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                        }
                        #endregion

                        #region OBTENER EL DETALLE DEL SPLIT LEIDO
                        string[] numtarnumcajas = codigoetiqueta.Text.Split(", ");
                        mtar = Convert.ToInt32(numtarnumcajas[0]);

                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        query = "Select * from tb_det_split where emb_folio = '" + pedido.Text + "' and tarima = '" + mtar + "' and estatus = 'A'";
                        cmd = new SqlCommand(query);
                        cmd.Connection = thisConnection;
                        Info = cmd.ExecuteReader();
                        //string CveTar = "";
                        string szSQL = "", b = "", c = "", h = "", TarI = "", TarF = "", d = "", FLote = "", TarT = "", FCad = "", TSPL = "", FCap = "";
                        int f = 0;
                        while (Info.Read())
                        {
                            b = Info["prod_clave"].ToString().Trim();
                            c = Info["emb_tipo"].ToString().Trim();
                            d = Info["no_lote"].ToString().Trim();
                            f = Convert.ToInt32(Info["cajas"].ToString().Trim());
                            h = Info["tipo_rec"].ToString().Trim();
                            TSPL = Info["TARINI"].ToString().Trim();
                            TarI = Convert.ToInt32(Info["TARINI"].ToString().Trim()).ToString();
                            TarF = Convert.ToInt32(Info["TARFIN"].ToString().Trim()).ToString();
                            FCad = Info["DIACAD"].ToString().Trim() + Info["MESCAD"].ToString().Trim();
                            FCap = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                            FCap = FCap.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.");

                            if (h == "PTP")
                            {
                                FLote = d.Trim() + b.Trim() + TarI.PadLeft(3, ' ').Trim();
                                TarT = TarI.PadLeft(3, ' ');
                                string Cadenasz = "Select num_lote from tb_det_Eti_Final where folio = '" + d + "' and tarima = '" + Convert.ToInt32(TarI.Trim()) + "' and cve_prod = '" + b.Trim() + "'";
                                SqlCommand cmdx = new SqlCommand(Cadenasz, thisConnection);
                                FCad = cmdx.ExecuteScalar().ToString();
                            }
                            else
                            {
                                FLote = d.Trim() + b.Trim() + TarI.PadLeft(2, '0').Trim() + TarF.PadLeft(2, '0').Trim();
                                TarT = TarI.PadLeft(2, '0');
                            }

                            szSQL = "IF EXISTS(SELECT emb_folio FROM tb_det_split WHERE emb_folio = '" + pedido.Text.Trim() + "' AND no_lote = '" + d + "' AND  prod_clave = '" + b + "' AND cajas = '" + f + "' AND tarima = '" + mtar + "' AND TARINI = '" + TSPL + "' AND Estatus = 'A') BEGIN IF NOT EXISTS(SELECT emb_folio FROM tb_det_embarque WHERE emb_folio = '" + pedido.Text.Trim() + "' AND no_lote = '" + FLote + "' AND  prod_clave = '" + b + "' AND tarima = '" + TarI + "' AND tipo_rec = '" + h + "' AND recibo = '" + d.Trim() + "' AND OpCap = 'X' AND Estatus = 'A' AND seccion = '" + Posicion.Text + "') BEGIN Insert into tb_det_embarque (emb_folio, prod_clave, no_lote, cajas, seccion, temp, emb_tipo, tarima, tarima_f, tipo_rec, estatus,FEC_CAD,FECHACAP,OPCAP,ID_TARIMA,RECIBO,FECHACAD, id_Lectora, datecaptura, latitud, longitud)  Values ('" + pedido.Text.Trim() + "','" + b + "','" + FLote + "','" + f + "','" + Posicion.Text + "','" + temperatura.Text.Trim() + "','" + c + "','" + TarI + "','" + TarF + "','" + h + "','A','" + FCad + "','" + FCap + "','X','" + CveTar + "','" + d.Trim() + "','" + FCad.Trim() + "', '" + imei + "', GETDATE(), '" + latitud + "', '" + longitud + "') END ELSE BEGIN UPDATE tb_det_embarque SET cajas = cajas + '" + f + "' WHERE emb_folio = '" + pedido.Text.Trim() + "' AND no_lote = '" + FLote + "' AND  prod_clave = '" + b + "' AND tarima = '" + TarI + "' AND tipo_rec = '" + h + "' AND recibo = '" + d.Trim() + "' AND OpCap = 'X' AND Estatus = 'A' AND seccion = '" + Posicion.Text + "' END Update tb_det_split set estatus = 'S' where emb_folio = '" + pedido.Text.Trim() + "' AND no_lote = '" + d + "' AND  prod_clave = '" + b + "' AND cajas = '" + f + "' AND tarima = '" + mtar + "' AND TARINI = '" + TSPL + "' END";
                            SqlCommand szcmd = new SqlCommand(szSQL, thisConnection);
                            szcmd.ExecuteNonQuery();
                        }
                        #endregion

                        #region INSERT INTO TB_REGISTRO_MOVIMIENTOS
                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        string Cadproduct = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                               "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" +
                               pedido.Text.Trim() + "','Posicion " + Posicion.Text + " Mod " + (Convert.ToInt32(Posicion.Text.Trim()) % 2) + " NAL','CARGAEMB','" + pedido.Text.Trim() + "')";
                        SqlCommand cm = new SqlCommand(Cadproduct, thisConnection);
                        cm.ExecuteNonQuery();
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                        #endregion

                        #region VALIDA QUE CADA 2 POSICIONES DE TARIMAS SE HABILITE EL BOTON FOTO PARA SPLITS DE NACIONAL
                        if ((Convert.ToInt32(Posicion.Text.Trim()) % 2) == 0)
                        {
                            #region INSERT INTO TB_REGISTRO_MOVIMIENTOS
                            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                            string Cadenaproductonoesta = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                   "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" +
                                   pedido.Text.Trim() + "','Embarque Posicion " + Posicion.Text + " Activar Boton Foto Split NAL','CARGAEMB','" + pedido.Text.Trim() + "')";
                            SqlCommand cmdx = new SqlCommand(Cadenaproductonoesta, thisConnection);
                            cmdx.ExecuteNonQuery();
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            #endregion

                            //fotoevent.PerformClick();
                            //fotoevent.Visibility = Android.Views.ViewStates.Visible;
                            fotoevent.Enabled = true;
                            fotoevent.RequestFocus();
                        }
                        else
                        {
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            codigoetiqueta.Text = "";
                            codigoetiqueta.Enabled = false;
                            confirmprod.Text = "";
                            confirmprod.Enabled = false;
                            Posicion.Text = "";
                            Posicion.Enabled = false;
                            temperatura.Text = "";
                            temperatura.Enabled = false;
                            TRAE_PESO();
                            Cajas.Text = "0";
                            Cajas.Enabled = false;
                            //fotoevent.Visibility = Android.Views.ViewStates.Invisible;
                            fotoevent.Enabled = false;
                            TipoTar.Enabled = false;
                            //codigoetiqueta.Text = "";
                            codigoetiqueta.Enabled = true;
                            codigoetiqueta.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                        }
                        #endregion
                    }
                    #endregion
                    #region VALIDACION - CAPTURA DE SPLIT INACTIVA
                    else
                    {
                        if (tipopedido.Trim() != "EXP")
                        {
                            #region INSERT INTO TB_REGISTRO_MOVIMIENTOS
                            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                            string Cadproduct = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                   "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" +
                                   pedido.Text.Trim() + "','Posicion " + Posicion.Text + " Mod " + (Convert.ToInt32(Posicion.Text.Trim()) % 2) + " NAL','CARGAEMB','" + pedido.Text.Trim() + "')";
                            SqlCommand cm = new SqlCommand(Cadproduct, thisConnection);
                            cm.ExecuteNonQuery();
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            #endregion

                            #region VALIDA QUE CADA 2 POSICIONES DE TARIMAS SE HABILITE EL BOTON FOTO PARA TARIMAS COMPLETAS DE NACIONAL
                            if ((Convert.ToInt32(Posicion.Text.Trim()) % 2) == 0)
                            {
                                #region INSERT INTO TB_REGISTRO_MOVIMIENTOS
                                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                                string Cadenaproductonoesta = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                       "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" +
                                       pedido.Text.Trim() + "','Embarque Posicion " + Posicion.Text + " Activar Boton Foto Tarima Completa NAL','CARGAEMB','" + pedido.Text.Trim() + "')";
                                SqlCommand cmdx = new SqlCommand(Cadenaproductonoesta, thisConnection);
                                cmdx.ExecuteNonQuery();
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                #endregion

                                //fotoevent.PerformClick();
                                //fotoevent.Visibility = Android.Views.ViewStates.Visible;
                                fotoevent.Enabled = true;
                                fotoevent.RequestFocus();
                            }
                            else
                            {
                                //GuardarInformacion();
                                Cajas.Enabled = true;
                                Cajas.Text = Cajas.Text.Trim();
                                Cajas.RequestFocus();
                                InputMethodManager immx = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                immx.ShowSoftInput(Cajas, ShowFlags.Implicit);
                            }
                            #endregion
                        }
                        else
                        {
                            #region INSERT INTO TB_REGISTRO_MOVIMIENTOS
                            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                            string Cadproduct = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                   "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" +
                                   pedido.Text.Trim() + "','Posicion " + Posicion.Text + " Mod " + (Convert.ToInt32(Posicion.Text.Trim()) % 2) + " EXP','CARGAEMB','" + pedido.Text.Trim() + "')";
                            SqlCommand cm = new SqlCommand(Cadproduct, thisConnection);
                            cm.ExecuteNonQuery();
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            #endregion

                            #region VALIDA QUE CADA 2 POSICIONES DE TARIMAS SE HABILITE EL BOTON FOTO PARA TARIMAS COMPLETAS DE EXPORTACION
                            if ((Convert.ToInt32(Posicion.Text.Trim()) % 2) == 0)
                            {
                                #region INSERT INTO TB_REGISTRO_MOVIMIENTOS
                                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                                string Cadenaproductonoesta = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                       "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" +
                                       pedido.Text.Trim() + "','Embarque Posicion " + Posicion.Text + " Activar Boton Foto EXP','CARGAEMB','" + pedido.Text.Trim() + "')";
                                SqlCommand cmdx = new SqlCommand(Cadenaproductonoesta, thisConnection);
                                cmdx.ExecuteNonQuery();
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                #endregion

                                //fotoevent.Visibility = Android.Views.ViewStates.Visible;
                                fotoevent.Enabled = true;
                                fotoevent.RequestFocus();

                            }
                            else
                            {
                                Cajas.Enabled = true;
                                Cajas.Text = Cajas.Text.Trim();
                                Cajas.RequestFocus();
                                InputMethodManager immx = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                immx.ShowSoftInput(Cajas, ShowFlags.Implicit);
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                else
                {
                    e.Handled = false;
                }
            };

            Cajas.EditorAction += (sender, e) =>
            {
                if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next || e.ActionId == ImeAction.ImeNull)
                {
                    if (CapturaSplitActiva == "1")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        GuardarInformacion();
                    }
                }
                else
                {
                    e.Handled = false;
                }
            };

            temperatura.KeyPress += (sender, e) =>
            {
                #region VALIDACION DE TEMPERATURA
                if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter))
                {
                    #region VALIDACION - LONGITUD DEL VALOR TEMPERATURA MAYOR A CERO
                    if (temperatura.Text.Trim().Length > 0)
                    {
                        #region VALIDACION - LONGITUD DE TEMPERATURA DEBE DE SER DE DOS POSICIONES
                        if (temperatura.Text.Trim().Length > 2)
                        {
                            Toast.MakeText(this, "El dato debe ser de 2 posiciones", ToastLength.Long).Show();
                            temperatura.Text = "";
                            temperatura.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(temperatura, ShowFlags.Implicit);
                        }
                        #endregion
                        #region VLAIDACION -  TIPO DE DATO NUMERICO DE TEMPERATURA
                        int i = 0;
                        if (int.TryParse(temperatura.Text.Trim(), out i) == false)
                        {
                            Toast.MakeText(this, "El dato debe ser numerico", ToastLength.Long).Show();
                            temperatura.Text = "";
                            temperatura.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(temperatura, ShowFlags.Implicit);
                        }
                        #endregion
                        #region VALIDACION -  EL VALOR DE TEMPERATURA NO DEBE DE EXCEDER DE 60
                        if (Convert.ToInt32(temperatura.Text.Trim()) > 59)
                        {
                            Toast.MakeText(this, "El Valor de la Temperatura no puede ser Mayor a 60", ToastLength.Long).Show();
                            temperatura.Text = "";
                            temperatura.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(temperatura, ShowFlags.Implicit);
                        }
                        #endregion
                        #region VALIDACION - EL VALOR DE TEMPERATURA NO DEBE DE SER CERO
                        if (Convert.ToInt32(temperatura.Text.Trim()) == 0)
                        {
                            Toast.MakeText(this, "El Valor de la Temperatura no puede ser Igual a 0", ToastLength.Long).Show();
                            temperatura.Text = "";
                            temperatura.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(temperatura, ShowFlags.Implicit);
                        }
                        #endregion
                        #region OBTENER EL VALOR DE LA POSICION SIGUIENTE AL ULTIMO REGISTRO ENCONTRADO EN TB_DET_EMBARQUE
                        int posicionsiguiente = 0;
                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        cadena = "SELECT A.prod_clave, A.cajas, A.tipo_rec, A.recibo, A.seccion FROM tb_det_embarque AS A INNER JOIN tb_mstr_embarque AS B ON A.emb_folio = B.emb_folio AND A.emb_tipo = B.emb_tipo WHERE A.Estatus != 'C' AND B.no_trailer = '" + Notrailer.Text + "' AND B.hora_trailer = '" + fecha.Text + "' ORDER BY A.seccion";
                        SqlCommand cmd = new SqlCommand(cadena);
                        SqlDataReader Info;
                        cmd.Connection = thisConnection;
                        Info = cmd.ExecuteReader();
                        while (Info.Read())
                        {
                            posicionsiguiente = Convert.ToInt32(Info["seccion"].ToString().Trim());
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                        Posicion.Enabled = true;
                        posicionsiguiente = posicionsiguiente + 1;
                        Posicion.Text = posicionsiguiente.ToString().Trim();
                        Posicion.Enabled = false;
                        #endregion
                        TipoTar.Enabled = true;
                        TipoTar.Focusable = true;
                        //TipoTar.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_tarimas);
                        TipoTar.FocusableInTouchMode = true;
                        TipoTar.FocusChange += (Sender, args) => { DropDownFocusChanged(sender, args); };
                        TipoTar.RequestFocus();
                    }
                    #endregion
                }
                else
                {
                    e.Handled = false;
                }
                #endregion
            };

            Posicion.FocusChange += (senderx, e) =>
            {
                bool hasFocus = e.HasFocus;
                if (hasFocus)
                {
                    Cajas.Enabled = false;
                }
            };

            TipoTar.ItemSelected += (sender, args) =>
            {
                tipotarima = "";
                //throw new NotImplementedException();
                Spinner spinner = (Spinner)sender;
                tipotarima = spinner.GetItemAtPosition(args.Position).ToString();
                if (tipotarima != "TAR")
                {
                    int posicionsiguiente = 0;
                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    cadena = "SELECT A.prod_clave, A.cajas, A.tipo_rec, A.recibo, A.seccion FROM tb_det_embarque AS A INNER JOIN tb_mstr_embarque AS B ON A.emb_folio = B.emb_folio AND A.emb_tipo = B.emb_tipo WHERE A.Estatus != 'C' AND B.no_trailer = '" + Notrailer.Text + "' AND B.hora_trailer = '" + fecha.Text + "' ORDER BY A.seccion ASC";
                    SqlCommand cmd = new SqlCommand(cadena);
                    SqlDataReader Info;
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        posicionsiguiente = Convert.ToInt32(Info["seccion"].ToString().Trim());
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    Posicion.Enabled = true;
                    posicionsiguiente = posicionsiguiente + 1;
                    Posicion.Text = posicionsiguiente.ToString().Trim();
                    Posicion.RequestFocus();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(Posicion, ShowFlags.Implicit);
                }
            };

            ////COMIENZA VALIDACION ETIQUETA BLANCA
            //confirmprod.InputType = Android.Text.InputTypes.Null;
            confirmprod.KeyPress += (senderx, ex) =>
            {
                ex.Handled = false;
                string etiVerdeCapturada = confirmprod.ToString();
                string loteCapturado = "";
                string clvProductoCapturado = "";
                string tarimaCapturada = "";
                string cajaCapturada = "";
                string prod = "";
                //if (ex.Event.Action == KeyEventActions.Down && ex.KeyCode == Keycode.Enter)
                if ((ex.Event.Action == KeyEventActions.Up) && (ex.KeyCode == Keycode.Enter))
                {
                    //if (confirmprod.Text.Contains("http://www.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") != true && confirmprod.Text.Contains("http://gab.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") != true)
                    if ((confirmprod.Text.Contains("HTTP://WWW.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=") == false && confirmprod.Text.Contains("http://www.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") == false) && (confirmprod.Text.Contains("HTTP://GAB.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=") == false && confirmprod.Text.Contains("http://gab.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") == false))
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>🔴 Etiqueta no reconocida</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La etiqueta escaneada no pudo ser validada. Por favor, revisa que el código sea legible y vuelve a intentarlo. ¡Tu atención nos ayuda a un mejor registro!</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                            confirmprod.Text = "";
                        });
                        alertDialog.Show();
                    }
                    else
                    {
                        try
                        {
                            DataTable CatProd = new DataTable();
                            if (thisConnection.State == ConnectionState.Closed)
                            {
                                thisConnection.Open();
                            }
                            string cade = "Select prod_clave,prod_nombre from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC') order by LEN(prod_clave) DESC";
                            SqlDataAdapter da = new SqlDataAdapter(cade, thisConnection);
                            DataSet ds = new DataSet();
                            da.Fill(ds, "CatProd");
                            CatProd = ds.Tables["CatProd"];
                            if (thisConnection.State == ConnectionState.Open)
                            {
                                thisConnection.Close();
                            }

                            string V_Recibo = "", V_Prd = "", V_Existe = "", Mtipo = "", Fechacad = "", fecha_cad = "", diacad = "", mescad = "", prod_nombre = "", mtar = "", preautorizado = "", mcaj = "";
                            string restocaptura = "";

                            string captura = confirmprod.Text.Trim();
                            int posi = 0;
                            posi = captura.IndexOf("=");
                            if (posi == 0)
                            {
                                confirmprod.Text = "";
                                confirmprod.RequestFocus();

                                return;
                            }
                            captura = captura.Replace("http://www.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=", "");
                            captura = captura.Replace("http://gab.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=", "");

                            validarCapturas(captura, out Mtipo, out V_Recibo, out V_Prd, out mtar, out mcaj);
                            if (Mtipo == null || V_Recibo == null || V_Prd == null || mtar == null || mcaj == null)
                            {
                                for (int i = 0; i < CatProd.Rows.Count; i++)
                                {
                                    string producto_clave = CatProd.Rows[i]["Prod_Clave"].ToString().Trim();
                                    bool esta = captura.Contains(producto_clave);

                                    if (esta)
                                    {
                                        V_Prd = producto_clave;
                                        break;
                                    }
                                }

                                prod = V_Prd;
                                int posprod = captura.Trim().IndexOf(V_Prd);
                                V_Recibo = captura.Substring(0, posprod).Trim();
                                Mtipo = "PTP";
                                restocaptura = captura.Replace(V_Recibo, "").Replace(V_Prd, "");
                                if (restocaptura.Length == 6)
                                {
                                    if (V_Recibo.Length == 5)
                                    {
                                        Mtipo = "PTC";
                                    }
                                    //mcaj = restocaptura.Substring(3, 3);
                                    mtar = restocaptura.Substring(0, 3);
                                }
                                else if (restocaptura.Length == 9)
                                {
                                    Mtipo = "PTC";
                                    //mcaj = restocaptura.Substring(6, 3);
                                    mtar = restocaptura.Substring(0, 3);
                                }
                                else
                                {
                                    Mtipo = "PTC";
                                    //mcaj = restocaptura.Substring(4, 3);
                                    mtar = restocaptura.Substring(0, 2);
                                }
                            }




                            #region ValidacionEtiquetaOld
                            /*int tam = 0;
                            tam = captura.Length;
                            string mCaj, Ent, mtarf;
                            mCaj = "";
                            Ent = "N";
                            if (tam > 20)
                            {
                                int valorfolio = Convert.ToInt32(captura.Substring(0, 6));
                                if (valorfolio > foliocampo)
                                {
                                    Ent = "S";
                                }
                            }
                            if (Ent == "N")
                            {
                                mCaj = captura.Substring(tam - 3, 3);
                                mtar = captura.Substring(tam - 6, 3);
                                mtar = Convert.ToInt32(mtar).ToString();
                                int tam2 = tam - 6;
                                Mtipo = "PTP";
                                if (tam2 == 15)
                                {
                                    V_Recibo = captura.Substring(0, 5);
                                    V_Prd = captura.Substring(5, tam - 11);
                                    Mtipo = "PTC";
                                    codigoetiqueta.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                                }
                                else if (tam2 <= 14)
                                {
                                    V_Recibo = captura.Substring(0, 4);
                                    V_Prd = captura.Substring(4, tam - 10);
                                    Mtipo = "PTC";
                                    codigoetiqueta.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                                }
                                else
                                {
                                    V_Recibo = captura.Substring(0, 6);
                                    V_Prd = captura.Substring(6, tam - 12);
                                    codigoetiqueta.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                                }
                                string nombreproducto = "";
                                nombreproducto = traenom(V_Prd);

                                if (nombreproducto == "")
                                {
                                    V_Recibo = captura.Substring(0, 6);
                                    V_Prd = captura.Substring(6, tam - 12);
                                    Mtipo = "PTP";
                                    codigoetiqueta.Text = V_Recibo + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                                }

                                nombreproducto = traenom(V_Prd);

                                if (nombreproducto == "")
                                {
                                    mtar = captura.Substring(tam - 4, 2);
                                    V_Recibo = captura.Substring(0, 6);
                                    V_Prd = captura.Substring(6, tam - 10);
                                    Mtipo = "PTC";
                                    codigoetiqueta.Text = V_Recibo + V_Prd + mtar + mtar;
                                }
                                nombreproducto = traenom(V_Prd);

                                if (nombreproducto == "")
                                {
                                    //mcaj = captura.Substring(tam - 3, 3);
                                    mtar = captura.Substring(tam - 6, 3);
                                    V_Recibo = captura.Substring(0, 5);
                                    V_Prd = captura.Substring(5, tam - 11);
                                    Mtipo = "PTC";
                                    codigoetiqueta.Text = V_Recibo.PadLeft(6, '0') + V_Prd + Convert.ToInt32(mtar).ToString().PadLeft(3, ' ');
                                }
                            }
                            else
                            {
                                mCaj = captura.Substring(tam - 3, 3);
                                mtar = captura.Substring(tam - 7, 2);
                                mtarf = captura.Substring(tam - 5, 2);
                                V_Recibo = captura.Substring(0, 6);
                                V_Prd = captura.Substring(6, tam - 13);
                                Mtipo = "PTC";
                                confirmprod.Text = V_Recibo + V_Prd + mtar + mtarf;
                            }*/
                            #endregion

                            if (Convert.ToInt32(V_Recibo) == Convert.ToInt32(FolioProducto) && V_Prd.Trim() == ProductoProducto.Trim() && Convert.ToInt32(mtar) == Convert.ToInt32(TarimaProducto))
                            {
                                temperatura.Enabled = true;
                                temperatura.RequestFocus();
                                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                imm.ShowSoftInput(temperatura, ShowFlags.Implicit);

                            }
                            else
                            {
                                temperatura.Enabled = false;
                                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>LECTURAS NO COINCIDEN</font>"));
                                alertDialog.SetIcon(Resource.Drawable.no);
                                alertDialog.SetCancelable(false);
                                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>ERROR!! La lectura realizada de la etiqueta Verde contra la etiqueta Blanca no coinciden, Favor de informar al area de calidad o Descargue Segun el Caso para validacion de etiquetas</font>"));
                                alertDialog.SetNeutralButton("Ok", delegate
                                {
                                    alertDialog.Dispose();
                                    //codigoetiqueta.Text = "";
                                    //confirmprod.Text = "";
                                    codigoetiqueta.RequestFocus();
                                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                    imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                                });
                                alertDialog.Show();

                                #region ENVIO DE EMAIL - ERROR AL CONFIRMAR ETIQUETA BLANCA
                                loteCapturado = V_Recibo;
                                clvProductoCapturado = V_Prd;
                                tarimaCapturada = MTar;
                                cajaCapturada = restocaptura;
                                //var notificarFalloEtiquetas = new WebServiceEmbarques.WebServiceEmbarques();
                                //var notificarFalloEtiquetas = new WSEmbarques.WebServiceEmbarques();
                                if (INFO_FILE == "http://192.168.123.4:81/EmbarquesApk/estado_respaldo.txt")
                                {
                                    notificarFalloEtiquetasLocal.SendMail("jgalvan@mrlucky.com.mx", "\nEtiqueta Capturada: " + EtiquetaCapturada + "\nOP|Lote: " + loteCapturado +
                                "\nClave del Producto: " + clvProductoCapturado + "\nTarima: " + tarimaCapturada + "\nCaja: " + cajaCapturada, "Error al confirmar Etiqueta Blanca");
                                }
                                else
                                {
                                    notificarFalloEtiquetas.SendMail("jgalvan@mrlucky.com.mx", "\nEtiqueta Capturada: " + EtiquetaCapturada + "\nOP|Lote: " + loteCapturado +
                                "\nClave del Producto: " + clvProductoCapturado + "\nTarima: " + tarimaCapturada + "\nCaja: " + cajaCapturada, "Error al confirmar Etiqueta Blanca");
                                }


                                #endregion
                            }

                        }
                        catch
                        {
                            SendMail("jgalvan@mrlucky.com.mx", "Ha ocurrido un ERROR al momento de confirmar la etiqueta blanca del PRODUCTO" + prod, "ERROR al confimar Etiqueta Blanca");
                        }
                    }

                    ex.Handled = true;
                }
                else
                {
                    ex.Handled = false;
                }
            };


            ////COMIENZA VALIDACION ETIQUETA VERDE [CLAVE PRODUCTO]
            //codigoetiqueta.InputType = Android.Text.InputTypes.Null;

            codigoetiqueta.KeyPress += (senderx, ex) =>
            {
                ex.Handled = false;

                string EtiquetaVerde = codigoetiqueta.Text;
                string V_Recibo = "", V_Prd = "", V_Existe = "", Mtipo = "", Fechacad = "", fecha_cad = "", diacad = "", mescad = "", prod_nombre = "", mtar = "", preautorizado = "";
                int L_Cad = 0, V_Tamaño = 0, wrkcen = 0, pos = 0;
                string id_pallet = "";

                PenXAuto = "N";
                ConsultaInserFolioAdelantado = "";

                if ((ex.Event.Action == KeyEventActions.Up) && (ex.KeyCode == Keycode.Enter))
                {
                    Segundos = 0;
                    Timer1.Enabled = false;

                    if (codigoetiqueta.Text.Trim().Length == 10 || codigoetiqueta.Text.Trim().Contains("FAC") == true)
                    {
                        validarCargaAdicional();
                        codigoetiqueta.Text = "";
                        confirmprod.Text = "";
                        Cajas.Text = "";
                        temperatura.Text = "";
                        Posicion.Text = "";
                        codigoetiqueta.RequestFocus();

                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                        return;
                    }

                    #region VALIDA LA CORRECTA ESTRUCTURA DE LA ETIQUETA VERDE

                    if ((codigoetiqueta.Text.Contains("HTTP://WWW.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=") == false &&
                         codigoetiqueta.Text.Contains("http://www.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") == false) &&
                        (codigoetiqueta.Text.Contains("HTTP://GAB.MRLUCKY.COM.MX/TR/TRAZABILIDAD2_DMI.PHP?ID_CODIGO=") == false &&
                         codigoetiqueta.Text.Contains("http://gab.mrlucky.com.mx/tr/trazabilidad2_dmi.php?id_codigo=") == false))
                    {
                        #region VALIDA QUE LA LECTURA NO SEA UN SPLIT

                        if (codigoetiqueta.Text.Contains("SPLIT*") != true)
                        {
                            CapturaSplitActiva = "0";

                            if (codigoetiqueta.Text.Trim().Length > 10)
                            {
                                var infoEtiqueta = ProcesarEtiqueta(codigoetiqueta.Text.Trim());

                                if (infoEtiqueta != null)
                                {
                                    Mtipo = infoEtiqueta.Value.Tipo;
                                    V_Recibo = infoEtiqueta.Value.Recibo;
                                    V_Prd = infoEtiqueta.Value.ProdClave;
                                    mtar = infoEtiqueta.Value.Tarima;

                                    #region VALIDA LECTURA DE PTI FAMOUS

                                    if ((V_Recibo == "" || mtar == "" || V_Prd == "" || Mtipo == "") && codigoetiqueta.Text.Trim().Length == 12)
                                    {
                                        string pti_famous = codigoetiqueta.Text.Trim();

                                        if (codigoetiqueta.Text.StartsWith("0"))
                                        {
                                            pti_famous = codigoetiqueta.Text.TrimStart('0');
                                        }

                                        if (thisConnection.State == ConnectionState.Closed)
                                        {
                                            thisConnection.Open();
                                        }

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

                                        if (thisConnection.State == ConnectionState.Open)
                                        {
                                            thisConnection.Close();
                                        }
                                    }

                                    #endregion

                                    #region VALIDA LECTURA DE SERIAL SHIPPING CONTAINER CODE

                                    else if ((V_Recibo == "" || mtar == "" || V_Prd == "" || Mtipo == "") && codigoetiqueta.Text.Contains(SerialShippingContainerCode) == true)
                                    {
                                        Match match = Regex.Match(codigoetiqueta.Text, patron);
                                        id_pallet = match.Groups[1].Value;

                                        if (thisConnection.State == ConnectionState.Closed)
                                        {
                                            thisConnection.Open();
                                        }

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

                                        if (thisConnection.State == ConnectionState.Open)
                                        {
                                            thisConnection.Close();
                                        }
                                    }

                                    #endregion

                                    #region VALIDA LECTURA DE PTI CLAVE

                                    else if ((V_Recibo == "" || mtar == "" || V_Prd == "" || Mtipo == "") && !Regex.IsMatch(codigoetiqueta.Text.Trim(), @"\s"))
                                    {
                                        #region VALIDAR ETIQUETA NUEVA

                                        var datos = ValidarEtiquetaVerde(codigoetiqueta.Text.Trim());

                                        if (datos != null)
                                        {
                                            Mtipo = datos.Value.Tipo;
                                            V_Recibo = datos.Value.Recibo;
                                            V_Prd = datos.Value.ProdClave;
                                            mtar = datos.Value.Tarima;
                                        }

                                        #endregion

                                        if (thisConnection.State == ConnectionState.Closed)
                                        {
                                            thisConnection.Open();
                                        }

                                        string querySSCC = "select*from tb_det_trazabilidad where pti_clave='" + codigoetiqueta.Text.Trim() + "'";
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

                                        if (thisConnection.State == ConnectionState.Open)
                                        {
                                            thisConnection.Close();
                                        }
                                    }

                                    #endregion

                                    #region VALIDA LECTURA DE ETIQUETA ANTERIOR

                                    else if ((V_Recibo == "" || mtar == "" || V_Prd == "" || Mtipo == "") && codigoetiqueta.Text.Trim().Contains(" ") == true)
                                    {
                                        if (codigoetiqueta.Text.Trim().Length < 18)
                                        {
                                            mtar = codigoetiqueta.Text.Substring(codigoetiqueta.Text.Trim().Length - 3, 3);
                                            V_Recibo = codigoetiqueta.Text.Substring(0, 5);
                                            V_Prd = codigoetiqueta.Text.Replace(V_Recibo, "");
                                            V_Prd = V_Prd.Replace(mtar, "");
                                            mtar = mtar.Replace(" ", "0");
                                            Mtipo = "PTC";
                                        }
                                        else
                                        {
                                            mtar = codigoetiqueta.Text.Substring(codigoetiqueta.Text.Trim().Length - 3, 3);
                                            V_Recibo = codigoetiqueta.Text.Substring(0, 6);
                                            V_Prd = codigoetiqueta.Text.Replace(V_Recibo, "");
                                            V_Prd = V_Prd.Replace(mtar, "");
                                            mtar = mtar.Replace(" ", "0");
                                            Mtipo = "PTP";

                                            if (V_Recibo.Substring(0, 1) == "0")
                                            {
                                                Mtipo = "PTC";
                                                V_Recibo = Convert.ToInt32(V_Recibo).ToString();
                                            }
                                        }
                                    }

                                    #endregion

                                    #region VALIDA LECTURA DE ETIQUETA POR DESCARTE

                                    else if (V_Recibo == "" || mtar == "" || V_Prd == "" || Mtipo == "")
                                    {
                                        V_Tamaño = codigoetiqueta.Text.Trim().Length;
                                        int posstring = codigoetiqueta.Text.Trim().IndexOf(" ", 0);

                                        if (posstring > -1)
                                        {
                                            DataTable CatalogodeProducto = new DataTable();

                                            if (thisConnection.State == ConnectionState.Closed)
                                            {
                                                thisConnection.Open();
                                            }

                                            string cade = "Select prod_clave,prod_nombre from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC') order by LEN(prod_clave) DESC";
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
                                                bool esta = codigoetiqueta.Text.Trim().Contains(producto_clave);

                                                if (esta)
                                                {
                                                    V_Prd = producto_clave;
                                                    break;
                                                }
                                            }

                                            string restocaptura = "";
                                            int posprod = codigoetiqueta.Text.Trim().IndexOf(V_Prd);
                                            V_Recibo = codigoetiqueta.Text.Trim().Substring(0, posprod).Trim();

                                            if (V_Recibo.Length > 0 && V_Prd.Length > 0)
                                            {
                                                restocaptura = codigoetiqueta.Text.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                                            }
                                            else
                                            {
                                                Toast.MakeText(this, "Por favor leer nuevamenmte la etiqueta.", ToastLength.Long).Show();
                                            }

                                            if (restocaptura.Length == 6)
                                            {
                                                Mtipo = "PTC";
                                                mtar = restocaptura.Substring(0, 3);
                                            }
                                            else
                                            {
                                                Mtipo = "PTC";
                                                mtar = restocaptura.Trim();
                                            }
                                        }
                                        else
                                        {
                                            L_Cad = V_Tamaño - 9;
                                            Mtipo = "PTP";
                                            mtar = codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3);
                                            V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6);

                                            if (V_Recibo.Substring(0, 1) == "0")
                                            {
                                                Mtipo = "PTC";
                                                V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                                            }

                                            V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad);
                                        }
                                    }

                                    #endregion
                                }

                                V_Recibo = V_Recibo.TrimStart('0');
                                mtar = mtar.TrimStart('0');

                                //Asignar a Valores Globales
                                FolioProducto = V_Recibo;
                                TarimaProducto = mtar;
                                ProductoProducto = V_Prd;

                                #region VALIDA EL ESTATUS DE LA ORDEN DE PRODUCCION 

                                string ordenProduccionEstatus = "";
                                string campobd = "";

                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }

                                if (Mtipo == "PTC")
                                {
                                    query = "Select rpt_estatus AS status From tb_mstr_recepcion_pt WHERE rpt_recibo = '" + V_Recibo + "'";
                                    campobd = "rpt_estatus";
                                }
                                else if (Mtipo == "PTP")
                                {
                                    query = "Select ordp_estatus AS status From tb_mstr_ordenes_prod WHERE ordp_folio = '" + V_Recibo + "'";
                                    campobd = "ordp_estatus";
                                }

                                SqlCommand cmd = new SqlCommand(query);
                                cmd.Connection = thisConnection;
                                SqlDataReader Info = cmd.ExecuteReader();

                                while (Info.Read())
                                {
                                    ordenProduccionEstatus = Info["status"].ToString().Trim();
                                }

                                if (thisConnection.State == ConnectionState.Open)
                                {
                                    thisConnection.Close();
                                }

                                if ((ordenProduccionEstatus == "F" && Mtipo == "PTC") || (ordenProduccionEstatus == "C" && Mtipo == "PTP"))
                                {
                                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>ORDEN DE PRODUCCION CANCELADA</font>"));
                                    alertDialog.SetIcon(Resource.Drawable.no);
                                    alertDialog.SetCancelable(false);
                                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden leida ha sido cancelada, favor de contactar con personal de calidad (fresco/ensaldas) si es de produccion o de materia prima si es de Campo</font>"));
                                    alertDialog.SetNeutralButton("Ok", delegate
                                    {
                                        alertDialog.Dispose();
                                        codigoetiqueta.Text = "";
                                        confirmprod.Text = "";
                                    });
                                    alertDialog.Show();
                                }

                                #endregion

                                #region VALIDA QUE LA ETIQUETA NO ESTE SURTIDA - PARTE 1

                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }

                                query = "Select emb_folio, prod_clave, recibo, cajas, seccion, temp, tarima From tb_det_embarque WHERE emb_folio = '" + pedido.Text.Trim() + "' and prod_clave = '" + V_Prd + "' and recibo = '" + V_Recibo + "' and tarima = '" + mtar + "' and OpCap = 'N' and Estatus != 'C'";
                                cmd = new SqlCommand(query);
                                cmd.Connection = thisConnection;
                                Info = cmd.ExecuteReader();

                                while (Info.Read())
                                {
                                    wrkcen = wrkcen + 1;
                                    temperatura.Text = Info["temp"].ToString().Trim();
                                    Posicion.Text = Info["seccion"].ToString().Trim();
                                    Cajas.Text = Info["cajas"].ToString().Trim();
                                }

                                if (thisConnection.State == ConnectionState.Open)
                                {
                                    thisConnection.Close();
                                }

                                #endregion

                                #region SE VALIDA EL NOMBRE DEL PRODUCTO PARA ESTABLECER LOS DIAS DE CADUCIDAD

                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }

                                query = "SELECT prod_nombre FROM tb_cat_producto WHERE prod_clave = '" + V_Prd + "'";
                                cmd = new SqlCommand(query);
                                cmd.Connection = thisConnection;
                                Info = cmd.ExecuteReader();

                                while (Info.Read())
                                {
                                    prod_nombre = Info["prod_nombre"].ToString().Trim();
                                }

                                if (thisConnection.State == ConnectionState.Open)
                                {
                                    thisConnection.Close();
                                }

                                int dias = 14;

                                if (prod_nombre.Contains("BETABEL"))
                                {
                                    dias = 60;
                                }
                                else if (prod_nombre.Contains("AJO"))
                                {
                                    dias = 180;
                                }

                                if (prod_nombre.Contains("ADEREZO") || prod_nombre.Contains("VINAGRETA") || prod_nombre.Contains("QUESO"))
                                {
                                    dias = 90;
                                }

                                #endregion

                                #region VALIDA QUE LA ETIQUETA EXISTA

                                V_Existe = "N";

                                if (Mtipo == "PTP")
                                {
                                    query = "Select a.folio,b.prod_nombre,a.num_cajas, a.cajas_sur, a.NUM_LOTE AS FECCAD, ISNULL(a.fechacad, FORMAT( DATEADD(day, " + dias + ", a.fecha), 'yyyyMMdd', 'en-US' )) AS fecha_cad, A.preautorizado From tb_det_eti_final A, tb_cat_producto B Where a.folio = '" + V_Recibo + "' and a.cve_prod = '" + V_Prd + "' and tarima = '" + mtar + "' and a.cve_prod = b.prod_clave order by b.prod_nombre";
                                }
                                else
                                {
                                    query = "Select RECIBO,prod_nombre,etiqueta,SURTIDO, FECHA_CAD AS FECCAD, (CASE fecha_cad WHEN '' THEN  FORMAT( DATEADD(day, " + dias + ", pti_fecha), 'dd/MM/yyyy', 'en-US' ) WHEN fecha_cad THEN fecha_cad END) AS fecha_cad, preautorizado From tb_det_trazabilidad Where RECIBO = '" + V_Recibo + "' and prod_clave = '" + V_Prd + "' and tarima = '" + Convert.ToInt32(mtar) + "' order by prod_nombre";
                                }

                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }

                                cmd = new SqlCommand(query);
                                cmd.Connection = thisConnection;
                                Info = cmd.ExecuteReader();

                                while (Info.Read())
                                {
                                    V_Existe = "S";
                                    Fechacad = Info["FECCAD"].ToString().Trim();
                                    fecha_cad = Info["fecha_cad"].ToString().Trim();
                                    preautorizado = Info["preautorizado"].ToString().Trim();
                                }

                                if (thisConnection.State == ConnectionState.Open)
                                {
                                    thisConnection.Close();
                                }

                                if (V_Existe == "N")
                                {
                                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>FOLIO INEXISTENTE</font>"));
                                    alertDialog.SetIcon(Resource.Drawable.no);
                                    alertDialog.SetCancelable(false);
                                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>No Existe Folio " + V_Recibo + " Producto " + V_Prd + " y Tarima" + mtar + " Verifique los Datos</font>"));
                                    alertDialog.SetNeutralButton("Ok", delegate
                                    {
                                        alertDialog.Dispose();
                                        codigoetiqueta.Text = "";
                                        confirmprod.Text = "";
                                    });
                                    alertDialog.Show();
                                }

                                #endregion

                                #region VALIDA QUE LA ETIQUETA NO ESTE SURTIDA - PARTE 2

                                ALTA = "A";

                                if (wrkcen != 0)
                                {
                                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>FOLIO YA REGISTRADO</font>"));
                                    alertDialog.SetIcon(Resource.Drawable.warning);
                                    alertDialog.SetCancelable(false);
                                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>EL CODIGO YA SE HA REGISTRADO, ¿DESEA MODIFICARLO?</font>"));

                                    alertDialog.SetPositiveButton("Modificar", (senderAlert, args) =>
                                    {
                                        ALTA = "M";
                                        alertDialog.Dispose();
                                    });

                                    alertDialog.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                    {
                                        codigoetiqueta.Text = "";
                                        confirmprod.Text = "";
                                        Cajas.Text = "";
                                        temperatura.Text = "";
                                        Posicion.Text = "";
                                        codigoetiqueta.RequestFocus();

                                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                        imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                                        return;
                                    });
                                }

                                #endregion

                                #region VALIDA VIDA ANAQUEL DE UN PRODUCTO POR CLIENTE

                                if (validaVidaAnaquel == "1")
                                {
                                    //Obtiene la cantidad de dias que faltan para su fecha de caducidad
                                    int diasFechaCaducidad = getDiasFechaCaducidad(Mtipo, V_Recibo, V_Prd, mtar, fecha_cad);
                                    //Obtiene la cantidad de dias minimos para su fecha de caducidad segun sea el cliente
                                    int diasMinimos = getDiasMinimos(V_Prd, cnte_clave, cve_subcli, ProductosVidaAnaquel);

                                    #region diasFechaCaducidad ES MENOR QUE diasMinimos

                                    if (diasFechaCaducidad < diasMinimos)
                                    {
                                        string ReciboAtrazado = LoteAtrazadoVAPP(V_Prd, Mtipo, mtar, fecha_cad, dias, diasMinimos);

                                        if (thisConnection.State == ConnectionState.Closed)
                                        {
                                            thisConnection.Open();
                                        }

                                        FolioAtrazadoVAPP(V_Prd, Mtipo, V_Recibo, dias, diasMinimos);
                                        string autorizarec = "";

                                        if (ReciboAtrazado.Trim().Length > 0)
                                        {
                                            if (V_Recibo != "0")
                                            {
                                                #region VALIDACION POR NIVEL SUPERIOR
                                                #region ENVIAR NOTIFICACION TEAMS

                                                // Instanciar la clase con el Webhook
                                                notiTeams = new TeamsNotifier("https://mrluckycommx.webhook.office.com/webhookb2/10baebcf-a990-473a-b619-4c0902d824bd@d20460dd-675d-4b51-87cc-9d10f9175633/IncomingWebhook/a3c759e44abc4a9e83eb693cf604b0e0/bbba0cb3-31d4-4d5a-ac47-264700d7b7d0/V2YNn6avVMmBQOeCJ19fSVRNzO81iARPHZ1pwzl5QmfeA1");

                                                string cardJsonWithButtons = @"{
        ""type"": ""AdaptiveCard"",
        ""version"": ""1.0"",
        ""body"": [
            {
                ""type"": ""TextBlock"",
                ""text"": ""¿Deseas realizar esta acción?""
            },
            {
                ""type"": ""ActionSet"",
                ""actions"": [
            {
                ""type"": ""Action.OpenUrl"",
                ""title"": ""Sí, quiero"",
                ""url"": ""https://www.ejemplo.com""
            },
            {
                ""type"": ""Action.OpenUrl"",
                ""title"": ""No, gracias"",
                ""url"": ""https://www.ejemplo.com/no""
            }
                ]
            }
        ]
    }";
                                                notiTeams.SendAdaptiveCard(cardJsonWithButtons);

                                                #endregion
                                                #endregion

                                                Android.App.AlertDialog.Builder alertcaducidad = new Android.App.AlertDialog.Builder(this);
                                                alertcaducidad.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>FOLIO ADELANTADO DISPONIBLE</font>"));
                                                alertcaducidad.SetIcon(Resource.Drawable.warning);
                                                alertcaducidad.SetCancelable(false);
                                                alertcaducidad.SetMessage(Html.FromHtml(
                                                    "<font color='#000000' size='10'>" +
                                                    "EXISTE UN RECIBO QUE CUMPLE CON LOS DÍAS MINIMOS REQUERIDOS POR CLIENTE PARA EL PRODUCTO " + prod_nombre.Trim() + "! " +
                                                    "DEBE DE TOMAR ESTE RECIBO: <br><br>" +
                                                    "<b>FOLIO:</b> " + FolioAtrasado + "<br>" +
                                                    "<b>TARIMA:</b> " + TarimaAtrasada + "<br>" +
                                                    "<b>FECHA CADUCIDAD:</b> " + FechaAtrasada + "<br><br>" +
                                                    "</font>"
                                                ));

                                                alertcaducidad.SetNeutralButton("OK", (senderAlert, args) =>
                                                {
                                                    codigoetiqueta.Text = "";
                                                    confirmprod.Text = "";
                                                    Cajas.Text = "";
                                                    temperatura.Text = "";
                                                    Posicion.Text = "";
                                                    codigoetiqueta.RequestFocus();

                                                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                                    imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);

                                                    if (thisConnection.State == ConnectionState.Open)
                                                    {
                                                        thisConnection.Close();
                                                    }
                                                    return;
                                                });

                                                alertcaducidad.Show();
                                            }
                                        }
                                    }

                                    #endregion

                                    #region diasFechaCaducidad ES MAYOR IGUAL QUE diasMinimos

                                    if (diasFechaCaducidad >= diasMinimos)
                                    {
                                        #region VALIDACION FIFO

                                        string ReciboAtrazado = LoteAtrazadoVAPP(V_Prd, Mtipo, mtar, fecha_cad, dias, diasMinimos);

                                        if (thisConnection.State == ConnectionState.Closed)
                                        {
                                            thisConnection.Open();
                                        }

                                        FolioAtrazadoVAPP(V_Prd, Mtipo, V_Recibo, dias, diasMinimos);

                                        if (ReciboAtrazado.Trim().Length > 0)
                                        {
                                            if (V_Recibo != "0")
                                            {
                                                Android.App.AlertDialog.Builder alertcaducidad = new Android.App.AlertDialog.Builder(this);
                                                alertcaducidad.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>FOLIO ATRASADO DISPONIBLE</font>"));
                                                alertcaducidad.SetIcon(Resource.Drawable.warning);
                                                alertcaducidad.SetCancelable(false);
                                                alertcaducidad.SetMessage(Html.FromHtml("<font color='#000000' size = 10>¿HAY UN RECIBO MAS ATRASADO PARA EL PRODUCTO " + prod_nombre.Trim() + " DESEA TOMAR ESTE RECIBO ? FOLIO : " + FolioAtrasado + " DE LA TARIMA " + TarimaAtrasada + ", CON FECHA DE CADUCIDAD " + FechaAtrasada + ", DE LO CONTRARIO SE DEBE AUTORIZAR EL RECIBO ACTUAL</font>"));

                                                alertcaducidad.SetPositiveButton("Autorizar", (senderAlert, args) =>
                                                {
                                                    FolioLeido = V_Recibo;
                                                    TarimaLeido = mtar;
                                                    FechaLeido = fecha_cad;
                                                    Producto = prod_nombre;
                                                    Productocve = V_Prd;
                                                    alertcaducidad.Dispose();

                                                    IniciarFlujoCargaATU(
                                                        folioLeido: FolioLeido,
                                                        fechaLeido: FechaLeido,
                                                        folioAtrasado: FolioAtrasado,
                                                        fechaAtrasada: FechaAtrasada,
                                                        productocve: Productocve,
                                                        producto: Producto,
                                                        cajasDisp: CajasDisp,
                                                        tarimaLeido: TarimaLeido,
                                                        tarimaAtrasada: TarimaAtrasada);

                                                    //MostrarDialogoATU(
                                                    //    motivo: motivoautorizafechaadelantada.Trim(),
                                                    //    folioLeido: FolioLeido,
                                                    //    fechaLeido: FechaLeido,
                                                    //    folioAtrasado: FolioAtrasado,
                                                    //    fechaAtrasada: FechaAtrasada,
                                                    //    productocve: Productocve,
                                                    //    producto: Producto,
                                                    //    cajasDisp: CajasDisp,
                                                    //    tarimaLeido: TarimaLeido,
                                                    //    tarimaAtrasada: TarimaAtrasada);
                                                });

                                                alertcaducidad.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                                {
                                                    codigoetiqueta.Text = "";
                                                    confirmprod.Text = "";
                                                    Cajas.Text = "";
                                                    temperatura.Text = "";
                                                    Posicion.Text = "";
                                                    codigoetiqueta.RequestFocus();

                                                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                                    imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);

                                                    if (thisConnection.State == ConnectionState.Open)
                                                    {
                                                        thisConnection.Close();
                                                    }
                                                    return;
                                                });

                                                alertcaducidad.Show();
                                            }
                                        }

                                        #endregion
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region VALIDACION DE FOLIOS ATRAZADOS

                                    string RecAtra = LoteAtrazado(V_Prd, Mtipo, mtar, fecha_cad, dias);

                                    if (thisConnection.State == ConnectionState.Closed)
                                    {
                                        thisConnection.Open();
                                    }

                                    //METODO UTILIZADO PARA MOSTRAR EN PANTALLA LOS FOLIOS ATRAZADOS DISPONIBLES A SURTIR
                                    FolioAtrazado(V_Prd, Mtipo, V_Recibo);

                                    if (RecAtra.Trim().Length > 0)
                                    {
                                        if (V_Recibo != "0")
                                        {
                                            Android.App.AlertDialog.Builder alertcaducidad = new Android.App.AlertDialog.Builder(this);
                                            alertcaducidad.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>FOLIO ATRASADO DISPONIBLE</font>"));
                                            alertcaducidad.SetIcon(Resource.Drawable.warning);
                                            alertcaducidad.SetCancelable(false);
                                            alertcaducidad.SetMessage(Html.FromHtml("<font color='#000000' size = 10>¿HAY UN RECIBO MAS ATRASADO PARA EL PRODUCTO " + prod_nombre.Trim() + " DESEA TOMAR ESTE RECIBO ? FOLIO : " + FolioAtrasado + " DE LA TARIMA " + TarimaAtrasada + ", CON FECHA DE CADUCIDAD " + FechaAtrasada + ", DE LO CONTRARIO SE DEBE AUTORIZAR EL RECIBO ACTUAL</font>"));

                                            alertcaducidad.SetPositiveButton("Autorizar", (senderAlert, args) =>
                                            {
                                                FolioLeido = V_Recibo;
                                                TarimaLeido = mtar;
                                                FechaLeido = fecha_cad;
                                                Producto = prod_nombre;
                                                Productocve = V_Prd;
                                                alertcaducidad.Dispose();

                                                IniciarFlujoCargaATU(
                                                        folioLeido: FolioLeido,
                                                        fechaLeido: FechaLeido,
                                                        folioAtrasado: FolioAtrasado,
                                                        fechaAtrasada: FechaAtrasada,
                                                        productocve: Productocve,
                                                        producto: Producto,
                                                        cajasDisp: CajasDisp,
                                                        tarimaLeido: TarimaLeido,
                                                        tarimaAtrasada: TarimaAtrasada);

                                                //MostrarDialogoATU(
                                                //    motivo: motivoautorizafechaadelantada.Trim(),
                                                //    folioLeido: FolioLeido,
                                                //    fechaLeido: FechaLeido,
                                                //    folioAtrasado: FolioAtrasado,
                                                //    fechaAtrasada: FechaAtrasada,
                                                //    productocve: Productocve,
                                                //    producto: Producto,
                                                //    cajasDisp: CajasDisp,
                                                //    tarimaLeido: TarimaLeido,
                                                //    tarimaAtrasada: TarimaAtrasada);
                                            });

                                            alertcaducidad.SetNegativeButton("Cancelar", (senderAlert, args) =>
                                            {
                                                codigoetiqueta.Text = "";
                                                confirmprod.Text = "";
                                                Cajas.Text = "";
                                                temperatura.Text = "";
                                                Posicion.Text = "";
                                                codigoetiqueta.RequestFocus();

                                                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                                imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);

                                                if (thisConnection.State == ConnectionState.Open)
                                                {
                                                    thisConnection.Close();
                                                }
                                                return;
                                            });

                                            alertcaducidad.Show();
                                        }
                                    }

                                    #endregion
                                }

                                #endregion

                                #region VALIDACION PARA LAS CAJAS DISPONIBLES POR TARIMA

                                CantidadDisponibleTarima();

                                if (v_dif == 0)
                                {
                                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>FOLIO SIN EXISTENCIA</font>"));
                                    alertDialog.SetIcon(Resource.Drawable.no);
                                    alertDialog.SetCancelable(false);
                                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Folio " + V_Recibo + " Producto " + V_Prd + " y Tarima " + mtar + ". No tiene cajas disponibles, favor de verificarlo</font>"));
                                    alertDialog.SetNeutralButton("Ok", delegate
                                    {
                                        if (thisConnection.State == ConnectionState.Open)
                                        {
                                            thisConnection.Close();
                                        }
                                    });
                                    alertDialog.Show();

                                    codigoetiqueta.RequestFocus();
                                }

                                #endregion

                                #region VALIDACION PARA LOS PRODUCTOS VALIDADOS SOLO CON ETIQUETA VERDE

                                DataRow[] result = det_producto_sin_blanca.Select("prod_clave = '" + V_Prd.Trim() + "'");

                                if (result.Count() == 0)
                                {
                                    codigoetiqueta.Enabled = false;
                                    confirmprod.Enabled = true;
                                    confirmprod.RequestFocus();

                                    #region FRAGMENTO PESO X EJES
                                    updatePesoPorEjes(Notrailer.Text, fecha.Text, FolioProducto, ProductoProducto, TarimaProducto, "", "");
                                    #endregion
                                }
                                else
                                {
                                    codigoetiqueta.Enabled = false;
                                    confirmprod.Enabled = false;
                                    temperatura.Enabled = true;
                                    temperatura.RequestFocus();

                                    #region FRAGMENTO PESO X EJES
                                    updatePesoPorEjes(Notrailer.Text, fecha.Text, FolioProducto, ProductoProducto, TarimaProducto, "", "");
                                    #endregion
                                }

                                #endregion
                            }
                        }

                        #endregion

                        #region VALIDACION PARA SPLITS EN EL CAMPO CLAVE DEL PRODUCTO

                        else
                        {
                            CapturaSplitActiva = "1";
                            codigoetiqueta.Text = codigoetiqueta.Text.Replace("SPLIT*", "");
                            string Pedidostring = codigoetiqueta.Text.Substring(0, 6);

                            if (Pedidostring == pedido.Text)
                            {
                                codigoetiqueta.Text = codigoetiqueta.Text.Replace(Pedidostring + "*", "");
                                updatePesoPorEjes(Notrailer.Text, fecha.Text, "", "", "", codigoetiqueta.Text.Split(',')[0].Trim(), pedido.Text);
                                codigoetiqueta.Enabled = false;
                                confirmprod.Enabled = false;
                                temperatura.Enabled = true;
                                temperatura.RequestFocus();

                                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                imm.ShowSoftInput(temperatura, ShowFlags.Implicit);
                            }
                            else
                            {
                                Toast.MakeText(this, "El Split no corresponde a la orden", ToastLength.Long).Show();
                                codigoetiqueta.Text = "";
                                codigoetiqueta.RequestFocus();
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>🔴 Etiqueta no reconocida</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La etiqueta escaneada no pudo ser validada. Por favor, revisa que el código sea legible y vuelve a intentarlo. ¡Tu atención nos ayuda a un mejor registro!</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                            codigoetiqueta.Text = "";
                        });
                        alertDialog.Show();
                    }

                    #endregion

                    #region CARGA EXCESIVA

                    if (CapturaSplitActiva == "0")
                    {
                        int pedidoactualvalidar = Convert.ToInt32(pedido.Text.Trim());
                        string tipoembval = "NAL";

                        if (pedidoactualvalidar < 500000)
                        {
                            tipoembval = "EXP";
                        }

                        if (ValidarProd(V_Prd, tipoembval) == "CARGANDOMAS")
                        {
                            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                            alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>CARGA EXCESIVA</font>"));
                            alertDialog.SetIcon(Resource.Drawable.warning);
                            alertDialog.SetCancelable(false);
                            alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>LA SUMA DE LA CARGA ACTUAL ES MAYOR A LO PEDIDO POR VENTAS, ¿DESEA CONTINUAR?</font>"));

                            alertDialog.SetPositiveButton("FORZAR CARGADO", (senderAlert, args) =>
                            {
                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }

                                string Cadenaproductonoesta = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                        "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','A','7.10','" +
                                        pedido.Text.Trim() + "','Cargado mas pedido menos" + V_Prd + "','CARGAEMB','" + pedido.Text.Trim() + "')";

                                SqlCommand cmdx = new SqlCommand(Cadenaproductonoesta, thisConnection);
                                cmdx.ExecuteNonQuery();

                                string not = setBodyEmail(pedido.Text.Trim(), V_Prd);

                                if (INFO_FILE == "http://192.168.123.4:81/EmbarquesApk/estado_respaldo.txt")
                                {
                                    notificarCargaExcesivaLocal.SendMail("ricardo.cortes@mrlucky.com.mx;ahernandez@mrlucky.com.mx;logistica@mrlucky.com.mx;jgalvan@mrlucky.com.mx", not, "CARGA EXCESIVA EN ORDEN DE EMBARQUES");
                                }
                                else
                                {
                                    notificarCargaExcesiva.SendMail("ricardo.cortes@mrlucky.com.mx;ahernandez@mrlucky.com.mx;logistica@mrlucky.com.mx;jgalvan@mrlucky.com.mx", not, "CARGA EXCESIVA EN ORDEN DE EMBARQUES");
                                }

                                if (thisConnection.State == ConnectionState.Open)
                                {
                                    thisConnection.Close();
                                }

                                alertDialog.Dispose();
                            });

                            alertDialog.SetNegativeButton("CANCELAR", (senderAlert, args) =>
                            {
                                codigoetiqueta.Text = "";
                                confirmprod.Text = "";
                                Cajas.Text = "";
                                temperatura.Text = "";
                                Posicion.Text = "";
                                codigoetiqueta.RequestFocus();

                                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);

                                if (thisConnection.State == ConnectionState.Open)
                                {
                                    thisConnection.Close();
                                }
                                return;
                            });

                            alertDialog.Show();
                        }

                        int i = 0;

                        if (int.TryParse(Cajas.Text.Trim(), out i) == false)
                        {
                            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                            alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>FOLIO SIN EXISTENCIA</font>"));
                            alertDialog.SetIcon(Resource.Drawable.no);
                            alertDialog.SetCancelable(false);
                            alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Folio " + V_Recibo + " Producto " + V_Prd + " y Tarima " + mtar + ". No tiene cajas disponibles, favor de verificarlo</font>"));
                            alertDialog.SetNeutralButton("Ok", delegate
                            {
                                if (thisConnection.State == ConnectionState.Open)
                                {
                                    thisConnection.Close();
                                }
                                return;
                            });
                            alertDialog.Show();
                        }
                    }

                    #endregion

                    ex.Handled = true;
                }
                else if ((ex.Event.Action == KeyEventActions.Down && ex.KeyCode == Keycode.Del) || (ex.Event.Action == KeyEventActions.Down && ex.KeyCode == Keycode.ForwardDel))
                {
                    ex.Handled = true;
                }
                else
                {
                    ex.Handled = false;
                }
            };


            pedido.KeyPress += Pedido_KeyPress;

            Ordenes.ItemSelected += (sender, e) =>
            {
                Spinner spinner = (Spinner)sender;
                string orden = spinner.GetItemAtPosition(e.Position).ToString();
                if (orden != "PEDIDO" || orden is null)
                {
                    pedido.Text = orden;

                    Pedido_KeyPress(pedido, new View.KeyEventArgs(false, Keycode.Enter, new KeyEvent(KeyEventActions.Up, Keycode.Enter)));
                }
            };

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Capturar Pedido";
            updatePesoPorEjes(Notrailer.Text, fecha.Text, "", "", "", "", pedido.Text);

        }

        #region ACTUALIZA EL PESO POR EJES
        private void updatePesoPorEjes(string no_trailer, string hora_trailer, string recibo,
                              string prod_clave, string tarima, string split, string pdn_folio)
        {
            RunOnUiThread(() =>
            {
                // Intentar obtener el fragmento existente
                var fragment = FragmentManager.FindFragmentById(Resource.Id.contenedorFragment) as PesoXEjesFragment2;

                if (fragment != null && fragment.IsAdded)
                {
                    // Fragmento existe - actualizar datos según lo que necesites
                    if (!string.IsNullOrEmpty(split))
                    {
                        // Caso SPLIT
                        fragment.ActualizarDatosSplit(split, pdn_folio);
                    }
                    else if (!string.IsNullOrEmpty(tarima))
                    {
                        // Caso TARIMA
                        fragment.ActualizarDatosTarima(recibo, prod_clave, tarima, pdn_folio);
                    }
                    else
                    {
                        // Caso TRAILER
                        fragment.ActualizarDatosTrailer(no_trailer, hora_trailer, pdn_folio);
                    }
                }
                else
                {
                    // Fragmento no existe - crear uno nuevo
                    fragment = new PesoXEjesFragment2();
                    Bundle args = new Bundle();
                    args.PutString("no_trailer", no_trailer);
                    args.PutString("hora_trailer", hora_trailer);
                    args.PutString("recibo", recibo);
                    args.PutString("prod_clave", prod_clave);
                    args.PutString("tarima", tarima);
                    args.PutString("codigoetiqueta", split);
                    args.PutString("pdn_folio", pdn_folio);
                    fragment.Arguments = args;

                    FragmentManager.BeginTransaction()
                        .Replace(Resource.Id.contenedorFragment, fragment)
                        .Commit();
                }
            });
        }

        private void updatePesoTarima()
        {
            #region FRAGMENTO PESO X EJES
            var fragment = new PesoXEjesFragment2();
            Bundle args = new Bundle();
            args.PutString("recibo", FolioProducto); // aquí pones el valor real
            args.PutString("prod_clave", ProductoProducto); // valor real
            args.PutString("tarima", TarimaProducto); // valor real
            fragment.Arguments = args;

            FragmentManager.BeginTransaction()
                .Replace(Resource.Id.contenedorFragment, fragment)
                .Commit();
            #endregion
        }
        #endregion

        #region METODOS UTILIZADOS PARA DESHABILITAR COPIAR Y PEGAR
        private class NoPasteCallback : Java.Lang.Object, Android.Views.ActionMode.ICallback
        {
            public bool OnActionItemClicked(ActionMode mode, IMenuItem item) => false;
            public bool OnCreateActionMode(ActionMode mode, IMenu menu) => false;
            public void OnDestroyActionMode(ActionMode mode) { }
            public bool OnPrepareActionMode(ActionMode mode, IMenu menu) => false;
        }
        #endregion

        #region VALIDAR VIDA DE ANAQUEL DEL PRODUCTO
        //getVidaAnaquel retorna 1 si el pedido entra a la validacion de Vida Anaquel del Producto por Cliente
        //en caso contrario retorna 0 y se salta esta validacion; También se almacena la lista de productos correspondientes.
        private string getVidaAnaquel(string mped)
        {
            string resultado = "0";
            string valor = "";
            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            string Cadena = "SELECT C.pdn_folio, C.prod_clave, C.pdn_num_unidades, A.dias_minimos, A.cnte_clave, A.cve_subcli FROM tb_det_pedidos AS C INNER JOIN tb_mstr_pedidos_nal AS B ON B.pdn_folio = C.pdn_folio INNER JOIN tb_vida_anaquel AS A ON A.cnte_clave = B.cnte_clave AND A.cve_subcli = B.cve_subcli AND A.prod_clave = C.prod_clave WHERE B.pdn_folio = '" + mped + "'";
            //string Cadena = "SELECT COUNT(*) FROM tb_det_pedidos AS C INNER JOIN tb_mstr_pedidos_nal AS B ON B.pdn_folio = C.pdn_folio INNER JOIN tb_vida_anaquel AS A ON A.cnte_clave = B.cnte_clave AND A.cve_subcli = B.cve_subcli AND A.prod_clave = C.prod_clave WHERE B.pdn_folio = '" + mped + "'";
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(Cadena, thisConnection);
            da.Fill(ds, "Info2");
            ProductosVidaAnaquel = ds.Tables["Info2"];
            thisConnection.Close();

            if (ProductosVidaAnaquel.Rows.Count > 0)
            {
                resultado = "1";
            }

            return resultado;
        }
        //getDiasMinimos retorna el número de días minimos de carga (días hasta que se cumpla su fecha de caducidad) por producto, cliente, subcliente
        private int getDiasMinimos(string prod_clave, string cnte_clave, string cve_subcli, DataTable productosVidaAnaquel)
        {
            int diasMinimos = 0;
            string strDiasMinimos = "";
            DataRow[] datos = productosVidaAnaquel.Select("prod_clave = '" + prod_clave + "' AND cnte_clave = '" + cnte_clave + "' AND cve_subcli = '" + cve_subcli + "'");

            if (datos.Length > 0)
            {
                strDiasMinimos = datos[0].ItemArray[3].ToString();
                diasMinimos = Convert.ToInt32(strDiasMinimos);
            }

            return diasMinimos;
        }
        //getDiasFechaCaducidad retorna el numero de días hasta que se cumpla su fecha de caducidad por tarima leida
        private int getDiasFechaCaducidadES(string tipo, string folio, string cve_prod, string tarima, string fechacad)
        {
            int dias = 0;
            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            string Cadena = "SELECT * FROM tb_det_eti_final WHERE folio = " + folio + " AND cve_prod = '" + cve_prod + "' AND tarima = " + tarima;
            DataSet ds = new DataSet();
            DataTable Info2 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(Cadena, thisConnection);
            da.Fill(ds, "Info2");
            Info2 = ds.Tables["Info2"];

            TimeSpan Mdias = TimeSpan.Zero;
            foreach (DataRow row in Info2.Rows)
            {

                int Mdia = 0;
                DateTime FecCad = Convert.ToDateTime(row["FECHA"].ToString().Trim());
                //DateTime FecCad = DateTime.ParseExact(row["FECHA"].ToString().Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                string Mfeca = "";
                if (row["NUM_LOTE"].ToString().Trim().Length > 0)
                {
                    int Mtam = row["NUM_LOTE"].ToString().Trim().Length;
                    if (row["fechacad"].ToString().Trim().Length > 0)
                        Mfeca = row["fechacad"].ToString().Substring(6, 2) + "/" + row["fechacad"].ToString().Substring(4, 2) + "/" + row["fechacad"].ToString().Substring(0, 4);
                    else
                        Mfeca = ConviertetoFecha(row["NUM_LOTE"].ToString().Substring((Mtam == 12) ? 7 : 6, 5));

                    string Mfol = row["FOLIO"].ToString();
                    Mdias = Convert.ToDateTime(Mfeca) - System.DateTime.Now.AddDays(-1);
                    //Mdias = DateTime.Now.AddDays(-1) + FecCad;
                }
                else
                {
                    if (traenom(cve_prod).Contains("BETABEL"))
                        FecCad = FecCad.AddDays(60);
                    else
                        if (traenom(cve_prod).Contains("AJO"))
                        FecCad = FecCad.AddDays(180);
                    else
                            if (traenom(cve_prod).Contains("ADEREZO") || traenom(cve_prod).Contains("VINAGRETA") || traenom(cve_prod).Contains("QUESO"))
                        FecCad = FecCad.AddDays(90);
                    else
                        FecCad = FecCad.AddDays(14);
                    Mdias = FecCad - System.DateTime.Now.AddDays(-1);
                    Mfeca = FecCad.ToShortDateString();
                }
            }

            dias = Convert.ToInt32(Mdias);

            return dias;
        }

        private int getDiasFechaCaducidad(string tipo, string folio, string cve_prod, string tarima, string fechacad)
        {
            int dias = 14;
            string Cadena = "";
            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }

            if (tipo == "PTP")
            {
                Cadena = "SELECT * FROM tb_det_eti_final WHERE folio = " + folio + " AND cve_prod = '" + cve_prod + "' AND tarima = " + tarima;
            }
            else
            {
                Cadena = "SELECT pti_fecha AS FECHA, lote AS NUM_LOTE, fecha_cad AS fechacad FROM tb_det_trazabilidad WHERE recibo = " + folio + " AND prod_clave = '" + cve_prod + "' AND tarima = " + tarima;
            }

            DataSet ds = new DataSet();
            DataTable Info2 = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(Cadena, thisConnection);
            da.Fill(ds, "Info2");
            Info2 = ds.Tables["Info2"];

            TimeSpan Mdias = TimeSpan.Zero;

            foreach (DataRow row in Info2.Rows)
            {
                DateTime FecCad;
                string Mfeca = "";
                bool fechaValida = false;

                if (!DateTime.TryParse(row["FECHA"].ToString().Trim(), out FecCad))
                {
                    throw new System.FormatException($"Error: La fecha '{row["FECHA"].ToString().Trim()}' no tiene el formato correcto.");
                }

                if (row["NUM_LOTE"].ToString().Trim().Length > 0)
                {
                    int Mtam = row["NUM_LOTE"].ToString().Trim().Length;

                    if (row["fechacad"].ToString().Trim().Length > 0)
                    {
                        Mfeca = row["fechacad"].ToString().Trim();
                        if (Mfeca.Length == 8) // Aseguramos que sea un formato YYYYMMDD
                        {
                            Mfeca = Mfeca.Substring(6, 2) + "/" + Mfeca.Substring(4, 2) + "/" + Mfeca.Substring(0, 4);
                        }
                    }
                    else
                    {
                        //Mfeca = ConviertetoFecha(row["NUM_LOTE"].ToString().Substring((Mtam == 12) ? 7 : 6, 5));
                        string numLote = row["NUM_LOTE"].ToString();
                        int inicio = (Mtam == 12) ? 7 : 6;
                        int longitud = 5;

                        if (numLote.Length >= inicio + longitud)
                        {
                            Mfeca = ConviertetoFecha(numLote.Substring(inicio, longitud));
                        }
                    }

                    DateTime fechaConvertida;
                    string formatoFecha = "dd/MM/yyyy"; // Formato esperado

                    fechaValida = DateTime.TryParseExact(Mfeca, formatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaConvertida);

                    if (!fechaValida)
                    {
                        //throw new System.FormatException($"Error: La fecha '{Mfeca}' no tiene el formato correcto.");
                        Android.App.AlertDialog.Builder alertcaducidad = new Android.App.AlertDialog.Builder(this);
                        alertcaducidad.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>PRODUCTO SIN FECHA DE CADUCIDAD</font>"));
                        alertcaducidad.SetIcon(Resource.Drawable.warning);
                        alertcaducidad.SetCancelable(false);
                        alertcaducidad.SetMessage(Html.FromHtml(
                            "<font color='#000000' size='10'>" +
                            "EXISTE UN PROBLEMA AL OBTENER LA FECHA DE CADUCIDAD DEL PRODUCTO " + prod_nombre.Trim() + "! " +
                            "DEBE DE TOMAR ESTE RECIBO: <br><br>" +
                            "<b>FOLIO:</b> " + FolioAtrasado + "<br>" +
                            "<b>TARIMA:</b> " + TarimaAtrasada + "<br>" +
                            "<b>FECHA CADUCIDAD:</b> " + FechaAtrasada + "<br><br>" +
                            "</font>"
                        ));
                        alertcaducidad.SetNeutralButton("OK", (senderAlert, args) =>
                        {
                            codigoetiqueta.Text = "";
                            confirmprod.Text = "";
                            Cajas.Text = "";
                            temperatura.Text = "";
                            Posicion.Text = "";
                            codigoetiqueta.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            return;
                        });
                        alertcaducidad.Show();
                    }

                    Mdias = fechaConvertida - System.DateTime.Now.AddDays(-1);
                }
                else
                {
                    if (traenom(cve_prod).Contains("BETABEL"))
                        FecCad = FecCad.AddDays(60);
                    else if (traenom(cve_prod).Contains("AJO"))
                        FecCad = FecCad.AddDays(180);
                    else if (traenom(cve_prod).Contains("ADEREZO") || traenom(cve_prod).Contains("VINAGRETA") || traenom(cve_prod).Contains("QUESO"))
                        FecCad = FecCad.AddDays(90);
                    else
                        FecCad = FecCad.AddDays(14);

                    Mdias = FecCad - System.DateTime.Now.AddDays(-1);
                    Mfeca = FecCad.ToShortDateString();
                }
            }

            dias = Convert.ToInt32(Mdias.TotalDays);

            return dias;
        }

        private string ConviertetoFecha(string FEC)
        {
            string mdia = FEC.Substring(3, 2);
            string mmes = FEC.Substring(0, 3);
            string nmes = "";
            if (mmes == "ENE")
                nmes = "01";
            if (mmes == "FEB")
                nmes = "02";
            if (mmes == "MAR")
                nmes = "03";
            if (mmes == "ABR")
                nmes = "04";
            if (mmes == "MAY")
                nmes = "05";
            if (mmes == "JUN")
                nmes = "06";
            if (mmes == "JUL")
                nmes = "07";
            if (mmes == "AGO")
                nmes = "08";
            if (mmes == "SEP")
                nmes = "09";
            if (mmes == "OCT")
                nmes = "10";
            if (mmes == "NOV")
                nmes = "11";
            if (mmes == "DIC")
                nmes = "12";
            int MES = System.DateTime.Now.Month;
            int anio = System.DateTime.Now.Year + (MES == 12 && nmes == "01" ? 1 : 0);
            //if (Convert.ToInt32(nmes) < MES)
            //    anio++;
            string cad = mdia + "/" + nmes + "/" + anio.ToString();
            return cad;
        }

        #region NUEVA VERSION DE DIAS HASTA FECHA DE CADUCIDAD
        const int DiasFallback = 14;
        private int GetDiasHastaCaducidad(string tipo, string folio, string cveProd, string tarima, string fechaCaducidadProporcionada)
        {

            int dias = DiasFallback;
            // Asumimos que thisConnection es un SqlConnection definido en la clase
            // y que se maneja adecuadamente (e.g., inyectado o gestionado por un using)
            using (SqlConnection conn = new SqlConnection(cadenaConexion)) // Usa 'using' para auto-cerrar
            {
                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string query = BuildQuery(tipo, folio, cveProd, tarima);
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddParameters(cmd, tipo, folio, cveProd, tarima);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "Info");
                            DataTable infoTable = ds.Tables["Info"];

                            if (infoTable.Rows.Count == 0)
                            {
                                // No hay datos: retorna fallback
                                return dias;
                            }

                            // Asumimos una sola fila; si múltiples, usa la primera (o maneja lógicamente)
                            DataRow row = infoTable.Rows[0];

                            DateTime fechaInicial;
                            if (!DateTime.TryParse(row["FECHA"].ToString().Trim(), out fechaInicial))
                            {
                                throw new System.FormatException($"Error: La fecha '{row["FECHA"].ToString().Trim()}' no tiene el formato correcto.");
                            }

                            string fechaCadStr = ObtenerFechaCaducidadDeFila(row, cveProd);

                            if (string.IsNullOrEmpty(fechaCadStr))
                            {
                                MostrarAlertaCaducidad(cveProd, folio, tarima, fechaCaducidadProporcionada);
                                return dias; // O lanza excepción si prefieres detener
                            }

                            DateTime fechaCaducidad;
                            if (!DateTime.TryParseExact(fechaCadStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaCaducidad))
                            {
                                MostrarAlertaCaducidad(cveProd, folio, tarima, fechaCaducidadProporcionada);
                                return dias;
                            }

                            TimeSpan diferencia = fechaCaducidad - DateTime.Now.AddDays(-1);
                            dias = (int)diferencia.TotalDays;
                        }
                    }
                }
                catch (Java.Lang.Exception ex)
                {
                    // Manejo de errores: loggea o muestra alerta
                    MostrarAlertaError(ex.Message);
                    return dias;
                }
            }

            return dias;
        }
        private string BuildQuery(string tipo, string folio, string cveProd, string tarima)
        {
            if (tipo == "PTP")
            {
                // Ajusta columnas para consistencia; asume que tb_det_eti_final tiene FECHA, NUM_LOTE, fechacad
                return "SELECT FECHA, NUM_LOTE, fechacad FROM tb_det_eti_final WHERE folio = @folio AND cve_prod = @cveProd AND tarima = @tarima";
            }
            else
            {
                return "SELECT pti_fecha AS FECHA, lote AS NUM_LOTE, fecha_cad AS fechacad FROM tb_det_trazabilidad WHERE recibo = @folio AND prod_clave = @cveProd AND tarima = @tarima";
            }
        }
        private void AddParameters(SqlCommand cmd, string tipo, string folio, string cveProd, string tarima)
        {
            cmd.Parameters.AddWithValue("@folio", folio);
            cmd.Parameters.AddWithValue("@cveProd", cveProd);
            cmd.Parameters.AddWithValue("@tarima", tarima);
        }
        private string ObtenerFechaCaducidadDeFila(DataRow row, string cveProd)
        {
            string numLote = row["NUM_LOTE"]?.ToString().Trim() ?? string.Empty;
            string fechaCadDb = row["fechacad"]?.ToString().Trim() ?? string.Empty;

            if (!string.IsNullOrEmpty(numLote))
            {
                if (!string.IsNullOrEmpty(fechaCadDb))
                {
                    if (fechaCadDb.Length == 8) // YYYYMMDD
                    {
                        return $"{fechaCadDb.Substring(6, 2)}/{fechaCadDb.Substring(4, 2)}/{fechaCadDb.Substring(0, 4)}";
                    }
                    return fechaCadDb; // Asume ya en formato
                }
                else
                {
                    int tam = numLote.Length;
                    int inicio = (tam == 12) ? 7 : 6;
                    int longitud = 5;
                    if (tam >= inicio + longitud)
                    {
                        string codigoFecha = numLote.Substring(inicio, longitud);
                        return ConviertetoFecha(codigoFecha); // Asume esta función existe y retorna dd/MM/yyyy
                    }
                }
            }
            else
            {
                // Lógica fallback basada en producto
                string nombreProd = traenom(cveProd); // Asume existe
                DateTime fechaInicial = (DateTime)row["FECHA"]; // Ya parseada antes

                int diasAgregar = DiasFallback;
                if (nombreProd.Contains("BETABEL"))
                    diasAgregar = 60;
                else if (nombreProd.Contains("AJO"))
                    diasAgregar = 180;
                else if (nombreProd.Contains("ADEREZO") || nombreProd.Contains("VINAGRETA") || nombreProd.Contains("QUESO"))
                    diasAgregar = 90;

                DateTime fechaCalc = fechaInicial.AddDays(diasAgregar);
                return fechaCalc.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return null; // Si no se obtiene
        }
        private void MostrarAlertaCaducidad(string cveProd, string folio, string tarima, string fechaAtrasada)
        {
            // Asume prod_nombre, etc., son propiedades o campos de la clase
            string prodNombre = traenom(cveProd).Trim(); // O usa una propiedad

            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>PRODUCTO SIN FECHA DE CADUCIDAD</font>"));
            alert.SetIcon(Resource.Drawable.warning);
            alert.SetCancelable(false);
            alert.SetMessage(Html.FromHtml(
                "<font color='#000000' size='10'>" +
                "EXISTE UN PROBLEMA AL OBTENER LA FECHA DE CADUCIDAD DEL PRODUCTO " + prodNombre + "! " +
                "DEBE DE INFORMAR DE ESTE RECIBO: <br><br>" +
                "<b>FOLIO:</b> " + folio + "<br>" +
                "<b>TARIMA:</b> " + tarima + "<br>" +
                "<b>FECHA CADUCIDAD:</b> " + fechaAtrasada + "<br><br>" +
                "</font>"
            ));
            alert.SetNeutralButton("OK", (sender, args) =>
            {
                // Reset UI: asume estos son controles de la clase
                codigoetiqueta.Text = "";
                confirmprod.Text = "";
                Cajas.Text = "";
                temperatura.Text = "";
                Posicion.Text = "";
                codigoetiqueta.RequestFocus();
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
            });
            alert.Show();
        }
        private void MostrarAlertaError(string mensaje)
        {
            // Implementa una alerta genérica para errores
            AlertDialog.Builder alert = new AlertDialog.Builder(this);
            alert.SetTitle("Error");
            alert.SetMessage(mensaje);
            alert.SetNeutralButton("OK", (sender, args) => { });
            alert.Show();
        }
        #endregion
        #endregion

        public string setBodyEmail(string ordenMail, string producto)
        {
            string body = "";
            string body2 = "";

            body2 = "<!doctypehtml><html xmlns=http://www.w3.org/1999/xhtml><meta content='text/html; charset=utf-8'http-equiv=Content-Type><meta content='width=device-width,initial-scale=1'name=viewport><title>CargaEmbarques - Notificación</title><body style=margin:0;padding:0;background-color:#f4f4f4;font-family:Arial,sans-serif><table bgcolor=#f4f4f4 border=0 cellpadding=0 cellspacing=0 role=presentation width=100%><tr><td style=padding:20px align=center><table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 role=presentation width=600 style='border-radius:8px;box-shadow:0 4px 8px rgba(0,0,0,.1);border:1px solid #ddd'><tr><td style=background-color:#fff;padding:20px;border-top-left-radius:8px;border-top-right-radius:8px align=center><img alt=CargaEmbarques src='data:image/png;base64,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'style=display:block;max-width:100%;border:none width=560><tr><td style='font-size:20px;font-weight:700;color:#0a85ea;padding:20px 30px;text-transform:uppercase'align=center>Notificación de Carga Adicional<tr><td style='padding:20px 40px;font-size:16px;color:#333;text-align:justify'>Se ha detectado una carga superior a la cantidad solicitada por ventas en la siguiente orden:<tr><td style=padding:10px align=center><table bgcolor=#d4edda border=0 cellpadding=10 cellspacing=0 role=presentation width=80% style=border-radius:5px><tr><td style=font-size:18px;font-weight:700;color:#155724 align=center>" + ordenMail + "</table><tr><td style='padding:20px 40px;font-size:16px;color:#333;text-align:justify'>Producto involucrado:<tr><td style=padding:10px align=center><table bgcolor=#f8d7da border=0 cellpadding=10 cellspacing=0 role=presentation width=80% style=border-radius:5px><tr><td style=font-size:18px;font-weight:700;color:#721c24 align=center>" + getNombreProducto(producto.Trim()) + "</table><tr><td style='padding:20px 40px;font-size:12px;color:#666;text-align:justify'>Este correo es una notificación automática sobre productos adicionales en órdenes de embarque.<tr><td style=padding:10px;font-size:11px;color:#888;text-align:center>© 2025 CargaEmbarques - Todos los derechos reservados</table></table>";
            return body2;
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

        private string getPTI_Clave(string codigoEtiqueta)
        {
            string result = "";
            // Paso 1: Definir el patrón de la expresión regular
            string pattern = @"^(.{15})(\d{3})\d{3}$";
            // Paso 2: Aplicar la expresión regular a la cadena de entrada
            Match match = Regex.Match(codigoEtiqueta, pattern);

            // Paso 3: Verificar si la expresión regular encontró una coincidencia
            if (match.Success)
            {
                // Paso 4: Capturar los grupos de interés
                string prefix = match.Groups[1].Value; // Los primeros 15 caracteres
                string lastThreeDigits = match.Groups[2].Value; // Los 3 dígitos de interés

                // Paso 5: Transformar los dígitos según la lógica proporcionada
                // "001" se convierte en "0101"
                // Tomamos el primer dígito '0' y lo duplicamos "00"
                // Tomamos el segundo dígito '0' y lo duplicamos "00"
                // Tomamos el tercer dígito '1' y lo duplicamos "11"
                // La concatenación resulta en "0101"
                string transformedDigits = lastThreeDigits[0].ToString() + lastThreeDigits[0] +
                                            lastThreeDigits[1].ToString() + lastThreeDigits[1];

                // Paso 6: Construir la cadena resultante
                result = prefix + transformedDigits;

            }
            else
            {
                return result;
            }

            return result;
        }

        private void validarCapturas(string captura, out string mtip, out string mfol, out string mcod, out string mtar, out string mcaj)
        {
            mtip = mfol = mcod = mtar = mcaj = null;

            if (string.IsNullOrEmpty(captura) || captura.Length < 3)
            {
                Limpiar();
                //Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                //alertDialog.SetTitle(Html.FromHtml("<font color='#fa6400' size = 10>Error en la estructura de Etiqueta</font>"));
                //alertDialog.SetIcon(Resource.Drawable.Info);
                //alertDialog.SetMessage(Html.FromHtml("<font color='#fff000' size = 10>Etiqueta no encontrada por favor tomar una evidencia de la etiqueta leida y ponerse en contacto con los desarrolladores.</font>"));
                //alertDialog.SetCancelable(false);
                //alertDialog.SetNeutralButton("Ok", delegate
                //{
                //    alertDialog.Dispose();
                //});

                //RunOnUiThread(() => alertDialog.Show());

                return;
            }

            mcaj = captura.Substring(captura.Length - 3, 3);
            captura = captura.Substring(0, captura.Length - 3);

            string querySSCC = "SELECT recibo, tarima, prod_clave, tipo FROM tb_det_trazabilidad WHERE pti_clave = @captura";

            using (SqlConnection thisConnectionA = new SqlConnection(cadenaConexion))
            {
                thisConnectionA.Open();

                using (SqlCommand sqlCommand = new SqlCommand(querySSCC, thisConnectionA))
                {
                    sqlCommand.Parameters.AddWithValue("@captura", captura.Trim());

                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                mfol = sqlDataReader["recibo"].ToString().Trim();
                                mtar = sqlDataReader["tarima"].ToString().Trim();
                                mcod = sqlDataReader["prod_clave"].ToString().Trim();
                                mtip = sqlDataReader["tipo"].ToString().Trim();
                            }
                        }

                    }
                }
            }
        }

        public override void OnBackPressed()
        {
            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Cerrar Sesion</font>"));
            alertDialog.SetIcon(Resource.Drawable.question);
            alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>¿Desea Cerrar su sesion en este equipo?</font>"));
            alertDialog.SetPositiveButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Sí</font>"), SaveAction);
            alertDialog.SetNegativeButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>No</font>"), CancelaAction);
            alertDialog.Create();
            alertDialog.Show();
        }//end onBackPressed()

        private void PedidoS_KeyPress(object sender, View.KeyEventArgs e)
        {
            string pdnobs = "";
            if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter) && pedido.Text.Trim() != "")
            {
                //Borrar datos almacenados de la bd
                string ordenventa = pedido.Text.Trim();
                Limpiar();
                pedido.Text = ordenventa;

                #region FOLIO NO ES VALIDO
                if (ordenventa.Trim().Length > 0)
                {
                    if (ordenventa.Trim().Length < 6 || ordenventa.Trim().Length > 6)
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>El Folio No Es Valido</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El pedido: " + ordenventa + " debe ser de 6 caracteres</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                        });
                        alertDialog.Show();
                    }
                }
                #endregion

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                query = "Select * from tb_mstr_embarque Where emb_folio = '" + ordenventa + "'";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = thisConnection;
                SqlDataReader Info;
                Info = cmd.ExecuteReader();
                string TipoEmb = "";
                string Transporte = "";
                string pdn_sts = "";
                string HrTrailer = "";

                string estatusor = "";

                while (Info.Read())
                {
                    estatusor = Info["STS"].ToString().Trim();
                    Anden.Text = Info["anden"].ToString().Trim();
                    if (ordenventa == Info["EMB_FOLIO"].ToString().Trim())
                    {
                        if (Info["STS"].ToString().Trim() == "R" || Info["STS"].ToString().Trim() == "T")
                        {
                            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                            AlertDialog alert = dialog.Create();
                            alert.SetTitle("AVISO!!");
                            alert.SetMessage("El Embarque ya fue Guardado, Desea Visualizarlo?");
                            alert.SetIcon(Resource.Drawable.warning);
                            alert.SetButton("SI", (c, ev) =>
                            {
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                ModificaPedido = "N";
                                mostrar_emb();
                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }
                                TRAE_PESO();
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                updatePesoPorEjes(Notrailer.Text, fecha.Text, "", "", "", "", pedido.Text);
                                return;
                            });
                            alert.SetButton2("NO", (c, ev) =>
                            {
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                Limpiar();
                                return;
                            });
                            alert.Show();
                        }
                        else
                        {
                            ModificaPedido = "S";
                            pedido.Text = ordenventa.Trim();
                            fecha.Text = Info["hora_trailer"].ToString().Trim();
                            if (fecha.Text.Trim().Length > 10)
                            {
                                string[] fechatrailer = fecha.Text.Trim().Split(" ");
                                fecha.Text = fechatrailer[0].Trim();
                            }
                            horainicial.Text = Info["hora_ini"].ToString().Trim();
                            Notrailer.Text = Info["no_trailer"].ToString().Trim();
                            HrTrailer = Info["hora_trailer"].ToString().Trim();


                            if (Info["hora_fin"].ToString().Trim() == null || Info["hora_fin"].ToString().Trim() == "")
                            {
                                Horafinal.Text = "--:--";
                            }
                            else
                            {
                                Horafinal.Text = Info["hora_fin"].ToString().Trim();
                            }
                            TipoEmb = Info["emb_tipo"].ToString().Trim();
                            tipopedido = TipoEmb.Trim();

                            switch (Info["emb_tipo"].ToString().Trim())
                            {
                                case "FC":
                                    lugar.Text = "Cancún";
                                    break;
                                case "FG":
                                    lugar.Text = "Guadalajara";
                                    break;
                                case "FD":
                                    lugar.Text = "Distrito Federal";
                                    break;
                                case "FE":
                                    lugar.Text = "Externos";
                                    break;
                                case "FV":
                                    lugar.Text = "Puerto Vallarta";
                                    break;
                                case "FM":
                                    lugar.Text = "Cuautitlan";
                                    break;
                                case "EXP":
                                    lugar.Text = "Exportación";
                                    break;
                                case "NAL":
                                    lugar.Text = "Nacional";
                                    break;
                                case "TRA":
                                    lugar.Text = "Maquila";
                                    break;
                            }
                            Anden.Text = Info["anden"].ToString().Trim();


                            //ClaveAnden.Enabled = true;
                            if (Mymenu != null)
                            {
                                Mymenu.FindItem(Resource.Id.MenuItem5).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem6).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem7).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem8).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem8DE).SetEnabled(true);
                            }
                            iniarCarga.Enabled = false;
                            //iniarCarga.Visibility = ViewStates.Invisible;
                            codigoetiqueta.Enabled = true;
                            codigoetiqueta.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                            confirmprod.Enabled = false;
                            temperatura.Enabled = false;
                            TipoTar.Enabled = false;
                            Posicion.Enabled = false;
                            Cajas.Enabled = false;
                            //fotoevent.Visibility = Android.Views.ViewStates.Invisible;
                            fotoevent.Enabled = false;
                            TRAE_PESO();
                            LLenaDetPed(ordenventa, TipoEmb.Trim());
                        }
                    }

                    if (TipoEmb == "NAL")
                    {
                        query = "SELECT pdn_folio, prov_clave, pdn_observacion, pdn_diasmin, pdn_estatus, cnte_clave, cve_subcli FROM tb_mstr_pedidos_nal WHERE pdn_folio = '" + ordenventa + "'";
                    }
                    else
                    {
                        query = "SELECT pdn_folio, prov_clave, pdn_observacion, pdn_diasmin, pdn_estatus, cnte_clave, cve_subcli FROM tb_mstr_pedidos_exp WHERE pdn_folio = '" + ordenventa + "'";
                    }

                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }

                    cmd = new SqlCommand(query);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        cnte_clave = Info["cnte_clave"].ToString().Trim();
                        cve_subcli = Info["cve_subcli"].ToString().Trim();
                        Transporte = Info["prov_clave"].ToString().Trim();
                        pdnobs = Info["pdn_observacion"].ToString().Trim();
                        try
                        {
                            pdn_diasmin = Convert.ToInt32(Info["pdn_diasmin"].ToString().Trim());
                        }
                        catch
                        {
                            pdn_diasmin = 12;
                        }
                    }

                    AsignarAnden();
                    validaVidaAnaquel = getVidaAnaquel(ordenventa);
                    //Comentada la funcion de actualizacion de trasnporte debido a los errores que vigilancia no podia darles salida

                    /*cadena = "UPDATE tb_mstr_trailer set transporte = '" + Transporte + "' WHERE no_trailer = '" + Notrailer.Text + "' and hora_trailer = '" + HrTrailer + "'";
                    cmd = new SqlCommand(cadena, thisConnection);
                    cmd.ExecuteNonQuery();*/
                    if (Convert.ToInt32(Anden.Text) != AndenValida && (estatusor.Trim() != "R" && estatusor.ToString().Trim() == "T"))
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>UNIDAD INCORRECTA " + Convert.ToInt32(Anden.Text.Trim()) + " && " + Convert.ToInt32(AndenValida) + "</font>"));
                        alertDialog.SetIcon(Resource.Drawable.Info);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden no se puede cargar en este Dispositivo, debido a que no esta designado para este anden</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                            Limpiar();
                        });
                        alertDialog.Show();
                        return;
                    }

                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    if (pdnobs.Trim().Length > 0)
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Pedido con Observaciones</font>"));
                        alertDialog.SetIcon(Resource.Drawable.Info);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>" + pdnobs.Trim() + "</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                            if (iniarCarga.Enabled == false)
                            {
                                updatePesoPorEjes(Notrailer.Text, fecha.Text, "", "", "", "", pedido.Text);
                                codigoetiqueta.Enabled = true;
                                codigoetiqueta.RequestFocus();
                                //InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                //imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                            }
                        });
                        alertDialog.Show();
                    }
                    else
                    {
                        if (iniarCarga.Enabled == false)
                        {

                            codigoetiqueta.Enabled = true;
                            codigoetiqueta.RequestFocus();
                            //InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            //imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                            updatePesoPorEjes(Notrailer.Text, fecha.Text, FolioProducto, ProductoProducto, TarimaProducto, codigoetiqueta.Text, pedido.Text);
                        }
                    }
                    //653095
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    cadena = "SELECT * FROM tb_det_pend_embarque WHERE hora_trailer = '" + HrTrailer + "' AND no_trailer = '" + Notrailer.Text + "' AND estatus = 'A'";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    string detallec = "";
                    string responsablec = "";
                    string subioad = "";
                    while (Info.Read())
                    {
                        detallec = Info["observaciones"].ToString().Trim();
                        responsablec = Info["arearesp"].ToString().Trim();
                        subioad = Info["solicitante"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    if (detallec.Trim() != "")
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#FF8300' size = 10>UNIDAD CON CARGA ADICIONAL</font>"));
                        alertDialog.SetIcon(Resource.Drawable.warning);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#009BFF' size = 10>" + detallec + " - Area Responsable: " + responsablec + " - Solicitante:" + subioad + "</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                        });
                        alertDialog.Show();

                    }


                    return;
                }

                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (Notrailer.Text.Trim() == "")
                {
                    TipoEmb = "NAL";
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    cadena = "Select * from tb_mstr_pedidos_nal Where pdn_folio = '" + ordenventa + "'";
                    if (Convert.ToInt32(ordenventa) < 400000)
                    {
                        TipoEmb = "EXP";
                        cadena = "Select * from tb_mstr_pedidos_exp Where pdn_folio = '" + ordenventa + "'";
                    }
                    Transporte = "";
                    string pedidoorigen = "";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        pdnobs = Info["pdn_observacion"].ToString().Trim();
                        if (Info["PDN_PEDORIGEN"].ToString().Trim() == "0" || Info["PDN_PEDORIGEN"].ToString().Trim() == "")
                        {
                            pedidoorigen = Info["PDN_FOLIO"].ToString().Trim();
                        }
                        else
                        {
                            pedidoorigen = Info["PDN_PEDORIGEN"].ToString().Trim();
                        }
                        pdn_sts = Info["pdn_estatus"].ToString().Trim();
                    }



                    if (pdn_sts == "C")
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#F31608' size = 10>ORDEN DE VENTA CANCELADA</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#6E0000' size = 10>LA ORDEN DE VENTA ESTA CANCELADA! INFORMAR A VENTAS</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                            Limpiar();
                        });
                        alertDialog.Show();
                        return;
                    }
                    cadena = "Select * from  tb_mstr_trailer Where pdn_folio = '" + pedidoorigen + "'";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    Transporte = "";
                    while (Info.Read())
                    {
                        Notrailer.Text = Info["no_trailer"].ToString().Trim();
                        Anden.Text = Info["anden"].ToString().Trim();
                        LblFT.Text = Info["hora_trailer"].ToString().Trim();
                    }

                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    switch (TipoEmb.Trim())
                    {
                        case "FC":
                            lugar.Text = "Cancún";
                            break;
                        case "FG":
                            lugar.Text = "Guadalajara";
                            break;
                        case "FD":
                            lugar.Text = "Distrito Federal";
                            break;
                        case "FE":
                            lugar.Text = "Externos";
                            break;
                        case "FV":
                            lugar.Text = "Puerto Vallarta";
                            break;
                        case "FM":
                            lugar.Text = "Cuautitlan";
                            break;
                        case "EXP":
                            lugar.Text = "Exportación";
                            break;
                        case "NAL":
                            lugar.Text = "Nacional";
                            break;
                        case "TRA":
                            lugar.Text = "Maquila";
                            break;
                    }


                }

                if (Notrailer.Text.Trim() == "")
                {
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    cadena = "Select * from  tb_mstr_trailer WHERE (no_trailer = 'PC') AND (tempfin = '')";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    Transporte = "";
                    while (Info.Read())
                    {
                        Notrailer.Text = Info["no_trailer"].ToString().Trim();
                        Anden.Text = Info["anden"].ToString().Trim();
                        LblFT.Text = Info["hora_trailer"].ToString().Trim();
                        iniarCarga.Enabled = true;
                        //iniarCarga.Visibility = ViewStates.Visible;
                        Ordenes.Enabled = false;
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    if (Notrailer.Text.Trim() == "")
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>No se Encuentra el Propio Conducto</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Propio Conducto no ha sido registrado</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                        });
                        alertDialog.Show();

                    }
                }
                else
                {
                    iniarCarga.Enabled = true;
                    //iniarCarga.Visibility = ViewStates.Visible;
                    Ordenes.Enabled = false;
                }

                if (Convert.ToInt32(Anden.Text.Trim()) != AndenValida)
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>UNIDAD INCORRECTA " + Convert.ToInt32(Anden.Text.Trim()) + " && " + Convert.ToInt32(AndenValida) + "</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden no se puede cargar en este Dispositivo, debido a que no esta designado para este anden</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        Limpiar();
                    });
                    alertDialog.Show();
                    return;
                }

                if (pdnobs.Trim().Length > 0)
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Pedido con Observaciones</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>" + pdnobs.Trim() + "</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        if (iniarCarga.Enabled == false)
                        {
                            codigoetiqueta.Enabled = true;
                            codigoetiqueta.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                        }
                    });
                    alertDialog.Show();
                }
                else
                {
                    if (iniarCarga.Enabled == false)
                    {
                        codigoetiqueta.Enabled = true;
                        codigoetiqueta.RequestFocus();
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                    }
                }

                //validacion de Carga Adicional
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                cadena = "SELECT * FROM tb_det_pend_embarque WHERE hora_trailer = '" + HrTrailer + "' AND no_trailer = '" + Notrailer.Text + "' AND estatus = 'A'";
                cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                Info = cmd.ExecuteReader();
                string detallecX = "";
                string responsablecX = "";
                string subioadX = "";
                while (Info.Read())
                {
                    detallecX = Info["observaciones"].ToString().Trim();
                    responsablecX = Info["arearesp"].ToString().Trim();
                    subioadX = Info["solicitante"].ToString().Trim();
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (detallecX.Trim() != "")
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>UNIDAD CON CARGA ADICIONAL</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>" + detallecX + " - Area Responsable: " + responsablecX + " - Solicitante:" + subioadX + "</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                    });
                    alertDialog.Show();

                }
            }
            else
            {
                e.Handled = false;
            }

        }
        private void Pedido_KeyPress(object sender, View.KeyEventArgs e)
        {
            string pdnobs = "";
            if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter) && pedido.Text.Trim() != "")
            {
                //Borrar datos almacenados de la bd
                string ordenventa = pedido.Text.Trim();
                Limpiar();
                pedido.Text = ordenventa;

                if (ordenventa.Trim().Length > 0)
                {
                    if (ordenventa.Trim().Length < 6 || ordenventa.Trim().Length > 6)
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>El Folio No Es Valido</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El pedido: " + ordenventa + " debe ser de 6 caracteres</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                        });
                        alertDialog.Show();
                    }
                }

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                query = "Select * from tb_mstr_embarque Where emb_folio = '" + ordenventa + "'";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = thisConnection;
                SqlDataReader Info;
                Info = cmd.ExecuteReader();
                string TipoEmb = "";
                string Transporte = "";
                string pdn_sts = "";
                string HrTrailer = "";
                string estatusor = "";

                while (Info.Read())
                {
                    estatusor = Info["STS"].ToString().Trim();
                    Anden.Text = Info["anden"].ToString().Trim();
                    if (ordenventa == Info["EMB_FOLIO"].ToString().Trim())
                    {
                        if (Info["STS"].ToString().Trim() == "R" || Info["STS"].ToString().Trim() == "T")
                        {
                            AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                            AlertDialog alert = dialog.Create();
                            alert.SetTitle("AVISO!!");
                            alert.SetMessage("El Embarque ya fue Guardado, Desea Visualizarlo?");
                            alert.SetIcon(Resource.Drawable.warning);
                            alert.SetButton("SI", (c, ev) =>
                            {
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                ModificaPedido = "N";
                                mostrar_emb();
                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }
                                TRAE_PESO();
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                updatePesoPorEjes(Notrailer.Text, fecha.Text, "", "", "", "", pedido.Text);
                                return;
                            });
                            alert.SetButton2("NO", (c, ev) =>
                            {
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                Limpiar();
                                return;
                            });
                            alert.Show();
                        }
                        else
                        {
                            ModificaPedido = "S";
                            pedido.Text = ordenventa.Trim();
                            fecha.Text = Info["hora_trailer"].ToString().Trim();
                            if (fecha.Text.Trim().Length > 10)
                            {
                                string[] fechatrailer = fecha.Text.Trim().Split(" ");
                                fecha.Text = fechatrailer[0].Trim();
                            }
                            horainicial.Text = Info["hora_ini"].ToString().Trim();
                            Notrailer.Text = Info["no_trailer"].ToString().Trim();
                            HrTrailer = Info["hora_trailer"].ToString().Trim();

                            if (Info["hora_fin"].ToString().Trim() == null || Info["hora_fin"].ToString().Trim() == "")
                            {
                                Horafinal.Text = "--:--";
                            }
                            else
                            {
                                Horafinal.Text = Info["hora_fin"].ToString().Trim();
                            }
                            TipoEmb = Info["emb_tipo"].ToString().Trim();
                            tipopedido = TipoEmb.Trim();

                            switch (Info["emb_tipo"].ToString().Trim())
                            {
                                case "FC":
                                    lugar.Text = "Cancún";
                                    break;
                                case "FG":
                                    lugar.Text = "Guadalajara";
                                    break;
                                case "FD":
                                    lugar.Text = "Distrito Federal";
                                    break;
                                case "FE":
                                    lugar.Text = "Externos";
                                    break;
                                case "FV":
                                    lugar.Text = "Puerto Vallarta";
                                    break;
                                case "FM":
                                    lugar.Text = "Cuautitlan";
                                    break;
                                case "EXP":
                                    lugar.Text = "Exportación";
                                    break;
                                case "NAL":
                                    lugar.Text = "Nacional";
                                    break;
                                case "TRA":
                                    lugar.Text = "Maquila";
                                    break;
                            }
                            Anden.Text = Info["anden"].ToString().Trim();

                            if (Mymenu != null)
                            {
                                Mymenu.FindItem(Resource.Id.MenuItem5).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem6).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem7).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem8).SetEnabled(true);
                                Mymenu.FindItem(Resource.Id.MenuItem8DE).SetEnabled(true);
                            }
                            iniarCarga.Enabled = false;
                            codigoetiqueta.Enabled = true;
                            codigoetiqueta.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                            confirmprod.Enabled = false;
                            temperatura.Enabled = false;
                            TipoTar.Enabled = false;
                            Posicion.Enabled = false;
                            Cajas.Enabled = false;
                            fotoevent.Enabled = false;
                            TRAE_PESO();
                            LLenaDetPed(ordenventa, TipoEmb.Trim());
                        }
                    }

                    if (TipoEmb == "NAL")
                    {
                        query = "SELECT pdn_folio, prov_clave, pdn_observacion, pdn_diasmin, pdn_estatus, cnte_clave, cve_subcli FROM tb_mstr_pedidos_nal WHERE pdn_folio = '" + ordenventa + "'";
                    }
                    else
                    {
                        query = "SELECT pdn_folio, prov_clave, pdn_observacion, pdn_diasmin, pdn_estatus, cnte_clave, cve_subcli FROM tb_mstr_pedidos_exp WHERE pdn_folio = '" + ordenventa + "'";
                    }

                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }

                    cmd = new SqlCommand(query);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        cnte_clave = Info["cnte_clave"].ToString().Trim();
                        cve_subcli = Info["cve_subcli"].ToString().Trim();
                        Transporte = Info["prov_clave"].ToString().Trim();
                        pdnobs = Info["pdn_observacion"].ToString().Trim();
                        try
                        {
                            pdn_diasmin = Convert.ToInt32(Info["pdn_diasmin"].ToString().Trim());
                        }
                        catch
                        {
                            pdn_diasmin = 12;
                        }
                    }

                    AsignarAnden();
                    validaVidaAnaquel = getVidaAnaquel(ordenventa);

                    if (Convert.ToInt32(Anden.Text) != AndenValida && (estatusor.Trim() != "R" && estatusor.ToString().Trim() == "T"))
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>UNIDAD INCORRECTA " + Convert.ToInt32(Anden.Text.Trim()) + " && " + Convert.ToInt32(AndenValida) + "</font>"));
                        alertDialog.SetIcon(Resource.Drawable.Info);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden no se puede cargar en este Dispositivo, debido a que no esta designado para este anden</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                            Limpiar();
                        });
                        alertDialog.Show();
                        return;
                    }

                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    // ===================================================================
                    // NUEVA VALIDACIÓN: Verificar observaciones de HEB en tb_det_pedidos
                    // ===================================================================
                    if (TipoEmb == "EXP")
                    {
                        var observacionesHEB = ObtenerObservacionesHEB(ordenventa);
                        if (observacionesHEB.Count > 0)
                        {
                            MostrarAlertaObservacionesHEB(observacionesHEB, () =>
                            {
                                // Callback al cerrar el diálogo - continuar flujo normal
                                ContinuarFlujoDespuesObservaciones(pdnobs, iniarCarga.Enabled, ordenventa, HrTrailer);
                            });
                            return; // Detener ejecución aquí, el callback continuará
                        }
                    }
                    // ===================================================================

                    // Si no hay observaciones HEB, continuar con flujo normal
                    ContinuarFlujoDespuesObservaciones(pdnobs, iniarCarga.Enabled, ordenventa, HrTrailer);

                    return;
                }

                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (Notrailer.Text.Trim() == "")
                {
                    TipoEmb = "NAL";
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    cadena = "Select * from tb_mstr_pedidos_nal Where pdn_folio = '" + ordenventa + "'";
                    if (Convert.ToInt32(ordenventa) < 400000)
                    {
                        TipoEmb = "EXP";
                        cadena = "Select * from tb_mstr_pedidos_exp Where pdn_folio = '" + ordenventa + "'";
                    }
                    Transporte = "";
                    string pedidoorigen = "";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        pdnobs = Info["pdn_observacion"].ToString().Trim();
                        if (Info["PDN_PEDORIGEN"].ToString().Trim() == "0" || Info["PDN_PEDORIGEN"].ToString().Trim() == "")
                        {
                            pedidoorigen = Info["PDN_FOLIO"].ToString().Trim();
                        }
                        else
                        {
                            pedidoorigen = Info["PDN_PEDORIGEN"].ToString().Trim();
                        }
                        pdn_sts = Info["pdn_estatus"].ToString().Trim();
                    }

                    if (pdn_sts == "C")
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#F31608' size = 10>ORDEN DE VENTA CANCELADA</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#6E0000' size = 10>LA ORDEN DE VENTA ESTA CANCELADA! INFORMAR A VENTAS</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                            Limpiar();
                        });
                        alertDialog.Show();
                        return;
                    }
                    cadena = "Select * from  tb_mstr_trailer Where pdn_folio = '" + pedidoorigen + "'";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    Transporte = "";
                    while (Info.Read())
                    {
                        Notrailer.Text = Info["no_trailer"].ToString().Trim();
                        Anden.Text = Info["anden"].ToString().Trim();
                        LblFT.Text = Info["hora_trailer"].ToString().Trim();
                    }

                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    switch (TipoEmb.Trim())
                    {
                        case "FC":
                            lugar.Text = "Cancún";
                            break;
                        case "FG":
                            lugar.Text = "Guadalajara";
                            break;
                        case "FD":
                            lugar.Text = "Distrito Federal";
                            break;
                        case "FE":
                            lugar.Text = "Externos";
                            break;
                        case "FV":
                            lugar.Text = "Puerto Vallarta";
                            break;
                        case "FM":
                            lugar.Text = "Cuautitlan";
                            break;
                        case "EXP":
                            lugar.Text = "Exportación";
                            break;
                        case "NAL":
                            lugar.Text = "Nacional";
                            break;
                        case "TRA":
                            lugar.Text = "Maquila";
                            break;
                    }
                }

                if (Notrailer.Text.Trim() == "")
                {
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    cadena = "Select * from  tb_mstr_trailer WHERE (no_trailer = 'PC') AND (tempfin = '')";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    Transporte = "";
                    while (Info.Read())
                    {
                        Notrailer.Text = Info["no_trailer"].ToString().Trim();
                        Anden.Text = Info["anden"].ToString().Trim();
                        LblFT.Text = Info["hora_trailer"].ToString().Trim();
                        iniarCarga.Enabled = true;
                        Ordenes.Enabled = false;
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    if (Notrailer.Text.Trim() == "")
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>No se Encuentra el Propio Conducto</font>"));
                        alertDialog.SetIcon(Resource.Drawable.no);
                        alertDialog.SetCancelable(false);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Propio Conducto no ha sido registrado</font>"));
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();
                        });
                        alertDialog.Show();
                    }
                }
                else
                {
                    iniarCarga.Enabled = true;
                    Ordenes.Enabled = false;
                }

                if (Convert.ToInt32(Anden.Text.Trim()) != AndenValida)
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>UNIDAD INCORRECTA " + Convert.ToInt32(Anden.Text.Trim()) + " && " + Convert.ToInt32(AndenValida) + "</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden no se puede cargar en este Dispositivo, debido a que no esta designado para este anden</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        Limpiar();
                    });
                    alertDialog.Show();
                    return;
                }

                // ===================================================================
                // NUEVA VALIDACIÓN: Verificar observaciones HEB para pedidos nuevos
                // ===================================================================
                if (TipoEmb == "EXP")
                {
                    var observacionesHEB = ObtenerObservacionesHEB(ordenventa);
                    if (observacionesHEB.Count > 0)
                    {
                        MostrarAlertaObservacionesHEB(observacionesHEB, () =>
                        {
                            ContinuarFlujoPedidoNuevo(pdnobs, ordenventa, HrTrailer);
                        });
                        return;
                    }
                }
                // ===================================================================

                ContinuarFlujoPedidoNuevo(pdnobs, ordenventa, HrTrailer);
            }
            else
            {
                e.Handled = false;
            }
        }

        #region VALIDACIONES DE OBSERVACIONES HEB
        /// <summary>
        /// Obtiene las observaciones de HEB desde tb_det_pedidos
        /// </summary>
        private List<ObservacionHEB> ObtenerObservacionesHEB(string folio)
        {
            var lista = new List<ObservacionHEB>();

            try
            {
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }

                string query = @"SELECT prod_clave, pdn_subcli, pdn_observaciones 
                        FROM tb_det_pedidos 
                        WHERE pdn_folio = @folio 
                        AND pdn_tipo = 'EXP' 
                        AND pdn_subcli LIKE '%HEB%'
                        AND (pdn_observaciones IS NOT NULL AND LTRIM(RTRIM(pdn_observaciones)) != '')";

                using (SqlCommand cmd = new SqlCommand(query, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@folio", folio);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ObservacionHEB
                            {
                                Producto = reader["prod_clave"]?.ToString() ?? "N/A",
                                Cliente = reader["pdn_subcli"]?.ToString() ?? "N/A",
                                Observaciones = reader["pdn_observaciones"]?.ToString() ?? "Sin observaciones"
                            });
                        }
                    }
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Error("ObtenerObservacionesHEB", ex.Message);
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open)
                {
                    thisConnection.Close();
                }
            }

            return lista;
        }

        /// <summary>
        /// Muestra el diálogo de observaciones HEB con formato de tabla
        /// </summary>
        private void MostrarAlertaObservacionesHEB(List<ObservacionHEB> observaciones, Action onDismiss)
        {
            // Construir tabla HTML
            StringBuilder html = new StringBuilder();
            html.Append("<table style='width:100%; border-collapse: collapse;'>");
            html.Append("<tr style='background-color: #D32F2F; color: white;'>");
            html.Append("<th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Producto</th>");
            html.Append("<th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Cliente</th>");
            html.Append("<th style='padding: 8px; border: 1px solid #ddd; text-align: left;'>Observaciones</th>");
            html.Append("</tr>");

            bool alternar = false;
            foreach (var obs in observaciones)
            {
                string bgColor = alternar ? "#f2f2f2" : "#ffffff";
                html.Append($"<tr style='background-color: {bgColor};'>");
                html.Append($"<td style='padding: 8px; border: 1px solid #ddd;'><b>{obs.Producto}</b></td>");
                html.Append($"<td style='padding: 8px; border: 1px solid #ddd; color: #FF6F00;'>{obs.Cliente}</td>");
                html.Append($"<td style='padding: 8px; border: 1px solid #ddd;'>{obs.Observaciones}</td>");
                html.Append("</tr>");
                alternar = !alternar;
            }
            html.Append("</table>");

            // Crear ScrollView con WebView para renderizar HTML
            WebView webView = new WebView(this);
            webView.LoadData(html.ToString(), "text/html", "UTF-8");
            webView.SetPadding(10, 10, 10, 10);

            ScrollView scrollView = new ScrollView(this);
            scrollView.AddView(webView);

            // Calcular altura dinámica (máximo 70% de pantalla)
            var displayMetrics = Resources.DisplayMetrics;
            int maxHeight = (int)(displayMetrics.HeightPixels * 0.7);
            scrollView.LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                maxHeight);

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(Html.FromHtml("<font color='#D32F2F'>Pedido con Observaciones HEB</font>"));
            builder.SetIcon(Resource.Drawable.warning);
            builder.SetView(scrollView);
            builder.SetCancelable(false);
            builder.SetPositiveButton("ENTENDIDO", (sender, e) =>
            {
                onDismiss?.Invoke();
            });

            AlertDialog dialog = builder.Create();
            dialog.Show();

            // Estilizar botón
            var button = dialog.GetButton((int)DialogButtonType.Positive);
            if (button != null)
            {
                button.SetTextColor(Android.Graphics.Color.ParseColor("#2E7D32"));
                button.SetBackgroundResource(Resource.Drawable.buttonAceptar);
            }
        }

        /// <summary>
        /// Continúa el flujo después de mostrar observaciones (embarque existente)
        /// </summary>
        private void ContinuarFlujoDespuesObservaciones(string pdnobs, bool inicargaEnabled, string ordenventa, string HrTrailer)
        {
            if (pdnobs.Trim().Length > 0)
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Pedido con Observaciones</font>"));
                alertDialog.SetIcon(Resource.Drawable.Info);
                alertDialog.SetCancelable(false);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>" + pdnobs.Trim() + "</font>"));
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                    if (inicargaEnabled == false)
                    {
                        updatePesoPorEjes(Notrailer.Text, fecha.Text, "", "", "", "", pedido.Text);
                        codigoetiqueta.Enabled = true;
                        codigoetiqueta.RequestFocus();
                    }
                });
                alertDialog.Show();
            }
            else
            {
                if (inicargaEnabled == false)
                {
                    codigoetiqueta.Enabled = true;
                    codigoetiqueta.RequestFocus();
                    updatePesoPorEjes(Notrailer.Text, fecha.Text, FolioProducto, ProductoProducto, TarimaProducto, codigoetiqueta.Text, pedido.Text);
                }
            }

            // Validación de Carga Adicional
            ValidarCargaAdicional(HrTrailer);
        }

        /// <summary>
        /// Continúa el flujo para pedidos nuevos
        /// </summary>
        private void ContinuarFlujoPedidoNuevo(string pdnobs, string ordenventa, string HrTrailer)
        {
            if (pdnobs.Trim().Length > 0)
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Pedido con Observaciones</font>"));
                alertDialog.SetIcon(Resource.Drawable.Info);
                alertDialog.SetCancelable(false);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>" + pdnobs.Trim() + "</font>"));
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                    if (iniarCarga.Enabled == false)
                    {
                        codigoetiqueta.Enabled = true;
                        codigoetiqueta.RequestFocus();
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                    }
                });
                alertDialog.Show();
            }
            else
            {
                if (iniarCarga.Enabled == false)
                {
                    codigoetiqueta.Enabled = true;
                    codigoetiqueta.RequestFocus();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                }
            }

            // Validación de Carga Adicional
            ValidarCargaAdicional(HrTrailer);
        }

        /// <summary>
        /// Valida si hay carga adicional pendiente
        /// </summary>
        private void ValidarCargaAdicional(string HrTrailer)
        {
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }

            string cadena = "SELECT * FROM tb_det_pend_embarque WHERE hora_trailer = '" + HrTrailer + "' AND no_trailer = '" + Notrailer.Text + "' AND estatus = 'A'";
            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();

            string detallec = "";
            string responsablec = "";
            string subioad = "";

            while (Info.Read())
            {
                detallec = Info["observaciones"].ToString().Trim();
                responsablec = Info["arearesp"].ToString().Trim();
                subioad = Info["solicitante"].ToString().Trim();
            }

            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            if (detallec.Trim() != "")
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#FF8300' size = 10>UNIDAD CON CARGA ADICIONAL</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetCancelable(false);
                alertDialog.SetMessage(Html.FromHtml("<font color='#009BFF' size = 10>" + detallec + " - Area Responsable: " + responsablec + " - Solicitante:" + subioad + "</font>"));
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                });
                alertDialog.Show();
            }
        }

        /// <summary>
        /// Clase modelo para observaciones HEB
        /// </summary>
        private class ObservacionHEB
        {
            public string Producto { get; set; }
            public string Cliente { get; set; }
            public string Observaciones { get; set; }
        }
        #endregion

        private void AsignarAnden()
        {
            Android.Telephony.TelephonyManager mTelephonyMgr;
            mTelephonyMgr = (Android.Telephony.TelephonyManager)GetSystemService(TelephonyService);
            string uniqueID = UUID.RandomUUID().ToString();
            //string imei = mTelephonyMgr.DeviceId;
            string imei = uniqueID;

            var deviceId = CrossDeviceInfo.Current.Id;

            if (imei == null || imei.Length > 17)
            {
                imei = deviceId;

            }

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cmnd = thisConnection.CreateCommand();
            cmnd.CommandText = "select Id_Anden from Tb_Cat_Anden Where ClaveTablet = '" + imei + "' AND Estado= 'A'";
            AndenValida = Convert.ToInt32(cmnd.ExecuteScalar());
            ds.Clear();
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

        }

        #region METODOS UTILIZADOS PARA TOMAR FOTOS A LA CARGA DEL EMBARQUE

        private async void Btnevent_Click(object sender, EventArgs e)
        {
            // 1. Deshabilitar el botón de inmediato para evitar que el usuario haga doble clic mientras espera
            var button = (Android.Widget.Button)sender;
            button.Enabled = false;

            string conse = "0";

            try
            {
                // 2. Abrir la conexión de forma asincrónica (no bloquea la pantalla)
                if (thisConnection.State == ConnectionState.Closed)
                {
                    await thisConnection.OpenAsync();
                }

                // BUENA PRÁCTICA: Usar parámetros evita que la app falle con caracteres especiales y protege de SQL Injection
                cadena = "SELECT conse FROM tb_mstr_trailer WHERE (no_trailer = @noTrailer) AND (hora_trailer = @horaTrailer)";

                using (SqlCommand cmd = new SqlCommand(cadena, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@noTrailer", Notrailer.Text.Trim());
                    cmd.Parameters.AddWithValue("@horaTrailer", fecha.Text.Trim());

                    // 3. Ejecutar la consulta de forma asincrónica
                    using (SqlDataReader Info = await cmd.ExecuteReaderAsync())
                    {
                        while (await Info.ReadAsync())
                        {
                            conse = Info["conse"].ToString().Trim();
                        }
                    }
                }
            }
            catch (Java.Lang.Exception ex)
            {
                // Manejar errores de base de datos de forma segura (ej. sin internet, timeout)
                Android.Util.Log.Error("CargaEmbarques", $"Error de Base de Datos: {ex.Message}");
            }
            finally
            {
                // 4. Asegurar que la conexión se cierre y el botón se vuelva a activar siempre
                if (thisConnection.State == ConnectionState.Open)
                {
                    thisConnection.Close();
                }
                button.Enabled = true;
            }

            // 5. Lanzar el Intent de manera fluida una vez obtenidos los datos
            Intent intent = new Intent(this, typeof(subirFoto));
            intent.PutExtra("responsable", responsable.ToString().Trim());
            intent.PutExtra("OrdenVenta", pedido.Text.ToString().Trim());
            intent.PutExtra("Posicion", Posicion.Text.ToString().Trim());
            intent.PutExtra("placastrailer", Notrailer.Text.Trim());
            intent.PutExtra("fechatrailer", fecha.Text.Trim());
            intent.PutExtra("conse", conse.Trim());
            intent.PutExtra("imei", imei.Trim().ToString());

            StartActivityForResult(intent, PICK_CONTACT_REQUEST);
        }
        private void Btnevent_ClickLEGACY(object sender, EventArgs e)
        {
            string conse = "0";
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cadena = "Select * from  tb_mstr_trailer WHERE (no_trailer = '" + Notrailer.Text.Trim() + "') AND (hora_trailer = '" + fecha.Text.Trim() + "')";
            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                conse = Info["conse"].ToString().Trim();
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }


            Intent intent = new Intent(this, typeof(subirFoto));
            intent.PutExtra("responsable", responsable.ToString().Trim());
            intent.PutExtra("OrdenVenta", pedido.Text.ToString().Trim());
            intent.PutExtra("Posicion", Posicion.Text.ToString().Trim());
            //intent.PutExtra("Posicion", "2");
            intent.PutExtra("placastrailer", Notrailer.Text.Trim());
            intent.PutExtra("fechatrailer", fecha.Text.Trim());
            intent.PutExtra("conse", conse.Trim());
            intent.PutExtra("imei", imei.Trim().ToString());
            StartActivityForResult(intent, PICK_CONTACT_REQUEST);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == PICK_CONTACT_REQUEST && resultCode == Result.Ok)
            {
                if (CapturaSplitActiva == "1")
                {
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                    codigoetiqueta.Text = "";
                    codigoetiqueta.Enabled = false;
                    confirmprod.Text = "";
                    confirmprod.Enabled = false;
                    Posicion.Text = "";
                    Posicion.Enabled = false;
                    temperatura.Text = "";
                    temperatura.Enabled = false;
                    TRAE_PESO();
                    Cajas.Text = "0";
                    Cajas.Enabled = false;
                    //fotoevent.Visibility = Android.Views.ViewStates.Invisible;
                    fotoevent.Enabled = false;
                    TipoTar.Enabled = false;
                    //codigoetiqueta.Text = "";
                    codigoetiqueta.Enabled = true;
                    codigoetiqueta.RequestFocus();
                    //InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    //imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
                }
                else
                {
                    fotoevent.Enabled = false;
                    CapturaSplitActiva = "0";
                    Cajas.Enabled = true;
                    Cajas.Text = Cajas.Text.Trim();
                    Cajas.RequestFocus();
                    InputMethodManager immx = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    immx.ShowSoftInput(Cajas, ShowFlags.Implicit);
                }
            }

        }
        #endregion

        private void DropDownFocusChanged(object sender, View.FocusChangeEventArgs args)
        {
            if (TipoTar.HasFocus)
            {
                TipoTar.PerformClick();
            }
            else
            {
                if (Posicion.HasFocus)
                {
                    Posicion.Enabled = true;
                    Posicion.RequestFocus();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(Posicion, ShowFlags.Implicit);
                }
            }
        }

        private void GuardarInformacion()
        {
            WifiManager wifi = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
            if (wifi.IsWifiEnabled == false)
            {
                GuardarLocal GuardaError = new GuardarLocal();
                GuardaError.creartxt("Wifi Deshabilitada");
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Error en el Adaptador WIFI</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Dispositivo no tiene la Wifi Activada, favor de activarlo</font>"));
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                    return;

                });
                alertDialog.Show();
            }


            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
            if (!isOnline)
            {
                GuardarLocal GuardaError = new GuardarLocal();
                GuardaError.creartxt("Error en la conexion de red, No esta conectado a ninguna red");
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Error en la Conexion a Internet</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Dispositivo no Esta conectado a ninguna Red, favor de verificarlo</font>"));
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                    return;

                });
                alertDialog.Show();
                return;
            }

            //validaservidores();


            string V_Recibo = "", V_Prd = "", V_Tip = "", Mtipo = "", MNalExp = "", V_Existe = "", V_FecCad = "", id_pallet = "";
            int L_Cad, V_Tamaño, mtar = 0, mtarf = 0, v_cajas, v_dif, mactual = 0;

            string size = TipoTar.SelectedItem.ToString();



            if (Cajas.Text.Trim().Length <= 0)
            {
                Toast.MakeText(this, "La cantidad debe ser mayor que 0", ToastLength.Long).Show();
                Cajas.Text = "";
                Cajas.RequestFocus();
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(Cajas, ShowFlags.Implicit);
                return;
            }
            int i = 0;
            if (int.TryParse(Cajas.Text.Trim(), out i) == false)
            {
                Toast.MakeText(this, "El dato debe ser numerico", ToastLength.Long).Show();
                Cajas.Text = "";
                Cajas.RequestFocus();
                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(Cajas, ShowFlags.Implicit);
                return;
            }
            if (codigoetiqueta.Text == "")
            {
                Toast.MakeText(this, "Informacion incompleta... Falta el Codigo de la Etiqueta", ToastLength.Long).Show();
                return;
            }
            if (Cajas.Text == "")
            {
                Toast.MakeText(this, "Informacion incompleta... Falta Cajas", ToastLength.Long).Show();
                return;
            }
            if (pedido.Text == "")
            {
                Toast.MakeText(this, "Informacion incompleta... Falta El Folio", ToastLength.Long).Show();
                return;
            }
            if (tipotarima == "")
            {
                Toast.MakeText(this, "Informacion incompleta... Falta El Tipo de Tarima", ToastLength.Long).Show();
                return;
            }
            if (Posicion.Text == "")
            {
                Toast.MakeText(this, "Informacion incompleta... Falta Posicion", ToastLength.Long).Show();
                return;
            }


            V_Tamaño = codigoetiqueta.Text.Trim().Length;

            if (V_Tamaño == 11)
            {
                V_Recibo = codigoetiqueta.Text;
                V_Prd = "";
            }
            else
            {
                /*if (codigoetiqueta.Text.Contains(" ") == true)
                {
                    L_Cad = V_Tamaño - 9;
                    mtar = Convert.ToInt32(codigoetiqueta.Text.Substring(V_Tamaño - 2, 3));
                    mtarf = 0;
                    Mtipo = "PTP";
                }
                else {
                    L_Cad = V_Tamaño - 10;
                    mtar = Convert.ToInt32(codigoetiqueta.Text.Substring(V_Tamaño - 3, 2));
                    mtarf = Convert.ToInt32(codigoetiqueta.Text.Substring(V_Tamaño - 1, 2));
                    Mtipo = "PTC";
                }
                V_Recibo = codigoetiqueta.Text.Substring(0, 6);
                if (codigoetiqueta.Text.Substring(0, 1) == "0") {
                    Mtipo = "PTC";
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                }
                V_Prd = codigoetiqueta.Text.Substring(6, L_Cad);*/



                if ((V_Recibo == "" || V_Prd == "" || Mtipo == "") && codigoetiqueta.Text.Trim().Length == 12)
                {
                    string pti_famous = codigoetiqueta.Text.Trim();
                    if (codigoetiqueta.Text.StartsWith("0"))
                    {
                        pti_famous = codigoetiqueta.Text.TrimStart('0');
                    }
                    //string pti_famous = Regex.Replace(codigoetiqueta.Text, patron, "");

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_famous='" + pti_famous + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if ((V_Recibo == "" || V_Prd == "" || Mtipo == "") && codigoetiqueta.Text.Contains(SerialShippingContainerCode) == true)
                {
                    Match match = Regex.Match(codigoetiqueta.Text, patron);
                    id_pallet = match.Groups[1].Value;

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where id_Pallet='" + id_pallet + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if ((V_Recibo == "" || V_Prd == "" || Mtipo == "") && !Regex.IsMatch(codigoetiqueta.Text, @"\s"))
                {
                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_clave='" + codigoetiqueta.Text.Trim() + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else
                {
                    V_Tamaño = codigoetiqueta.Text.Trim().Length;

                    int posstring = codigoetiqueta.Text.Trim().IndexOf(" ", 0);

                    if (posstring > -1)
                    {
                        DataTable CatalogodeProducto = new DataTable();
                        if (thisConnection.State == ConnectionState.Closed)
                        {
                            thisConnection.Open();
                        }
                        string cade = "Select prod_clave,prod_nombre from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC') order by LEN(prod_clave) DESC";
                        SqlDataAdapter da = new SqlDataAdapter(cade, thisConnection);
                        DataSet ds = new DataSet();
                        da.Fill(ds, "CatalogodeProducto");
                        CatalogodeProducto = ds.Tables["CatalogodeProducto"];
                        if (thisConnection.State == ConnectionState.Open)
                        {
                            thisConnection.Close();
                        }

                        for (int ic = 0; ic < CatalogodeProducto.Rows.Count; ic++)
                        {
                            string producto_clave = CatalogodeProducto.Rows[ic]["Prod_Clave"].ToString().Trim();
                            bool esta = codigoetiqueta.Text.Trim().Contains(producto_clave);

                            if (esta)
                            {
                                V_Prd = producto_clave;
                                break;
                            }
                        }



                        int posprod = codigoetiqueta.Text.Trim().IndexOf(V_Prd);
                        V_Recibo = codigoetiqueta.Text.Trim().Substring(0, posprod).Trim();

                        string restocaptura = codigoetiqueta.Text.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                        string subcadena = "";
                        if (restocaptura.Length == 6)
                        {
                            Mtipo = "PTC";
                            mtar = Convert.ToInt32(restocaptura.Substring(0, 3));
                            mtarf = Convert.ToInt32(restocaptura.Substring(3, 3));
                        }
                        else
                        {
                            Mtipo = "PTC";
                            //subcadena = restocaptura.Substring(0, 2);
                            //subcadena = subcadena.Replace(" ", "");
                            //mtar = Convert.ToInt32(subcadena);
                            //restocaptura=restocaptura.Substring(0, 3);
                            mtar = Convert.ToInt32(restocaptura);
                            //mtarf = Convert.ToInt32(restocaptura.Substring(2, 2));
                        }
                    }
                    else
                    {
                        L_Cad = V_Tamaño - 9;
                        Mtipo = "PTP";
                        mtar = Convert.ToInt32(codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3));
                        mtarf = mtar;
                        V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6); //no_lote
                        if (V_Recibo.Substring(0, 1) == "0")
                        {
                            Mtipo = "PTC";
                            V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                        }
                        V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad);
                    }
                }

                /*V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6); //no_lote
                if (V_Recibo.Substring(0, 1) == "0")
                {
                    Mtipo = "PTC";
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                }
                V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad); //UCase(Mid(txtCod.Text, 7, L_Cad))*/







                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        V_Tip = "FC"; ;
                        break;
                    case "Guadalajara":
                        V_Tip = "FG"; ;
                        break;
                    case "Distrito Federal":
                        V_Tip = "FD"; ;
                        break;
                    case "Externos":
                        V_Tip = "FE"; ;
                        break;
                    case "Puerto Vallarta":
                        V_Tip = "FV"; ;
                        break;
                    case "Cuautitlan":
                        V_Tip = "FM"; ;
                        break;
                    case "Exportación":
                        V_Tip = "EXP"; ;
                        break;
                    case "Nacional":
                        V_Tip = "NAL"; ;
                        break;
                    case "Maquila":
                        V_Tip = "TRA"; ;
                        break;
                }

                switch (V_Tip)
                {
                    case "EXP":
                        MNalExp = "EXP";
                        break;
                    case "NAL":
                        MNalExp = "NAL";
                        break;
                    case "TRA":
                        MNalExp = "TRA";
                        break;
                }
                if (V_Recibo.Substring(0, 1) == "0")
                {
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                    Mtipo = "PTC";
                }

                v_cajas = 0;
                v_dif = 0;
                V_Existe = "N";

                if (Mtipo == "PTP")
                {
                    cadena = "Select a.folio As RECIBO,b.prod_nombre AS prod_nombre,a.num_cajas As etiqueta, a.cajas_sur As SURTIDO,a.num_lote As fecha_cad From tb_det_eti_final A, tb_cat_producto B Where a.folio = '" + V_Recibo + "' and a.cve_prod = '" + V_Prd + "' and tarima = '" + mtar.ToString() + "' and a.cve_prod = b.prod_clave order by b.prod_nombre";
                }
                else
                {
                    cadena = "Select RECIBO,prod_nombre,etiqueta,SURTIDO,fecha_cad From tb_det_trazabilidad Where RECIBO = '" + V_Recibo + "' and prod_clave = '" + V_Prd + "' and tarima = '" + mtar.ToString() + "' order by prod_nombre";
                }

                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                SqlCommand cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                SqlDataReader Info;
                Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    V_Existe = "S";
                    v_dif = Convert.ToInt32(Info["etiqueta"].ToString().Trim()) - Convert.ToInt32(Info["SURTIDO"].ToString().Trim());
                    mactual = Convert.ToInt32(Info["SURTIDO"].ToString().Trim());

                    if (Mtipo == "PTP")
                    {
                        V_FecCad = Info["fecha_cad"].ToString().Trim();
                    }
                    else
                    {
                        if (Info["fecha_cad"].ToString().Trim().Length > 0)
                        {
                            V_FecCad = Info["fecha_cad"].ToString().Trim().Substring(0, 2) + DIATOMES(Info["fecha_cad"].ToString().Trim().Substring(3, 2));
                        }
                    }
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (V_Existe == "N")
                {
                    Toast.MakeText(this, "No Existe Folio " + V_Recibo + " Producto " + V_Prd + " y Tarima " + mtar.ToString() + " Verifique los Datos", ToastLength.Long).Show();
                    return;
                }
                //if (Convert.ToInt32(Cajas.Text) > v_dif)
                int cajasParse = 0;
                if (int.TryParse(Cajas.Text.Trim(), out cajasParse) == false)
                {
                    if (Convert.ToInt32(Cajas.Text) > v_dif)
                    {
                        Toast.MakeText(this, "La Cantidad Solicitada es Mayor a la Existencia de la Tarima Existencia: " + v_dif.ToString() + "    Solicitadas: " + Cajas.Text, ToastLength.Long).Show();
                        Cajas.Text = v_dif.ToString();
                        return;
                    }
                    Toast.MakeText(this, "La Cantidad Solicitada es Mayor a la Existencia de la Tarima Existencia: " + v_dif.ToString() + "    Solicitadas: " + Cajas.Text, ToastLength.Long).Show();
                    Cajas.Text = v_dif.ToString();
                    return;
                }

                if (tipotarima.Length == 0)
                {
                    Toast.MakeText(this, "Error no se ha capturado El tipo de Tarima", ToastLength.Long).Show();
                    TipoTar.RequestFocus();
                    return;
                }


                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                cadena = "Select Id_Tarima, Pdn_Folio, Posicion, Pdn_Fecha From Tb_Det_Tar Where Posicion = '" + Posicion.Text + "' and No_Trailer = '" + Notrailer.Text + "' and pdn_fecha = '" + fecha.Text + "'";
                cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                Info = cmd.ExecuteReader();
                string hay = "N";
                while (Info.Read())
                {
                    hay = "S";
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                cadena = "Select Id_Tarima, Nom_Tarima From Tb_Cat_Tarima Where Nom_Tarima = '" + tipotarima.Trim() + "'";
                cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                Info = cmd.ExecuteReader();
                string CveTar = "";
                while (Info.Read())
                {
                    CveTar = Info["Id_Tarima"].ToString().Trim();
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }


                if (ValidarProductoEnPedido(V_Prd, V_Tip) != "")
                {

                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    string szproductonopedido = "Insert into tb_ped_embarque (emb_folio, prod_clave, emb_tipo, cant_ped, cant_sur, nom_prod, nalexp, adicional) " +
                                        " Values " +
                                        "('" + pedido.Text.Trim() + "', '" + V_Prd + "', '" + V_Tip + "','0', '" + Cajas.Text + "', ' ', '" + V_Tip + "','S')";
                    SqlCommand cmdproductonoenpedido = new SqlCommand(szproductonopedido, thisConnection);
                    cmdproductonoenpedido.ExecuteNonQuery();
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                }



                string szSQL = "";
                if (ALTA == "A")
                {
                    szSQL = "Insert into tb_det_embarque (emb_folio, prod_clave, no_lote, cajas, seccion, temp, emb_tipo, tarima, tarima_f, tipo_rec, estatus,FEC_CAD,FECHACAD,FECHACAP,OPCAP,ID_TARIMA,RECIBO, id_lectora, datecaptura, latitud, longitud) Values ('" + pedido.Text + "', '" + V_Prd + "', '" + codigoetiqueta.Text.Trim() + "', " + Cajas.Text + ", " + Posicion.Text + ", '" + temperatura.Text + "', '" + V_Tip + "', '" + mtar + "', '" + mtarf + "', '" + Mtipo + "', 'A','" + V_FecCad + "','" + V_FecCad + "','" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "','N','" + CveTar + "','" + V_Recibo + "', '" + imei + "', GETDATE(), '" + latitud + "', '" + longitud + "')";
                }
                else
                {
                    mactual = mactual + Convert.ToInt32(Cajas.Text);
                    szSQL = "UPDATE tb_det_embarque SET cajas = '" + Cajas.Text + "' ,seccion = '" + Posicion.Text + "', temp = '" + temperatura.Text + "' WHERE emb_folio = '" + Posicion.Text + "' and prod_clave = '" + V_Prd + "' and recibo = '" + V_Recibo + "' and tarima = '" + mtar + "'";
                }

                if (hay == "N")
                {
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    string Cadena = "Insert into Tb_Det_Tar (Id_Tarima, Pdn_Folio, Posicion, Pdn_Fecha, No_Trailer, FechaCap, OPCAP)  Values ('" + CveTar + "', '" + pedido.Text + "', '" + Posicion.Text + "',' " + fecha.Text + "','" + Notrailer.Text + "','" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "','N')";
                    SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
                    cmdx.ExecuteNonQuery();
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }


                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                SqlCommand cmdsz = new SqlCommand(szSQL, thisConnection);
                cmdsz.ExecuteNonQuery();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (Mtipo == "PTP")
                {
                    szSQL = "UPDATE tb_det_eti_final SET cajas_sur = CAJAS_SUR + " + Convert.ToInt32(Cajas.Text.ToString()) + "Where folio = '" + V_Recibo + "' and cve_prod = '" + V_Prd + "' and tarima = '" + mtar.ToString() + "'";

                }
                else
                {
                    mactual = mactual + Convert.ToInt32(Cajas.Text);
                    szSQL = "UPDATE tb_det_trazabilidad SET SURTIDO = SURTIDO + " + Cajas.Text.ToString() + "Where RECIBO = '" + V_Recibo + "' and prod_clave = '" + V_Prd + "' and tarima = '" + mtar.ToString() + "'";
                }

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                cmdsz = new SqlCommand(szSQL, thisConnection);
                cmdsz.ExecuteNonQuery();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                string consultamensajes = "insert into Tb_Etiqueta_Mensajes_Validar(Fecha, emb_folio, titulo, mensaje, split, veces)Values(GETDATE(), '" + pedido.Text + "','CONSULTA " + imei + "','" + szSQL.Replace("'", "*") + "','" + mactual.ToString() + "', '" + (mactual + Convert.ToInt32(Cajas.Text)) + "')";
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                cmdsz = new SqlCommand(consultamensajes, thisConnection);
                cmdsz.ExecuteNonQuery();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (ConsultaInserFolioAdelantado.Length > 0)
                {
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    cmdsz = new SqlCommand(ConsultaInserFolioAdelantado, thisConnection);
                    cmdsz.ExecuteNonQuery();
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }

                Cajas.Text = "";
                tipotarima = "";
                temperatura.Text = "";
                Posicion.Text = "";
                codigoetiqueta.Text = "";
                confirmprod.Text = "";
                Cajas.Enabled = false;
                //fotoevent.Visibility = Android.Views.ViewStates.Invisible;
                fotoevent.Enabled = false;
                TipoTar.SetSelection(0);
                TipoTar.Enabled = false;
                temperatura.Enabled = false;
                Posicion.Enabled = false;
                codigoetiqueta.Enabled = true;
                confirmprod.Enabled = false;
                ConsultaInserFolioAdelantado = "";
                codigoetiqueta.RequestFocus();
                /*InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);*/
                TRAE_PESO();

            }
        }

        private string traenom(string mcod)
        {
            string prodnombre = "";
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cadena = "SELECT prod_nombre FROM tb_cat_producto WHERE prod_clave = '" + mcod + "'";
            SqlCommand cmd = new SqlCommand(cadena, thisConnection);
            prodnombre = Convert.ToString(cmd.ExecuteScalar());
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            return prodnombre;
        }

        #region CANTIDAD DISPONIBLE POR TARIMA 
        #region LEGACY
        private void CantidadDisponibleTarimaOG()
        {
            string V_Recibo = "", V_Prd = "", V_Tip = "", Mtipo = "", MNalExp, V_Existe, V_FecCad, id_pallet;
            int L_Cad, V_Tamaño, mtar = 0, mtarf, v_cajas, mactual;
            V_Tamaño = codigoetiqueta.Text.Trim().Length;



            if (V_Tamaño == 11)
            {
                V_Recibo = codigoetiqueta.Text.Trim();
                V_Prd = "";
            }
            else
            {
                if (codigoetiqueta.Text.Trim().Length == 12)
                {
                    string pti_famous = codigoetiqueta.Text.Trim();
                    if (codigoetiqueta.Text.StartsWith("0"))
                    {
                        pti_famous = codigoetiqueta.Text.TrimStart('0');
                    }
                    //string pti_famous = Regex.Replace(codigoetiqueta.Text, patron, "");

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_famous='" + pti_famous + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if (codigoetiqueta.Text.Contains(SerialShippingContainerCode) == true)
                {
                    Match match = Regex.Match(codigoetiqueta.Text, patron);
                    id_pallet = match.Groups[1].Value;

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where id_Pallet='" + id_pallet + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if (!Regex.IsMatch(codigoetiqueta.Text, @"\s"))
                {
                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_clave='" + codigoetiqueta.Text.Trim() + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else
                {
                    if (codigoetiqueta.Text.Trim().Contains(" ") == true)
                    {
                        DataTable CatalogodeProducto = new DataTable();
                        if (thisConnection.State == ConnectionState.Closed)
                        {
                            thisConnection.Open();
                        }
                        string cade = "Select prod_clave,prod_nombre from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC')  order by LEN(prod_clave) DESC";
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
                            bool esta = codigoetiqueta.Text.Trim().Contains(producto_clave);

                            if (esta)
                            {
                                V_Prd = producto_clave;
                                break;
                            }
                        }



                        int posprod = codigoetiqueta.Text.Trim().IndexOf(V_Prd);
                        V_Recibo = codigoetiqueta.Text.Trim().Substring(0, posprod).Trim();

                        string restocaptura = codigoetiqueta.Text.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                        if (restocaptura.Length == 6)
                        {
                            Mtipo = "PTC";
                            mtar = Convert.ToInt32(restocaptura.Substring(0, 3));
                        }
                        else
                        {
                            Mtipo = "PTC";
                            //mcaj = restocaptura.Substring(4, 3);
                            //mtar = Convert.ToInt32(restocaptura.Substring(0, 2));
                            mtar = Convert.ToInt32(restocaptura.Trim());
                        }
                    }
                    else
                    {
                        L_Cad = V_Tamaño - 9;
                        mtar = Convert.ToInt32(codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3));
                        mtar = Convert.ToInt32(codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3));
                        mtarf = 0;
                        Mtipo = "PTP";
                        V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6);
                        if (codigoetiqueta.Text.Trim().Substring(0, 1) == "0")
                        {
                            Mtipo = "PTC";
                            V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                        }
                        V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad);
                    }
                }


                //V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6);
                /*if (codigoetiqueta.Text.Trim().Substring(0, 1) == "0")
                {
                    Mtipo = "PTC";
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                }
                V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad);*/
                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        V_Tip = "FC";
                        break;
                    case "Guadalajara":
                        V_Tip = "FG";
                        break;
                    case "Distrito Federal":
                        V_Tip = "FD";
                        break;
                    case "Externos":
                        V_Tip = "FE";
                        break;
                    case "Puerto Vallarta":
                        V_Tip = "FV";
                        break;
                    case "Cuautitlan":
                        V_Tip = "FM";
                        break;
                    case "Exportación":
                        V_Tip = "EXP";
                        break;
                    case "Nacional":
                        V_Tip = "NAL";
                        break;
                    case "Maquila":
                        V_Tip = "TRA";
                        break;
                }

                switch (V_Tip)
                {
                    case "EXP":
                        MNalExp = "EXP";
                        break;
                    case "NAL":
                        MNalExp = "NAL";
                        break;
                    case "TRA":
                        MNalExp = "TRA";
                        break;
                }
                if (V_Recibo.Substring(0, 2) == "00")
                {
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                    Mtipo = "PTC";
                }
                v_cajas = 0;
                v_dif = 0;
                V_Existe = "N";
                V_FecCad = "";
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                if (Mtipo == "PTP")
                {
                    cadena = "Select a.folio,b.prod_nombre,a.num_cajas As etiqueta, a.cajas_sur AS SURTIDO,a.num_lote From tb_det_eti_final A, tb_cat_producto B Where a.folio = '" + V_Recibo + "' and a.cve_prod = '" + V_Prd + "' and tarima = '" + mtar + "' and a.cve_prod = b.prod_clave order by b.prod_nombre";
                }
                else
                {
                    cadena = "Select RECIBO,prod_nombre,etiqueta,SURTIDO,fecha_cad From tb_det_trazabilidad Where RECIBO = '" + Convert.ToInt32(V_Recibo) + "' and prod_clave = '" + V_Prd + "' and tarima = '" + mtar + "' order by prod_nombre";
                }

                SqlCommand cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                SqlDataReader Info;
                Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    V_Existe = "S";
                    v_dif = Convert.ToInt32(Info["etiqueta"].ToString()) - Convert.ToInt32(Info["SURTIDO"].ToString());
                    mactual = Convert.ToInt32(Info["SURTIDO"].ToString());
                    if (Mtipo == "PTP")
                    {
                        V_FecCad = Info["num_lote"].ToString().Trim();
                    }
                    else
                    {
                        if (Convert.ToString(Info["fecha_cad"]).Trim().Length > 0)
                        {
                            V_FecCad = Convert.ToString(Info["fecha_cad"]).Substring(0, 2) + DIATOMES(Convert.ToString(Info["fecha_cad"]).Substring(3, 2));
                        }
                    }
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (V_Existe == "N")
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Folio No Existe</font>"));
                    alertDialog.SetIcon(Resource.Drawable.no);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>No Existe Folio " + V_Recibo + " Producto " + V_Prd + " y Tarima " + mtar + " Verifique los Datos</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        Segundos = 0;
                        Timer1.Enabled = false;
                        codigoetiqueta.Text = "";
                        confirmprod.Text = "";
                    });
                    alertDialog.Show();
                    return;
                }



                Cajas.Text = v_dif.ToString();
                Cajas.Enabled = true;
                //fotoevent.Visibility = Android.Views.ViewStates.Invisible;
                fotoevent.Enabled = false;
            }



        }
        private void CantidadDisponibleTarimaS()
        {
            string V_Recibo = "", V_Prd = "", V_Tip = "", Mtipo = "", MNalExp, V_Existe, V_FecCad, id_pallet;
            int L_Cad, V_Tamaño, mtar = 0, mtarf, v_cajas, mactual;
            V_Tamaño = codigoetiqueta.Text.Trim().Length;



            if (V_Tamaño == 11)
            {
                V_Recibo = codigoetiqueta.Text.Trim();
                V_Prd = "";
            }
            else
            {
                var infoEtiqueta = ProcesarEtiqueta(codigoetiqueta.Text.Trim());
                if (infoEtiqueta != null)
                {
                    Mtipo = infoEtiqueta.Value.Tipo;
                    V_Recibo = infoEtiqueta.Value.Recibo;
                    V_Prd = infoEtiqueta.Value.ProdClave;
                    mtar = Convert.ToInt32(infoEtiqueta.Value.Tarima);
                    #region VALIDA LECTURA DE PTI FAMOUS
                    if ((V_Recibo == "" || V_Prd == "" || Mtipo == "") && codigoetiqueta.Text.Trim().Length == 12)
                    {
                        string pti_famous = codigoetiqueta.Text.Trim();
                        if (codigoetiqueta.Text.StartsWith("0"))
                        {
                            pti_famous = codigoetiqueta.Text.TrimStart('0');
                        }
                        //string pti_famous = Regex.Replace(codigoetiqueta.Text, patron, "");

                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        string querySSCC = "select*from tb_det_trazabilidad where pti_famous='" + pti_famous + "'";
                        SqlCommand sqlCommand = new SqlCommand(querySSCC);
                        sqlCommand.Connection = thisConnection;
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                            mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                            V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                            Mtipo = sqlDataReader["tipo"].ToString().Trim();
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                    }
                    #endregion
                    #region VALIDA LECTURA DE SERIAL SHIPPING CONTAINER CODE
                    else if ((V_Recibo == "" || V_Prd == "" || Mtipo == "") && codigoetiqueta.Text.Contains(SerialShippingContainerCode) == true)
                    {
                        Match match = Regex.Match(codigoetiqueta.Text, patron);
                        id_pallet = match.Groups[1].Value;

                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        string querySSCC = "select*from tb_det_trazabilidad where id_Pallet='" + id_pallet + "'";
                        SqlCommand sqlCommand = new SqlCommand(querySSCC);
                        sqlCommand.Connection = thisConnection;
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                            mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                            V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                            Mtipo = sqlDataReader["tipo"].ToString().Trim();
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                    }
                    #endregion
                    #region VALIDA LECTURA DE PTI CLAVE
                    else if ((V_Recibo == "" || V_Prd == "" || Mtipo == "") && !Regex.IsMatch(codigoetiqueta.Text, @"\s"))
                    {
                        #region VALIDAR ETIQUETA NUEVA
                        var datos = ValidarEtiquetaVerde(codigoetiqueta.Text.Trim());
                        if (datos != null)
                        {
                            Mtipo = datos.Value.Tipo;
                            V_Recibo = datos.Value.Recibo;
                            V_Prd = datos.Value.ProdClave;
                            mtar = Convert.ToInt32(datos.Value.Tarima);
                        }
                        #endregion

                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        string querySSCC = "select*from tb_det_trazabilidad where pti_clave='" + codigoetiqueta.Text.Trim() + "'";
                        SqlCommand sqlCommand = new SqlCommand(querySSCC);
                        sqlCommand.Connection = thisConnection;
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                            mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                            V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                            Mtipo = sqlDataReader["tipo"].ToString().Trim();
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                    }
                    #endregion
                    else
                    {
                        if (codigoetiqueta.Text.Trim().Contains(" ") == true)
                        {
                            DataTable CatalogodeProducto = new DataTable();
                            if (thisConnection.State == ConnectionState.Closed)
                            {
                                thisConnection.Open();
                            }
                            string cade = "Select prod_clave,prod_nombre from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC')  order by LEN(prod_clave) DESC";
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
                                bool esta = codigoetiqueta.Text.Trim().Contains(producto_clave);

                                if (esta)
                                {
                                    V_Prd = producto_clave;
                                    break;
                                }
                            }



                            int posprod = codigoetiqueta.Text.Trim().IndexOf(V_Prd);
                            V_Recibo = codigoetiqueta.Text.Trim().Substring(0, posprod).Trim();

                            string restocaptura = codigoetiqueta.Text.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                            if (restocaptura.Length == 6)
                            {
                                Mtipo = "PTC";
                                mtar = Convert.ToInt32(restocaptura.Substring(0, 3));
                            }
                            else
                            {
                                Mtipo = "PTC";
                                //mcaj = restocaptura.Substring(4, 3);
                                //mtar = Convert.ToInt32(restocaptura.Substring(0, 2));
                                mtar = Convert.ToInt32(restocaptura.Trim());
                            }
                        }
                        else
                        {
                            L_Cad = V_Tamaño - 9;
                            mtar = Convert.ToInt32(codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3));
                            mtar = Convert.ToInt32(codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3));
                            mtarf = 0;
                            Mtipo = "PTP";
                            V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6);
                            if (codigoetiqueta.Text.Trim().Substring(0, 1) == "0")
                            {
                                Mtipo = "PTC";
                                V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                            }
                            V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad);
                        }
                    }
                }


                //V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6);
                /*if (codigoetiqueta.Text.Trim().Substring(0, 1) == "0")
                {
                    Mtipo = "PTC";
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                }
                V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad);*/
                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        V_Tip = "FC";
                        break;
                    case "Guadalajara":
                        V_Tip = "FG";
                        break;
                    case "Distrito Federal":
                        V_Tip = "FD";
                        break;
                    case "Externos":
                        V_Tip = "FE";
                        break;
                    case "Puerto Vallarta":
                        V_Tip = "FV";
                        break;
                    case "Cuautitlan":
                        V_Tip = "FM";
                        break;
                    case "Exportación":
                        V_Tip = "EXP";
                        break;
                    case "Nacional":
                        V_Tip = "NAL";
                        break;
                    case "Maquila":
                        V_Tip = "TRA";
                        break;
                }

                switch (V_Tip)
                {
                    case "EXP":
                        MNalExp = "EXP";
                        break;
                    case "NAL":
                        MNalExp = "NAL";
                        break;
                    case "TRA":
                        MNalExp = "TRA";
                        break;
                }
                if (V_Recibo.Substring(0, 2) == "00")
                {
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                    Mtipo = "PTC";
                }
                v_cajas = 0;
                v_dif = 0;
                V_Existe = "N";
                V_FecCad = "";
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                if (Mtipo == "PTP")
                {
                    cadena = "Select a.folio,b.prod_nombre,a.num_cajas As etiqueta, a.cajas_sur AS SURTIDO,a.num_lote From tb_det_eti_final A, tb_cat_producto B Where a.folio = '" + V_Recibo + "' and a.cve_prod = '" + V_Prd + "' and tarima = '" + mtar + "' and a.cve_prod = b.prod_clave order by b.prod_nombre";
                }
                else
                {
                    cadena = "Select RECIBO,prod_nombre,etiqueta,SURTIDO,fecha_cad From tb_det_trazabilidad Where RECIBO = '" + Convert.ToInt32(V_Recibo) + "' and prod_clave = '" + V_Prd + "' and tarima = '" + mtar + "' order by prod_nombre";
                }

                SqlCommand cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                SqlDataReader Info;
                Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    V_Existe = "S";
                    v_dif = Convert.ToInt32(Info["etiqueta"].ToString()) - Convert.ToInt32(Info["SURTIDO"].ToString());
                    mactual = Convert.ToInt32(Info["SURTIDO"].ToString());
                    if (Mtipo == "PTP")
                    {
                        V_FecCad = Info["num_lote"].ToString().Trim();
                    }
                    else
                    {
                        if (Convert.ToString(Info["fecha_cad"]).Trim().Length > 0)
                        {
                            V_FecCad = Convert.ToString(Info["fecha_cad"]).Substring(0, 2) + DIATOMES(Convert.ToString(Info["fecha_cad"]).Substring(3, 2));
                        }
                    }
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (V_Existe == "N")
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Folio No Existe</font>"));
                    alertDialog.SetIcon(Resource.Drawable.no);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>No Existe Folio " + V_Recibo + " Producto " + V_Prd + " y Tarima " + mtar + " Verifique los Datos</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        Segundos = 0;
                        Timer1.Enabled = false;
                        codigoetiqueta.Text = "";
                        confirmprod.Text = "";
                    });
                    alertDialog.Show();
                    return;
                }

                Cajas.Text = v_dif.ToString();
                Cajas.Enabled = true;
                //fotoevent.Visibility = Android.Views.ViewStates.Invisible;
                fotoevent.Enabled = false;
            }
        }
        #endregion

        #region REFACTORIZADO
        /// <summary>
        /// Refactorización del método CantidadDisponibleTarima
        /// IMPORTANTE: Se mantienen los nombres originales de todas las variables por ser código legacy
        /// </summary>
        private void CantidadDisponibleTarima()
        {
            string V_Recibo = "", V_Prd = "", V_Tip = "", Mtipo = "", MNalExp = "", V_Existe = "N", V_FecCad = "", id_pallet = "";
            int L_Cad = 0, V_Tamaño = 0, mtar = 0, mtarf = 0, v_cajas = 0, mactual = 0;
            V_Tamaño = codigoetiqueta.Text.Trim().Length;

            if (V_Tamaño == 11)
            {
                V_Recibo = codigoetiqueta.Text.Trim();
                V_Prd = "";
            }
            else
            {
                if (codigoetiqueta.Text.Trim().Length == 12)
                {
                    string pti_famous = codigoetiqueta.Text.Trim();
                    if (codigoetiqueta.Text.StartsWith("0"))
                    {
                        pti_famous = codigoetiqueta.Text.TrimStart('0');
                    }

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_famous='" + pti_famous + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if (codigoetiqueta.Text.Contains(SerialShippingContainerCode) == true)
                {
                    Match match = Regex.Match(codigoetiqueta.Text, patron);
                    id_pallet = match.Groups[1].Value;

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where id_Pallet='" + id_pallet + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else if (!Regex.IsMatch(codigoetiqueta.Text, @"\s"))
                {
                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    string querySSCC = "select*from tb_det_trazabilidad where pti_clave='" + codigoetiqueta.Text.Trim() + "'";
                    SqlCommand sqlCommand = new SqlCommand(querySSCC);
                    sqlCommand.Connection = thisConnection;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                        mtar = int.Parse(sqlDataReader["tarima"].ToString().Trim());
                        V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                        Mtipo = sqlDataReader["tipo"].ToString().Trim();
                    }
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                }
                else
                {
                    if (codigoetiqueta.Text.Trim().Contains(" ") == true)
                    {
                        DataTable CatalogodeProducto = new DataTable();
                        if (thisConnection.State == ConnectionState.Closed)
                        {
                            thisConnection.Open();
                        }
                        string cade = "Select prod_clave,prod_nombre from tb_cat_producto where estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC')  order by LEN(prod_clave) DESC";
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
                            bool esta = codigoetiqueta.Text.Trim().Contains(producto_clave);

                            if (esta)
                            {
                                V_Prd = producto_clave;
                                break;
                            }
                        }

                        int posprod = codigoetiqueta.Text.Trim().IndexOf(V_Prd);
                        V_Recibo = codigoetiqueta.Text.Trim().Substring(0, posprod).Trim();

                        string restocaptura = codigoetiqueta.Text.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                        if (restocaptura.Length == 6)
                        {
                            Mtipo = "PTC";
                            mtar = Convert.ToInt32(restocaptura.Substring(0, 3));
                        }
                        else
                        {
                            Mtipo = "PTC";
                            mtar = Convert.ToInt32(restocaptura.Trim());
                        }
                    }
                    else
                    {
                        L_Cad = V_Tamaño - 9;
                        mtar = Convert.ToInt32(codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3));
                        mtar = Convert.ToInt32(codigoetiqueta.Text.Trim().Substring(V_Tamaño - 3, 3));
                        mtarf = 0;
                        Mtipo = "PTP";
                        V_Recibo = codigoetiqueta.Text.Trim().Substring(0, 6);
                        if (codigoetiqueta.Text.Trim().Substring(0, 1) == "0")
                        {
                            Mtipo = "PTC";
                            V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                        }
                        V_Prd = codigoetiqueta.Text.Trim().Substring(6, L_Cad);
                    }
                }

                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        V_Tip = "FC";
                        break;
                    case "Guadalajara":
                        V_Tip = "FG";
                        break;
                    case "Distrito Federal":
                        V_Tip = "FD";
                        break;
                    case "Externos":
                        V_Tip = "FE";
                        break;
                    case "Puerto Vallarta":
                        V_Tip = "FV";
                        break;
                    case "Cuautitlan":
                        V_Tip = "FM";
                        break;
                    case "Exportación":
                        V_Tip = "EXP";
                        break;
                    case "Nacional":
                        V_Tip = "NAL";
                        break;
                    case "Maquila":
                        V_Tip = "TRA";
                        break;
                }

                switch (V_Tip)
                {
                    case "EXP":
                        MNalExp = "EXP";
                        break;
                    case "NAL":
                        MNalExp = "NAL";
                        break;
                    case "TRA":
                        MNalExp = "TRA";
                        break;
                }

                if (V_Recibo.Substring(0, 2) == "00")
                {
                    V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                    Mtipo = "PTC";
                }

                v_cajas = 0;
                v_dif = 0;
                V_Existe = "N";
                V_FecCad = "";

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }

                if (Mtipo == "PTP")
                {
                    cadena = "Select a.folio,b.prod_nombre,a.num_cajas As etiqueta, a.cajas_sur AS SURTIDO,a.num_lote From tb_det_eti_final A, tb_cat_producto B Where a.folio = '" + V_Recibo + "' and a.cve_prod = '" + V_Prd + "' and tarima = '" + mtar + "' and a.cve_prod = b.prod_clave order by b.prod_nombre";
                }
                else
                {
                    cadena = "Select RECIBO,prod_nombre,etiqueta,SURTIDO,fecha_cad From tb_det_trazabilidad Where RECIBO = '" + Convert.ToInt32(V_Recibo) + "' and prod_clave = '" + V_Prd + "' and tarima = '" + mtar + "' order by prod_nombre";
                }

                SqlCommand cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                SqlDataReader Info;
                Info = cmd.ExecuteReader();

                while (Info.Read())
                {
                    V_Existe = "S";
                    v_dif = Convert.ToInt32(Info["etiqueta"].ToString()) - Convert.ToInt32(Info["SURTIDO"].ToString());
                    mactual = Convert.ToInt32(Info["SURTIDO"].ToString());

                    if (Mtipo == "PTP")
                    {
                        V_FecCad = Info["num_lote"].ToString().Trim();
                    }
                    else
                    {
                        if (Convert.ToString(Info["fecha_cad"]).Trim().Length > 0)
                        {
                            V_FecCad = Convert.ToString(Info["fecha_cad"]).Substring(0, 2) + DIATOMES(Convert.ToString(Info["fecha_cad"]).Substring(3, 2));
                        }
                    }
                }

                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                if (V_Existe == "N")
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Folio No Existe</font>"));
                    alertDialog.SetIcon(Resource.Drawable.no);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>No Existe Folio " + V_Recibo + " Producto " + V_Prd + " y Tarima " + mtar + " Verifique los Datos</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        Segundos = 0;
                        Timer1.Enabled = false;
                        codigoetiqueta.Text = "";
                        confirmprod.Text = "";
                    });
                    alertDialog.Show();
                    return;
                }

                Cajas.Text = v_dif.ToString();
                Cajas.Enabled = true;
                fotoevent.Enabled = false;
            }
        }

        #endregion
        #endregion



        private string DIATOMES(string MES)
        {
            string NonMes = "";

            switch (MES)
            {
                case "01":
                    NonMes = "ENE";
                    break;
                case "02":
                    NonMes = "FEB";
                    break;
                case "03":
                    NonMes = "MAR";
                    break;
                case "04":
                    NonMes = "ABR";
                    break;
                case "05":
                    NonMes = "MAY";
                    break;
                case "06":
                    NonMes = "JUN";
                    break;
                case "07":
                    NonMes = "JUL";
                    break;
                case "08":
                    NonMes = "AGO";
                    break;
                case "09":
                    NonMes = "SEP";
                    break;
                case "10":
                    NonMes = "OCT";
                    break;
                case "11":
                    NonMes = "NOV";
                    break;
                case "12":
                    NonMes = "DIC";
                    break;
            }

            return NonMes;
        }

        private void motivoautoriza_ItemSelected2(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            motivoautorizafechaadelantada = spinner.GetItemAtPosition(e.Position).ToString();
        }

        private void Borrar()
        {
            iniarCarga.Enabled = false;
            //iniarCarga.Visibility = ViewStates.Invisible;
            codigoetiqueta.Enabled = false;
            confirmprod.Enabled = false;
            Cajas.Enabled = false;
            //fotoevent.Visibility = Android.Views.ViewStates.Invisible;
            fotoevent.Enabled = false;
            temperatura.Enabled = false;
            Posicion.Enabled = false;
            TipoTar.Enabled = false;
            codigoetiqueta.Text = "";
            confirmprod.Text = "";
            codigoetiqueta.Text = "";
            Cajas.Text = "";
            temperatura.Text = "";
            Posicion.Text = "";
            codigoetiqueta.Enabled = true;
            codigoetiqueta.RequestFocus();
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);
        }

        #region METODOS UTILIZADOS PARA AUTORIZACIONES
        #region LEGACY
        /*
         * LoteAtrazado, sirve para verificar si un lote(o recibo) está atrasado en función de su fecha de caducidad,
         * y retorna información básica del primero que esté atrasado(si lo hay).
        */
        private string LoteAtrazadoLEGACY(string v_Prd, string mtipo, string mtar, string fecha_cad, int dias)
        {
            FolioAtrasado = "";
            FechaAtrasada = "";
            string recibo = "", mfec = "";
            TarimaAtrasada = "0";
            DateTime fechatar = DateTime.Now;
            DateTime datesactual = DateTime.Now;
            int contador = 0;

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            if (mtipo == "PTP")
            {
                datesactual = DateTime.ParseExact(fecha_cad.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                //cadena = "SELECT (num_cajas - cajas_sur) AS disponible, ISNULL(fechacad, FORMAT( DATEADD(day, " + dias + ", fecha), 'yyyyMMdd', 'en-US' )) AS fecha_cad, folio AS recibo, tarima, DATEDIFF(day, GETDATE(), fechacad) AS diasdisp FROM tb_det_eti_final Inner JOIN tb_mstr_ordenes_prod ON folio = ordp_folio WHERE num_cajas > 32 AND cve_prod = '" + v_Prd.ToString().Trim() + "' AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X Where X.Eti_Recibo = folio AND X.Eti_Producto  = cve_prod AND X.Eti_TarIni  = tarima AND X.Eti_Lectura LIKE 'PTP%' AND X.Estatus = 'A') = 0 AND estatus_sur != 'S' AND ordp_estatus != 'C' AND etiqueta = 'S' AND cajas_sur = 0 AND DATEDIFF(day, GETDATE(), fechacad) >= " + pdn_diasmin.ToString() + " Order By fecha_cad, recibo, tarima";
                cadena = "SELECT (num_cajas - cajas_sur) AS disponible, ISNULL(fechacad, FORMAT( DATEADD(day, " + dias + ", fecha), 'yyyyMMdd', 'en-US' )) AS fecha_cad, folio AS recibo, tarima, DATEDIFF(day, GETDATE(), fechacad) AS diasdisp FROM tb_det_eti_final Inner JOIN tb_mstr_ordenes_prod ON folio = ordp_folio WHERE (num_cajas - cajas_sur) > 0 AND (preautorizado = '' or preautorizado is null) AND cve_prod = '" + v_Prd.ToString().Trim() + "' AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X Where X.Eti_Recibo = folio AND X.Eti_Producto  = cve_prod AND X.Eti_TarIni  = tarima AND X.Eti_Lectura LIKE 'PTP%' AND X.Estatus = 'A') = 0 AND estatus_sur != 'S' AND ordp_estatus != 'C' AND etiqueta = 'S' Order By fecha_cad, recibo, tarima";
            }
            else
            {
                datesactual = DateTime.ParseExact(fecha_cad.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cadena = "SELECT  (etiqueta - surtido) AS disponible, (CASE fecha_cad WHEN '' THEN  FORMAT( DATEADD(day, " + dias + ", pti_fecha), 'dd/MM/yyyy', 'en-US' ) WHEN fecha_cad THEN fecha_cad END) AS fecha_cad, (CASE fecha_cad WHEN '' THEN  FORMAT( DATEADD(day, " + dias + ", pti_fecha), 'yyyyMMdd', 'en-US' ) WHEN fecha_cad THEN FORMAT(convert(datetime,fecha_cad), 'yyyyMMdd', 'en-US' ) END) AS fecha_cadu, recibo, tarima, DATEDIFF(day, GETDATE(), (CASE fecha_cad WHEN '' THEN  FORMAT( DATEADD(day, " + dias + ", pti_fecha), 'yyyyMMdd', 'en-US' ) WHEN fecha_cad THEN FORMAT(convert(datetime,fecha_cad), 'yyyyMMdd', 'en-US' ) END)) AS diasdisp  FROM TB_DET_TRAZABILIDAD Inner JOIN tb_mstr_recepcion_pt ON rpt_recibo = recibo WHERE (etiqueta - surtido) > 0 AND (preautorizado = '' or preautorizado is null) AND PROD_CLAVE = '" + v_Prd.ToString().Trim() + "' AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X Where X.Eti_Recibo = recibo AND X.Eti_Producto  = PROD_CLAVE AND X.Eti_TarIni  = tarima AND X.Eti_Lectura LIKE 'PTC%' AND X.Estatus = 'A') = 0 AND pti_estatus_sur = '' AND tipo = 'PTC' AND (rpt_tipo != 'TR' OR (rpt_tipo != 'TR' AND rpt_inventario = 'S')) AND rpt_estatus = ''  Order By fecha_cadu, recibo, tarima";
            }

            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {

                if (mtipo == "PTP")
                {
                    fechatar = DateTime.ParseExact(Convert.ToString(Info["fecha_cad"].ToString().ToString().Trim()), "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else
                {

                    fechatar = DateTime.ParseExact(Convert.ToString(Info["fecha_cad"].ToString().Trim()), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                if (DateTime.Compare(datesactual, fechatar) > 0 && (contador == 0))
                {
                    mfec = Convert.ToString(fechatar);
                    recibo = Convert.ToString(Info["recibo"]) + "," + mfec;
                    FolioAtrasado = Convert.ToString(Info["recibo"]);
                    CajasDisp = Convert.ToString(Info["disponible"]);
                    FechaAtrasada = fechatar.ToString("dd/MMM/yy");
                    TarimaAtrasada = Convert.ToString(Info["tarima"]);
                }

                contador = contador + 1;
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            return recibo;
        }
        private string LoteAtrazadoOG(string v_Prd, string mtipo, string mtar, string fecha_cad, int dias)
        {
            FolioAtrasado = "";
            FechaAtrasada = "";
            string recibo = "", mfec = "";
            TarimaAtrasada = "0";
            DateTime fechatar = DateTime.Now;          // Variable de ámbito del método
            DateTime datesactual = DateTime.Now;
            int contador = 0;

            // Validar y convertir la fecha de referencia (fecha_cad) que llega como parámetro
            if (string.IsNullOrWhiteSpace(fecha_cad))
            {
                // Si viene vacía, se toma la fecha actual (ajusta según tu lógica de negocio)
                datesactual = DateTime.Now;
            }
            else
            {
                try
                {
                    if (mtipo == "PTP")
                        datesactual = DateTime.ParseExact(fecha_cad.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                    else
                        datesactual = DateTime.ParseExact(fecha_cad.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    // Si falla el parseo, se usa la fecha actual para no interrumpir el proceso
                    datesactual = DateTime.Now;
                }
            }

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }

            string cadena;
            if (mtipo == "PTP")
            {
                // Para PTP: si fechacad es NULL o vacío, se usa fecha (elaboración) + 14 días
                cadena = @"
            SELECT 
                (num_cajas - cajas_sur) AS disponible,
                COALESCE(NULLIF(fechacad, ''), FORMAT(DATEADD(day, 14, fecha), 'yyyyMMdd', 'en-US')) AS fecha_cad,
                folio AS recibo,
                tarima,
                DATEDIFF(day, GETDATE(), COALESCE(NULLIF(fechacad, ''), FORMAT(DATEADD(day, 14, fecha), 'yyyyMMdd', 'en-US'))) AS diasdisp
            FROM tb_det_eti_final 
            INNER JOIN tb_mstr_ordenes_prod ON folio = ordp_folio 
            WHERE (num_cajas - cajas_sur) > 0 
                AND (preautorizado = '' or preautorizado is null) 
                AND cve_prod = @producto 
                AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X 
                     WHERE X.Eti_Recibo = folio 
                       AND X.Eti_Producto = cve_prod 
                       AND X.Eti_TarIni = tarima 
                       AND X.Eti_Lectura LIKE 'PTP%' 
                       AND X.Estatus = 'A') = 0 
                AND estatus_sur != 'S' 
                AND ordp_estatus != 'C' 
                AND etiqueta = 'S' 
            ORDER BY fecha_cad, recibo, tarima";
            }
            else
            {
                // Para PTC: si fecha_cad es NULL o vacío, se usa pti_fecha (elaboración) + 14 días
                cadena = @"
            SELECT  
                (etiqueta - surtido) AS disponible,
                CASE 
                    WHEN fecha_cad IS NULL OR fecha_cad = '' 
                    THEN FORMAT(DATEADD(day, 14, pti_fecha), 'dd/MM/yyyy', 'en-US')
                    ELSE fecha_cad 
                END AS fecha_cad,
                CASE 
                    WHEN fecha_cad IS NULL OR fecha_cad = '' 
                    THEN FORMAT(DATEADD(day, 14, pti_fecha), 'yyyyMMdd', 'en-US')
                    ELSE FORMAT(CONVERT(datetime, fecha_cad), 'yyyyMMdd', 'en-US')
                END AS fecha_cadu,
                recibo,
                tarima,
                DATEDIFF(day, GETDATE(), 
                    CASE 
                        WHEN fecha_cad IS NULL OR fecha_cad = '' 
                        THEN FORMAT(DATEADD(day, 14, pti_fecha), 'yyyyMMdd', 'en-US')
                        ELSE FORMAT(CONVERT(datetime, fecha_cad), 'yyyyMMdd', 'en-US')
                    END) AS diasdisp
            FROM TB_DET_TRAZABILIDAD 
            INNER JOIN tb_mstr_recepcion_pt ON rpt_recibo = recibo 
            WHERE (etiqueta - surtido) > 0 
                AND (preautorizado = '' or preautorizado is null) 
                AND PROD_CLAVE = @producto 
                AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X 
                     WHERE X.Eti_Recibo = recibo 
                       AND X.Eti_Producto = PROD_CLAVE 
                       AND X.Eti_TarIni = tarima 
                       AND X.Eti_Lectura LIKE 'PTC%' 
                       AND X.Estatus = 'A') = 0 
                AND pti_estatus_sur = '' 
                AND tipo = 'PTC' 
                AND (rpt_tipo != 'TR' OR (rpt_tipo != 'TR' AND rpt_inventario = 'S')) 
                AND rpt_estatus = ''  
            ORDER BY fecha_cadu, recibo, tarima";
            }

            SqlCommand cmd = new SqlCommand(cadena, thisConnection);
            cmd.Parameters.AddWithValue("@producto", v_Prd.Trim());

            SqlDataReader Info = null;
            try
            {
                Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    string fechaCadStr = Convert.ToString(Info["fecha_cad"]).Trim();
                    DateTime tempDate;   // Variable temporal para el parseo
                    bool parseOk;

                    if (mtipo == "PTP")
                    {
                        parseOk = DateTime.TryParseExact(fechaCadStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                    }
                    else
                    {
                        parseOk = DateTime.TryParseExact(fechaCadStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                    }

                    if (!parseOk)
                    {
                        // No se pudo interpretar la fecha; se omite este lote (no se considera atrasado)
                        continue;
                    }

                    // Asignar la fecha parseada a la variable del método
                    fechatar = tempDate;

                    // Si es el primer registro que cumple la condición de atraso (datesactual > fechatar), lo capturamos
                    if (DateTime.Compare(datesactual, fechatar) > 0 && contador == 0)
                    {
                        mfec = fechatar.ToString(); // o el formato que requieras
                        recibo = Convert.ToString(Info["recibo"]) + "," + mfec;
                        FolioAtrasado = Convert.ToString(Info["recibo"]);
                        CajasDisp = Convert.ToString(Info["disponible"]);
                        FechaAtrasada = fechatar.ToString("dd/MMM/yy");
                        TarimaAtrasada = Convert.ToString(Info["tarima"]);
                    }

                    contador++;
                }
            }
            finally
            {
                if (Info != null)
                    Info.Close();
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }

            return recibo;
        }
        #endregion

        #region REFACTORIZADO
        /// <summary>
        /// Busca el primer lote atrasado para un producto específico
        /// </summary>
        /// <param name="v_Prd">Código del producto</param>
        /// <param name="mtipo">Tipo de producto (PTP o PTC)</param>
        /// <param name="mtar">Número de tarima</param>
        /// <param name="fecha_cad">Fecha de caducidad de referencia</param>
        /// <param name="dias">Días de tolerancia</param>
        /// <returns>Recibo + fecha en formato original</returns>
        private string LoteAtrazado(string v_Prd, string mtipo, string mtar, string fecha_cad, int dias)
        {
            FolioAtrasado = "";
            FechaAtrasada = "";
            string recibo = "", mfec = "";
            TarimaAtrasada = "0";
            DateTime fechatar = DateTime.Now;
            DateTime datesactual = DateTime.Now;
            int contador = 0;

            // Validar y convertir la fecha de referencia (fecha_cad) que llega como parámetro
            if (string.IsNullOrWhiteSpace(fecha_cad))
            {
                // Si viene vacía, se toma la fecha actual
                datesactual = DateTime.Now;
            }
            else
            {
                try
                {
                    if (mtipo == "PTP")
                        datesactual = DateTime.ParseExact(fecha_cad.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                    else
                        datesactual = DateTime.ParseExact(fecha_cad.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    // Si falla el parseo, se usa la fecha actual para no interrumpir el proceso
                    datesactual = DateTime.Now;
                }
            }

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }

            string cadena;
            if (mtipo == "PTP")
            {
                // Para PTP: si fechacad es NULL o vacío, se usa fecha (elaboración) + 14 días
                cadena = @"
    SELECT
        (num_cajas - cajas_sur) AS disponible,
        COALESCE(NULLIF(fechacad, ''), FORMAT(DATEADD(day, 14, fecha), 'yyyyMMdd', 'en-US')) AS fecha_cad,
        folio AS recibo,
        tarima,
        DATEDIFF(day, GETDATE(), COALESCE(NULLIF(fechacad, ''), FORMAT(DATEADD(day, 14, fecha), 'yyyyMMdd', 'en-US'))) AS diasdisp
    FROM tb_det_eti_final
    INNER JOIN tb_mstr_ordenes_prod ON folio = ordp_folio
    WHERE (num_cajas - cajas_sur) > 0
        AND (preautorizado = '' or preautorizado is null)
        AND cve_prod = @producto
        AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X
             WHERE X.Eti_Recibo = folio
               AND X.Eti_Producto = cve_prod
               AND X.Eti_TarIni = tarima
               AND X.Eti_Lectura LIKE 'PTP%'
               AND X.Estatus = 'A') = 0
        AND estatus_sur != 'S'
        AND ordp_estatus != 'C'
        AND etiqueta = 'S'
    ORDER BY fecha_cad, recibo, tarima";
            }
            else
            {
                // Para PTC: si fecha_cad es NULL o vacío, se usa pti_fecha (elaboración) + 14 días
                cadena = @"
    SELECT
        (etiqueta - surtido) AS disponible,
        CASE
            WHEN fecha_cad IS NULL OR fecha_cad = ''
            THEN FORMAT(DATEADD(day, 14, pti_fecha), 'dd/MM/yyyy', 'en-US')
            ELSE fecha_cad
        END AS fecha_cad,
        CASE
            WHEN fecha_cad IS NULL OR fecha_cad = ''
            THEN FORMAT(DATEADD(day, 14, pti_fecha), 'yyyyMMdd', 'en-US')
            ELSE FORMAT(CONVERT(datetime, fecha_cad), 'yyyyMMdd', 'en-US')
        END AS fecha_cadu,
        recibo,
        tarima,
        DATEDIFF(day, GETDATE(),
            CASE
                WHEN fecha_cad IS NULL OR fecha_cad = ''
                THEN FORMAT(DATEADD(day, 14, pti_fecha), 'yyyyMMdd', 'en-US')
                ELSE FORMAT(CONVERT(datetime, fecha_cad), 'yyyyMMdd', 'en-US')
            END) AS diasdisp
    FROM TB_DET_TRAZABILIDAD
    INNER JOIN tb_mstr_recepcion_pt ON rpt_recibo = recibo
    WHERE (etiqueta - surtido) > 0
        AND (preautorizado = '' or preautorizado is null)
        AND PROD_CLAVE = @producto
        AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X
             WHERE X.Eti_Recibo = recibo
               AND X.Eti_Producto = PROD_CLAVE
               AND X.Eti_TarIni = tarima
               AND X.Eti_Lectura LIKE 'PTC%'
               AND X.Estatus = 'A') = 0
        AND pti_estatus_sur = ''
        AND tipo = 'PTC'
        AND (rpt_tipo != 'TR' OR (rpt_tipo != 'TR' AND rpt_inventario = 'S'))
        AND rpt_estatus = ''
    ORDER BY fecha_cadu, recibo, tarima";
            }

            SqlCommand cmd = new SqlCommand(cadena, thisConnection);
            cmd.Parameters.AddWithValue("@producto", v_Prd.Trim());

            SqlDataReader Info = null;
            try
            {
                Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    string fechaCadStr = Convert.ToString(Info["fecha_cad"]).Trim();
                    DateTime tempDate;
                    bool parseOk;

                    if (mtipo == "PTP")
                    {
                        parseOk = DateTime.TryParseExact(fechaCadStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                    }
                    else
                    {
                        parseOk = DateTime.TryParseExact(fechaCadStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate);
                    }

                    if (!parseOk)
                    {
                        // No se pudo interpretar la fecha; se omite este lote (no se considera atrasado)
                        continue;
                    }

                    // Asignar la fecha parseada a la variable del método
                    fechatar = tempDate;

                    // Si es el primer registro que cumple la condición de atraso (datesactual > fechatar), lo capturamos
                    if (DateTime.Compare(datesactual, fechatar) > 0 && contador == 0)
                    {
                        mfec = fechatar.ToString(); // o el formato que requieras
                        recibo = Convert.ToString(Info["recibo"]) + "," + mfec;
                        FolioAtrasado = Convert.ToString(Info["recibo"]);
                        CajasDisp = Convert.ToString(Info["disponible"]);
                        FechaAtrasada = fechatar.ToString("dd/MMM/yy");
                        TarimaAtrasada = Convert.ToString(Info["tarima"]);
                    }

                    contador++;
                }
            }
            finally
            {
                if (Info != null)
                    Info.Close();
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }

            return recibo;
        }

        /// <summary>
        /// Parsea la fecha de caducidad de referencia según el tipo de producto
        /// </summary>
        private DateTime ParsearFechaReferencia(string fecha_cad, string mtipo)
        {
            DateTime datesactual = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(fecha_cad))
            {
                try
                {
                    string formatoFecha = (mtipo == "PTP") ? "yyyyMMdd" : "dd/MM/yyyy";
                    datesactual = DateTime.ParseExact(fecha_cad.Trim(), formatoFecha, CultureInfo.InvariantCulture);
                }
                catch
                {
                    // Si falla el parseo, usar fecha actual
                    datesactual = DateTime.Now;
                }
            }

            return datesactual;
        }

        /// <summary>
        /// Construye la consulta SQL para buscar lotes atrasados según el tipo de producto
        /// </summary>
        private string ConstruirConsultaLotesAtrasados(string v_Prd, string mtipo)
        {
            if (mtipo == "PTP")
            {
                // Consulta para productos PTP (Producto Terminado Principal)
                return @"
            SELECT
                (num_cajas - cajas_sur) AS disponible,
                COALESCE(NULLIF(fechacad, ''), FORMAT(DATEADD(day, 14, fecha), 'yyyyMMdd', 'en-US')) AS fecha_cad,
                folio AS recibo,
                tarima,
                DATEDIFF(day, GETDATE(), COALESCE(NULLIF(fechacad, ''), FORMAT(DATEADD(day, 14, fecha), 'yyyyMMdd', 'en-US'))) AS diasdisp
            FROM tb_det_eti_final
            INNER JOIN tb_mstr_ordenes_prod ON folio = ordp_folio
            WHERE (num_cajas - cajas_sur) > 0
                AND (preautorizado = '' or preautorizado is null)
                AND cve_prod = @producto
                AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X
                     WHERE X.Eti_Recibo = folio
                       AND X.Eti_Producto = cve_prod
                       AND X.Eti_TarIni = tarima
                       AND X.Eti_Lectura LIKE 'PTP%'
                       AND X.Estatus = 'A') = 0
                AND estatus_sur != 'S'
                AND ordp_estatus != 'C'
                AND etiqueta = 'S'
            ORDER BY fecha_cad, recibo, tarima";
            }
            else
            {
                // Consulta para productos PTC (Producto Terminado Comercial)
                return @"
            SELECT
                (etiqueta - surtido) AS disponible,
                CASE
                    WHEN fecha_cad IS NULL OR fecha_cad = ''
                    THEN FORMAT(DATEADD(day, 14, pti_fecha), 'dd/MM/yyyy', 'en-US')
                    ELSE fecha_cad
                END AS fecha_cad,
                CASE
                    WHEN fecha_cad IS NULL OR fecha_cad = ''
                    THEN FORMAT(DATEADD(day, 14, pti_fecha), 'yyyyMMdd', 'en-US')
                    ELSE FORMAT(CONVERT(datetime, fecha_cad), 'yyyyMMdd', 'en-US')
                END AS fecha_cadu,
                recibo,
                tarima,
                DATEDIFF(day, GETDATE(),
                    CASE
                        WHEN fecha_cad IS NULL OR fecha_cad = ''
                        THEN FORMAT(DATEADD(day, 14, pti_fecha), 'yyyyMMdd', 'en-US')
                        ELSE FORMAT(CONVERT(datetime, fecha_cad), 'yyyyMMdd', 'en-US')
                    END) AS diasdisp
            FROM TB_DET_TRAZABILIDAD
            INNER JOIN tb_mstr_recepcion_pt ON rpt_recibo = recibo
            WHERE (etiqueta - surtido) > 0
                AND (preautorizado = '' or preautorizado is null)
                AND PROD_CLAVE = @producto
                AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X
                     WHERE X.Eti_Recibo = recibo
                       AND X.Eti_Producto = PROD_CLAVE
                       AND X.Eti_TarIni = tarima
                       AND X.Eti_Lectura LIKE 'PTC%'
                       AND X.Estatus = 'A') = 0
                AND pti_estatus_sur = ''
                AND tipo = 'PTC'
                AND (rpt_tipo != 'TR' OR (rpt_tipo != 'TR' AND rpt_inventario = 'S'))
                AND rpt_estatus = ''
            ORDER BY fecha_cadu, recibo, tarima";
            }
        }

        #endregion
        /*
         * FolioAtrazado, tiene como propósito verificar si existe un folio anterior disponible para surtir producto 
         * antes de utilizar el folio actual. Es útil, por ejemplo, en un sistema de trazabilidad donde se prioriza 
         * el uso de producto más antiguo (FIFO).
         */
        private void FolioAtrazado(string v_Prd, string mtipo, string v_Recibo)
        {

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            if (mtipo == "PTP")
            {

                cadena = "SELECT folio AS recibo, count(tarima) as tarimas FROM tb_det_eti_final Inner JOIN tb_mstr_ordenes_prod ON folio = ordp_folio WHERE cve_prod = '" + v_Prd + "' AND estatus_sur != 'S' AND ordp_estatus != 'C' AND etiqueta = 'S' AND cajas_sur = 0 Group by folio Order By folio";
            }
            else
            {
                cadena = "SELECT recibo, COUNT(tarima) as TARIMAS FROM TB_DET_TRAZABILIDAD Inner JOIN tb_mstr_recepcion_pt ON rpt_recibo = recibo WHERE PROD_CLAVE = '" + v_Prd + "' AND pti_estatus_sur = '' AND tipo = 'PTC' AND (rpt_tipo != 'TR' OR (rpt_tipo != 'TR' AND rpt_inventario = 'S')) AND rpt_estatus = '' AND  surtido = 0 GROUP BY recibo Order By recibo";
            }

            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info;
            Info = cmd.ExecuteReader();
            string szSQLnew = "";
            while (Info.Read())
            {

                if (v_Recibo == Info["recibo"].ToString().ToString().Trim())
                {
                    break;
                }
                else
                {
                    szSQLnew = szSQLnew + "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) VALUES (GETDATE(),'" + imei + "','" + responsable.Trim() + "','FA','7.1','" + pedido.Text.Trim() + "','Folio No Consecutivo Detectado Folio Actual " + v_Recibo.Trim() + " Folio notificado: " + Convert.ToString(Info["recibo"]).Trim() + " Con " + Convert.ToString(Info["tarimas"]).Trim() + " Tarimas Disponibles','SIPGAB','" + pedido.Text.Trim() + "') ";

                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Folio Anterior Disponible</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>Existe Disponible El recibo " + Convert.ToString(Info["recibo"]).Trim() + " Con " + Convert.ToString(Info["tarimas"]).Trim() + " Tarimas Disponibles</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                    });
                    alertDialog.Show();
                    return;
                }

            }

            if (szSQLnew.Trim().Length > 0)
            {
                cmd = new SqlCommand(szSQLnew, thisConnection);
                cmd.ExecuteNonQuery();

            }

            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

        }
        #endregion

        #region METODOS UTILIZADOS PARA VIDA DE ANAQUEL DEL PRODUCTO POR CLIENTE
        private string LoteAtrazadoVAPP(string v_Prd, string mtipo, string mtar, string fecha_cad, int dias, int diasMinimos)
        {
            FolioAtrasado = "";
            FechaAtrasada = "";
            string recibo = "", mfec = "";
            TarimaAtrasada = "0";
            DateTime fechatar = DateTime.Now;
            DateTime datesactual = DateTime.Now;
            int contador = 0;

            //thisConnection.ConnectionString = cadenaConexion;

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            if (mtipo == "PTP")
            {
                datesactual = DateTime.ParseExact(fecha_cad.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
                //cadena = "SELECT (num_cajas - cajas_sur) AS disponible, ISNULL(fechacad, FORMAT( DATEADD(day, " + dias + ", fecha), 'yyyyMMdd', 'en-US' )) AS fecha_cad, folio AS recibo, tarima, DATEDIFF(day, GETDATE(), fechacad) AS diasdisp FROM tb_det_eti_final Inner JOIN tb_mstr_ordenes_prod ON folio = ordp_folio WHERE num_cajas > 32 AND cve_prod = '" + v_Prd.ToString().Trim() + "' AND (select COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X Where X.Eti_Recibo = folio AND X.Eti_Producto  = cve_prod AND X.Eti_TarIni  = tarima AND X.Eti_Lectura LIKE 'PTP%' AND X.Estatus = 'A') = 0 AND estatus_sur != 'S' AND ordp_estatus != 'C' AND etiqueta = 'S' AND cajas_sur = 0 AND DATEDIFF(day, GETDATE(), fechacad) >= " + pdn_diasmin.ToString() + " Order By fecha_cad, recibo, tarima";
                cadena = "SELECT (num_cajas - cajas_sur) AS disponible, ISNULL(fechacad, FORMAT(DATEADD(DAY, " + dias + ", fecha), 'yyyyMMdd', 'en-US')) AS fecha_cad, folio AS recibo, tarima, DATEDIFF(DAY, GETDATE(), fechacad) AS diasdisp FROM tb_det_eti_final INNER JOIN tb_mstr_ordenes_prod ON folio = ordp_folio WHERE (num_cajas - cajas_sur) > 0 AND (preautorizado = '' OR preautorizado IS NULL) AND cve_prod = '" + v_Prd.ToString().Trim() + "' AND (SELECT COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X WHERE X.Eti_Recibo = folio AND X.Eti_Producto = cve_prod AND X.Eti_TarIni = tarima AND X.Eti_Lectura LIKE 'PTP%' AND X.Estatus = 'A') = 0 AND estatus_sur != 'S' AND ordp_estatus != 'C' AND etiqueta = 'S' AND DATEDIFF(DAY, GETDATE(), ISNULL(fechacad, DATEADD(DAY, " + dias + ", fecha))) >= " + diasMinimos + " ORDER BY fecha_cad, recibo, tarima";
            }
            else
            {
                datesactual = DateTime.ParseExact(fecha_cad.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                cadena = "SELECT (etiqueta - surtido) AS disponible, (CASE fecha_cad WHEN '' THEN FORMAT(DATEADD(DAY, " + dias + ", pti_fecha), 'dd/MM/yyyy', 'en-US') WHEN fecha_cad THEN fecha_cad END) AS fecha_cad, (CASE fecha_cad WHEN '' THEN FORMAT(DATEADD(DAY, " + dias + ", pti_fecha), 'yyyyMMdd', 'en-US') WHEN fecha_cad THEN FORMAT(convert(datetime, fecha_cad), 'yyyyMMdd', 'en-US') END) AS fecha_cadu, recibo, tarima, DATEDIFF(DAY, GETDATE(), (CASE fecha_cad WHEN '' THEN FORMAT(DATEADD(DAY, " + dias + ", pti_fecha), 'yyyyMMdd', 'en-US') WHEN fecha_cad THEN FORMAT(convert(datetime, fecha_cad), 'yyyyMMdd', 'en-US') END)) AS diasdisp FROM TB_DET_TRAZABILIDAD INNER JOIN tb_mstr_recepcion_pt ON rpt_recibo = recibo WHERE (etiqueta - surtido) > 0 AND (preautorizado = '' OR preautorizado IS NULL) AND PROD_CLAVE = '" + v_Prd.ToString().Trim() + "' AND (SELECT COUNT(X.Eti_caja) FROM Tb_Det_Etiqueta_Presplit X WHERE X.Eti_Recibo = recibo AND X.Eti_Producto = PROD_CLAVE AND X.Eti_TarIni = tarima AND X.Eti_Lectura LIKE 'PTC%' AND X.Estatus = 'A') = 0 AND pti_estatus_sur = '' AND tipo = 'PTC' AND (rpt_tipo != 'TR' OR (rpt_tipo != 'TR' AND rpt_inventario = 'S')) AND rpt_estatus = '' AND DATEDIFF(DAY, GETDATE(), CASE fecha_cad WHEN '' THEN DATEADD(DAY, " + dias + ", pti_fecha) ELSE CONVERT(datetime, fecha_cad) END ) >= " + diasMinimos + " ORDER BY fecha_cadu, recibo, tarima";
            }

            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {

                if (mtipo == "PTP")
                {
                    fechatar = DateTime.ParseExact(Convert.ToString(Info["fecha_cad"].ToString().ToString().Trim()), "yyyyMMdd", CultureInfo.InvariantCulture);
                }
                else
                {

                    fechatar = DateTime.ParseExact(Convert.ToString(Info["fecha_cad"].ToString().Trim()), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                if (DateTime.Compare(datesactual, fechatar) > 0 && (contador == 0))
                {
                    mfec = Convert.ToString(fechatar);
                    recibo = Convert.ToString(Info["recibo"]) + "," + mfec;
                    FolioAtrasado = Convert.ToString(Info["recibo"]);
                    CajasDisp = Convert.ToString(Info["disponible"]);
                    FechaAtrasada = fechatar.ToString("dd/MMM/yy");
                    TarimaAtrasada = Convert.ToString(Info["tarima"]);
                }

                contador = contador + 1;
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            return recibo;
        }
        private void FolioAtrazadoVAPP(string v_Prd, string mtipo, string v_Recibo, int dias, int diasMinimos)
        {

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            if (mtipo == "PTP")
            {

                cadena = "SELECT folio AS recibo, count(tarima) AS tarimas FROM tb_det_eti_final INNER JOIN tb_mstr_ordenes_prod ON folio = ordp_folio WHERE cve_prod = '" + v_Prd + "' AND estatus_sur != 'S' AND ordp_estatus != 'C' AND etiqueta = 'S' AND cajas_sur = 0 AND DATEDIFF(DAY, GETDATE(), ISNULL(fechacad, DATEADD(DAY, " + dias + ", fecha))) >= " + diasMinimos + " GROUP BY folio ORDER BY folio";
            }
            else
            {
                cadena = "SELECT recibo, COUNT(tarima) AS TARIMAS FROM TB_DET_TRAZABILIDAD INNER JOIN tb_mstr_recepcion_pt ON rpt_recibo = recibo WHERE PROD_CLAVE = '" + v_Prd + "' AND pti_estatus_sur = '' AND tipo = 'PTC' AND (rpt_tipo != 'TR' OR (rpt_tipo != 'TR' AND rpt_inventario = 'S')) AND rpt_estatus = '' AND surtido = 0 AND DATEDIFF(DAY, GETDATE(), CASE fecha_cad WHEN '' THEN DATEADD(DAY, " + dias + ", pti_fecha) ELSE CONVERT(datetime, fecha_cad) END ) >= " + diasMinimos + " GROUP BY recibo ORDER BY recibo";
            }

            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info;
            Info = cmd.ExecuteReader();
            string szSQLnew = "";
            while (Info.Read())
            {

                if (v_Recibo == Info["recibo"].ToString().ToString().Trim())
                {
                    break;
                }
                else
                {
                    szSQLnew = szSQLnew + "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) VALUES (GETDATE(),'" + imei + "','" + responsable.Trim() + "','FA','7.1','" + pedido.Text.Trim() + "','Folio No Consecutivo Detectado Folio Actual " + v_Recibo.Trim() + " Folio notificado: " + Convert.ToString(Info["recibo"]).Trim() + " Con " + Convert.ToString(Info["tarimas"]).Trim() + " Tarimas Disponibles','SIPGAB','" + pedido.Text.Trim() + "') ";

                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Folio Anterior Disponible</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>Existe Disponible El recibo " + Convert.ToString(Info["recibo"]).Trim() + " Con " + Convert.ToString(Info["tarimas"]).Trim() + " Tarimas Disponibles</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                    });
                    alertDialog.Show();
                    return;
                }

            }

            if (szSQLnew.Trim().Length > 0)
            {
                cmd = new SqlCommand(szSQLnew, thisConnection);
                cmd.ExecuteNonQuery();

            }

            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

        }
        #endregion

        private void BtnIniciarCarga_Click(object sender, EventArgs e)
        {
            string Existe = "", tmpanden = "", mfec = "";
            DateTime mfec2 = DateTime.Now;


            if (Anden.Text.Trim() == "99" && Notrailer.Text.Trim() != "PC")
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>ANDEN NO DISPONIBLE</font>"));
                alertDialog.SetIcon(Resource.Drawable.Info);
                alertDialog.SetCancelable(false);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>EL ANDEN 99 ESTA DESTINADO PARA PROPIO CONDUCTO</font>"));
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                });
                alertDialog.Show();
                iniarCarga.Enabled = false;
                //iniarCarga.Visibility = ViewStates.Invisible;
                pedido.Enabled = true;
                pedido.RequestFocus();
                InputMethodManager immA = (InputMethodManager)GetSystemService(Context.InputMethodService);
                immA.ShowSoftInput(pedido, ShowFlags.Implicit);
                return;
            }



            if (pedido.Text.Trim() == "" || Notrailer.Text.Trim() == "" || lugar.Text.Trim() == "")
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Informacion Incmpleta</font>"));
                alertDialog.SetIcon(Resource.Drawable.Info);
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                });
                alertDialog.Show();
                iniarCarga.Enabled = false;
                //iniarCarga.Visibility = ViewStates.Invisible;
                pedido.Enabled = true;
                pedido.RequestFocus();
                InputMethodManager immC = (InputMethodManager)GetSystemService(Context.InputMethodService);
                immC.ShowSoftInput(pedido, ShowFlags.Implicit);
                return;

            }

            string responsablecarga = "";
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cadena = "Select responsable From tb_mstr_trailer Where no_trailer = '" + Notrailer.Text + "' and guardar = 'N'";
            SqlCommand cmdi = new SqlCommand(cadena);
            cmdi.Connection = thisConnection;
            SqlDataReader Infoi = cmdi.ExecuteReader();
            while (Infoi.Read())
            {
                responsablecarga = Infoi["responsable"].ToString().Trim();
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            if (responsablecarga.Trim() == "")
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Trailer sin Responsable</font>"));
                alertDialog.SetIcon(Resource.Drawable.Info);
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                });
                alertDialog.Show();
                iniarCarga.Enabled = true;
                //iniarCarga.Visibility = ViewStates.Visible;
                pedido.Enabled = true;
                pedido.RequestFocus();
                InputMethodManager immB = (InputMethodManager)GetSystemService(Context.InputMethodService);
                immB.ShowSoftInput(pedido, ShowFlags.Implicit);
                return;
            }




            string tipodeEmbarque = "";

            switch (lugar.Text.Trim())
            {
                case "Cancún":
                    tipodeEmbarque = "FC"; ;
                    break;
                case "Guadalajara":
                    tipodeEmbarque = "FG"; ;
                    break;
                case "Distrito Federal":
                    tipodeEmbarque = "FD"; ;
                    break;
                case "Externos":
                    tipodeEmbarque = "FE"; ;
                    break;
                case "Puerto Vallarta":
                    tipodeEmbarque = "FV"; ;
                    break;
                case "Cuautitlan":
                    tipodeEmbarque = "FM"; ;
                    break;
                case "Exportación":
                    tipodeEmbarque = "EXP"; ;
                    break;
                case "Nacional":
                    tipodeEmbarque = "NAL"; ;
                    break;
                case "Maquila":
                    tipodeEmbarque = "TRA"; ;
                    break;
            }

            fecha.Text = DateTime.Now.ToString("dd/MM/yyyy").Trim();
            horainicial.Text = DateTime.Now.ToString("hh:mm tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.").Trim();
            Existe = "F";

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            cadena = "Select sts from tb_mstr_embarque where emb_folio = '" + pedido.Text + "'";

            SqlCommand cmd = new SqlCommand(cadena);
            SqlDataReader Info;
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            int TotReg = 0;
            while (Info.Read())
            {
                Existe = Info["sts"].ToString().Trim();
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            if (Existe == "F")
            {
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                cadena = "IF NOT EXISTS(SELECT emb_folio FROM tb_mstr_embarque WHERE emb_folio = '" + pedido.Text + "' AND emb_tipo = '" + tipodeEmbarque + "') Insert Into tb_mstr_embarque(emb_folio, fecha_cap, hora_ini, no_trailer, sts, emb_tipo, anden, hora_fin, cajas, guardado, HORA_TRAILER, nalexp, emb_obs, PESO, grabomov, equipomov, turno, responsable)  Values ('" + pedido.Text + "', '" + fecha.Text + "', '" + horainicial.Text.Substring(0, 10) + "', '" + Notrailer.Text + "', 'C', '" + tipodeEmbarque + "', '" + Anden.Text + "', '--:--', '0','N','" + LblFT.Text + "','" + tipodeEmbarque + "','','0','','','0', '" + responsable + "')";
                cmd = new SqlCommand(cadena, thisConnection);
                cmd.ExecuteNonQuery();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            }
            else if (Existe == "")
            {
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                cadena = "DELETE from tb_mstr_embarque WHERE emb_folio = '" + pedido.Text + "' AND emb_tipo = '" + tipodeEmbarque + "'";
                cmd = new SqlCommand(cadena, thisConnection);
                cmd.ExecuteNonQuery();

                cadena = "IF NOT EXISTS(SELECT emb_folio FROM tb_mstr_embarque WHERE emb_folio = '" + pedido.Text + "' AND emb_tipo = '" + tipodeEmbarque + "') Insert Into tb_mstr_embarque(emb_folio, fecha_cap, hora_ini, no_trailer, sts, emb_tipo, anden, hora_fin, cajas, guardado, HORA_TRAILER, nalexp, emb_obs, PESO, grabomov, equipomov, turno, responsable)  Values ('" + pedido.Text + "', '" + fecha.Text + "', '" + horainicial.Text.Substring(0, 10) + "', '" + Notrailer.Text + "', 'C', '" + tipodeEmbarque + "', '" + Anden.Text + "', '--:--', '0','N','" + LblFT.Text + "','" + tipodeEmbarque + "','','0','','','0', '" + responsable + "')";
                cmd = new SqlCommand(cadena, thisConnection);
                cmd.ExecuteNonQuery();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            }
            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            cadena = "Select NO_TRAILER,anden,turno,horaini From tb_mstr_trailer Where no_trailer = '" + Notrailer.Text + "' and guardar = 'N'";
            cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                mfec = Info["horaini"].ToString().Trim();
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            if (mfec.Trim() == "--:--")
            {
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                string Cadena = "SELECT GETDATE()";
                SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
                string valor = Convert.ToDateTime(cmdx.ExecuteScalar()).ToString("dd/MM/yyyy hh:mm:ss tt");
                valor = valor.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.");


                cadena = "Update tb_mstr_trailer SET horaini = '" + valor + "' Where no_trailer = '" + Notrailer.Text + "' and guardar = 'N'";
                cmd = new SqlCommand(cadena, thisConnection);
                cmd.ExecuteNonQuery();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            }
            LLenaDetPed(pedido.Text, tipodeEmbarque);

            codigoetiqueta.Enabled = false;
            confirmprod.Enabled = false;
            fecha.Text = LblFT.Text.Trim();

            if (fecha.Text.Trim().Length > 10)
            {
                string[] fechatrailer = fecha.Text.Trim().Split(" ");
                fecha.Text = fechatrailer[0].Trim();
            }


            Mymenu.FindItem(Resource.Id.MenuItem5).SetEnabled(true);
            Mymenu.FindItem(Resource.Id.MenuItem6).SetEnabled(true);
            Mymenu.FindItem(Resource.Id.MenuItem7).SetEnabled(true);
            iniarCarga.Enabled = false;
            //iniarCarga.Visibility = ViewStates.Invisible;
            codigoetiqueta.Enabled = true;
            codigoetiqueta.RequestFocus();
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(codigoetiqueta, ShowFlags.Implicit);


        }

        private void LLenaDetPed(string orden_venta, string Tipo_emb)
        {
            string Prod = "", Hay = "", cant = "", desc = "";

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }


            if (Tipo_emb != "TRA")
            {
                cadena = "SELECT A.pdn_folio, A.prod_clave, A.pdn_num_unidades, A.pdn_tipo, B.Prod_Nombre FROM tb_det_pedidos A, Tb_Cat_Producto B Where A.pdn_folio = '" + orden_venta + "' and A.pdn_tipo = '" + Tipo_emb + "' and A.Prod_Clave = B.Prod_Clave ";

            }
            else
            {

                cadena = "SELECT A.EMB_FOLIO as PDN_FOLIO, A.prod_clave, A.EMB_unidades AS PDN_NUM_UNIDADES, A.EMB_tipo AS PDN_FOLIO, B.Prod_Nombre FROM tb_det_ordenes_emb A, Tb_Cat_Producto B Where A.emb_folio = '" + orden_venta + "' and A.emb_tipo = 'MAQ'  and A.Prod_Clave = B.Prod_Clave ";

            }
            SqlCommand cmd = new SqlCommand(cadena);
            SqlDataReader Info;
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            int TotReg = 0;
            while (Info.Read())
            {
                TotReg = TotReg + 1;
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            if (TotReg == 0)
            {
                return;
            }

            TbPed.Rows.Clear();

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            string Cadena = "DELETE tb_ped_embarque WHERE emb_folio = '" + orden_venta + "' and emb_tipo = '" + Tipo_emb + "'";
            SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
            cmdx.ExecuteNonQuery();
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }


            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            cadena = "Select emb_folio, prod_clave, emb_tipo, cant_ped from tb_ped_embarque Where emb_folio = '" + orden_venta + "' and emb_tipo = '" + Tipo_emb + "'";
            cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            try
            {
                TbPed.Columns.Add("Prod_Clave");
                TbPed.Columns.Add("Cantidad");
            }
            catch
            {

            }
            while (Info.Read())
            {
                TbPed.Rows.Add(Info["prod_clave"].ToString().Trim(), Info["cant_ped"].ToString().Trim());
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            if (Tipo_emb != "TRA")
            {
                cadena = "SELECT A.pdn_folio, A.prod_clave, A.pdn_num_unidades, A.pdn_tipo, B.Prod_Nombre FROM tb_det_pedidos A, Tb_Cat_Producto B Where A.pdn_folio = '" + orden_venta + "' and A.pdn_tipo = '" + Tipo_emb + "' and A.Prod_Clave = B.Prod_Clave ";

            }
            else
            {

                cadena = "SELECT A.EMB_FOLIO as PDN_FOLIO, A.prod_clave, A.EMB_unidades AS PDN_NUM_UNIDADES, A.EMB_tipo AS PDN_FOLIO, B.Prod_Nombre FROM tb_det_ordenes_emb A, Tb_Cat_Producto B Where A.emb_folio = '" + orden_venta + "' and A.emb_tipo = 'MAQ' and A.Prod_Clave = B.Prod_Clave ";

            }
            cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                Prod = Info["prod_clave"].ToString().Trim();
                //cant = Convert.ToInt32(Info["pdn_num_unidades"].ToString().Trim().Replace(".000", "")).ToString().Trim();
                // Convertimos primero a decimal para que entienda el punto, luego a int
                decimal valorDecimal = Convert.ToDecimal(Info["pdn_num_unidades"]);
                cant = ((int)valorDecimal).ToString();
                desc = Info["Prod_Nombre"].ToString().Trim();
                desc = desc.Replace("'", "").Trim();

                DataRow[] foundRows = TbPed.Select("Prod_Clave = '" + Prod + "'");
                Hay = "F";
                if (foundRows.Length > 0)
                {
                    foreach (DataRow row in foundRows)
                    {
                        Hay = "T";
                        Cadena = "UPDATE tb_ped_embarque set cant_ped = '" + cant + "', Borrar = 'N', nom_prod = '" + desc + "' WHERE emb_folio = '" + orden_venta + "' and prod_clave = '" + Prod + "' and emb_tipo = '" + Tipo_emb + "'";
                        cmdx = new SqlCommand(Cadena, thisConnection);
                        cmdx.ExecuteNonQuery();

                        //Console.WriteLine("{0}, {1}", row[0], row[1]);
                    }
                }
                if (Hay == "F")
                {

                    Cadena = "Insert into tb_ped_embarque (emb_folio, prod_clave, emb_tipo, cant_ped, cant_sur, nom_prod, nalexp, adicional,Borrar) Values ('" + orden_venta + "', '" + Prod + "', '" + Tipo_emb + "',' " + cant + "','0','" + desc + "','" + Tipo_emb + "',' ','N')";
                    cmdx = new SqlCommand(Cadena, thisConnection);
                    cmdx.ExecuteNonQuery();

                    TbPed.Rows.Add(Prod.ToString().Trim(), (Convert.ToInt32(cant).ToString().Trim()));

                }


            }
            //if (thisConnection.State == ConnectionState.Open){thisConnection.Close();}



            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            cadena = "SELECT prod_clave, cajas FROM tb_det_embarque Where emb_folio = '" + orden_venta + "' and emb_tipo = '" + Tipo_emb + "' and estatus != 'C'";
            cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                Prod = Info["prod_clave"].ToString().Trim();
                cant = Convert.ToInt32(Info["cajas"].ToString().Trim()).ToString();


                DataRow[] foundRows = TbPed.Select("Prod_Clave = '" + Prod + "'");
                Hay = "F";
                if (foundRows.Length > 0)
                {
                    foreach (DataRow row in foundRows)
                    {
                        Hay = "T";
                        Cadena = "UPDATE tb_ped_embarque set cant_sur = cant_sur + '" + cant + "', Borrar = 'N' WHERE emb_folio = '" + orden_venta + "' and prod_clave = '" + Prod + "' and emb_tipo = '" + Tipo_emb + "'";
                        cmdx = new SqlCommand(Cadena, thisConnection);
                        cmdx.ExecuteNonQuery();

                        //Console.WriteLine("{0}, {1}", row[0], row[1]);
                    }
                }
                if (Hay == "F")
                {
                    Cadena = "Insert into tb_ped_embarque (emb_folio, prod_clave, emb_tipo, cant_ped, cant_sur, nom_prod, nalexp, adicional,Borrar)  Values ('" + orden_venta + "', '" + Prod + "', '" + Tipo_emb + "','0',' " + cant + "','" + desc + "','" + Tipo_emb + "','S','N')";
                    cmdx = new SqlCommand(Cadena, thisConnection);
                    cmdx.ExecuteNonQuery();
                }
            }
            thisConnection.Close();

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            Cadena = "DELETE tb_ped_embarque WHERE emb_folio = '" + orden_venta + "' and emb_tipo = '" + Tipo_emb + "' AND Borrar = ' ' and Adicional <> 'S'";
            cmdx = new SqlCommand(Cadena, thisConnection);
            cmdx.ExecuteNonQuery();
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }


            TipoTar.Adapter = null;

            System.String[] strTarimas;
            //Llenar combo de pedidos
            System.Collections.ArrayList listadeTarimas = new System.Collections.ArrayList();

            if (Tipo_emb == "NAL")
            {
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                Cadena = "Select COUNT(Distinct(A.Id_Tarima)) From Tb_Cat_Tarima A, TB_TARIMAS_VTA_NAL B Where A.Id_Tarima = B.Clave_Tarima AND B.PDN_FOLIO = '" + Convert.ToInt32(orden_venta) + "'";
                cmdx = new SqlCommand(Cadena, thisConnection);
                string valorx = Convert.ToString(cmdx.ExecuteScalar());
                strTarimas = new System.String[Convert.ToInt32(valorx) + 1];
                strTarimas[0] = "TAR";
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                int x = 1;
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                cadena = "Select A.Id_Tarima, A.Nom_Tarima From Tb_Cat_Tarima A, TB_TARIMAS_VTA_NAL B Where A.Id_Tarima = B.Clave_Tarima AND B.PDN_FOLIO = '" + Convert.ToInt32(orden_venta) + "' Group by A.Id_Tarima, A.Nom_Tarima Order by A.Id_Tarima";
                cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    strTarimas[x] = Info["Nom_Tarima"].ToString().Trim();
                    x++;
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            }
            else
            {
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                Cadena = "Select COUNT(DISTINCT B.PDN_TIPOTAR) From Tb_Cat_Tarima A, TB_DET_PEDIDOS B WHERE B.PDN_FOLIO = '" + Convert.ToInt32(orden_venta) + "' AND  A.Id_Tarima = B.PDN_TIPOTAR";
                cmdx = new SqlCommand(Cadena, thisConnection);
                string valor = Convert.ToString(cmdx.ExecuteScalar());
                strTarimas = new System.String[Convert.ToInt32(valor) + 1];
                strTarimas[0] = "TAR";
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                int x = 1;
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                cadena = "Select DISTINCT B.PDN_TIPOTAR, A.Nom_Tarima From Tb_Cat_Tarima A, TB_DET_PEDIDOS B WHERE B.PDN_FOLIO = '" + Convert.ToInt32(orden_venta) + "' AND  A.Id_Tarima = B.PDN_TIPOTAR order by A.Nom_Tarima";
                cmd = new SqlCommand(cadena);
                cmd.Connection = thisConnection;
                Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    strTarimas[x] = Info["Nom_Tarima"].ToString().Trim();
                    x++;
                }
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            }

            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }


            Collections.AddAll(listadeTarimas, strTarimas);
            comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, strTarimas);
            TipoTar.Adapter = comboAdapter;


        }

        private void TRAE_PESO()
        {
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }

            cadena = "Select PESO,NO_TRAILER, FECHA From TB_MSTR_TRAILER Where NO_TRAILER = '" + Notrailer.Text + "' and hora_trailer = '" + fecha.Text + "'";
            SqlCommand cmd = new SqlCommand(cadena);
            SqlDataReader Info;
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                peso.Text = Info["PESO"].ToString().Trim();
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
        }

        private void mostrar_emb()
        {
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cadena = "Select emb_folio, fecha_cap, emb_tipo, hora_ini, hora_fin, no_trailer, hora_trailer from tb_mstr_embarque Where emb_folio = '" + pedido.Text + "'";
            SqlCommand cmd = new SqlCommand(cadena);
            SqlDataReader Info;
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                fecha.Text = Info["hora_trailer"].ToString().Trim();
                horainicial.Text = Info["hora_ini"].ToString().Trim();

                if (Info["hora_fin"].ToString().Trim().Length > 0)
                {
                    Horafinal.Text = Info["hora_fin"].ToString().Trim();
                }
                else
                {
                    Horafinal.Text = "";
                }
                switch (Info["emb_tipo"].ToString().Trim())
                {
                    case "FC":
                        lugar.Text = "Cancún";
                        break;
                    case "FG":
                        lugar.Text = "Guadalajara";
                        break;
                    case "FD":
                        lugar.Text = "Distrito Federal";
                        break;
                    case "FE":
                        lugar.Text = "Externos";
                        break;
                    case "FV":
                        lugar.Text = "Puerto Vallarta";
                        break;
                    case "FM":
                        lugar.Text = "Cuautitlan";
                        break;
                    case "EXP":
                        lugar.Text = "Exportación";
                        tipoped = "EXP";
                        break;
                    case "NAL":
                        lugar.Text = "Nacional";
                        tipoped = "NAL";
                        break;
                    case "TRA":
                        lugar.Text = "Maquila";
                        break;
                }
                Notrailer.Text = Info["no_trailer"].ToString().Trim();
                codigoetiqueta.Enabled = false;
                confirmprod.Enabled = false;

                if (Mymenu != null)
                {
                    Mymenu.FindItem(Resource.Id.MenuItem5).SetEnabled(false);
                    Mymenu.FindItem(Resource.Id.MenuItem6).SetEnabled(false);
                    Mymenu.FindItem(Resource.Id.MenuItem7).SetEnabled(true);
                    Mymenu.FindItem(Resource.Id.MenuItem8).SetEnabled(true);
                    Mymenu.FindItem(Resource.Id.MenuItem8DE).SetEnabled(false);
                }
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
        }

        private void Limpiar()
        {
            pedido.Enabled = true;
            Ordenes.Enabled = true;
            Notrailer.Text = "";
            lugar.Text = "---";
            Llenar_Combo();
            iniarCarga.Enabled = false;
            codigoetiqueta.Enabled = false;
            confirmprod.Enabled = false;
            //Anden.Text = "--";
            Anden.Text = AndenValida.ToString();
            Cajas.Enabled = false;
            fotoevent.Enabled = false;
            temperatura.Enabled = false;
            Mymenu.FindItem(Resource.Id.MenuItem5).SetEnabled(false);
            Mymenu.FindItem(Resource.Id.MenuItem6).SetEnabled(false);
            Mymenu.FindItem(Resource.Id.MenuItem7).SetEnabled(false);
            pedido.Text = "";
            fecha.Text = "";
            Horafinal.Text = "";
            horainicial.Text = "";
            codigoetiqueta.Text = "";
            confirmprod.Text = "";
            Cajas.Text = "";
            temperatura.Text = "";
            Posicion.Text = "";
            TipoTar.Enabled = false;
            pedido.RequestFocus();
            //InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            //imm.ShowSoftInput(pedido, ShowFlags.Implicit);
            peso.Text = "0.0";
            updatePesoPorEjes(Notrailer.Text, fecha.Text, "", "", "", "", pedido.Text);
        }

        #region Comboordenes
        private void Llenar_Combo()
        {
            System.String[] strOrdenes;
            System.String[] strTarimas;

            AsignarAnden();

            System.Collections.ArrayList listadeordenes = new System.Collections.ArrayList();
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            string Cadena = "Select Count(emb_folio) From tb_MSTR_embarque WHERE STS = 'C' AND anden = '" + AndenValida + "'";
            SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
            string valor = Convert.ToString(cmdx.ExecuteScalar());
            strOrdenes = new System.String[Convert.ToInt32(valor.Trim()) + 1];
            strOrdenes[0] = "PEDIDO";
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            int x = 1;
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cadena = "Select emb_folio From tb_MSTR_embarque WHERE STS = 'C'  AND anden = '" + AndenValida + "' ORDER BY EMB_FOLIO";
            SqlCommand cmd = new SqlCommand(cadena);
            SqlDataReader Info;
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                strOrdenes[x] = Info["emb_folio"].ToString().Trim();
                x++;
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            Collections.AddAll(listadeordenes, strOrdenes);
            comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, strOrdenes);
            Ordenes.Adapter = comboAdapter;
            Ordenes.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ordenes);

        }
        private void spinner_ordenes(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //throw new NotImplementedException();
            Spinner spinner = (Spinner)sender;
            if (spinner.GetItemAtPosition(e.Position).ToString() != "PEDIDO")
            {
                pedido.Text = spinner.GetItemAtPosition(e.Position).ToString().Trim();
                pedido.RequestFocus();
            }
            pedido.RequestFocus();
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(pedido, ShowFlags.Implicit);
        }
        #endregion

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuembarques, menu);
            Mymenu = menu;
            Limpiar();
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (Convert.ToString(item.TitleFormatted) == "Guardar")
            {
                string szSQL = "";
                int V_Cajas = 0, V_TCajas = 0, V_Tamaño = 0, i = 0, j = 0, cont = 0;
                string Cadena_V, v_Folio, Prd, Lote, Cajas, TipoPed, Errores, Observa, orden;


                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>GUARDAR EMBARQUE</font>"));
                alertDialog.SetIcon(Resource.Drawable.Info);
                alertDialog.SetCancelable(false);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>¿DESEA GUARDAR EL EMBARQUE?</font>"));
                alertDialog.SetPositiveButton("Guardar", (senderAlert, args) =>
                {
                    if (pedido.Text == "")
                    {
                        Toast.MakeText(this, "Informacion Incompleta....... Verifique los Datos", ToastLength.Long).Show();
                        return;
                    }
                    orden = pedido.Text.Trim();
                    TipoPed = "EXP";

                    switch (lugar.Text.Trim())
                    {
                        case "Nacional":
                            TipoPed = "NAL"; ;
                            break;
                        case "Exportación":
                            TipoPed = "EXP"; ;
                            break;
                        case "Maquila":
                            TipoPed = "TRA"; ;
                            break;
                    }

                    MostrarDialogoObservacionesSiAplica(orden, (observacionExtra) =>
                    {
                        if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                        cadena = "Select DISTINCT PROD_CLAVE,CANT_PED,CANT_SUR,NOM_PROD from tb_ped_embarque Where emb_folio='" + orden + "' and NALEXP = '" + TipoPed + "' order by NOM_PROD";
                        SqlCommand cmd = new SqlCommand(cadena);

                        SqlDataReader Info;
                        cmd.Connection = thisConnection;
                        Info = cmd.ExecuteReader();
                        Errores = "";
                        while (Info.Read())
                        {
                            if (Convert.ToInt32(Info["CANT_PED"].ToString().Trim()) != Convert.ToInt32(Info["CANT_SUR"].ToString().Trim()))
                            {
                                Errores = Errores + Info["CANT_PED"].ToString().Trim() + "   ," + Info["CANT_SUR"].ToString().Trim() + "   ," + Info["NOM_PROD"].ToString().Trim();
                            }
                        }
                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                        string observaciones = "";
                        string correo = "";

                        if (Errores.Trim().Length > 0)
                        {

                            et = new EditText(this);
                            et.InputType = Android.Text.InputTypes.TextVariationNormal | Android.Text.InputTypes.TextFlagImeMultiLine;
                            et.LongClickable = false;
                            et.Text = observaciones;

                            #region OBSERVACIONES ORDEN CON DIFERENCIA
                            AlertDialog.Builder ad = new AlertDialog.Builder(this);
                            ad.SetTitle("Observaciones Orden con Diferencia");
                            ad.SetCancelable(false);
                            ad.SetView(et);
                            ad.SetPositiveButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Guardar</font>"), (senderdiferencia, argsdiferencia) =>
                            {
                                observaciones = et.Text;
                                string obsFinal = (string.IsNullOrEmpty(observacionExtra) ? "" : observacionExtra + ". ") + observaciones;
                                if (correo == "")
                                {
                                    Toast.MakeText(this, "Favor de Notificar Posible Diferencia de Embarques Antes de Guardar el embarque", ToastLength.Long).Show();
                                }
                                else
                                {
                                    if (observaciones.Length < 10)
                                    {
                                        Toast.MakeText(this, "No se ha capturado la Observación", ToastLength.Long).Show();
                                        et.RequestFocus();
                                    }
                                    else
                                    {
                                        int splitpendientes = 0;
                                        if (thisConnection.State == ConnectionState.Closed)
                                        {
                                            thisConnection.Open();
                                        }
                                        cadena = "select COUNT(cajas) as total from tb_det_split Where emb_folio = '" + pedido.Text + "' And estatus = 'A'";
                                        cmd = new SqlCommand(cadena);
                                        cmd.Connection = thisConnection;
                                        Info = cmd.ExecuteReader();
                                        Errores = "";
                                        while (Info.Read())
                                        {
                                            splitpendientes = Convert.ToInt32(Info["total"].ToString().Trim());
                                        }
                                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                        if (splitpendientes > 0)
                                        {
                                            Toast.MakeText(this, "El pedido actual No puede ser cerrado, debido a que tiene Split Pendientes por Cargar", ToastLength.Long).Show();
                                            return;
                                        }

                                        if (thisConnection.State == ConnectionState.Closed)
                                        {
                                            thisConnection.Open();
                                        }
                                        cadena = "select ISNULL(sum(cajas),0) as total from tb_det_embarque Where emb_folio = '" + pedido.Text + "'";
                                        cmd = new SqlCommand(cadena, thisConnection);
                                        V_Cajas = Convert.ToInt32(cmd.ExecuteScalar());

                                        cadena = "UPDATE tb_mstr_embarque SET  hora_fin = '" + DateTime.Now.ToString("hh:mm tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "', cajas = '" + V_Cajas + "', sts = 'T', EMB_obs = '" + obsFinal + "' WHERE emb_folio = '" + pedido.Text + "' ";
                                        cmd = new SqlCommand(cadena, thisConnection);
                                        cmd.ExecuteNonQuery();

                                        if (TipoPed == "EXP")
                                        {
                                            cadena = "UPDATE tb_mstr_pedidos_exp SET  pdn_surtido = 'S' WHERE pdn_folio = '" + pedido.Text.Trim() + "'";
                                        }
                                        else
                                        {
                                            cadena = "UPDATE tb_mstr_pedidos_nal SET  pdn_surtido = 'S' WHERE pdn_folio = '" + pedido.Text.Trim() + "'";
                                        }
                                        cmd = new SqlCommand(cadena, thisConnection);
                                        cmd.ExecuteNonQuery();

                                        cadena = "UPDATE tb_det_acceso_celulares SET estado = 'T' WHERE estado = 'A' AND sistema = 'CargaEmbarques' AND folio ='" + pedido.Text.Trim() + "'";
                                        cmd = new SqlCommand(cadena, thisConnection);
                                        cmd.ExecuteNonQuery();
                                        if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                    }
                                }
                                ad.Dispose();
                                Limpiar();
                            });
                            ad.SetNegativeButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Cancelar</font>"), (senderdiferencia, argsdiferencia) =>
                            {
                                return;
                            });
                            ad.SetNeutralButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Notificar</font>"), (senderdiferencia, argsdiferencia) =>
                            {
                                observaciones = et.Text;
                                if (observaciones.Length < 10)
                                {
                                    Toast.MakeText(this, "No se ha capturado la Observación", ToastLength.Long).Show();
                                    et.RequestFocus();
                                }

                                switch (lugar.Text.Trim())
                                {
                                    case "Nacional":
                                        TipoPed = "NAL"; ;
                                        break;
                                    case "Exportación":
                                        TipoPed = "EXP"; ;
                                        break;
                                    case "Maquila":
                                        TipoPed = "TRA"; ;
                                        break;
                                }

                                int minutos = 0;
                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }
                                cadena = "IF EXISTS(SELECT emb_folio FROM tb_mstr_embarque WHERE emb_folio = '" + pedido.Text + "' AND EMB_obs != '') Select ISNULL(datediff (mi, (Select horaenvcorreo FROM tb_mstr_embarque WHERE  emb_folio = '" + pedido.Text + "'), GETDATE() ), 15) ELSE SELECT '15'";
                                cmd = new SqlCommand(cadena, thisConnection);
                                minutos = Convert.ToInt32(cmd.ExecuteScalar());
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                                if (minutos > 14)
                                {

                                    if (thisConnection.State == ConnectionState.Closed)
                                    {
                                        thisConnection.Open();
                                    }
                                    string obsFinal = (string.IsNullOrEmpty(observacionExtra) ? "" : observacionExtra + ". ") + observaciones;
                                    cadena = "UPDATE tb_mstr_embarque SET  EMB_obs = '" + obsFinal + "', horaenvcorreo = GETDATE() WHERE emb_folio = '" + pedido.Text + "' ";
                                    cmd = new SqlCommand(cadena, thisConnection);
                                    minutos = Convert.ToInt32(cmd.ExecuteScalar());
                                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                                    //var proxy = new WebServiceEmbarques.WebServiceEmbarques();
                                    //var proxy = new WSEmbarques.WebServiceEmbarques();

                                    if (INFO_FILE == "http://192.168.123.4:81/EmbarquesApk/estado_respaldo.txt")
                                    {
                                        proxyLocal.EnviarPosibleDiferenciaEmbarques(pedido.Text.Trim(), TipoPed);
                                    }
                                    else
                                    {
                                        proxy.EnviarPosibleDiferenciaEmbarques(pedido.Text.Trim(), TipoPed);
                                    }
                                    //var proxy = new WSCargaEmbarques189.WebServiceEmbarques();
                                    //proxy.EnviarPosibleDiferenciaEmbarques(pedido.Text.Trim(), TipoPed);
                                    Toast.MakeText(this, "POSIBLE DIFERENCIA DE EMBARQUES ENVIADA CORRECTAMENTE", ToastLength.Long).Show();

                                }
                                else
                                {
                                    Toast.MakeText(this, "Debe pasar un intervalo de 15 minutos Para volver a enviar el correo de Posible Diferencia de Embarques", ToastLength.Long).Show();
                                }
                            });
                            #endregion
                            #region GUARDAR EMBARQUE CON DIFERENCIA
                            Android.App.AlertDialog.Builder dialogdiferencia = new Android.App.AlertDialog.Builder(this);
                            dialogdiferencia.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>GUARDAR EMBARQUE CON DIFERENCIA</font>"));
                            dialogdiferencia.SetIcon(Resource.Drawable.Info);
                            dialogdiferencia.SetCancelable(false);
                            dialogdiferencia.SetMessage(Html.FromHtml("<font color='#000000' size = 10>¿Hay Diferencias en el Embarque desea Guardarlo?</font>"));
                            dialogdiferencia.SetPositiveButton("Continuar", (senderdiferencia, argsdiferencia) =>
                            {
                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }
                                cadena = "Select EMB_obs, horaenvcorreo FROM tb_mstr_embarque WHERE  emb_folio = '" + pedido.Text + "'";
                                cmd = new SqlCommand(cadena);
                                cmd.Connection = thisConnection;
                                Info = cmd.ExecuteReader();
                                Errores = "";
                                while (Info.Read())
                                {
                                    observaciones = observaciones + Info["EMB_obs"].ToString().Trim();
                                    correo = Info["horaenvcorreo"].ToString().Trim();
                                }
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                dialogdiferencia.Dispose();
                                ad.Show();


                            });
                            dialogdiferencia.SetNegativeButton("Cancelar", (senderdiferencia, argsdiferencia) =>
                            {
                                dialogdiferencia.Dispose();
                                return;
                            });
                            dialogdiferencia.Show();
                            #endregion
                        }
                        else
                        {
                            int splitpendientes = 0;
                            if (thisConnection.State == ConnectionState.Closed)
                            {
                                thisConnection.Open();
                            }
                            cadena = "select COUNT(cajas) as total from tb_det_split Where emb_folio = '" + pedido.Text + "' And estatus = 'A'";
                            cmd = new SqlCommand(cadena);
                            cmd.Connection = thisConnection;
                            Info = cmd.ExecuteReader();
                            Errores = "";
                            while (Info.Read())
                            {
                                splitpendientes = Convert.ToInt32(Info["total"].ToString().Trim());
                            }
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            if (splitpendientes > 0)
                            {
                                Toast.MakeText(this, "El pedido actual No puede ser cerrado, debido a que tiene Split Pendientes por Cargar", ToastLength.Long).Show();
                                return;
                            }

                            if (thisConnection.State == ConnectionState.Closed)
                            {
                                thisConnection.Open();
                            }
                            cadena = "select sum(cajas) as total from tb_det_embarque Where emb_folio = '" + pedido.Text + "'";
                            cmd = new SqlCommand(cadena, thisConnection);
                            V_Cajas = Convert.ToInt32(cmd.ExecuteScalar());

                            string obsFinal = (string.IsNullOrEmpty(observacionExtra) ? "" : observacionExtra + ". ") + observaciones;

                            cadena = "UPDATE tb_mstr_embarque SET  hora_fin = '" + DateTime.Now.ToString("hh:mm tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "', cajas = '" + V_Cajas + "', sts = 'T', EMB_obs = '" + obsFinal + "' WHERE emb_folio = '" + pedido.Text + "' ";
                            cmd = new SqlCommand(cadena, thisConnection);
                            cmd.ExecuteNonQuery();

                            if (TipoPed == "EXP")
                            {
                                cadena = "UPDATE tb_mstr_pedidos_exp SET  pdn_surtido = 'S' WHERE pdn_folio = '" + pedido.Text.Trim() + "'";
                            }
                            else
                            {
                                cadena = "UPDATE tb_mstr_pedidos_nal SET  pdn_surtido = 'S' WHERE pdn_folio = '" + pedido.Text.Trim() + "'";
                            }
                            cmd = new SqlCommand(cadena, thisConnection);
                            cmd.ExecuteNonQuery();

                            cadena = "UPDATE tb_det_acceso_celulares SET estado = 'T' WHERE estado = 'A' AND sistema = 'SplitTrailer' AND folio ='" + pedido.Text.Trim() + "'";
                            cmd = new SqlCommand(cadena, thisConnection);
                            cmd.ExecuteNonQuery();
                            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            Limpiar();

                        }
                    });



                });

                alertDialog.SetNegativeButton("REGRESAR", (senderAlert, args) =>
                {
                    return;
                });
                alertDialog.Show();
            }
            else if (Convert.ToString(item.TitleFormatted) == "Cancelar")
            {
                string szSQL, MTIPO, mrecibo, mprod, mti;
                int V_Cajas, V_DIF, mtar, X, mtam;
                string usuario = "";
                if (pedido.Text == "")
                {
                    Toast.MakeText(this, "Informacion Incompleta...", ToastLength.Long).Show();
                }
                else
                {
                    int continuar = 0;
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Cancelar Embarque</font>"));
                    alertDialog.SetIcon(Resource.Drawable.question);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>Esta iniciando el proceso de cancelacion completo del embarque ¿Desea Continuar?</font>"));
                    alertDialog.SetPositiveButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Sí</font>"), (senderAlert, args) =>
                    {
                        continuar = 1;
                        alertDialog.Dispose();
                        if (continuar == 1)
                        {
                            et = new EditText(this);
                            et.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
                            et.LongClickable = false;
                            et.Hint = "Password";
                            AlertDialog.Builder ad = new AlertDialog.Builder(this);
                            ad.SetTitle("Autorizacion Cancelar Embarque");
                            ad.SetCancelable(false);
                            ad.SetView(et);
                            ad.SetPositiveButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Guardar</font>"), (senderAlertx, argsx) =>
                            {

                                if (thisConnection.State == ConnectionState.Closed)
                                {
                                    thisConnection.Open();
                                }
                                cadena = "SELECT CONCAT(Nombre, ' ', Ape_Pat, ' ', Ape_Mat) As Nombre FROM TB_RESPON_CARGA WHERE status = 'A' AND password = '" + et.Text.Trim().ToUpper() + "'";
                                SqlCommand cmd = new SqlCommand(cadena, thisConnection);
                                usuario = Convert.ToString(cmd.ExecuteScalar());
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                MTIPO = "";
                                if (usuario.Trim().Length > 0)
                                {
                                    switch (lugar.Text.Trim())
                                    {
                                        case "Nacional":
                                            MTIPO = "NAL"; ;
                                            break;
                                        case "Exportación":
                                            MTIPO = "EXP"; ;
                                            break;
                                        case "Maquila":
                                            MTIPO = "TRA"; ;
                                            break;
                                    }

                                    if (thisConnection.State == ConnectionState.Closed)
                                    {
                                        thisConnection.Open();
                                    }
                                    cadena = "SELECT recibo,tipo_rec,prod_clave,tarima, sum(cajas) aS cAJAS from tb_det_embarque WHERE emb_folio = '" + pedido.Text + "' AND emb_tipo = '" + MTIPO + "' AND Estatus = 'A' AND opCap = 'N' group by recibo,tipo_rec,prod_clave,tarima";
                                    cmd = new SqlCommand(cadena);
                                    cmd.Connection = thisConnection;
                                    SqlDataReader Info;
                                    Info = cmd.ExecuteReader();
                                    while (Info.Read())
                                    {
                                        V_Cajas = Convert.ToInt32(Info["cAJAS"].ToString().Trim());
                                        mrecibo = Convert.ToString(Info["recibo"].ToString().Trim());
                                        mprod = Convert.ToString(Info["prod_clave"].ToString().Trim());
                                        mtar = Convert.ToInt32(Info["tarima"].ToString().Trim());
                                        mti = Convert.ToString(Info["tipo_rec"].ToString().Trim());
                                        //splitpendientes = Convert.ToInt32(Info["total"].ToString().Trim());
                                        if (V_Cajas > 0)
                                        {
                                            if (mti == "PTC")
                                            {
                                                szSQL = "UPDATE TB_DET_TRAZABILIDAD SET pti_estatus_sur = ' ', SURTIDO = SURTIDO - '" + V_Cajas.ToString() + "' WHERE RECIBO = '" + mrecibo + "' and prod_clave = '" + mprod + "' AND TARIMA = '" + mtar.ToString() + "'";
                                                szSQL = szSQL + " UPDATE tb_det_embarque SET estatus = 'C' WHERE emb_folio = '" + pedido.Text + "' and emb_tipo = '" + MTIPO + "' AND prod_clave = '" + mprod + "' AND tipo_rec = 'PTC' AND tarima = '" + mtar.ToString() + "' AND recibo = '" + mrecibo + "'";
                                                cmd = new SqlCommand(szSQL, thisConnection);
                                                cmd.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                szSQL = "UPDATE TB_DET_ETI_FINAL SET estatus_sur = ' ',cajas_sur = cajas_sur - '" + V_Cajas.ToString() + "' WHERE folio = '" + mrecibo + "' and cve_prod = '" + mprod + "' AND TARIMA = '" + mtar.ToString() + "'";
                                                szSQL = szSQL + " UPDATE tb_det_embarque SET estatus = 'C' WHERE emb_folio = '" + pedido.Text + "' and emb_tipo = '" + MTIPO + "' AND prod_clave = '" + mprod + "' AND tipo_rec = 'PTP' AND tarima = '" + mtar.ToString() + "' AND recibo = '" + mrecibo + "'";
                                                cmd = new SqlCommand(szSQL, thisConnection);
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }

                                    szSQL = "DELETE FROM tb_mstr_embarque WHERE emb_folio = '" + pedido.Text + "' and emb_tipo = '" + MTIPO + "'";
                                    cmd = new SqlCommand(szSQL, thisConnection);
                                    cmd.ExecuteNonQuery();

                                    szSQL = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) VALUES (GETDATE(),'" + imei + "','" + usuario + "','C','7.1','" + pedido.Text + "','CANCELACION DE EMBARQUE DESDE LECTORA','SIPGAB','" + pedido.Text + "')";
                                    cmd = new SqlCommand(szSQL, thisConnection);
                                    cmd.ExecuteNonQuery();

                                    if (MTIPO == "EXP")
                                    {
                                        szSQL = "UPDATE tb_mstr_pedidos_exp SET  pdn_surtido = '' WHERE pdn_folio = '" + pedido.Text.Trim() + "'";
                                    }
                                    else
                                    {
                                        szSQL = "UPDATE tb_mstr_pedidos_nal SET  pdn_surtido = '' WHERE pdn_folio = '" + pedido.Text.Trim() + "'";
                                    }
                                    cmd = new SqlCommand(szSQL, thisConnection);
                                    cmd.ExecuteNonQuery();

                                    szSQL = "UPDATE tb_det_embarque SET estatus = 'C' WHERE emb_folio = '" + pedido.Text.Trim() + "' and emb_tipo = '" + MTIPO + "'";
                                    cmd = new SqlCommand(szSQL, thisConnection);
                                    cmd.ExecuteNonQuery();

                                    szSQL = "UPDATE tb_det_split SET estatus = 'A'  WHERE emb_folio = '" + pedido.Text.Trim() + "' and emb_tipo = '" + MTIPO + "' and estatus = 'S'";
                                    cmd = new SqlCommand(szSQL, thisConnection);
                                    cmd.ExecuteNonQuery();

                                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                                    ad.Dispose();
                                    Android.App.AlertDialog.Builder alertDialogx = new Android.App.AlertDialog.Builder(this);
                                    alertDialogx.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Embarque Cancelado</font>"));
                                    alertDialogx.SetIcon(Resource.Drawable.warning);
                                    alertDialogx.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El embarque Actual Fue Cancelado con Exito</font>"));
                                    alertDialogx.SetCancelable(false);
                                    alertDialogx.SetNeutralButton("Ok", delegate
                                    {
                                        Limpiar();
                                        alertDialogx.Dispose();

                                    });
                                    alertDialogx.Show();
                                }
                                else
                                {
                                    Toast.MakeText(this, "El usuario no es valido, verifique las contraseñas", ToastLength.Long).Show();
                                }

                            });
                            ad.SetNegativeButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Cancelar</font>"), (senderAlertx, argsx) =>
                            {
                                ad.Dispose();
                                return;

                            });
                            ad.Show();

                        }
                    });
                    alertDialog.SetNegativeButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>No</font>"), (senderAlert, args) =>
                    {
                        return;
                    });
                    alertDialog.Create();
                    alertDialog.Show();
                }

            }
            else if (Convert.ToString(item.TitleFormatted) == "Consultar")
            {
                string orden = pedido.Text.Trim();
                string tipoped = "";

                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        tipoped = "FC"; ;
                        break;
                    case "Guadalajara":
                        tipoped = "FG"; ;
                        break;
                    case "Distrito Federal":
                        tipoped = "FD"; ;
                        break;
                    case "Externos":
                        tipoped = "FE"; ;
                        break;
                    case "Puerto Vallarta":
                        tipoped = "FV"; ;
                        break;
                    case "Cuautitlan":
                        tipoped = "FM"; ;
                        break;
                    case "Exportación":
                        tipoped = "EXP"; ;
                        break;
                    case "Nacional":
                        tipoped = "NAL"; ;
                        break;
                    case "Maquila":
                        tipoped = "TRA"; ;
                        break;
                }

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                LLenaDetPed(pedido.Text, tipoped);
                TRAE_PESO();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                Intent intent = new Intent(this, typeof(frmpedVsSur));
                intent.PutExtra("ordenventa", pedido.Text.Trim());
                intent.PutExtra("tipoorden", tipoped);
                StartActivity(intent);

            }
            else if (Convert.ToString(item.TitleFormatted) == "Detalle de Etiquetas")
            {
                string orden = pedido.Text.Trim();
                string tipoped = "";

                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        tipoped = "FC"; ;
                        break;
                    case "Guadalajara":
                        tipoped = "FG"; ;
                        break;
                    case "Distrito Federal":
                        tipoped = "FD"; ;
                        break;
                    case "Externos":
                        tipoped = "FE"; ;
                        break;
                    case "Puerto Vallarta":
                        tipoped = "FV"; ;
                        break;
                    case "Cuautitlan":
                        tipoped = "FM"; ;
                        break;
                    case "Exportación":
                        tipoped = "EXP"; ;
                        break;
                    case "Nacional":
                        tipoped = "NAL"; ;
                        break;
                    case "Maquila":
                        tipoped = "TRA"; ;
                        break;
                }

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                TRAE_PESO();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                Intent intent = new Intent(this, typeof(DetalleEtiquetas));
                intent.PutExtra("ordenventa", pedido.Text.Trim());
                intent.PutExtra("tipoorden", tipoped);
                intent.PutExtra("responsable", responsable.Trim());
                StartActivity(intent);
            }
            else if (Convert.ToString(item.TitleFormatted) == "Consulta PesoXEjes")
            {
                string tipoped = "";

                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        tipoped = "FC"; ;
                        break;
                    case "Guadalajara":
                        tipoped = "FG"; ;
                        break;
                    case "Distrito Federal":
                        tipoped = "FD"; ;
                        break;
                    case "Externos":
                        tipoped = "FE"; ;
                        break;
                    case "Puerto Vallarta":
                        tipoped = "FV"; ;
                        break;
                    case "Cuautitlan":
                        tipoped = "FM"; ;
                        break;
                    case "Exportación":
                        tipoped = "EXP"; ;
                        break;
                    case "Nacional":
                        tipoped = "NAL"; ;
                        break;
                    case "Maquila":
                        tipoped = "TRA"; ;
                        break;
                }

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }

                Intent intent = new Intent(this, typeof(PesoXejes));
                intent.PutExtra("no_trailer", Notrailer.Text.Trim());
                intent.PutExtra("horatrailer", fecha.Text.Trim());
                intent.PutExtra("emb_tipo", tipoped.Trim());
                StartActivity(intent);
            }
            else if (Convert.ToString(item.TitleFormatted) == "Nuevo")
            {
                Mymenu.FindItem(Resource.Id.MenuItem3).SetEnabled(true);
                Limpiar();
                AsignarAnden();
                pedido.Enabled = true;
                Ordenes.Enabled = true;
                Ordenes.RequestFocus();
            }
            else if (Convert.ToString(item.TitleFormatted) == "Limpiar")
            {
                if (TipoTar.HasFocus)
                {
                    TipoTar.PerformClick();
                }
                else
                {
                    if (Posicion.HasFocus)
                    {
                        Posicion.Enabled = true;
                        Posicion.RequestFocus();
                        InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                        imm.ShowSoftInput(Posicion, ShowFlags.Implicit);
                    }
                }
                Borrar();
            }
            else if (Convert.ToString(item.TitleFormatted) == "Trailer")
            {
                Intent intent = new Intent(this, typeof(Frmtrailer));
                intent.PutExtra("responsable", responsable.Trim());
                intent.PutExtra("Anden", AndenValida.ToString().Trim());
                intent.PutExtra("no_trailer", Ordenes.ToString().Trim());
                StartActivity(intent);
            }
            else if (Convert.ToString(item.TitleFormatted) == "Split")
            {
                string orden = pedido.Text.Trim();


                switch (lugar.Text.Trim())
                {
                    case "Cancún":
                        tipoped = "FC";
                        break;
                    case "Guadalajara":
                        tipoped = "FG";
                        break;
                    case "Distrito Federal":
                        tipoped = "FD";
                        break;
                    case "Externos":
                        tipoped = "FE";
                        break;
                    case "Puerto Vallarta":
                        tipoped = "FV";
                        break;
                    case "Cuautitlan":
                        tipoped = "FM";
                        break;
                    case "Exportación":
                        tipoped = "EXP";
                        break;
                    case "Nacional":
                        tipoped = "NAL";
                        break;
                    case "Maquila":
                        tipoped = "TRA";
                        break;
                }

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }
                TRAE_PESO();
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                Intent intent = new Intent(this, typeof(DetalleSplit));
                intent.PutExtra("ordenventa", pedido.Text.Trim());
                intent.PutExtra("tipoorden", tipoped);
                StartActivity(intent);
            }
            else if (Convert.ToString(item.TitleFormatted) == "Cerrar Sesion")
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Cerrar Sesion</font>"));
                alertDialog.SetIcon(Resource.Drawable.question);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>¿Desea Cerrar su sesion en este equipo?</font>"));
                alertDialog.SetPositiveButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Sí</font>"), SaveAction);
                alertDialog.SetNegativeButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>No</font>"), CancelaAction);
                alertDialog.Create();
                alertDialog.Show();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SaveAction(object sender, DialogClickEventArgs e)
        {
            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            string cadena = "UPDATE tb_det_acceso_celulares SET estado = 'T' WHERE " +
                        "nom_usu = '" + responsable + "' AND sistema = 'CAPTURAEMBARQUE' AND folio = '' AND estado = 'A' AND IMEI = '" + imei + "'";
            SqlCommand cmd = new SqlCommand(cadena, thisConnection);
            cmd.ExecuteNonQuery();
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            Intent intent = new Intent(this, typeof(MainActivity));
            //intent.PutExtra("cvresponsable", responsable.ToString());
            //intent.PutExtra("responsable", responsablesplit.ToString());
            //intent.PutExtra("imei", imei.ToString());
            StartActivity(intent);
            Finish();
        }

        private void CancelaAction(object sender, DialogClickEventArgs e)
        {
            return;
        }

        public string ValidarProductoEnPedido(string clave_producto, string NALExp)
        {
            string estado = "";

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cadena = "Select * From tb_PED_embarque Where emb_folio = '" + pedido.Text + "' and prod_clave = '" + clave_producto + "' and Nalexp = '" + NALExp + "'";
            SqlCommand cmdproductoenorden = new SqlCommand(cadena);
            cmdproductoenorden.Connection = thisConnection;
            SqlDataReader InfoProductoorden = cmdproductoenorden.ExecuteReader();
            int v_cajasPedidas = 0;
            while (InfoProductoorden.Read())
            {
                v_cajasPedidas = Convert.ToInt32(InfoProductoorden["cant_ped"].ToString().Trim()) - Convert.ToInt32(InfoProductoorden["cant_sur"].ToString().Trim());
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            if (v_cajasPedidas == 0)
            {
                estado = "SIN PEDIDO";

            }
            return estado;
        }

        public string ValidarProd(string clave_producto, string NALExp)
        {
            string estado = "";
            int cantpedido = 0;
            int cantsurtido = 0;


            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }

            SqlCommand cmdproductoenorden;
            cmdproductoenorden = thisConnection.CreateCommand();
            //cmdproductoenorden.CommandText = "SELECT pdn_num_unidades FROM tb_det_pedidos WHERE pdn_folio = '" + Convert.ToInt32(pedido.Text).ToString() + "' and prod_clave = '" + clave_producto + "' and pdn_tipo = '" + NALExp + "'";
            cmdproductoenorden.CommandText = "IF EXISTS (SELECT pdn_num_unidades FROM tb_det_pedidos WHERE pdn_folio = '" + Convert.ToInt32(pedido.Text).ToString() + "' and prod_clave = '" + clave_producto + "' and pdn_tipo = '" + NALExp + "') SELECT pdn_num_unidades FROM tb_det_pedidos WHERE pdn_folio = '" + Convert.ToInt32(pedido.Text).ToString() + "' and prod_clave = '" + clave_producto + "' and pdn_tipo = '" + NALExp + "' ELSE select 0 AS pdn_num_unidades";

            try
            {
                cantpedido = Convert.ToInt32(cmdproductoenorden.ExecuteScalar());
            }
            catch
            {
                cantpedido = 0;
            }

            SqlCommand cmdproductosurtido;
            cmdproductosurtido = thisConnection.CreateCommand();
            cmdproductosurtido.CommandText = "SELECT SUM(cajas) FROM tb_det_embarque WHERE emb_folio = '" + pedido.Text.Trim() + "' and prod_clave = '" + clave_producto + "' and emb_tipo = '" + NALExp + "' AND estatus = 'A'";
            try
            {
                cantsurtido = Convert.ToInt32(cmdproductosurtido.ExecuteScalar());
            }
            catch
            {
                cantsurtido = 0;
            }

            if (Cajas.Text == "")
            {
                Toast.MakeText(this, "Por favor leer nuevamenmte la etiqueta.", ToastLength.Long).Show();
            }
            else
            {
                cantsurtido = cantsurtido + Convert.ToInt32(Cajas.Text.Trim());
            }



            if ((cantpedido - cantsurtido) < 0)
            {
                estado = "CARGANDOMAS";

            }
            return estado;
        }

        public void validarCargaAdicional()
        {
            #region TERMOGRABADOR
            Func<string, bool> esTermograValido = codigo => codigo.Trim() == "17TERMOGRA" && ProductoExisteEnPedido(pedido.Text.Trim(), "17TERMOGRA");
            if (esTermograValido(codigoetiqueta.Text))
            {
                Surtir17TermograDirecto(); // método que encapsula el surtido directo
                return; // salimos sin ejecutar el resto del código de carga adicional
            }
            // Si el código era "17TERMOGRA" pero no está en el pedido, podrías mostrar un error
            if (codigoetiqueta.Text.Trim() == "17TERMOGRA")
            {
                MostrarAlerta("Producto no válido", "Este pedido no requiere 17TERMOGRA", Resource.Drawable.warning);
                return;
            }
            #endregion
            string placatrailer = "1";
            string fechatrailer = "1";
            string Observaciones = "";
            string pedido_adicional_facturado = "";

            string VTipoAdicional = "";

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            cadena = "Select hora_trailer, no_trailer, observaciones, pdn_adicional From tb_det_pend_embarque WHERE claveunica = '" + codigoetiqueta.Text.Trim() + "'";
            SqlCommand cmdproductoenorden = new SqlCommand(cadena);
            cmdproductoenorden.Connection = thisConnection;
            SqlDataReader InfoProductoorden = cmdproductoenorden.ExecuteReader();
            int v_cajasPedidas = 0;
            while (InfoProductoorden.Read())
            {
                placatrailer = InfoProductoorden["no_trailer"].ToString().Trim();
                fechatrailer = InfoProductoorden["hora_trailer"].ToString().Trim();
                Observaciones = InfoProductoorden["observaciones"].ToString().Trim();
                pedido_adicional_facturado = InfoProductoorden["pdn_adicional"].ToString().Trim();
            }


            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            if (codigoetiqueta.Text.Trim().Contains("FAC") == true && Convert.ToInt32(pedido_adicional_facturado) != Convert.ToInt32(pedido.Text.Trim()))
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Orden No corresponde</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden no corresponde a la carga adicional Facturada</font>"));
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();

                });
                alertDialog.Show();
            }


            if (placatrailer == "1" && fechatrailer == "1")
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Codigo No Existe</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Codigo que deseea Ingresar no existe</font>"));
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();

                });
                alertDialog.Show();

            }
            else
            {
                if (placatrailer != Notrailer.Text.Trim() && fechatrailer == fecha.Text.Trim())
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Fecha de Carga No Correspondiente</font>"));
                    alertDialog.SetIcon(Resource.Drawable.warning);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>EL Pendiente no se carga el dia de hoy, este pendiente se carga el" + fechatrailer + "</font>"));
                    alertDialog.SetCancelable(false);
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();

                    });
                    alertDialog.Show();

                }
                else if (placatrailer == "" && fechatrailer == "")
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Pendiente Sin Trailer</font>"));
                    alertDialog.SetIcon(Resource.Drawable.warning);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>EL Pendiente de embarque aun no tiene un trailer Asignado</font>"));
                    alertDialog.SetCancelable(false);
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();

                    });
                    alertDialog.Show();
                }
                else if (placatrailer != Notrailer.Text.Trim() && fechatrailer != fecha.Text.Trim())
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Trailer Incorrecto</font>"));
                    alertDialog.SetIcon(Resource.Drawable.warning);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>EL Pendiente de embarque No corresponde a este Trailer</font>"));
                    alertDialog.SetCancelable(false);
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();

                    });
                    alertDialog.Show();
                }
                else
                {
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }
                    string cadena = "UPDATE tb_det_pend_embarque set estatus = 'S' WHERE claveunica = '" + codigoetiqueta.Text.Trim() + "' and estatus = 'A'";
                    SqlCommand cmd = new SqlCommand(cadena, thisConnection);
                    cmd.ExecuteNonQuery();
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }


                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Carga Adicional Surtido con Exito</font>"));
                    alertDialog.SetIcon(Resource.Drawable.exito);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Carga Adicional de embarque Se ha surtido Con Exito!</font>"));
                    alertDialog.SetCancelable(false);
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();

                    });
                    alertDialog.Show();

                    if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                    cadena = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) VALUES (GETDATE(),'" + imei + "','','S','7.1','" + pedido.Text + "','SURTIDO PENDIENTE EMBARQUES " + codigoetiqueta.Text.Trim() + "','SIPGAB','" + pedido.Text + "')";
                    cmd = new SqlCommand(cadena, thisConnection);
                    cmd.ExecuteNonQuery();
                    if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

                    if (codigoetiqueta.Text.Trim().Contains("FAC") == true)
                    {
                        switch (lugar.Text.Trim())
                        {
                            case "Cancún":
                                VTipoAdicional = "FC";
                                break;
                            case "Guadalajara":
                                VTipoAdicional = "FG";
                                break;
                            case "Distrito Federal":
                                VTipoAdicional = "FD";
                                break;
                            case "Externos":
                                VTipoAdicional = "FE";
                                break;
                            case "Puerto Vallarta":
                                VTipoAdicional = "FV";
                                break;
                            case "Cuautitlan":
                                VTipoAdicional = "FM";
                                break;
                            case "Exportación":
                                VTipoAdicional = "EXP";
                                break;
                            case "Nacional":
                                VTipoAdicional = "NAL";
                                break;
                            case "Maquila":
                                VTipoAdicional = "TRA";
                                break;
                        }

                        string[] arrSplitproductos;
                        string[] arrSplitproductosdetalle;
                        string[] arrSplitproductosdetallecompleto;

                        arrSplitproductos = Observaciones.Replace("-ñx-", "°").Split("°");

                        for (int i = 0; i < arrSplitproductos.Length; i++)
                        {
                            if (arrSplitproductos[i].ToString().Trim().Length > 0)
                            {
                                string lineaprod = arrSplitproductos[i].ToString().Trim();
                                arrSplitproductosdetalle = lineaprod.Replace("-*-", "°").Split("°");
                                string cveprod = arrSplitproductosdetalle[0];
                                string nombreprodcant = arrSplitproductosdetalle[1];
                                arrSplitproductosdetallecompleto = nombreprodcant.Replace("-ñ-", "°").Split("°");
                                string nomprod = arrSplitproductosdetallecompleto[0];
                                string cantprod = arrSplitproductosdetallecompleto[1];
                                string no_lote = "111111" + cveprod + " 1";


                                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
                                cadena = "Insert into tb_det_embarque (emb_folio, prod_clave, no_lote, cajas, seccion, temp, emb_tipo, tarima, tarima_f, tipo_rec, estatus,FEC_CAD,FECHACAD,FECHACAP,OPCAP,ID_TARIMA,RECIBO, id_lectora, datecaptura)  Values ('" + pedido.Text + "', '" + cveprod + "', '" + no_lote.Trim() + "', " + cantprod + ", 30, '34', '" + VTipoAdicional + "', '1', '1', 'PTP', 'A','','','" + fecha.Text + "','N','ESP','111111', '" + imei + "', GETDATE())";
                                cmd = new SqlCommand(cadena, thisConnection);
                                cmd.ExecuteNonQuery();
                                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
                            }

                        }
                    }
                }
            }
        }
        #region METODOS DE CARGA ADICIONAL PARA PRODUCTOS ESPECIALES
        private void Surtir17TermograDirecto()
        {
            // Ajusta los valores fijos según tu proceso (tipo, sección, etc.)
            string insertEmb = @"INSERT INTO tb_det_embarque 
        (emb_folio, prod_clave, no_lote, cajas, seccion, temp, emb_tipo, tarima, tarima_f, tipo_rec, estatus,
         FEC_CAD, FECHACAD, FECHACAP, OPCAP, ID_TARIMA, RECIBO, id_lectora, datecaptura)
        VALUES 
        (@folio, '17TERMOGRA', '11111117TERMOGRA 1', 1, 30, '34', 'NAL', '1', '1', 'PTP', 'A',
         '', '', @fecha, 'N', 'ESP', '111111', @imei, GETDATE())";

            string insertMov = @"INSERT INTO TB_REGISTRO_MOVIMIENTOS 
        (FECHA, NOM_COMPU, NOM_USU, TIPO_MOV, OP_CLAVE, FOLIO, DETALLE, SISTEMA, MOV_FOLIO)
        VALUES 
        (GETDATE(), @imei, '', 'S', '7.1', @folio, 'SURTIDO DIRECTO 17TERMOGRA', 'SIPGAB', @folio)";

            try
            {
                if (thisConnection.State == ConnectionState.Closed)
                    thisConnection.Open();

                using (var cmdEmb = new SqlCommand(insertEmb, thisConnection))
                {
                    cmdEmb.Parameters.AddWithValue("@folio", pedido.Text.Trim());
                    cmdEmb.Parameters.AddWithValue("@fecha", fecha.Text.Trim());
                    cmdEmb.Parameters.AddWithValue("@imei", imei);
                    cmdEmb.ExecuteNonQuery();
                }

                using (var cmdMov = new SqlCommand(insertMov, thisConnection))
                {
                    cmdMov.Parameters.AddWithValue("@folio", pedido.Text.Trim());
                    cmdMov.Parameters.AddWithValue("@imei", imei);
                    cmdMov.ExecuteNonQuery();
                }

                // Alerta de éxito (puedes usar el helper MostrarAlerta)
                MostrarAlerta("Producto Directo", "17TERMOGRA surtido exitosamente al pedido", Resource.Drawable.exito);
            }
            catch (Java.Lang.Exception ex)
            {
                MostrarAlerta("Error", "No se pudo surtir 17TERMOGRA: " + ex.Message, Resource.Drawable.warning);
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }
        }
        private void MostrarAlerta(string titulo, string mensaje, int icono)
        {
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle(Html.FromHtml($"<font color='#FFC107' size=10>{titulo}</font>"));
            alert.SetIcon(icono);
            alert.SetMessage(Html.FromHtml($"<font color='#000000' size=10>{mensaje}</font>"));
            alert.SetCancelable(false);
            alert.SetNeutralButton("Ok", delegate { alert.Dispose(); });
            alert.Show();
        }
        private bool ProductoExisteEnPedido(string folioPedido, string prodClave)
        {
            string query = @"SELECT COUNT(*) 
                     FROM tb_det_pedidos 
                     WHERE pdn_folio = @folio AND prod_clave = @clave 
                     AND pdn_num_unidades > 0";  // ajusta según tu esquema

            try
            {
                if (thisConnection.State == ConnectionState.Closed)
                    thisConnection.Open();

                using (var cmd = new SqlCommand(query, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@folio", folioPedido);
                    cmd.Parameters.AddWithValue("@clave", prodClave);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }
        }
        #endregion
        protected override void OnResume()
        {
            base.OnResume();

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                try
                {
                    locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                }
                catch
                {
                    Toast.MakeText(this, "Fallo Proveedor", ToastLength.Long).Show();
                }

            }
            else
            {
                // FIX #3: requestCodes distintos por permiso. Antes los 3 usaban 1,
                // por lo que OnRequestPermissionsResult no podía distinguir cuál se denegó.
                ActivityCompat.RequestPermissions(this, new System.String[] { Manifest.Permission.AccessFineLocation }, REQ_LOCATION);
                ActivityCompat.RequestPermissions(this, new System.String[] { Manifest.Permission.WriteExternalStorage }, REQ_WRITE_STORAGE);
                ActivityCompat.RequestPermissions(this, new System.String[] { Manifest.Permission.ReadExternalStorage }, REQ_READ_STORAGE);
            }
            //Llenar_Combo();
            //AsignarAnden();
            //Limpiar();
        }
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
            //Llenar_Combo();
            //AsignarAnden();
            //Limpiar();
        }
        protected override void OnDestroy()
        {
            // FIX #2: Liberar recursos que antes quedaban enganchados hasta el GC.
            try
            {
                if (Timer1 != null)
                {
                    Timer1.Enabled = false;
                    Timer1.Stop();
                    Timer1.Dispose();
                    Timer1 = null;
                }

                if (locationManager != null)
                {
                    locationManager.RemoveUpdates(this);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Android.Util.Log.Warn("CargaEmbarques", "OnDestroy cleanup: " + ex.Message);
            }
            finally
            {
                base.OnDestroy();
            }
        }

        // FIX #3: Handler de permisos diferenciado por requestCode.
        // Antes, los 3 permisos se pedían con requestCode=1, así que no había forma
        // de saber cuál se había denegado. Ahora cada uno tiene su código y se
        // reacciona de forma específica.
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            bool granted = grantResults != null
                           && grantResults.Length > 0
                           && grantResults[0] == Permission.Granted;

            switch (requestCode)
            {
                case REQ_LOCATION:
                    if (granted)
                    {
                        try
                        {
                            if (locationManager != null && !string.IsNullOrEmpty(locationProvider))
                            {
                                locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                            }
                        }
                        catch (Java.Lang.Exception ex)
                        {
                            Android.Util.Log.Warn("CargaEmbarques", "RequestLocationUpdates: " + ex.Message);
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Sin permiso de ubicación, no se registrarán coordenadas en la captura.", ToastLength.Long).Show();
                    }
                    break;

                case REQ_WRITE_STORAGE:
                    if (!granted)
                    {
                        Toast.MakeText(this, "Sin permiso de escritura, no se podrán guardar fotos del embarque.", ToastLength.Long).Show();
                    }
                    break;

                case REQ_READ_STORAGE:
                    if (!granted)
                    {
                        Toast.MakeText(this, "Sin permiso de lectura, no se podrán revisar fotos del embarque.", ToastLength.Long).Show();
                    }
                    break;

                default:
                    Android.Util.Log.Warn("CargaEmbarques", "OnRequestPermissionsResult requestCode desconocido: " + requestCode);
                    break;
            }
        }
        protected override void OnRestart()
        {
            base.OnRestart();
            //Limpiar();
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
            //email.To.Add(new MailAddress("gcamacho@mrlucky.com.mx"));
            email.From = new MailAddress("jgalvan@mrlucky.com.mx");
            //email.From = new MailAddress("dmunoz@mrlucky.com.mx");
            email.Subject = mAsunto; //"Mensaje de Prueba";
            email.Body = mBody;  //"Información de la factura";
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail1.mrlucky.com.mx";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new NetworkCredential("dmunoz", "GuIraSis003$1234");
            smtp.Credentials = new NetworkCredential("jgalvan", "mnK3a2aN@1|Q21VV");

            try
            {
                smtp.Send(email);
                email.Dispose();
                RunOnUiThread(() => Toast.MakeText(this, "correo enviado exitosamente\r\n", ToastLength.Short).Show());
            }
            catch (System.Exception ex)
            {

                RunOnUiThread(() => Toast.MakeText(this, "correo no enviado\r\n" + ex.ToString(), ToastLength.Short).Show());
            }
        }

        private void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);

            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);
            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }

        }

        void ILocationListener.OnLocationChanged(Location location)
        {
            //throw new NotImplementedException();
            currentLocation = location;

            currentLocation = location;

            if (currentLocation == null)
            {
                //Error Message  
            }
            else
            {
                latitud = currentLocation.Latitude.ToString();
                longitud = currentLocation.Longitude.ToString();
            }
        }

        void ILocationListener.OnProviderDisabled(string provider)
        {
            // FIX #1: No-op seguro. Antes lanzaba NotImplementedException → crash
            // cuando el usuario apagaba el GPS a media carga.
            Android.Util.Log.Warn("CargaEmbarques", "Location provider disabled: " + (provider ?? "(null)"));
        }

        void ILocationListener.OnProviderEnabled(string provider)
        {
            // FIX #1: No-op seguro.
            Android.Util.Log.Info("CargaEmbarques", "Location provider enabled: " + (provider ?? "(null)"));
        }

        void ILocationListener.OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            // FIX #1: No-op seguro. Antes lanzaba NotImplementedException → crash
            // cuando el proveedor cambiaba de estado (p.ej. GPS → Network).
            Android.Util.Log.Debug("CargaEmbarques", "Location provider '" + (provider ?? "(null)") + "' status=" + status);
        }

        #region NUEVA VALIDACION DE ETIQUETAS VERDES
        #region MÉTODOS AUXILIARES

        /// <summary>
        /// Ejecuta una consulta en tb_det_trazabilidad de forma segura.
        /// </summary>
        private (string Recibo, string Tarima, string ProdClave, string Tipo)? ObtenerDatosTrazabilidad(string campo, string valor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(valor)) return null;

                string query = $"SELECT recibo, tarima, prod_clave, tipo FROM tb_det_trazabilidad WHERE {campo} = @valor";

                using (SqlCommand cmd = new SqlCommand(query, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@valor", valor);

                    if (thisConnection.State == ConnectionState.Closed)
                        thisConnection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (
                                reader["recibo"].ToString().Trim(),
                                reader["tarima"].ToString().Trim(),
                                reader["prod_clave"].ToString().Trim(),
                                reader["tipo"].ToString().Trim()
                            );
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                //Console.WriteLine($"[ERROR ObtenerDatosTrazabilidad] {ex.Message}");
                Toast.MakeText(this, "Error al consultar datos de trazabilidad.", ToastLength.Long).Show();
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }

            return null;
        }

        /// <summary>
        /// Obtiene catálogo de productos activos (PTC o PTP).
        /// </summary>
        private DataTable ObtenerCatalogoProductos()
        {
            DataTable catalogo = new DataTable();

            try
            {
                string query = "SELECT prod_clave, prod_nombre " +
                               "FROM tb_cat_producto " +
                               "WHERE estatus = 'A' AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC') " +
                               "ORDER BY LEN(prod_clave) DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, thisConnection))
                {
                    if (thisConnection.State == ConnectionState.Closed)
                        thisConnection.Open();

                    da.Fill(catalogo);
                }
            }
            catch (System.Exception ex)
            {
                //Console.WriteLine($"[ERROR ObtenerCatalogoProductos] {ex.Message}");
                Toast.MakeText(this, "Error al cargar catálogo de productos.", ToastLength.Long).Show();
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }

            return catalogo;
        }

        #endregion

        #region VALIDAR ETIQUETA VERDE
        /// <summary>
        /// Analiza el texto de la etiqueta escaneada y devuelve los datos interpretados.
        /// </summary>
        /// <param name="textoEtiqueta">Texto leído del código escaneado.</param>
        /// <returns>Una tupla (Recibo, Tarima, ProdClave, Tipo) o null si no se reconoce.</returns>
        private (string Recibo, string Tarima, string ProdClave, string Tipo)? ProcesarEtiqueta(string codigoEtiqueta)
        {
            string V_Recibo = "", V_Prd = "", mtar = "", Mtipo = "", resultado = "";
            int V_Tamaño = codigoEtiqueta.Trim().Length;

            try
            {
                // PRIMERA VALIDACIÓN: INTENTAR ETIQUETA VERDE
                var datos = ValidarEtiquetaVerde(codigoEtiqueta.Trim());
                if (datos != null)
                {
                    if (datos.Value.Tarima.Length == 4)
                    {
                        resultado = datos.Value.Tarima.Substring(0, 2); // primeros 2 caracteres
                    }
                    else if (mtar.Length == 6)
                    {
                        resultado = datos.Value.Tarima.Substring(0, 3); // primeros 3 caracteres
                    }
                    else
                    {
                        resultado = datos.Value.Tarima;
                    }

                    return (datos.Value.Recibo, resultado, datos.Value.ProdClave, datos.Value.Tipo);
                }

                // CONEXIÓN BD
                if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }

                #region VALIDA LECTURA DE PTI FAMOUS
                if (codigoEtiqueta.Trim().Length == 12)
                {
                    string pti_famous = codigoEtiqueta.Trim().StartsWith("0")
                        ? codigoEtiqueta.TrimStart('0')
                        : codigoEtiqueta.Trim();

                    string querySSCC = "SELECT * FROM tb_det_trazabilidad WHERE pti_famous='" + pti_famous + "'";
                    using (SqlCommand sqlCommand = new SqlCommand(querySSCC, thisConnection))
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        if (sqlDataReader.Read())
                        {
                            V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                            mtar = sqlDataReader["tarima"].ToString().Trim();
                            V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                            Mtipo = sqlDataReader["tipo"].ToString().Trim();
                            return (V_Recibo, mtar, V_Prd, Mtipo);
                        }
                    }
                }
                #endregion

                #region VALIDA LECTURA DE SERIAL SHIPPING CONTAINER CODE
                string patron = @"00(\d+)";
                if (codigoEtiqueta.Trim().Contains("00"))
                {
                    Match match = Regex.Match(codigoEtiqueta.Trim(), patron);
                    if (match.Success)
                    {
                        string id_pallet = match.Groups[1].Value;
                        string querySSCC = "SELECT * FROM tb_det_trazabilidad WHERE id_Pallet='" + id_pallet + "'";
                        using (SqlCommand sqlCommand = new SqlCommand(querySSCC, thisConnection))
                        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (sqlDataReader.Read())
                            {
                                V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                                mtar = sqlDataReader["tarima"].ToString().Trim();
                                V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                                Mtipo = sqlDataReader["tipo"].ToString().Trim();
                                return (V_Recibo, mtar, V_Prd, Mtipo);
                            }
                        }
                    }
                }
                #endregion

                #region VALIDA LECTURA DE PTI CLAVE
                if (!Regex.IsMatch(codigoEtiqueta.Trim(), @"\s"))
                {
                    string querySSCC = "SELECT * FROM tb_det_trazabilidad WHERE pti_clave='" + codigoEtiqueta.Trim() + "'";
                    using (SqlCommand sqlCommand = new SqlCommand(querySSCC, thisConnection))
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        if (sqlDataReader.Read())
                        {
                            V_Recibo = sqlDataReader["recibo"].ToString().Trim();
                            mtar = sqlDataReader["tarima"].ToString().Trim();
                            V_Prd = sqlDataReader["prod_clave"].ToString().Trim();
                            Mtipo = sqlDataReader["tipo"].ToString().Trim();
                            return (V_Recibo, mtar, V_Prd, Mtipo);
                        }
                    }
                }
                #endregion

                #region VALIDA LECTURA DE ETIQUETA ANTERIOR
                if (codigoEtiqueta.Trim().Contains(" "))
                {
                    if (codigoEtiqueta.Trim().Length < 18)
                    {
                        mtar = codigoEtiqueta.Trim().Substring(codigoEtiqueta.Trim().Length - 3, 3);
                        V_Recibo = codigoEtiqueta.Trim().Substring(0, 5);
                        V_Prd = codigoEtiqueta.Trim().Replace(V_Recibo, "").Replace(mtar, "");
                        mtar = mtar.Replace(" ", "0");
                        Mtipo = "PTC";
                    }
                    else
                    {
                        mtar = codigoEtiqueta.Trim().Substring(codigoEtiqueta.Trim().Length - 3, 3);
                        V_Recibo = codigoEtiqueta.Trim().Substring(0, 6);
                        V_Prd = codigoEtiqueta.Trim().Replace(V_Recibo, "").Replace(mtar, "");
                        mtar = mtar.Replace(" ", "0");
                        Mtipo = "PTP";
                        if (V_Recibo.Substring(0, 1) == "0")
                        {
                            Mtipo = "PTC";
                            V_Recibo = Convert.ToInt32(V_Recibo).ToString();
                        }
                    }
                    return (V_Recibo, mtar, V_Prd.Trim(), Mtipo);
                }
                #endregion

                #region VALIDA LECTURA DE ETIQUETA POR DESCARTE
                int posstring = codigoEtiqueta.Trim().IndexOf(" ", 0);
                if (posstring > -1)
                {
                    DataTable CatalogodeProducto = new DataTable();
                    string cade = "SELECT prod_clave,prod_nombre FROM tb_cat_producto WHERE estatus='A' AND (prod_tipo='PTP' OR prod_tipo='PTC') ORDER BY LEN(prod_clave) DESC";
                    SqlDataAdapter da = new SqlDataAdapter(cade, thisConnection);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "CatalogodeProducto");
                    CatalogodeProducto = ds.Tables["CatalogodeProducto"];

                    for (int i = 0; i < CatalogodeProducto.Rows.Count; i++)
                    {
                        string producto_clave = CatalogodeProducto.Rows[i]["Prod_Clave"].ToString().Trim();
                        if (codigoEtiqueta.Trim().Contains(producto_clave))
                        {
                            V_Prd = producto_clave;
                            break;
                        }
                    }

                    string restocaptura = "";
                    int posprod = codigoEtiqueta.Trim().IndexOf(V_Prd);
                    V_Recibo = codigoEtiqueta.Trim().Substring(0, posprod).Trim();
                    if (V_Recibo.Length > 0 && V_Prd.Length > 0)
                    {
                        restocaptura = codigoEtiqueta.Trim().Replace(V_Recibo, "").Replace(V_Prd, "");
                    }
                    else
                    {
                        Toast.MakeText(this, "Por favor leer nuevamente la etiqueta.", ToastLength.Long).Show();
                    }

                    if (restocaptura.Length == 6)
                    {
                        Mtipo = "PTC";
                        mtar = restocaptura.Substring(0, 3);
                    }
                    else
                    {
                        Mtipo = "PTC";
                        mtar = restocaptura.Trim();
                    }
                }
                else
                {
                    int L_Cad = V_Tamaño - 9;
                    Mtipo = "PTP";
                    mtar = codigoEtiqueta.Trim().Substring(V_Tamaño - 3, 3);
                    V_Recibo = codigoEtiqueta.Trim().Substring(0, 6);
                    if (V_Recibo.Substring(0, 1) == "0")
                    {
                        Mtipo = "PTC";
                        V_Recibo = Convert.ToInt32(V_Recibo).ToString().Trim();
                    }
                    V_Prd = codigoEtiqueta.Trim().Substring(6, L_Cad);
                }
                #endregion
            }
            catch (Java.Lang.Exception ex)
            {
                Toast.MakeText(this, "Error al procesar etiqueta: " + ex.Message, ToastLength.Long).Show();
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
            }
            if (mtar.Length == 4)
            {
                resultado = mtar.Substring(0, 2); // primeros 2 caracteres
            }
            else if (mtar.Length == 6)
            {
                resultado = mtar.Substring(0, 3); // primeros 3 caracteres
            }

            mtar = resultado.Trim();

            return (V_Recibo, mtar, V_Prd, Mtipo);
        }


        #endregion

        private (string Recibo, string Tarima, string ProdClave, string Tipo)? ValidarEtiquetaVerde(string textoEtiqueta)
        {
            string resultado = "";
            try
            {
                if (string.IsNullOrWhiteSpace(textoEtiqueta))
                    return null;

                textoEtiqueta = textoEtiqueta.Trim();
                int longitud = textoEtiqueta.Length;

                if (longitud < 10)
                    return null;

                string V_Recibo = textoEtiqueta.Substring(0, 6);
                string V_Prd = "";
                string mtar = "";
                string Mtipo = "";
                bool coincidencia = false;

                // 🔹 Cargar catálogo (solo si no lo tienes cacheado)
                DataTable Catalogo = new DataTable();
                if (thisConnection.State == ConnectionState.Closed)
                    thisConnection.Open();

                string query = @"
            SELECT prod_clave, prod_tipo 
            FROM tb_cat_producto 
            WHERE estatus = 'A' 
              AND (prod_tipo = 'PTP' OR prod_tipo = 'PTC')
            ORDER BY LEN(prod_clave) DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(query, thisConnection))
                {
                    da.Fill(Catalogo);
                }

                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();

                // 🔹 Crear índice para búsquedas más rápidas
                var indice = Catalogo.AsEnumerable()
                    .ToDictionary(r => r["prod_clave"].ToString().Trim(),
                                  r => r["prod_tipo"].ToString().Trim());

                // 🔹 Generar posibles casos
                string ultimos4 = textoEtiqueta.Substring(longitud - 4);
                string ultimos3 = textoEtiqueta.Substring(longitud - 3);
                string restoCaso4 = textoEtiqueta.Substring(6, longitud - 10);
                string restoCaso3 = textoEtiqueta.Substring(6, longitud - 9);

                string claveCaso4 = "";
                string tipoCaso4 = "";
                string claveCaso3 = "";
                string tipoCaso3 = "";

                // 🔹 Buscar coincidencias más rápido
                foreach (var kvp in indice)
                {
                    string clave = kvp.Key;
                    string tipo = kvp.Value;

                    if (string.IsNullOrEmpty(claveCaso4) && restoCaso4.Contains(clave))
                    {
                        claveCaso4 = clave;
                        tipoCaso4 = tipo;
                    }

                    if (string.IsNullOrEmpty(claveCaso3) && restoCaso3.Contains(clave))
                    {
                        claveCaso3 = clave;
                        tipoCaso3 = tipo;
                    }

                    if (!string.IsNullOrEmpty(claveCaso4) && !string.IsNullOrEmpty(claveCaso3))
                        break; // ya se encontraron ambos
                }

                // 🔹 Elegir mejor coincidencia
                if (!string.IsNullOrEmpty(claveCaso4) && !string.IsNullOrEmpty(claveCaso3))
                {
                    if (claveCaso4.Length >= claveCaso3.Length)
                    {
                        V_Prd = claveCaso4;
                        mtar = ultimos4;
                        Mtipo = tipoCaso4;
                    }
                    else
                    {
                        V_Prd = claveCaso3;
                        mtar = ultimos3;
                        Mtipo = tipoCaso3;
                    }
                    coincidencia = true;
                }
                else if (!string.IsNullOrEmpty(claveCaso4))
                {
                    V_Prd = claveCaso4;
                    mtar = ultimos4;
                    Mtipo = tipoCaso4;
                    coincidencia = true;
                }
                else if (!string.IsNullOrEmpty(claveCaso3))
                {
                    V_Prd = claveCaso3;
                    mtar = ultimos3;
                    Mtipo = tipoCaso3;
                    coincidencia = true;
                }

                // 🔹 Si no hay coincidencias, usar detección básica
                if (!coincidencia)
                {
                    int L_Cad = longitud - 9;
                    Mtipo = "PTP";
                    mtar = textoEtiqueta.Substring(longitud - 3, 3);
                    V_Prd = textoEtiqueta.Substring(6, L_Cad);

                    if (V_Recibo.StartsWith("0"))
                    {
                        Mtipo = "PTC";
                        V_Recibo = Convert.ToInt32(V_Recibo).ToString();
                    }
                }
                if (mtar.Length == 4)
                {
                    resultado = mtar.Substring(0, 2); // primeros 2 caracteres
                }
                else if (mtar.Length == 6)
                {
                    resultado = mtar.Substring(0, 3); // primeros 3 caracteres
                }

                mtar = resultado.Trim();

                return (V_Recibo, mtar, V_Prd, Mtipo);
            }
            catch (Java.Lang.Exception ex)
            {
                Console.WriteLine($"[ERROR ValidarEtiquetaVerde] {ex.Message}");
                return null;
            }
        }

        public (string Recibo, string Tarima, string ProdClave, string Tipo)? Procesar(string codigo)
        {
            string V_Prd = "", V_Recibo = "", mtar = "";
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código no puede estar vacío.");

            // Detectar el bloque final de 3 o 4 dígitos (tarima)
            var match = Regex.Match(codigo, @"(\d{3,4})$");
            if (!match.Success)
                throw new ArgumentException("No se pudo identificar la tarima en el código.");

            mtar = match.Groups[1].Value;
            string parteSinTarima = codigo.Substring(0, codigo.Length - mtar.Length);

            // Folio siempre son los primeros 6 dígitos
            V_Recibo = parteSinTarima.Substring(0, 6);
            V_Prd = parteSinTarima.Substring(6);

            // Regla especial: si tarima tiene 4 dígitos y se repiten (ej: 0102 → 01)
            if (mtar.Length == 4 && mtar.Substring(0, 2) == mtar.Substring(2, 2))
                mtar = mtar.Substring(0, 2);

            // Casos tipo 0927, 0532, 1232 → también sólo primeros 2
            else if (mtar.Length == 4)
                mtar = mtar.Substring(0, 2);

            return (V_Recibo, mtar, V_Prd, Mtipo);
        }

        #region NUEVA VALIDACION 

        #endregion
        #endregion

        #region PEDIDOS CON OBSERVACIONES - HEB
        /// <summary>
        /// Verifica si existen pedidos con observaciones para el folio especificado y muestra alerta al usuario
        /// </summary>
        /// <param name="pdnFolio">Folio del pedido/embarque a verificar</param>
        private void VerificarObservacionesPedido(string pdnFolio)
        {
            try
            {
                // Lista para almacenar las observaciones encontradas
                var observacionesList = new List<ObservacionPedido>();

                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }

                // Consulta con parámetros para evitar inyección SQL
                string cadena = @"SELECT prod_clave, pdn_subcli, pdn_observaciones 
                         FROM tb_det_pedidos 
                         WHERE pdn_folio = @pdn_folio 
                         AND pdn_tipo = 'EXP' 
                         AND pdn_subcli LIKE '%HEB%'
                         AND (pdn_observaciones IS NOT NULL AND LTRIM(RTRIM(pdn_observaciones)) != '')";

                using (SqlCommand cmd = new SqlCommand(cadena, thisConnection))
                {
                    // Agregar parámetro para evitar inyección SQL
                    cmd.Parameters.AddWithValue("@pdn_folio", pdnFolio);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observacionesList.Add(new ObservacionPedido
                            {
                                Producto = reader["prod_clave"]?.ToString() ?? "N/A",
                                Cliente = reader["pdn_subcli"]?.ToString() ?? "N/A",
                                Observaciones = reader["pdn_observaciones"]?.ToString() ?? "Sin observaciones"
                            });
                        }
                    }
                }

                // Cerrar conexión
                if (thisConnection.State == ConnectionState.Open)
                {
                    thisConnection.Close();
                }

                // Si hay observaciones, mostrar el diálogo
                if (observacionesList.Count > 0)
                {
                    MostrarAlertaObservaciones(observacionesList);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                // Cerrar conexión en caso de error
                if (thisConnection.State == ConnectionState.Open)
                {
                    thisConnection.Close();
                }

                // Opcional: Log del error
                Android.Util.Log.Error("VerificarObservaciones", ex.Message);
            }
        }

        /// <summary>
        /// Muestra el AlertDialog con las observaciones formateadas
        /// </summary>
        private void MostrarAlertaObservaciones(List<ObservacionPedido> observaciones)
        {
            // Construir el mensaje formateado
            StringBuilder mensaje = new StringBuilder();

            foreach (var obs in observaciones)
            {
                // Formato: Producto | Cliente | Observaciones
                mensaje.Append($"<b>{obs.Producto}</b> | <font color='#FFA500'>{obs.Cliente}</font> | {obs.Observaciones}");
                mensaje.Append("<br/><br/>"); // Espacio entre registros
            }

            // Crear TextView personalizado para el contenido con formato HTML
            TextView textView = new TextView(this)
            {
                TextFormatted = Android.Text.Html.FromHtml(mensaje.ToString(), Android.Text.FromHtmlOptions.ModeLegacy),
                TextSize = 14
            };
            textView.SetPadding(50, 40, 50, 40);
            textView.SetTextColor(Android.Graphics.Color.ParseColor("#333333"));

            // Crear ScrollView para contenido largo
            ScrollView scrollView = new ScrollView(this);
            scrollView.AddView(textView);

            // Crear el AlertDialog con estilo personalizado
            AlertDialog.Builder builder = new AlertDialog.Builder(this, Android.Resource.Style.ThemeMaterialDialogAlert);

            builder.SetTitle("Pedido con Observaciones")
                   .SetIcon(Resource.Drawable.warning) // Usa tu icono de warning existente
                   .SetView(scrollView)
                   .SetPositiveButton("Entendido", (sender, e) =>
                   {
                       // Acción al aceptar
                       ((AlertDialog)sender).Dismiss();
                   })
                   .SetCancelable(false); // No permitir cerrar tocando fuera

            AlertDialog dialog = builder.Create();

            // Personalizar colores después de crear el diálogo
            dialog.Show();

            // Personalizar el botón positivo (color verde GAB)
            var positiveButton = dialog.GetButton((int)DialogButtonType.Positive);
            if (positiveButton != null)
            {
                positiveButton.SetTextColor(Android.Graphics.Color.ParseColor("#2E7D32")); // Verde corporativo
                positiveButton.SetBackgroundResource(Resource.Drawable.buttonAceptar);
            }

            // Personalizar el título
            var titleView = dialog.FindViewById(Android.Resource.Id.Title);
            if (titleView is TextView titleTextView)
            {
                titleTextView.SetTextColor(Android.Graphics.Color.ParseColor("#D32F2F")); // Rojo alerta
                titleTextView.TextAlignment = Android.Views.TextAlignment.Center;
            }
        }

        /// <summary>
        /// Clase modelo para las observaciones de pedido
        /// </summary>
        private class ObservacionPedido
        {
            public string Producto { get; set; }
            public string Cliente { get; set; }
            public string Observaciones { get; set; }
        }
        #endregion

        #region CERRAR EMBARQUE CON OBSERVACIONES
        /// <summary>
        /// Evalúa si el trailer lleva más de 6 horas en planta o la hora actual
        /// es posterior a las 12:00 a.m. Si alguna condición se cumple, muestra
        /// el AlertDialog "Cerrar Embarque con Observaciones" con un Spinner
        /// cargado desde tb_cat_observaciones. Al confirmar, invoca <paramref name="onGuardar"/>
        /// con la observación seleccionada; si ninguna condición aplica, invoca
        /// directamente el callback sin mostrar el diálogo.
        /// </summary>
        /// <param name="folioEmbarque">Folio actual del embarque.</param>
        /// <param name="onGuardar">
        ///     Callback que recibe la observación (vacía si no aplica) y
        ///     ejecuta el resto de la lógica de guardado.
        /// </param>
        private void MostrarDialogoObservacionesSiAplica(string folioEmbarque, Action<string> onGuardar)
        {
            bool masde6Horas = false;
            bool despues12am = false;

            // ── Condición 2: hora actual entre 00:00 y 05:59 (madrugada) ─────────
            int horaActual = DateTime.Now.Hour;
            despues12am = (horaActual >= 0 && horaActual < 6);

            // ── Condición 1: más de 6 horas en planta (desde registro de vigilancia) ────────────────────────────
            try
            {
                if (thisConnection.State == ConnectionState.Closed)
                    thisConnection.Open();

                // Se une tb_mstr_embarque con tb_mstr_trailer via pdn_folio = emb_folio
                // y se toma HoraRegVig que es char(5) en formato HH:mm
                string sqlHoraVig =
                    @"SELECT TOP 1 t.HoraRegVig
FROM tb_mstr_trailer t
INNER JOIN tb_mstr_embarque e 
    ON CAST(e.emb_folio AS INT) = CAST(t.pdn_folio AS INT) -- Convertimos ambos a enteros
WHERE CAST(e.emb_folio AS INT) = @folio
  AND t.HoraRegVig IS NOT NULL
  AND LTRIM(RTRIM(t.HoraRegVig)) != ''
ORDER BY t.fecha DESC";

                using (SqlCommand cmd = new SqlCommand(sqlHoraVig, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@folio", folioEmbarque);
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                    {
                        string horaStr = resultado.ToString().Trim();

                        // HoraRegVig es char(5) → formato "HH:mm" (ej. "08:30", "23:45")
                        bool parsed = DateTime.TryParseExact(
                            horaStr,
                            new[] { "HH:mm", "H:mm" },
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime horaEntrada
                        );

                        if (parsed)
                        {
                            // hora_ini solo tiene hora/minuto → combinar con la fecha de hoy
                            DateTime ahora = DateTime.Now;
                            DateTime entradaHoy = new DateTime(
                                ahora.Year, ahora.Month, ahora.Day,
                                horaEntrada.Hour, horaEntrada.Minute, 0
                            );

                            // Si la hora de entrada es mayor que ahora, el trailer
                            // entró el día anterior (cruzó medianoche)
                            if (entradaHoy > ahora)
                                entradaHoy = entradaHoy.AddDays(-1);

                            double horasEnPlanta = (ahora - entradaHoy).TotalHours;
                            masde6Horas = horasEnPlanta > 6;

                            Android.Util.Log.Debug("CargaEmbarques",
                                $"hora_ini='{horaStr}' → entrada={entradaHoy:HH:mm} | " +
                                $"ahora={ahora:HH:mm} | horas={horasEnPlanta:F1} | >6h={masde6Horas}");
                        }
                        else
                        {
                            // Parse fallido → NO activar la condición (evita falsos positivos)
                            Android.Util.Log.Warn("CargaEmbarques",
                                $"No se pudo parsear hora_ini: '{horaStr}' — condición >6h ignorada.");
                        }
                    }
                    else
                    {
                        // No se encontró registro de vigilancia para este folio
                        Android.Util.Log.Warn("CargaEmbarques",
                            $"Sin registro de vigilancia para folio '{folioEmbarque}' — condición >6h ignorada.");
                    }
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Toast.MakeText(this, "Error al verificar hora de entrada: " + ex.Message,
                               ToastLength.Long).Show();
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }

            // ── Si ninguna condición aplica → guardar directo ─────────────────────
            if (!masde6Horas && !despues12am)
            {
                onGuardar?.Invoke(string.Empty);
                return;
            }

            // ── Cargar catálogo de observaciones desde SQL Server ─────────────────
            var listaObservaciones = new List<string>();

            try
            {
                if (thisConnection.State == ConnectionState.Closed)
                    thisConnection.Open();

                string sqlObs =
                    @"SELECT obs_descripcion 
              FROM tb_cat_observaciones
              WHERE obs_activo = 'S'
                AND (obs_sistema = 'CargaEmbarques' OR obs_sistema IS NULL)
              ORDER BY obs_descripcion";

                using (SqlCommand cmd = new SqlCommand(sqlObs, thisConnection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        listaObservaciones.Add(reader["obs_descripcion"].ToString().Trim());
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Toast.MakeText(this, "Error al cargar observaciones: " + ex.Message,
                               ToastLength.Long).Show();
            }
            finally
            {
                if (thisConnection.State == ConnectionState.Open)
                    thisConnection.Close();
            }

            // ── Construir la vista del diálogo ────────────────────────────────────
            LinearLayout layout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            layout.SetPadding(48, 24, 48, 8);

            // Mensaje descriptivo según la condición detectada
            string motivoCondicion = (masde6Horas && despues12am)
                ? "El trailer lleva más de 6 horas en planta y la salida es después de las 12:00 a.m."
                : masde6Horas
                    ? "El trailer lleva más de 6 horas en planta."
                    : "La salida del trailer es después de las 12:00 a.m.";

            TextView tvMensaje = new TextView(this);
            tvMensaje.Text = motivoCondicion;
            tvMensaje.TextSize = 14f;
            tvMensaje.SetTextColor(Android.Graphics.Color.ParseColor("#333333"));
            tvMensaje.SetPadding(0, 0, 0, 20);
            layout.AddView(tvMensaje);

            TextView tvLabel = new TextView(this);
            tvLabel.Text = "Razón de la observación:";
            tvLabel.TextSize = 14f;
            tvLabel.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
            layout.AddView(tvLabel);

            Spinner spinner = new Spinner(this);
            var adapter = new ArrayAdapter<string>(
                this,
                Android.Resource.Layout.SimpleSpinnerDropDownItem,
                listaObservaciones.Count > 0
                    ? listaObservaciones
                    : new List<string> { "(Sin opciones disponibles)" }
            );
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            layout.AddView(spinner);

            // ── Mostrar AlertDialog ───────────────────────────────────────────────
            AlertDialog.Builder dialogObs = new AlertDialog.Builder(this);
            dialogObs.SetTitle(
                Html.FromHtml("<font color='#DF0101' size='10'><b>CERRAR EMBARQUE CON OBSERVACIONES</b></font>")
            );
            dialogObs.SetIcon(Resource.Drawable.Info);
            dialogObs.SetCancelable(false);
            dialogObs.SetView(layout);

            dialogObs.SetPositiveButton(
                Html.FromHtml("<font color='#DF0101'>Aceptar</font>"),
                (senderObs, argsObs) =>
                {
                    if (listaObservaciones.Count == 0)
                    {
                        Toast.MakeText(this, "No hay observaciones disponibles en el catálogo.",
                                       ToastLength.Long).Show();
                        return;
                    }

                    string observacionSeleccionada = spinner.SelectedItem?.ToString() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(observacionSeleccionada))
                    {
                        Toast.MakeText(this, "Debe seleccionar una observación antes de continuar.",
                                       ToastLength.Long).Show();
                        return;
                    }

                    onGuardar?.Invoke(observacionSeleccionada);
                }
            );

            dialogObs.SetNegativeButton(
                Html.FromHtml("<font color='#DF0101'>Cancelar</font>"),
                (senderObs, argsObs) => { /* el usuario canceló, no se guarda nada */ }
            );

            dialogObs.Show();
        }
        #endregion


        #region ATU
        #region ATU Legacy Password Dialog
        private void MostrarDialogoPasswordLegacy(
            string motivo,
            string folioLeido, string fechaLeido,
            string folioAtrasado, string fechaAtrasada,
            string productocve, string producto,
            string cajasDisp, string tarimaLeido,
            string tarimaAtrasada)
        {
            View view = LayoutInflater.Inflate(Resource.Layout.AutorizarFolios, null);
            AlertDialog builder = new AlertDialog.Builder(this).Create();
            builder.SetView(view);
            builder.SetCanceledOnTouchOutside(false);
            builder.SetCancelable(false);

            password = view.FindViewById<EditText>(Resource.Id.passwordAutoriza);
            password.LongClickable = false;

            Spinner motivoautoriza = view.FindViewById<Spinner>(Resource.Id.motivoautoriza);
            System.Collections.ArrayList listaFrutas2 = new System.Collections.ArrayList();

            string[] camotivosrs = { "Folio Adelantado Requerido Por Cliente", "Folio Adelantado Caja Inexistente", "Folio Adelantado Caja No Encontrada", "Folio Adelantado No Apto Para Carga" };

            Collections.AddAll(listaFrutas2, camotivosrs);
            comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, camotivosrs);
            motivoautoriza.Adapter = comboAdapter;
            motivoautoriza.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(motivoautoriza_ItemSelected2);

            password.LongClickable = false;
            password.RequestFocus();
            InputMethodManager immD = (InputMethodManager)GetSystemService(Context.InputMethodService);
            immD.ShowSoftInput(password, ShowFlags.Implicit);

            Button buttonaceptar = view.FindViewById<Button>(Resource.Id.CargarTarima);
            Button button = view.FindViewById<Button>(Resource.Id.CanCarTar);

            button.Click += delegate
            {
                Borrar();
                password.Text = "";
                builder.Dismiss();
                return;
            };

            string OK = "N";
            buttonaceptar.Click += delegate
            {
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }

                string cadena = "Select usuario From tb_Autoriza_OdeP Where password = '" + password.Text.Trim().ToUpper() + "' AND clave = 'EM' AND obs = 'Autoriza Caducidad'";
                SqlCommand cmd = new SqlCommand(cadena, thisConnection);
                mAutoriza = Convert.ToString(cmd.ExecuteScalar());

                if (mAutoriza.Trim().Length > 0)
                {
                    if (thisConnection.State == ConnectionState.Open)
                    {
                        thisConnection.Close();
                    }
                    OK = "S";
                    AutoPed = "S";
                }
                else
                {
                    Toast.MakeText(this, "PASSWORD INCORRECTO!!!", ToastLength.Short).Show();
                    password.Text = "";
                    password.RequestFocus();
                    thisConnection.Close();
                }

                if (mAutoriza.Trim() == "USER X")
                {
                    if (thisConnection.State == ConnectionState.Closed)
                    {
                        thisConnection.Open();
                    }

                    cadena = "SELECT CASE When (SELECT DATENAME(dw, GETDATE())) = 'Domingo' THEN '1' WHEN ((SELECT CONVERT(datetime, (SELECT Convert(varchar(8),GetDate(), 108) hora), 108)) < (SELECT Convert(datetime,'07:00:00', 108) HoraServidor)) OR ((SELECT CONVERT(datetime, (SELECT Convert(varchar(8),GetDate(), 108) hora), 108)) > (SELECT Convert(datetime,'22:00:00', 108) HoraServidor)) THEN '1' WHEN ((SELECT CONVERT(datetime, (SELECT Convert(varchar(8),GetDate(), 108) hora), 108)) > (SELECT Convert(datetime,'10:24:00', 108) HoraServidor)) AND ((SELECT CONVERT(datetime, (SELECT Convert(varchar(8),GetDate(), 108) hora), 108)) < (SELECT Convert(datetime,'11:06:00', 108) HoraServidor)) THEN '1' WHEN ((SELECT CONVERT(datetime, (SELECT Convert(varchar(8),GetDate(), 108) hora), 108)) > (SELECT Convert(datetime,'17:54:00', 108) HoraServidor)) AND ((SELECT CONVERT(datetime, (SELECT Convert(varchar(8),GetDate(), 108) hora), 108)) < (SELECT Convert(datetime,'18:36:00', 108) HoraServidor)) THEN '1' ELSE '2' END";
                    cmd = new SqlCommand(cadena, thisConnection);
                    string dia = Convert.ToString(cmd.ExecuteScalar());

                    if (thisConnection.State == ConnectionState.Open)
                    {
                        thisConnection.Close();
                    }

                    if (dia.Trim() == "1")
                    {
                        OK = "S";
                    }
                    else
                    {
                        OK = "NS";
                    }
                }

                if (OK == "N")
                {
                    Toast.MakeText(this, "PASSWORD INCORRECTO!!!", ToastLength.Short).Show();
                    password.Text = "";
                    password.RequestFocus();
                }
                else if (OK == "NS")
                {
                    Toast.MakeText(this, "El USUARIO X No esta habilitado para autorizar a esta hora, la hora de autorizacion es de 10:00 PM a 07:00 AM De Lunes A Sabado y Domingos Todo el dia", ToastLength.Long).Show();
                    password.Text = "";
                    password.RequestFocus();
                }
                else
                {
                    string responsableAjuste = responsable.Trim().Length > 25 ? responsable.Trim().Substring(0, 25) : responsable.Trim();
                    ConsultaInserFolioAdelantado = "insert into tb_det_folio_adelantado (responsable, fecha, emb_folio, recibo_cap, fecreccap, recibo_sug, fecrecsug, prod_clave, producto, cantidad, autorizo, tarimacap, tarimasug, imei, motivo, fechareal) values ('" + responsableAjuste.Trim() + "','" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "','" + pedido.Text + "', '" + folioLeido.Trim() + "', '" + fechaLeido + "','" + folioAtrasado + "', '" + fechaAtrasada + "', '" + productocve + "', '" + producto + "', '" + cajasDisp + "', '" + mAutoriza.Trim() + "', '" + tarimaLeido + "', '" + tarimaAtrasada.Trim() + "', '" + imei.Trim() + "', '" + motivoautorizafechaadelantada.Trim() + "', GETDATE())";
                    builder.Dismiss();
                }
            };

            builder.Show();
        }
        #endregion
        #endregion

        #region ATU FUSIONADO (Robusto + Diseño XML)

        #endregion

        // ── PASO 1: Mostrar dialog de MOTIVO antes de enviar la solicitud ─────────────
        // Llama esto DONDE ANTES mostraba el dialog de contraseña (botón "Autorizar"):
        private void IniciarFlujoCargaATU(
    string folioLeido, string fechaLeido,
    string folioAtrasado, string fechaAtrasada,
    string productocve, string producto,
    string cajasDisp, string tarimaLeido,
    string tarimaAtrasada)
        {
            var motivos = new[]
            {
            "Folio Adelantado Requerido Por Cliente",
            "Folio Adelantado Caja Inexistente",
            "Folio Adelantado Caja No Encontrada",
            "Folio Adelantado No Apto Para Carga"
        };
            string motivoSeleccionado = motivos[0];

            // ✅ MANDAMIENTO 1: Verificar que el XML exista antes de inflarlo
            var inflater = LayoutInflater.From(this);
            if (inflater == null) { Toast.MakeText(this, "Error de sistema", ToastLength.Long).Show(); return; }

            var viewMotivo = inflater.Inflate(Resource.Layout.dialog_atu_motivo, null);
            if (viewMotivo == null) { Toast.MakeText(this, "Error: Layout dialog_atu_motivo no encontrado", ToastLength.Long).Show(); return; }

            var spinnerMotivo = viewMotivo.FindViewById<Spinner>(Resource.Id.spinnerMotivo);
            if (spinnerMotivo == null) { Toast.MakeText(this, "Error: Falta ID spinnerMotivo en el XML", ToastLength.Long).Show(); return; }

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, motivos);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMotivo.Adapter = adapter;
            spinnerMotivo.ItemSelected += (s, e) => motivoSeleccionado = motivos[e.Position];

            new Android.App.AlertDialog.Builder(this)
                .SetTitle("Motivo del folio adelantado")
                .SetView(viewMotivo)
                .SetCancelable(false)
                .SetPositiveButton("Continuar", (s, e) =>
                {
                    MostrarDialogoATU(motivoSeleccionado, folioLeido, fechaLeido, folioAtrasado, fechaAtrasada, productocve, producto, cajasDisp, tarimaLeido, tarimaAtrasada);
                })
                .SetNegativeButton("Cancelar", (s, e) => Borrar())
                .Show();
        }

        private void MostrarDialogoATU(
            string motivo, string folioLeido, string fechaLeido,
            string folioAtrasado, string fechaAtrasada,
            string productocve, string producto,
            string cajasDisp, string tarimaLeido, string tarimaAtrasada)
        {
            Task.Run(async () =>
            {
                bool servidorOk = await _atuService.ServidorDisponibleAsync();
                RunOnUiThread(() =>
                {
                    if (!servidorOk)
                    {
                        MostrarDialogoPasswordLegacy(motivo, folioLeido, fechaLeido, folioAtrasado, fechaAtrasada, productocve, producto, cajasDisp, tarimaLeido, tarimaAtrasada);
                        return;
                    }
                    EjecutarFlujoOTP(motivo, folioLeido, fechaLeido, folioAtrasado, fechaAtrasada, productocve, producto, cajasDisp, tarimaLeido, tarimaAtrasada);
                });
            });
        }

        private void EjecutarFlujoOTP(
            string motivo, string folioLeido, string fechaLeido,
            string folioAtrasado, string fechaAtrasada,
            string productocve, string producto,
            string cajasDisp, string tarimaLeido, string tarimaAtrasada)
        {
            var responsableCorto = responsable.Trim().Length > 25 ? responsable.Trim()[..25] : responsable.Trim();

            Task.Run(async () => await _atuService.CrearSolicitudAsync(
                embFolio: pedido.Text.Trim(), reciboCap: folioLeido.Trim(), reciboSug: folioAtrasado.Trim(),
                fechaRecCap: fechaLeido, fechaRecSug: fechaAtrasada, prodClave: productocve.Trim(),
                producto: producto.Trim(), cantidad: cajasDisp.Trim(), tarimaCap: tarimaLeido.Trim(),
                tarimaSug: tarimaAtrasada.Trim(), responsable: responsableCorto, motivo: motivo, imei: imei.Trim()));

            var inflater = LayoutInflater.From(this);
            var view = inflater.Inflate(Resource.Layout.dialog_atu_otp, null);

            // ✅ MANDAMIENTO 1: Check estricto de IDs sin usar !
            if (view == null) { Toast.MakeText(this, "Error: No se encontró dialog_atu_otp.xml", ToastLength.Long).Show(); return; }

            var lblInfo = view.FindViewById<TextView>(Resource.Id.lblAtuInfo);
            //var txtSupervisor = view.FindViewById<EditText>(Resource.Id.NombreSupervisor);
            var txtSupervisor = MainActivity.responsablesplit?.Trim() ?? "";
            var txtOTP = view.FindViewById<EditText>(Resource.Id.txtAtuOtp);
            var lblEstado = view.FindViewById<TextView>(Resource.Id.lblAtuEstado);

            if (txtSupervisor == null || txtOTP == null || lblEstado == null)
            {
                Toast.MakeText(this, "Error crítico: Faltan IDs (NombreSupervisor/txtAtuOtp/lblAtuEstado) en dialog_atu_otp.xml", ToastLength.Long).Show();
                return;
            }

            lblInfo.Text = $"Folio: {pedido.Text.Trim()}\nProducto: {productocve} — {producto}\nRecibo: {folioLeido}  |  Tarima: {tarimaLeido}\n\nEspera el código OTP del supervisor.";
            txtOTP.InputType = Android.Text.InputTypes.ClassNumber;

            var builder = new Android.App.AlertDialog.Builder(this);
            builder.SetView(view);
            builder.SetCancelable(false);

            Android.App.AlertDialog dialog = null;
            builder.SetPositiveButton("✓ VALIDAR OTP", (Android.Content.IDialogInterfaceOnClickListener)null);
            builder.SetNegativeButton("✕ CANCELAR", (s, e) => { Borrar(); dialog?.Dismiss(); });

            dialog = builder.Create();
            dialog.Show();

            var btnValidar = dialog.GetButton((int)Android.Content.DialogButtonType.Positive);
            if (btnValidar == null) return; // Seguro extremo

            btnValidar.Click += async (s, e) =>
            {
                try
                {
                    //var supId = txtSupervisor.Text.Trim();
                    var supId = txtSupervisor.Trim();
                    var otp = txtOTP.Text.Trim();

                    if (string.IsNullOrEmpty(supId)) { lblEstado.Text = "⚠️ Ingresa el No. de Empleado"; lblEstado.SetTextColor(Android.Graphics.Color.Orange); return; }
                    if (otp.Length != 6) { lblEstado.Text = "⚠️ El OTP deben ser 6 dígitos"; lblEstado.SetTextColor(Android.Graphics.Color.Orange); return; }

                    btnValidar.Enabled = false;
                    lblEstado.Text = "⏳ Validando con el servidor...";
                    lblEstado.SetTextColor(Android.Graphics.Color.Gray);

                    // ✅ MANDAMIENTO 2: Se envía el IMEI como DeviceFingerprint (El backend lo exige)
                    var resultado = await _atuService.ValidarOTPAsync(
                        otp: otp,
                        embFolio: pedido.Text.Trim(),
                        prodClave: productocve.Trim(),
                        reciboCap: folioAtrasado.Trim(),
                        tarimaCap: tarimaAtrasada.Trim(),
                        responsable: supId);

                    bool isValid = resultado.IsValid;
                    string supNombre = resultado.SupervisorId ?? "";
                    string mensaje = resultado.Mensaje ?? "";

                    if (isValid)
                    {
                        mAutoriza = !string.IsNullOrEmpty(supNombre) ? supNombre.Trim() : supId;
                        ConsultaInserFolioAdelantado = "insert into tb_det_folio_adelantado (responsable, fecha, emb_folio, recibo_cap, fecreccap, recibo_sug, fecrecsug, prod_clave, producto, cantidad, autorizo, tarimacap, tarimasug, imei, motivo, fechareal) values ('" + responsableCorto + "','" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "','" + pedido.Text.Trim() + "', '" + folioLeido.Trim() + "', '" + fechaLeido + "','" + folioAtrasado + "', '" + fechaAtrasada + "', '" + productocve + "', '" + producto + "', '" + cajasDisp + "', '" + mAutoriza + "', '" + tarimaLeido + "', '" + tarimaAtrasada.Trim() + "', '" + imei.Trim() + "', '" + motivo + "', GETDATE())";

                        lblEstado.Text = "✓ Autorización válida";
                        lblEstado.SetTextColor(Android.Graphics.Color.ParseColor("#00AA44"));
                        await Task.Delay(1000);
                        dialog.Dismiss();
                    }
                    else
                    {
                        btnValidar.Enabled = true;
                        if (mensaje.Contains("FRAUDE"))
                        {
                            new Android.App.AlertDialog.Builder(this).SetTitle("🚨 FRAUDE DETECTADO").SetMessage($"El intento queda registrado.\n\n{mensaje}").SetNeutralButton("ENTENDIDO", (s2, e2) => { }).SetCancelable(false).Show();
                        }
                        lblEstado.Text = mensaje.Contains("FRAUDE") ? $"🔴 {mensaje}" : $"❌ {mensaje}";
                        lblEstado.SetTextColor(Android.Graphics.Color.Red);
                        txtOTP.Text = "";
                        txtOTP.RequestFocus();
                    }
                }
                catch (Java.Lang.Exception exGeneral)
                {
                    btnValidar.Enabled = true;
                    lblEstado.Text = $"❌ Error: {exGeneral.Message}";
                    lblEstado.SetTextColor(Android.Graphics.Color.Red);
                }
            };
        }
        private void IniciarFlujoCargaATULEGACY(
            string folioLeido, string fechaLeido,
            string folioAtrasado, string fechaAtrasada,
            string productocve, string producto,
            string cajasDisp, string tarimaLeido,
            string tarimaAtrasada)
        {
            // Spinner/picker de motivos (igual que el original)
            var motivos = new[]
            {
        "Folio Adelantado Requerido Por Cliente",
        "Folio Adelantado Caja Inexistente",
        "Folio Adelantado Caja No Encontrada",
        "Folio Adelantado No Apto Para Carga"
    };

            string motivoSeleccionado = motivos[0];

            var inflater = LayoutInflater.From(this)!;
            var viewMotivo = inflater.Inflate(Resource.Layout.dialog_atu_motivo, null)!;

            var spinnerMotivo = viewMotivo.FindViewById<Spinner>(Resource.Id.spinnerMotivo)!;
            var adapter = new ArrayAdapter<string>(this,
                Android.Resource.Layout.SimpleSpinnerItem, motivos);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMotivo.Adapter = adapter;
            spinnerMotivo.ItemSelected += (s, e) => motivoSeleccionado = motivos[e.Position];

            new Android.App.AlertDialog.Builder(this)
                .SetTitle("Motivo del folio adelantado")
                .SetView(viewMotivo)
                .SetCancelable(false)
                .SetPositiveButton("Continuar", (s, e) =>
                {
                    // PASO 2: Mostrar el dialog de OTP con el motivo ya seleccionado
                    MostrarDialogoATU(
                        motivoSeleccionado,
                        folioLeido, fechaLeido,
                        folioAtrasado, fechaAtrasada,
                        productocve, producto,
                        cajasDisp, tarimaLeido, tarimaAtrasada);
                })
                .SetNegativeButton("Cancelar", (s, e) => Borrar())
                .Show();
        }
        private void MostrarDialogoATULEGACY(
    string motivo,
    string folioLeido, string fechaLeido,
    string folioAtrasado, string fechaAtrasada,
    string productocve, string producto,
    string cajasDisp, string tarimaLeido,
    string tarimaAtrasada)
        {
            // 1. Verificación previa de disponibilidad (Fallback del Bloque 2)
            Task.Run(async () =>
            {
                bool servidorOk = await _atuService.ServidorDisponibleAsync();

                RunOnUiThread(() =>
                {
                    if (!servidorOk)
                    {
                        // Si el servidor no responde, usamos el método de contraseña anterior
                        MostrarDialogoPasswordLegacy(motivo, folioLeido, fechaLeido,
                            folioAtrasado, fechaAtrasada, productocve, producto,
                            cajasDisp, tarimaLeido, tarimaAtrasada);
                        return;
                    }

                    // Si el servidor está OK, procedemos con el flujo OTP
                    EjecutarFlujoOTP(motivo, folioLeido, fechaLeido, folioAtrasado, fechaAtrasada,
                                     productocve, producto, cajasDisp, tarimaLeido, tarimaAtrasada);
                });
            });
        }

        private void EjecutarFlujoOTPLEGACY(
            string motivo,
            string folioLeido, string fechaLeido,
            string folioAtrasado, string fechaAtrasada,
            string productocve, string producto,
            string cajasDisp, string tarimaLeido,
            string tarimaAtrasada)
        {
            // Preparación de datos (Lógica Bloque 1)
            var responsableCorto = responsable.Trim().Length > 25
                ? responsable.Trim()[..25]
                : responsable.Trim();

            // Notificar al backend en segundo plano de inmediato
            Task.Run(async () => await _atuService.CrearSolicitudAsync(
                embFolio: pedido.Text.Trim(),
                reciboCap: folioLeido.Trim(),
                reciboSug: folioAtrasado.Trim(),
                fechaRecCap: fechaLeido,
                fechaRecSug: fechaAtrasada,
                prodClave: productocve.Trim(),
                producto: producto.Trim(),
                cantidad: cajasDisp.Trim(),
                tarimaCap: tarimaLeido.Trim(),
                tarimaSug: tarimaAtrasada.Trim(),
                responsable: responsableCorto,
                motivo: motivo,
                imei: imei.Trim()));

            // Inflar Interfaz XML
            var inflater = LayoutInflater.From(this)!;
            var view = inflater.Inflate(Resource.Layout.dialog_atu_otp, null)!;

            var lblInfo = view.FindViewById<TextView>(Resource.Id.lblAtuInfo)!;
            var txtSupervisor = view.FindViewById<EditText>(Resource.Id.NombreSupervisor)!;
            var txtOTP = view.FindViewById<EditText>(Resource.Id.txtAtuOtp)!;
            var lblEstado = view.FindViewById<TextView>(Resource.Id.lblAtuEstado)!;

            // Texto de ayuda detallado (Combinado Bloque 1 + 2)
            lblInfo.Text =
                $"Folio: {pedido.Text.Trim()}\n" +
                $"Producto: {productocve} — {producto}\n" +
                $"Recibo: {folioAtrasado}  |  Tarima: {tarimaAtrasada}\n\n" +
                $"Espera el código OTP del supervisor de cámaras frías.\n" +
                $"El supervisor recibirá la solicitud en su celular.";

            txtOTP.InputType = Android.Text.InputTypes.ClassNumber;

            var builder = new Android.App.AlertDialog.Builder(this);
            //builder.SetTitle("🔐 Autorización ATU");
            builder.SetView(view);
            builder.SetCancelable(false);

            Android.App.AlertDialog? dialog = null;

            builder.SetPositiveButton("✓ VALIDAR OTP", (Android.Content.IDialogInterfaceOnClickListener?)null);
            builder.SetNegativeButton("✕ CANCELAR", (s, e) =>
            {
                Borrar();
                dialog?.Dismiss(); // Cierre explícito del Bloque 1
            });

            dialog = builder.Create()!;
            dialog.Show();

            // Lógica de Validación con Override
            var btnValidar = dialog.GetButton((int)Android.Content.DialogButtonType.Positive)!;
            btnValidar.Click += async (s, e) =>
            {
                try // ✅ ESCUDO PRINCIPAL
                {
                    var supId = txtSupervisor.Text.Trim();
                    var otp = txtOTP.Text.Trim();

                    if (string.IsNullOrEmpty(supId))
                    {
                        lblEstado.Text = "⚠️ Ingresa el No. de Empleado";
                        lblEstado.SetTextColor(Android.Graphics.Color.Orange);
                        return;
                    }
                    if (otp.Length != 6)
                    {
                        lblEstado.Text = "⚠️ El OTP deben ser 6 dígitos";
                        lblEstado.SetTextColor(Android.Graphics.Color.Orange);
                        return;
                    }

                    btnValidar.Enabled = false;
                    lblEstado.Text = "⏳ Validando con el servidor...";
                    lblEstado.SetTextColor(Android.Graphics.Color.Gray);

                    // ✅ ESCUDO SECUNDARIO (por si la red falla o el servidor explota)
                    bool isValid = false;
                    string supNombre = "";
                    string mensaje = "";

                    try
                    {
                        var resultado = await _atuService.ValidarOTPAsync(
                            otp: otp,
                            embFolio: pedido.Text.Trim(),
                            prodClave: productocve.Trim(),
                            reciboCap: folioAtrasado.Trim(),
                            tarimaCap: tarimaAtrasada.Trim(),
                            responsable: supId);

                        isValid = resultado.IsValid;
                        supNombre = resultado.SupervisorId;
                        mensaje = resultado.Mensaje;
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        isValid = false;
                        mensaje = $"Error de red o servidor: {ex.Message}";
                    }

                    if (isValid)
                    {
                        mAutoriza = !string.IsNullOrEmpty(supNombre) ? supNombre.Trim() : supId;

                        ConsultaInserFolioAdelantado =
                            "insert into tb_det_folio_adelantado " +
                            "(responsable, fecha, emb_folio, recibo_cap, fecreccap, " +
                            "recibo_sug, fecrecsug, prod_clave, producto, cantidad, " +
                            "autorizo, tarimacap, tarimasug, imei, motivo, fechareal) " +
                            "values ('" + responsableCorto + "','" +
                            DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "','" +
                            pedido.Text.Trim() + "', '" + folioLeido.Trim() + "', '" + fechaLeido + "','" +
                            folioAtrasado + "', '" + fechaAtrasada + "', '" + productocve + "', '" +
                            producto + "', '" + cajasDisp + "', '" + mAutoriza + "', '" +
                            tarimaLeido + "', '" + tarimaAtrasada.Trim() + "', '" + imei.Trim() +
                            "', '" + motivo + "', GETDATE())";

                        lblEstado.Text = "✓ Autorización válida";
                        lblEstado.SetTextColor(Android.Graphics.Color.ParseColor("#00AA44"));
                        await Task.Delay(1000);
                        dialog.Dismiss();
                    }
                    else
                    {
                        btnValidar.Enabled = true;

                        if (mensaje.Contains("FRAUDE"))
                        {
                            new Android.App.AlertDialog.Builder(this)
                                .SetTitle("🚨 FRAUDE DETECTADO")
                                .SetMessage($"El intento queda registrado.\n\n{mensaje}")
                                .SetNeutralButton("ENTENDIDO", (s2, e2) => { })
                                .SetCancelable(false)
                                .Show();
                        }

                        lblEstado.Text = mensaje.Contains("FRAUDE") ? $"🔴 {mensaje}" : $"❌ {mensaje}";
                        lblEstado.SetTextColor(Android.Graphics.Color.Red);
                        txtOTP.Text = "";
                        txtOTP.RequestFocus();
                    }
                }
                catch (Java.Lang.Exception exGeneral) // ✅ SI ALGO MATA LA APP, CAE AQUÍ
                {
                    // En lugar de cerrar la app, muestra el error en la pantalla
                    btnValidar.Enabled = true;
                    lblEstado.Text = $"❌ Error interno: {exGeneral.Message}";
                    lblEstado.SetTextColor(Android.Graphics.Color.Red);

                    // Opcional: ver el error completo en la consola de Visual Studio
                    System.Diagnostics.Debug.WriteLine($"ERROR OTP: {exGeneral}");
                }
            };
        }

        // --- MÉTODOS DE APOYO PARA VALIDACIÓN Y CIERRE DE EMBARQUE ---

        private void ValidarYGuardarEmbarque(string folio)
        {
            string erroresDiferencia = "";

            try
            {
                if (thisConnection.State == ConnectionState.Closed) thisConnection.Open();

                // 1. Verificar si hay diferencias entre pedido y surtido
                string sqlDif = "SELECT DISTINCT CANT_PED, CANT_SUR, NOM_PROD FROM tb_ped_embarque WHERE emb_folio=@folio AND NALEXP=@tipo";
                using (SqlCommand cmd = new SqlCommand(sqlDif, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@folio", folio);
                    cmd.Parameters.AddWithValue("@tipo", tipoped);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int pedidoCant = Convert.ToInt32(reader["CANT_PED"]);
                            int surtidoCant = Convert.ToInt32(reader["CANT_SUR"]);
                            if (pedidoCant != surtidoCant)
                            {
                                erroresDiferencia += $"{reader["NOM_PROD"]}: Ped {pedidoCant} / Sur {surtidoCant}\n";
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex) { Toast.MakeText(this, "Error: " + ex.Message, ToastLength.Long).Show(); }
            finally { if (thisConnection.State == ConnectionState.Open) thisConnection.Close(); }

            // 2. Definir la acción final de guardado
            Action<string> accionFinal = (obsFinal) =>
            {
                EjecutarGuardadoFinal(folio, obsFinal);
            };

            // 3. Flujo de Diálogos
            if (!string.IsNullOrEmpty(erroresDiferencia))
            {
                // Caso A: Hay diferencias de mercancía
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("DIFERENCIA DETECTADA");
                builder.SetMessage("El surtido no coincide con el pedido. ¿Desea continuar con el cierre?");
                builder.SetPositiveButton("Continuar", (s, a) =>
                {
                    // Si acepta la diferencia, ahora evaluamos si necesita el diálogo de observaciones (6h / 12am)
                    MostrarDialogoObservacionesSiAplica(folio, accionFinal);
                });
                builder.SetNegativeButton("Cancelar", (s, a) => { });
                builder.Show();
            }
            else
            {
                // Caso B: No hay diferencias, ir directo a validar tiempo en planta
                MostrarDialogoObservacionesSiAplica(folio, accionFinal);
            }
        }

        private void EjecutarGuardadoFinal(string folio, string observacionExtra)
        {
            try
            {
                if (thisConnection.State == ConnectionState.Closed) thisConnection.Open();

                // 1. Validar Split
                SqlCommand cmdSplit = new SqlCommand("SELECT COUNT(*) FROM tb_det_split WHERE emb_folio=@f AND estatus='A'", thisConnection);
                cmdSplit.Parameters.AddWithValue("@f", folio);
                if ((int)cmdSplit.ExecuteScalar() > 0)
                {
                    Toast.MakeText(this, "No se puede cerrar: Tiene Splits pendientes.", ToastLength.Long).Show();
                    return;
                }

                // 2. Obtener cajas
                SqlCommand cmdCajas = new SqlCommand("SELECT ISNULL(SUM(cajas),0) FROM tb_det_embarque WHERE emb_folio=@f", thisConnection);
                cmdCajas.Parameters.AddWithValue("@f", folio);
                int totalCajas = Convert.ToInt32(cmdCajas.ExecuteScalar());

                // 3. Actualizar Maestro
                string hora = DateTime.Now.ToString("hh:mm tt").Replace(" ", "").ToLower();
                string sqlUpd = "UPDATE tb_mstr_embarque SET hora_fin=@h, cajas=@c, sts='T', EMB_obs=@obs WHERE emb_folio=@f";
                using (SqlCommand cmd = new SqlCommand(sqlUpd, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@h", hora);
                    cmd.Parameters.AddWithValue("@c", totalCajas);
                    cmd.Parameters.AddWithValue("@obs", observacionExtra);
                    cmd.Parameters.AddWithValue("@f", folio);
                    cmd.ExecuteNonQuery();
                }

                // 4. Actualizar Pedidos (Nacional o Expo)
                string tabla = (tipoped == "EXP") ? "tb_mstr_pedidos_exp" : "tb_mstr_pedidos_nal";
                string sqlPed = $"UPDATE {tabla} SET pdn_surtido='S' WHERE pdn_folio=@f";
                using (SqlCommand cmd = new SqlCommand(sqlPed, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@f", folio);
                    cmd.ExecuteNonQuery();
                }

                // 5. Cerrar Acceso Celulares
                SqlCommand cmdAcc = new SqlCommand("UPDATE tb_det_acceso_celulares SET estado='T' WHERE folio=@f AND sistema='CargaEmbarques'", thisConnection);
                cmdAcc.Parameters.AddWithValue("@f", folio);
                cmdAcc.ExecuteNonQuery();

                Toast.MakeText(this, "Embarque Cerrado con Éxito", ToastLength.Short).Show();
                Limpiar(); // Método para resetear la pantalla
            }
            catch (System.Exception ex) { Toast.MakeText(this, "Error Final: " + ex.Message, ToastLength.Long).Show(); }
            finally { if (thisConnection.State == ConnectionState.Open) thisConnection.Close(); }
        }
    }
}