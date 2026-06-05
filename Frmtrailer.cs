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
using Android.Nfc;
using Android.Views;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace CargaEmbarques
{
    [Activity(Label = "INFORMACION DEL TRAILER", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Sensor)]
    public class Frmtrailer : Activity
    {
        #region VARIABLES GLOBALES DE LA CLASE
        public static string responsable;
        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        SqlDataAdapter da;
        DataSet ds = new DataSet();
        SqlCommand cmnd = new SqlCommand();
        SqlCommand cmnd1 = new SqlCommand();

        EditText txtfecha;
        TextView nuanden;
        //Radio button
        CheckBox Externo;
        CheckBox Particular;
        CheckBox Aguilares;

        EditText txtNo_trailer;
        Spinner numtrail;
        Spinner Turno;

        EditText destino;
        EditText chofer;

        TextView NombreSupervisor;

        string turnoactual = "";

        EditText temperaturaInicial;
        EditText Horainicial;

        EditText temperaturaFinal;
        EditText HoraFinal;

        Button Mas;
        Button Grabar;

        EditText claveanden;
        EditText claveadicional;


        EditText txtgatas;
        EditText txtlargo;
        EditText txttempsetpoint;
        EditText txtryan1;
        EditText txtposryan1;
        EditText txtryan2;
        EditText txtposryan2;

        EditText idanden;

        private NfcAdapter _nfcAdapter;
        //VARIABLES DE ALTA DEL DETALLE DEL TRAILER
        string concepto1 = "";
        string concepto2 = "";
        string concepto3 = "";
        string concepto4 = "";
        string concepto5A = "";
        string concepto5B = "";
        string concepto5C = "";
        string concepto5D = "";
        string concepto5E = "";
        string concepto5F = "";
        string concepto5G = "";
        string concepto5H = "";
        string concepto5I = "";
        string concepto5J = "";
        string concepto5K = "";
        string concepto5L = "";
        string concepto6 = "";
        string concepto7 = "";
        string concepto8 = "";
        string concepto9 = "";
        string concepto10 = "";
        string largo = "";
        string gatas = "";
        string ryan1 = "";
        string ryan2 = "";
        string setpointinicial = "";
        string posr1 = "";
        string posr2 = "";


        //****************+++++++++++++++++  ++++++++++++++++*******************

        string Id_Anden = "";
        string Clave = "";
        string Estado = "";
        string ClaveTablet = "";


        string AndenValidar = "";

        string Externos = "";

        IMenu Mymenu;

        ArrayAdapter<System.String> comboAdapter;
        string status = "";

        string imei = "";
        #endregion
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FrmTrailer);

            responsable = Intent.GetStringExtra("responsable");
            AndenValidar = Intent.GetStringExtra("Anden");

            AsignarAnden();

            txtfecha = FindViewById<EditText>(Resource.Id.txtfecha);
            nuanden = FindViewById<TextView>(Resource.Id.NumerAnden);
            //Radio button
            Externo = FindViewById<CheckBox>(Resource.Id.chkExt);
            Particular = FindViewById<CheckBox>(Resource.Id.chkparti);
            Aguilares = FindViewById<CheckBox>(Resource.Id.chkpagui);

            txtNo_trailer = FindViewById<EditText>(Resource.Id.txtNotrailer);
            numtrail = FindViewById<Spinner>(Resource.Id.cmbNotrailer);
            Turno = FindViewById<Spinner>(Resource.Id.cmbTurno);

            destino = FindViewById<EditText>(Resource.Id.Destin);
            chofer = FindViewById<EditText>(Resource.Id.Nomchofer);

            NombreSupervisor = FindViewById<TextView>(Resource.Id.NombreSupervisor);

            temperaturaInicial = FindViewById<EditText>(Resource.Id.TempIni);
            Horainicial = FindViewById<EditText>(Resource.Id.HoraIni);

            temperaturaFinal = FindViewById<EditText>(Resource.Id.TempFin);
            HoraFinal = FindViewById<EditText>(Resource.Id.HoraFin);

            Mas = FindViewById<Button>(Resource.Id.CajasCargadas);
            Grabar = FindViewById<Button>(Resource.Id.guardarbtn);

            Mas.Click += Mas_Click;
            Grabar.Click += Grabar_Click;

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            string cadena;
            SqlCommand cmd;
            SqlDataReader Info;


            #region cargarInfoTrailer
            cargarInfoTrailer();
            #endregion

            //nuanden.Text = "No Asignado";
            nuanden.Text = AndenValidar;
            Turno.Enabled = false;
            //clanden.Enabled = false;
            destino.Enabled = false;
            HoraFinal.Enabled = false;
            Horainicial.Enabled = false;
            temperaturaFinal.Enabled = false;
            temperaturaInicial.Enabled = false;
            chofer.Enabled = false;
            Grabar.Enabled = false;
            //Mas.Enabled = false;

            //txtNo_trailer.RequestFocus();
            numtrail.RequestFocus();
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(txtNo_trailer, ShowFlags.Implicit);

            HoraFinal.EditorAction += (sender, e) =>
            {
                if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next)
                {
                    Grabar.Enabled = true;
                    Grabar.RequestFocus();

                }
                else
                {
                    e.Handled = false;
                }
            };

            temperaturaFinal.EditorAction += (sender, e) =>
            {
                if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next)
                {
                    HoraFinal.Enabled = true;
                    HoraFinal.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.");
                    Grabar.Enabled = true;
                    Grabar.RequestFocus();
                }
                else
                {
                    e.Handled = false;
                }
            };


            Horainicial.EditorAction += (sender, e) =>
            {
                if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next)
                {
                    Mas.Enabled = true;
                    Mas.RequestFocus();

                }
                else
                {
                    e.Handled = false;
                }
            };

            temperaturaInicial.EditorAction += (sender, e) =>
            {

                if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next)
                {
                    Horainicial.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.");
                    Horainicial.Enabled = true;
                    Horainicial.RequestFocus();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(Horainicial, ShowFlags.Implicit);
                }
                else
                {
                    e.Handled = false;
                }
            };

            chofer.KeyPress += (sender, e) =>
            {
                if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter))
                {
                    if (chofer.Text.Contains("0") || chofer.Text.Contains("1") || chofer.Text.Contains("2") || chofer.Text.Contains("3") || chofer.Text.Contains("4") || chofer.Text.Contains("5") || chofer.Text.Contains("6") || chofer.Text.Contains("7") || chofer.Text.Contains("8") || chofer.Text.Contains("9"))
                    {
                        Toast.MakeText(this, "El Nombre del chofer debe ser un nombre valido, sin numeros", ToastLength.Long).Show();
                        chofer.Text = "";
                    }
                    else
                    {
                        if (chofer.Text.Contains(" "))
                        {
                            if (chofer.Text.Trim().Length > 10)
                            {
                                chofer.Text = chofer.Text.Trim().ToUpper();
                                if (Particular.Checked || Externo.Checked)
                                {
                                    temperaturaInicial.Text = "38";
                                    Horainicial.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.");
                                    Mas.Enabled = false;
                                    Grabar.Enabled = true;
                                    Grabar.RequestFocus();
                                }
                                else
                                {
                                    temperaturaInicial.Text = "38";
                                    Horainicial.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt").Replace("a. m.", "a.m.").Replace("p. m.", "p.m.");
                                    Mas.Enabled = true;
                                    Mas.RequestFocus();
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, "Debe incluir un nombre real valido", ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "Debe incluir un nombre y un apellido real valido", ToastLength.Long).Show();
                        }
                    }

                    chofer.Text = chofer.Text.ToUpper();
                    chofer.Enabled = true;
                    chofer.RequestFocus();
                }
                else
                {
                    e.Handled = false;
                }
            };

            destino.KeyPress += (sender, e) =>
            {
                if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter))
                {
                    if (destino.Text.Trim().Length > 0)
                    {
                        destino.Text = destino.Text.ToUpper();
                        chofer.Enabled = true;
                        chofer.RequestFocus();
                    }
                }
                else
                {
                    e.Handled = false;
                }
            };

            nuanden.KeyPress += (sender, e) =>
            {
                numtrail.SetSelection(-1);
                Turno.SetSelection(-1);
                nuanden.Text = "sin asignar";
                Mymenu.FindItem(Resource.Id.MnuClose).SetEnabled(false);
                Mymenu.FindItem(Resource.Id.MnuAdicional).SetEnabled(false);
                Mymenu.FindItem(Resource.Id.MnuReasigar).SetEnabled(false);
                Turno.Enabled = false;
                //clanden.Enabled = false;
                destino.Enabled = false;
                temperaturaInicial.Enabled = false;
                temperaturaFinal.Enabled = false;
                Horainicial.Enabled = false;
                HoraFinal.Enabled = false;
                chofer.Enabled = false;
                Grabar.Enabled = false;
                //Mas.Enabled = false;
                destino.Text = "";
                temperaturaFinal.Text = "";
                temperaturaInicial.Text = "";
                HoraFinal.Text = "";
                Horainicial.Text = "";
                chofer.Text = "";
                entertxtboxplaca();
                if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter))
                {
                    int registros = 0;
                    thisConnection.Open();
                    cadena = "Select FECHA,NO_TRAILER From tb_mstr_trailer Where guardar = 'N' AND NO_TRAILER = '" + txtNo_trailer.Text + "'";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        registros++;
                    }
                    thisConnection.Close();
                    if (registros > 1)
                    {
                        Toast.MakeText(this, "ERROR: EL TRAILER ACTUAL TIENE REGISTROS PENDIENTES POR CERRAR", ToastLength.Short).Show();
                    }
                    else
                    {
                        entertxtboxplaca();
                    }

                }
                else
                {
                    e.Handled = false;
                }

            };


            Externo.CheckedChange += Externo_CheckedChange;
            Particular.CheckedChange += Particular_CheckedChange;
            Aguilares.CheckedChange += Aguilares_CheckedChange;

            txtNo_trailer.EditorAction += TxtNotrailer_EditorAction;

            /*numtrail.ItemSelected += (sender, e) =>
            {
                Spinner spinner = (Spinner)sender;
                string trailer = spinner.GetItemAtPosition(e.Position).ToString();
                if (trailer != "TRAILER" || trailer is null)
                {
                    txtNo_trailer.Text = trailer;
                    TxtNotrailer_EditorAction(txtNo_trailer, new TextView.EditorActionEventArgs(false, Android.Views.InputMethods.ImeAction.Done, null));
                }
            };*/

            //cargatxtplaca();
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Registro Trailer";
        }

        private void TxtNotrailer_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                if ((e.ActionId == ImeAction.Done))
                {

                    numtrail.SetSelection(-1);
                    Turno.SetSelection(-1);
                    //nuanden.Text = "sin asignar";
                    Mymenu.FindItem(Resource.Id.MnuClose).SetEnabled(false);
                    Mymenu.FindItem(Resource.Id.MnuAdicional).SetEnabled(false);
                    Mymenu.FindItem(Resource.Id.MnuReasigar).SetEnabled(false);
                    Turno.Enabled = false;
                    //clanden.Enabled = false;
                    destino.Enabled = false;
                    temperaturaInicial.Enabled = false;
                    temperaturaFinal.Enabled = false;
                    Horainicial.Enabled = false;
                    HoraFinal.Enabled = false;
                    chofer.Enabled = false;
                    Grabar.Enabled = false;
                    Mas.Enabled = false;
                    destino.Text = "";
                    temperaturaFinal.Text = "";
                    temperaturaInicial.Text = "";
                    HoraFinal.Text = "";
                    Horainicial.Text = "";
                    chofer.Text = "";
                    concepto1 = "";
                    concepto2 = "";
                    concepto3 = "";
                    concepto4 = "";
                    concepto5A = "";
                    concepto5B = "";
                    concepto5C = "";
                    concepto5D = "";
                    concepto5E = "";
                    concepto5F = "";
                    concepto5G = "";
                    concepto5H = "";
                    concepto5I = "";
                    concepto5J = "";
                    concepto5K = "";
                    concepto5L = "";
                    concepto6 = "";
                    concepto7 = "";
                    concepto8 = "";
                    concepto9 = "";
                    concepto10 = "";
                    largo = "";
                    gatas = "";
                    ryan1 = "";
                    ryan2 = "";
                    setpointinicial = "";
                    posr1 = "";
                    posr2 = "";
                    entertxtboxplaca();
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        private void Grabar_Click(object sender, EventArgs e)
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

            string cadena = "";
            SqlCommand cmd;
            SqlDataReader Info;

            string szSQL, mFEC, MF;
            System.String[] strTrailerx;
            System.Collections.ArrayList listadetrailer = new System.Collections.ArrayList();

            if (status == "A")
            {
                if (txtNo_trailer.Text.Trim().Length == 0)
                {
                    Toast.MakeText(this, "Error: No se ha Capturado el No. de Trailer", ToastLength.Short).Show();
                    txtNo_trailer.Text = "";
                    txtNo_trailer.RequestFocus();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(txtNo_trailer, ShowFlags.Implicit);
                }
                if (destino.Text.Trim().Length == 0)
                {
                    Toast.MakeText(this, "Error: No se ha Capturado el Destino", ToastLength.Short).Show();
                    destino.Text = "";
                    destino.RequestFocus();
                }
                if (Horainicial.Text.Trim().Length == 0)
                {
                    Toast.MakeText(this, "Error: No se ha Capturado la Hora Inicial", ToastLength.Short).Show();
                    Horainicial.Text = "";
                    Horainicial.RequestFocus();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(Horainicial, ShowFlags.Implicit);
                }

                MF = DateTime.Now.ToString("dd/MM/yyyy");
                if (Particular.Checked)
                {
                    szSQL = "IF NOT EXISTS(SELECT No_Trailer FROM tb_mstr_trailer WHERE hora_trailer = '" + txtfecha.Text.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "' AND no_trailer = 'PC') Insert into tb_mstr_trailer (FECHA,HORA_TRAILER,NO_TRAILER, TURNO, DESTINO, TRANSPORTE, TEMPINI, TEMPFIN, HORAINI, HORAFIN, GUARDAR, ANDEN, TRANSFER,CHOFER,RESPONSABLE," +
                        "largo,gatas,ryan1,ryan2,concepto1,concepto2,concepto3,concepto4,concepto5a,concepto5b,concepto5c,concepto5d,concepto5e,concepto5f,concepto5g,concepto5h," +
                        "concepto5i,concepto5j,concepto5k,concepto5l,concepto6,concepto7,concepto8,concepto9,concepto10,PosRyan1,PosRyan2,peso," +
                        "conse, temp, HoraRegVig, HoraEnt, HoraSal, TiempoTot, Radio, Surtible, Placa, TiempoCar, PesoBascula, TempSetPoint)" +
                        " Values " + "('" + MF + "' ,'" + txtfecha.Text.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "' ,'" + txtNo_trailer.Text + "', '" + turnoactual + "', '" + destino.Text.Trim() + "', 'PC', '" +
                        temperaturaInicial.Text + "', '', '" + Horainicial.Text.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "','--:--','N','" + nuanden.Text.Trim() + "','N','" + chofer.Text.Trim() + "','" + responsable.Trim() + "'," +
                        "'','0','0','0','','','','','','','','','','','','','','','','','','','','','','0','0','0.0'," +
                        "'0','0','','','','','0','','','','0','')";

                }
                else if (Externo.Checked)
                {
                    szSQL = "IF NOT EXISTS(SELECT No_Trailer FROM tb_mstr_trailer WHERE hora_trailer = '" + txtfecha.Text.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "' AND no_trailer = '" + txtNo_trailer.Text.Trim() + "') Insert into tb_mstr_trailer (FECHA,HORA_TRAILER,NO_TRAILER, TURNO, DESTINO, TRANSPORTE, TEMPINI, TEMPFIN, HORAINI, HORAFIN, GUARDAR, ANDEN, TRANSFER,CHOFER,RESPONSABLE," +
                        "largo,gatas,ryan1,ryan2,concepto1,concepto2,concepto3,concepto4,concepto5a,concepto5b,concepto5c,concepto5d,concepto5e,concepto5f,concepto5g,concepto5h," +
                        "concepto5i,concepto5j,concepto5k,concepto5l,concepto6,concepto7,concepto8,concepto9,concepto10,PosRyan1,PosRyan2,peso," +
                        "conse, temp, HoraRegVig, HoraEnt, HoraSal, TiempoTot, Radio, Surtible, Placa, TiempoCar, PesoBascula, TempSetPoint, obs)" +
                        " Values " + "('" + MF + "' ,'" + txtfecha.Text.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "' ,'" + txtNo_trailer.Text + "', '" + turnoactual + "', '" + destino.Text.Trim() + "', 'EXTERNOS', '" +
                        temperaturaInicial.Text + "', '', '" + Horainicial.Text.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "','--:--','N','" + nuanden.Text.Trim() + "','N','" + chofer.Text.Trim() + "','" + responsable.Trim() + "'," +
                        "'','0','0','0','','','','','','','','','','','','','','','','','','','','','','0','0','0.0'," +
                        "'0','0','','','','','0','','','','0', '', 'EXTERNO CARGA ADICIONAL EMBARQUES')";
                }
                else
                {
                    if (txtryan1.Text.Trim().Length == 0)
                    {
                        Toast.MakeText(this, "Error: No se ha Capturado el No. de Ryan", ToastLength.Long).Show();
                        return;
                    }
                    if (txtposryan1.Text.Trim().Length == 0)
                    {
                        Toast.MakeText(this, "Error: No se ha Capturado la Posicion del Ryan", ToastLength.Long).Show();
                        return;
                    }
                    if (txttempsetpoint.Text.Trim().Length == 0)
                    {
                        Toast.MakeText(this, "Error: No se ha Capturado la Temp de Set Point", ToastLength.Long).Show();
                        return;
                    }
                    if (Aguilares.Checked)
                    {
                        if (txtryan2.Text.Trim().Length == 0)
                        {
                            txtryan2.Text = "0";
                            txtposryan2.Text = "0";
                        }
                        szSQL = "Insert into tb_mstr_trailer (FECHA,HORA_TRAILER,NO_TRAILER, TURNO, DESTINO, TRANSPORTE, TEMPINI, TEMPFIN, HORAINI, HORAFIN, GUARDAR, ANDEN, TRANSFER,CHOFER,RESPONSABLE," +
                                            "largo,gatas,ryan1,ryan2,concepto1,concepto2,concepto3,concepto4,concepto5a,concepto5b,concepto5c,concepto5d,concepto5e,concepto5f,concepto5g,concepto5h," +
                                            "concepto5i,concepto5j,concepto5k,concepto5l,concepto6,concepto7,concepto8,concepto9,concepto10,PosRyan1,PosRyan2,peso," +
                                            "conse, temp, HoraRegVig, HoraEnt, HoraSal, TiempoTot, Radio, Surtible, Placa, TiempoCar, PesoBascula)" +
                                            " Values " +
                                            "('" + MF + "' ,'" + txtfecha.Text + "' ,'" + txtNo_trailer.Text + "', '" + turnoactual + "', '" + destino.Text.Trim() + "', '', '" +
                                            temperaturaInicial.Text + "', '', '" + Horainicial.Text.Replace("a. m.", "a.m.").Replace("p. m.", "p.m.") + "','--:--','N','" + nuanden.Text + "','N','" + chofer.Text.Trim() + "','" + responsable.Trim() + "'," +
                                            "'" + largo.Trim() + "','" + gatas.Trim() + "','" + ryan1.Trim() + "','" + ryan2.Trim() + "','" + concepto1 + "','" + concepto2 + "','" + concepto3 + "','" +
                                            concepto4 + "','" + concepto5A + "','" + concepto5B + "','" + concepto5C + "','" + concepto5D + "','" + concepto5E + "','" + concepto5F + "','" + concepto5G + "','" +
                                            concepto5H + "','" + concepto5I + "','" + concepto5J + "','" + concepto5K + "','" + concepto5L + "','" + concepto6 + "','" + concepto7 + "','" + concepto8 + "','" +
                                            concepto9 + "','" + concepto10 + "','" + posr1.Trim() + "','" + posr2.Trim() + "','0.0','0','0','','','','','0','','','','0')";
                    }
                    else
                    {
                        szSQL = "UPDATE tb_mstr_trailer SET deaguilares = '', TURNO = '" + turnoactual + "', TEMPINI = '" + temperaturaInicial.Text + "'," +
                                        "ANDEN = '" + nuanden.Text + "', RESPONSABLE = '" + responsable + "', LARGO = '" + largo.Trim() + "', GATAS = '" + gatas.Trim() + "'," +
                                        "ryan1 = '" + ryan1.Trim() + "',ryan2 = '" + ryan2.Trim() + "',concepto1 = '" + concepto1 + "',concepto2 = '" + concepto2 + "'," +
                                        "concepto3 = '" + concepto3 + "',concepto4 = '" + concepto4 + "',concepto5i ='" + concepto5A + "',concepto5j ='" + concepto5B + "'," +
                                        "concepto5k ='" + concepto5K + "',concepto6 ='" + concepto6 + "',concepto7 ='" + concepto7 + "',concepto8 ='" + concepto8 + "'," +
                                        "concepto9 ='" + concepto9 + "',concepto10 ='" + concepto10 + "',PosRyan1 = '" + posr1.Trim() + "',PosRyan2 = '" + posr2.Trim() + "'," +
                                        "tempsetpoint='" + setpointinicial + "'" +
                                        " WHERE NO_TRAILER = '" + txtNo_trailer.Text + "' AND HORA_TRAILER = '" + txtfecha.Text + "'";
                    }
                }

                thisConnection.Open();
                cmd = new SqlCommand(szSQL, thisConnection);
                cmd.ExecuteNonQuery();
                thisConnection.Close();
                Toast.MakeText(this, "DATOS GRABADOS CON EXITO", ToastLength.Long).Show();
                if (AndenValidar != "99")
                {
                    Nuevo();
                }
                //Nuevo();
                cargarInfoTrailer();
            }
            else
            {
                string folio_pedido = "";
                string folio_embarque = "";
                string horafin = "";

                thisConnection.Open();
                try
                {
                    cadena = "SELECT pdn_folio, ISNULL(emb_folio, '') AS emb_folio, ISNULL(hora_fin, '') AS hora_fin FROM tb_mstr_pedidos_nal LEFT JOIN tb_mstr_embarque ON pdn_folio = emb_folio AND sts != 'C' Where placacaja = '" + txtNo_trailer.Text.Trim() + "' and pdn_fecha = '" + txtfecha.Text.Trim() + "' and pdn_estatus != 'C' UNION SELECT pdn_folio, ISNULL(emb_folio, '') AS emb_folio, ISNULL(hora_fin, '') AS hora_fin FROM tb_mstr_pedidos_exp LEFT JOIN tb_mstr_embarque ON pdn_folio = emb_folio AND sts != 'C' Where placacaja = '" + txtNo_trailer.Text.Trim() + "' and pdn_fecha = '" + txtfecha.Text.Trim() + "' and pdn_estatus != 'C' ORDER BY HORA_FIN desc";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    SqlDataReader Infox = cmd.ExecuteReader();
                    while (Infox.Read())
                    {
                        folio_embarque = Infox["emb_folio"].ToString().Trim();
                        folio_pedido = Infox["pdn_folio"].ToString().Trim();
                        horafin = Infox["hora_fin"].ToString().Trim();
                    }
                }
                catch
                {


                }
                thisConnection.Close();

                if (folio_pedido != "")
                {
                    if (folio_embarque == "")
                    {
                        Toast.MakeText(this, "Error: Hay Ordenes de Venta Pendientes Por Cargar", ToastLength.Long).Show();
                        return;

                    }
                    else
                    {
                        if (Convert.ToInt32(folio_embarque).ToString() == folio_pedido)
                        {
                            if (horafin.Trim() == "--:--")
                            {
                                Toast.MakeText(this, "Error: Hay Ordenes de Venta Pendientes Por Cerrar", ToastLength.Long).Show();
                                return;
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, " Error: Ordenes de Venta Pendientes por cargar a este trailer", ToastLength.Long).Show();
                            return;
                        }
                    }
                }
                if (txtNo_trailer.Text.Trim() != "PC" && Externos.Trim() != "EXTERNO CARGA ADICIONAL EMBARQUES")
                {
                    string porcentaje = "0";
                    thisConnection.Open();
                    cadena = "SELECT A.porcentaje FROM tb_det_revision_trailer AS A INNER JOIN tb_mstr_trailer AS B ON A.fecha = B.fecha AND A.conseini = B.conse WHERE B.NO_TRAILER = '" + txtNo_trailer.Text.Trim() + "' AND B.HORA_TRAILER = '" + txtfecha.Text.Trim() + "'";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        porcentaje = Info["porcentaje"].ToString().Trim();
                    }
                    thisConnection.Close();

                    if (porcentaje.Replace(".00", "") != "100")
                    {
                        Toast.MakeText(this, "Error: No se Puede Cerrar El Trailer porque las Fotos de Verificacion no estan completadas al 100%", ToastLength.Long).Show();
                        return;
                    }
                }

                // *** INTEGRACIÓN DEL DIÁLOGO DE OBSERVACIONES ***
                // Se llama al método pasando el folio del embarque (folio_pedido) y un callback que realizará el cierre.
                MostrarDialogoObservacionesSiAplica(folio_pedido, (observacionSeleccionada) =>
                {
                    // Este código se ejecutará cuando:
                    // - No aplican condiciones (observacionSeleccionada = "") o
                    // - El usuario seleccionó una observación y aceptó.
                    mFEC = Convert.ToString(HoraFinal.Text);
                    string sqlUpdate;
                    if (string.IsNullOrEmpty(observacionSeleccionada))
                    {
                        sqlUpdate = "UPDATE tb_mstr_trailer SET TEMPFIN = @tempFin, HORAFIN = @horaFin, GUARDAR = 'S' WHERE NO_TRAILER = @noTrailer AND HORA_TRAILER = @horaTrailer";
                    }
                    else
                    {
                        sqlUpdate = "UPDATE tb_mstr_trailer SET TEMPFIN = @tempFin, HORAFIN = @horaFin, GUARDAR = 'S', obs = @obs WHERE NO_TRAILER = @noTrailer AND HORA_TRAILER = @horaTrailer";
                    }

                    using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, thisConnection))
                    {
                        cmdUpdate.Parameters.AddWithValue("@tempFin", temperaturaFinal.Text);
                        cmdUpdate.Parameters.AddWithValue("@horaFin", mFEC.Replace("a. m.", "a.m.").Replace("p. m.", "p.m."));
                        cmdUpdate.Parameters.AddWithValue("@noTrailer", txtNo_trailer.Text);
                        cmdUpdate.Parameters.AddWithValue("@horaTrailer", txtfecha.Text);
                        if (!string.IsNullOrEmpty(observacionSeleccionada))
                            cmdUpdate.Parameters.AddWithValue("@obs", observacionSeleccionada);

                        thisConnection.Open();
                        cmdUpdate.ExecuteNonQuery();
                        thisConnection.Close();
                    }

                    Toast.MakeText(this, "DATOS GRABADOS CON EXITO", ToastLength.Long).Show();

                    // Actualizar la lista de trailers en el Spinner (código original)
                    thisConnection.Open();
                    string Cadena = "Select Count(NO_TRAILER) From tb_mstr_trailer Where guardar = 'N'  AND Anden = '" + AndenValidar + "'";
                    SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
                    string valor = Convert.ToString(cmdx.ExecuteScalar());
                    strTrailerx = new System.String[Convert.ToInt32(valor) + 1];
                    strTrailerx[0] = "";
                    thisConnection.Close();
                    int x = 1;
                    thisConnection.Open();
                    cadena = "Select NO_TRAILER From tb_mstr_trailer Where guardar = 'N' AND Anden = '" + AndenValidar + "' order by NO_TRAILER";
                    cmd = new SqlCommand(cadena);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        strTrailerx[x] = Info["NO_TRAILER"].ToString().Trim();
                        x++;
                    }
                    thisConnection.Close();
                    Collections.AddAll(listadetrailer, strTrailerx);
                    numtrail.Adapter = null;
                    comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, strTrailerx);
                    numtrail.Adapter = comboAdapter;
                    numtrail.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_Trailer);
                    if (AndenValidar != "99")
                    {
                        Nuevo();
                    }
                    cargarInfoTrailer();
                });
                return; // Importante: salir del evento para no ejecutar el código antiguo
            }
            thisConnection.Open();
            string Cadena2 = "Select Count(NO_TRAILER) From tb_mstr_trailer Where guardar = 'N'  AND Anden = '" + AndenValidar + "'";
            SqlCommand cmdx2 = new SqlCommand(Cadena2, thisConnection);
            string valor2 = Convert.ToString(cmdx2.ExecuteScalar());
            strTrailerx = new System.String[Convert.ToInt32(valor2) + 1];
            strTrailerx[0] = "";
            thisConnection.Close();
            int x2 = 1;
            thisConnection.Open();
            cadena = "Select NO_TRAILER From tb_mstr_trailer Where guardar = 'N' AND Anden = '" + AndenValidar + "' order by NO_TRAILER";
            cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                strTrailerx[x2] = Info["NO_TRAILER"].ToString().Trim();
                x2++;
            }
            thisConnection.Close();
            Collections.AddAll(listadetrailer, strTrailerx);
            numtrail.Adapter = null;
            comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, strTrailerx);
            numtrail.Adapter = comboAdapter;
            numtrail.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_Trailer);
            if (AndenValidar != "99")
            {
                Nuevo();
            }
            //Nuevo();
            cargarInfoTrailer();
        }

        private void Mas_Click(object sender, EventArgs e)
        {
            System.Collections.ArrayList listadeopciones = new System.Collections.ArrayList();


            View view = LayoutInflater.Inflate(Resource.Layout.frmDetalleTrailer, null);
            //view.ScrollbarFadingEnabled = true;
            Android.App.AlertDialog builder = new Android.App.AlertDialog.Builder(this).Create();
            //builder.Window.SetLayout(100, 450);
            builder.SetView(view);
            builder.SetCanceledOnTouchOutside(false);

            Spinner cmbcon1 = view.FindViewById<Spinner>(Resource.Id.cmbcon1);
            Spinner cmbcon2 = view.FindViewById<Spinner>(Resource.Id.cmbcon2);
            Spinner cmbcon3 = view.FindViewById<Spinner>(Resource.Id.cmbcon3);
            Spinner cmbcon4 = view.FindViewById<Spinner>(Resource.Id.cmbcon4);
            Spinner cmbcon5A = view.FindViewById<Spinner>(Resource.Id.cmbcon5A);
            Spinner cmbcon5B = view.FindViewById<Spinner>(Resource.Id.cmbcon5B);
            Spinner cmbcon5C = view.FindViewById<Spinner>(Resource.Id.cmbcon5C);
            Spinner cmbcon5D = view.FindViewById<Spinner>(Resource.Id.cmbcon5D);
            Spinner cmbcon5E = view.FindViewById<Spinner>(Resource.Id.cmbcon5E);
            Spinner cmbcon5F = view.FindViewById<Spinner>(Resource.Id.cmbcon5F);
            Spinner cmbcon5G = view.FindViewById<Spinner>(Resource.Id.cmbcon5G);
            Spinner cmbcon5H = view.FindViewById<Spinner>(Resource.Id.cmbcon5H);
            Spinner cmbcon5I = view.FindViewById<Spinner>(Resource.Id.cmbcon5I);
            Spinner cmbcon5J = view.FindViewById<Spinner>(Resource.Id.cmbcon5J);
            Spinner cmbcon5K = view.FindViewById<Spinner>(Resource.Id.cmbcon5K);
            Spinner cmbcon5L = view.FindViewById<Spinner>(Resource.Id.cmbcon5L);
            Spinner cmbcon6 = view.FindViewById<Spinner>(Resource.Id.cmbcon6);
            Spinner cmbcon7 = view.FindViewById<Spinner>(Resource.Id.cmbcon7);
            Spinner cmbcon8 = view.FindViewById<Spinner>(Resource.Id.cmbcon8);
            Spinner cmbcon9 = view.FindViewById<Spinner>(Resource.Id.cmbcon9);
            Spinner cmbcon10 = view.FindViewById<Spinner>(Resource.Id.cmbcon10);
            txtgatas = view.FindViewById<EditText>(Resource.Id.txtgatas);
            txtlargo = view.FindViewById<EditText>(Resource.Id.txtlargo);
            txttempsetpoint = view.FindViewById<EditText>(Resource.Id.txttempsetpoint);
            txtryan1 = view.FindViewById<EditText>(Resource.Id.txtryan1);
            txtposryan1 = view.FindViewById<EditText>(Resource.Id.txtposryan1);
            txtryan2 = view.FindViewById<EditText>(Resource.Id.txtryan2);
            txtposryan2 = view.FindViewById<EditText>(Resource.Id.txtposryan2);
            txtgatas.LongClickable = false;
            txtlargo.LongClickable = false;
            txtryan1.LongClickable = false;
            txtposryan1.LongClickable = false;
            txtryan2.LongClickable = false;
            txtposryan2.LongClickable = false;
            txtlargo.Text = largo;
            txtgatas.Text = gatas;
            txtryan1.Text = ryan1;
            txtryan2.Text = ryan2;
            txttempsetpoint.Text = setpointinicial;
            txtposryan1.Text = posr1;
            txtposryan2.Text = posr2;

            //Llenado de Spinners
            string[] arrayOpciones = new string[] { "", "BIEN", "MAL" };
            Collections.AddAll(listadeopciones, arrayOpciones);
            comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, arrayOpciones);
            cmbcon1.Adapter = comboAdapter;
            if (concepto1.Trim() == "")
            {
                cmbcon1.SetSelection(0);
            }
            else if (concepto1.Trim() == "BIEN")
            {
                cmbcon1.SetSelection(1);
            }
            else
            {
                cmbcon1.SetSelection(2);
            }
            cmbcon2.Adapter = comboAdapter;
            if (concepto2.Trim() == "")
            {
                cmbcon2.SetSelection(0);
            }
            else if (concepto2.Trim() == "BIEN")
            {
                cmbcon2.SetSelection(1);
            }
            else
            {
                cmbcon2.SetSelection(2);
            }
            cmbcon3.Adapter = comboAdapter;
            if (concepto3.Trim() == "")
            {
                cmbcon3.SetSelection(0);
            }
            else if (concepto3.Trim() == "BIEN")
            {
                cmbcon3.SetSelection(1);
            }
            else
            {
                cmbcon3.SetSelection(2);
            }
            cmbcon4.Adapter = comboAdapter;
            if (concepto4.Trim() == "")
            {
                cmbcon4.SetSelection(0);
            }
            else if (concepto4.Trim() == "BIEN")
            {
                cmbcon4.SetSelection(1);
            }
            else
            {
                cmbcon4.SetSelection(2);
            }
            cmbcon5A.Adapter = comboAdapter;
            if (concepto5A.Trim() == "")
            {
                cmbcon5A.SetSelection(0);
            }
            else if (concepto5A.Trim() == "BIEN")
            {
                cmbcon5A.SetSelection(1);
            }
            else
            {
                cmbcon5A.SetSelection(2);
            }
            cmbcon5B.Adapter = comboAdapter;
            if (concepto5B.Trim() == "")
            {
                cmbcon5B.SetSelection(0);
            }
            else if (concepto5B.Trim() == "BIEN")
            {
                cmbcon5B.SetSelection(1);
            }
            else
            {
                cmbcon5B.SetSelection(2);
            }
            cmbcon5C.Adapter = comboAdapter;
            if (concepto5C.Trim() == "")
            {
                cmbcon5C.SetSelection(0);
            }
            else if (concepto5C.Trim() == "BIEN")
            {
                cmbcon5C.SetSelection(1);
            }
            else
            {
                cmbcon5C.SetSelection(2);
            }
            cmbcon5D.Adapter = comboAdapter;
            if (concepto5D.Trim() == "")
            {
                cmbcon5D.SetSelection(0);
            }
            else if (concepto5D.Trim() == "BIEN")
            {
                cmbcon5D.SetSelection(1);
            }
            else
            {
                cmbcon5D.SetSelection(2);
            }
            cmbcon5E.Adapter = comboAdapter;
            if (concepto5E.Trim() == "")
            {
                cmbcon5E.SetSelection(0);
            }
            else if (concepto5E.Trim() == "BIEN")
            {
                cmbcon5E.SetSelection(1);
            }
            else
            {
                cmbcon5E.SetSelection(2);
            }
            cmbcon5F.Adapter = comboAdapter;
            if (concepto5F.Trim() == "")
            {
                cmbcon5F.SetSelection(0);
            }
            else if (concepto5F.Trim() == "BIEN")
            {
                cmbcon5F.SetSelection(1);
            }
            else
            {
                cmbcon5F.SetSelection(2);
            }
            cmbcon5G.Adapter = comboAdapter;
            if (concepto5G.Trim() == "")
            {
                cmbcon5G.SetSelection(0);
            }
            else if (concepto5G.Trim() == "BIEN")
            {
                cmbcon5G.SetSelection(1);
            }
            else
            {
                cmbcon5G.SetSelection(2);
            }
            cmbcon5H.Adapter = comboAdapter;
            if (concepto5H.Trim() == "")
            {
                cmbcon5H.SetSelection(0);
            }
            else if (concepto5H.Trim() == "BIEN")
            {
                cmbcon5H.SetSelection(1);
            }
            else
            {
                cmbcon5H.SetSelection(2);
            }
            cmbcon5I.Adapter = comboAdapter;
            if (concepto5I.Trim() == "")
            {
                cmbcon5I.SetSelection(0);
            }
            else if (concepto5I.Trim() == "BIEN")
            {
                cmbcon5I.SetSelection(1);
            }
            else
            {
                cmbcon5I.SetSelection(2);
            }
            cmbcon5J.Adapter = comboAdapter;
            if (concepto5J.Trim() == "")
            {
                cmbcon5J.SetSelection(0);
            }
            else if (concepto5J.Trim() == "BIEN")
            {
                cmbcon5J.SetSelection(1);
            }
            else
            {
                cmbcon5J.SetSelection(2);
            }
            cmbcon5K.Adapter = comboAdapter;
            if (concepto5K.Trim() == "")
            {
                cmbcon5K.SetSelection(0);
            }
            else if (concepto5K.Trim() == "BIEN")
            {
                cmbcon5K.SetSelection(1);
            }
            else
            {
                cmbcon5K.SetSelection(2);
            }
            cmbcon5L.Adapter = comboAdapter;
            if (concepto5L.Trim() == "")
            {
                cmbcon5L.SetSelection(0);
            }
            else if (concepto5L.Trim() == "BIEN")
            {
                cmbcon5L.SetSelection(1);
            }
            else
            {
                cmbcon5L.SetSelection(2);
            }
            cmbcon6.Adapter = comboAdapter;
            if (concepto6.Trim() == "")
            {
                cmbcon6.SetSelection(0);
            }
            else if (concepto6.Trim() == "BIEN")
            {
                cmbcon6.SetSelection(1);
            }
            else
            {
                cmbcon6.SetSelection(2);
            }
            cmbcon7.Adapter = comboAdapter;
            if (concepto7.Trim() == "")
            {
                cmbcon7.SetSelection(0);
            }
            else if (concepto7.Trim() == "BIEN")
            {
                cmbcon7.SetSelection(1);
            }
            else
            {
                cmbcon7.SetSelection(2);
            }
            cmbcon8.Adapter = comboAdapter;
            if (concepto8.Trim() == "")
            {
                cmbcon8.SetSelection(0);
            }
            else if (concepto8.Trim() == "BIEN")
            {
                cmbcon8.SetSelection(1);
            }
            else
            {
                cmbcon8.SetSelection(2);
            }
            cmbcon9.Adapter = comboAdapter;
            if (concepto9.Trim() == "")
            {
                cmbcon9.SetSelection(0);
            }
            else if (concepto9.Trim() == "BIEN")
            {
                cmbcon9.SetSelection(1);
            }
            else
            {
                cmbcon9.SetSelection(2);
            }
            cmbcon10.Adapter = comboAdapter;
            if (concepto10.Trim() == "")
            {
                cmbcon10.SetSelection(0);
            }
            else if (concepto10.Trim() == "BIEN")
            {
                cmbcon10.SetSelection(1);
            }
            else
            {
                cmbcon10.SetSelection(2);
            }

            Button buttonaceptar = view.FindViewById<Button>(Resource.Id.Guardarinfotra);
            Button button = view.FindViewById<Button>(Resource.Id.Cancelarinfotra);
            button.Click += delegate
            {
                builder.Dismiss();
                return;
            };
            buttonaceptar.Click += delegate
            {
                concepto1 = cmbcon1.SelectedItem.ToString();
                concepto2 = cmbcon2.SelectedItem.ToString();
                concepto3 = cmbcon3.SelectedItem.ToString();
                concepto4 = cmbcon4.SelectedItem.ToString();
                concepto5A = cmbcon5A.SelectedItem.ToString();
                concepto5B = cmbcon5B.SelectedItem.ToString();
                concepto5C = cmbcon5C.SelectedItem.ToString();
                concepto5D = cmbcon5D.SelectedItem.ToString();
                concepto5E = cmbcon5E.SelectedItem.ToString();
                concepto5F = cmbcon5F.SelectedItem.ToString();
                concepto5G = cmbcon5G.SelectedItem.ToString();
                concepto5H = cmbcon5H.SelectedItem.ToString();
                concepto5I = cmbcon5I.SelectedItem.ToString();
                concepto5J = cmbcon5J.SelectedItem.ToString();
                concepto5K = cmbcon5K.SelectedItem.ToString();
                concepto5L = cmbcon5L.SelectedItem.ToString();
                concepto6 = cmbcon6.SelectedItem.ToString();
                concepto7 = cmbcon7.SelectedItem.ToString();
                concepto8 = cmbcon8.SelectedItem.ToString();
                concepto9 = cmbcon9.SelectedItem.ToString();
                concepto10 = cmbcon10.SelectedItem.ToString();
                largo = txtlargo.Text.Trim();
                gatas = txtgatas.Text.Trim();
                ryan1 = txtryan1.Text.Trim();
                ryan2 = txtryan2.Text.Trim();
                setpointinicial = txttempsetpoint.Text.Trim();
                posr1 = txtposryan1.Text.Trim();
                posr2 = txtposryan2.Text.Trim();

                builder.Dismiss();
                Grabar.Enabled = true;
                Grabar.RequestFocus();
            };
            builder.Show();
            //builder.Window.SetLayout(1000, 1500);
            cmbcon1.RequestFocus();
        }

        private void Aguilares_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (Aguilares.Checked == true)
            {
                txtNo_trailer.Text = "";
                txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                txtNo_trailer.RequestFocus();
                entertxtboxplaca();
                txtNo_trailer.Enabled = true;
                numtrail.Enabled = true;
                Externo.Checked = false;
                Particular.Checked = false;
            }
            else
            {
                txtNo_trailer.Text = "";
                txtNo_trailer.RequestFocus();
                numtrail.Enabled = true;
            }
        }

        private void Particular_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (Particular.Checked == true)
            {
                txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                if (AndenValidar != "99")
                {
                    asignarPC(AndenValidar, imei);
                    nuanden.Text = "99";
                    txtNo_trailer.Text = "PC";
                    txtNo_trailer.RequestFocus();
                }
                AsignarAnden();
                //entertxtboxplaca();
                txtNo_trailer.Enabled = true;
                numtrail.Enabled = true;
                Externo.Checked = false;
                Aguilares.Checked = false;
                cargarInfoTrailer();
            }
            else
            {
                if (AndenValidar == "99" && imei != "adb8b0f853917ed8")
                {
                    desasignarPC(AndenValidar, imei);
                }
                AsignarAnden();
                nuanden.Text = AndenValidar;
                txtNo_trailer.Text = "";
                txtNo_trailer.RequestFocus();
                numtrail.Enabled = true;
                cargarInfoTrailer();
            }
        }

        private void asignarPC(string andenValidar, string imei)
        {
            #region BAJA DE ANDEN ACTUAL
            /*thisConnection.Open();
            string strBajaAndenActual = "UPDATE tb_cat_anden SET Estado = 'B' WHERE ClaveTablet = '" + imei + "' AND Estado = 'A'";
            SqlCommand cmdBajaAndenActual = new SqlCommand(strBajaAndenActual, thisConnection);
            cmdBajaAndenActual.ExecuteNonQuery();
            thisConnection.Close();
            Toast.MakeText(this, "ASIGNACION DE ANDEN/TABLET PARA PC CORRECTA", ToastLength.Long).Show();*/
            #endregion


            #region ASIGNACION DE ANDEN PARA LOS PROPIOS CONDUCTOS
            thisConnection.Open();
            string strActualizarAnden = "UPDATE tb_cat_anden SET ClaveTablet = '" + imei + "' WHERE Id_Anden = 99 AND Estado = 'A' AND Clave = '53CE4ABD0167C0'";
            string strActualizarAnden2 = "IF ((SELECT ClaveTablet FROM Tb_Cat_Anden WHERE ClaveTablet = (SELECT IDDispositivo FROM tb_cat_EquiposSistemas WHERE IdEquipo = '10' AND NombreEquipo = 'TABLET-PC') AND Estado = 'A' AND Clave = '') = '" + imei + "') BEGIN UPDATE Tb_Cat_Anden SET Estado = 'B' WHERE ClaveTablet = (SELECT IDDispositivo FROM tb_cat_EquiposSistemas WHERE IdEquipo = '10' AND NombreEquipo = 'TABLET-PC') AND Estado = 'A' AND Clave = ''; UPDATE Tb_Cat_Anden SET ClaveTablet = (SELECT IDDispositivo FROM tb_cat_EquiposSistemas WHERE IdEquipo = '10' AND NombreEquipo = 'TABLET-PC') WHERE Clave = (SELECT Clave FROM Tb_Cat_Anden WHERE Id_Anden = '99') AND Estado = 'A'; END ELSE BEGIN UPDATE Tb_Cat_Anden SET ClaveTablet = '" + imei + "' WHERE Clave = (SELECT Clave FROM Tb_Cat_Anden WHERE Id_Anden = '99') AND Estado = 'A'; END";
            SqlCommand cmd = new SqlCommand(strActualizarAnden2, thisConnection);
            cmd.ExecuteNonQuery();
            thisConnection.Close();
            Toast.MakeText(this, "ASIGNACION DE ANDEN/TABLET PARA PC CORRECTA", ToastLength.Long).Show();
            #endregion

            //RunOnUiThread(new Action(() => { nuanden.Text = AndenValidar; }));
        }

        private void desasignarPC(string andenValidar, string imei)
        {
            #region BAJA DE ANDEN ACTUAL (PROPIO CONDUCTO)
            thisConnection.Open();
            string strActualizarAnden = "UPDATE tb_cat_anden SET ClaveTablet = (SELECT IDDispositivo FROM tb_cat_EquiposSistemas WHERE IdEquipo = '10' AND NombreEquipo = 'TABLET-PC') WHERE Id_Anden = 99 AND Estado = 'A' AND Clave = '53CE4ABD0167C0'";
            SqlCommand cmd = new SqlCommand(strActualizarAnden, thisConnection);
            cmd.ExecuteNonQuery();
            thisConnection.Close();
            Toast.MakeText(this, "ASIGNACION DE ANDEN/TABLET PARA PC CORRECTA", ToastLength.Long).Show();
            #endregion


            #region ASIGNACION DE ANDEN ORIGINAL
            /*thisConnection.Open();
            string strBajaAndenActual = "UPDATE tb_cat_anden SET Estado = 'A' WHERE ClaveTablet = '" + imei + "' AND Estado = 'B'";
            SqlCommand cmdBajaAndenActual = new SqlCommand(strBajaAndenActual, thisConnection);
            cmdBajaAndenActual.ExecuteNonQuery();
            thisConnection.Close();
            Toast.MakeText(this, "ASIGNACION DE ANDEN/TABLET PARA PC CORRECTA", ToastLength.Long).Show();*/
            #endregion
        }

        private void entertxtboxplaca()
        {
            //
            int hay = 0;
            Mas.Enabled = false;
            thisConnection.Open();
            string cadena = "Select * from tb_mstr_trailer Where NO_TRAILER = '" + txtNo_trailer.Text + "' and guardar = 'N'";
            SqlCommand cmd = new SqlCommand(cadena);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                status = "A";
                txtfecha.Text = Info["HORA_TRAILER"].ToString().Trim();
                Turno.SetSelection(Convert.ToInt32(Info["TURNO"].ToString().Trim()));
                destino.Text = Info["DESTINO"].ToString().Trim();
                temperaturaInicial.Text = Info["TEMPINI"].ToString().Trim();
                temperaturaFinal.Text = Info["TEMPFIN"].ToString().Trim();
                Horainicial.Text = Info["HORAINI"].ToString().Trim();
                HoraFinal.Text = Info["HORAFIN"].ToString().Trim();
                nuanden.Text = Info["ANDEN"].ToString().Trim();
                string status_aguilares = Info["deaguilares"].ToString().Trim();
                if (Horainicial.Text != "--:--" && status_aguilares != "A")
                {
                    hay = 1;
                }
                chofer.Text = Info["CHOFER"].ToString().Trim();
                NombreSupervisor.Text = Info["responsable"].ToString().Trim();
                if (NombreSupervisor.Text.Trim().Length == 0)
                {
                    NombreSupervisor.Text = responsable;
                }

                concepto1 = Info["concepto1"].ToString().Trim();
                concepto2 = Info["concepto2"].ToString().Trim();
                concepto3 = Info["concepto3"].ToString().Trim();
                concepto4 = Info["concepto4"].ToString().Trim();
                concepto5A = Info["concepto5a"].ToString().Trim();
                concepto5B = Info["concepto5b"].ToString().Trim();
                concepto5C = Info["concepto5c"].ToString().Trim();
                concepto5D = Info["concepto5d"].ToString().Trim();
                concepto5E = Info["concepto5e"].ToString().Trim();
                concepto5F = Info["concepto5f"].ToString().Trim();
                concepto5G = Info["concepto5g"].ToString().Trim();
                concepto5H = Info["concepto5h"].ToString().Trim();
                concepto5I = Info["concepto5i"].ToString().Trim();
                concepto5J = Info["concepto5j"].ToString().Trim();
                concepto5K = Info["concepto5k"].ToString().Trim();
                concepto5L = Info["concepto5l"].ToString().Trim();
                concepto6 = Info["concepto6"].ToString().Trim();
                concepto7 = Info["concepto7"].ToString().Trim();
                concepto8 = Info["concepto8"].ToString().Trim();
                concepto9 = Info["concepto9"].ToString().Trim();
                concepto10 = Info["concepto10"].ToString().Trim();
                largo = Info["largo"].ToString().Trim();
                gatas = Info["gatas"].ToString().Trim();
                posr1 = Info["posryan1"].ToString().Trim();
                posr2 = Info["posryan2"].ToString().Trim();
                ryan1 = Info["ryan1"].ToString().Trim();
                ryan2 = Info["ryan2"].ToString().Trim();
                setpointinicial = Info["tempsetpoint"].ToString().Trim();

            }
            thisConnection.Close();
            if (hay == 1)
            {
                Mymenu.FindItem(Resource.Id.MnuClose).SetEnabled(true);
                Mymenu.FindItem(Resource.Id.MnuAdicional).SetEnabled(true);
                Mymenu.FindItem(Resource.Id.MnuReasigar).SetEnabled(true);
                return;
            }
            else
            {
                if (nuanden.Text == "0")
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Trailer Sin Anden</font>"));
                    alertDialog.SetIcon(Resource.Drawable.warning);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Anden no ha sido Asignado por temperaturas, No se puede registrar trailer</font>"));
                    alertDialog.SetCancelable(false);
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        return;
                    });
                    alertDialog.Show();

                }


                if (Aguilares.Checked == false && Externo.Checked == false && Particular.Checked == false)
                {
                    if (concepto5A.Trim() == "" || concepto5B.Trim() == "")
                    {
                        Toast.MakeText(this, "ERROR: ESTA PENDIENTE LA CAPTURA DE INFORMACION POR PARTE DE BASCULA", ToastLength.Short).Show();
                        return;
                    }
                }
                status = "A";
                Mas.Enabled = false;
                Turno.Enabled = true;
                Turno.RequestFocus();
            }
            if ((nuanden.Text != "No Asignado") && (nuanden.Text != "sin asignar"))
            {
                if (Convert.ToInt32(nuanden.Text) != Convert.ToInt32(AndenValidar))
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>UNIDAD INCORRECTA</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden no se puede cargar en este Dispositivo, debido a que no esta designado para este anden</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        Nuevo();
                    });
                    alertDialog.Show();
                }
            }

        }

        private void cargatxtplaca()
        {
            //
            //SetContentView(Resource.Layout.LecturaCarga);
            string IdAnden = Intent.GetStringExtra("Anden");
            int hay = 0;

            thisConnection.Open();
            string cadenaEmbarque = "Select top(1) * From tb_MSTR_embarque A left JOIN tb_mstr_trailer B ON A.emb_folio=B.pdn_folio WHERE A.STS = 'C' AND A.anden in (select Id_Anden from Tb_Cat_Anden Where Id_Anden ='" + AndenValidar + "') order by A.anden";
            SqlCommand cmdEmbarque = new SqlCommand(cadenaEmbarque);
            cmdEmbarque.Connection = thisConnection;
            SqlDataReader InfoEmbarque = cmdEmbarque.ExecuteReader();
            while (InfoEmbarque.Read())
            {
                if (IdAnden != "")
                {
                    txtNo_trailer.Text = InfoEmbarque["NO_TRAILER"].ToString().Trim();
                    NombreSupervisor.Text = InfoEmbarque["RESPONSABLE"].ToString().Trim();
                    txtfecha.Text = InfoEmbarque["HORA_TRAILER"].ToString().Trim();
                    Turno.SetSelection(Convert.ToInt32(InfoEmbarque["TURNO"].ToString().Trim()));
                    destino.Text = InfoEmbarque["DESTINO"].ToString().Trim();
                    temperaturaInicial.Text = InfoEmbarque["TEMPINI"].ToString().Trim();
                    temperaturaFinal.Text = InfoEmbarque["TEMPFIN"].ToString().Trim();
                    Horainicial.Text = InfoEmbarque["HORAINI"].ToString().Trim();
                    HoraFinal.Text = InfoEmbarque["HORAFIN"].ToString().Trim();
                    nuanden.Text = InfoEmbarque["ANDEN"].ToString().Trim();
                    string status_aguilares = InfoEmbarque["deaguilares"].ToString().Trim();
                    if (Horainicial.Text != "--:--" && status_aguilares != "A")
                    {
                        hay = 1;
                    }
                    chofer.Text = InfoEmbarque["CHOFER"].ToString().Trim();
                    if (NombreSupervisor.Text.Trim().Length == 0)
                    {
                        NombreSupervisor.Text = responsable;
                    }

                    if (InfoEmbarque["emb_folio"].ToString().Trim() != "")
                    {
                        Mas.Enabled = true;
                    }

                    concepto1 = InfoEmbarque["concepto1"].ToString().Trim();
                    concepto2 = InfoEmbarque["concepto2"].ToString().Trim();
                    concepto3 = InfoEmbarque["concepto3"].ToString().Trim();
                    concepto4 = InfoEmbarque["concepto4"].ToString().Trim();
                    concepto5A = InfoEmbarque["concepto5a"].ToString().Trim();
                    concepto5B = InfoEmbarque["concepto5b"].ToString().Trim();
                    concepto5C = InfoEmbarque["concepto5c"].ToString().Trim();
                    concepto5D = InfoEmbarque["concepto5d"].ToString().Trim();
                    concepto5E = InfoEmbarque["concepto5e"].ToString().Trim();
                    concepto5F = InfoEmbarque["concepto5f"].ToString().Trim();
                    concepto5G = InfoEmbarque["concepto5g"].ToString().Trim();
                    concepto5H = InfoEmbarque["concepto5h"].ToString().Trim();
                    concepto5I = InfoEmbarque["concepto5i"].ToString().Trim();
                    concepto5J = InfoEmbarque["concepto5j"].ToString().Trim();
                    concepto5K = InfoEmbarque["concepto5k"].ToString().Trim();
                    concepto5L = InfoEmbarque["concepto5l"].ToString().Trim();
                    concepto6 = InfoEmbarque["concepto6"].ToString().Trim();
                    concepto7 = InfoEmbarque["concepto7"].ToString().Trim();
                    concepto8 = InfoEmbarque["concepto8"].ToString().Trim();
                    concepto9 = InfoEmbarque["concepto9"].ToString().Trim();
                    concepto10 = InfoEmbarque["concepto10"].ToString().Trim();
                    largo = InfoEmbarque["largo"].ToString().Trim();
                    gatas = InfoEmbarque["gatas"].ToString().Trim();
                    posr1 = InfoEmbarque["posryan1"].ToString().Trim();
                    posr2 = InfoEmbarque["posryan2"].ToString().Trim();
                    ryan1 = InfoEmbarque["ryan1"].ToString().Trim();
                    ryan2 = InfoEmbarque["ryan2"].ToString().Trim();
                    setpointinicial = InfoEmbarque["tempsetpoint"].ToString().Trim();
                }

            }
            thisConnection.Close();

            if (nuanden.Text == "0")
            {
                Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Trailer Sin Anden</font>"));
                alertDialog.SetIcon(Resource.Drawable.warning);
                alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Anden no ha sido Asignado por temperaturas, No se puede registrar trailer</font>"));
                alertDialog.SetCancelable(false);
                alertDialog.SetNeutralButton("Ok", delegate
                {
                    alertDialog.Dispose();
                    return;
                });
                alertDialog.Show();

            }


            if (Aguilares.Checked == false && Externo.Checked == false && Particular.Checked == false)
            {
                if (concepto5A.Trim() == "" || concepto5B.Trim() == "")
                {
                    Toast.MakeText(this, "ERROR: ESTA PENDIENTE LA CAPTURA DE INFORMACION POR PARTE DE BASCULA", ToastLength.Short).Show();
                    return;
                }
            }
            status = "A";
            Mas.Enabled = false;
            Turno.Enabled = true;
            Turno.RequestFocus();

            if ((nuanden.Text != "No Asignado") && (nuanden.Text != "sin asignar"))
            {
                if (Convert.ToInt32(nuanden.Text) != Convert.ToInt32(AndenValidar))
                {
                    Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                    alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>UNIDAD INCORRECTA</font>"));
                    alertDialog.SetIcon(Resource.Drawable.Info);
                    alertDialog.SetCancelable(false);
                    alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>La Orden no se puede cargar en este Dispositivo, debido a que no esta designado para este anden</font>"));
                    alertDialog.SetNeutralButton("Ok", delegate
                    {
                        alertDialog.Dispose();
                        Nuevo();
                    });
                    alertDialog.Show();
                }
            }

        }

        private void Externo_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (Externo.Checked == true)
            {
                txtNo_trailer.Text = "";
                txtfecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtNo_trailer.RequestFocus();
                txtNo_trailer.Enabled = true;
                numtrail.Enabled = true;
                Particular.Checked = false;
                Aguilares.Checked = false;
            }
            else
            {
                txtNo_trailer.Text = "";
                txtNo_trailer.RequestFocus();
                numtrail.Enabled = true;
            }
        }

        private void Turno_spinner(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            turnoactual = spinner.GetItemAtPosition(e.Position).ToString();
            if (turnoactual != "")
            {
                //clanden.Enabled = true;
                //clanden.RequestFocus();
                ValidarAnden();
            }
        }

        private void spinner_Trailer(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            if (AndenValidar == "99")
            {
                txtNo_trailer.Text = "PC";
            }
            else
            {
                txtNo_trailer.Text = spinner.GetItemAtPosition(e.Position).ToString();
            }

            txtNo_trailer.RequestFocus();
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(txtNo_trailer, ShowFlags.Implicit);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MenuTrailer, menu);
            Mymenu = menu;
            Nuevo();
            return base.OnCreateOptionsMenu(menu);
        }

        private void Nuevo()
        {
            txtNo_trailer.Text = "";
            txtfecha.Text = DateTime.Now.ToString("dd-MM-yyyy");
            status = "A";
            numtrail.SetSelection(-1);
            Turno.SetSelection(-1);
            Mymenu.FindItem(Resource.Id.MnuClose).SetEnabled(false);
            Mymenu.FindItem(Resource.Id.MnuAdicional).SetEnabled(false);
            Mymenu.FindItem(Resource.Id.MnuReasigar).SetEnabled(false);
            Turno.Enabled = false;
            //clanden.Enabled = false;
            destino.Enabled = false;
            temperaturaInicial.Enabled = false;
            temperaturaFinal.Enabled = false;
            Horainicial.Enabled = false;
            HoraFinal.Enabled = false;
            chofer.Enabled = false;
            //clanden.Text = "";
            destino.Text = "";
            temperaturaInicial.Text = "";
            temperaturaFinal.Text = "";
            Horainicial.Text = "";
            HoraFinal.Text = "";
            chofer.Text = "";
            Particular.Checked = false;
            Aguilares.Checked = false;
            Externo.Checked = false;
            concepto1 = "";
            concepto2 = "";
            concepto3 = "";
            concepto4 = "";
            concepto5A = "";
            concepto5B = "";
            concepto5C = "";
            concepto5D = "";
            concepto5E = "";
            concepto5F = "";
            concepto5G = "";
            concepto5H = "";
            concepto5I = "";
            concepto5J = "";
            concepto5K = "";
            concepto5L = "";
            concepto6 = "";
            concepto7 = "";
            concepto8 = "";
            concepto9 = "";
            concepto10 = "";
            Externos = "1";
            NombreSupervisor.Text = responsable.Trim();
            Grabar.Enabled = false;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (Convert.ToString(item.TitleFormatted) == "Terminar Carga")
            {
                Cerrar();
                if (AndenValidar == "99" && imei != "adb8b0f853917ed8")
                {
                    desasignarPC(AndenValidar, imei);
                    AsignarAnden();
                }

            }
            else if (Convert.ToString(item.TitleFormatted) == "Nuevo")
            {
                Nuevo();
            }
            else if (Convert.ToString(item.TitleFormatted) == "Adicional")
            {
                Adicional();
            }
            else if (Convert.ToString(item.TitleFormatted) == "Reasignar Aguilares")
            {
                ReasignarAguilares();
            }
            else if (Convert.ToString(item.TitleFormatted) == "Salir")
            {
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ReasignarAguilares()
        {
            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
            alertDialog.SetTitle(Html.FromHtml("<font color='#DF0101' size = 10>Reasignar Orden a Aguilares</font>"));
            alertDialog.SetIcon(Resource.Drawable.warning);
            alertDialog.SetCancelable(false);
            alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>EL TRAILER ACTUAL SE TERMINARÁ DE CARGAR EN AGUILARES... ¿DESEA CONTINUAR?</font>"));
            alertDialog.SetPositiveButton("Reasignar", (senderAlert, args) =>
            {
                thisConnection.Open();
                string Cadenareasignar = "UPDATE TB_MSTR_TRAILER SET Anden = '8', Responsable = 'J CONCEPCION RAZO PIZANO'  WHERE NO_TRAILER = '" + txtNo_trailer.Text + "' AND HORA_TRAILER = '" + txtfecha.Text + "'";
                SqlCommand cmdxreasignar = new SqlCommand(Cadenareasignar, thisConnection);
                cmdxreasignar.ExecuteNonQuery();
                thisConnection.Close();
                alertDialog.Dispose();
                Toast.MakeText(this, "Trailer Reasignado a Aguilares", ToastLength.Long).Show();
                Nuevo();
            });

            alertDialog.SetNegativeButton("Cancelar", (senderAlert, args) =>
            {
                return;
            });
            alertDialog.Show();
        }

        private void Adicional()
        {
            View view = LayoutInflater.Inflate(Resource.Layout.cargaAdicional, null);
            Android.App.AlertDialog builderxs = new Android.App.AlertDialog.Builder(this).Create();
            builderxs.SetView(view);
            builderxs.SetCanceledOnTouchOutside(false);
            claveanden = view.FindViewById<EditText>(Resource.Id.idanden);
            claveanden.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
            claveanden.LongClickable = false;

            claveadicional = view.FindViewById<EditText>(Resource.Id.idadicional);
            claveadicional.LongClickable = false;
            claveadicional.RequestFocus();
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(claveadicional, ShowFlags.Implicit);

            Button buttonaceptar = view.FindViewById<Button>(Resource.Id.CargarAdicional);
            Button button = view.FindViewById<Button>(Resource.Id.CancelarAdicional);
            button.Click += delegate
            {
                claveanden.Text = "";
                claveadicional.Text = "";
                builderxs.Cancel();
                return;
            };
            var deviceId = CrossDeviceInfo.Current.Id;
            string cadena = "";
            EditText idanden = view.FindViewById<EditText>(Resource.Id.idanden);
            if (thisConnection.State == ConnectionState.Closed) { thisConnection.Open(); }
            cadena = "select Clave from tb_cat_anden WHERE Estado='A' and ClaveTablet = '" + deviceId + "'";
            SqlCommand cmdCatAnden = new SqlCommand(cadena);
            cmdCatAnden.Connection = thisConnection;
            SqlDataReader InfoCatAnden = cmdCatAnden.ExecuteReader();
            while (InfoCatAnden.Read())
            {
                claveanden.Text = InfoCatAnden["Clave"].ToString().Trim();
                //Id_Anden = InfoCatAnden["Id_Anden"].ToString().Trim();
                //Clave = InfoCatAnden["Clave"].ToString().Trim();
                //Estado = InfoCatAnden["Estado"].ToString().Trim();
                //ClaveTablet = InfoCatAnden["ClaveTablet"].ToString().Trim();
            }
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }

            #region VALIDACLAVEANDENADICIONAL
            string idandenleido = "";
            thisConnection.Open();
            string cadenaClaveAnden = "Select ID_ANDEN From tb_cat_anden WHERE Clave = '" + claveanden.Text.Trim() + "' AND ClaveTablet = '" + deviceId + "'";
            SqlCommand cmd = new SqlCommand(cadenaClaveAnden, thisConnection);
            idandenleido = Convert.ToString(cmd.ExecuteScalar());
            if (idandenleido.Trim() == nuanden.Text.Trim())
            {
                claveadicional.Enabled = true;
                claveadicional.RequestFocus();
            }
            else
            {
                Toast.MakeText(this, "El anden no corresponde al trailer Verifiquelo", ToastLength.Short).Show();

            }
            thisConnection.Close();
            #endregion

            /*claveanden.KeyPress += (sender, e) =>
            {
                if ((e.Event.Action == KeyEventActions.Up) && (e.KeyCode == Keycode.Enter))
                {
                    string idandenleido = "";
                    thisConnection.Open();
                    string cadena = "Select ID_ANDEN From tb_cat_anden WHERE Clave = '" + claveanden.Text.Trim() + "'";
                    SqlCommand cmd = new SqlCommand(cadena, thisConnection);
                    idandenleido = Convert.ToString(cmd.ExecuteScalar());
                    if (idandenleido.Trim() == nuanden.Text.Trim())
                    {
                        claveadicional.Enabled = true;
                        claveadicional.RequestFocus();
                    }
                    else
                    {
                        Toast.MakeText(this, "El anden no corresponde al trailer Verifiquelo", ToastLength.Short).Show();

                    }
                    thisConnection.Close();
                }
                else
                {
                    e.Handled = false;
                }
            };*/

            string OK = "N";
            buttonaceptar.Click += delegate
            {
                string placatrailer = "1";
                string fechatrailer = "1";

                thisConnection.Open();
                string query = "Select hora_trailer, no_trailer From tb_det_pend_embarque WHERE claveunica = '" + claveadicional.Text.Trim() + "'";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = thisConnection;
                SqlDataReader Info = cmd.ExecuteReader();
                while (Info.Read())
                {
                    placatrailer = Info["no_trailer"].ToString().Trim();
                    fechatrailer = Info["hora_trailer"].ToString().Trim();
                }
                thisConnection.Close();

                if (placatrailer == "1" && fechatrailer == "1")
                {
                    Toast.MakeText(this, "EL Codigo leido no Existe", ToastLength.Short).Show();
                }
                else
                {
                    if (placatrailer.Trim() != txtNo_trailer.Text.Trim() && fechatrailer.Trim() == txtfecha.Text.Trim())
                    {
                        Toast.MakeText(this, "Esta Carga no corresponde a este Trailer, esta carga corresponde a la placa " + placatrailer.Trim(), ToastLength.Short).Show();
                        claveadicional.Text = "";
                        claveadicional.RequestFocus();
                    }
                    else if (placatrailer.Trim() == txtNo_trailer.Text.Trim() && fechatrailer.Trim() != txtfecha.Text.Trim())
                    {
                        Toast.MakeText(this, "EL Pendiente no se carga el dia de hoy, este pendiente se carga el " + fechatrailer.Trim(), ToastLength.Short).Show();
                        claveadicional.Text = "";
                        claveadicional.RequestFocus();
                    }
                    else if (placatrailer.Trim() == "" && fechatrailer.Trim() != "")
                    {
                        Toast.MakeText(this, "EL Pendiente de embarque aun no tiene un trailer Asignado", ToastLength.Short).Show();
                        claveadicional.Text = "";
                        claveadicional.RequestFocus();
                    }
                    else if (placatrailer.Trim() != txtNo_trailer.Text.Trim() && fechatrailer.Trim() != txtfecha.Text.Trim())
                    {
                        Toast.MakeText(this, "La carga adicional no se carga hoy y tampoco en este trailer!", ToastLength.Short).Show();
                        claveadicional.Text = "";
                        claveadicional.RequestFocus();
                    }
                    else
                    {
                        thisConnection.Open();
                        string Cadena = "UPDATE tb_det_pend_embarque set estatus = 'S' WHERE claveunica = '" + claveadicional.Text.Trim() + "' and estatus = 'A'";
                        SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
                        cmdx.ExecuteNonQuery();
                        thisConnection.Close();

                        Toast.MakeText(this, "Carga adicional Cargada Correctamente", ToastLength.Short).Show();

                        thisConnection.Open();
                        Cadena = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO) VALUES (GETDATE(),'" + System.Net.Dns.GetHostName() + "','','S','7.1','" + txtNo_trailer.Text + "','SURTIDO PENDIENTE EMBARQUES " + claveadicional.Text.Trim() + "','SIPGAB','" + txtNo_trailer.Text + "')";
                        cmdx = new SqlCommand(Cadena, thisConnection);
                        cmdx.ExecuteNonQuery();
                        thisConnection.Close();
                        claveadicional.Text = "";
                        claveanden.Text = "";
                        builderxs.Cancel();
                    }
                }
            };
            builderxs.Show();
        }

        private void Cerrar()
        {
            string hora_inicio = "";
            string OrdenVenta = "";
            string szSQL = "", mFEC = "";
            thisConnection.Open();
            string query = "SELECT horaini, pdn_folio, obs FROM tb_mstr_trailer WHERE no_trailer = '" + txtNo_trailer.Text.Trim() + "' AND hora_trailer = '" + txtfecha.Text.Trim() + "'";
            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                hora_inicio = Info["horaini"].ToString().Trim();
                OrdenVenta = Info["pdn_folio"].ToString().Trim();
                Externos = Info["obs"].ToString().Trim();
            }
            thisConnection.Close();

            int pendientes = 0;

            thisConnection.Open();
            query = "SELECT * FROM tb_det_pend_embarque WHERE no_trailer = '" + txtNo_trailer.Text.Trim() + "' AND hora_trailer = '" + txtfecha.Text.Trim() + "' AND estatus = 'A'";
            cmd = new SqlCommand(query);
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                pendientes++;
            }
            thisConnection.Close();
            if (pendientes > 0)
            {
                Toast.MakeText(this, "El trailer tiene Cargas Adicionales de Embarque Sin Surtir, Favor de leer la carga adicional, Cargarlo e Intentar de nuevo", ToastLength.Long).Show();
                return;
            }

            if (hora_inicio.Trim() == "--:--")
            {
                Toast.MakeText(this, "El trailer no tiene hora de inicio de carga, no se puede Cerrar", ToastLength.Short).Show();
            }
            else
            {
                if (txtNo_trailer.Text.Trim() == "PC" || Externos.Trim() == "EXTERNO CARGA ADICIONAL EMBARQUES")
                {
                    status = "M";
                    temperaturaFinal.Text = "";
                    temperaturaFinal.Enabled = true;
                    temperaturaFinal.RequestFocus();
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.ShowSoftInput(temperaturaFinal, ShowFlags.Implicit);
                }
                else
                {

                    View view = LayoutInflater.Inflate(Resource.Layout.ConfirmacionRyan, null);
                    Android.App.AlertDialog builder = new Android.App.AlertDialog.Builder(this).Create();
                    builder.SetView(view);
                    builder.SetCanceledOnTouchOutside(false);
                    EditText ryan1 = view.FindViewById<EditText>(Resource.Id.TXTRYAN01);
                    EditText ryan2 = view.FindViewById<EditText>(Resource.Id.TXTRYAN02);
                    EditText posryan1 = view.FindViewById<EditText>(Resource.Id.TxtPosR01);
                    EditText posryan2 = view.FindViewById<EditText>(Resource.Id.TxtPosR02);
                    ryan1.LongClickable = false;
                    ryan2.LongClickable = false;
                    posryan1.LongClickable = false;
                    posryan2.LongClickable = false;

                    thisConnection.Open();
                    query = "SELECT ryan1, ryan2, posryan1, posryan2 FROM tb_mstr_trailer WHERE NO_TRAILER = '" + txtNo_trailer.Text + "' AND HORA_TRAILER = '" + txtfecha.Text + "'";
                    cmd = new SqlCommand(query);
                    cmd.Connection = thisConnection;
                    Info = cmd.ExecuteReader();
                    while (Info.Read())
                    {
                        ryan1.Text = Info["ryan1"].ToString().Trim();
                        posryan1.Text = Info["posryan1"].ToString().Trim();
                        ryan2.Text = Info["ryan2"].ToString().Trim();
                        posryan2.Text = Info["posryan2"].ToString().Trim();
                    }
                    thisConnection.Close();


                    Button buttonaceptar = view.FindViewById<Button>(Resource.Id.ContinuarRYAN);
                    Button button = view.FindViewById<Button>(Resource.Id.CancelarRYAN);
                    button.Click += delegate
                    {
                        builder.Dismiss();
                        return;
                    };


                    buttonaceptar.Click += delegate
                    {
                        if (ryan1.Text.Trim() != "" && posryan1.Text.Trim() != "")
                        {
                            if (ryan1.Text.Trim().Length > 5 && posryan1.Text != "0")
                            {
                                builder.Dismiss();
                                thisConnection.Open();
                                string Cadena = "UPDATE tb_mstr_trailer SET ryan1 = '" + ryan1.Text + "', ryan2 = '" + ryan2.Text + "', posryan1 = '" + posryan1.Text + "', posryan2 = '" + posryan2.Text + "' WHERE NO_TRAILER = '" + txtNo_trailer.Text + "' AND HORA_TRAILER = '" + txtfecha.Text + "'";
                                SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
                                cmdx.ExecuteNonQuery();

                                Cadena = "INSERT INTO TB_REGISTRO_MOVIMIENTOS(FECHA,NOM_COMPU,NOM_USU,TIPO_MOV,OP_CLAVE,FOLIO,DETALLE,SISTEMA,MOV_FOLIO, obs) VALUES (GETDATE(),'" + System.Net.Dns.GetHostName() + "','LECTORA','CONFIRM','EMBRYAN','" + txtNo_trailer.Text + "','ACTUALIZACION DE RYAN A LA SALIDA DE TRAILER','SIPGAB','" + txtNo_trailer.Text + "', 'RYAN1: " + ryan1.Text + " POSICION1: " + posryan1.Text + ", RYAN2: " + ryan2.Text + " POSICION2: " + posryan2.Text + "')";
                                cmdx = new SqlCommand(Cadena, thisConnection);
                                cmdx.ExecuteNonQuery();
                                thisConnection.Close();
                                status = "M";
                                temperaturaFinal.Text = "";
                                temperaturaFinal.Enabled = true;
                                temperaturaFinal.RequestFocus();
                                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                                imm.ShowSoftInput(temperaturaFinal, ShowFlags.Implicit);

                            }
                            else
                            {
                                Toast.MakeText(this, "Los Datos del Ryan y su posicion deben ser reales", ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "Debe Ingresar Al Menos los Datos del Ryan1", ToastLength.Short).Show();
                        }
                    };
                    builder.Show();
                }
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            /* var alertMessage = new Android.App.AlertDialog.Builder(this).Create();
             var rawMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
             if (rawMessages == null) return;
             var msg = (NdefMessage)rawMessages[0];
             var record = msg.GetRecords()[0];
             if (record == null) return;
             // The data is defined by the Record Type Definition (RTD) specification available from http://members.nfc-forum.org/specs/spec_list/
             if (record.Tnf != NdefRecord.TnfWellKnown) return;
             // Get the transmitted data
             var data = Encoding.ASCII.GetString(record.GetPayload());
             data = data.Substring(3, data.Length - 3);*/

        }

        public static string ByteArrayToString(byte[] ba)
        {
            var shb = new SoapHexBinary(ba);
            return shb.ToString();
        }

        public bool validaservidores()
        {
            bool online = true;
            string[] sitios = new string[2];
            //sitios[0] = "http://192.168.123.4:81/EmbarquesApk/";
            //sitios[1] = "http://192.168.123.6";
            sitios[0] = "http://189.206.160.206:81/EmbarquesApk/";
            sitios[1] = "http://189.206.160.206";

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

        private void ValidarAnden()
        {
            string andenleido = "";
            string traileractual = "";

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

            andenleido = "";
            traileractual = "";
            thisConnection.Open();
            string cadena = "Select ID_ANDEN, no_trailer From tb_cat_anden LEFT JOIN tb_mstr_trailer ON Id_Anden = anden AND horafin = '--:--' AND Guardar = 'N' AND responsable != 'J CONCEPCION RAZO PIZANO' AND FECHA = CONVERT(varchar,getdate(),112) WHERE ClaveTablet = '" + imei + "'";
            SqlCommand cmd = new SqlCommand(cadena);
            cmd.CommandTimeout = 0;
            cmd.Connection = thisConnection;
            SqlDataReader Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                andenleido = Info["ID_ANDEN"].ToString().Trim();
                traileractual = Info["no_trailer"].ToString().Trim();
            }
            thisConnection.Close();

            if (Particular.Checked == true)
            {
                andenleido = "99";
            }

            if (traileractual == "" || traileractual == txtNo_trailer.Text.Trim())
            {
                if (andenleido != "99")
                {
                    if (Particular.Checked == true || Aguilares.Checked == true || Externo.Checked == true)
                    {
                        nuanden.Text = andenleido;
                        destino.Enabled = true;
                        destino.RequestFocus();
                    }
                    else
                    {
                        if (nuanden.Text.Trim() == andenleido)
                        {
                            temperaturaInicial.Enabled = true;
                            temperaturaInicial.Text = "";
                            temperaturaInicial.RequestFocus();
                            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                            imm.ShowSoftInput(temperaturaInicial, ShowFlags.Implicit);

                        }
                        else
                        {
                            Android.App.AlertDialog.Builder alertDialog = new Android.App.AlertDialog.Builder(this);
                            alertDialog.SetTitle(Html.FromHtml("<font color='#FFC107' size = 10>Anden No Correspondiente</font>"));
                            alertDialog.SetIcon(Resource.Drawable.warning);
                            alertDialog.SetMessage(Html.FromHtml("<font color='#000000' size = 10>El Trailer Actual no esta registrado en este anden, Favor de Verificarlo con Temperaturas</font>"));
                            alertDialog.SetCancelable(false);
                            alertDialog.SetNeutralButton("Ok", delegate
                            {
                                alertDialog.Dispose();
                                return;
                            });
                            alertDialog.Show();
                        }
                    }
                }
                else
                {
                    if (txtNo_trailer.Text == "PC")
                    {
                        nuanden.Text = andenleido;
                        destino.Enabled = true;
                        destino.RequestFocus();
                    }
                    else
                    {
                        Toast.MakeText(this, "El Anden Actual Solo puede ser usado por un Propio Conducto", ToastLength.Long).Show();
                        Nuevo();
                    }
                }
            }
            else
            {
                Toast.MakeText(this, "El Anden Actual Esta ocupado por el Trailer Numero " + traileractual, ToastLength.Long).Show();
                Nuevo();
            }
        }

        private void AsignarAnden()
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

            if (thisConnection.State == ConnectionState.Closed)
            {
                thisConnection.Open();
            }
            cmnd = thisConnection.CreateCommand();
            cmnd.CommandText = "select Id_Anden from Tb_Cat_Anden Where ClaveTablet = '" + imei + "' AND estado='A'";
            AndenValidar = Convert.ToInt32(cmnd.ExecuteScalar()).ToString();
            ds.Clear();
            if (thisConnection.State == ConnectionState.Open) { thisConnection.Close(); }
        }

        public void cargarInfoTrailer()
        {
            System.Collections.ArrayList listadeordenes = new System.Collections.ArrayList();
            System.Collections.ArrayList listadeturnos = new System.Collections.ArrayList();
            System.Collections.ArrayList listadetrailer = new System.Collections.ArrayList();
            System.String[] strTrailer;
            thisConnection.Open();
            string Cadena = "Select Count(NO_TRAILER) From tb_mstr_trailer Where guardar = 'N'  AND Anden = '" + AndenValidar + "'";
            SqlCommand cmdx = new SqlCommand(Cadena, thisConnection);
            string valor = Convert.ToString(cmdx.ExecuteScalar());
            strTrailer = new System.String[Convert.ToInt32(valor) + 1];
            strTrailer[0] = "TRAILER";
            thisConnection.Close();
            int x = 1;
            thisConnection.Open();
            string cadena = "Select NO_TRAILER From tb_mstr_trailer Where guardar = 'N'  AND Anden = '" + AndenValidar + "' order by NO_TRAILER";
            SqlCommand cmd = new SqlCommand(cadena);
            SqlDataReader Info;
            cmd.Connection = thisConnection;
            Info = cmd.ExecuteReader();
            while (Info.Read())
            {
                strTrailer[x] = Info["NO_TRAILER"].ToString().Trim();
                x++;
            }
            thisConnection.Close();
            Collections.AddAll(listadetrailer, strTrailer);
            comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, strTrailer);
            numtrail.Adapter = comboAdapter;
            numtrail.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_Trailer);

            NombreSupervisor.Text = responsable.Trim();
            //txtfecha.Text = DateTime.Now.ToString("dd-MM-yyyy");

            status = "A";

            string[] arrayturno = new string[] { "", "1", "2" };
            Collections.AddAll(listadeturnos, arrayturno);
            comboAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, arrayturno);
            Turno.Adapter = comboAdapter;
            Turno.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Turno_spinner);
        }

        // ==================== MÉTODO INTEGRADO ====================
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
            Android.App.AlertDialog.Builder dialogObs = new Android.App.AlertDialog.Builder(this);
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
    }
}