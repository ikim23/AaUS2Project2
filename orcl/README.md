## Docker commands:

Run application inside Docker container:
```
docker-compose up app
```
Start bash inside Docker container:
```
docker-compose run -p 3000:3000 shell
```
Show running Docker containers:
```
docker ps
```
Stop all Docker containers:
```
docker stop $(docker ps -aq)
```

## How to debug:

### Requirements:

- Install Google Chrome (not Chromium)
- Install Debugger for Chrome extension in VSCode

## Debug Backend:

- Stop all Docker containers
- Open debugger `Ctrl+Shift+D`
- Choose `Node` configuration
- Start debugging sesstion `F5`

Backend code supports code reload. You can change the code and after `Ctrl+S` content will refresh automatically without need of stopping debug session.

## Debug Backend + Frontend at once:

1. Open Google Chrome in debug mode:
```
google-chrome --remote-debugging-port=9222 http://localhost:3000/
```

2. Start debug session:
- Stop all Docker containers
- Open debugger `Ctrl+Shift+D`
- Choose `Full-Stack` configuration
- Start debugging sesstion `F5`
