-- Create Tables section

Create table osoba (
	rod_cislo Char (10) NOT NULL ,
	PSC Char (5) NOT NULL ,
	meno Varchar2 (30) NOT NULL ,
	priezvisko Varchar2 (30) NOT NULL ,
	ulica Varchar2 (30),
primary key (rod_cislo) 
) 
/

Create table krajina (
	id_krajiny Char (2) NOT NULL ,
	nazov Varchar2 (30) NOT NULL ,
primary key (id_krajiny) 
) 
/

Create table region (
	id_regionu Char (2) NOT NULL ,
	nazov Varchar2 (30) NOT NULL ,
	id_krajiny Char (2) NOT NULL ,
primary key (id_regionu) 
) 
/

Create table mesto (
	PSC Char (5) NOT NULL ,
	nazov Varchar2 (30) NOT NULL ,
	id_regionu Char (2) NOT NULL ,
primary key (PSC) 
) 
/

Create table bm_udaje (
	rod_cislo Char (10) NOT NULL ,
	vyska Integer,
	vaha Integer,
	farba_pleti Char (7),
	farba_vlasov Char (7),
	farba_oci Char (7),
	odtlacok_prsta Blob,
	odtlacok_prsta_filename Varchar2 (50),
	fotka Blob,
	fotka_filename Varchar2 (50),
primary key (rod_cislo) 
) 
/

Create table trestny_cin (
	id_cinu Integer NOT NULL ,
	id_obvodu Varchar2 (30) NOT NULL ,
	druh Varchar2 (30) NOT NULL ,
	popis Clob,
	popis_filename Varchar2 (50),
	hodnota_skody Integer,
	objasneny Char (1) NOT NULL ,
	datum_zapisu Date NOT NULL ,
	datum_uzavretia Date,
primary key (id_cinu) 
) 
/

Create table obvod (
	id_obvodu Varchar2 (30) NOT NULL ,
	nazov Varchar2 (30) NOT NULL ,
	PSC Char (5) NOT NULL ,
primary key (id_obvodu) 
) 
/

Create table ucastnik_cinu (
	rod_cislo Char (10) NOT NULL ,
	id_cinu Integer NOT NULL ,
	typ Char (1) NOT NULL ,
	od Date NOT NULL ,
	do Date,
	svedectvo Clob,
	svedectvo_filename Varchar2 (50),
	dovod Clob,
	dovod_filename Varchar2 (50),
	dat_nastupu Date,
	dlzka_trestu Integer,
	miesto_vyk_trestu Varchar2 (30),
primary key (rod_cislo,id_cinu,typ,od) 
) 
/

-- Create Foreign keys section

Alter table bm_udaje add  foreign key (rod_cislo) references osoba (rod_cislo) 
/

Alter table ucastnik_cinu add  foreign key (rod_cislo) references osoba (rod_cislo) 
/

Alter table region add  foreign key (id_krajiny) references krajina (id_krajiny) 
/

Alter table mesto add  foreign key (id_regionu) references region (id_regionu) 
/

Alter table osoba add  foreign key (PSC) references mesto (PSC) 
/

Alter table obvod add  foreign key (PSC) references mesto (PSC) 
/

Alter table ucastnik_cinu add  foreign key (id_cinu) references trestny_cin (id_cinu) 
/

Alter table trestny_cin add  foreign key (id_obvodu) references obvod (id_obvodu) 
/

-- Create Objects section

create sequence sekv_id_plat start with 1 increment by 1;

create sequence sekv_id_cin start with 1 increment by 1;

create or replace type t_plat is object (
    id integer,
    suma integer,
    od date,
    do date,
    constructor function t_plat(p_suma integer, p_od date) return self as result,
    member function get_suma return integer
);
/

create or replace type body t_plat as
    constructor function t_plat(p_suma integer, p_od date) return self as result is
    begin
        self.id := sekv_id_plat.nextval;
        self.suma := p_suma;
        self.od := p_od;
        self.do := null;
        return;
    end;
    member function get_suma return integer is
    begin
        return suma;
    end;
end;
/

create type t_tab_plat is table of t_plat;
/

create or replace type t_zamestnanec is object (
    id_zam integer,
    rod_cislo char(10),
    funkcia varchar2(15),
	hodnost varchar2(15),
    od date,
    do date,
    platy t_tab_plat,
    member function get_rocny_plat(rok integer) return integer
);
/

create sequence sekv_id_zam start with 1 increment by 1;

create or replace type body t_zamestnanec as
    member function get_rocny_plat(rok integer) return integer is
        v_plat integer;
    begin
        select sum(
            ceil(months_between(
                -- konecny mesiac, ak nema konecny datum pouzije sa 1.1. buduceho roku
                case
                    when extract(year from nvl(do, add_months(to_date('01.01.' || rok, 'DD.MM.YYYY'), 12))) > rok then to_date('01.01.' || (rok + 1), 'DD.MM.YYYY') - 1
                    else do
                end,
                -- pociatocny mesiac, ak je < ako rok, zobere 1.1. daneho roku
                case
                    when extract(year from od) < rok then to_date('01.01.' || rok,'DD.MM.YYYY')
                    else od
                end
            )) * suma
        ) into v_plat
        from table(self.platy)
        where rok between extract(year from od) and extract(year from nvl(do, sysdate));
        return v_plat;
    end get_rocny_plat;
end;
/

create table zamestnanec of t_zamestnanec (
    constraint cons_zamestnanec_pk primary key(id_zam),
    constraint cons_zamestnanes_fk foreign key(rod_cislo) references osoba
)
nested table platy store as platy;
/

create index idx_osoba_meno_priezvisko on osoba(lower(meno || ' ' || priezvisko));
