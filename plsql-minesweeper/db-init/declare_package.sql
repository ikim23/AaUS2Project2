create or replace package ms as
    game_id games.id%type;
    field_size games.field_size%type;
    mines_count games.mines_count%type;
    procedure list_games;
    procedure choose_game(p_game_id in games.id%type);
    procedure new_game(p_field_size in games.field_size%type, p_mines_count in games.mines_count%type);
    procedure create_game(p_field_size in games.field_size%type, p_mines_count in games.mines_count%type);
    procedure create_points(p_points out points_list);
    procedure create_mines(p_points in out points_list);
    procedure create_grounds(p_points in out points_list);
    procedure calc_mine_counts;
    procedure increment_mine_count(x in integer, y in integer);
    procedure print;
    procedure set_all_visible;
end ms;
/
