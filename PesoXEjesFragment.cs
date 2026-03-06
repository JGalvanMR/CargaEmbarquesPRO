using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using CargaEmbarques.Modal;

namespace CargaEmbarques
{
    [Activity(Label = "DETALLE DEL PESO DEL EMBARQUE")]
    public class PesoXEjesFragment : Android.App.Fragment
    {
        public readonly string connectionString = MainActivity.cadenaConexion;
        SqlConnection thisConnection = new SqlConnection(MainActivity.cadenaConexion);
        public string numerotrailer;
        public string fechatrailerx;
        public string recibo;
        public string prod_clave2;
        public string tarima2;
        public decimal pesoinicialeje1;
        public decimal pesoinicialeje2;
        public decimal pesoinicialeje3;
        public readonly List<PesoItem> listPesoItem = new List<PesoItem>();
        public DataTable PesoProd;
        public DataTable TbCar;

        // Constants
        public const string DateFormat = "dd/MM/yyyy";
        public const decimal PalletWeight = 20;
        public static readonly string[] IceWeightProducts = { "02002ML00", "02002BROFR", "02BRCO2025" };
        public const decimal IceWeightHigh = 8.5m;
        public const decimal IceWeightLow = 4m;

        public TextView placas;
        public TextView fecha;
        public TextView pedorigen;
        public TextView pesototalproducto;
        public TextView pesototaltrailer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.PesoXEjes, container, false);

            // Initialize data from arguments
            numerotrailer = Arguments?.GetString("no_trailer") ?? string.Empty;
            fechatrailerx = Arguments?.GetString("horatrailer") ?? string.Empty;
            recibo = Arguments?.GetString("recibo") ?? string.Empty;
            prod_clave2 = Arguments?.GetString("prod_clave") ?? string.Empty;
            tarima2 = Arguments?.GetString("tarima") ?? string.Empty;

            // Bind UI elements
            //placas = view.FindViewById<TextView>(Resource.Id.trailerpla);
            //fecha = view.FindViewById<TextView>(Resource.Id.fectrai);
            //pedorigen = view.FindViewById<TextView>(Resource.Id.pediori);
            pesototalproducto = view.FindViewById<TextView>(Resource.Id.pestotalcarga);
            pesototaltrailer = view.FindViewById<TextView>(Resource.Id.pestotaltrailer);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "DETALLE DEL PESO DEL EMBARQUE";

            try
            {
                CargarPesos(view);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Context, $"Error loading weights: {ex.Message}", ToastLength.Long).Show();
            }

            return view;
        }

        private void CargarPesos(View view)
        {
            try
            {
                CargarPesosInicialesTrailer();
                MostrarDatosBasicos();
                CargarPesosProductos();
                CargarDetalleEmbarque();
                CalcularDistribucionPesos(view);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Toast.MakeText(Context, "Error al cargar pesos: " + ex.Message, ToastLength.Long).Show();
            }
        }

        private void CargarPesosInicialesTrailer()
        {
            thisConnection.Open();
            try
            {
                string cadena = "SELECT pesoEje1, pesoEje2, pesoEje3 FROM tb_mstr_trailer WHERE no_trailer = @no_trailer AND hora_trailer = @hora_trailer";
                using (SqlCommand cmd = new SqlCommand(cadena, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@no_trailer", numerotrailer);
                    cmd.Parameters.AddWithValue("@hora_trailer", fechatrailerx);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pesoinicialeje1 = Convert.ToDecimal(reader["pesoEje1"].ToString().Trim());
                            pesoinicialeje2 = Convert.ToDecimal(reader["pesoEje2"].ToString().Trim());
                            pesoinicialeje3 = Convert.ToDecimal(reader["pesoEje3"].ToString().Trim());
                        }
                    }
                }
            }
            finally
            {
                thisConnection.Close();
            }
        }

        private void MostrarDatosBasicos()
        {
            //placas.Text = "TRAILER: " + numerotrailer;
            //fecha.Text = "FECHA: " + fechatrailerx;
            pedorigen.Text = " ";
        }

        private void CargarPesosProductos()
        {
            thisConnection.Open();
            try
            {
                string cadena = "SELECT a.prod_clave, a.prod_nombre, a.prod_presentacion, B.env_peso " +
                               "FROM tb_cat_producto A, tb_cat_envases B " +
                               "WHERE A.prod_presentacion = b.env_clave";

                using (SqlDataAdapter da = new SqlDataAdapter(cadena, thisConnection))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "PesoProd");
                    PesoProd = ds.Tables["PesoProd"];
                }
            }
            finally
            {
                thisConnection.Close();
            }
        }

        private void CargarDetalleEmbarque()
        {
            TbCar.Columns.Add("Prod_Clave");
            TbCar.Columns.Add("tipo");
            TbCar.Columns.Add("recibo");
            TbCar.Columns.Add("cajas");
            TbCar.Columns.Add("seccion");

            thisConnection.Open();
            try
            {
                string cadena = "SELECT A.prod_clave, A.cajas, A.tipo_rec, A.recibo, A.seccion " +
                               "FROM tb_det_embarque AS A " +
                               "INNER JOIN tb_mstr_embarque AS B ON A.emb_folio = B.emb_folio AND A.emb_tipo = B.emb_tipo " +
                               "WHERE A.Estatus != 'C' AND B.no_trailer = @no_trailer AND B.hora_trailer = @hora_trailer " +
                               "ORDER BY A.seccion";

                using (SqlCommand cmd = new SqlCommand(cadena, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@no_trailer", numerotrailer);
                    cmd.Parameters.AddWithValue("@hora_trailer", fechatrailerx);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DataRow row = TbCar.NewRow();
                            row["Prod_Clave"] = reader["prod_clave"].ToString().Trim();
                            row["tipo"] = reader["tipo_rec"].ToString().Trim();
                            row["recibo"] = reader["recibo"].ToString().Trim();
                            row["cajas"] = reader["cajas"].ToString().Trim();
                            row["seccion"] = reader["seccion"].ToString().Trim();
                            TbCar.Rows.Add(row);
                        }
                    }
                }
            }
            finally
            {
                thisConnection.Close();
            }
        }

        private void CalcularDistribucionPesos(View view)
        {
            decimal Pesoejes = 0;
            decimal pesototal = 0;
            int posiciones = 0;
            int posicionprimer14 = 0;
            int posicionanterior = 0;

            for (int i = 0; i < TbCar.Rows.Count; i++)
            {
                string producto = TbCar.Rows[i]["Prod_Clave"].ToString();
                string tipo = TbCar.Rows[i]["tipo"].ToString();
                string recibo = TbCar.Rows[i]["recibo"].ToString();
                string cajas = TbCar.Rows[i]["cajas"].ToString();
                string seccion = TbCar.Rows[i]["seccion"].ToString();

                if (posicionanterior != Convert.ToInt32(seccion))
                {
                    posicionanterior = Convert.ToInt32(seccion);
                    posiciones++;
                }

                decimal mKilos = CalcularPesoItem(producto, tipo, recibo, cajas);
                Pesoejes += mKilos;
                pesototal += mKilos;

                if (Convert.ToInt32(seccion) == 14)
                {
                    AgregarPesoEje1(Pesoejes, posiciones);
                    posicionprimer14 = posiciones;
                    Pesoejes = 0;
                }
            }

            AgregarPesoEje2y3(Pesoejes, posiciones, posicionprimer14);
            AgregarPesoEje4y5(Pesoejes, posiciones, posicionprimer14);
            MostrarPesosTotales(pesototal, posiciones);
            ActualizarGridView(view);
        }

        private decimal CalcularPesoItem(string producto, string tipo, string recibo, string cajas)
        {
            decimal mKilos = 0;

            if (tipo.Trim() == "PTC")
            {
                mKilos = CalcularPesoProductoTerminadoCampo(producto, recibo, cajas);
            }
            else
            {
                mKilos = CalcularPesoProductoTerminadoPlanta(producto, recibo, cajas);
            }

            return mKilos;
        }

        private decimal CalcularPesoProductoTerminadoCampo(string producto, string recibo, string cajas)
        {
            decimal mKilos = 0;

            thisConnection.Open();
            try
            {
                string cadena = "SELECT A.rptd_peso_bruto, A.rptd_tara, A.rptd_cantidad, B.ENV_PESO, C.RPT_FECHA " +
                               "FROM tb_det_recepcion_pt A, tb_cat_envases B, TB_MSTR_RECEPCION_PT C " +
                               "WHERE A.rptd_estatus != 'C' AND A.RPT_RECIBO = @recibo AND A.PROD_CLAVE = @producto " +
                               "AND A.ENV_CLAVE = B.ENV_CLAVE AND A.RPT_RECIBO = C.RPT_RECIBO";

                using (SqlCommand cmd = new SqlCommand(cadena, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@recibo", recibo);
                    cmd.Parameters.AddWithValue("@producto", producto);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal pesoBruto = Convert.ToDecimal(reader["rptd_peso_bruto"]);
                            decimal tara = Convert.ToDecimal(reader["rptd_tara"]);
                            decimal cantidad = Convert.ToDecimal(reader["rptd_cantidad"]);

                            mKilos += ((pesoBruto - tara) / cantidad) * Convert.ToDecimal(cajas);

                            DateTime mF = DateTime.ParseExact(fechatrailerx.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime fechaRecepcion = Convert.ToDateTime(reader["RPT_FECHA"]);

                            if (producto == "02002ML00" || producto == "02002BROFR" || producto == "02BRCO2025")
                            {
                                mKilos += Fn_PesoHielo(8.5m, fechaRecepcion, mF) * Convert.ToInt32(cajas);
                            }
                            else if (producto == "02002BRHEB")
                            {
                                mKilos += Fn_PesoHielo(4m, fechaRecepcion, mF);
                            }
                        }
                    }
                }
            }
            finally
            {
                thisConnection.Close();
            }

            return mKilos;
        }

        private decimal CalcularPesoProductoTerminadoPlanta(string producto, string recibo, string cajas)
        {
            decimal mKilos = 0;

            thisConnection.Open();
            try
            {
                string cadena = "SELECT B.PROD_CLAVE, C.PROD_NOMBRE, B.PROD_PESO_VAR, B.FODP_UNIDADES, " +
                               "C.PROD_PRESENTACION, D.ENV_PESO, E.HRP_PESO_NETO, E.HRP_NUM_UNIDADES, E.hrp_fecha " +
                               "FROM TB_DET_FINAL_ODP B, TB_CAT_PRODUCTO C, tb_cat_envases D, TB_HIST_RECEPCION E " +
                               "WHERE B.PROD_CLAVE = @producto AND B.ORDP_FOLIO = @recibo " +
                               "AND B.PROD_CLAVE = C.PROD_CLAVE AND C.PROD_PRESENTACION = D.ENV_CLAVE " +
                               "AND B.ORDP_FOLIO = E.hrp_recibo AND B.PROD_CLAVE = E.PROD_CLAVE";

                using (SqlCommand cmd = new SqlCommand(cadena, thisConnection))
                {
                    cmd.Parameters.AddWithValue("@producto", producto);
                    cmd.Parameters.AddWithValue("@recibo", recibo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal pesoVar = Convert.ToDecimal(reader["PROD_PESO_VAR"]);
                            decimal pesoEnvase = Convert.ToDecimal(reader["ENV_PESO"]);
                            decimal pesoNeto = Convert.ToDecimal(reader["HRP_PESO_NETO"]);
                            decimal numUnidades = Convert.ToDecimal(reader["HRP_NUM_UNIDADES"]);

                            if (pesoVar > 0)
                            {
                                mKilos += (pesoVar + pesoEnvase) * Convert.ToInt32(cajas);
                            }
                            else
                            {
                                mKilos += ((pesoNeto / numUnidades) + pesoEnvase) * Convert.ToInt32(cajas);
                            }

                            DateTime xcs = DateTime.ParseExact(fechatrailerx.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime fechaRecepcion = Convert.ToDateTime(reader["hrp_fecha"]);

                            if (producto == "02002ML00" || producto == "02002BROFR" || producto == "02BRCO2025")
                            {
                                mKilos += Fn_PesoHielo(8.5m, fechaRecepcion, xcs) * Convert.ToInt32(cajas);
                            }
                            else
                            {
                                mKilos += Fn_PesoHielo(4m, fechaRecepcion, xcs);
                            }
                        }
                    }
                }
            }
            finally
            {
                thisConnection.Close();
            }

            return mKilos;
        }

        private void AgregarPesoEje1(decimal Pesoejes, int posiciones)
        {
            listPesoItem.Add(new PesoItem()
            {
                Nombre = "PESO EJE NUMERO 1:",
                Peso = $"{Math.Round(Pesoejes + (20 * posiciones), 2)} | {Math.Round(Pesoejes + (20 * posiciones) + pesoinicialeje2, 2)}",
                ImagenResourceId = Resource.Drawable.pesoEje1
            });
        }

        private void AgregarPesoEje2y3(decimal Pesoejes, int posiciones, int posicionprimer14)
        {
            listPesoItem.Add(new PesoItem()
            {
                Nombre = "PESO EJE NUMERO 2:",
                Peso = $"{Math.Round(Pesoejes + (20 * (posiciones - posicionprimer14)), 2)} | {Math.Round(Pesoejes + (20 * (posiciones - posicionprimer14)) + pesoinicialeje3, 2)}",
                ImagenResourceId = Resource.Drawable.PesoEje23
            });
        }

        private void AgregarPesoEje4y5(decimal Pesoejes, int posiciones, int posicionprimer14)
        {
            listPesoItem.Add(new PesoItem()
            {
                Nombre = "PESO EJE NUMERO 3:",
                Peso = $"{Math.Round(Pesoejes + (20 * (posiciones - posicionprimer14)), 2)} | {Math.Round(Pesoejes + (20 * (posiciones - posicionprimer14)) + pesoinicialeje3, 2)}",
                ImagenResourceId = Resource.Drawable.PesoEje45
            });
        }

        private void MostrarPesosTotales(decimal pesototal, int posiciones)
        {
            pesototalproducto.Text = $"PESO TOTAL: {Math.Round(pesototal + (20 * posiciones), 2)}";
            pesototaltrailer.Text = $"PESO TOTAL TRAILER: {Math.Round(pesototal + (20 * posiciones) + pesoinicialeje2 + pesoinicialeje3, 2)}";
        }

        private void ActualizarGridView(View view)
        {
            var gvObject = view.FindViewById<GridView>(Resource.Id.infopesos);
            gvObject.Adapter = new PesoAdapter((Activity)Context, listPesoItem);
        }




        public void CargarPesos2(View view)
        {
            LoadInitialWeights();
            pedorigen.Text = " ";
            LoadProductWeights();
            LoadShipmentDetails();
            CalculateShipmentWeight();
            DisplayWeights(view);
        }

        public void LoadInitialWeights()
        {
            thisConnection.Open();
            string query = $"SELECT pesoEje1, pesoEje2, pesoEje3 FROM tb_mstr_trailer WHERE no_trailer = '{numerotrailer}' AND hora_trailer = '{fechatrailerx}'";
            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                using (SqlDataReader info = cmd.ExecuteReader())
                {
                    while (info.Read())
                    {
                        pesoinicialeje1 = Convert.ToDecimal(info["pesoEje1"].ToString().Trim());
                        pesoinicialeje2 = Convert.ToDecimal(info["pesoEje2"].ToString().Trim());
                        pesoinicialeje3 = Convert.ToDecimal(info["pesoEje3"].ToString().Trim());
                    }
                }
            }
            thisConnection.Close();
        }

        public void LoadProductWeights()
        {
            thisConnection.Open();
            string query = "SELECT a.prod_clave, a.prod_nombre, a.prod_presentacion, B.env_peso FROM tb_cat_producto A, tb_cat_envases B WHERE A.prod_presentacion = b.env_clave";
            using (SqlDataAdapter da = new SqlDataAdapter(query, thisConnection))
            {
                DataSet ds = new DataSet();
                da.Fill(ds, "PesoProd");
                PesoProd = ds.Tables["PesoProd"];
            }
            thisConnection.Close();
        }

        public void LoadShipmentDetails()
        {
            TbCar.Columns.Add("Prod_Clave");
            TbCar.Columns.Add("tipo");
            TbCar.Columns.Add("recibo");
            TbCar.Columns.Add("cajas");
            TbCar.Columns.Add("seccion");

            thisConnection.Open();
            string query = $"SELECT A.prod_clave, A.cajas, A.tipo_rec, A.recibo, A.seccion FROM tb_det_embarque AS A INNER JOIN tb_mstr_embarque AS B ON A.emb_folio = B.emb_folio AND A.emb_tipo = B.emb_tipo WHERE A.Estatus != 'C' AND B.no_trailer = '{numerotrailer}' AND B.hora_trailer = '{fechatrailerx}' ORDER BY A.seccion";
            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                using (SqlDataReader info = cmd.ExecuteReader())
                {
                    while (info.Read())
                    {
                        DataRow row = TbCar.NewRow();
                        row["Prod_Clave"] = info["prod_clave"].ToString().Trim();
                        row["tipo"] = info["tipo_rec"].ToString().Trim();
                        row["recibo"] = info["recibo"].ToString().Trim();
                        row["cajas"] = info["cajas"].ToString().Trim();
                        row["seccion"] = info["seccion"].ToString().Trim();
                        TbCar.Rows.Add(row);
                    }
                }
            }
            thisConnection.Close();
        }

        public decimal CalculateFieldProductWeight(string producto, string recibo, string cajas)
        {
            decimal mKilos = 0;
            thisConnection.Open();
            string query = $"SELECT A.rptd_peso_bruto, A.rptd_tara, A.rptd_cantidad, B.ENV_PESO, C.RPT_FECHA FROM tb_det_recepcion_pt A, tb_cat_envases B, TB_MSTR_RECEPCION_PT C WHERE A.rptd_estatus != 'C' AND A.RPT_RECIBO = '{recibo}' AND A.PROD_CLAVE = '{producto}' AND A.ENV_CLAVE = B.ENV_CLAVE AND A.RPT_RECIBO = C.RPT_RECIBO";
            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                using (SqlDataReader info = cmd.ExecuteReader())
                {
                    while (info.Read())
                    {
                        mKilos += ((Convert.ToDecimal(info["rptd_peso_bruto"].ToString().Trim()) -
                            Convert.ToDecimal(info["rptd_tara"].ToString().Trim())) /
                            Convert.ToDecimal(info["rptd_cantidad"].ToString().Trim())) * Convert.ToDecimal(cajas);

                        DateTime mF = DateTime.ParseExact(fechatrailerx.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (producto.Trim() == "02002ML00" || producto.Trim() == "02002BROFR" || producto.Trim() == "02BRCO2025")
                        {
                            mKilos += Fn_PesoHielo(Convert.ToDecimal(8.5), Convert.ToDateTime(info["RPT_FECHA"].ToString().Trim()), mF) * Convert.ToInt32(cajas);
                        }
                        else if (producto.Trim() == "02002BRHEB")
                        {
                            mKilos += Fn_PesoHielo(Convert.ToDecimal(4), Convert.ToDateTime(info["RPT_FECHA"].ToString().Trim()), mF);
                        }
                    }
                }
            }
            thisConnection.Close();
            return mKilos;
        }

        public decimal CalculatePlantProductWeight(string producto, string recibo, string cajas)
        {
            decimal mKilos = 0;
            thisConnection.Open();
            string query = $"SELECT B.PROD_CLAVE, C.PROD_NOMBRE, B.PROD_PESO_VAR, B.FODP_UNIDADES, C.PROD_PRESENTACION, D.ENV_PESO, E.HRP_PESO_NETO, E.HRP_NUM_UNIDADES, E.hrp_fecha FROM TB_DET_FINAL_ODP B, TB_CAT_PRODUCTO C, tb_cat_envases D, TB_HIST_RECEPCION E WHERE B.PROD_CLAVE = '{producto}' AND B.ORDP_FOLIO = '{recibo}' AND B.PROD_CLAVE = C.PROD_CLAVE AND C.PROD_PRESENTACION = D.ENV_CLAVE AND B.ORDP_FOLIO = E.hrp_recibo AND B.PROD_CLAVE = E.PROD_CLAVE";
            using (SqlCommand cmd = new SqlCommand(query, thisConnection))
            {
                using (SqlDataReader info = cmd.ExecuteReader())
                {
                    while (info.Read())
                    {
                        decimal Mpe = 0;
                        if (Convert.ToDecimal(info["PROD_PESO_VAR"].ToString().Trim()) > 0)
                        {
                            Mpe = Convert.ToDecimal(info["PROD_PESO_VAR"]) * Convert.ToInt32(cajas) +
                                (Convert.ToDecimal(info["ENV_PESO"]) * Convert.ToInt32(cajas));
                        }
                        else
                        {
                            Mpe = Convert.ToDecimal(info["ENV_PESO"]) * Convert.ToInt32(cajas) +
                                (Convert.ToDecimal(info["HRP_PESO_NETO"]) / Convert.ToDecimal(info["HRP_NUM_UNIDADES"]) * Convert.ToInt32(cajas));
                        }

                        mKilos += Mpe;
                        DateTime xcs = DateTime.ParseExact(fechatrailerx.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (producto == "02002ML00" || producto == "02002BROFR" || producto == "02BRCO2025")
                        {
                            mKilos += Fn_PesoHielo(Convert.ToDecimal(8.5), Convert.ToDateTime(info["hrp_fecha"]), xcs) * Convert.ToInt32(cajas);
                        }
                        else
                        {
                            mKilos += Fn_PesoHielo(Convert.ToDecimal(4), Convert.ToDateTime(info["hrp_fecha"]), xcs);
                        }
                    }
                }
            }
            thisConnection.Close();
            return mKilos;
        }

        public void CalculateShipmentWeight()
        {
            decimal pesoejes = 0;
            decimal pesototal = 0;
            int posiciones = 0;
            int posicionprimer14 = 0;
            int posicionanterior = 0;

            for (int i = 0; i < TbCar.Rows.Count; i++)
            {
                string producto = TbCar.Rows[i]["Prod_Clave"].ToString();
                string tipo = TbCar.Rows[i]["tipo"].ToString();
                string recibo = TbCar.Rows[i]["recibo"].ToString();
                string cajas = TbCar.Rows[i]["cajas"].ToString();
                string seccion = TbCar.Rows[i]["seccion"].ToString();

                if (posicionanterior != Convert.ToInt32(seccion))
                {
                    posicionanterior = Convert.ToInt32(seccion);
                    posiciones++;
                }

                decimal mKilos = tipo.Trim() == "PTC"
                    ? CalculateFieldProductWeight(producto, recibo, cajas)
                    : CalculatePlantProductWeight(producto, recibo, cajas);

                pesoejes += mKilos;
                pesototal += mKilos;

                if (Convert.ToInt32(seccion) == 14)
                {
                    listPesoItem.Add(new PesoItem()
                    {
                        Nombre = "PESO EJE NUMERO 1:",
                        Peso = $"{Math.Round(pesoejes + (20 * posiciones), 2)} | {Math.Round(pesoejes + (20 * posiciones) + pesoinicialeje2, 2)}",
                        ImagenResourceId = Resource.Drawable.pesoEje1
                    });
                    posicionprimer14 = posiciones;
                    pesoejes = 0;
                }
            }

            listPesoItem.Add(new PesoItem()
            {
                Nombre = "PESO EJE NUMERO 2:",
                Peso = $"{Math.Round(pesoejes + (20 * (posiciones - posicionprimer14)), 2)} | {Math.Round(pesoejes + (20 * (posiciones - posicionprimer14)) + pesoinicialeje3, 2)}",
                ImagenResourceId = Resource.Drawable.PesoEje23
            });

            listPesoItem.Add(new PesoItem()
            {
                Nombre = "PESO EJE NUMERO 3:",
                Peso = $"{Math.Round(pesoejes + (20 * (posiciones - posicionprimer14)), 2)} | {Math.Round(pesoejes + (20 * (posiciones - posicionprimer14)) + pesoinicialeje3, 2)}",
                ImagenResourceId = Resource.Drawable.PesoEje45
            });

            pesototalproducto.Text = $"PESO TOTAL: {Math.Round(pesototal + (20 * posiciones), 2)}";
            pesototaltrailer.Text = $"PESO TOTAL TRAILER: {Math.Round(pesototal + (20 * posiciones) + pesoinicialeje2 + pesoinicialeje3, 2)}";
        }

        public void DisplayWeights(View view)
        {
            var gvObject = view.FindViewById<GridView>(Resource.Id.infopesos);
            gvObject.Adapter = new PesoAdapter((Activity)Context, listPesoItem);
        }

        public void GetPesosInicialesTrailer(SqlConnection connection)
        {
            const string query = "SELECT pesoEje1, pesoEje2, pesoEje3 FROM tb_mstr_trailer WHERE no_trailer = @Trailer AND hora_trailer = @Hora";
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Trailer", numerotrailer);
                cmd.Parameters.AddWithValue("@Hora", fechatrailerx);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pesoinicialeje1 = reader.GetDecimalOrDefault("pesoEje1");
                        pesoinicialeje2 = reader.GetDecimalOrDefault("pesoEje2");
                        pesoinicialeje3 = reader.GetDecimalOrDefault("pesoEje3");
                    }
                }
            }
        }

        public decimal CalcularPesoPTC(SqlConnection connection, string producto, string recibo, string cajas, DateTime fechaEmbarque)
        {
            decimal mKilos = 0;
            const string query = "SELECT a.rptd_peso_bruto, a.rptd_tara, a.rptd_cantidad, b.env_peso, c.rpt_fecha " +
                               "FROM tb_det_recepcion_pt a " +
                               "JOIN tb_cat_envases b ON a.env_clave = b.env_clave " +
                               "JOIN tb_mstr_recepcion_pt c ON a.rpt_recibo = c.rpt_recibo " +
                               "WHERE a.rptd_estatus != 'C' AND a.rpt_recibo = @Recibo AND a.prod_clave = @Producto";
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Recibo", recibo);
                cmd.Parameters.AddWithValue("@Producto", producto);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mKilos += ((reader.GetDecimalOrDefault("rptd_peso_bruto") - reader.GetDecimalOrDefault("rptd_tara")) /
                                   reader.GetDecimalOrDefault("rptd_cantidad")) * Convert.ToDecimal(cajas);

                        if (DateTime.TryParse(reader.GetStringOrDefault("rpt_fecha"), out var fechaRecepcion))
                        {
                            decimal iceWeight = IceWeightProducts.Contains(producto) ? IceWeightHigh : IceWeightLow;
                            mKilos += Fn_PesoHielo(iceWeight, fechaRecepcion, fechaEmbarque) * Convert.ToInt32(cajas);
                        }
                    }
                }
            }
            return mKilos;
        }

        public decimal CalcularPesoPTP(SqlConnection connection, string producto, string recibo, string cajas, DateTime fechaEmbarque)
        {
            decimal mKilos = 0;
            const string query = "SELECT b.prod_clave, b.prod_peso_var, b.fodp_unidades, c.prod_presentacion, d.env_peso, e.hrp_peso_neto, e.hrp_num_unidades, e.hrp_fecha " +
                               "FROM tb_det_final_odp b " +
                               "JOIN tb_cat_producto c ON b.prod_clave = c.prod_clave " +
                               "JOIN tb_cat_envases d ON c.prod_presentacion = d.env_clave " +
                               "JOIN tb_hist_recepcion e ON b.ordp_folio = e.hrp_recibo AND b.prod_clave = e.prod_clave " +
                               "WHERE b.prod_clave = @Producto AND b.ordp_folio = @Recibo";
            using (var cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Producto", producto);
                cmd.Parameters.AddWithValue("@Recibo", recibo);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal mpe = reader.GetDecimalOrDefault("prod_peso_var") > 0
                            ? reader.GetDecimalOrDefault("prod_peso_var") * Convert.ToInt32(cajas) +
                              reader.GetDecimalOrDefault("env_peso") * Convert.ToInt32(cajas)
                            : reader.GetDecimalOrDefault("env_peso") * Convert.ToInt32(cajas) +
                              (reader.GetDecimalOrDefault("hrp_peso_neto") / reader.GetDecimalOrDefault("hrp_num_unidades") * Convert.ToInt32(cajas));

                        mKilos += mpe;

                        if (DateTime.TryParse(reader.GetStringOrDefault("hrp_fecha"), out var fechaRecepcion))
                        {
                            decimal iceWeight = IceWeightProducts.Contains(producto) ? IceWeightHigh : IceWeightLow;
                            mKilos += Fn_PesoHielo(iceWeight, fechaRecepcion, fechaEmbarque) * Convert.ToInt32(cajas);
                        }
                    }
                }
            }
            return mKilos;
        }

        public decimal Fn_PesoHielo(decimal peso, DateTime fecRec, DateTime fecEmb)
        {
            int daysDiff = fecEmb.Subtract(fecRec).Days;
            return daysDiff switch
            {
                0 => peso,
                1 => peso * 0.85m,
                2 => peso * 0.75m,
                3 => peso * 0.50m,
                4 => peso * 0.35m,
                5 => peso * 0.20m,
                6 => peso * 0.10m,
                _ => 0
            };
        }

        public void MostrarDatosGenerales()
        {
            placas.Text = $"TRAILER: {numerotrailer}";
            fecha.Text = $"FECHA: {fechatrailerx}";
            pedorigen.Text = " ";
        }
    }

    // Extension method for safe SQL data retrieval
    public static class SqlDataReaderExtensions
    {
        public static string GetStringOrDefault(this SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? string.Empty : reader.GetString(reader.GetOrdinal(columnName));
        }

        public static decimal GetDecimalOrDefault(this SqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? 0 : reader.GetDecimal(reader.GetOrdinal(columnName));
        }
    }
}