![](https://user-images.githubusercontent.com/13739520/30005364-3973b666-909e-11e7-96ad-e71a31234ed6.png)

# Oracle-Tablespace-Monitor

## Laboratorio de Administracion de Bases de Datos
Este repositorio funciona como el control de versiones del laboratorio de Monitoreo de tablespaces de Oracle 11g. Este debe poseer las siguientes caracteristicas:

- Mostar el uso de cada tablespaces
- Dar la posibilidad de mostrar el uso de los tablespaces de sistema, aunque el foco son los tablespace de usuario
- Calcular el peso en bytes de un registro de una tabla, para con esto calcular la tasa de crecimiento de un tablespaces
- Calcular el tiempo en dias para llegar al HWM (High Water Mark) de un tablespace
- Calcular el tiempo en dias para llegar al Maximo de un tablespace

## Funciones de la Solución



## Querys necesarios para la ejecucion

``` sql
CONNECT system/manager AS SYSDBA;

CREATE USER dev IDENTIFIED BY dev;
GRANT CONNECT, RESOURCE, DBA TO dev;
GRANT CREATE SESSION TO dev;
GRANT EXECUTE ON DBMS_OUTPUT TO dev WITH GRANT OPTION;
GRANT SELECT ON V_$SGA TO dev;
GRANT SELECT ON V_$SGASTAT TO dev;
GRANT SELECT ON V_$SQL TO dev;
GRANT SELECT ON V_$SESSION TO dev;
GRANT SELECT ON V_$SQLAREA TO dev;

GRANT SELECT ON dba_free_space TO dev;
GRANT SELECT ON dba_tablespaces TO dev;
GRANT SELECT ON dba_data_files TO dev;


GRANT EXECUTE ON SYS.UTL_FILE TO DEV;
CREATE OR REPLACE DIRECTORY MY_DIR AS 'C:\temp';
GRANT READ, WRITE ON DIRECTORY MY_DIR TO DEV;
GRANT SELECT ON DBA_SEGMENTS TO DEV;
GRANT SELECT ON USER_SEGMENTS TO DEV;



--http://dbaforums.org/oracle/lofiversion/index.php?t13112.html
CREATE OR REPLACE FUNCTION get_tablespace_info
  RETURN SYS_REFCURSOR IS
  cr SYS_REFCURSOR;
  BEGIN
    OPEN cr FOR
    SELECT
      ts.tablespace_name,
      TRUNC("SIZE(B)", 2)                       "BYTES_SIZE",
      TRUNC(fr."FREE(B)", 2)                    "BYTES_FREE",
      TRUNC("SIZE(B)" - "FREE(B)", 2)           "BYTES_USED",
      TRUNC((1 - (fr."FREE(B)" / df."SIZE(B)")) * 100, 10) "PCT_USED"
    FROM
      (SELECT
         tablespace_name,
         SUM(bytes) "FREE(B)"
       FROM dba_free_space
       GROUP BY tablespace_name) fr,
      (SELECT
         tablespace_name,
         SUM(bytes)    "SIZE(B)",
         SUM(maxbytes) "MAX_EXT"
       FROM dba_data_files
       GROUP BY tablespace_name) df,
      (SELECT tablespace_name
       FROM dba_tablespaces) ts
    WHERE fr.tablespace_name = df.tablespace_name
          AND fr.tablespace_name = ts.tablespace_name;
    RETURN CR;
  END;
/


SELECT
        tablespace_name,
        sum(bytes) "BYTES",
        current_timestamp
FROM (
  SELECT
    tablespace_name,
    sum(bytes) "BYTES",
    current_timestamp
  FROM user_segments
  WHERE segment_type = 'INDEX' OR segment_type = 'TABLE'
  GROUP BY tablespace_name, current_timestamp
  UNION ALL
  SELECT
    tablespace_name,
    sum(bytes) "BYTES",
    current_timestamp
  FROM dba_segments
  WHERE segment_type = 'INDEX' OR segment_type = 'TABLE'
  GROUP BY tablespace_name, current_timestamp)
      GROUP BY tablespace_name, current_timestamp;


CREATE OR REPLACE PROCEDURE save_tablespace_usage
IS
  v_ctx  DBMS_XMLGEN.CTXHANDLE;
  v_file UTL_FILE.FILE_TYPE;
  v_xml  CLOB;
  v_more BOOLEAN := TRUE;
  fecha  VARCHAR2(100);

  BEGIN

    fecha := to_char(sysdate, 'YYYY-MM-DD HH MI SS AM') || '.xml';
    DBMS_OUTPUT.put_line(fecha);
    -- Create XML context.
    v_ctx := DBMS_XMLGEN.newcontext(
      'SELECT
        tablespace_name,
        sum(bytes) "BYTES",
        current_timestamp
      FROM
      (
        SELECT
          tablespace_name,
          sum(bytes) "BYTES",
          current_timestamp
        FROM user_segments
        WHERE segment_type = ''INDEX'' OR segment_type = ''TABLE''
        GROUP BY tablespace_name, current_timestamp
        UNION ALL
        SELECT
          tablespace_name,
          sum(bytes) "BYTES",
          current_timestamp
        FROM dba_segments
        WHERE segment_type = ''INDEX'' OR segment_type = ''TABLE''
      GROUP BY tablespace_name, current_timestamp)
      GROUP BY tablespace_name, current_timestamp');
    v_xml := DBMS_XMLGEN.getxml(v_ctx);
    DBMS_XMLGEN.closecontext(v_ctx);
    -- Output XML document to file.
    v_file := UTL_FILE.fopen('MY_DIR', fecha, 'w');
    WHILE v_more LOOP
      UTL_FILE.put(v_file, SUBSTR(v_xml, 1, 32767));
      IF LENGTH(v_xml) > 32767
      THEN
        v_xml := SUBSTR(v_xml, 32768);
      ELSE
        v_more := FALSE;
      END IF;
    END LOOP;
    UTL_FILE.fclose(v_file);

  END;
/


BEGIN
DBMS_SCHEDULER.CREATE_JOB (
 job_name        => 'TrabajoTablespace',
 job_type        => 'STORED_PROCEDURE',
 job_action      => 'save_tablespace_usage',
 start_date      => '7-SEP-17 10.30.00PM',
 repeat_interval => 'FREQ=MINUTELY;INTERVAL=2;',
 enabled         => TRUE);
END;

```


## Segun la respuesta de TOM

``` sql
alter system set control_management_pack_access='DIAGNOSTIC+TUNING';


select * from (
select 
   dhs.begin_interval_time,
   dt.tablespace_name,
   trunc(dhtsu.tablespace_size*dt.block_size)  bytes,
   trunc(dhtsu.tablespace_usedsize*dt.block_size) bytes_used,
   row_number()
            OVER (
              PARTITION BY dt.tablespace_name
              ORDER BY dhs.begin_interval_time DESC ) AS rank
from
   dba_hist_tbspc_space_usage dhtsu,
   v$tablespace vts,
   dba_tablespaces dt,
   dba_hist_snapshot dhs
where dhtsu.snap_id = dhs.snap_id
and   dhtsu.tablespace_id = vts.ts#
and   vts.name          = dt.tablespace_name 
) aaa where rank < 2;




select 
   dhs.begin_interval_time,
   dt.tablespace_name,
   trunc(dhtsu.tablespace_size*dt.block_size/1024/1024/1024)  gb,
   trunc(dhtsu.tablespace_usedsize*dt.block_size/1024/1024/1024) gb_used
from
   dba_hist_tbspc_space_usage dhtsu,
   v$tablespace vts,
   dba_tablespaces dt,
   dba_hist_snapshot dhs
where dhtsu.snap_id = dhs.snap_id
and   dhtsu.tablespace_id = vts.ts#
and   vts.name          = dt.tablespace_name 
order by 2,1;



```
