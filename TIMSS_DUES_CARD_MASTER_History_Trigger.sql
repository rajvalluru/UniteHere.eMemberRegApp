CREATE OR REPLACE TRIGGER TIMSS.DUES_CARD_MASTER_TS
AFTER UPDATE
ON TIMSS.DUES_CARD_MASTER
FOR EACH ROW
Declare
/*****************************************************************
Raj Valluru  04/09/2017 - Keeping track of changes to DUES_CARD_MASTER
*****************************************************************/
 begin
		insert into DUES_CARD_MASTER_HISTORY  (
		   customer,
		   FILE_NAME,
		   STORAGE_OPTION,
		   addoper,
		   adddate
		   ) values (
		   :new.customer,
		   :new.FILE_NAME,
		   :new.STORAGE_OPTION,
		   :new.modoper,
		   :new.moddate
		);
   --
   exception
      when OTHERS then
         dbms_output.put_line ('====> ' || sqlerrm);
		 session_err.put_err('DUES_CARD_MASTER_TS: ' || sqlerrm);
         raise_application_error (-20217,'DUES_CARD_MASTER_TS: ' || sqlerrm);
end;
/
