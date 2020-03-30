Declare
/*****************************************************************
Raj Valluru  04/09/2017 - One time script to populate DUES_CARD_MASTER_HISTORY
*****************************************************************/
	ln_existing_rec_count NUMBER :=0;
 begin
		Select count(*) into ln_existing_rec_count FROM DUES_CARD_MASTER_HISTORY;
		if ln_existing_rec_count = 0 then
			insert into DUES_CARD_MASTER_HISTORY  (
			   customer,
			   FILE_NAME,
			   STORAGE_OPTION,
			   addoper,
			   adddate
			   ) 
			Select customer,
			   FILE_NAME,
			   STORAGE_OPTION,
			   NVL(modoper, addoper), NVL(moddate, adddate) FROM DUES_CARD_MASTER;
		else
			dbms_output.put_line ('Initial data is already loaded!!');
		end if;
   --
   exception
      when OTHERS then
         dbms_output.put_line ('====> ' || sqlerrm);
         raise_application_error (-20417,'Insert to DUES_CARD_MASTER_HISTORY failed: ' || sqlerrm);
end;
/
