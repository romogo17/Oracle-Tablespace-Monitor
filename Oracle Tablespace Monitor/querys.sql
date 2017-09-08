﻿CONNECT system/manager AS SYSDBA;

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
CREATE OR REPLACE DIRECTORY MY_DIR AS 'C:\Users\Roberto\Desktop\tablespace_usage';
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
      TRUNC("SIZE(B)", 2)                                  "BYTES_SIZE",
      TRUNC(fr."FREE(B)", 2)                               "BYTES_FREE",
      TRUNC("SIZE(B)" - "FREE(B)", 2)                      "BYTES_USED",
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
