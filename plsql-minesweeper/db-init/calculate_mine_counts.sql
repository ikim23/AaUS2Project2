create or replace procedure calc_mine_counts(p_game_id in games.id%type) is
    cursor mines(p_id integer) is
        select x, y
        from fields f
        where game_id = p_id
        and value(f) is of (only mine);
    field_size games.field_size%type;
    deltaX integer := -1;
    deltaY integer := -1;
    v_ground ground;
begin
    select field_size into field_size
    from games
    where id = p_game_id;
    for mine in mines(p_game_id) loop
        dbms_output.put_line('mine: ' || mine.x || ' ' || mine. y);
        while deltaX <= 1 loop
            while deltaY <= 1 loop
                if not (deltaY = 0 and deltaX = deltaY) then
                    if mine.x + deltaX > 0 and mine.x + deltaX <= field_size and mine.y + deltaY > 0 and mine.y + deltaY <= field_size then
                        select treat(value(f) as ground) into v_ground
                        from fields f
                        where f.x = mine.x + deltaX
                        and f.y = mine.y + deltaY;
                        dbms_output.put_line((mine.x + deltaX) || ' ' || (mine.y + deltaY));
--                        gr.mine_count := gr.mine_count + 1;
                        update fields f
                        set f = v_ground.increment_mine_count
                        where f.x = v_ground.x
                        and f.y = v_ground.y;
                    end if;
                end if;
                deltaY := deltaY + 1;
            end loop;
            deltaY := -1;
            deltaX := deltaX + 1;
        end loop;
    end loop;
end;
/


select * from games;

exec calc_mine_counts(7);

