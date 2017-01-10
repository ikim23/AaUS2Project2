import React, { Component } from 'react';
import { View } from 'react-native';
import { connect } from 'react-redux';
import { move, undo } from '../../actions/sokobanActions';
import { Toolbar } from '../../components/Toolbar';
import { Board } from '../../components/Board';
import { Navigation } from '../../components/Navigation';

class BoardScene extends Component {

  _onBack = () => {
    this.props.navigator.pop();
  }

  _onUndo = () => {
    this.props.dispatch(undo());
  }

  _onMove = (vector) => {
    this.props.dispatch(move(vector));
  }

  render() {
    return (
      <View style={{flex: 1}}>
        <Toolbar onBack={this._onBack} onUndo={this._onUndo}/>
        <Board board={this.props.board}/>
        <Navigation onMove={this._onMove}/>
      </View>
    );
  }

}

const mapStateToProps = (state) => {
  return {
    board: state.sokoban.board,
  };
}

export const route = {
  id: 'BoardScene',
  component: connect(mapStateToProps)(BoardScene),
};
