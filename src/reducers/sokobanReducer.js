import { INIT_BOARD, MOVE, UNDO } from '../actions/sokobanActions';
import Sokoban from '../logic/sokoban';

let sokoban = null;
const initState = {
  board: [],
  move: 0,
};

export default function sokobanReducer(state = initState, action) {
  let board;
  switch (action.type) {
    case INIT_BOARD:
      board = action.payload;
      sokoban = new Sokoban(board);
      return {
        ...state,
        board,
        move: 0,
      }
    case MOVE:
      board = sokoban.move(action.payload);
      if (!board) break;
      return {
        ...state,
        board,
        move: state.move + 1,
      };
    case UNDO:
      board = sokoban.undo();
      if (!board) break;
      return {
        ...state,
        board,
        move: state.move - 1,
      };
  }
  return state;
};
