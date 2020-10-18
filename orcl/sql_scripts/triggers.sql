
create or replace trigger insert_trestny_cin
before insert on trestny_cin
referencing new as novy
for each row
begin
  select sekv_id_cin.nextval into :novy.id_cinu from dual;
end;
/
