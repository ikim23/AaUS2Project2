drop type body ground;
drop type body mine;
drop type body field;
drop type ground;
drop type mine;
drop type field;

create or replace type field as object (
    game_id integer,
    x integer,
    y integer,
    visible integer,
    member function to_string return char
) not instantiable not final;
/

create or replace type body field as
    member function to_string return char is
        symbol char;
    begin
        select symbol into symbol
        from field_symbols
        where id = 'hidden_field';
        return symbol;
    end;
end;
/

create or replace type mine under field (
    exploded integer,
    constructor function mine(game_id integer, x integer, y integer) return self as result,
    overriding member function to_string return char
);
/

create or replace type body mine as
    constructor function mine(game_id integer, x integer, y integer) return self as result is
    begin
        self.game_id := game_id;
        self.x := x;
        self.y := y;
        self.visible := 0;
        self.exploded := 0;
        return;
    end;
    overriding member function to_string return char is
        symbol char;
    begin
        symbol := (self as field).to_string;
        if self.visible > 0 then
            if self.exploded = 0 then
                select symbol into symbol
                from field_symbols
                where id = 'mine_found';
            else
                select symbol into symbol
                from field_symbols
                where id = 'mine_exploded';
            end if;
        end if;
        return symbol;
    end;
end;
/

create or replace type ground under field (
    mine_count integer,
    constructor function ground(game_id integer, x integer, y integer) return self as result,
    member function increment_mine_count return ground,
    overriding member function to_string return char
);
/

create or replace type body ground as
    constructor function ground(game_id integer, x integer, y integer) return self as result is
    begin
        self.game_id := game_id;
        self.x := x;
        self.y := y;
        self.visible := 0;
        self.mine_count := 0;
        return;
    end;
    member function increment_mine_count return ground is
    begin
        return ground(self.game_id, self.x, self.y, self.visible, self.mine_count + 1);
    end;
    overriding member function to_string return char is
        symbol char;
    begin
        symbol := (self as field).to_string;
        if self.visible > 0 and self.mine_count > 0 then
            return to_char(self.mine_count);
        end if;
        return symbol;
    end;
end;
/
