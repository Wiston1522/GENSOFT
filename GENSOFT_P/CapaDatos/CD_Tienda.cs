﻿using CapaModelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Tienda
    {
        public static CD_Tienda _instancia = null;

        private CD_Tienda()
        {

        }

        public static CD_Tienda Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new CD_Tienda();
                }
                return _instancia;
            }
        }

        public List<Tienda> ObtenerTiendas()
        {
            List<Tienda> rptListaUsuario = new List<Tienda>();
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("usp_ObtenerTienda", oConexion);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    oConexion.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        rptListaUsuario.Add(new Tienda()
                        {
                            IdTienda = Convert.ToInt32(dr["IdTienda"].ToString()),
                            Nombre = dr["Nombre"].ToString(),
                            RUC = dr["RUC"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"].ToString())

                        });
                    }
                    dr.Close();

                    return rptListaUsuario;

                }
                catch (Exception ex)
                {
                    rptListaUsuario = null;
                    return rptListaUsuario;
                }
            }
        }

        public bool RegistrarTienda(Tienda oTienda)
        {
            bool respuesta = true;

            // Validaciones
            if (string.IsNullOrWhiteSpace(oTienda.Nombre) || oTienda.Nombre.Length > 100 || !EsTextoValido(oTienda.Nombre))
            {
                respuesta = false;
                return respuesta; // Nombre no válido
            }


            if (string.IsNullOrWhiteSpace(oTienda.RUC) || oTienda.RUC.Length != 11)
            {
                respuesta = false;
                return respuesta; // RUC no válido
            }

            if (string.IsNullOrWhiteSpace(oTienda.Direccion) || oTienda.Direccion.Length > 200)
            {
                respuesta = false;
                return respuesta; // Dirección no válida
            }

            if (string.IsNullOrWhiteSpace(oTienda.Telefono) || oTienda.Telefono.Length != 10 || !EsNumeroValido(oTienda.Telefono))
            {
                respuesta = false;
                return respuesta; // Teléfono no válido
            }

            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_RegistrarTienda", oConexion);
                    cmd.Parameters.AddWithValue("Nombre", oTienda.Nombre);
                    cmd.Parameters.AddWithValue("RUT", oTienda.RUC);
                    cmd.Parameters.AddWithValue("Direccion", oTienda.Direccion);
                    cmd.Parameters.AddWithValue("Telefono", oTienda.Telefono);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception)
                {
                    respuesta = false;
                }
            }

            return respuesta;
        }

        private bool EsTextoValido(string texto)
        {
            return !texto.Any(char.IsDigit); // Verifica que el texto no contenga dígitos
        }

        private bool EsNumeroValido(string numero)
        {
            return long.TryParse(numero, out _) && numero.Length == 10; // Verifica que el número sea válido y tenga 10 dígitos
        }


        public bool ModificarTienda(Tienda oTienda)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_ModificarTienda", oConexion);
                    cmd.Parameters.AddWithValue("IdTienda", oTienda.IdTienda);
                    cmd.Parameters.AddWithValue("Nombre", oTienda.Nombre);
                    cmd.Parameters.AddWithValue("RUT", oTienda.RUC);
                    cmd.Parameters.AddWithValue("Direccion", oTienda.Direccion);
                    cmd.Parameters.AddWithValue("Telefono", oTienda.Telefono);
                    cmd.Parameters.AddWithValue("Activo", oTienda.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception ex)
                {
                    respuesta = false;
                }

            }

            return respuesta;

        }

        public bool EliminarTienda(int IdTienda)
        {
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(Conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_EliminarTienda", oConexion);
                    cmd.Parameters.AddWithValue("IdTienda", IdTienda);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);

                }
                catch (Exception ex)
                {
                    respuesta = false;
                }

            }

            return respuesta;

        }
    }
}
