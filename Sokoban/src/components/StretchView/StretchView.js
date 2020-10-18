import React, { Component } from 'react';
import { View, Dimensions } from 'react-native';

class StretchView extends Component {

  constructor() {
    super();
    this.state = {
      boardStyle: {}
    };
  }

  _calcDimens = (event) => {
      const { cols, rows } = this.props;
      const { width, height } = event.nativeEvent.layout;
      const boxSize = Math.min(width / cols, height / rows);
      this.setState({
        boardStyle: {
          width: boxSize * cols,
          height: boxSize * rows,
        }
      });
  }

  render() {
    return (
      <View style={{
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
      }} onLayout={this._calcDimens}>
        <View style={this.state.boardStyle}>
          {this.props.children}
        </View>
      </View>
    );
  }

}

export { StretchView };
