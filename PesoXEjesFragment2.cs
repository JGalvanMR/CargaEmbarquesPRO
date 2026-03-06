using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using CargaEmbarques.Modal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using static Android.App.LauncherActivity;
using static Android.Bluetooth.BluetoothClass;
using static Android.Icu.Text.IDNA;
using static Android.Icu.Text.Transliterator;
using Math = System.Math;

namespace CargaEmbarques
{
    [Activity(Label = "DETALLE DEL PESO DEL EMBARQUE")]
    public class PesoXEjesFragment2 : Android.App.Fragment
    {
        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        SqlDataAdapter da;
        DataSet ds = new DataSet();
        SqlCommand cmnd = new SqlCommand();
        SqlCommand cmnd1 = new SqlCommand();
        SqlDataReader reader1;
        public string numerotrailer, fechatrailerx, emb_tipo, pedido;
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


        #region ELEMENTOS VISUALES PARA ACTUALIZAR PESOS
        TextView txtNameTarima;
        TextView txtAgePesoTarima;
        TextView txtAgeUNO;
        TextView txtAgeDOS;
        TextView txtAgeTRES;
        TextView txtAgePesoTotal;
        ImageView imgTarima;
        #endregion

        #region VARIABLES UTILIZADAS PARA OBTENER EL PESO DE LA TARIMA
        public string recibo, prod_clave2, tarima2;
        decimal PesoTarima = 0;
        #endregion

        #region VARIABLES UTILIZADAS PARA OBTENER EL PESO DE LA TARIMA SPLIT
        public string codigoetiqueta;
        string SPLITno_lote, SPLITprod_clave, SPLITtarima, SPLITcajas;
        DataTable dtSplit = new DataTable();
        #endregion

        #region VARIABLES UTILIZADAS PARA OBTENER EL PESO DE LA CARGA DEL TRAILER
        decimal PesoCargaTrailer = 0;
        decimal PesoCargaTarimas = 0;
        decimal PesoTotalCargaTrailer = 0;
        #endregion

        #region VARIABLE UTILIZADAS PARA CALCULAR LA DISTRIBUCION DEL PESO POR EJES GROK
        // Constantes de pesos iniciales (kg)
        private const double InitialWeightAxle1 = 5140.0; // Eje simple
        private const double InitialWeightTandem23 = 12350.0; // Eje tandem 2-3
        private const double InitialWeightTandem45 = 4040.0; // Eje tandem 4-5

        // Constantes de dimensiones (m)
        private const double TrailerLength = 16.15;
        private const double PalletLength = 1.10;
        private const double PalletLengthB = 1;
        private const double DistanceAxle1ToTandem23 = 5.80;
        private const double DistanceTandem23ToTandem45 = 11.80;
        #endregion

        #region VARIABLE UTILIZADAS PARA CALCULAR LA DISTRIBUCION DEL PESO POR EJES DEEPSEEK
        // Constantes fijas
        private const double LargoTrailer = 16.15;
        private const double AnchoTrailer = 2.6;
        private const double LongitudPallet = 1.10;

        private const double DistanciaEje1ATandem23 = 5.80;
        private const double DistanciaTandem23ATandem45 = 11.80;
        private const double DistanciaTotalEjes = DistanciaEje1ATandem23 + DistanciaTandem23ATandem45;

        // Pesos iniciales de los ejes (tractor + semirremolque vacío)
        private const double PesoInicialEje1 = 5140;
        private const double PesoInicialTandem23 = 12350;
        private const double PesoInicialTandem45 = 4040;
        #endregion

        #region VARIABLE UTILIZADAS PARA VALIDAR EL LIMITE DE PESO MAXIMO LEGAL
        private const decimal PesoMaximoEje1 = 5443.108m;
        private const decimal PesoMaximoTandem23 = 15422.14m;
        private const decimal PesoMaximoTandem45 = 15422.14m;
        private const decimal PesoMaximoTotal = 36287.39m;
        LinearLayout LLPesosXEjesBG1;
        LinearLayout LLPesosXEjesBG2;
        LinearLayout LLPesosXEjesBG3;
        LinearLayout LLPesoTotalBG;
        #endregion

        string NombreProducto = "";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.PesoXEjes, container, false);

            // Obtener datos del Bundle (equivalente a Intent en Activity)
            numerotrailer = Arguments?.GetString("no_trailer") ?? "";
            fechatrailerx = Arguments?.GetString("hora_trailer") ?? "";

            recibo = Arguments?.GetString("recibo") ?? ""; //PARA CALCULAR PESO DE TARIMA
            prod_clave2 = Arguments?.GetString("prod_clave") ?? ""; //PARA CALCULAR PESO DE TARIMA
            tarima2 = Arguments?.GetString("tarima") ?? ""; //PARA CALCULAR PESO DE TARIMA

            codigoetiqueta = Arguments?.GetString("codigoetiqueta") ?? ""; //PARA CALCULAR PESO DEL SPLIT

            pedido = Arguments?.GetString("pdn_folio") ?? "";

            // Vincular UI
            //placas = view.FindViewById<TextView>(Resource.Id.trailerpla);
            //fecha = view.FindViewById<TextView>(Resource.Id.fectrai);
            //pedorigen = view.FindViewById<TextView>(Resource.Id.pediori);
            pesototalproducto = view.FindViewById<TextView>(Resource.Id.pestotalcarga);
            pesototaltrailer = view.FindViewById<TextView>(Resource.Id.pestotaltrailer);

            txtNameTarima = view.FindViewById<TextView>(Resource.Id.txtNameTarima);
            txtAgePesoTarima = view.FindViewById<TextView>(Resource.Id.txtAgePesoTarima);
            txtAgeUNO = view.FindViewById<TextView>(Resource.Id.txtAgeUNO);
            txtAgeDOS = view.FindViewById<TextView>(Resource.Id.txtAgeDOS);
            txtAgeTRES = view.FindViewById<TextView>(Resource.Id.txtAgeTRES);
            txtAgePesoTotal = view.FindViewById<TextView>(Resource.Id.txtAgePesoTotal);

            imgTarima = view.FindViewById<ImageView>(Resource.Id.imgTarima);

            LLPesosXEjesBG1 = view.FindViewById<LinearLayout>(Resource.Id.LLPesosXEjesBG1);
            LLPesosXEjesBG2 = view.FindViewById<LinearLayout>(Resource.Id.LLPesosXEjesBG2);
            LLPesosXEjesBG3 = view.FindViewById<LinearLayout>(Resource.Id.LLPesosXEjesBG3);
            LLPesoTotalBG = view.FindViewById<LinearLayout>(Resource.Id.LLPesoTotalBG);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "DETALLE DEL PESO DEL EMBARQUE";

            //CargarPesos(view);
            //CalcularPesoTotal(view);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            // Procesar argumentos solo si es la primera vez
            if (Arguments != null)
            {
                numerotrailer = Arguments.GetString("no_trailer") ?? "";
                fechatrailerx = Arguments.GetString("hora_trailer") ?? "";
                // ... otros argumentos

                // Limpiar arguments para futuras actualizaciones
                Arguments = null;

                // Calcular pesos iniciales si es necesario
                if (!string.IsNullOrEmpty(pedido))
                    CalcularPesoTotal(View);
            }
        }

        #region METODOS PARA OBTENER EL PESO DE LA ULTIMA LECTURA
        #region METODO UTILIZADO PARA OBTENER EL PESO DE LA ULTIMA TARIMA LEIDA
        public decimal getPesoTarima(string recibo, string prod_clave, string tarima)
        {
            decimal result = 0;
            string storedProc = "sp_ObtenerPesoPorTarima";
            using (SqlConnection conn = new SqlConnection(thisConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Recibo", recibo);
                    cmd.Parameters.AddWithValue("@Producto", prod_clave);
                    cmd.Parameters.AddWithValue("@Tarima", tarima);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal pesoTarima = reader["PesoPorTarima"] != DBNull.Value
                                                 ? Convert.ToDecimal(reader["PesoPorTarima"])
                                                 : 0;
                            NombreProducto = reader["NombreProducto"] != DBNull.Value ? reader["NombreProducto"].ToString() : "";
                            // Si quieres sumar 20 como antes
                            result = pesoTarima += 20;
                        }
                    }
                }
            }

            return result;
        }
        #endregion
        #region METODOS UTILLIZADOS PARA OBTENER EL PESO DE LOS SPLIT LEIDOS
        public decimal getPesoTotalPorSplit(string embFolio, int tarima, string estatus)
        {
            decimal pesoTotalAcumulado = 0;
            string storedProc = "sp_ObtenerPesoPorSplit";
            using (SqlConnection conn = new SqlConnection(thisConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmbFolio", embFolio);
                    cmd.Parameters.AddWithValue("@Tarima", tarima);
                    cmd.Parameters.AddWithValue("@Estatus", estatus);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["peso_total"] != DBNull.Value)
                            {
                                pesoTotalAcumulado += Convert.ToDecimal(reader["peso_total"]);
                            }
                        }
                    }
                }
            }
            return pesoTotalAcumulado + 20;
        }
        #endregion
        #endregion
        #region METODOS PARA OBTENER EL PESO DE LA CARGA DEL EMBARQUE
        public decimal getPesoCargaTrailer(string no_trailer, string hora_trailer, string emb_folio)
        {
            decimal PesoCargaTrailer = 0;
            string query = @"SELECT SUM(pesoneto) + (COUNT(DISTINCT seccion) * 20) AS PesoTotalCarga
                            FROM tb_det_embarque
                            WHERE emb_folio IN
                              (SELECT pdn_folio
                               FROM
                                (SELECT pdn_folio,
                                        pdn_fecha
                                 FROM tb_mstr_pedidos_nal
                                 WHERE (pdn_folio = @pdn_folio
                                        OR pdn_pedorigen = @pdn_folio)
                                 UNION ALL SELECT pdn_folio,
                                                  pdn_fecha
                                 FROM tb_mstr_pedidos_exp
                                 WHERE (pdn_folio = @pdn_folio
                                        OR pdn_pedorigen = @pdn_folio)) AS Pedidos
                               WHERE pdn_fecha = @pdn_fecha)";
            using (SqlConnection conn = new SqlConnection(thisConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pdn_folio", emb_folio);
                    cmd.Parameters.AddWithValue("pdn_fecha", hora_trailer);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PesoCargaTrailer = reader["PesoTotalCarga"] == DBNull.Value || Convert.ToDecimal(reader["PesoTotalCarga"]) == 0
                                ? (decimal)0
                                : Convert.ToDecimal(reader["PesoTotalCarga"]);
                        }
                    }
                }
            }
            return PesoCargaTrailer;
        }
        public DataTable getPesoCargaTrailerXPallet(string no_trailer, string hora_trailer, string emb_folio)
        {
            DataTable PesoCargaTrailer = new DataTable();
            string query = @"SELECT seccion, ISNULL(SUM(pesoneto), 0) AS Weight
                    FROM tb_det_embarque
                    WHERE emb_folio IN
                        (SELECT pdn_folio
                         FROM
                           (SELECT pdn_folio,
                                   pdn_fecha
                            FROM tb_mstr_pedidos_nal
                            WHERE (pdn_folio = @pdn_folio
                                   OR pdn_pedorigen = @pdn_folio)
                            UNION ALL 
                            SELECT pdn_folio,
                                   pdn_fecha
                            FROM tb_mstr_pedidos_exp
                            WHERE (pdn_folio = @pdn_folio
                                   OR pdn_pedorigen = @pdn_folio)) AS Pedidos
                         WHERE pdn_fecha = @pdn_fecha)
                    GROUP BY seccion
                    ORDER BY seccion ASC";

            using (SqlConnection conn = new SqlConnection(thisConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pdn_folio", emb_folio);
                    cmd.Parameters.AddWithValue("@pdn_fecha", hora_trailer);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) // Asignar el comando al adaptador
                    {
                        conn.Open();
                        adapter.Fill(PesoCargaTrailer);
                    }
                }
            }

            return PesoCargaTrailer;
        }
        #endregion
        #region NUEVO METODO PESOS POR EJES
        public void getPesosInicialesTrailer(string no_trailer, string hora_trailer)
        {
            string query = "SELECT pesoEje1, pesoEje2, pesoEje3, largo FROM tb_mstr_trailer WHERE no_trailer = @Trailer AND hora_trailer = @Hora";
            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                cmd.Parameters.AddWithValue("@Trailer", numerotrailer);
                cmd.Parameters.AddWithValue("@Hora", fechatrailerx);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pesoinicialeje1 = reader["pesoEje1"] == DBNull.Value || Convert.ToDecimal(reader["pesoEje1"]) == 0
                          ? (decimal)5140 // Valor predeterminado
                          : Convert.ToDecimal(reader["pesoEje1"]);
                        pesoinicialeje2 = reader["pesoEje2"] == DBNull.Value || Convert.ToDecimal(reader["pesoEje2"]) == 0
                          ? (decimal)12350 // Valor predeterminado
                          : Convert.ToDecimal(reader["pesoEje2"]);
                        pesoinicialeje3 = reader["pesoEje3"] == DBNull.Value || Convert.ToDecimal(reader["pesoEje3"]) == 0
                          ? (decimal)4040 // Valor predeterminado
                          : Convert.ToDecimal(reader["pesoEje3"]);
                        //LongitudTrailer = reader["largo"] == DBNull.Value || Convert.ToDecimal(reader["largo"]) == 0
                        //  ? (decimal)53 //valor predeterminado
                        //  : Convert.ToDecimal(reader["largo"]);
                    }
                }
            }
        }
        public void CalcularPesoTotal2(View view)
        {
            if (pedido != "" && codigoetiqueta == "" && recibo == "" && prod_clave2 == "" && tarima2 == "")
            {
                // Caso 1: Actualiza TODOS los controles (ejes + total)
                PesoTotalCargaTrailer = getPesoCargaTrailer(numerotrailer, fechatrailerx, pedido);
                var (axle1, tandem23, tandem45) = CalculateWeightDistribution(getPesoCargaTrailerXPallet(numerotrailer, fechatrailerx, pedido));

                AgregarPesoEje1(Convert.ToDecimal(axle1));
                AgregarPesoEje2y3(Convert.ToDecimal(tandem23));
                AgregarPesoEje4y5(Convert.ToDecimal(tandem45));
                AgregarPesoTotal(PesoTotalCargaTrailer);
                return;
            }
            else if (pedido != "" && codigoetiqueta != "" && recibo == "" && prod_clave2 == "" && tarima2 == "")
            {
                // Caso 2: Actualiza SOLO el peso de la última lectura (tarima/split)
                decimal resultado = getPesoTotalPorSplit(pedido, Convert.ToInt32(codigoetiqueta), "A");
                if (resultado != 0)
                {
                    ActualizarPesoUltimaLectura(resultado); // Solo txtAgePesoTarima y txtNameTarima
                }
                return;
            }
            else if (pedido == "" && codigoetiqueta == "" && recibo != "" && prod_clave2 != "" && tarima2 != "")
            {
                // Caso 3: Similar al anterior, pero para tarimas
                decimal resultado = getPesoTarima(recibo, prod_clave2, tarima2);
                if (resultado != 0)
                {
                    ActualizarPesoUltimaLectura(resultado); // Solo txtAgePesoTarima y txtNameTarima
                }
                return;
            }
        }

        public void CalcularPesoTotal(View view)
        {
            // Caso 1: Solo trailer (actualiza todos los controles)
            if (pedido != "" && codigoetiqueta == "" && recibo == "" && prod_clave2 == "" && tarima2 == "")
            {
                PesoTotalCargaTrailer = getPesoCargaTrailer(numerotrailer, fechatrailerx, pedido);
                var (axle1, tandem23, tandem45) = CalculateWeightDistribution(getPesoCargaTrailerXPallet(numerotrailer, fechatrailerx, pedido));

                AgregarPesoEje1(Convert.ToDecimal(axle1));
                AgregarPesoEje2y3(Convert.ToDecimal(tandem23));
                AgregarPesoEje4y5(Convert.ToDecimal(tandem45));
                //AgregarPesoTotal(PesoTotalCargaTrailer);
                AgregarPesoTotal(Convert.ToDecimal(axle1) + Convert.ToDecimal(tandem23) + Convert.ToDecimal(tandem45));
            }
            // Caso 2: Solo split (actualiza solo peso última lectura)
            else if (pedido != "" && codigoetiqueta != "" && recibo == "" && prod_clave2 == "" && tarima2 == "")
            {
                decimal resultado = getPesoTotalPorSplit(pedido, Convert.ToInt32(codigoetiqueta), "A");
                if (resultado != 0)
                {
                    ActualizarPesoUltimaLectura(resultado);
                }
            }
            // Caso 3: Solo tarima (actualiza solo peso última lectura)
            else if (pedido == "" && codigoetiqueta == "" && recibo != "" && prod_clave2 != "" && tarima2 != "")
            {
                decimal resultado = getPesoTarima(recibo, prod_clave2, tarima2);
                if (resultado != 0)
                {
                    ActualizarPesoUltimaLectura(resultado);
                }
            }
        }

        public void AgregarPesoEje1(decimal eje1)
        {
            Activity?.RunOnUiThread(() =>
            {
                if (txtAgeUNO != null)
                    txtAgeUNO.Text = eje1.ToString("F2") + " Kg.";

                if (LLPesosXEjesBG1 != null)
                {
                    if (eje1 > PesoMaximoEje1)
                    {
                        LLPesosXEjesBG1.SetBackgroundResource(Resource.Drawable.alert_background);
                    }
                    else
                    {
                        LLPesosXEjesBG1.SetBackgroundResource(Resource.Drawable.bg_neumorph_light); // Cambia a un fondo predeterminado
                    }
                }
            });
        }
        public void AgregarPesoEje2y3(decimal tandem23)
        {
            Activity?.RunOnUiThread(() =>
            {
                if (txtAgeDOS != null)
                    txtAgeDOS.Text = tandem23.ToString("F2") + " Kg.";

                if(LLPesosXEjesBG2 != null)
                {
                    if(tandem23 > PesoMaximoTandem23)
                    {
                        LLPesosXEjesBG2.SetBackgroundResource(Resource.Drawable.alert_background);
                    }
                    else
                    {
                        LLPesosXEjesBG2.SetBackgroundResource(Resource.Drawable.bg_neumorph_light);
                    }
                }
            });
        }
        public void AgregarPesoEje4y5(decimal tandem45)
        {
            Activity?.RunOnUiThread(() =>
            {
                if (txtAgeTRES != null)
                    txtAgeTRES.Text = tandem45.ToString("F2") + " Kg.";

                if (LLPesosXEjesBG3 != null)
                {
                    if (tandem45 > PesoMaximoTandem45)
                    {
                        LLPesosXEjesBG3.SetBackgroundResource(Resource.Drawable.alert_background);
                    }
                    else
                    {
                        LLPesosXEjesBG3.SetBackgroundResource(Resource.Drawable.bg_neumorph_light);
                    }
                }
            });
        }
        public void AgregarPesoTotal(decimal PesoTotal)
        {
            Activity?.RunOnUiThread(() =>
            {
                if (txtAgePesoTotal != null)
                    txtAgePesoTotal.Text = PesoTotal.ToString("F2") + " Kg.";

                if (LLPesoTotalBG != null)
                {
                    if (PesoTotal > PesoMaximoTotal)
                    {
                        LLPesoTotalBG.SetBackgroundResource(Resource.Drawable.alert_background);
                    }
                    else
                    {
                        LLPesoTotalBG.SetBackgroundResource(Resource.Drawable.bg_neumorph_light);
                    }
                }
            });
        }
        public void ActualizarPesoUltimaLectura(decimal PesoTarima)
        {
            Activity?.RunOnUiThread(() =>
            {
                if (txtAgePesoTarima != null)
                    txtAgePesoTarima.Text = PesoTarima.ToString("F2") + " Kg.";

                if (txtNameTarima != null)
                    txtNameTarima.Text = NombreProducto ?? "N/A";
            });
        }
        #endregion

        #region METODOS PUBLICOS PARA ACTUALIZACION SELECTIVA
        // Métodos públicos para actualización selectiva
        public void ActualizarDatosTrailer(string noTrailer, string horaTrailer, string pdnFolio)
        {
            Activity?.RunOnUiThread(() =>
            {
                numerotrailer = noTrailer;
                fechatrailerx = horaTrailer;
                pedido = pdnFolio;
                // Solo actualiza controles relacionados con trailer
                CalcularPesoTotal(View); // Esto ejecutará el caso 1
            });
        }

        public void ActualizarDatosTarima(string recibo, string prodClave, string tarima, string pdnFolio)
        {
            Activity?.RunOnUiThread(() =>
            {
                this.recibo = recibo;
                this.prod_clave2 = prodClave;
                this.tarima2 = tarima;
                this.pedido = pdnFolio;

                this.imgTarima.SetImageResource(Resource.Drawable.producto);

                // Solo actualiza controles relacionados con tarima
                CalcularPesoTotal(View); // Esto ejecutará el caso 3
            });
        }

        public void ActualizarDatosSplit(string splitCode, string pdnFolio)
        {
            Activity?.RunOnUiThread(() =>
            {
                this.codigoetiqueta = splitCode;
                this.pedido = pdnFolio;

                this.imgTarima.SetImageResource(Resource.Drawable.cargasplit);
                this.txtNameTarima.Text = pdnFolio + ", " + splitCode;
                
                // Solo actualiza controles relacionados con split
                CalcularPesoTotal(View); // Esto ejecutará el caso 2
            });
        }
        #endregion

        #region CALCULAR LA DISTRIBUCION DEL PESO POR EJES GROK
        /// <summary>
        /// Calcula la distribución del peso entre los ejes basado en los pesos de los pallets en un DataTable.
        /// </summary>
        /// <param name="palletWeightsTable">DataTable con una columna "Weight" (en kg) para los pesos de los pallets.</param>
        /// <returns>Tupla con los pesos en el eje 1, eje tandem (2-3) y eje tandem (4-5) en kg.</returns>
        public static (double axle1Weight, double tandem23Weight, double tandem45Weight)
            CalculateWeightDistribution(DataTable palletWeightsTable)
        {
            // Validar DataTable
            if (palletWeightsTable == null || !palletWeightsTable.Columns.Contains("Weight"))
                throw new ArgumentException("DataTable must contain a 'Weight' column.");

            int palletCount = palletWeightsTable.Rows.Count;
            if (palletCount < 0 || palletCount > (int)(TrailerLength / PalletLength) * 2)
            {
                if (palletCount < 0 || palletCount > (int)(TrailerLength / PalletLengthB) * 2)
                    throw new ArgumentException($"Pallet count must be between 0 and {(int)(TrailerLength / PalletLength) * 2}.");
            }
                

            // Calcular peso total de la carga
            double totalCargoWeight = 0;
            List<double> palletWeights = new List<double>();
            foreach (DataRow row in palletWeightsTable.Rows)
            {
                double weight = Convert.ToDouble(row["Weight"]);
                if (weight < 0)
                    throw new ArgumentException("Pallet weights cannot be negative.");
                totalCargoWeight += weight;
                palletWeights.Add(weight);
            }

            // Calcular posiciones y pesos consolidados por fila
            List<double> rowPositions = new List<double>();
            List<double> rowWeights = new List<double>();
            int fullRows = palletCount / 2;
            bool hasExtraPallet = palletCount % 2 == 1;

            // Procesar filas completas (2 pallets por fila)
            for (int i = 0; i < fullRows; i++)
            {
                double position = (i + 0.5) * PalletLength; // Centro de la fila
                double rowWeight = palletWeights[i * 2] + palletWeights[i * 2 + 1]; // Suma de los dos pallets
                rowPositions.Add(position);
                rowWeights.Add(rowWeight);
            }

            // Procesar pallet adicional si existe
            if (hasExtraPallet)
            {
                double position = (fullRows + 0.5) * PalletLength;
                rowPositions.Add(position);
                rowWeights.Add(palletWeights[palletCount - 1]);
            }

            // Calcular el momento total y el centro de gravedad de la carga
            double totalMoment = 0;
            for (int i = 0; i < rowPositions.Count; i++)
            {
                totalMoment += rowWeights[i] * rowPositions[i];
            }
            double cargoCenterOfGravity = palletCount > 0 ? totalMoment / totalCargoWeight : 0;

            // Posiciones de los ejes (relativas al eje 1 en x=0)
            double tandem23Position = DistanceAxle1ToTandem23;
            double tandem45Position = DistanceAxle1ToTandem23 + DistanceTandem23ToTandem45;

            // Calcular distribución del peso usando equilibrio de momentos
            double weightTandem45 = 0;
            double weightTandem23 = 0;
            double weightAxle1 = 0;

            if (palletCount > 0)
            {
                // Suma de momentos alrededor del eje 1: F23 * d23 + F45 * d45 = totalMoment
                // Suma de fuerzas: F1 + F23 + F45 = totalCargoWeight
                double d23 = tandem23Position;
                double d45 = tandem45Position;

                weightTandem45 = totalMoment / d45;
                weightTandem23 = (totalCargoWeight * (d45 - cargoCenterOfGravity) - totalMoment) / (d45 - d23);
                weightAxle1 = totalCargoWeight - weightTandem23 - weightTandem45;
            }

            // Sumar pesos iniciales
            weightAxle1 += InitialWeightAxle1;
            weightTandem23 += InitialWeightTandem23;
            weightTandem45 += InitialWeightTandem45;

            if (double.IsNaN(weightAxle1))
            {
                weightAxle1 = 0;
            }
            if (double.IsNaN(weightTandem23))
            {
                weightTandem23 = 0;
            }
            if (double.IsNaN(weightTandem45))
            {
                weightTandem45 = 0;
            }

            return (weightAxle1, weightTandem23, weightTandem45);
        }
        #endregion

        #region CALCULAR LA DISTRIBUCION DEL PESO POR EJES DEEPSEEK
        // Método principal mejorado
        public static (double eje1, double tandem23, double tandem45) CalcularDistribucionPeso(
            DataTable dataPallets,
            int cantidadTarimasCargadas)
        {
            // 1. Validar datos de entrada
            if (dataPallets == null) throw new ArgumentNullException(nameof(dataPallets));
            if (cantidadTarimasCargadas < 0) throw new ArgumentException("La cantidad de tarimas no puede ser negativa");

            // 2. Calcular capacidad máxima
            int capacidadMaxima = (int)(Math.Floor(LargoTrailer / LongitudPallet) * 2);
            if (cantidadTarimasCargadas > capacidadMaxima)
            {
                throw new ArgumentException($"La cantidad de tarimas ({cantidadTarimasCargadas}) excede la capacidad máxima del trailer ({capacidadMaxima})");
            }

            // 3. Inicializar pesos con los valores iniciales del vehículo
            double pesoEje1 = PesoInicialEje1;
            double pesoTandem23 = PesoInicialTandem23;
            double pesoTandem45 = PesoInicialTandem45;

            // 4. Procesar cada pallet cargado
            for (int i = 0; i < cantidadTarimasCargadas; i++)
            {
                if (i >= dataPallets.Rows.Count) break;

                // Obtener peso del pallet actual
                double pesoPallet = Convert.ToDouble(dataPallets.Rows[i]["Peso"]);

                // Calcular posición del pallet (considerando que se cargan en pares)
                int posicionPar = i / 2;
                double distanciaDesdeFrente = (posicionPar * LongitudPallet) + (LongitudPallet / 2);

                // Calcular distribución de este pallet en los ejes
                var (e1, t23, t45) = CalcularPesoPorEjeIndividual(pesoPallet, distanciaDesdeFrente);

                // Acumular pesos
                pesoEje1 += e1;
                pesoTandem23 += t23;
                pesoTandem45 += t45;
            }

            return (pesoEje1, pesoTandem23, pesoTandem45);
        }
        private static (double eje1, double tandem23, double tandem45) CalcularPesoPorEjeIndividual(
        double pesoPallet, double distanciaDesdeFrente)
        {
            // Convertir distancia desde frente a distancia desde el primer eje
            double distanciaDesdeEje1 = distanciaDesdeFrente + DistanciaEje1ATandem23;

            // Calcular reacciones usando el principio de momentos
            double reaccionTandem45 = pesoPallet * distanciaDesdeEje1 / DistanciaTotalEjes;
            double reaccionEje1 = pesoPallet * (DistanciaTotalEjes - distanciaDesdeEje1) / DistanciaTotalEjes;

            // El peso que no está en los ejes extremos está en el tandem central
            double reaccionTandem23 = pesoPallet - reaccionEje1 - reaccionTandem45;

            return (reaccionEje1, reaccionTandem23, reaccionTandem45);
        }
        // Método para previsualizar la distribución antes de cargar
        public static (double eje1, double tandem23, double tandem45) PrevisualizarDistribucion(
            DataTable dataPallets,
            int cantidadTarimasActual,
            int tarimasAdicionales)
        {
            return CalcularDistribucionPeso(dataPallets, cantidadTarimasActual + tarimasAdicionales);
        }
        #endregion

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
        List<PesoItem> listPesoItem = new List<PesoItem>();

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
    }
}