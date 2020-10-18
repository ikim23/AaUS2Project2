create or replace procedure new_game(p_field_size in games.field_size%type, p_mines_count in integer) is
    game_id games.id%type;
    field_size games.field_size%type;
    mines_count games.mines_count%type;
    type t_point is record (x integer, y integer);
    type t_points is table of t_point;
    points t_points;    
    i integer := 0;
    idx integer := 0;
begin
    -- create game
    insert into games(field_size, mines_count)
    values(p_field_size, p_mines_count);
    select id, field_size, mines_count
    into game_id, field_size, mines_count
    from games
    where id = (select max(id) from games);
    -- generate points
    points := t_points();
    for x in 1..field_size loop
        for y in 1..field_size loop
            points.extend();
            points(points.last).x := x;
            points(points.last).y := y;
        end loop;
    end loop;    
    -- create mine fields
    loop
        if i >= mines_count then
            exit;
        end if;
        idx := trunc(dbms_random.value(1, points.last));
        if points.exists(idx) then
            insert into fields values(mine(game_id, points(idx).x, points(idx).y));
            points.delete(idx);
            i := i + 1;
        end if;
    end loop;
    -- create ground fields
    i := points.first;
    while i is not null loop
        insert into fields values(ground(game_id, points(i).x, points(i).y));
        i := points.next(i);
    end loop;
end;
/

set serveroutput on;

execute new_game(10, 10);

select * from games;
select * from fields;