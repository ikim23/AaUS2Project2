create sequence seq_game_id start with 1 increment by 1;

create table games (
    id integer primary key,
    field_size integer not null,
    mines_count integer not null,
    start_date date not null,
    end_date date
);

create or replace trigger trig_set_game_id before insert on games
for each row
begin
    :new.id := seq_game_id.nextval;
    :new.start_date := sysdate;
    if :new.field_size < 10 then
        :new.field_size := 10;
    end if;
    if :new.mines_count < :new.field_size or :new.mines_count >= :new.field_size * :new.field_size then
        :new.mines_count := :new.field_size * 2;
    end if;
end;
/

create table field_symbols (
    id varchar2(30) primary key,
    symbol char not null
);

insert into field_symbols(id, symbol) values('hidden_field', ' ');
insert into field_symbols(id, symbol) values('mine_found', 'm');
insert into field_symbols(id, symbol) values('mine_exploded', 'M');

create table fields of field (
    constraint fields_pk primary key (game_id, x, y),
    constraint fields_fk foreign key (game_id) references games (id)
);

create type point is object(
    x integer,
    y integer
);
/

create type points_list is table of point;
/


insert into games(field_size) values(5);
select * from games;
drop table games;
drop table fields;
