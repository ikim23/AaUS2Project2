export const INIT_BOARD = 'INIT_BOARD';
export const LOAD_LEVELS = 'LOAD_LEVELS';
export const MOVE = 'MOVE';
export const UNDO = 'UNDO';

export function initBoard(board) {
  return {
    type: INIT_BOARD,
    payload: board,
  };
}

export function loadLevels() {
  return {
    type: LOAD_LEVELS,
  };
}

export function move(vector) {
  return {
    type: MOVE,
    payload: vector,
  };
}

export function undo() {
  return {
    type: UNDO,
  };
}
