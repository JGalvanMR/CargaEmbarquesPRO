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
using Android.Graphics;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace CargaEmbarques
{
    [Activity(Label = "SUBIR FOTO", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class subirFoto : Activity
    {
        public static int valido = 0, veces = 0;
        public static string cvvehiculo, cvresponsable, Version = "12.0";
        public static string vehiculo, responsable;
        public string Nombre = "", Mtipo = "", MProd = "", MTar = "", MFol = "", mUser = "", mAutoriza = "", user = "", motfolade = "";
        public string cvecam = "", muser = "", mconcen = "1";
        public static string AutoPed = "N";
        public int proceso = 0;
        public static string EtiquetaExiste = "S", EtiquetaCapturada = "S", FechaCaducada = "S";
        public static string HayExistencias = "S";
        public static string Surtidomayor = "S";
        public static string ValiFechacad = "S";
        public static string EstructuraEtiqueta = "S";
        public static string dondegenera = "";
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

        string conse;
        string trailerplaca;
        string fechatrailer;
        string posicionactual;
        string ordenventa;

        public static string imei = "";

        //Declarar los datos de los items en el layout CapturarSplit
        TextView orden;
        TextView posicion;
        ImageView Captura;

        Button Camara;
        Button Guardar;

        public string pathimagen = "";

        string valorfinal = "";

        public static int ScreenWidth;
        public static int ScreenHeight;

        //Variables de solicitud al servidor si realiza o no guardado de datos de la bd interna a la bd del servidor antes de borrar

        Context context;
        Runnable listener;
        //private static string INFO_FILE = "http://192.168.123.4:81/EmbarquesApk/estado_respaldo.txt";
        private static string INFO_FILE = "http://189.206.160.206:81/EmbarquesApk/estado_respaldo.txt";
        private int respaldo_activo = 1;

        string latitud = "";
        string longitud = "";

        public string TAG
        {
            get;
            private set;
        }
        string esFinal = "NO";

        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.capturarposicion);

            //responsable = Intent.GetStringExtra("responsable");

            orden = FindViewById<TextView>(Resource.Id.txtordenventa);
            posicion = FindViewById<TextView>(Resource.Id.txtposicion);
            Captura = FindViewById<ImageView>(Resource.Id.fotopos);
            Camara = FindViewById<Button>(Resource.Id.TomarFoto);
            Guardar = FindViewById<Button>(Resource.Id.GuardarFoto);

            Camara.Click += camara_click;
            RequestPermissions(permissionGroup, 0);
            var xs = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);

            Guardar.Click += Guardar_Click;
            Captura.SetImageResource(Resource.Drawable.logo);


            responsable = Intent.GetStringExtra("responsable");
            ordenventa = Intent.GetStringExtra("OrdenVenta");
            posicionactual = Intent.GetStringExtra("Posicion");
            trailerplaca = Intent.GetStringExtra("placastrailer");
            fechatrailer = Intent.GetStringExtra("fechatrailer");
            conse = Intent.GetStringExtra("conse");
            imei = Intent.GetStringExtra("imei");


            posicion.Text = "Posicion: " + posicionactual;
            orden.Text = "Orden de venta: " + ordenventa;
            /*
             intent.PutExtra("responsable", responsable.ToString().Trim());G
            intent.PutExtra("OrdenVenta", pedido.Text.ToString().Trim());
            intent.PutExtra("Posicion", Posicion.Text.ToString().Trim());
            intent.PutExtra("placastrailer", Notrailer.Text.Trim());
            intent.PutExtra("fechatrailer", fecha.Text.Trim());
            intent.PutExtra("conse", fecha.Text.Trim());
             */

            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }

            string Cadproduct = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) " +
                   "VALUES(GETDATE(),'CEL " + imei + "','" + responsable.Trim() + "','F','7.10','" + ordenventa + "','Posicion " + posicionactual + " Formulario Subir Foto ','CARGAEMB','" + ordenventa + "')";
            SqlCommand cm = new SqlCommand(Cadproduct, thisConnection);
            cm.ExecuteNonQuery();
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Subir Foto";
        }

        private void Guardar_Click(object sender, EventArgs e)
        {
            //string nombrefoto = subir_firma(pathimagen);

            /*thisConnection.Open();
            cmnd1 = thisConnection.CreateCommand();
            cmnd1.CommandText = "insert into Tb_Det_Rep_Recorrido (Ubicacion, detalle, nombrefoto, latitud, longitud, fecha, responsable) values ('" + ubicacion.Text + "', '" + incidencia.Text + "', '" + nombrefoto + "', '" + latitud + "', '" + longitud + "', GETDATE(), '" + responsable + "')";
            reader1 = cmnd1.ExecuteReader();
            reader1.Dispose();
            thisConnection.Close();*/

            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle(Html.FromHtml("<font color='#68E36C' size = 10>Foto Subida Correctamente</font>"));
            alertDialog.SetIcon(Resource.Drawable.exito);
            alertDialog.SetCancelable(false);
            alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El reporte se ha enviado de manera correcta</font>"));
            alertDialog.SetNeutralButton("Ok", delegate
            {
                alertDialog.Dispose();
                Captura.SetImageResource(Resource.Drawable.logo);
                //File.Delete(pathimagen);
                SetResult(Result.Ok);
                this.Finish();
            });
            alertDialog.Show();
        }

        private void camara_click(object sender, EventArgs e)
        {
            var xs = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);
            TakePhoto2();
        }

        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 40,
                Name = "evidencia.jpg",
                Directory = "CargaEmbarques"
            });


            if (file == null)
            {
                return;
            }

            pathimagen = file.Path;

            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            Android.Graphics.Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            Captura.SetImageBitmap(bitmap);
        }

        async void TakePhoto2()
        {
            try
            {
                // Inicializa el plugin de cámara
                await CrossMedia.Current.Initialize();

                // Verifica si la cámara está disponible
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Console.WriteLine("No se puede acceder a la cámara.");
                    return;
                }

                // Abre la cámara para capturar una foto
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 40,
                    Name = "evidencia.jpg",
                    Directory = "CargaEmbarques"
                });

                if (file == null)
                    return;

                pathimagen = file.Path;  // Guarda la ruta de la imagen

                // Convierte la imagen en un Bitmap para mostrarla en un ImageView de Android
                byte[] imageArray = File.ReadAllBytes(file.Path);
                Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
                Captura.SetImageBitmap(bitmap); // `Captura` es tu ImageView en la UI
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al tomar la foto: {ex.Message}");
            }
        }

        async void TakePhoto3()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Toast.MakeText(this, "No se puede acceder a la cámara", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 40,
                    Name = $"evidencia_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
                });

                if (file == null)
                {
                    Toast.MakeText(this, "No se tomó la foto", ToastLength.Short).Show();
                    return;
                }

                RunOnUiThread(() =>
                {
                    Bitmap bitmap = BitmapFactory.DecodeStream(file.GetStream());
                    Captura.SetImageBitmap(bitmap);
                });
            }
            catch (Java.Lang.Exception ex)
            {
                Toast.MakeText(this, $"Error: {ex.Message}", ToastLength.Long).Show();
            }
        }


        public string subir_firma(string path)
        {

            byte[] imageArray = System.IO.File.ReadAllBytes(path);

            //var proxy = new WebServiceFoto.webservicefotosSoapClient(WebServiceFoto.webservicefotosSoapClient.EndpointConfiguration.webservicefotosSoap);

            var proxy = new WebServiceFoto.webservicefotos();
            //var proxy = new WebServiceFoto189.webservicefotos();
            //var proxy = new WSFotosTrailer.WSFotosTrailer();

            string nombrefoto = "EM_" + fechatrailer.Replace("/", "").ToString() + "_" + conse.ToString() + "_" + posicionactual.ToString().Trim() + ".jpg";

            string xc = proxy.BajarRecibo(imageArray, nombrefoto, fechatrailer, conse, posicionactual);

            if (xc == "1")
            {
                Toast.MakeText(this, "Foto Guardada Correctamente", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, xc, ToastLength.Long).Show();
            }

            return xc;

        }

        protected override void OnResume()
        {
            base.OnResume();
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

    }
}