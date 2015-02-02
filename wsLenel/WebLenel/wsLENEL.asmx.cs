using System;
using System.Collections.Generic;
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
        public void ENROLL_GATENET(string Nombre ,string ApellidoPaterno,string ApellidoMaterno ,string IdUsuario ,string NIP ,string Cereso,int Perfil,
            int TipoUsuario,string Departamento ,int Contador,int TipoBiometrico ,int Deploy)
        {
            /*
             CREATE OR REPLACE
PROCEDURE "SP_ENROLL_GATENET" (P_NUM_VISITA IN NUMBER,P_CERESO IN NUMBER)
AS  P_RESULTADO CHAR(1);
SQLString  varchar2(4096);
SQLString2  varchar2(4096);
V_NOMBRE VARCHAR2(100);
V_PATERNO VARCHAR2(50);
V_MATERNO VARCHAR2(50);
V_ID_USUARIO VARCHAR2(10);
V_NIP VARCHAR2(10);
V_CERESO VARCHAR2(5);
V_PERFIL NUMBER;
V_TIPO_USUARIO NUMBER;
V_DEPTO VARCHAR2(25);
V_CONT NUMBER;
V_TIPO_BIOMETRICO NUMBER;
V_DEPLOY NUMBER;
CURSOR index1 IS   SELECT BIOMETRIC_TYPE, MEDIA_TYPE, DBMS_LOB.GETLENGTH(BIOMETRIC_DATA) AS INFO_SIZE,NSEQ_BIOMETRIC FROM BIOQCONT2.BIOMETRIC_INFO WHERE ID_USER = V_ID_USUARIO;
BEGIN
    --SELECT CERESO INTO V_CERESO FROM BTS.PARAM@DBL_JUSTICIA;
    --V_ID_USUARIO := LPAD(TO_CHAR(V_CERESO),2,'0')||TO_CHAR(P_NUM_VISITA);
    V_CERESO:='00006';
    V_ID_USUARIO := '06'||TO_CHAR(P_NUM_VISITA);
    P_RESULTADO := 'S';
    SELECT COUNT(1) INTO V_CONT FROM GATENET.USUARIO WHERE ID_USUARIO = V_ID_USUARIO;
    DBMS_OUTPUT.PUT_LINE(TO_CHAR(V_CONT));
    IF  V_CONT = 0 THEN
    BEGIN
        V_PERFIL := 1;    --PERFIL 1 = PERFIL USUARIO NORMAL
        V_TIPO_USUARIO := 0; --TIPO_USUARIO 0 = ACTIVO
        SELECT COUNT(1) INTO V_CONT FROM BTS.VISITA@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
        IF V_CONT = 1 THEN
            SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO FROM BTS.VISITA@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
        ELSE
            SELECT COUNT(1) INTO V_CONT FROM BTS.PADRON_REGISTRO@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
            IF V_CONT = 1 THEN
                SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO FROM BTS.PADRON_REGISTRO@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
            ELSE
                SELECT COUNT(1) INTO V_CONT FROM BTS.ABOGADOS@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                IF V_CONT = 1 THEN
                    SELECT NOMBRE,PATERNO,MATERNO,NIP,TIPO_VISITANTE INTO V_NOMBRE,V_PATERNO,V_MATERNO,V_NIP,V_DEPTO FROM BTS.ABOGADOS@DBL_JUSTICIA WHERE NUM_VISITA = P_NUM_VISITA;
                END IF;
            END IF;
        END IF;
        IF V_DEPTO IS NULL THEN
            V_DEPTO:=15;
        END IF;
        DBMS_OUTPUT.PUT_LINE(SQLString);
        SQLString  := 'INSERT INTO GATENET.USUARIO(ID_USUARIO,ID_PERFIL,TIPO_USUARIO,NOMBRE,APELLIDO_PATERNO,APELLIDO_MATERNO,NIP,FECHA_ACTIVACION,ID_DEPTO)
        VALUES ('||CHR(39)||V_ID_USUARIO||CHR(39)||','||V_PERFIL||','||V_TIPO_USUARIO||','||CHR(39)||V_NOMBRE||CHR(39)||','||CHR(39)||V_PATERNO||CHR(39)||','
 ||CHR(39)||V_MATERNO||CHR(39)||','||CHR(39)||V_NIP||CHR(39)||',SYSDATE,'||V_DEPTO||')';
        DBMS_OUTPUT.PUT_LINE(SQLString);
        EXECUTE IMMEDIATE SQLString;
        --EXCEPTION WHEN OTHERS THEN
        IF SQLCode != -1 THEN
            DBMS_OUTPUT.PUT_LINE(TO_CHAR('INSERCIÓN CORRECTA EN TABLA GATENET.USUARIO'));
            DBMS_OUTPUT.PUT_LINE('FUERA');
            FOR REG IN index1 LOOP
                DBMS_OUTPUT.PUT_LINE('SI ENTRA');
                V_TIPO_BIOMETRICO := REG.BIOMETRIC_TYPE -1;
                V_DEPLOY := 1;
                IF REG.MEDIA_TYPE = 0 THEN
                    V_TIPO_BIOMETRICO := V_TIPO_BIOMETRICO + 40;
                    V_DEPLOY := 0;
                END IF;
                SQLString:='INSERT INTO GATENET.INFO_BIOMETRICO_DG(TIPO_BIOMETRICO,ID_USUARIO,INFO_SIZE,FECHA_REGISTRO) 
 VALUES('||V_TIPO_BIOMETRICO||','||CHR(39)||V_ID_USUARIO||CHR(39)||','||REG.INFO_SIZE||',SYSDATE)';
                DBMS_OUTPUT.PUT_LINE(SQLString);
                EXECUTE IMMEDIATE SQLString;
                IF SQLCode != -1 THEN
                    SQLString:='INSERT INTO GATENET.INFO_BIOMETRICO_DT(TIPO_BIOMETRICO,ID_USUARIO,DEPLOY,INFO)
 VALUES('||V_TIPO_BIOMETRICO||','||CHR(39)||V_ID_USUARIO||CHR(39)||','||V_DEPLOY||', (SELECT BIOMETRIC_DATA FROM BIOQCONT2.BIOMETRIC_INFO WHERE NSEQ_BIOMETRIC = '
                    ||REG.NSEQ_BIOMETRIC||'))';
                    DBMS_OUTPUT.PUT_LINE(SQLString);
                    EXECUTE IMMEDIATE SQLString;
                    IF SQLCode != -1 THEN
                        COMMIT;
                    ELSE
                        P_RESULTADO := 'N';
                        ROLLBACK;
                    END IF;
                ELSE
                    P_RESULTADO := 'N';
                    ROLLBACK;
                END IF;
            END LOOP;
        ELSE
            P_RESULTADO := 'N';
            DBMS_OUTPUT.PUT_LINE(TO_CHAR('IMPOSIBLE INSERTAR EN TABLA GATENET.USUARIO'));
            ROLLBACK;
    END IF;
        END;
    END IF;
END; "

             
             */
        }


    }
}
