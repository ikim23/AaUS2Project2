create or replace package body ms as
    procedure list_games is
        cursor unfinished_games is
            select *
            from games g
            where end_date is null
            order by id;
    begin
        dbms_output.put_line('|  ID | SIZE | MINES |    START DATE    |');
        dbms_output.put_line('-----------------------------------------');
        for game in unfinished_games loop
            dbms_output.put_line(
                '| ' || lpad(game.id, 3) || ' | ' ||
                lpad(game.field_size, 4) || ' | ' ||
                lpad(game.mines_count, 5) || ' | ' ||
                to_char(game.start_date, 'DD.MM.YYYY HH24:MI') || ' |'
            );
        end loop;
    end;
    
    procedure choose_game(p_game_id in games.id%type) is
    begin
        select id, field_size, mines_count
        into game_id, field_size, mines_count
        from games
        where id = p_game_id;
    end;
        
    procedure new_game(p_field_size in games.field_size%type, p_mines_count in games.mines_count%type) is
        points points_list;
        i integer := 0;
        idx integer := 0;
    begin
        create_game(p_field_size, p_mines_count);
        create_points(points);
        create_mines(points);
        create_grounds(points);
    end;
    
    procedure create_game(p_field_size in games.field_size%type, p_mines_count in games.mines_count%type) is
    begin
        insert into games(field_size, mines_count)
        values (p_field_size, p_mines_count);
        select id, field_size, mines_count
        into game_id, field_size, mines_count
        from games
        where id = (select max(id) from games);
    end;
    
    procedure create_points(p_points out points_list) is
    begin
        p_points := points_list();
        for x in 1..field_size loop
            for y in 1..field_size loop
                p_points.extend();
                p_points(p_points.last) := point(x, y);
            end loop;
        end loop;
    end;
    
    procedure create_mines(p_points in out points_list) is
        mine_count integer := 0;
        idx integer;
    begin
        loop
            if mine_count >= mines_count then
                exit;
            end if;
            idx := trunc(dbms_random.value(p_points.first, p_points.last));
            if p_points.exists(idx) then
                insert into fields
                values(mine(game_id, p_points(idx).x, p_points(idx).y));
                p_points.delete(idx);
                mine_count := mine_count + 1;
            end if;
        end loop;
    end;
    
    procedure create_grounds(p_points in out points_list) is
        idx integer;
    begin
        idx := p_points.first;
        while idx is not null loop
            insert into fields values(ground(game_id, p_points(idx).x, p_points(idx).y));
            idx := p_points.next(idx);
        end loop;
    end;
    
    procedure calc_mine_counts is
        cursor mines is
            select x, y
            from fields f
            where game_id = game_id
            and value(f) is of (only mine);
        deltaX integer := -1;
        deltaY integer := -1;
    begin
        for mine in mines loop
            dbms_output.put_line('mine: ' || mine.x || ' ' || mine. y);
            while deltaX <= 1 loop
                while deltaY <= 1 loop
                    if not (deltaY = 0 and deltaX = deltaY) then
                        if mine.x + deltaX > 0 and mine.x + deltaX <= field_size and
                           mine.y + deltaY > 0 and mine.y + deltaY <= field_size then
                            increment_mine_count(mine.x + deltaX, mine.y + deltaY);
                        end if;
                    end if;
                    deltaY := deltaY + 1;
                end loop;
                deltaY := -1;
                deltaX := deltaX + 1;
            end loop;
        end loop;
    end;
    
    procedure increment_mine_count(x in integer, y in integer) is
        v_ground ground;
    begin
        select treat(value(f) as ground) into v_ground
        from fields f
        where f.x = x and f.y = y;
        update fields f
        set f = v_ground.increment_mine_count
        where f.x = x and f.y = y;
    end;
    
    procedure print is
        row_idx integer := 0;
        col_idx integer := 0;
        cursor fields is 
            select value(f) as val
            from fields f
            where game_id = game_id
            order by x, y;
    begin
        dbms_output.put('*  |');
        for i in 1..field_size loop
            dbms_output.put(lpad(i, 2) || ' |');
        end loop;
        for field in fields loop
            if mod(col_idx, field_size) = 0 then
                row_idx := row_idx + 1;
                dbms_output.put_line(chr(10) || rpad('-', 4 + field_size * 4, '-'));
                dbms_output.put(lpad(row_idx || ' | ', 5));
            end if;
            col_idx := col_idx + 1;
            dbms_output.put(field.val.to_string || ' | ');
        end loop;
        dbms_output.put_line('');
    end;
    
    procedure set_all_visible is
    begin
        update fields
        set visible = 1
        where game_id = game_id;
    end;
end ms;
/

set SERVEROUT on;
execute ms.new_game(12, 19);
execute ms.list_games;
execute ms.choose_game(7);
execute ms.set_all_visible;
execute ms.print;
