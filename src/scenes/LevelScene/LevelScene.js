import React, { Component } from 'react';
import { View } from 'react-native';
import { connect } from 'react-redux';
import { initBoard, loadLevels } from '../../actions/sokobanActions';
import { route as boardRoute } from '../BoardScene';
import { ToolbarSimple } from '../../components/Toolbar';
import { LevelList } from '../../components/LevelList';

class LevelScene extends Component {

  componentDidMount() {
    this.props.dispatch(loadLevels());
  }

  _loadLevel = (board) => {
    this.props.dispatch(initBoard(board));
    this.props.navigator.push(boardRoute);
  }

  render() {
    return (
      <View style={{flex: 1}}>
        <ToolbarSimple/>
        <LevelList levels={this.props.levels} onItemPress={this._loadLevel}/>
      </View>
    );
  }

}

const mapStateToProps = (state) => {
  return {
    levels: state.data.levels,
  };
}

export const route = {
  id: 'LevelScene',
  component: connect(mapStateToProps)(LevelScene),
};
