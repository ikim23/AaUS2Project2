export const BOX = 'b';
export const BOX_IN_HOME = 'B';
export const FREE = ' ';
export const HOME = 'H';
export const PLAYER = 'p';
export const PLAYER_IN_HOME = 'P';
export const WALL = '#';

export default class Sokoban {

    constructor(board) {
      this._initBoard(board);
      this._history = new Front();
    }

    move(vector) {
      const board = this._getBoard();
      const newPlayerPosition = this._addVector(this._playerPosition, vector);
      if (this._isWall(newPlayerPosition)) return false;
      if (this._isBox(newPlayerPosition)) {
        const newBoxPosition = this._addVector(newPlayerPosition, vector);
        if (!this._canMoveBox(newBoxPosition)) return false;
        this._boxes.remove(newPlayerPosition);
        this._boxes.add(newBoxPosition);
      }
      this._playerPosition = newPlayerPosition;
      this._history.push(board);
      return this._getBoard();
    }

    undo() {
      const board = this._history.pop();
      if (board)
        this._initBoard(board);
      return board;
    }

    get undoLength() {
      return this._history.length;
    }

    isComplete() {
      return this._boxes.every(position => this._isHome(position));
    }

    _isBox(position) {
      return this._boxes.has(position);
    }

    _canMoveBox(position) {
      return !this._isWall(position) && !this._boxes.has(position);
    }

    _isHome({x, y}) {
      return this._board[y][x] === HOME;
    }

    _isWall({x, y}) {
      return this._board[y][x] === WALL;
    }

    _addVector({x, y}, [deltaX, deltaY]) {
      return {x: x + deltaX, y: y + deltaY};
    }

    _cloneBoard(board = this._board) {
      return board.map(row => row.concat());
    }

    _initBoard(board) {
      this._board = this._cloneBoard(board);
      this._boxes = new PositionSet();
      this._board.forEach((row, y) => row.forEach((char, x) => {
        switch (char) {
          case BOX:
            this._boxes.add({x, y});
            row[x] = FREE;
            break;
          case BOX_IN_HOME:
            this._boxes.add({x, y});
            row[x] = HOME;
            break;
          case PLAYER:
            this._playerPosition = {x, y};
            row[x] = FREE;
            break;
        }
      }));
    }

    _getBoard() {
      let board = this._cloneBoard();
      const { x, y } = this._playerPosition;
      board[y][x] = this._isHome({x, y}) ? PLAYER_IN_HOME : PLAYER;
      this._boxes.forEach(({x, y}) => {
          board[y][x] = this._isHome({x, y}) ? BOX_IN_HOME : BOX;
      });
      return board;
    }

}

class PositionSet {

    constructor(positions) {
        this._positions = [];
        if (Array.isArray(positions))
            positions.forEach(position => this.add(position));
    }

    has({x, y}) {
        return this._positions.some(position => position.x === x && position.y === y);
    }

    add(position) {
        if (!this.has(position))
            this._positions.push(position);
    }

    remove({x, y}) {
        this._positions = this._positions
            .filter(position => position.x !== x || position.y !== y);
    }

    every(callback) {
      return this._positions.every(callback);
    }

    forEach(callback) {
      this._positions.forEach(callback);
    }

}

class Front {

  constructor() {
    this._front = [];
  }

  push(element) {
    if (this.length == 3)
      this._front.shift();
    this._front.push(element);
  }

  pop() {
    return this._front.pop();
  }

  get length() {
    return this._front.length;
  }

}
