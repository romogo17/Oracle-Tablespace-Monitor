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
```
