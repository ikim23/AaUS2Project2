# Docker Example

## Start Docker Compose
```
docker-compose up
```

## Initialize database
- Open URL: http://localhost:8080/
- Import todos table schema:
    1. Log-in to Adminer (Username: user, Password: user, Database: todos_db)
    2. Import > Choose Files > Open `mysql/create_table.sql` > Execute
    3. Open todos_db > todos > Select data > Import > Choose File > Open `mysql/todos.csv` > Import

## Check Application:

Frontend (HTML + CSS + JavaScript): http://localhost:4000/

Backend API (Node.js): http://localhost:3000/

MySQL Adminer: http://localhost:8080/
