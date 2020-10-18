create user PLAYER identified by player;
grant connect, resource to PLAYER;
alter user PLAYER quota unlimited on users;

exit;
