import { combineReducers } from 'redux';
import sokoban from './sokobanReducer';
import data from './levelReducer';

export default combineReducers({
  sokoban,
  data,
});
