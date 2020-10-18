-- vytvor zaznam v tabulke OSOBA a ZAMESTNANEC
-- prirad pociatocny PLAT ku datumu nastupu do prace
-- datum nastupu do prace je orezany na zaciatok mesiaca
-- vrati ID_ZAM po uspesnom vytvoreni zaznamu
create or replace procedure insert_zamestnanec(
    p_meno in osoba.meno%type,
    p_priezvisko in osoba.priezvisko%type,
    p_rod_cislo in osoba.rod_cislo%type,
    p_psc in osoba.psc%type,
    p_ulica in osoba.ulica%type,
    p_funkcia in zamestnanec.funkcia%type,
    p_hodnost in zamestnanec.hodnost%type,
    p_od in zamestnanec.od%type,
    p_suma in integer,
    p_id_zam out number
) is
    v_od zamestnanec.od%type;
begin
    p_id_zam := sekv_id_zam.nextval;
    insert into osoba(meno, priezvisko, rod_cislo, psc, ulica)
    values(p_meno, p_priezvisko, p_rod_cislo, p_psc, p_ulica);
    select trunc(p_od, 'MM') into v_od from dual;
    insert into zamestnanec values(
        new t_zamestnanec(
            p_id_zam,
            p_rod_cislo,
            p_funkcia,
            p_hodnost,
            v_od,
            null,
            t_tab_plat(
                t_plat(p_suma, v_od)
            )
        )
    );
end;
/

-- aktualizuj zaznam v tabulke ZAMESTNANEC a OSOBA
-- datum nastupu do prace je orezany na zaciatok mesiaca
-- datum ukocenia pracovneho pomeru je orezany na koniec mesiaca
create or replace procedure update_zamestnanec(
    p_id_zam in zamestnanec.id_zam%type,
    p_meno in osoba.meno%type,
    p_priezvisko in osoba.priezvisko%type,
    p_psc in osoba.psc%type,
    p_ulica in osoba.ulica%type,
    p_funkcia in zamestnanec.funkcia%type,
    p_hodnost in zamestnanec.hodnost%type,
    p_od in zamestnanec.od%type,
    p_do in zamestnanec.do%type
) is
    v_old_od zamestnanec.od%type;
    v_old_do zamestnanec.do%type;
    v_platy zamestnanec.platy%type;
    v_od zamestnanec.od%type;
    v_do zamestnanec.do%type;
begin
    select od, do, platy
    into v_old_od, v_old_do, v_platy
    from zamestnanec
    where id_zam = p_id_zam;
    
    select trunc(p_od, 'MM'), trunc(p_do, 'MM') - 1
    into v_od, v_do
    from dual;
    
    if v_od <> v_old_od then
        if v_od > v_old_od then
            RAISE_APPLICATION_ERROR(-20001, 'Datum p_od musi byt mensi ako ' || to_char(v_old_od));
        end if;
        v_platy(v_platy.first).od := v_od;
    end if;
    if v_old_do is not null and v_do < v_old_do then
        RAISE_APPLICATION_ERROR(-20002, 'Datum p_do musi byt vacsi ako ' || to_char(v_old_do));
    end if;
    v_platy(v_platy.last).do := v_do;
    
    update zamestnanec
    set funkcia = p_funkcia,
    hodnost = p_hodnost,
    od = v_od,
    do = v_do,
    platy = v_platy
    where id_zam = p_id_zam;
    
    update osoba
    set meno = p_meno,
    priezvisko = p_priezvisko,
    psc = p_psc,
    ulica = p_ulica
    where rod_cislo = (
        select rod_cislo
        from zamestnanec
        where id_zam = p_id_zam
    );
end;
/

-- odstran zaznam z tabulky ZAMESTNANEC, BM_UDAJE a OSOBA
create or replace procedure delete_zamestnanec(p_id_zam in zamestnanec.id_zam%type) is
    v_rod_cislo osoba.rod_cislo%type;
begin
    select rod_cislo into v_rod_cislo
    from zamestnanec where id_zam = p_id_zam;
    delete zamestnanec
    where id_zam = p_id_zam;
    delete bm_udaje
    where rod_cislo = v_rod_cislo;
    delete osoba
    where rod_cislo = v_rod_cislo;
end;
/

-- ukonci aktualny plat a vloz novy
create or replace procedure insert_plat(
    p_id_zam in zamestnanec.id_zam%type,
    p_suma in integer,
    p_od in date
) is
    v_posledny_plat date;
    v_od date;
begin
    select max(od)
    into v_posledny_plat
    from table(
        select platy
        from zamestnanec
        where id_zam = p_id_zam
    );
    select trunc(p_od, 'MM') into v_od from dual;
    if p_od < v_posledny_plat then
        RAISE_APPLICATION_ERROR(-20001, 'Datum musi byt vacsi ako ' || to_char(v_posledny_plat));
    end if;
    -- ukonci posledny plat
    update table(
        select platy
        from zamestnanec
        where id_zam = p_id_zam
    ) set do = trunc(v_od, 'MM') - 1
    where od = v_posledny_plat;
    -- vloz novy plat
    insert into table(
        select platy
        from zamestnanec
        where id_zam = p_id_zam
    ) values (t_plat(p_suma, v_od));
end;
/

create or replace procedure delete_trestny_cin(p_id_cinu in integer) is
begin
  delete ucastnik_cinu
  where id_cinu = p_id_cinu;
  delete trestny_cin
  where id_cinu = p_id_cinu;
  commit;
end;
/

-- odstran zaznam z tabulky ZAMESTNANEC, BM_UDAJE a OSOBA
create or replace procedure delete_osoba(p_rod_cislo in osoba.rod_cislo%type) is
begin
    delete from zamestnanec
    where id_zam in (
        select id_zam
        from zamestnanec
        where rod_cislo = p_rod_cislo
    );
    delete bm_udaje
    where rod_cislo = p_rod_cislo;
    delete osoba
    where rod_cislo = p_rod_cislo;
end;
/

create or replace procedure insert_crime(
  p_id_obvodu in trestny_cin.id_obvodu%type,
  p_type in trestny_cin.druh%type,
  p_popis in blob,
  p_popisNazovSub in trestny_cin.popis_filename%type,
  p_hod_skody in integer,
  p_objasneny in trestny_cin.objasneny%type,
  p_dat_zapisu in varchar2,
  p_dat_uzavr in varchar2
) is
begin
  insert into trestny_cin(id_obvodu, druh, popis, popis_filename, hodnota_skody, objasneny, datum_zapisu, datum_uzavretia) 
  values(
    p_id_obvodu,
    p_type,
    p_popis,
    p_popisNazovSub,
    p_hod_skody,
    p_objasneny,
    to_date(p_dat_zapisu, 'YYYY-MM-DD'),
    to_date(p_dat_uzavr, 'YYYY-MM-DD')
  );
  commit;
end;
/

create or replace procedure update_crime(
    p_id_cinu in trestny_cin.id_cinu%type,
    p_id_obvodu in trestny_cin.id_obvodu%type,
    p_type in trestny_cin.druh%type,
    p_popis in blob,
    p_popisNazovSuboru in trestny_cin.popis_filename%type,
    p_hod_skody in integer,
    p_objasneny in trestny_cin.objasneny%type,
    p_dat_zapisu in varchar2,
    p_dat_uzavr in varchar2
) is
begin
    update trestny_cin
    set
    id_obvodu = p_id_obvodu,
    druh = p_type,
    popis = p_popis,
    popis_filename = p_popisNazovSuboru,
    hodnota_skody = p_hod_skody,
    objasneny = p_objasneny,
    datum_zapisu = to_date(p_dat_zapisu, 'YYYY-MM-DD'),
    datum_uzavretia = to_date(p_dat_uzavr, 'YYYY-MM-DD')
    where id_cinu = p_id_cinu;
end;
/

create or replace procedure insert_participant(
    p_personNum in osoba.rod_cislo%type,
    p_crimeId in trestny_cin.id_cinu%type,
    p_type in ucastnik_cinu.typ%type,
    p_fromDate in ucastnik_cinu.od%type,
    p_toDate in ucastnik_cinu.do%type,
    p_witness in blob,
    p_witnessFileName in ucastnik_cinu.svedectvo_filename%type,
    p_reason in blob,
    p_reasonFileName in ucastnik_cinu.dovod_filename%type,
    p_date in ucastnik_cinu.dat_nastupu%type,
    p_punishmentLength in integer,
    p_punishmentPlace in ucastnik_cinu.miesto_vyk_trestu%type
) is
begin
    insert into ucastnik_cinu(rod_cislo, id_cinu, typ, od, do, svedectvo, svedectvo_filename,
        dovod, dovod_filename, dat_nastupu, dlzka_trestu, miesto_vyk_trestu)
    values(p_personNum, p_crimeId, p_type, p_fromDate, p_toDate, p_witness, p_witnessFileName,
        p_reason, p_reasonFileName, p_date, p_punishmentLength, p_punishmentPlace);
end;
/

create or replace procedure update_participant(
    p_personNum in osoba.rod_cislo%type,
    p_crimeId in trestny_cin.id_cinu%type,
    p_type in ucastnik_cinu.typ%type,
    p_fromDate in ucastnik_cinu.od%type,
    p_toDate in ucastnik_cinu.do%type,
    p_witness in blob,
    p_witnessFileName in ucastnik_cinu.svedectvo_filename%type,
    p_reason in blob,
    p_reasonFileName in ucastnik_cinu.dovod_filename%type,
    p_date in ucastnik_cinu.dat_nastupu%type,
    p_punishmentLength in integer,
    p_punishmentPlace in ucastnik_cinu.miesto_vyk_trestu%type
) is
begin
    update ucastnik_cinu
    set
    do = p_toDate,
    svedectvo = p_witness,
    svedectvo_filename = p_witnessFileName,
    dovod = p_reason,
    dovod_filename = p_reasonFileName,
    dat_nastupu = p_date,
    dlzka_trestu = p_punishmentLength,
    miesto_vyk_trestu = p_punishmentPlace
    where rod_cislo = p_personNum
    and id_cinu = p_crimeId
    and typ = p_type
    and od = p_fromDate;
end;
/

create or replace procedure delete_participant(
    p_personNum in osoba.rod_cislo%type,
    p_crimeId in trestny_cin.id_cinu%type,
    p_type in ucastnik_cinu.typ%type,
    p_fromDate in ucastnik_cinu.od%type
) is
begin
    delete from ucastnik_cinu
    where rod_cislo = p_personNum
    and id_cinu = p_crimeId
    and typ = p_type
    and od = p_fromDate;
end;
/
