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
using System.Linq;

namespace CargaEmbarques
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/logo", Theme = "@style/MyTheme.Light", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private const int RequestPermissionCode = 1;
        private const int PickFileRequestCode = 2;

        WSCargaEmbarques192.WebServiceEmbarques proxyLocal = new WSCargaEmbarques192.WebServiceEmbarques();
        WSCargaEmbarques189.WebServiceEmbarques proxy = new WSCargaEmbarques189.WebServiceEmbarques();

        public static string cadenaConexion = "Persist Security Info=False;user id=sa; password=Gabira2026$;Initial Catalog = GAB_Irapuato; server=tcp:189.206.160.206,2352; MultipleActiveResultSets=true; Connect Timeout = 0";
        //public static string cadenaConexion = "Persist Security Info=False;user id=sa; password=Gabira2026$;Initial Catalog = GAB_Irapuato; server=tcp:192.168.123.6,1433; MultipleActiveResultSets=true; Connect Timeout = 0";

        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        public static string veh = "";
        public static int captura = 0;
        SqlCommand cmnd = new SqlCommand();
        SqlDataReader reader;
        SqlCommand cmnd1 = new SqlCommand();
        SqlDataReader reader1;
        System.String[] strFrutas;
        System.String[] stremb_folios;
        ArrayAdapter<System.String> comboAdapter;
        SqlDataAdapter da;
        SqlDataAdapter da1;
        public static DataTable camionetas = new DataTable("camionetas");
        public static DataTable responsables = new DataTable("responsables");
        public static DataTable vehiculos = new DataTable("vehiculos");
        public static DataTable version = new DataTable("version");
        public static DataTable formulario = new DataTable("formulario");
        string query = "";

        public static DataTable Pedidostotales = new DataTable("formulario");

        DataSet ds = new DataSet();
        DataSet ds1 = new DataSet();
        public static string vehiculo = "";
        public static string responsablesplit = "";
        public static string imei = "";
        public static string imeiT = "";
        public static string ip = "";
        public static string id_anden = "";
        public static string emb_folio = "";
        public static string no_trailer = "";

        TextView versionapp;

        //Variables del servicio Web
        Context context;
        Runnable listener;
        //private static string INFO_FILE = "http://mrlucky.com.mx/ventasnew/CargaEmbarquesTablet/version.txt";
        //private static string INFO_FILE = "http://192.168.123.4:81/EmbarquesApk/CargaEmbarquesTablet/version.txt";
        private static string INFO_FILE = "http://189.206.160.206:81/EmbarquesApk/CargaEmbarquesTablet/version.txt";
        //private static string INFO_FILE = "http://192.168.123.4:81/EmbarquesApk/CargaEmbarquesTablet/version.txt";
        private int currentVersionCode;
        private string currentVersionName;
        private int latestVersionCode;
        private string latestVersionName;
        private string downloadURL;

        EditText pass;

        private const int RequestPermissionsCode = 1;

        private TeamsNotifier notiTeams;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
            StrictMode.SetThreadPolicy(policy);
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Solicitar permisos necesarios
            RequestNecessaryPermissions();

            // Solicitar permiso especial para almacenamiento externo
            //RequestManageExternalStoragePermission();

            Button log = FindViewById<Button>(Resource.Id.btn_login);
            log.Click += Btnlogin_Click;

            #region ValidaWiFi
            WifiManager wifi = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
            if (wifi.IsWifiEnabled == false)
            {
                GuardarLocal GuardaError = new GuardarLocal();
                GuardaError.creartxt("Wifi Deshabilitada");
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#ffc107' size = 10>Error en el Adaptador WIFI</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Dispositivo no tiene la Wifi Activada, favor de activarlo</font>"));
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                    Finish();

                });
                alertDialog.Show();
            }
            #endregion

            #region ValidaConexionRed
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
            bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
            if (!isOnline || !validaservidores())
            {
                INFO_FILE = "http://192.168.123.4:81/EmbarquesApk/CargaEmbarquesTablet/version.txt";
                cadenaConexion = "Persist Security Info=False;user id=sa; password=Gabira2026$;Initial Catalog = GAB_Irapuato; server=tcp:192.168.123.6,1433; MultipleActiveResultSets=true; Connect Timeout = 0";

                //GuardarLocal GuardaError = new GuardarLocal();
                //GuardaError.creartxt("Error en la conexion de red, No esta conectado a ninguna red");
                //Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                //alertDialog.SetTitle(Html.FromHtml("<font color='#ffc107' size = 10>Error en la Conexion a Internet</font>"));
                //alertDialog.SetIcon(Resource.Drawable.warning);
                //alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Dispositivo Continuara De Forma Local</font>"));
                //alertDialog.SetCancelable(false);
                //alertDialog.SetNeutralButton("Ok", delegate
                //{
                //    alertDialog.Dispose();
                //    //Finish();
                //});
                //alertDialog.Show();

            }
            #endregion

            imei = getDeviceID();

            versionapp = FindViewById<TextView>(Resource.Id.versionapp);

            thisConnection = new SqlConnection(cadenaConexion);

            try
            {
                if (thisConnection.State == ConnectionState.Closed)
                {
                    thisConnection.Open();
                }

                #region Llenado Spinner 2

                query = "SELECT nomina, CONCAT(RTRIM(Nombre), ' ', RTRIM(Ape_Pat), ' ', RTRIM(Ape_Mat)) As Nombre, password FROM TB_RESPON_CARGA WHERE status = 'A' ORDER BY Nombre";
                da = new SqlDataAdapter(query, thisConnection);
                da.Fill(ds, "responsables");
                responsables = ds.Tables["responsables"];
                thisConnection.Close();

                Spinner spinner2 = FindViewById<Spinner>(Resource.Id.spinner2);
                System.Collections.ArrayList listaFrutas2 = new System.Collections.ArrayList();

                strFrutas = new System.String[responsables.Rows.Count + 1];
                strFrutas[0] = "Seleccione un Responsable";
                for (int i = 1; i <= responsables.Rows.Count; i++)
                {
                    int x = i - 1;
                    strFrutas[i] = responsables.Rows[x]["Nombre"].ToString();
                }


                Collections.AddAll(listaFrutas2, strFrutas);
                comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, strFrutas);
                spinner2.Adapter = comboAdapter;
                spinner2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected2);
                #endregion


                #region VALIDAR ACTUALIZACION DE LA APLICACION
                //Inicio de Validacion de Actualizacion *******************************************************************
                try
                {
                    getData();
                }
                catch
                {

                }
                versionapp.Text = "CARGA EMBARQUES - Version: " + currentVersionName;
                if (isNewVersionAvailable())
                {
                    //Crea mensaje con datos de versión.
                    string msj = "Nueva Version: " + isNewVersionAvailable();
                    msj += "\nActual Version: " + currentVersionName + "(" + currentVersionCode + ")";
                    msj += "\nUltima Version: " + latestVersionName + "(" + latestVersionCode + ")";
                    msj += "\nDesea Actualizar?";
                    //Crea ventana de alerta.
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Actualizacion Disponible"));
                    alertDialog.SetIcon(Resource.Drawable.update);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>" + msj + "</font>"));
                    alertDialog.SetPositiveButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>Sí</font>"), SaveAction);
                    //alertDialog.SetNegativeButton(Html.FromHtml("<font face = 'Comic Sans MS, arial' color='#DF0101' size = '10'>No</font>"), CancelaAction);
                    alertDialog.SetCancelable(false);
                    alertDialog.Create();
                    alertDialog.Show();
                    //Muestra la ventana esperando respuesta.
                }
            }
            catch (Java.Lang.Exception e)
            {
                GuardarLocal GuardaError = new GuardarLocal();
                GuardaError.creartxt("Error En Conexion SQL");
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#ffc107' size = 10>Error En Conexion SQL</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El sistema no se puede conectar a la base de datos GAB_Irapuato (SQL Server), Favor de informar a Sistemas a la brevedad</font>"));
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                    Finish();

                });
                alertDialog.Show();
                #endregion
            }

            #region TEAMS
            //            #region CONFIGURAR WEBHOOKSERVER
            //            var server = new WebhookServer();
            //            server.Start();
            //            #endregion

            //            #region ENVIAR NOTIFICACION TEAMS
            //            // Instanciar la clase con el Webhook
            //            notiTeams = new TeamsNotifier("https://mrluckycommx.webhook.office.com/webhookb2/10baebcf-a990-473a-b619-4c0902d824bd@d20460dd-675d-4b51-87cc-9d10f9175633/IncomingWebhook/a3c759e44abc4a9e83eb693cf604b0e0/bbba0cb3-31d4-4d5a-ac47-264700d7b7d0/V2YNn6avVMmBQOeCJ19fSVRNzO81iARPHZ1pwzl5QmfeA1");

            //            // Enviar mensaje al iniciar la aplicación (de manera sincrónica)
            //            //notiTeams.SendMessageToTeamsSync("📢 La aplicación ha sido iniciada.");

            //            string cardJsonWithButtons =
            //                @"{
            //                    ""type"": ""AdaptiveCard"",
            //                    ""version"": ""1.0"",
            //                    ""body"": [
            //                        {
            //                            ""type"": ""TextBlock"",
            //                            ""text"": ""¿Deseas realizar esta acción?""
            //                        },
            //                        {
            //                            ""type"": ""ActionSet"",
            //                            ""actions"": [
            //                                {
            //                                    ""type"": ""Action.Submit"",
            //                                    ""title"": ""Sí, quiero"",
            //                                    ""method"": ""POST"",
            //                                    ""url"": ""http://189.206.160.206:81/EmbarquesApk/WSBusCheckIn/WSBusCheckIn.asmx/HelloWorld"",
            //                                    ""body"": ""{\""response\"": \""yes\""}""
            //                                },
            //                                {
            //                                    ""type"": ""Action.Submit"",
            //                                    ""title"": ""No, gracias"",
            //                                    ""method"": ""POST"",
            //                                    ""url"": ""http://189.206.160.206:81/EmbarquesApk/WSBusCheckIn/WSBusCheckIn.asmx"",
            //                                    ""body"": ""{\""response\"": \""no\""}""
            //                                }
            //                            ]
            //                        }
            //                    ]
            //                }";

            //            string cardJsonWithButton =
            //                @"{
            //    ""type"": ""AdaptiveCard"",
            //    ""version"": ""1.0"",
            //    ""body"": [
            //        {
            //            ""type"": ""TextBlock"",
            //            ""text"": ""¿Deseas realizar esta acción?""
            //        },
            //        {
            //            ""type"": ""ActionSet"",
            //            ""actions"": [
            //                {
            //                    ""type"": ""Action.Submit"",
            //                    ""title"": ""Sí, quiero"",
            //                    ""data"": {
            //                        ""response"": ""si""
            //                    }
            //                },
            //                {
            //                    ""type"": ""Action.Submit"",
            //                    ""title"": ""No, gracias"",
            //                    ""data"": {
            //                        ""response"": ""no""
            //                    }
            //                }
            //            ]
            //        }
            //    ]
            //}
            //";

            //            notiTeams.SendAdaptiveCard(cardJsonWithButtons);

            //            #endregion
            #endregion


            //AQUI ESTABA LA OBTENCION DEL ID


            ValidarErroresLocal();


            var toolbar = FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "CARGA POR PEDIDOS";

            pass = FindViewById<EditText>(Resource.Id.password);


        }

        private void RequestNecessaryPermissions()
        {
            // Lista de permisos peligrosos
            string[] dangerousPermissions = {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.Camera,
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ReadPhoneState,
            Manifest.Permission.AccessWifiState,
            Manifest.Permission.InstallPackages,
            Manifest.Permission.RequestInstallPackages,
            Manifest.Permission.AccessCheckinProperties,
            Manifest.Permission.WakeLock
        };

            // Filtrar permisos no concedidos
            var permissionsToRequest = dangerousPermissions
                .Where(permission => ContextCompat.CheckSelfPermission(this, permission) != (int)Permission.Granted)
                .ToArray();

            // Solicitar permisos
            if (permissionsToRequest.Any())
            {
                ActivityCompat.RequestPermissions(this, permissionsToRequest, RequestPermissionsCode);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestPermissionsCode)
            {
                for (int i = 0; i < permissions.Length; i++)
                {
                    if (grantResults[i] == Permission.Granted)
                    {
                        // Permiso concedido
                        Toast.MakeText(this, $"{permissions[i]} concedido", ToastLength.Short).Show();
                    }
                    else
                    {
                        // Permiso denegado
                        Toast.MakeText(this, $"{permissions[i]} denegado", ToastLength.Short).Show();
                    }
                }
            }
        }

        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    const int RequestPermissionCode = 1; // Usar una constante para el código de solicitud

        //    if (requestCode == RequestPermissionCode)
        //    {
        //        // Verificar si todos los permisos fueron concedidos
        //        if (grantResults.Length > 0 && grantResults.All(result => result == Permission.Granted))
        //        {
        //            Toast.MakeText(this, "Permiso(s) Concedido(s)", ToastLength.Long).Show();
        //            Logeo(); // Llamar a la lógica que depende del permiso
        //        }
        //        else
        //        {
        //            Toast.MakeText(this, "Permiso(s) Denegado(s)", ToastLength.Long).Show();
        //        }
        //    }

        //    // Llamar a Xamarin.Essentials.Platform.OnRequestPermissionsResult si estás usando Essentials
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        //    // Llamar a la implementación de la clase base
        //    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}


        private void CancelaAction(object sender, DialogClickEventArgs e)
        {
            Finish();
        }

        private void SaveAction(object sender, DialogClickEventArgs e)
        {
            //downloadApp();
            DownloadApp();
            //downloadApps();
            //await DownloadAppAsync();
        }

        void Btnlogin_Click(object sender, EventArgs e)
        {

            System.String[] opciones = new System.String[] {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission.Camera,
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage
            };

            var xs = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);


            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == (int)Android.Content.PM.Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Android.Content.PM.Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == (int)Android.Content.PM.Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Android.Content.PM.Permission.Granted &&
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Android.Content.PM.Permission.Granted)
            {
                Logeo();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, opciones, 1);
            }
        }

        private void Logeo()
        {
            if (responsablesplit == "Seleccione un Responsable")
            {
                Toast.MakeText(this, "Por favor, asegurese de seleccionar un responsable y volver a intentarlo", ToastLength.Long).Show();
                return;
            }

            if (vehiculo == "Seleccione un vehiculo")
            {
                Toast.MakeText(this, "Por favor, asegurese de seleccionar un vehiculo y volver a intentarlo", ToastLength.Long).Show();
                return;
            }


            if (pass.Text.Length == 0)
            {
                Toast.MakeText(this, "Por favor, asegurese de ingresar una contraseña y volver intentarlo", ToastLength.Long).Show();
                return;
            }

            var responsable = "";
            if (responsables.Rows.Count != 0)
            {
                for (int i = 0; i < responsables.Rows.Count; i++)
                {
                    if ((responsables.Rows[i]["Nombre"].ToString() == responsablesplit) && (responsables.Rows[i]["password"].ToString() == pass.Text.ToString().Trim()))
                    {
                        responsable = responsables.Rows[i]["nomina"].ToString();
                    }
                }
            }
            else
            {
                Toast.MakeText(this, "Por favor, Seleccione un responsable", ToastLength.Long).Show();
                return;
            }

            if (responsable.Trim().Length > 0)
            {
                //obtener Ip del telefono
                WifiManager wifiManager = (WifiManager)this.GetSystemService(Android.App.Service.WifiService);
                ip = GetIPAddress();
                //obtener Imei del telefono
                //imei = getDeviceID();
                //Termina obtener datos 
                List<System.String> embfolios = new List<System.String>();
                //Valido inicio de sesion activa********************************************************************************
                thisConnection.Open();
                //string Cadena = "SELECT B.Id_Anden, C.no_trailer, C.hora_trailer, A.emb_folio, B.ClaveTablet FROM tb_mstr_embarque A INNER JOIN Tb_Cat_Anden B ON A.anden = B.Id_Anden left JOIN tb_mstr_trailer C ON A.emb_folio = C.pdn_folio WHERE A.sts = 'C' AND B.ClaveTablet IN ( Select imei from tb_det_acceso_celulares where nom_usu = '" + responsablesplit.Trim() + "' AND sistema = 'CAPTURAEMBARQUE' AND folio = '' AND estado = 'A' ) ORDER BY emb_folio";
                string Cadena = "Select imei from tb_det_acceso_celulares where nom_usu = '" + responsablesplit.Trim() + "' AND sistema = 'CAPTURAEMBARQUE' AND folio = '' AND estado = 'A'";
                SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
                //cmdx.CommandTimeout = 0;
                string valor = Convert.ToString(cmdx.ExecuteScalar());

                thisConnection.Close();
                //imeiT.Trim().Length > 0
                if (valor.Trim().Length > 0)
                {
                    #region VALIDACION DE SESION ACTIVA LECTORAS
                    /*if (valor == imei)
					{
						thisConnection.Open();
						string cadena = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
									"VALUES('" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','CEL " + imei + "','" + responsablesplit + "','E','" + ip + "','','Ingreso a sistema CARGA ERMBARQUES - TABLET Imei: " + imei + ", Ip: " + ip + " ','CAPTURAEMB','')";
						SqlCommand cmd = new SqlCommand(cadena, thisConnection);
						cmd.ExecuteNonQuery();
						thisConnection.Close();

						Intent intent = new Intent(this, typeof(CapturarPedido));
						intent.PutExtra("cvresponsable", responsable.ToString());
						intent.PutExtra("responsable", responsablesplit.ToString());
						intent.PutExtra("imei", imei.ToString());
						StartActivity(intent);
						//var testSendMail = new WebServiceEmbarques.WebServiceEmbarques();
						//var testSendMail = new WebServiceEmbarques189.WebServiceEmbarques();
						//testSendMail.SendMail("jgalvan@mrlucky.com.mx", "COMIENZA CAPTURA DEL EMBARQUE " + embfolios + "DESDE LA TABLET " + imei + "EN EL ANDEN #" + id_anden + cadena.ToString(), "COMIENZA CARGA DE EMBARQUES DESDE TABLET"); ;
						Finish();
					}
					else
					{
						Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
						alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Sesion Activa en otro Equipo</font>"));
						alertDialog.SetIcon(Resource.Drawable.warning);
						alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>No puede iniciar otra sesion, debido a que hay un equipo con su sesion activa, favor de cerrar su sesion anterior e intentarlo de nuevo</font>"));
						alertDialog.SetCancelable(false);
						alertDialog.SetNeutralButton("Ok", delegate
						{
							alertDialog.Dispose();

						});
						alertDialog.Show();

					}*/
                    #endregion
                    #region VALIDACION DE SESION ACTIVA TABLETS
                    thisConnection.Open();
                    string cadena = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                "VALUES('" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','CEL " + imei + "','" + responsablesplit + "','E','" + ip + "','','Ingreso a sistema CARGA ERMBARQUES - TABLET Imei: " + imei + ", Ip: " + ip + " ','CAPTURAEMB','')";
                    SqlCommand cmd = new SqlCommand(cadena, thisConnection);
                    cmd.ExecuteNonQuery();
                    thisConnection.Close();

                    Intent intent = new Intent(this, typeof(CapturarPedido));
                    intent.PutExtra("cvresponsable", responsable.ToString());
                    intent.PutExtra("responsable", responsablesplit.ToString());
                    intent.PutExtra("imei", imei.ToString());
                    StartActivity(intent);
                    //var testSendMail = new WebServiceEmbarques.WebServiceEmbarques();
                    //var testSendMail = new WebServiceEmbarques189.WebServiceEmbarques();
                    //testSendMail.SendMail("jgalvan@mrlimeiucky.com.mx", "COMIENZA CAPTURA DEL EMBARQUE " + embfolios + "DESDE LA TABLET " + imei + "EN EL ANDEN #" + id_anden + cadena.ToString(), "COMIENZA CARGA DE EMBARQUES DESDE TABLET"); 
                    Finish();
                    #endregion
                }
                else
                {

                    thisConnection.Open();
                    Cadena = "Select nom_usu from tb_det_acceso_celulares where imei = '" + imei.Trim() + "' AND Folio = '' AND estado = 'A' AND sistema = 'CAPTURAEMBARQUE'";
                    cmdx = new SqlCommand(Cadena, thisConnection);
                    string nombre = Convert.ToString(cmdx.ExecuteScalar());
                    thisConnection.Close();

                    if (nombre.Trim().Length > 0)
                    {
                        Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                        alertDialog.SetTitle(Html.FromHtml("<font color='#ffc107' size = 10>Sesion Activa en Este Equipo</font>"));
                        alertDialog.SetIcon(Resource.Drawable.warning);
                        alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>No puede iniciar sesion debido a que este equipo se encuentra actualmente en uso por " + nombre
                            + "</font>"));
                        alertDialog.SetCancelable(false);
                        alertDialog.SetNeutralButton("Ok", delegate
                        {
                            alertDialog.Dispose();

                        });
                        alertDialog.Show();
                    }
                    else
                    {
                        //Registro de Ingreso Al Sistema.
                        thisConnection.Open();
                        string cadena = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                                    "VALUES('" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "','CEL " + imei + "','" + responsablesplit + "','E','" + ip + "','','Ingreso a sistema CAPTURA EMBARQUE Imei: " + imei + ", Ip: " + ip + " ','CAPTUEMBAR','')";
                        SqlCommand cmd = new SqlCommand(cadena, thisConnection);
                        cmd.ExecuteNonQuery();

                        cadena = "INSERT INTO tb_det_acceso_celulares ( fecha, imei, nom_usu, sistema, folio, version, estado) " +
                                    "VALUES(GETDATE(),'" + imei + "','" + responsablesplit + "','CAPTURAEMBARQUE','','" + currentVersionName + "','A')";
                        cmd = new SqlCommand(cadena, thisConnection);
                        cmd.ExecuteNonQuery();

                        thisConnection.Close();

                        Intent intent = new Intent(this, typeof(CapturarPedido));
                        intent.PutExtra("cvresponsable", responsable.ToString());
                        intent.PutExtra("responsable", responsablesplit.ToString());
                        intent.PutExtra("imei", imei.ToString());
                        StartActivity(intent);
                        Finish();
                    }
                }
            }
            else
            {

                Toast.MakeText(this, "Contraseña Invalida para este usuario", ToastLength.Long).Show();
                return;
            }
        }

        private void spinner_ItemSelected2(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            responsablesplit = spinner.GetItemAtPosition(e.Position).ToString();
            pass.RequestFocus();
            InputMethodManager immx = (InputMethodManager)GetSystemService(Context.InputMethodService);
            immx.ShowSoftInput(pass, ShowFlags.Implicit);
        }

        private void getData()
        {
            try
            {
                context = this;
                // Datos locales
                System.Console.WriteLine("AutoUpdater", "GetData");
                Android.Content.PM.PackageInfo pckginfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);

                currentVersionCode = pckginfo.VersionCode;
                currentVersionName = pckginfo.VersionName;

                // Datos remotos
                string data = downloadHttp(new URL(INFO_FILE));
                JSONObject json = new JSONObject(data.ToString());
                latestVersionCode = json.GetInt("versionCode");
                latestVersionName = json.OptString("versionName");
                downloadURL = json.GetString("downloadURL");
                System.Console.WriteLine("AutoUpdate", "Datos obtenidos con éxito");
            }
            catch (JSONException e)
            {
                System.Console.WriteLine("AutoUpdate", "Ha habido un error con el JSON", e);
            }
            catch (Android.Content.PM.PackageManager.NameNotFoundException e)
            {
                System.Console.WriteLine("AutoUpdate", "Ha habido un error con el packete :S", e);
            }
            catch (System.IO.IOException e)
            {
                System.Console.WriteLine("AutoUpdate", "Ha habido un error con la descarga", e);
            }
        }

        private static string downloadHttp(URL url)
        {
            try
            {
                // Abrir la conexión
                HttpWebRequest solicitud = (HttpWebRequest)WebRequest.Create(new System.Uri((string)url));
                solicitud.Method = "GET";
                solicitud.Timeout = 15 * 1000;

                // Obtener la respuesta
                using (HttpWebResponse respuesta = (HttpWebResponse)solicitud.GetResponse())
                {
                    if (respuesta.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream flujoRespuesta = respuesta.GetResponseStream())
                        {
                            using (StreamReader lector = new StreamReader(flujoRespuesta))
                            {
                                string resultado = lector.ReadToEnd();
                                return resultado;
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine($"Error al descargar el archivo. Código de estado: {respuesta.StatusCode}");
                        return null;
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error al descargar el archivo: {ex.Message}");
                return null;
            }
        }

        public bool isNewVersionAvailable()
        {
            return latestVersionCode > currentVersionCode;
        }

        private string DownloadApp()
        {
            var progressDialog = ProgressDialog.Show(this, "Espere Por Favor...", "Descargando Actualización", true);

            new System.Threading.Thread(new ThreadStart(delegate
            {
                try
                {
                    // Usar ContextCompat para obtener el directorio público de la aplicación
                    var pathToNewFolder = System.IO.Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, "CargaEmbarques");
                    Directory.CreateDirectory(pathToNewFolder);

                    string archivo = System.IO.Path.Combine(pathToNewFolder, "apk");

                    var webClient = new WebClient();
                    webClient.DownloadFileCompleted += (s, ex) =>
                    {
                        RunOnUiThread(() => progressDialog.Hide());

                        if (ex.Error != null)
                        {
                            RunOnUiThread(() => Toast.MakeText(this, "Error en la descarga: " + ex.Error.Message, ToastLength.Long).Show());
                            return;
                        }

                        Java.IO.File toInstall = new Java.IO.File(archivo);
                        Android.Net.Uri downloadUri = FileProvider.GetUriForFile(this, this.ApplicationContext.PackageName + ".fileprovider", toInstall);

                        Intent intentx = new Intent(Intent.ActionView);
                        intentx.SetDataAndType(downloadUri, "application/vnd.android.package-archive");
                        intentx.SetFlags(ActivityFlags.NewTask);
                        intentx.AddFlags(ActivityFlags.GrantReadUriPermission);

                        StartActivity(intentx);
                    };

                    if (INFO_FILE == "http://192.168.123.4:81/EmbarquesApk/CargaEmbarquesTablet/version.txt")
                    {
                        webClient.DownloadFileAsync(new System.Uri("http://192.168.123.4:81/EmbarquesApk/CargaEmbarquesTablet/CargaEmbarques.apk"), archivo);
                    }
                    else
                    {
                        webClient.DownloadFileAsync(new System.Uri("http://189.206.160.206:81/EmbarquesApk/CargaEmbarquesTablet/CargaEmbarques.apk"), archivo);
                    }
                }
                catch (System.IO.IOException e)
                {
                    RunOnUiThread(() => progressDialog.Hide());
                    RunOnUiThread(() => Toast.MakeText(this, e.ToString(), ToastLength.Long).Show());
                }
            })).Start();

            return "1";
        }

        public string GetIPAddress()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Dns.GetHostName());

            if (adresses != null && adresses[0] != null)
            {
                return adresses[0].ToString();
            }
            else
            {
                return null;
            }
        }

        public bool validaservidores()
        {
            bool online = true;
            string[] sitios = new string[1];
            //sitios[0] = "http://192.168.123.4:81/EmbarquesApk/";
            //sitios[1] = "http://192.168.123.6";
            sitios[0] = "http://189.206.160.206:81/EmbarquesApk/";
            //sitios[1] = "http://189.206.160.206";


            for (int i = 0; i < sitios.Length; i++)
            {
                GuardarLocal ValidarServidor = new GuardarLocal();
                bool onlinex = ValidarServidor.HayConexion(sitios[i]);

                if (onlinex == false)
                {
                    ValidarServidor.creartxt("Error al Conectar a " + sitios[i]);
                }
            }
            return online;
        }

        private void ValidarErroresLocal()
        {
            //Java.IO.File dir = Android.App.Application.Context.GetExternalFilesDir(null);
            //Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File sdCard = Android.App.Application.Context.GetExternalFilesDir(null);

            Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/MyFolder");
            dir.Mkdirs();
            Java.IO.File file = new Java.IO.File(dir, "errores.txt");
            string FileToRead = file.ToString();
            // Creating string array  
            if (file.Exists())
            {
                string[] lines = System.IO.File.ReadAllLines(FileToRead);
                string correo = string.Join(System.Environment.NewLine, lines).Replace("\n", "<br>");
                //var proxy = new WebServiceEmbarques.WebServiceEmbarques();
                //var proxy = new WSEmbarques.WebServiceEmbarques();
                //var proxy = new WSCargaEmbarques.Resource.Drawable192.WebServiceEmbarques();
                //var proxy = new WSCargaEmbarques.Resource.Drawable189.WebServiceEmbarques();
                if (INFO_FILE == "http://192.168.123.4:81/EmbarquesApk/CargaEmbarquesTablet/version.txt")
                {
                    proxyLocal.SendMailPersonal("ricardo.cortes@mrlucky.com.mx;jgalvan@mrlucky.com.mx", correo, "Error Generado Tablet: " + imei + "", "jgalvan", "mnK3a2aN@1|Q21VV", "jgalvan@mrlucky.com.mx");
                }
                else
                {
                    proxy.SendMailPersonal("ricardo.cortes@mrlucky.com.mx;jgalvan@mrlucky.com.mx", correo, "Error Generado Tablet: " + imei + "", "jgalvan", "mnK3a2aN@1|Q21VV", "jgalvan@mrlucky.com.mx");
                }
                //proxy.SendMailPersonal("ricardo.cortes@mrlucky.com.mx;jgalvan@mrlucky.com.mx", correo, "Error Generado Tablet: " + imei + "", "jgalvan", "Programador2", "jgalvan@mrlucky.com.mx");
                file.Delete();
            }
        }

        private string getDeviceID()
        {
            Android.Telephony.TelephonyManager mTelephonyMgr;
            mTelephonyMgr = (Android.Telephony.TelephonyManager)GetSystemService(TelephonyService);
            string uniqueID = UUID.RandomUUID().ToString();
            //imei = mTelephonyMgr.DeviceId;
            imei = uniqueID;

            var deviceId = CrossDeviceInfo.Current.Id;

            if (imei == null || imei.Length > 17)
            {
                imei = deviceId;
            }

            return imei;
        }

        protected override void OnPause()
        {
            base.OnPause();
        }
    }
}