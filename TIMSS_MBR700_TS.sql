CREATE OR REPLACE TRIGGER TIMSS.MBR700_TS
/*****************************************************************************/
/*
TMA Resources Inc,
Proprietary AND Confidential
Version 	: 1.4.5.2 RELEASE
DATE		: 07/06/2001
-----------------------------------------------------------------------------*/
/*  Date         Author      Description                                     */
/*  ----------   ---------   ----------------------------------------------- */
/*  08/30/1999   KShanthi    Inserts or Updates table membership_billing on  */
/*                           on insert or update of house_customer of        */
/*                           membership table                                */
/*                                                                           */
/*  10/15/1999   VKurian     Whenever a new record created with housecustomer*/
/*                           not empty or when a house_customer is updated   */
/*                           a new record created in cus_employment          */
/* 12/28/1999    RMoin       A new record need to be inserted in membership  */
/* 				 			 billing table if member becomes checkoff.		 */
/*							 And also all records need to be deleted from    */
/*							 membership billing if he becomes self pay       */
/* 03/14/2008	 Sudhakar	 Added the code to insert the mbrlevel1,level2   */
/* 				 			 and level3 codes into membership_master table	 */
/* 03/31/2013    Raj Valluru  Taking care of creating Detail line in case of MBR011 */
/* 04/24/2017   Raj Valluru   Added code so that we do not update the End_Date of the current Primary if it already has EndDate */
/*****************************************************************************/
AFTER INSERT OR UPDATE
OF HOUSE_CUSTOMER, CHECKOFF_SELFPAY_FLAG
ON TIMSS.MEMBERSHIP FOR EACH ROW
DECLARE
  l_count NUMBER;
  lv_card_signed_date DATE;
  pv_emp_number CUS_EMPLOYMENT.EMPLOYEE_ID%TYPE;
  pv_wage CUS_EMPLOYMENT.wage%TYPE;
  pv_signed_date DATE;
  pv_dues_card_date DATE;
  pv_dept CUS_EMPLOYMENT.CUSDEPT_CODE%TYPE;
  pv_shift CUS_EMPLOYMENT.DEMSHIFT_CODE%TYPE;
  l_house CUSTOMER.CUSTOMER%TYPE;
  --
  lv_InitWithDues RAMS_PARAMETER.PARAMETER_VALUE%TYPE;
  lv_new_order MEMBERSHIP_MASTER.ORDER_NO%TYPE;
  ld_first_order_date DATE;
  ld_employment_start_date DATE;
  lv_local_enforce_unsec_flag VARCHAR2(40);
  lv_enforce_union_sec_flag VARCHAR2(12);
  ln_current_contract_exists NUMBER;
  ln_union_sec_days_start_doh NUMBER;
  lv_error_msg VARCHAR2(2000);
BEGIN
  --
  dbms_output.put_line('Entering MBR700_TS ........');
  --
  IF INSERTING OR UPDATING THEN
  --
  -- Taking care of MEMBERSHIP_BILLING TABLE
  --
  	IF :NEW.CHECKOFF_SELFPAY_FLAG = 'N' THEN
    --
     SELECT COUNT(*) INTO l_count
     FROM 	MEMBERSHIP_BILLING
     WHERE 	CUSTOMER = :NEW.CUSTOMER
	 AND    house_customer = :NEW.house_customer;
      --
      IF l_count = 0 THEN
        BEGIN
        INSERT INTO MEMBERSHIP_BILLING (CUSTOMER, house_customer, addoper,
                                      adddate, rowversion) VALUES
                                     (:NEW.CUSTOMER,:NEW.house_customer,
                                      '$TriggMBR700',SYSDATE,0);
        EXCEPTION
		 WHEN OTHERS THEN
          dbms_output.put_line ('Raising application error');
		  Session_Err.put_err('MBR700_TS: Error while inserting in MEMBERSHIP_BILLING table for '
            || :NEW.CUSTOMER);
          RAISE_APPLICATION_ERROR (-20119,
          'MBR700_TS: Error while inserting in MEMBERSHIP_BILLING table for '
            || :NEW.CUSTOMER);
        END;
      END IF;
	--
    ELSIF :NEW.CHECKOFF_SELFPAY_FLAG = 'Y' THEN
	--
	  DELETE FROM MEMBERSHIP_BILLING
      WHERE CUSTOMER = :NEW.CUSTOMER;
	--
    END IF;
    -- modification done by VK on 10/15/1999
    -- whenever a new record created with house_customer not empty
    -- or a record is updated with house_customer not empty,
    -- a record is created in CUS_EMPLOYMENT
    -- update all existing records for the customer with primary_flag='N'
    --
    IF NVL(:NEW.HOUSE_CUSTOMER, ' ') != NVL(:OLD.HOUSE_CUSTOMER, ' ') THEN
 	 --IF :new.house_customer is not null then
	 --
	  /*****  WE NEED TO STOP ANY MANUPULATION ON CUS_EMPLOYMENT TABLE,  IF MODOPER IS DEM002O ******/
	  /***** THIS MODIFICATION IS DONE ON 04/05/2000 for 1.3 enhancements *****/
	  IF :NEW.modoper != 'DEM002O' THEN
		  dbms_output.put_line('Updating the end date the current house...');
		  UPDATE CUS_EMPLOYMENT
		  SET    primary_flag='N'
				 ,end_date = TRUNC(SYSDATE) --added this line on 03/25/2007 by Sudhakar
		  WHERE  CUSTOMER = :NEW.CUSTOMER
		  AND    employer_customer != NVL(:NEW.house_customer, ' ')
		  AND    primary_flag = 'Y' AND End_Date IS NULL; --Raj for eMembership added AND End_Date IS NULL /***** 1.3 enhancements *******/
		  /*** 1.3 enhancements *****/
		  /*** If this house is already there, with end date is null, then update it with 'Y' flag ****/
		  IF :NEW.house_customer IS NOT NULL THEN
			  UPDATE CUS_EMPLOYMENT
			  SET    primary_flag ='Y'
			  WHERE  CUSTOMER = :NEW.CUSTOMER
			  AND    employer_customer = :NEW.house_customer
			  AND    end_date IS NULL;
			  --
			  /*** 1.3 enhancements *****/
			  IF SQL%ROWCOUNT = 0 THEN
			  -- This SQL%ROWCOUNT is for above update
			  -- It means that this house as employer is not there, so only then we should insert a record
			  -- in cus_employment table
			  --
			  -- Inserting a record into CUS_Employment table.
			  -- Get Authorization date from customer table.
			  --
			  BEGIN
			  --
				   SELECT MBR_CARD_SIGNED_DATE INTO lv_card_signed_date
				   FROM CUSTOMER
				   WHERE CUSTOMER = :NEW.CUSTOMER;
			   --
			   EXCEPTION
				 WHEN OTHERS THEN
				  dbms_output.put_line ('Raising application error');
				  Session_Err.put_err('MBR700_TS: Error WHILE SELECTING FROM CUSTOMER TABLE FOR '
					|| :NEW.CUSTOMER);
				  RAISE_APPLICATION_ERROR (-20121,
				  'MBR700_TS: Error WHILE SELECTING FROM CUSTOMER TABLE FOR '
					|| :NEW.CUSTOMER);
			  END;
			  --
			  /* Code modified for Ver 146 not to populate fields on
			  update of house customer on Membership
			  */
			  IF INSERTING THEN
				  pv_emp_number:=:NEW.employee_number;
				  pv_wage:=:NEW.wage;
				  pv_signed_date:=lv_card_signed_date;
				  pv_dues_card_date:=:NEW.DUES_CARD_DATE;
				  pv_dept:=:NEW.CUSDEPT_CODE;
				  pv_shift:=:NEW.DEMSHIFT_CODE;
				  -- 08/01/2015 As per Julie, we should always use Begin_Date(i.e. MBRSTATUS_DATE) for employment
				  ld_employment_start_date := NVL(:NEW.MBRSTATUS_DATE, TRUNC(SYSDATE));
					/*-- Use OVERRIDE_DATE, if provided
					IF :NEW.OVERRIDE_DATE IS NOT NULL THEN
						ld_employment_start_date := :NEW.OVERRIDE_DATE;
					ELSE
					   ld_employment_start_date := NVL(:NEW.MBRSTATUS_DATE, TRUNC(SYSDATE));
					END IF;*/
			  ELSE
				  pv_emp_number:=NULL;
				  pv_wage:=NULL;
				  pv_signed_date:=NULL;
				  pv_dues_card_date:=NULL;
				  pv_dept:=NULL;
				  pv_shift:=NULL;
				  ld_employment_start_date := TRUNC(SYSDATE);
			  END IF;
			  BEGIN
				    INSERT INTO  CUS_EMPLOYMENT (
						EMPLOYMENT_ID,
						CUSTOMER,
						PRIMARY_FLAG,
						START_DATE,
						EMPLOYER_CUSTOMER,
						EMPLOYEE_ID,
						AUTHORIZATION_DATE,
						CHECKOFF_DATE,
						WAGE                   ,
						HEALTH_INSURANCE_FLAG  ,
						POLITICAL_DEDUCT_FLAG  ,
						OFF_MONDAY_FLAG        ,
						OFF_TUESDAY_FLAG       ,
						OFF_WEDNESDAY_FLAG     ,
						OFF_THURSDAY_FLAG      ,
						OFF_FRIDAY_FLAG        ,
						OFF_SATURDAY_FLAG      ,
						OFF_SUNDAY_FLAG        ,
						CUSDEPT_CODE, DEMSHIFT_CODE,
						ADDOPER                ,
						ADDDATE                ,
						ROWVERSION             )
					   VALUES
						  (
						SEQ_EMPLOYMENT_ID.NEXTVAL,
					   :NEW.CUSTOMER,
					   'Y',
					   ld_employment_start_date,
					   :NEW.HOUSE_CUSTOMER,
					   pv_emp_number,
					   pv_signed_date,
					   pv_dues_card_date,
					   pv_wage,
					   'N',
					   'N',
					   'N',
					   'N',
					   'N',
					   'N',
					   'N',
					   'N',
					   'N',
					   pv_dept,
					   pv_shift,
					   SUBSTR(USER,1,12),
					   SYSDATE,
					   trans_rowversion.NEXTVAL );
			  --Note: Crafts and related columns are not done
			  --end of modification by VK
			   EXCEPTION
				 WHEN OTHERS THEN
				  dbms_output.put_line ('Raising application error');
				  Session_Err.put_err('MBR700_TS: Error WHILE INSERTING IN EMPLOYMENT TABLE FOR '
					|| :NEW.CUSTOMER);
				  RAISE_APPLICATION_ERROR (-20121,
				  'MBR700_TS: Error WHILE INSERTING IN EMPLOYMNENT TABLE FOR '
					|| :NEW.CUSTOMER);
			  END;
		END IF; /* END OF ADDOPER is DEM002O */
	  END IF; /* END OF IF SQL%ROWCOUNT = 0, it means that this employer is not existing */
    END IF; -- END OF HOUSE CUSTOMER NOT NULL
    --
	/*** 1.3 enhancements, call this even when house is null ***/
	--
	-- Taking care of CUSTOMER TABLE
	--
    Mbr700_Pk.hold_customer(:NEW.CUSTOMER, :NEW.house_customer) ;
	--
   END IF; -- END OF IF INSERTING OR UPDATING HOUSE CUSTOMER
  --
  END IF; -- END OF IF INSERTING OR UPDATING HOUSE CUSTOMER OR UPDATING CHECKOFF FLAG
  --
  IF INSERTING THEN
  --
    UPDATE CUSTOMER SET join_date = SYSDATE
    WHERE CUSTOMER = :NEW.CUSTOMER;
      --
    IF SQL%ROWCOUNT != 1 THEN
      --
      dbms_output.put_line ('Raising application error');
	  Session_Err.put_err('MBR700_TS: Error WHILE UPDATING CUSTOMER TABLE FOR '||
                                                            :NEW.CUSTOMER);
      RAISE_APPLICATION_ERROR (-20118,
        'MBR700_TS: Error WHILE UPDATING CUSTOMER TABLE FOR '||
                                                            :NEW.CUSTOMER);
    END IF;
  END IF;
--
-- Taking care of MEMBERSHIP TABLE
--
  IF INSERTING THEN
  --
   BEGIN
        -- Set the proper Order_Date.  Use Override_date, if provided.
		--        Otherwise, Use BeginDate (MBRSTATUS_DATE) if Dues_Card_Date<BeginDate; Otherwise use "Hire Date" + UNION_SEC_DAYS_START_DOH of the House, if there is current Union Security Contract exists
		ld_first_order_date := NVL(:NEW.OVERRIDE_DATE, NVL(:NEW.MBRSTATUS_DATE, TRUNC(SYSDATE)));
		IF :NEW.OVERRIDE_DATE IS NOT NULL THEN
		   		ld_first_order_date := :NEW.OVERRIDE_DATE;
		ELSE
				ld_first_order_date := NVL(:NEW.MBRSTATUS_DATE, TRUNC(SYSDATE));
				SELECT PARAMETER_VALUE INTO lv_local_enforce_unsec_flag FROM RAMS_PARAMETER WHERE PARAMETER_NAME = 'ENFORCE_UNION_SEC';
  				IF lv_local_enforce_unsec_flag = 'Y' THEN
						ln_current_contract_exists := 0;
				   		SELECT COUNT(*), MAX(UNION_SEC_DAYS_START_DOH) INTO ln_current_contract_exists, ln_union_sec_days_start_doh FROM HOUSE_CONTRACT WHERE HOUSE_ID = :NEW.house_customer
                            AND ld_first_order_date BETWEEN  UNION_SEC_EFFECTIVE_DATE AND UNION_SEC_END_DATE;
						dbms_output.put_line ('.......ln_union_sec_days_start_doh='||ln_union_sec_days_start_doh);
						dbms_output.put_line ('.......DUES_CARD_DATE='||:NEW.DUES_CARD_DATE);
						dbms_output.put_line ('.......mbrstatus_date='||:NEW.mbrstatus_date);
						IF ln_current_contract_exists>0 THEN
						   -- Note: :NEW.mbrstatus_date is the "Hire Date" on MBR011 screen
							IF TRUNC(NVL(:NEW.DUES_CARD_DATE, TRUNC(SYSDATE)), 'MM') >= TRUNC(NVL(:NEW.mbrstatus_date, TRUNC(SYSDATE)), 'MM') THEN
								-- Dues Card Month is same or more than "Hire Date" month - Thus 1st MONTH BILLED is the month of ("Hire Date" + #Days Union Sec)
								ld_first_order_date := :NEW.mbrstatus_date + NVL(ln_union_sec_days_start_doh, 30);
							ELSE
								-- Dues Card Month is prior to "Hire Date" month   - Thus 1st MONTH BILLED is "Hire Date" month
								ld_first_order_date := :NEW.mbrstatus_date;
							END IF;
						END IF;
						dbms_output.put_line ('..................ld_first_order_date='||ld_first_order_date);
						--
				END IF;
		END IF;
	   --added the code to insert the mbrlevel1,level2 and level3 on 03/14/2008 Sudhakar
	   SELECT seq_order2.NEXTVAL + 2000000000 INTO lv_new_order FROM DUAL;
	    INSERT INTO MEMBERSHIP_MASTER(order_no,original_order_no,
	        order_date,end_date,order_status,order_status_date,
	        mbrstatus_code,mbrstatus_date,invoice, invoice_date,
	        mbr_local_code,ship_customer,house_customer,
	        bill_customer,order_total,special_billing_flag,
			mbrlevel1_code,mbrlevel2_code,mbrlevel3_code,
			addoper,adddate,rowversion)
	    VALUES (lv_new_order,NULL,TRUNC(ld_first_order_date,'MM'),
	        LAST_DAY(ld_first_order_date),'P',SYSDATE,:NEW.mbrstatus_code,
	        :NEW.mbrstatus_date,NULL,NULL,
			:NEW.mbr_local_code,:NEW.CUSTOMER,:NEW.house_customer,
	        DECODE(:NEW.checkoff_selfpay_flag,'Y',:NEW.CUSTOMER,:NEW.house_customer),0,'N',
			:NEW.mbrlevel1_code,:NEW.mbrlevel2_code,:NEW.mbrlevel3_code,
			'$TriggMBR700',SYSDATE,0);
	   --
   EXCEPTION
    WHEN OTHERS THEN
    --
      dbms_output.put_line ('Raising application error');
	  Session_Err.put_err('MBR700_TS: Error WHILE INSERTING MEMBERSHIP_MASTER TABLE FOR '||
                                                          :NEW.CUSTOMER);
      RAISE_APPLICATION_ERROR (-20229,
      'MBR700_TS: Error WHILE INSERTING MEMBERSHIP_MASTER TABLE FOR '||
                                                          :NEW.CUSTOMER);
   END;
    --
  END IF;
  --
  --
  DBMS_OUTPUT.PUT_LINE('Exiting MBR700_TS ........');
  --
EXCEPTION
   WHEN OTHERS THEN
      Session_Err.put_err('MBR700_TS : '|| SQLERRM);
      RAISE_APPLICATION_ERROR(-20120,'Error IN MBR700_TS TRIGGER :'|| SQLERRM);
END;
/
