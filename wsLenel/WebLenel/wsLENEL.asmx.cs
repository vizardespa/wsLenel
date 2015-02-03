using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebLenel
{
    /// <summary>
    /// Descripción breve de wsLENEL
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class wsLENEL : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hola a todos";
        }

        [WebMethod]
        public void ENROLL_GATENET(int NumeroVisita, int PCereso)
        {
            string PResultado = "S";//P_RESULTADO := 'S';
            string SQLString;
            string SQLString2;
            string Nombre = "";
            string ApellidoPaterno = "";
            string ApellidoMaterno = "";
            string IdUsuario = "06" + NumeroVisita; //V_ID_USUARIO :='06'||TO_CHAR(P_NUM_VISITA);
            string NIP = "";
            string Cereso = "00006"; //V_CERESO:='00006';
            int Perfil;
            int TipoUsuario;
            string Departamento = "";
            int Contador = 0;
            int TipoBiometrico;
            int Deploy;


            OdbcConnection objCon = new OdbcConnection("DSN=Lenel;uid=LENEL;pwd=MULTIMEDIA");
            objCon.Open();

            //CURSOR index1 IS SELECT BIOMETRIC_TYPE, MEDIA_TYPE, DBMS_LOB.GETLENGTH(BIOMETRIC_DATA) AS INFO_SIZE,NSEQ_BIOMETRIC FROM BIOQCONT2.BIOMETRIC_INFO WHERE ID_USER = V_ID_USUARIO;
            string PsQuery = "SELECT BIOMETRIC_TYPE, MEDIA_TYPE, DBMS_LOB.GETLENGTH(BIOMETRIC_DATA) AS INFO_SIZE,NSEQ_BIOMETRIC FROM BIOQCONT2.BIOMETRIC_INFO WHERE ID_USER =" + IdUsuario + ";";
            OdbcCommand objCmd = new OdbcCommand(PsQuery, objCon);
            OdbcDataReader reader = objCmd.ExecuteReader();

            List<BiometricInfo> index1 = new List<BiometricInfo>();
            while (reader.Read())
            {
                index1.Add(new BiometricInfo(reader.GetString(0), reader.GetString(1), reader.GetString(2).Length, reader.GetString(3)));
            }

            //SELECT COUNT(1) INTO V_CONT FROM GATENET.USUARIO WHERE ID_USUARIO = V_ID_USUARIO;
            PsQuery = "SELECT COUNT(1) FROM GATENET.USUARIO WHERE ID_USUARIO =" + IdUsuario + ";";
            objCmd = new OdbcCommand(PsQuery, objCon);
            reader = objCmd.ExecuteReader();
            if (reader.Read())
            {
                Contador = int.Parse(reader.GetString(0));
            }

            //IF  V_CONT = 0 THEN BEGIN
            if (Contador == 0)
            {
                Perfil = 1; //V_PERFIL := 1;    --PERFIL 1 = PERFIL USUARIO NORMAL
                TipoUsuario = 0; //V_TIPO_USUARIO := 0; --TIPO_USUARIO 0 = ACTIVO

                // SELECT COUNT(1) INTO V_CONT FROM BTS.VISITA@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                PsQuery = "SELECT COUNT(1) FROM BTS.VISITA@DBL_JUSTICIA WHERE NUM_VISITA =" + NumeroVisita + ";";
                objCmd = new OdbcCommand(PsQuery, objCon);
                reader = objCmd.ExecuteReader();
                if (reader.Read())
                {
                    Contador = int.Parse(reader.GetString(0));
                }
                // IF V_CONT = 1 THEN
                if (Contador == 0)
                {
                    //SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO FROM BTS.VISITA@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                    PsQuery = "SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE FROM BTS.VISITA@DBL_JUSTICIA WHERE NUM_VISITA =" + NumeroVisita + ";";
                    objCmd = new OdbcCommand(PsQuery, objCon);
                    reader = objCmd.ExecuteReader();
                    //INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO
                    if (reader.Read())
                    {
                        Nombre = reader.GetString(0);
                        ApellidoPaterno = reader.GetString(1);
                        ApellidoMaterno = reader.GetString(2);
                        NIP = reader.GetString(3);
                        Departamento = reader.GetString(4);
                    }
                }
                else
                {
                    //SELECT COUNT(1) INTO V_CONT FROM BTS.PADRON_REGISTRO@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                    PsQuery = "SELECT COUNT(1) FROM BTS.PADRON_REGISTRO@DBL_JUSTICIA WHERE NUM_VISITA =" + NumeroVisita + ";";
                    objCmd = new OdbcCommand(PsQuery, objCon);
                    reader = objCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Contador = int.Parse(reader.GetString(0));
                    }
                    //IF V_CONT = 1 THEN  
                    if (Contador == 1)
                    {
                        //SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO FROM BTS.PADRON_REGISTRO@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                        PsQuery = "SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE FROM BTS.PADRON_REGISTRO@DBL_JUSTICIA WHERE NUM_VISITA =" + NumeroVisita + ";";
                        objCmd = new OdbcCommand(PsQuery, objCon);
                        reader = objCmd.ExecuteReader();
                        //INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO
                        if (reader.Read())
                        {
                            Nombre = reader.GetString(0);
                            ApellidoPaterno = reader.GetString(1);
                            ApellidoMaterno = reader.GetString(2);
                            NIP = reader.GetString(3);
                            Departamento = reader.GetString(4);
                        }
                    }
                    else
                    {
                        //SELECT COUNT(1) INTO V_CONT FROM BTS.ABOGADOS@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                        PsQuery = "SELECT COUNT(1) FROM BTS.ABOGADOS@DBL_JUSTICIA WHERE NUM_VISITA =" + NumeroVisita + ";";
                        objCmd = new OdbcCommand(PsQuery, objCon);
                        reader = objCmd.ExecuteReader();
                        if (reader.Read())
                        {
                            Contador = int.Parse(reader.GetString(0));
                        }
                        //IF V_CONT = 1 THEN
                        if (Contador == 1)
                        {
                            //SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO FROM BTS.ABOGADOS@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                            PsQuery = "SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE FROM BTS.ABOGADOS@DBL_JUSTICIA WHERE NUM_VISITA =" + NumeroVisita + ";";
                            objCmd = new OdbcCommand(PsQuery, objCon);
                            reader = objCmd.ExecuteReader();
                            //INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO
                            if (reader.Read())
                            {
                                Nombre = reader.GetString(0);
                                ApellidoPaterno = reader.GetString(1);
                                ApellidoMaterno = reader.GetString(2);
                                NIP = reader.GetString(3);
                                Departamento = reader.GetString(4);
                            }
                        }
                    }
                }
                /*
                 IF V_DEPTO IS NULL THEN
                 V_DEPTO:=15;
                 END IF;
                */
                if (Departamento == "")
                {
                    Departamento = "15";
                }

                try
                {
                    /*
                     SQLString  := 'INSERT INTO GATENET.USUARIO(ID_USUARIO,ID_PERFIL,TIPO_USUARIO,NOMBRE,APELLIDO_PATERNO,APELLIDO_MATERNO,NIP,FECHA_ACTIVACION,ID_DEPTO)
                     VALUES ('||CHR(39)||V_ID_USUARIO||CHR(39)||','||V_PERFIL||','||V_TIPO_USUARIO||','||CHR(39)||V_NOMBRE||CHR(39)||','||CHR(39)||V_PATERNO||CHR(39)||','
                        ||CHR(39)||V_MATERNO||CHR(39)||','||CHR(39)||V_NIP||CHR(39)||',SYSDATE,'||V_DEPTO||')';
                     */
                    SQLString = "INSERT INTO GATENET.USUARIO(ID_USUARIO,ID_PERFIL,TIPO_USUARIO,NOMBRE,APELLIDO_PATERNO,APELLIDO_MATERNO,NIP,FECHA_ACTIVACION,ID_DEPTO) VALUES ("
                        + "'" + IdUsuario + "'," + Perfil + "," + IdUsuario + "," + TipoUsuario + "," + "'" + Nombre + "','" + ApellidoPaterno + "','" + ApellidoMaterno + "','" + NIP + "'," + "SYSDATE," + Departamento + ")";
                    objCmd = new OdbcCommand(SQLString, objCon);
                    objCmd.ExecuteNonQuery();
                    foreach (BiometricInfo bi in index1)
                    {
                        TipoBiometrico = int.Parse(bi.BiometricType) - 1;//V_TIPO_BIOMETRICO := REG.BIOMETRIC_TYPE -1;
                        Deploy = 1;// V_DEPLOY := 1;
                        //IF REG.MEDIA_TYPE = 0 THEN
                        if (bi.MediaTye == "0")
                        {
                            TipoBiometrico += 40;// V_TIPO_BIOMETRICO := V_TIPO_BIOMETRICO + 40;
                            Deploy = 0;//V_DEPLOY := 0;
                        }
                        /*
                        SQLString:='INSERT INTO GATENET.INFO_BIOMETRICO_DG(TIPO_BIOMETRICO,ID_USUARIO,INFO_SIZE,FECHA_REGISTRO) 
                            VALUES('||V_TIPO_BIOMETRICO||','||CHR(39)||V_ID_USUARIO||CHR(39)||','||REG.INFO_SIZE||',SYSDATE)';
                         */
                        SQLString = "INSERT INTO GATENET.INFO_BIOMETRICO_DG(TIPO_BIOMETRICO,ID_USUARIO,INFO_SIZE,FECHA_REGISTRO) VALUES(" +
                            +TipoBiometrico + ",'" + IdUsuario + "'," + bi.InfoSize + ",SYSDATE)';";
                        objCmd = new OdbcCommand(SQLString, objCon);
                        objCmd.ExecuteNonQuery();

                        /*
                        SQLString:='INSERT INTO GATENET.INFO_BIOMETRICO_DT(TIPO_BIOMETRICO,ID_USUARIO,DEPLOY,INFO)
                        VALUES('||V_TIPO_BIOMETRICO||','||CHR(39)||V_ID_USUARIO||CHR(39)||','||V_DEPLOY||', (SELECT BIOMETRIC_DATA FROM BIOQCONT2.BIOMETRIC_INFO WHERE NSEQ_BIOMETRIC = '
                        ||REG.NSEQ_BIOMETRIC||'))';
                         */
                        SQLString = "INSERT INTO GATENET.INFO_BIOMETRICO_DT(TIPO_BIOMETRICO,ID_USUARIO,DEPLOY,INFO) VALUES("
                            + TipoBiometrico + ",'" + IdUsuario + "'," + Deploy + ",(SELECT BIOMETRIC_DATA FROM BIOQCONT2.BIOMETRIC_INFO WHERE NSEQ_BIOMETRIC = "
                        + bi.NSeqBiometric + "));";
                        objCmd = new OdbcCommand(SQLString, objCon);
                        objCmd.ExecuteNonQuery();

                        SQLString = "COMMIT;";
                        objCmd = new OdbcCommand(SQLString, objCon);
                        objCmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    /*
                    P_RESULTADO := 'N';
                    ROLLBACK;
                     */
                    PResultado = "N";
                    SQLString = "ROLLBACK;";
                    objCmd = new OdbcCommand(SQLString, objCon);
                    objCmd.ExecuteNonQuery();
                }
            }
        }

    }

    public class BiometricInfo
    {
        public string BiometricType { get; set; }
        public string MediaTye { get; set; }
        public int InfoSize { get; set; }
        public string NSeqBiometric { get; set; }

        public BiometricInfo()
        {

        }
        public BiometricInfo(string biometricType, string mediaType, int infoSize, string nSeqBiometric)
        {
            BiometricType = biometricType;
            MediaTye = mediaType;
            InfoSize = infoSize;
            NSeqBiometric = nSeqBiometric;
        }
    }
}
