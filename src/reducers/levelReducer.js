import { LOAD_LEVELS } from '../actions/sokobanActions';
import levels from '../logic/levels.json';

const initState = {
  levels: [],
};

export default function levelReducer(state = initState, action) {
  switch (action.type) {
      case LOAD_LEVELS:
      return {
        ...state,
        levels,
      };
  }
  return state;
};
